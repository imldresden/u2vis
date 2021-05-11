using UnityEngine;

namespace u2vis.NodeLink
{
    // Can't use generics because unity doesn't like them in their components...
    public abstract class BaseGraphProviderComponent : MonoBehaviour, IGraphProvider<INode, IEdge<INode>, IGraph<INode, IEdge<INode>>>
    {
        public abstract IGraph<INode, IEdge<INode>> GetGraph();
    }
}
