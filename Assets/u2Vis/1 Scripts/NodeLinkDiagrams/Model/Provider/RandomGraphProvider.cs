using System.Collections.Generic;
using UnityEngine;

namespace u2vis.NodeLink
{
    public class RandomGraphProvider : BaseGraphProviderComponent
    {
        [SerializeField]
        private int _numberOfNodes = 100;
        [SerializeField]
        private int _numberOfEdges = 500;
        [SerializeField]
        private Vector3 _minBounds = Vector3.zero;
        [SerializeField]
        private Vector3 _maxBounds = Vector3.one;

        private List<Node> CreateNodes<Node>() where Node : class, INode
        {
            UnityEngine.Random.InitState(0);
            var nodes = new List<Node>();
            for (int i = 0; i < _numberOfNodes; i++)
            {
                var node = new SimpleNode(
                    IdGenerator.GenId(),
                    UnityEngine.Random.Range(_minBounds.x, _maxBounds.x),
                    UnityEngine.Random.Range(_minBounds.y, _maxBounds.y),
                    UnityEngine.Random.Range(_minBounds.z, _maxBounds.z),
                    i.ToString()
                    );
                nodes.Add(node as Node);
            }
            return nodes;
        }

        private List<Edge> CreateEdges<Node, Edge>(List<Node> nodes) where Node : class, INode where Edge : class, IEdge<Node>
        {
            UnityEngine.Random.InitState(10000);
            var edges = new List<Edge>();
            for (int i = 0; i < _numberOfEdges; i++)
            {
                Node source = nodes[UnityEngine.Random.Range(0, _numberOfNodes)];
                // find a random target node that is not the source node
                Node target = null;
                do { target = nodes[UnityEngine.Random.Range(0, _numberOfNodes)]; } while (target == source);
                // check if there is already an identical edge
                bool alreadExists = false;
                foreach (var edge in edges)
                    if (edge.Source == source && edge.Target == target)
                    {
                        alreadExists = true;
                        break;
                    }
                // if the edge already exists, continue with the next one
                // this prevents deadlocks, but the graph may have fewer than
                // the specified number of edges
                if (alreadExists)
                    continue;
                edges.Add((new SimpleEdge<Node>(IdGenerator.GenId(), source, target)) as Edge);
            }
            return edges;
        }

        public SimpleGraph<SimpleNode, SimpleEdge<SimpleNode>> GetSimpleGraph()
        {
            var nodes = CreateNodes<SimpleNode>();
            var edges = CreateEdges<SimpleNode, SimpleEdge<SimpleNode>>(nodes);
            return new SimpleGraph<SimpleNode, SimpleEdge<SimpleNode>>(nodes, edges);
        }

        public override IGraph<INode, IEdge<INode>> GetGraph()
        {
            var nodes = CreateNodes<INode>();
            var edges = CreateEdges<INode, IEdge<INode>>(nodes);
            return new SimpleGraph<INode, IEdge<INode>>(nodes, edges);
        }
    }
}
