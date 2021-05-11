using System.Collections.Generic;
using UnityEngine;

namespace u2vis
{
    /// <summary>
    /// Class which represents a rectangular grid which can be used for visualizations.
    /// </summary>
    public class RectGrid : MonoBehaviour
    {
        #region Private Fields
        /// <summary>
        /// The size of the grid.
        /// </summary>
        [SerializeField]
        private Vector2 _size = Vector2.one;
        /// <summary>
        /// The spacing between horizontal and vertical grid lines.
        /// </summary>
        [SerializeField]
        private Vector2 _spacing = Vector2.one;
        /// <summary>
        /// Indicates if the grid geometry needs rebuilding.
        /// </summary>
        private bool _needsRebuild = true;
        #endregion

        #region Public Properties
        /// <summary>
        /// The size of the grid.
        /// </summary>
        public Vector2 Size
        {
            get { return _size; }
            set
            {
                _size = value;
                _needsRebuild = true;
            }
        }
        /// <summary>
        /// The spacing between horizontal and vertical grid lines.
        /// </summary>
        public Vector2 Spacing
        {
            get { return _spacing; }
            set
            {
                _spacing = value;
                _needsRebuild = true;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Called by Unity once at the start of this script.
        /// </summary>
        private void Start()
        {
        }
        /// <summary>
        /// Called by Unity every frame.
        /// </summary>
        private void Update()
        {
            if (_needsRebuild)
                RebuildGridGeometry();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Rebuild the geometry of this grid.
        /// </summary>
        public void RebuildGridGeometry()
        {
            _needsRebuild = false;

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
        #endregion
    }
}
