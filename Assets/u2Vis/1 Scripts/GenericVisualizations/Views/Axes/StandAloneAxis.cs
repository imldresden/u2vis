using System;
using UnityEngine;

namespace u2vis
{
    /// <summary>
    /// Represents an acus view that is not coupled to a visualization but can be placed freely in the scene.
    /// </summary>
    /// <remarks>
    /// How to use from code:
    /// a) Create a new instance of the StandAloneAxis
    /// b) Call the initilize method
    /// c) Call the generation method that suit the desired kind of axis.
    /// d) Set the AxisLength, IsMirroed, and IsSwapped to the desired values.
    /// </remarks>
    public class StandAloneAxis : MonoBehaviour
    {
        #region Protected Fields
        /// <summary>
        /// The presenter providing the data for this axis.
        /// </summary>
        [SerializeField]
        protected GenericDataPresenter _presenter = null;
        /// <summary>
        /// The index if the axis presenter containing the parameters for this axis.
        /// </summary>
        [SerializeField]
        protected int _axisPresenterIndex = 0;
        /// <summary>
        /// Prefab for the axis view.
        /// </summary>
        [SerializeField]
        protected GenericAxisView _axisViewPrefab = null;
        /// <summary>
        /// AxisView instantiated from the prefab.
        /// </summary>
        [SerializeField]
        protected GenericAxisView _axisView = null;
        /// <summary>
        /// The length of this axis in Unity units.
        /// </summary>
        [SerializeField]
        protected float _axisLength = 1.0f;
        /// <summary>
        /// Determines if the side of the axis the labels are shown is swapped or not.
        /// For a vertical axis, swapped shows labels on the right, not swapped on the left.
        /// </summary>
        [SerializeField]
        protected bool _swapped = false;
        /// <summary>
        /// Determines if the ticks on the axis are mirrored, i.e., if they start at the beginning or the end of the axis.
        /// </summary>
        [SerializeField]
        protected bool _mirrored = false;
        /// <summary>
        /// The current genration method for the axis labels.
        /// </summary>
        [SerializeField]
        protected GenerationMethod _generationMethod = GenerationMethod.Dimension;
        /// <summary>
        /// The first parameter for the generation method. The exact meaning depends in the method.
        /// </summary>
        [SerializeField]
        protected int _modeParam1 = 0;
        /// <summary>
        /// The second parameter for the generation method. The exact meaning depends in the method.
        /// </summary>
        [SerializeField]
        protected int _modeParam2 = 0;
        /// <summary>
        /// The minimum float value used by the MinMax generation method.
        /// </summary>
        [SerializeField]
        protected float _min = 0.0f;
        /// <summary>
        /// The maximum float value used by the MinMax generation method.
        /// </summary>
        [SerializeField]
        protected float _max = 1.0f;
        /// <summary>
        /// Indicates if the script was called from the editor mode.
        /// </summary>
        protected bool _fromEditor = true;
        /// <summary>
        /// Indicates if the script was already initialized or not.
        /// </summary>
        protected bool _isInitialized = false;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the presenter providing the data for this axis.
        /// </summary>
        public GenericDataPresenter DataPresenter => _presenter;
        /// <summary>
        /// Gets the prefab for the axis view.
        /// </summary>
        public GenericAxisView AxisViewPrefab => _axisViewPrefab;
        /// <summary>
        /// Gets the current method used to generate the axis labels.
        /// </summary>
        public GenerationMethod Method => _generationMethod;
        /// <summary>
        /// Gets or sets the length of this axis view.
        /// </summary>
        public float AxisLength
        {
            get { return _axisLength; }
            set { _axisLength = value; }
        }
        /// <summary>
        /// Determines if the side of the axis the labels are shown is swapped or not.
        /// For a vertical axis, swapped shows labels on the right, not swapped on the left.
        /// </summary>
        public bool IsSwapped
        {
            get { return _swapped; }
            set { _swapped = value; }
        }
        /// <summary>
        /// Determines if the ticks on the axis are mirrored, i.e., if they start at the beginning or the end of the axis.
        /// </summary>
        public bool IsMirrored
        {
            get { return _mirrored; }
            set { _mirrored = value; }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Called by unity once at the start of this script.
        /// </summary>
        protected virtual void Start()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                _fromEditor = false;
                if (_presenter != null)
                    _presenter.DataUpdated += DataPresenter_DataUpdated;
                DestroyAxisView();
                RebuildAxis();
            }
        }
        /// <summary>
        /// Calles whenever the data of the data presenter was updated. Rebuilds the axis view.
        /// </summary>
        /// <param name="sender">The data presenter which data was updated.</param>
        /// <param name="e">The event arguments.</param>
        protected virtual void DataPresenter_DataUpdated(object sender, System.EventArgs e)
        {
            RebuildAxis();
        }
        /// <summary>
        /// Destroy the current axis view.
        /// </summary>
        protected virtual void DestroyAxisView()
        {
            if (_axisView != null)
                DestroyImmediate(_axisView.gameObject);
            _axisView = null;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initilizite this stand alone axis.
        /// </summary>
        /// <param name="presenter">The presenter proving the data for the axis view.</param>
        /// <param name="axisPresenterIndex">The index of the axis presenter containing the parameters for this axis.</param>
        /// <param name="axisViewPrefab">A prefab from which the axis view is instantiated.</param>
        public virtual void Initialize(GenericDataPresenter presenter, int axisPresenterIndex, GenericAxisView axisViewPrefab)
        {
            if (presenter == null)
                throw new ArgumentException("Presenter can't be null");
            _isInitialized = true;
            if (_presenter != null)
                _presenter.DataUpdated -= DataPresenter_DataUpdated;
            _presenter = presenter;
            _axisPresenterIndex = axisPresenterIndex;
            _axisViewPrefab = axisViewPrefab;
            _fromEditor = false;
            if (_presenter != null)
                _presenter.DataUpdated += DataPresenter_DataUpdated;
            DestroyAxisView();
            RebuildAxis();
        }
        /// <summary>
        /// Rebuilds the axis view based on the current parameters.
        /// </summary>
        public virtual void RebuildAxis()
        {
            if (_fromEditor)
                DestroyAxisView();
            if (_axisViewPrefab == null)
                return;
            if (_axisView == null)
            {
                _axisView = Instantiate(_axisViewPrefab, transform, false);
                _axisView.Length = _axisLength;
                _axisView.Swapped = _swapped;
                _axisView.Mirrored = _mirrored;
                _axisView.AxisPresenter = _presenter.AxisPresenters[_axisPresenterIndex];
            }
            var axis = _presenter.AxisPresenters[_axisPresenterIndex];
            AxisTick[] ticks;
            switch (_generationMethod)
            {
                default:
                case GenerationMethod.Dimension:
                    ticks = axis.GenerateFromDimension(_presenter[_modeParam1], _presenter.SelectedMinItem, _presenter.SelectedMaxItem);
                    break;
                case GenerationMethod.DimensionCaptions:
                    ticks = axis.GenerateFromDimensionCaptions(_presenter, _modeParam1, _modeParam2);
                    break;
                case GenerationMethod.DiscreteRange:
                    ticks = axis.GenerateFromDiscreteRange(_modeParam1, _modeParam2);
                    break;
                case GenerationMethod.MinMaxValue:
                    ticks = axis.GenerateFromMinMaxValue(_min, _max);
                    break;
            }
            _axisView.RebuildAxis(ticks);
        }
        /// <summary>
        /// Generate axis labels from the default method for numerical or categorical dimensions.
        /// </summary>
        /// <param name="dimensionIndex">The index of the data dimension from which the labels will be generated.</param>
        public virtual void GenerateFromDimension(int dimensionIndex)
        {
            _generationMethod = GenerationMethod.Dimension;
            _modeParam1 = dimensionIndex;
            RebuildAxis();
        }
        /// <summary>
        /// Generates Axis Labels from the Captions of multiple data dimensions.
        /// </summary>
        /// <param name="start">The index of the start dimension.</param>
        /// <param name="end">the index of the end dimension.</param>
        public virtual void GenerateFromDimensioCaptions(int dimensionIndexStart, int dimensionIndexEnd)
        {
            _generationMethod = GenerationMethod.DimensionCaptions;
            _modeParam1 = dimensionIndexStart;
            _modeParam2 = dimensionIndexEnd;
            RebuildAxis();
        }
        /// <summary>
        /// Generates labels between the specified range.
        /// </summary>
        /// <param name="start">The start number.</param>
        /// <param name="end">The end number.</param>
        public virtual void GenerateFromDiscreteRange(int start, int end)
        {
            _generationMethod = GenerationMethod.DiscreteRange;
            _modeParam1 = start;
            _modeParam2 = end;
            RebuildAxis();
        }
        /// <summary>
        /// Generates labels between the specified minimum and maximum values.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        public virtual void GenerateFromMinMaxValue(float min, float max)
        {
            _generationMethod = GenerationMethod.MinMaxValue;
            _min = min;
            _max = max;
            RebuildAxis();
        }
        #endregion
        /// <summary>
        /// Enumeration representing the available generation methods for the ticks of an axis.
        /// </summary>
        public enum GenerationMethod
        {
            /// <summary>
            /// Uses the default generation method for numerical or categorical dimensions. modeParam1 contains the dimension of the index.
            /// </summary>
            Dimension = 0,
            /// <summary>
            /// Generates Axis Labels from the Captions of multiple data dimensions.. modeParam1 contains the start index, modeParam1 the end index.
            /// </summary>
            DimensionCaptions = 1,
            /// <summary>
            /// Generates labels based on the specified range. modeParam1 contains the start number, modeParam2 the end number.
            /// </summary>
            DiscreteRange = 2,
            /// <summary>
            /// Generates labels between the specified min and max value. uses the min and max fields.
            /// </summary>
            MinMaxValue = 3,
        }
    }
}
