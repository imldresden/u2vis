using u2vis.Input;
using Lean.Touch;
using System.Collections.Generic;
using UnityEngine;

namespace u2vis
{
    public class BaseVisualizationInteraction : UiElement
    {
        #region Protected Fields
        [SerializeField]
        protected GenericDataPresenter _presenter = null;
        [SerializeField]
        protected BaseVisualizationView _visualization = null;
        [Tooltip("Determines how much pixels a finger has to be dragged for the bar chart to scroll left or right.")]
        [SerializeField]
        protected float _dragThreshold = 100.0f;
        [Tooltip("Determines how the pich factor, after which the bar chart is zoomed")]
        [SerializeField]
        protected float _pinchThreshold = 0.25f;

        protected float _dragDeltaX = 0.0f;
        protected float _dragDeltaY = 0.0f;
        protected float _pinchDelta = 0.0f;
        protected List<LeanFinger> _fingers = new List<LeanFinger>();
        #endregion

        #region Public Properties
        public float DragThreshold
        {
            get { return _dragThreshold; }
            set { _dragThreshold = value; }
        }

        public float PinchThreshold
        {
            get { return _pinchThreshold; }
            set { _pinchThreshold = value; }
        }
        #endregion

        #region Protected Methods
        protected virtual void Start()
        {
            // Check if there is a colider, if not add a box collider as big as the size of the visualizatzion
            var collider = GetComponent<Collider>();
            if (collider == null)
            {
                var boxCollider = gameObject.AddComponent<BoxCollider>();
                boxCollider.size = _visualization.Size;
            }
        }

        protected virtual void Update()
        {
        }

        protected virtual void ResetHighlights()
        {
            for (int i = 0; i < _presenter.TotalItemCount; i++)
                if (_presenter.IsItemHighlighted(i))
                    _presenter.ToogleItemHighlight(i);
        }

        #region Panning
        protected virtual void DragToMove(float deltaX, float deltaY)
        {
            if (_fingers.Count != 1)
                return;
            _dragDeltaX += deltaX;
            // Move Left
            if (_dragDeltaX > _dragThreshold)
            {
                _dragDeltaX -= _dragThreshold;
                PanLeft();
            }
            // Move Right
            else if (_dragDeltaX < -_dragThreshold)
            {
                _dragDeltaX += _dragThreshold;
                PanRight();
            }

            _dragDeltaY += deltaY;
            // Move Left
            if (_dragDeltaY > _dragThreshold)
            {
                _dragDeltaY -= _dragThreshold;
                PanUp();
            }
            // Move Right
            else if (_dragDeltaY < -_dragThreshold)
            {
                _dragDeltaY += _dragThreshold;
                PanDown();
            }
        }

        protected virtual void PanLeft()
        {
            if (_presenter.SelectedMinItem > 0)
                _presenter.SetSelectedItemIndices(_presenter.SelectedMinItem - 1, _presenter.SelectedMaxItem - 1);
        }

        protected virtual void PanRight()
        {
            if (_presenter.SelectedMaxItem < _presenter[0].Count)
                _presenter.SetSelectedItemIndices(_presenter.SelectedMinItem + 1, _presenter.SelectedMaxItem + 1);
        }

        protected virtual void PanUp()
        {
        }

        protected virtual void PanDown()
        {
        }
        #endregion

        #region Zoom
        protected virtual void PinchToZoom(float delta)
        {
            if (_fingers.Count != 2)
                return;
            _pinchDelta += delta;
            // Zoom Out
            if (_pinchDelta < -_pinchThreshold)
            {
                _pinchDelta += _pinchThreshold;
                ZoomIn();
            }
            // Zoom In
            else if (_pinchDelta > _pinchThreshold)
            {
                _pinchDelta -= _pinchThreshold;
                ZoomOut();
            }
        }

        protected virtual void ZoomIn()
        {
            _presenter.SetSelectedItemIndices(_presenter.SelectedMinItem - 1, _presenter.SelectedMaxItem + 1);
        }

        protected virtual void ZoomOut()
        {
            _presenter.SetSelectedItemIndices(_presenter.SelectedMinItem + 1, _presenter.SelectedMaxItem - 1);
        }
        #endregion

        #endregion

        #region UiElement
        public override void OnFingerDown(LeanFinger finger)
        {
            _fingers.Add(finger);
        }

        public override void OnFingerMove(LeanFinger finger)
        {
            DragToMove(finger.ScaledDelta.x, finger.ScaledDelta.y);
        }

        public override void OnFingerUp(LeanFinger finger)
        {
            _fingers.Remove(finger);
        }

        public override void OnFingerPinch(float value)
        {
            PinchToZoom(value - 1.0f);
        }
        #endregion

        #region Public Methods
        public virtual void Initialize(GenericDataPresenter presenter, BaseVisualizationView visualization)
        {
            _presenter = presenter;
            _visualization = visualization;
        }
        #endregion
    }
}