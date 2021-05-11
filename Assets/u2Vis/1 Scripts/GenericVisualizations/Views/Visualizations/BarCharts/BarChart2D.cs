using UnityEngine;
using u2vis.Utilities;

namespace u2vis
{
    public class BarChart2D : BaseVisualizationView
    {
        #region Private Fields
        [SerializeField]
        protected Mesh _dataItemMesh = null;
        [SerializeField]
        protected float _barThickness = 0.9f;
        #endregion

        #region Public Properties
        public Mesh DataItemMesh
        {
            get { return _dataItemMesh; }
            set
            {
                _dataItemMesh = value;
                Rebuild();
            }
        }

        public float BarThickness
        {
            get { return _barThickness; }
            set
            {
                _barThickness = value;
                Rebuild();
            }
        }
        #endregion

        #region Constructors
        protected BarChart2D()
        {
        }
        #endregion

        #region Protected Methods
        protected override void RebuildVisualization()
        {
            if (_presenter == null || _presenter.NumberOfDimensions < 2)
            {
                Debug.LogError("Presenter is either null or has not enough dimensions to represent this visualization");
                return;
            }
            if (_dataItemMesh == null)
            {
                Debug.LogError("No DataMesh was set for this visualization.");
                return;
            }
            var iMesh = new IntermediateMesh();
            // temporary save the mesh data from the template for faster access
            var tVertices = _dataItemMesh.vertices;
            var tNromals = _dataItemMesh.normals;
            var tUVs = _dataItemMesh.uv;
            var tIndices = _dataItemMesh.triangles;

            int offset = _presenter.SelectedMinItem;
            int length = _presenter.SelectedItemsCount;
            float posXStep = _size.x / length;
            float posXOffset = posXStep * 0.5f;
            float uStep = 1.0f / length;
            float uOffset = uStep * 0.5f;
            for (int i = 0; i < length; i++)
            {
                int itemIndex = i + offset;
                var value = VisViewHelper.GetItemValueAbsolute(_presenter, 1, itemIndex, true);
                var pos = new Vector3(posXOffset + i * posXStep, 0, 0);
                var scale = new Vector3(posXStep * _barThickness, value * _size.y, _size.z);
                var startIndex = iMesh.Vertices.Count;
                var color = _style.GetColorContinous(_presenter.IsItemHighlighted(itemIndex), uOffset + uStep * i);
                foreach (var v in tVertices)
                {
                    iMesh.Vertices.Add(new Vector3(pos.x + v.x * scale.x, pos.y + v.y * scale.y, pos.z + v.z * scale.z));
                    iMesh.Colors.Add(color);
                }
                iMesh.Normals.AddRange(tNromals);
                iMesh.TexCoords.AddRange(tUVs);
                foreach (var j in tIndices)
                    iMesh.Indices.Add(startIndex + j);
            }
    
            var meshFilter = GetComponent<MeshFilter>();
            meshFilter.sharedMesh = iMesh.GenerateMesh("BarChart2DMesh", MeshTopology.Triangles);
        }
        #endregion

        #region Public Methods
        public virtual void Initialize(GenericDataPresenter presenter, GenericAxisView axisViewPrefab = null, GenericVisualizationStyle style = null, Mesh dataItemMesh = null)
        {
            _dataItemMesh = dataItemMesh;
            base.Initialize(presenter, axisViewPrefab, style);
        }

        public override bool TryGetPosForItemIndex(int itemIndex, out Vector3 pos)
        {
            pos = Vector3.zero;
            if (itemIndex < _presenter.SelectedMinItem || itemIndex >= _presenter.SelectedMaxItem)
                return false;
            var value = VisViewHelper.GetItemValueAbsolute(_presenter, 1, itemIndex, true);
            var i = itemIndex - _presenter.SelectedMinItem;
            int length = _presenter.SelectedItemsCount;
            float posXStep = _size.x / length;
            float posXOffset = posXStep * 0.5f;
            pos = new Vector3(posXOffset + i * posXStep, value * _size.y * 0.5f, 0);
            return true;
        }
        #endregion
    }
}
