namespace UVis.NodeLink
{
    public interface IGraphLayout
    {
        void LayoutGraph(IGraph<INode, IEdge<INode>> graph);
    }
}