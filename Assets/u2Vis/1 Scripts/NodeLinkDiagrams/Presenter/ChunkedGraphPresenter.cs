using System;
using System.Collections.Generic;
using UnityEngine;

namespace u2vis.NodeLink
{
    public delegate void NodeInteractionHandler(object sender, BaseNodePresenter node, int button);
    public class ChunkedGraphPresenter : MonoBehaviour
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
        private Vector3Int _chunkDimensions = Vector3Int.one;

        private IGraph<INode, IEdge<INode>> _graphModel = null;
        private GraphChunkContainer _chunks = null;
        private List<BaseNodePresenter> _nodePresenters = null;
        private List<BaseEdgePresenter> _edgePresenters = null;
        

        public event NodeInteractionHandler NodeMouseDown;
        public event NodeInteractionHandler NodeMouseMove;
        public event NodeInteractionHandler NodeMouseUp;

        public Vector3Int GetChunkDimensions()
        {
            return _chunkDimensions;
        }

        public ChunkedGraphPresenter()
        {
            _nodePresenters = new List<BaseNodePresenter>();
            _edgePresenters = new List<BaseEdgePresenter>();
        }

        public GraphChunkContainer getChunks()
        {
            return _chunks;
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

        private void Start()
        {
            _graphModel = _graphProvider.GetGraph();
            _chunks = new GraphChunkContainer(_chunkDimensions, this);
            _chunks.CalcPositionRangeFromNodes(_graphModel.Nodes);

            var nps = new Dictionary<INode, BaseNodePresenter>();
            foreach (var node in _graphModel.Nodes)
            {
                var np = new BaseNodePresenter(node);
                np.Size *= _nodeTemplate.Scale;
                _nodePresenters.Add(np);
                nps.Add(node, np);
                _chunks.GetChunkFromPosition(np.Position).NodePresenters.Add(np);
            }
            foreach (var edge in _graphModel.Edges)
            {
                var source = nps[edge.Source];
                var target = nps[edge.Target];
                var ep = new BaseEdgePresenter(edge, source, target);
                ep.Width *= _edgeTemplate.Scale;
                _edgePresenters.Add(ep);
                _chunks.GetChunkFromPosition(ep.Source.Position).EdgePresenters.Add(ep);
                source.targets.Add(ep);
                target.sources.Add(ep);
            }

            _chunks.InitGraphViews(_graphViewPrefab, transform, _nodeTemplate, _edgeTemplate);
        }

        public void Node_MouseDown(BaseNodePresenter node, int button)
        {
            if (NodeMouseDown != null)
                NodeMouseDown(this, node, button);
        }

        public void Node_MouseMove(BaseNodePresenter node, int button)
        {
            if (NodeMouseMove != null)
                NodeMouseMove(this, node, button);
        }

        private void Node_MouseUp(BaseNodePresenter node, int button)
        {
            if (NodeMouseUp != null)
                NodeMouseUp(this, node, button);
        }

        public class GraphChunk
        {
            public List<BaseNodePresenter> NodePresenters { get; }
            public List<BaseEdgePresenter> EdgePresenters { get; }
            public BaseGraphView GraphView { get; private set; }
            public Vector3Int Index { get; }
            public ChunkedGraphPresenter parentChunkedGraphPresenter { get; set; }

            public GraphChunk(int x, int y, int z, ChunkedGraphPresenter chunkedGraphPresenter)
            {
                Index = new Vector3Int(x, y, z);
                NodePresenters = new List<BaseNodePresenter>();
                EdgePresenters = new List<BaseEdgePresenter>();
                parentChunkedGraphPresenter = chunkedGraphPresenter;
                GraphView = null;
            }

            public void InitGraphView(BaseGraphView graphViewPrefab, Transform parent, NodeTemplate nodeTemplate, EdgeTemplate edgeTemplate)
            {
                GraphView = Instantiate(graphViewPrefab, parent, false);
                GraphView.NodeTemplate = nodeTemplate;
                GraphView.EdgeTemplate = edgeTemplate;
                GraphView.MouseDown += GraphView_MouseDown;
                GraphView.MouseMove += GraphView_MouseMove;
                GraphView.MouseUp += GraphView_MouseUp;
                GraphView.SetGraph(NodePresenters, EdgePresenters);
            }

            public List<BaseNodePresenter> getNodesInRect(float x, float y, float width, float height)
            {

                
                
                var resultNodes = new List<BaseNodePresenter>();
                Vector3 midPos = new Vector3(x + width * 0.5f, y + height * 0.5f, 0);
                foreach (var node in NodePresenters)
                    if (node.Position.x > x && node.Position.x < x + width && node.Position.y > y && node.Position.y < y + height)
                        resultNodes.Add(node);
                return resultNodes;
            }

            #region MouseInteraction
            private BaseNodePresenter _interactedNode;
            private void GraphView_MouseDown(object sender, Vector3 position, int button)
            {

                

                foreach (var node in NodePresenters)
                {
                    var sqMag = (node.Position - position).sqrMagnitude;
                    var sqRad = node.Size * node.Size;
                    if (sqMag < sqRad)
                    {
                        Debug.Log("Node hit! " + node.Position);
                        _interactedNode = node;
                        parentChunkedGraphPresenter.Node_MouseDown(_interactedNode, button);
                        break;
                    }
                }
            }

            private void GraphView_MouseMove(object sender, Vector3 position, int button)
            {
                if (_interactedNode == null)
                    return;
                _interactedNode.Position = position;
                _interactedNode.FirstPosition = position;
                _interactedNode.selected = true;
                GraphView.UpdateNodes(UpdateNodeInfo<BaseNodePresenter>.UpdateNodes(_interactedNode));
                parentChunkedGraphPresenter.Node_MouseMove(_interactedNode, button);
            }

            private void GraphView_MouseUp(object sender, Vector3 position, int button)
            {
                if (_interactedNode == null)
                    return;
                _interactedNode.Position = position;

                _interactedNode.FirstPosition = position;
                _interactedNode.selected = false;
                GraphView.UpdateNodes(UpdateNodeInfo<BaseNodePresenter>.UpdateNodes(_interactedNode));
                parentChunkedGraphPresenter.Node_MouseUp(_interactedNode, button);
                _interactedNode = null;
            }
            #endregion
        }

        public class GraphChunkContainer
        {
            private Vector3 _min = Vector3.zero;
            private Vector3 _max = Vector3.zero;
            private Vector3 _chunkRange = Vector3.zero;
            private Vector3Int _chunkDimensions;
            private GraphChunk[,,] _chunks;
            public ChunkedGraphPresenter parentChunkedGraphPresenter { get; set; }

            public GraphChunk[,,] Chunks { get { return _chunks; } }

            public GraphChunkContainer(Vector3Int chunkDimensions, ChunkedGraphPresenter chunkedGraphPresenter)
            {
                parentChunkedGraphPresenter = chunkedGraphPresenter;
                _chunkDimensions = new Vector3Int(
                    Mathf.Max(chunkDimensions.x, 1),
                    Mathf.Max(chunkDimensions.y, 1),
                    Mathf.Max(chunkDimensions.z, 1)
                    );
                _chunks = new GraphChunk[_chunkDimensions.x, _chunkDimensions.y, _chunkDimensions.z];
                for (int x = 0; x < _chunkDimensions.x; x++)
                    for (int y = 0; y < _chunkDimensions.y; y++)
                        for (int z = 0; z < _chunkDimensions.z; z++)
                            _chunks[x, y, z] = new GraphChunk(x, y, z, parentChunkedGraphPresenter);
            }

            public void CalcPositionRangeFromNodes(IReadOnlyCollection<INode> nodes)
            {
                foreach (var node in nodes)
                {
                    _min.x = Mathf.Min(_min.x, node.PosX);
                    _min.y = Mathf.Min(_min.y, node.PosY);
                    _min.z = Mathf.Min(_min.z, node.PosZ);
                    _max.x = Mathf.Max(_max.x, node.PosX);
                    _max.y = Mathf.Max(_max.y, node.PosY);
                    _max.z = Mathf.Max(_max.z, node.PosZ);
                }
                // prevent division by zero by having epsilon as the minimum amount for the range
                _chunkRange = new Vector3(
                    Mathf.Max(Mathf.Abs((_max.x - _min.x) / _chunkDimensions.x), Single.Epsilon),
                    Mathf.Max(Mathf.Abs( (_max.y - _min.y) / _chunkDimensions.y), Single.Epsilon),
                    Mathf.Max(Mathf.Abs((_max.z - _min.z) / _chunkDimensions.z), Single.Epsilon)
                    );
            }

            public void InitGraphViews(BaseGraphView graphViewPrefab, Transform parent, NodeTemplate nodeTemplate, EdgeTemplate edgeTemplate)
            {
                foreach (var chunk in _chunks)
                {
                    chunk.InitGraphView(graphViewPrefab, parent, nodeTemplate, edgeTemplate);
                    var center = _min + _chunkRange * 0.5f +
                        new Vector3(
                            _chunkRange.x * chunk.Index.x,
                            _chunkRange.y * chunk.Index.y,
                            _chunkRange.z * chunk.Index.z
                        );
                    chunk.GraphView.SetBounds(center, _chunkRange);
                    chunk.GraphView.name = "GraphView " + chunk.Index;
                }
            }

            public GraphChunk GetChunkFromPosition(Vector3 pos)
            {
                // Find an elegant solution for out of bounds errors
                pos = pos - _min;
                int x = Mathf.Clamp((int)(pos.x / _chunkRange.x), 0, _chunkDimensions.x - 1);
                int y = Mathf.Clamp((int)(pos.y / _chunkRange.y), 0, _chunkDimensions.y - 1);
                int z = Mathf.Clamp((int)(pos.z / _chunkRange.z), 0, _chunkDimensions.z - 1);
                return _chunks[x, y, z];
            }
        }
    }
}
