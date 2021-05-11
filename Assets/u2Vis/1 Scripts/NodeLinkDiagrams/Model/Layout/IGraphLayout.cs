namespace u2vis.NodeLink
{
    public interface IGraphLayout
    {
        void LayoutGraph(IGraph<INode, IEdge<INode>> graph);
    }
}