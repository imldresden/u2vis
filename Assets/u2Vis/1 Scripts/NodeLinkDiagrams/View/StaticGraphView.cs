using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace u2vis.NodeLink
{
    public class StaticGraphView : BaseGraphView
    {
        [SerializeField]
        private MeshFilter _nodesMeshFilter = null;
        [SerializeField]
        private MeshFilter _edgesMeshFilter = null;
        [SerializeField]
        private Canvas _nodeLabelCanvas = null;
        [SerializeField]
        private UnityEngine.UI.Text _nodeLabelPrefab = null;

        private List<BaseNodePresenter> _nodes = null;
        private List<BaseEdgePresenter> _edges = null;
        private Dictionary<int, UnityEngine.UI.Text> _nodeLabels = null;

        protected override void SetNodeTemplate(NodeTemplate nodeTemplate)
        {
            base.SetNodeTemplate(nodeTemplate);
            // TODO: set Material
        }

        public override void SetBounds(Vector3 center, Vector3 size)
        {
            var collider = GetComponent<BoxCollider>();
            if (collider == null)
                return;
            collider.center = center;
            collider.size = size;
        }

        #region Graph Methods
        public void RebuildNodes()
        {
            var vertices = new List<Vector3>();
            var texCoords = new List<Vector2>();
            var colors = new List<Color>();
            var indices = new List<int>();

            var tVertices = NodeTemplate.Mesh.vertices;
            var tUVs = NodeTemplate.Mesh.uv;
            var tIndices = NodeTemplate.Mesh.triangles;
            var tScale = NodeTemplate.Scale;
            foreach (var node in _nodes)
            {
                int start = vertices.Count;
                var c = new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f));
                foreach (var v in tVertices)
                {
                    vertices.Add(node.Position + v * node.Size);
                    colors.Add(c);
                }
                texCoords.AddRange(tUVs);
                foreach (var i in tIndices)
                    indices.Add(start + i);
            }

            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetColors(colors);
            mesh.SetUVs(0, texCoords);
            mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);

            _nodesMeshFilter.sharedMesh = mesh;
        }

        public void BuildNodeLabels()
        {
            for (int i = 0; i < _nodeLabelCanvas.transform.childCount; i++)
                Destroy(_nodeLabelCanvas.transform.GetChild(i));
            _nodeLabels = new Dictionary<int, UnityEngine.UI.Text>();
            foreach(var node in _nodes)
            {
                var text = GameObject.Instantiate(_nodeLabelPrefab, _nodeLabelCanvas.transform, false);
                text.transform.localPosition = node.Position + new Vector3(0,-0.1f,0);
                
                text.text = node.Label;
                _nodeLabels.Add(node.Uid, text);
            }
        }

        private void RebuildEdges()
        {
            float size = 0.005f;
            var vertices = new List<Vector3>();
            var texCoords = new List<Vector2>();
            var colors = new List<Color>();
            var indices = new List<int>();

            foreach (var edge in _edges)
            {
                size = edge.Width;
                int start = vertices.Count;
                Vector3 normal = (edge.Target.Position - edge.Source.Position).normalized;
                Vector3 ortho = Vector3.Cross(normal, Vector3.forward).normalized;

                // Vertices
                vertices.Add(edge.Target.Position + size * ortho);
                vertices.Add(edge.Source.Position + size * ortho);
                vertices.Add(edge.Target.Position - size * ortho);
                vertices.Add(edge.Source.Position - size * ortho);
                // Indices
                indices.Add(start + 0);
                indices.Add(start + 1);
                indices.Add(start + 2);

                indices.Add(start + 3);
                indices.Add(start + 2);
                indices.Add(start + 1);
                // colors
                Color c;
                if(edge.Source.selected || edge.Target.selected)
                {
                    c = new Color(1, 0, 0);
                } else
                {
                    c =  new Color(0, 0, 1);
                }
                colors.Add(c);
                colors.Add(c);
                colors.Add(c);
                colors.Add(c);
                // texture coordinates
                texCoords.Add(new Vector2(0, 0));
                texCoords.Add(new Vector2(0, 1));
                texCoords.Add(new Vector2(1, 0));
                texCoords.Add(new Vector2(1, 1));
            }

            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, texCoords);
            mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);

            mesh.SetColors(colors);
            _edgesMeshFilter.sharedMesh = mesh;
        }
        public override void UpdateNodes<NodePresenter>(UpdateNodeInfo<NodePresenter> updateNodeInfo)
        {
            foreach (var node in updateNodeInfo.Updated)
            {
                var text = _nodeLabels[node.Uid];

                text.transform.localPosition = node.Position + new Vector3(0, -0.1f, 0);
            }
            foreach (var node in updateNodeInfo.Removed)
            {
                _nodes.Remove(node);

                var text = _nodeLabels[node.Uid];
                Destroy(text.gameObject);
                _nodeLabels.Remove(node.Uid);
            }
            foreach (var node in updateNodeInfo.Added)
            {
                _nodes.Add(node);

                var text = GameObject.Instantiate(_nodeLabelPrefab, _nodeLabelCanvas.transform, false);
                text.transform.localPosition = node.Position + new Vector3(0, -0.1f, 0);
                text.text = node.Label;
                _nodeLabels.Add(node.Uid, text);
            }
            RebuildNodes();
            RebuildEdges();
        }

        public override void UpdateEdges<EdgePresenter>(UpdateEdgeInfo<EdgePresenter> updateEdgeInfo)
        {
            _edges.AddRange(updateEdgeInfo.Added);
            foreach (var edge in updateEdgeInfo.Removed)
                _edges.Remove(edge);

        }

        public override void UpdateGraph<NodePresenter, EdgePresenter>(UpdateNodeInfo<NodePresenter> updateNodeInfo, UpdateEdgeInfo<EdgePresenter> updateEdgeInfo)
        {
            UpdateNodes<NodePresenter>(updateNodeInfo);
            UpdateEdges<EdgePresenter>(updateEdgeInfo);
        }

        public override void SetGraph<NodePresenter, EdgePresenter>(IEnumerable<NodePresenter> nodePresenters, IEnumerable<EdgePresenter> edgePresenters)
        {
            _nodes = new List<BaseNodePresenter>(nodePresenters);
            _edges = new List<BaseEdgePresenter>(edgePresenters);

            RebuildNodes();
            RebuildEdges();
            BuildNodeLabels();
        }
        #endregion
    }
}
