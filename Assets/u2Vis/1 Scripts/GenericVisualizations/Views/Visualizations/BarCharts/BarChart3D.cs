using System.Collections.Generic;
using UnityEngine;
using u2vis.Utilities;

namespace u2vis
{
    public class BarChart3D : BaseVisualizationView
    {
        #region Private Fields
        [SerializeField]
        private Mesh _dataItemMesh = null;
        [SerializeField]
        private Vector2 _barThickness = new Vector2(0.9f, 0.9f);
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

        public Vector2 BarThickness
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
        protected BarChart3D()
        {
        }
        #endregion

        #region Protected Methods
        protected override void SetupInitialAxisViews()
        {
            if (_presenter.AxisPresenters.Length < 3)
            {
                Debug.LogError("Data Presenter has fewer than three AxisPresenters");
                return;
            }
            base.SetupInitialAxisViews();
            var vZ = Instantiate(_axisViewPrefab, transform, false);
            vZ.AxisPresenter = _presenter.AxisPresenters[2];
            vZ.Length = _size.z;
            vZ.transform.localRotation = Quaternion.Euler(0, 90, 0);
            vZ.transform.localPosition = new Vector3(0, 0, _size.z);
            vZ.Mirrored = true;
            _axisViews.Add(vZ);
        }

        protected override void RebuildVisualization()
        {
            if (_presenter == null || _presenter.NumberOfDimensions < 3)
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
            float maxValue = VisViewHelper.GetGlobalMaximum(_presenter);
            float posXStep = _size.x / length;
            float posXOffset = posXStep * 0.5f;
            float posZStep = _size.z / _presenter.NumberOfDimensions;
            float posZOffset = posZStep * 0.5f;

            float uStep = 1.0f / length;
            float uOffset = uStep * 0.5f;
            float vStep = 1.0f / _presenter.NumberOfDimensions;
            float vOffset = vStep * 0.5f;
            //TODO: UV coords of mesh based on height to add grid lines
            for (int dimIndex = 0; dimIndex < _presenter.NumberOfDimensions; dimIndex++)
            {
                for (int itemIndex = 0; itemIndex < length; itemIndex++)
                {
                    float value = VisViewHelper.GetItemValueAbsolute(_presenter, dimIndex, itemIndex + offset) / maxValue;
                    var pos = new Vector3(posXOffset + posXStep * itemIndex, 0, posZOffset + posZStep * dimIndex);
                    var scale = new Vector3(posXStep * _barThickness.x, value * _size.y, posZStep * _barThickness.y);
                    var startIndex = iMesh.Vertices.Count;
                    foreach (var v in tVertices)
                    {
                        var vPos = new Vector3(pos.x + v.x * scale.x, pos.y + v.y * scale.y, pos.z + v.z * scale.z);
                        iMesh.Vertices.Add(vPos);
                        iMesh.Colors.Add(_style.GetColorContinous(_presenter.IsItemHighlighted(itemIndex), uOffset + uStep * itemIndex, vOffset + vStep * dimIndex, vPos.y));
                    }
                    iMesh.Normals.AddRange(tNromals);
                    iMesh.TexCoords.AddRange(tUVs);
                    foreach (var index in tIndices)
                        iMesh.Indices.Add(startIndex + index);
                }
            }

            var mesh = iMesh.GenerateMesh("BarChart3DMesh", MeshTopology.Triangles);
            var meshFilter = GetComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;
            // TODO Move this into separate class
            var meshCollider = GetComponent<MeshCollider>();
            if (meshCollider != null)
                meshCollider.sharedMesh = mesh;
        }

        protected override void RebuildAxes()
        {
            if (_axisViews == null || _fromEditor)
                SetupInitialAxisViews();
            AxisTick[] ticks;
            if (_presenter is MultiDimDataPresenter mdp)
                ticks = _presenter.AxisPresenters[0].GenerateFromDimension(mdp.CaptionDimension, _presenter.SelectedMinItem, _presenter.SelectedMaxItem);
            else
                ticks = _presenter.AxisPresenters[0].GenerateFromDiscreteRange(_presenter.SelectedMinItem, _presenter.SelectedMaxItem);
            _axisViews[0].RebuildAxis(ticks);
            ticks = _presenter.AxisPresenters[1].GenerateFromMinMaxValue(0.0f, VisViewHelper.GetGlobalMaximum(_presenter));
            _axisViews[1].RebuildAxis(ticks);
            ticks = _presenter.AxisPresenters[2].GenerateFromDimensionCaptions(_presenter);
            _axisViews[2].RebuildAxis(ticks);
        }
        #endregion

        #region Public Methods
        public virtual void Initialize(GenericDataPresenter presenter, GenericAxisView axisViewPrefab = null, GenericVisualizationStyle style = null, Mesh dataItemMesh = null)
        {
            _dataItemMesh = dataItemMesh;
            base.Initialize(presenter, axisViewPrefab, style);
        }
        #endregion
    }
}
