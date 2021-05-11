using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace u2vis.NodeLink
{
    public class ForceDirectedGraphPresenter : MonoBehaviour
    {
        [SerializeField]
        private BaseGraphProviderComponent _graphProvider = null;
        [SerializeField]
        private BaseGraphView _graphViewPrefab = null;
        [SerializeField]
        private NodeTemplate _nodeTemplate = null;
        [SerializeField]
        private EdgeTemplate _edgeTemplate = null;
        [SerializeField]
        [Tooltip("Describes how Stiff connections are. Higher value => more movement of not draged nodes")]
        private float _stiffness = 81.76f;
        [SerializeField]
        [Tooltip("Describes how much nodes repulse each other. Higher value => nodes stay more apart")]
        private float _repulsion = 4000.00f;
        [SerializeField]
        [Tooltip("Describes friction. 1 => no friction. 0 => 100% friction. Higher value => longer movement. Values have to lie between 0 and 1")]
        private float _damping = 0.5f;

        private BaseGraphView _graphView = null;
        private IGraph<INode, IEdge<INode>> _graphModel = null;
        private List<BaseNodePresenter> _nodePresenters = null;
        private List<BaseEdgePresenter> _edgePresenters = null;
        private ForceDirected2D _forceDirected2D;
        private float timer = 0;
        public ReadOnlyCollection<BaseNodePresenter> NodePresenters { get; }
        public ReadOnlyCollection<BaseEdgePresenter> EdgePresenters { get; }

        public ForceDirectedGraphPresenter()
        {
            _nodePresenters = new List<BaseNodePresenter>();
            _edgePresenters = new List<BaseEdgePresenter>();
            NodePresenters = new ReadOnlyCollection<BaseNodePresenter>(_nodePresenters);
            EdgePresenters = new ReadOnlyCollection<BaseEdgePresenter>(_edgePresenters);
        }

        public void Update()
        {
            timer += Time.deltaTime;
            CalculateForceAndUpdate(Time.deltaTime);
        }

        //calls the force directed graph object, to calculate the movement, then applies it
        private void CalculateForceAndUpdate(float time)
        {
            _forceDirected2D.Calculate(time);
            //get new positions from model and apply to view
            Dictionary<int,Vector3> positionDict = _forceDirected2D.ApplyCalculation();
            foreach (var pos in positionDict)
            {
                BaseNodePresenter node = IDToNode(pos.Key);
               // if (!(node ==_interactedNode))
                    node.Position = pos.Value * 0.01f;
            }
            _graphView.UpdateNodes(UpdateNodeInfo<BaseNodePresenter>.UpdateNodes(_nodePresenters.ToArray()));
        }

        private void Start()
        {
           //setup graph start 
            _graphModel = _graphProvider.GetGraph();
            _forceDirected2D = new ForceDirected2D(_graphModel, _stiffness, _repulsion, _damping);
            var nps = new Dictionary<INode, BaseNodePresenter>();
            foreach (var node in _graphModel.Nodes)
            {
                var np = new BaseNodePresenter(node);
                np.Size *= _nodeTemplate.Scale;
                _nodePresenters.Add(np);
                nps.Add(node, np);
            }
            foreach (var edge in _graphModel.Edges)
            {
                var source = nps[edge.Source];
                var target = nps[edge.Target];
                var ep = new BaseEdgePresenter(edge, source, target);
                ep.Width *= _edgeTemplate.Scale;
                _edgePresenters.Add(ep);
            }
            _graphView = Instantiate(_graphViewPrefab, transform, false);
            _graphView.MouseDown += GraphView_MouseDown;
            _graphView.MouseMove += GraphView_MouseMove;
            _graphView.MouseUp += GraphView_MouseUp;
            _graphView.NodeTemplate = _nodeTemplate;
            _graphView.SetGraph(NodePresenters, EdgePresenters);
        }

        public BaseNodePresenter IDToNode(int nodeID)
        {
            foreach (var node in _nodePresenters)
            {
                if (node.Uid == nodeID)
                {
                    return node;
                }
            }
            return null;
        }

        private BaseNodePresenter _interactedNode;
        public event NodeInteractionHandler NodeMouseDown;
        public event NodeInteractionHandler NodeMouseMove;
        public event NodeInteractionHandler NodeMouseUp;
        private void GraphView_MouseDown(object sender, Vector3 position, int button)
        {
            //find interacted node
            foreach (var node in _nodePresenters)
            {
                var sqMag = (this.transform.TransformPoint(node.Position) - position).sqrMagnitude;
                var sqRad = node.Size * node.Size;
                if (sqMag < sqRad)
                {
                    Debug.Log("Node hit! " + this.transform.TransformPoint(node.Position) + " Button used: " + button);
                    _interactedNode = node;
                    Node_MouseDown(_interactedNode, button);
                    //Pin interacted node, so it gets no force calculations 
                    _forceDirected2D.PinNode(_interactedNode._nodeModel, true);

                    break;
                }
            }
        }

        public void Node_MouseDown(BaseNodePresenter node, int button)
        {
            if (NodeMouseDown != null)
                NodeMouseDown(this, node, button);
        }

        private void GraphView_MouseMove(object sender, Vector3 position, int button)
        {
            if (_interactedNode == null)
                return;
            _interactedNode.Position = this.transform.InverseTransformPoint(position);
            //update positions in graph model
            _forceDirected2D.UpdatePositionInteracted(_interactedNode._nodeModel, position /0.01f);
            _graphView.UpdateNodes(UpdateNodeInfo<BaseNodePresenter>.UpdateNodes(_interactedNode));
            Node_MouseMove(_interactedNode, button);
        }
        public void Node_MouseMove(BaseNodePresenter node, int button)
        {
            if (NodeMouseMove != null)
                NodeMouseMove(this, node, button);
        }
        private void GraphView_MouseUp(object sender, Vector3 position, int button)
        {
            if (_interactedNode == null)
                return;
            //  _interactedNode.Position = this.transform.InverseTransformPoint(position);
            //_graphView.UpdateNodes(UpdateNodeInfo<BaseNodePresenter>.UpdateNodes(_interactedNode));

            Node_MouseUp(_interactedNode, button);
          
            //unpin node
            _forceDirected2D.PinNode(_interactedNode._nodeModel, false);

            _interactedNode = null;

        }

        private void Node_MouseUp(BaseNodePresenter node, int button)
        {
            if (NodeMouseUp != null)
                NodeMouseUp(this, node, button);
        }
    }
}
