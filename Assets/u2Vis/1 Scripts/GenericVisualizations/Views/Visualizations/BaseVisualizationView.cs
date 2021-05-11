using System;
using System.Collections.Generic;
using UnityEngine;

namespace u2vis
{
    public abstract class BaseVisualizationView : MonoBehaviour
    {
        #region Protected Fields
        [SerializeField]
        protected GenericDataPresenter _presenter = null;
        [SerializeField]
        protected Vector3 _size = Vector3.one;
        [SerializeField]
        protected bool _showAxes = true;
        [SerializeField]
        protected GenericAxisView _axisViewPrefab = null;
        [SerializeField]
        protected List<GenericAxisView> _axisViews = null;
        [SerializeField]
        protected GenericVisualizationStyle _style = null;
            
        protected bool _fromEditor = true;
        protected bool _initialized = false;

        protected bool _lazyRebuild = true;
        protected RebuildState _rebuildState = RebuildState.All;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the data presenter for this visualization.
        /// </summary>
        public GenericDataPresenter Presenter => _presenter;
        /// <summary>
        /// Determines the Size of the visualization in unity units.
        /// </summary>
        public Vector3 Size
        {
            get { return _size; }
            set { SetSize(value); }
        }
        /// <summary>
        /// Gets or sets the axis view prefab used to generate axes for this view.
        /// </summary>
        public GenericAxisView AxisViewPrefab
        {
            get { return _axisViewPrefab; }
            set { SetAxisPrefab(value); }
        }
        /// <summary>
        /// Determines if Axis are shown for the visualization or not.
        /// </summary>
        public bool ShowAxes
        {
            get { return _showAxes; }
            set { SetShowAxis(value); }
        }
        /// <summary>
        /// Determines if the visualization should be rebuild every time an attribute changes
        /// or lazily before it needs to be rendered the next time.
        /// </summary>
        public bool LazyRebuild
        {
            get { return _lazyRebuild; }
            set { SetLazyRebuild(value); }
        }
        /// <summary>
        /// Gets or sets the style that is used for this visualization.
        /// </summary>
        public GenericVisualizationStyle Style
        {
            get { return _style; }
            set
            {
                _style = value;
                Rebuild();
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Raised after the visualization has finished a triggered rebuild.
        /// </summary>
        public event EventHandler RebuildFinished;
        #endregion

        #region Constructors
        protected BaseVisualizationView()
        {
        }
        #endregion

        #region Protected Methods
        protected virtual void Start()
        {
            _fromEditor = false;
            if (!_initialized)
            {
                _initialized = true;
                if (_presenter != null)
                    _presenter.DataUpdated += Presenter_DataUpdated;
                // if there is no axis prefab, there can be no axis views
                if (_axisViewPrefab == null)
                    _showAxes = false;
                // Destroy initial axis views because references to axis presenters somehow gets lost
                // when started from the editor, so we just rebuild them completely
                DestroyAxisViews();

                Rebuild_Internal();
            }
        }

        protected virtual void Update()
        {
            if (_lazyRebuild && _rebuildState != RebuildState.Nothing)
                Rebuild_Internal();
        }

        protected virtual void Presenter_DataUpdated(object sender, DataPresenterEventArgs e)
        {
            // Only rebuild when not called from editor
            if (_fromEditor)
                return;
            _rebuildState |= RebuildState.Visualization;
            Rebuild();
        }

        protected virtual void SetSize(Vector3 size)
        {
            if (_size == size)
                return;
            _size = size;
            if (!_initialized)
                return;
            _rebuildState |= RebuildState.AxisSetup;
            Rebuild();
        }

        protected virtual void SetLazyRebuild(bool value)
        {
            if (value == _lazyRebuild)
                return;
            _lazyRebuild = value;
            // if the visualization was already initialized, lazy rebuild was just disabled
            // and the visualization requires rebuilding, do that immediately
            if (_initialized && !_lazyRebuild && _rebuildState != RebuildState.Nothing)
                Rebuild();
        }

        #region Axis Related
        protected virtual void DestroyAxisViews()
        {
            if (_axisViews == null)
                return;
            foreach (var axisView in _axisViews)
                if (axisView != null)
                    DestroyImmediate(axisView.gameObject);
            _axisViews = null;
            _rebuildState |= RebuildState.AxisSetup;
        }
        /// <summary>
        /// Instantiates and transforms the default Axis Views for this visualization.
        /// The BaseVisualizationView defaults to a 2D visualization with an X and Y axis.
        /// If a derived visualization needs different axes, override this method.
        /// </summary>
        protected virtual void SetupInitialAxisViews()
        {
            DestroyAxisViews();
            _axisViews = new List<GenericAxisView>();
            // Generic X Axis
            var vX = Instantiate(_axisViewPrefab, transform, false);
            vX.AxisPresenter = _presenter.AxisPresenters[0];
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
        /// <summary>
        /// Sets the value of _showAxis and rebuilds or destroys axes accordingly.
        /// </summary>
        /// <param name="value">The new value.</param>
        protected virtual void SetShowAxis(bool value)
        {
            if (value == _showAxes)
                return;
            _showAxes = value;
            if (!_initialized)
                return;
            if (_showAxes)
            {
                SetupInitialAxisViews();
                RebuildAxes();
            }
            else
                DestroyAxisViews();
        }
        /// <summary>
        /// Sets the prefab used for the axis views of this visualization.
        /// </summary>
        /// <param name="prefab">The axis view prefab.</param>
        protected virtual void SetAxisPrefab(GenericAxisView prefab)
        {
            if (_axisViewPrefab == prefab)
                return;
            _axisViewPrefab = prefab;
            if (!_initialized)
                return;
            SetupInitialAxisViews();
            RebuildAxes();
        }
        /// <summary>
        /// Generates the axis scales and labels for every axis view,
        /// using the axis presenter and data dimensions with the same index from the data presenter.
        /// </summary>
        protected virtual void RebuildAxes()
        {
            if (_rebuildState.HasFlag(RebuildState.AxisSetup) || _fromEditor)
                SetupInitialAxisViews();
            for (int i = 0; i < _axisViews.Count; i++)
            {
                var axis = _presenter.AxisPresenters[i];
                var ticks = axis.GenerateFromDimension(_presenter[i], _presenter.SelectedMinItem, _presenter.SelectedMaxItem);
                _axisViews[i].RebuildAxis(ticks,_presenter.Dimensions[i].Name);
            }
        }
        #endregion

        /// <summary>
        /// Rebuilds the visualization itself.
        /// Called when the data has changed.
        /// </summary>
        protected abstract void RebuildVisualization();

        protected virtual void Rebuild_Internal()
        {
            if (_rebuildState.HasFlag(RebuildState.Visualization))
                RebuildVisualization();
            if (_showAxes && _rebuildState.HasFlag(RebuildState.Axis))
                RebuildAxes();
            // if this was triggered by the editor,
            else if (_fromEditor && _axisViews != null)
                DestroyAxisViews();
            _rebuildState = RebuildState.Nothing;
            OnRebuildFinished();
        }

        protected virtual void OnRebuildFinished()
        {
            RebuildFinished?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Public Methods

        public virtual void Initialize(GenericDataPresenter presenter, GenericAxisView axisViewPrefab = null, GenericVisualizationStyle style = null)
        {
            if (presenter == null)
                throw new ArgumentNullException("Presenter can't be null");
            if (_presenter != null)
                _presenter.DataUpdated -= Presenter_DataUpdated;

            _initialized = true;
            _fromEditor = false;
            _presenter = presenter;
            _presenter.DataUpdated += Presenter_DataUpdated;
            _axisViewPrefab = axisViewPrefab;
            // if there is no axis prefab, there can be no axis views
            if (_axisViewPrefab == null)
                _showAxes = false;
            // Destroy initial axis views because references to axis presenters somehow gets lost
            // when started from the editor, so we just rebuild them completely
            DestroyAxisViews();
            _style = style;

            Rebuild_Internal();
        }
        /// <summary>
        /// Rebuilds the Visualization and Axes (if visisble)
        /// </summary>
        public virtual void Rebuild()
        {
            // If this was called from the editor, ignore lazyRebuild and
            // rebuild immediately, because update is never called.
            if (_fromEditor)
            {
                _rebuildState = RebuildState.All;
                Rebuild_Internal();
                return;
            }
            _rebuildState |= RebuildState.Visualization | RebuildState.Axis;
            // If lazyRebuild is true, don't rebuild immediately, but just remember that we need to rebuild
            // next time update is called. 
            if (!_lazyRebuild)
                Rebuild_Internal();
        }
        /// <summary>
        /// Called from the Editor when the presenter has changed.
        /// </summary>
        public virtual void RebindPresenter()
        {
            if (_presenter != null)
                _presenter.DataUpdated += Presenter_DataUpdated;
            Rebuild();
        }

        public virtual bool TryGetPosForItemIndex(int itemIndex, out Vector3 pos)
        {
            //pos = Vector3.zero;
            //return false;
            throw new NotImplementedException();
        }
        #endregion

        #region Nested
        protected enum RebuildState : int
        {
            Nothing             = 0b0000,
            Visualization       = 0b0001,
            Axis                = 0b0010,
            AxisSetup           = 0b0110,
            All                 = 0xf,
        }
        #endregion
    }
}
