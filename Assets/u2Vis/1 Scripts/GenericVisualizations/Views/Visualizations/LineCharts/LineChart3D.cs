using System.Collections.Generic;
using UnityEngine;
using u2vis.Utilities;

namespace u2vis
{
    public class LineChart3D : BaseVisualizationView
    {
        #region Constructors
        protected LineChart3D() : base()
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
            if (_presenter == null || _presenter.NumberOfDimensions < 1)
            {
                Debug.LogError("Presenter is either null or has not enough dimensions to represent this visualization");
                return;
            }
            var iMesh = new IntermediateMesh();
            int offset = (int)_presenter.SelectedMinItem;
            int length = (int)_presenter.SelectedMaxItem - offset;
            float maxValue = VisViewHelper.GetGlobalMaximum(_presenter);
            float step = 1.0f / _presenter.NumberOfDimensions;
            for (int dimIndex = 0; dimIndex < _presenter.NumberOfDimensions; dimIndex++)
            {
                for (int itemIndex = 0; itemIndex < length; itemIndex++)
                {
                    float valueX = (float)itemIndex / length;
                    float valueY = VisViewHelper.GetItemValueAbsolute(_presenter, dimIndex, itemIndex + offset) / maxValue;
                    float valueZ = dimIndex * step + (0.5f * step);
                    iMesh.Vertices.Add(new Vector3(valueX * _size.x, valueY * _size.y, valueZ * _size.z));
                    iMesh.Normals.Add(-Vector3.forward);
                    iMesh.Colors.Add(_style.GetColorCategorical(dimIndex, valueX));
                    if (itemIndex == 0)
                        continue;
                    iMesh.Indices.Add(iMesh.Vertices.Count - 2);
                    iMesh.Indices.Add(iMesh.Vertices.Count - 1);
                }
            }
            var meshFilter = GetComponent<MeshFilter>();
            meshFilter.sharedMesh = iMesh.GenerateMesh("LineChart2DMesh", MeshTopology.Lines);
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
    }
}
