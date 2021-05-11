using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Lean.Touch;
using UnityEngine;

namespace u2vis.NodeLink
{
    public delegate void DefaultInteractionHanlder(object sender, UnityEngine.Vector3 positon, int button);
    public abstract class BaseGraphView : Input.UiElement
    {
        protected List<LeanFinger> _fingers = new List<LeanFinger>();
        protected virtual void Update()
        {
            
        }

        #region Node/Edge Templates
        private NodeTemplate _nodeTemplate = null;
        private EdgeTemplate _edgeTemplate = null;
        public NodeTemplate NodeTemplate
        {
            get { return _nodeTemplate; }
            set { SetNodeTemplate(value); }
        }
        public EdgeTemplate EdgeTemplate
        {
            get { return _edgeTemplate; }
            set { SetEdgeTemplate(value); }
        }
        protected virtual void SetNodeTemplate(NodeTemplate nodeTemplate)
        {
            _nodeTemplate = nodeTemplate;
        }

        protected virtual void SetEdgeTemplate(EdgeTemplate edgeTemplate)
        {
            _edgeTemplate = edgeTemplate;
        }
        #endregion

        #region Graph Methods
        public abstract void UpdateNodes<NodePresenter>(UpdateNodeInfo<NodePresenter> updateNodeInfo)
            where NodePresenter: BaseNodePresenter;
        public abstract void UpdateEdges<EdgePresenter>(UpdateEdgeInfo<EdgePresenter> updateEdgeInfo)
            where EdgePresenter: BaseEdgePresenter;
        public abstract void UpdateGraph<NodePresenter, EdgePresenter>(UpdateNodeInfo<NodePresenter> updateNodeInfo, UpdateEdgeInfo<EdgePresenter> updateEdgeInfo)
            where NodePresenter : BaseNodePresenter
            where EdgePresenter : BaseEdgePresenter;

        public abstract void SetGraph<NodePresenter, EdgePresenter>(IEnumerable<NodePresenter> nodePresenters, IEnumerable<EdgePresenter> edgePresenters)
            where NodePresenter : BaseNodePresenter
            where EdgePresenter : BaseEdgePresenter;
        #endregion

        public abstract void SetBounds(Vector3 center, Vector3 size);

        #region Mouse Interaction
        public event DefaultInteractionHanlder MouseDown;
        public event DefaultInteractionHanlder MouseMove;
        public event DefaultInteractionHanlder MouseUp;
        public override void OnFingerDown(LeanFinger finger, int order, RaycastHit hit)
        {
            _fingers.Add(finger);
            base.OnFingerDown(finger, order, hit);
            if (MouseDown != null && _fingers.Count==1)
                MouseDown(this, hit.point, 0);
        }

        public override void OnFingerMove(LeanFinger finger, int order, RaycastHit hit)
        {
            base.OnFingerMove(finger);
            if (MouseMove != null && _fingers.Count == 1)
                MouseMove(this, hit.point, 0);
        }

        public override void OnFingerUp(LeanFinger finger, int order, RaycastHit hit)
        {
            _fingers.Remove(finger);
            base.OnFingerUp(finger, order, hit);
            if (MouseUp != null && _fingers.Count == 0)
                MouseUp(this, hit.point, 0);
        }

        public override void OnMouseBtnDown(int button, int order, RaycastHit hit)
        {
            base.OnMouseBtnDown(button, order, hit);
            if (MouseDown != null)
                MouseDown(this, hit.point, button);
        }
        public override void OnMouseMove(int button, int order, RaycastHit hit)
        {
            base.OnMouseMove(button, order, hit);
            if (MouseMove != null)
                MouseMove(this, hit.point, button);
        }
        public override void OnMouseBtnUp(int button, int order, RaycastHit hit)
        {
            base.OnMouseBtnUp(button, order, hit);
            if (MouseUp != null)
                MouseUp(this, hit.point, button);
        }

        
        #endregion
    }

    public class UpdateNodeInfo<NodePresenter> where NodePresenter: BaseNodePresenter
    {
        public List<NodePresenter> Updated { get; }
        public List<NodePresenter> Added { get; }
        public List<NodePresenter> Removed { get; }

        public UpdateNodeInfo(List<NodePresenter> updated, List<NodePresenter> added = null, List<NodePresenter> removed = null)
        {
            Updated = updated;
            if (added == null)
                added = new List<NodePresenter>();
            Added = added;
            if (removed == null)
                removed = new List<NodePresenter>();
            Removed = removed;
        }

        public static UpdateNodeInfo<NodePresenter> UpdateNodes(params NodePresenter[] updatedNodes)
        {
            return new UpdateNodeInfo<NodePresenter>(updatedNodes.ToList(), null, null);
        }
    }

    public class UpdateEdgeInfo<EdgePresenter> where EdgePresenter : BaseEdgePresenter
    {
        public List<EdgePresenter> Updated { get; }
        public List<EdgePresenter> Added { get; }
        public List<EdgePresenter> Removed { get; }

        public UpdateEdgeInfo(List<EdgePresenter> updated, List<EdgePresenter> added, List<EdgePresenter> removed)
        {
            Updated = updated;
            if (added == null)
                added = new List<EdgePresenter>();
            Added = added;
            if (removed == null)
                removed = new List<EdgePresenter>();
            Removed = removed;
        }

        public static UpdateEdgeInfo<EdgePresenter> UpdateNodes(params EdgePresenter[] updatedEdges)
        {
            return new UpdateEdgeInfo<EdgePresenter>(updatedEdges.ToList(), null, null);
        }
    }
}
