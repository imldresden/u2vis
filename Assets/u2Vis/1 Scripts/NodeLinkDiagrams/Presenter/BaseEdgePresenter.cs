using UnityEngine;

namespace u2vis.NodeLink
{
    public class BaseEdgePresenter
    {
        private IEdge<INode> _edgeModel;
        public int Uid { get { return _edgeModel.Uid; } }
        public BaseNodePresenter Source { get; set; }
        public BaseNodePresenter Target { get; set;  }

        public float Width { get; set; }

        public float Attribute { get; set; }

        public Color color { get; set; }

        public BaseEdgePresenter(IEdge<INode> edge, BaseNodePresenter source, BaseNodePresenter target)
        {
            _edgeModel = edge;
            Source = source;
            Target = target;
            Width = 1;
            color = new Color(0, 0, 1, 1);
            Attribute = Random.Range(0.0f, 1.5f);
        }


        public override int GetHashCode() { return Source.copyID + Target.copyID * 2000; }
        public override bool Equals(object obj) {
            BaseEdgePresenter objE = (BaseEdgePresenter)obj;
            return objE.Target.copyID == this.Target.copyID && objE.Source.copyID == this.Source.copyID;
        }
    }
}
