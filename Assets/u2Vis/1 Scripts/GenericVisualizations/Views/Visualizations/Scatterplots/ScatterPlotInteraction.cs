using u2vis;
using Lean.Touch;
using System;
using UnityEngine;

namespace u2vis.ARGH
{
    public class ScatterPlotInteraction : BaseVisualizationInteraction
    {
        Scatterplot2D _scatterplot2d = null;
        Vector3 _panAmountX = new Vector3(0.05f, 0, 0);
        Vector3 _panAmountY = new Vector3(0, 0.05f, 0);
        Vector3 _zoomAmount = new Vector3(0.05f, 0.05f, 0);

        [SerializeField]
        private float _itemSize = 0.02f;

        protected override void Start()
        {
            base.Start();
            if (_visualization != null)
                _scatterplot2d = (Scatterplot2D)_visualization;
        }

        protected override void Update()
        {
            base.Update();
            if (UnityEngine.Input.GetKeyUp(KeyCode.V))
            {
                Debug.Log("Test");
                ResetHighlights();
            }
        }

        protected override void PanRight()
        {
            if (_scatterplot2d == null || _scatterplot2d.ZoomMin.x - 0.001f <= 0.0f)
                return;
            _scatterplot2d.SetZoomLevel(_scatterplot2d.ZoomMin + _panAmountX, _scatterplot2d.ZoomMax + _panAmountX);
        }

        protected override void PanLeft()
        {
            if (_scatterplot2d == null || _scatterplot2d.ZoomMax.x + 0.001f >= 1.0f)
                return;
            _scatterplot2d.SetZoomLevel(_scatterplot2d.ZoomMin - _panAmountX, _scatterplot2d.ZoomMax - _panAmountX);
        }

        protected override void PanUp()
        {
            if (_scatterplot2d == null || _scatterplot2d.ZoomMin.y - 0.001f <= 0.0f)
                return;
            _scatterplot2d.SetZoomLevel(_scatterplot2d.ZoomMin - _panAmountY, _scatterplot2d.ZoomMax - _panAmountY);
        }

        protected override void PanDown()
        {
            if (_scatterplot2d == null || _scatterplot2d.ZoomMax.y + 0.001f >= 1.0f)
                return;
            _scatterplot2d.SetZoomLevel(_scatterplot2d.ZoomMin + _panAmountY, _scatterplot2d.ZoomMax + _panAmountY);
        }

        // todo: fix zoom, calc amount of space left and right, resize accordingly to prevent non-uniform zoom
        protected override void ZoomIn()
        {
            if (_scatterplot2d == null)
                return;
            _scatterplot2d.SetZoomLevel(_scatterplot2d.ZoomMin - _zoomAmount, _scatterplot2d.ZoomMax + _zoomAmount);
        }

        protected override void ZoomOut()
        {
            if (_scatterplot2d == null)
                return;
            _scatterplot2d.SetZoomLevel(_scatterplot2d.ZoomMin + _zoomAmount, _scatterplot2d.ZoomMax - _zoomAmount);
        }

        protected virtual void PerformHitTest(Vector3 localHitPoint)
        {
            int dimNum = _presenter.NumberOfDimensions;
            var zoomArea = _scatterplot2d.ZoomMax - _scatterplot2d.ZoomMin;
            int hitIndex = -1;
            float sqSize = _itemSize * _itemSize;
            for (int itemIndex = _presenter.SelectedMinItem; itemIndex < _presenter.SelectedMaxItem; itemIndex++)
            {
                float valueX = VisViewHelper.GetItemValue(_presenter, 0, itemIndex, true, _scatterplot2d.DisplayRelativeValues);
                float valueY = VisViewHelper.GetItemValue(_presenter, 1, itemIndex, true, _scatterplot2d.DisplayRelativeValues);
                float valueZ = dimNum < 3 ? 0 : VisViewHelper.GetItemValue(_presenter, 2, itemIndex, true, _scatterplot2d.DisplayRelativeValues);
                if (valueX < _scatterplot2d.ZoomMin.x || valueX > _scatterplot2d.ZoomMax.x ||
                    valueY < _scatterplot2d.ZoomMin.y || valueY > _scatterplot2d.ZoomMax.y ||
                    (dimNum > 2 && (valueZ < _scatterplot2d.ZoomMin.z || valueZ > _scatterplot2d.ZoomMax.z))
                    )
                    continue;
                var point = new Vector3(
                    (valueX - _scatterplot2d.ZoomMin.x) / zoomArea.x * _scatterplot2d.Size.x,
                    (valueY - _scatterplot2d.ZoomMin.y) / zoomArea.y * _scatterplot2d.Size.y,
                    (valueZ - _scatterplot2d.ZoomMin.z) / zoomArea.z * _scatterplot2d.Size.z
                    );
                if ((localHitPoint - point).sqrMagnitude < sqSize)
                {
                    hitIndex = itemIndex;
                    break;
                }
            }
            if (hitIndex == -1)
                return;
            _presenter.ToogleItemHighlight(hitIndex);
        }

        public override void OnFingerTap(LeanFinger finger, int order, RaycastHit hit)
        {
            base.OnFingerTap(finger, order, hit);
            PerformHitTest(transform.InverseTransformPoint(hit.point));
        }

        public override void OnMouseBtnUp(int button, int order, RaycastHit hit)
        {
            base.OnMouseBtnUp(button, order, hit);
            PerformHitTest(transform.InverseTransformPoint(hit.point));
        }
    }
}