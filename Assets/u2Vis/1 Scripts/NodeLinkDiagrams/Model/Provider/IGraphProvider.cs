namespace u2vis.NodeLink
{
    public interface IGraphProvider<Node, Edge, Graph>
        where Node : class, INode
        where Edge : class, IEdge<Node>
        where Graph : class, IGraph<Node, Edge>
    {
        Graph GetGraph();
    }
}
