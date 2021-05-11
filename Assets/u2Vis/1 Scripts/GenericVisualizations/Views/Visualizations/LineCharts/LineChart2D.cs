using System.Collections.Generic;
using UnityEngine;

namespace u2vis
{
    public class LineChart2D : BaseVisualizationView
    {
        #region Constructors
        protected LineChart2D() : base()
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
            for (int dimIndex = 0; dimIndex < _presenter.NumberOfDimensions; dimIndex++)
            {
                for (int itemIndex = 0; itemIndex < length; itemIndex++)
                {
                    float valueX = (float)itemIndex / length;
                    float valueY = VisViewHelper.GetItemValueAbsolute(_presenter, dimIndex, itemIndex + offset) / maxValue;
                    vertices.Add(new Vector3(valueX * _size.x, valueY * _size.y, 0.0f));
                    normals.Add(-Vector3.forward);
                    colors.Add(_style.GetColorCategorical(dimIndex, valueX));
                    if (itemIndex == 0)
                        continue;
                    indices.Add(vertices.Count - 2);
                    indices.Add(vertices.Count - 1);
                }
            }

            var mesh = new Mesh();
            mesh.name = "LineChart2DMesh";
            mesh.vertices = vertices.ToArray();
            mesh.normals = normals.ToArray();
            mesh.colors = colors.ToArray();
            mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);

            var meshFilter = GetComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;
        }

        protected override void SetupInitialAxisViews()
        {
            DestroyAxisViews();
            _axisViews = new List<GenericAxisView>();
            // Generic X Axis
            var vX = Instantiate(_axisViewPrefab, transform, false);
            vX.AxisPresenter = _presenter.AxisPresenters[0];
            // Hack Align Axis and Lines
            float offset = _size.x / _presenter.SelectedItemsCount * 0.5f;
            vX.transform.localPosition = new Vector3(-offset, 0, 0);
            vX.transform.Find("AxisRoot").localPosition += new Vector3(offset, 0, 0);
            vX.Length = _size.x;
            _axisViews.Add(vX);
            // Generic Y Axis
            var vY = Instantiate(_axisViewPrefab, transform, false);
            vY.AxisPresenter = _presenter.AxisPresenters[1];
            vY.Length = _size.y;
            vY.transform.localRotation = Quaternion.Euler(0, 0, 90);
            vY.Swapped = true;
            _axisViews.Add(vY);
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
        }
        #endregion
    }
}
