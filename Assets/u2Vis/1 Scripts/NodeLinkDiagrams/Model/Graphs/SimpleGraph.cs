using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace u2vis.NodeLink
{
    public class SimpleGraph<Node, Edge> : IGraph<Node, Edge> where Node: class, INode where Edge: class, IEdge<Node>  
    {
        public List<Node> _nodes;
        public List<Edge> _edges;

        public ReadOnlyCollection<Node> Nodes { get; }
        public ReadOnlyCollection<Edge> Edges { get; }

        public SimpleGraph()
            : this(new List<Node>(), new List<Edge>())
        {
        }

        public SimpleGraph(List<Node> nodes, List<Edge> edges)
        {
            _nodes = nodes;
            _edges = edges;
            Nodes = _nodes.AsReadOnly();
            Edges = _edges.AsReadOnly();
        }

        

        public void AddNode(Node node)
        {
            if (_nodes.Contains(node))
                return;
            _nodes.Add(node);
        }

        public bool RemoveNode(Node node)
        {
            if (!_nodes.Remove(node))
                return false;
            for (int i = 0; i < _edges.Count; i++)
            {
                var edge = _edges[i];
                if (edge.Source != node && edge.Target != node)
                    continue;
                _edges.Remove(edge);
                i++;
            }
            return true;
        }

        public void AddEdge(Edge edge)
        {
            if (_edges.Contains(edge))
                return;
            _edges.Add(edge);
            AddNode(edge.Source);
            AddNode(edge.Target);
        }

        public bool RemoveEdge(Edge edge)
        {
            return _edges.Remove(edge);
        }

        public List<Edge> GetEdgesForNode(Node node)
        {
            var result = new List<Edge>();
            foreach (Edge edge in _edges)
                if (edge.Source == node || edge.Target == node)
                    result.Add(edge);
            return result;
        }
    }
}
