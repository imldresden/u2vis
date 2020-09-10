using System;
using UnityEngine;

namespace u2vis
{
    public delegate void HighlightChangedHandler(GenericDataPresenter sender, int itemIndex);
    public class GenericDataPresenter : MonoBehaviour
    {
        public const int DEFAULT_INDEX = 0;

        #region Protected Fields
        [SerializeField]
        protected AbstractDataProvider _dataProvider = null;
        [SerializeField]
        protected int _selectedMinItem = 0;
        [SerializeField]
        protected int _selectedMaxItem = 0;
        // Holds the data dimensions of this presenter as a list of indices to the data dimensions of the data provider.
        // Yes, it would be better and more efficient to simply hold the references to the data dimensions directly,
        // but have fun to try to get that to work with the unity editor in a stable way.
        [SerializeField]
        protected int[] _dataDimensionsIndices = null;
        [SerializeField]
        protected bool[] _highlightedItems = null;
        //Represented the reference of Axis which also is showed in the editor script
        [SerializeField]
        protected AxisPresenter[] _axisPresenters = null;
        protected bool _isInitialized = false;
        #endregion

        #region Public Properties
        public AbstractDataProvider DataProvider => _dataProvider;
        /// <summary>
        /// Returns a list of all data dimensions.
        /// WARNING: Not very efficient, use the index property instead.
        /// </summary>
        public DataDimension[] Dimensions => GetDimensionsArray();
        public int NumberOfDimensions => _dataDimensionsIndices.Length;
        public int TotalItemCount => NumberOfDimensions == 0 ? 0 : this[0].Count;
        public int SelectedItemsCount => _selectedMaxItem - _selectedMinItem;
        public int SelectedMinItem
        {
            get { return _selectedMinItem; }
            set { SetSelectedItemIndices(value, _selectedMaxItem); }
        }
        public int SelectedMaxItem
        {
            get { return _selectedMaxItem; }
            set { SetSelectedItemIndices(_selectedMinItem, value); }
        }
        public DataDimension this[int index] => _dataProvider.Data[_dataDimensionsIndices[index]];
        // TODO: return ReadOnly list instead of real array
        public AxisPresenter[] AxisPresenters => _axisPresenters;
        #endregion

        #region Events
        public event EventHandler<DataPresenterEventArgs> DataUpdated;
        public event HighlightChangedHandler HighlightChanged;
        #endregion

        #region Constructors
        protected GenericDataPresenter()
        {
            _dataDimensionsIndices = new int[1];
            _dataDimensionsIndices[0] = DEFAULT_INDEX;
            SetupInitialAxisPresenters();
        }
        #endregion

        #region Protected Methods
        protected virtual void Start()
        {
            // in case the provider was set in the editor
            if (!_isInitialized)
            {
                _dataProvider.DataChanged += Provider_DataUpdated;
                _highlightedItems = new bool[TotalItemCount];
            }
        }

        protected virtual DataDimension[] GetDimensionsArray()
        {
            var d = new DataDimension[_dataDimensionsIndices.Length];
            for (int i = 0; i<_dataDimensionsIndices.Length; i++)
                d[i] = _dataProvider.Data[_dataDimensionsIndices[i]];
            return d;
        }

        protected virtual void SetupInitialAxisPresenters()
        {
            if (_dataProvider == null)
                return;
            _axisPresenters = new AxisPresenter[NumberOfDimensions];
            for (int i = 0; i < NumberOfDimensions; i++)
                _axisPresenters[i] = new AxisPresenter(this[i].Name);
        }

        protected virtual void Provider_DataUpdated(object sender, EventArgs e)
        {
            if (_highlightedItems.Length != _dataProvider.Data.NumOfItems)
                _highlightedItems = new bool[_dataProvider.Data.NumOfItems];
            //TODO: Check dataSet dimensions, items count, etc.
            DataUpdated?.Invoke(this, new DataPresenterEventArgs(_selectedMinItem, _selectedMaxItem));
        }
        #endregion

        #region Public Methods
        public virtual void Initialize(IDataProvider provider, int minIndex, int maxIndex, params int[] dimensionIndices)
        {
            if (provider == null)
                throw new ArgumentException("DataProvider can't be null!");
            if (_dataProvider != null)
                _dataProvider.DataChanged -= Provider_DataUpdated;

            _dataProvider = (AbstractDataProvider)provider;
            _dataProvider.DataChanged += Provider_DataUpdated;
            _highlightedItems = new bool[TotalItemCount];
            _dataDimensionsIndices = dimensionIndices;
            SetupInitialAxisPresenters();
            _isInitialized = true;
            DataUpdated?.Invoke(this, new DataPresenterEventArgs());
        }

        // HACK: This should not be here and solved differently, because it messes up the internal state
        public virtual void SetDimensionAtIndex(int dataIndex, int dimensionIndex)
        {
            _dataDimensionsIndices[dimensionIndex] = dataIndex;
            var e = new DataPresenterEventArgs();
            DataUpdated?.Invoke(this, e);
        }

        public virtual void SetSelectedItemIndices(int min, int max)
        {
            int numOfItems = _dataProvider.Data.NumOfItems;
            min = Mathf.Clamp(min, 0, numOfItems);
            max = Mathf.Clamp(max, 0, numOfItems);
            if (min > max)
            {
                int t = min;
                min = max;
                max = t;
            }
            if (min == _selectedMinItem && max == _selectedMaxItem)
                return;

            var e = new DataPresenterEventArgs(_selectedMinItem, _selectedMaxItem, min, max);
            _selectedMinItem = min;
            _selectedMaxItem = max;
            DataUpdated?.Invoke(this, e);
        }

        public virtual void ResetAxisProperties()
        {
            SetupInitialAxisPresenters();
        }

        #region Highlights
        public virtual void ToogleItemHighlight(int index)
        {
            _highlightedItems[index] = !_highlightedItems[index];
            HighlightChanged?.Invoke(this, index);
            if (index >= _selectedMinItem || index < _selectedMaxItem)
                DataUpdated?.Invoke(this, new DataPresenterEventArgs());
        }

        public bool IsItemHighlighted(int index)
        {
            if (index < 0 || index >= _highlightedItems.Length)
                return false;
            return _highlightedItems[index];
        }

        public void ClearHighlights()
        {
            for (int i = 0; i < _highlightedItems.Length; i++)
                _highlightedItems[i] = false;
            DataUpdated?.Invoke(this, new DataPresenterEventArgs());
        }
        #endregion

        #endregion
    }
}
