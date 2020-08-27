using System;
using System.Collections.Generic;
using UnityEngine;

namespace UVis
{
    public class Scatterplot2D : BaseVisualizationView
    {
        #region Protected Fields
        [SerializeField]
        protected Vector3 _zoomMin = Vector3.zero;
        [SerializeField]
        protected Vector3 _zoomMax = Vector3.one;
        [SerializeField]
        protected bool _displayRelativeValues = true;
        #endregion

        #region Public Properties
        public Vector3 ZoomMin
        {
            get { return _zoomMin; }
            set { SetZoomLevel(value, _zoomMax); }
        }

        public Vector3 ZoomMax
        {
            get { return _zoomMax; }
            set { SetZoomLevel(_zoomMin, value); }
        }

        public bool DisplayRelativeValues
        {
            get { return _displayRelativeValues; }
        }
        #endregion

        #region Protected Methods
        protected override void SetupInitialAxisViews()
        {
            base.SetupInitialAxisViews();
            if (_presenter.NumberOfDimensions < 3)
                return;
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
            if (_presenter == null || _presenter.NumberOfDimensions < 2)
            {
                Debug.LogError("Presenter is either null or has not enough dimensions to represent this visualization");
                return;
            }
            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();
            List<Color> colors = new List<Color>();

            //int offset = (int)_presenter.SelectedMinItem;
            //int length = (int)_presenter.SelectedMaxItem - offset;
            int dimNum = _presenter.NumberOfDimensions;
            var zoomArea = _zoomMax - ZoomMin;

            for (int itemIndex = _presenter.SelectedMinItem; itemIndex < _presenter.SelectedMaxItem; itemIndex++)
            {
               float valueX = VisViewHelper.GetItemValue(_presenter, 0, itemIndex, true, _displayRelativeValues);
               float valueY = VisViewHelper.GetItemValue(_presenter, 1, itemIndex, true, _displayRelativeValues);
               float valueZ = dimNum < 3 ? 0 : VisViewHelper.GetItemValue(_presenter, 2, itemIndex, true, _displayRelativeValues);

                if (valueX < _zoomMin.x || valueX > _zoomMax.x ||
                    valueY < _zoomMin.y || valueY > _zoomMax.y ||
                    (dimNum > 2 && (valueZ < _zoomMin.z || valueZ > _zoomMax.z))
                    )
                    continue;
                vertices.Add(new Vector3(
                    (valueX - _zoomMin.x) / zoomArea.x * _size.x,
                    (valueY - _zoomMin.y) / zoomArea.y * _size.y,
                    (valueZ - _zoomMin.z) / zoomArea.z * _size.z
                    ));
                colors.Add(_style.GetColorContinous(_presenter.IsItemHighlighted(itemIndex), valueX, valueY, valueZ));
                indices.Add(vertices.Count - 1);
            }

            var mesh = new Mesh();
            mesh.name = "ScatterplotMesh";
            mesh.vertices = vertices.ToArray();
            mesh.colors = colors.ToArray();
            mesh.SetIndices(indices.ToArray(), MeshTopology.Points, 0);

            var meshFilter = GetComponent<MeshFilter>();
            Destroy(meshFilter.sharedMesh);
            meshFilter.sharedMesh = mesh;
        }

        protected override void RebuildAxes()
        {
            if (_axisViews == null || _fromEditor)
                SetupInitialAxisViews();
            for (int i = 0; i < _presenter.AxisPresenters.Length; i++)
            {
                AxisTick[] ticks;
                var numDim = _presenter[i] as INumericalDimension;
                if (numDim != null && !_presenter.AxisPresenters[i].IsCategorical)
                {
                    float range = numDim.MaximumFloatValue - (_displayRelativeValues ? numDim.MinimumFloatValue : 0);
                    float min = _displayRelativeValues ? numDim.MinimumFloatValue : 0;
                    ticks = _presenter.AxisPresenters[i].GenerateFromMinMaxValue(_zoomMin[i] * range + min, _zoomMax[i] * range + min);
                    _axisViews[i].RebuildAxis(ticks, _presenter[i].Name);
                }
            }
        }
        #endregion

        #region Public Methods
        public virtual void SetZoomLevel(Vector3 min, Vector3 max)
        {
            bool changed = false;
            min.x = Mathf.Clamp(min.x, 0, 1);
            min.y = Mathf.Clamp(min.y, 0, 1);
            min.z = Mathf.Clamp(min.z, 0, 1);

            max.x = Mathf.Clamp(max.x, 0, 1);
            max.y = Mathf.Clamp(max.y, 0, 1);
            max.z = Mathf.Clamp(max.z, 0, 1);
            if (min != _zoomMin)
            {
                changed = true;
                _zoomMin = min;
            }
            if (max != _zoomMax)
            {
                changed = true;
                _zoomMax = max;
            }
            if (changed)
                Rebuild();
        }

        public override bool TryGetPosForItemIndex(int itemIndex, out Vector3 pos)
        {
            pos = Vector3.zero;
            if (itemIndex < _presenter.SelectedMinItem || itemIndex >= _presenter.SelectedMaxItem)
                return false;
            int dimNum = _presenter.NumberOfDimensions;
            var zoomArea = _zoomMax - ZoomMin;
            float valueX = VisViewHelper.GetItemValue(_presenter, 0, itemIndex, true, _displayRelativeValues);
            float valueY = VisViewHelper.GetItemValue(_presenter, 1, itemIndex, true, _displayRelativeValues);
            float valueZ = dimNum < 3 ? 0 : VisViewHelper.GetItemValue(_presenter, 2, itemIndex, true, _displayRelativeValues);
            if (valueX < _zoomMin.x || valueX > _zoomMax.x ||
                valueY < _zoomMin.y || valueY > _zoomMax.y ||
                (dimNum > 2 && (valueZ < _zoomMin.z || valueZ > _zoomMax.z))
                )
                return false;
            pos = new Vector3(
                (valueX - _zoomMin.x) / zoomArea.x * _size.x,
                (valueY - _zoomMin.y) / zoomArea.y * _size.y,
                (valueZ - _zoomMin.z) / zoomArea.z * _size.z
                );
            return true;
        }
        #endregion
    }
}
