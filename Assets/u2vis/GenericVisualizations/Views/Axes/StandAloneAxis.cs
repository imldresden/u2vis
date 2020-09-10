using System;
using UnityEngine;

namespace u2vis
{
    // TODO implement create by code
    public class StandAloneAxis : MonoBehaviour
    {
        #region Protected Fields
        [SerializeField]
        protected GenericDataPresenter _presenter = null;
        [SerializeField]
        protected int _axisPresenterIndex = 0;
        [SerializeField]
        protected GenericAxisView _axisViewPrefab = null;
        [SerializeField]
        protected GenericAxisView _axisView = null;
        [SerializeField]
        protected float _axisLength = 1.0f;
        [SerializeField]
        protected bool _swapped = false;
        [SerializeField]
        protected bool _mirrored = false;

        [SerializeField]
        protected GenerationMethod _generationMethod = GenerationMethod.Dimension;
        [SerializeField]
        protected int _modeParam1 = 0;
        [SerializeField]
        protected int _modeParam2 = 0;
        [SerializeField]
        protected float _min = 0.0f;
        [SerializeField]
        protected float _max = 1.0f;

        protected bool _fromEditor = true;
        protected bool _isInitialized = false;
        #endregion

        #region Public Properties
        public GenericDataPresenter DataPresenter => _presenter;
        public GenericAxisView AxisViewPrefab => _axisViewPrefab;
        public GenerationMethod Method => _generationMethod;
        #endregion

        #region Protected Methods
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

        protected virtual void DataPresenter_DataUpdated(object sender, System.EventArgs e)
        {
            RebuildAxis();
        }

        protected virtual void DestroyAxisView()
        {
            if (_axisView != null)
                DestroyImmediate(_axisView.gameObject);
            _axisView = null;
        }
        #endregion

        #region Public Methods
        public virtual void Initialize(GenericDataPresenter presenter, GenericAxisView axisViewPrefab)
        {
            if (presenter == null)
                throw new ArgumentException("Presenter can't be null");
            _isInitialized = true;
            if (_presenter != null)
                _presenter.DataUpdated -= DataPresenter_DataUpdated;
            _presenter = presenter;
            _axisViewPrefab = axisViewPrefab;
            _fromEditor = false;
            if (_presenter != null)
                _presenter.DataUpdated += DataPresenter_DataUpdated;
            DestroyAxisView();
            RebuildAxis();
        }

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

        public virtual void GenerateFromDimension(int dimensionIndex)
        {
            _generationMethod = GenerationMethod.Dimension;
            _modeParam1 = dimensionIndex;
            RebuildAxis();
        }

        public virtual void GenerateFromDimensioCaptions(int dimensionIndexStart, int dimensionIndexEnd)
        {
            _generationMethod = GenerationMethod.DimensionCaptions;
            _modeParam1 = dimensionIndexStart;
            _modeParam2 = dimensionIndexEnd;
            RebuildAxis();
        }

        public virtual void GenerateFromDicreteRange(int start, int end)
        {
            _generationMethod = GenerationMethod.DiscreteRange;
            _modeParam1 = start;
            _modeParam2 = end;
            RebuildAxis();
        }

        public virtual void GenerateFromMinMaxValue(float min, float max)
        {
            _generationMethod = GenerationMethod.MinMaxValue;
            _min = min;
            _max = max;
            RebuildAxis();
        }
        #endregion

        public enum GenerationMethod
        {
            Dimension = 0,
            DimensionCaptions = 1,
            DiscreteRange = 2,
            MinMaxValue = 3,
        }
    }
}
