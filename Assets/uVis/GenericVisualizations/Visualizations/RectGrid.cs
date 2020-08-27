using System.Collections.Generic;
using UnityEngine;

namespace UVis
{
    public class RectGrid : MonoBehaviour
    {
        [SerializeField]
        private Vector2 _size = Vector2.one;
        [SerializeField]
        private Vector2 _spacing = Vector2.one;

        private void Start()
        {
            RebuildGridGeometry();
        }

        public void RebuildGridGeometry()
        {
            if (_spacing.x == 0)
                _spacing.x = 0.01f;
            if (_spacing.y == 0)
                _spacing.y = 0.01f;

            var vertices = new List<Vector3>();
            var indices = new List<int>();

            for (float x = 0; x <= _size.x; x += _spacing.x)
            {
                vertices.Add(new Vector3(x, 0, 0));
                vertices.Add(new Vector3(x, _size.y, 0));
                indices.Add(vertices.Count - 2);
                indices.Add(vertices.Count - 1);
            }
            for (float y = 0; y <= _size.y; y += _spacing.y)
            {
                vertices.Add(new Vector3(0, y, 0));
                vertices.Add(new Vector3(_size.x, y, 0));
                indices.Add(vertices.Count - 2);
                indices.Add(vertices.Count - 1);
            }

            var mesh = new Mesh();
            mesh.name = "gridMesh";
            mesh.vertices = vertices.ToArray();
            mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
            GetComponent<MeshFilter>().sharedMesh = mesh;
        }
    }
}
