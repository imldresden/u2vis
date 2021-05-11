using System.Collections.ObjectModel;

namespace u2vis.NodeLink
{
    public interface IGraph<Node, Edge>
        where Node : class, INode
        where Edge : class, IEdge<Node>
    {
        ReadOnlyCollection<Node> Nodes { get; }
        ReadOnlyCollection<Edge> Edges { get; }

        void AddNode(Node node);
        bool RemoveNode(Node node);
        void AddEdge(Edge edge);
        bool RemoveEdge(Edge edge);
    }
}
