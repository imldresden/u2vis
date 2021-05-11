using System;
using System.Collections.Generic;
using UnityEngine;

namespace u2vis
{
    public class PieChart2D : BaseVisualizationView
    {
        [SerializeField]
        private int _segments=360;
        [SerializeField]
        private int _valueIndex = 0;

        public int ValueIndex
        {
            get { return _valueIndex; }
            set
            {
                _valueIndex = value;
                Rebuild();
            }
        }

        #region Constructors
        protected PieChart2D() : base()
        {
        }
        #endregion

        #region Protected Methods
        protected override void RebuildVisualization()
        {
            if (_presenter == null || _presenter.NumberOfDimensions < 1)
            {
                Debug.LogError("Presenter is either null or has not enough dimensions to represent this visualization");
                return;
            }
            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var colors = new List<Color>();
            var indices = new List<int>();

            int offset = (int)_presenter.SelectedMinItem;
            int length = (int)_presenter.SelectedMaxItem - offset;
            float maxValue = VisViewHelper.GetGlobalMaximum(_presenter);
            float radius = 0.5f * _size.x;
            float divisor = 0;

            //TODO find better way to determine divisor (what 100% is)
            for (int dimIndex = 0; dimIndex < _presenter.NumberOfDimensions; dimIndex++)
            {
                divisor += VisViewHelper.GetItemValueAbsolute(_presenter, dimIndex, _valueIndex + offset) / maxValue;
            }
            MultiDimDataPresenter presenter = (MultiDimDataPresenter)_presenter;
            
            float startAngle = 0;
            for (int dimIndex = 0; dimIndex < _presenter.NumberOfDimensions; dimIndex++)
            {
                vertices.Add(Vector3.zero);
                normals.Add(-Vector3.forward);
                float sumOfDim = VisViewHelper.GetItemValueAbsolute(_presenter, dimIndex, _valueIndex + offset) / maxValue;
                float part = sumOfDim / divisor;
                colors.Add(_style.GetColorCategorical(dimIndex, part));
                float angle = part * Mathf.PI *2;
                float endAngle = startAngle + angle;
                List<Vector3> partMesh = Circles.CreatePartMesh(startAngle, endAngle,radius, _segments);
                for (int vertex =1; vertex < partMesh.Count; vertex++)
                {
                    colors.Add(_style.GetColorCategorical(dimIndex, part));
                    normals.Add(-Vector3.forward);
                    indices.Add(vertices.Count - 1);
                    indices.Add(vertices.Count + vertex);
                    indices.Add(vertices.Count - 1 + vertex);
                    
                }
                colors.Add(_style.GetColorCategorical(dimIndex, part));
                normals.Add(-Vector3.forward);
                startAngle = endAngle;
                vertices.AddRange(partMesh);
            }
            var mesh = new Mesh();
            mesh.name = "PieChart2DMesh";
            mesh.vertices = vertices.ToArray();
            mesh.normals = normals.ToArray();
            mesh.colors = colors.ToArray();
            mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);

            var meshFilter = GetComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;
        }
        #endregion

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
        }
    }
}
