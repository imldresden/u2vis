using System.Collections.Generic;

namespace u2vis.NodeLink
{
    public class SimpleEdge<Node> : IEdge<Node> where Node : class, INode
    {
        public int Uid { get; }
        public Node Source { get; }
        public Node Target { get; }
        public IDictionary<string, string> Values { get; }
        public float Thickness { get; set; }

        public SimpleEdge(int uid, Node source, Node target)
        {
            Uid = uid;
            Source = source;
            Target = target;
            Thickness = 1.0f;
        }

        public SimpleEdge(int uid, Node source, Node target, IDictionary<string, string> values)
        {
            Uid = uid;
            Source = source;
            Target = target;
            Values = values;
            Thickness = 1.0f;
        }
    }
}
