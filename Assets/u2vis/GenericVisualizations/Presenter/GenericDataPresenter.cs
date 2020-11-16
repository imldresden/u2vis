using System;
using UnityEngine;

namespace u2vis
{
    /// <summary>
    /// Delegate which indicates that the highlight at a speicifc index of a DataPresenter has changed.
    /// </summary>
    /// <param name="sender">The sender which raised this event.</param>
    /// <param name="itemIndex">The index which has changed.</param>
    public delegate void HighlightChangedHandler(GenericDataPresenter sender, int itemIndex);
    /// <summary>
    /// Class that represents a generic data presenter for all kinds of visualizations that support one axis per dimension (like 2D bar charts and scatterplots).
    /// </summary>
    public class GenericDataPresenter : MonoBehaviour
    {
        /// <summary>
        /// The default index of a data dimension within the data set. Used for the dimension indices array.
        /// </summary>
        public const int DEFAULT_INDEX = 0;

        #region Protected Fields
        /// <summary>
        /// The DataProvider which provides the DataSet used ba this DataPresenter.
        /// </summary>
        [SerializeField]
        protected AbstractDataProvider _dataProvider = null;
        /// <summary>
        /// The currently selected minimum item index within the data set. Default is 0.
        /// </summary>
        [SerializeField]
        protected int _selectedMinItem = 0;
        /// <summary>
        /// The currently selected maximum item index within the data set. Default is 0.
        /// </summary>
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
        /// <summary>
        /// Indicates if this DataPresenter has ben initialized or not.
        /// </summary>
        protected bool _isInitialized = false;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the DataProvider which provides the DataSet used by this DataPresenter.
        /// </summary>
        public AbstractDataProvider DataProvider => _dataProvider;
        /// <summary>
        /// Returns a list of all data dimensions.
        /// WARNING: Not very efficient, use the index property instead.
        /// </summary>
        public DataDimension[] Dimensions => GetDimensionsArray();
        /// <summary>
        /// Gets the number of data dimensions provided by this DataPresenter.
        /// </summary>
        public int NumberOfDimensions => _dataDimensionsIndices.Length;
        /// <summary>
        /// Gets the total number of items contained within the DataSet used by this DataPresenter.
        /// </summary>
        public int TotalItemCount => NumberOfDimensions == 0 ? 0 : this[0].Count;
        /// <summary>
        /// Gets the number of items currently selected.
        /// </summary>
        public int SelectedItemsCount => _selectedMaxItem - _selectedMinItem;
        /// <summary>
        /// Gets or sets the minimum index of the currently selected items.
        /// If the value is large than SelectedMaxIndex, both values are switched.
        /// </summary>
        public int SelectedMinItem
        {
            get { return _selectedMinItem; }
            set { SetSelectedItemIndices(value, _selectedMaxItem); }
        }
        /// <summary>
        /// Gets or sets the maximum index if the currently selected items.
        /// If the value is lower than SelectedMinIndex, both values are switched.
        /// </summary>
        public int SelectedMaxItem
        {
            get { return _selectedMaxItem; }
            set { SetSelectedItemIndices(_selectedMinItem, value); }
        }
        /// <summary>
        /// Gets the DataDimension at the specified index out of the dimensions this DataPresenter provides.
        /// Note: This is not the index within the DataSet, but within the DataPresenter.
        /// </summary>
        /// <param name="index">The index for which the DataDimension should be gotten.</param>
        /// <returns>the reulting DataDimension.</returns>
        public DataDimension this[int index] => _dataProvider.Data[_dataDimensionsIndices[index]];
        /// <summary>
        /// Gets the List of AxisPresenters defined for this DataPresenter.
        /// </summary>
        // TODO: return ReadOnly list instead of real array
        public AxisPresenter[] AxisPresenters => _axisPresenters;
        #endregion

        #region Events
        /// <summary>
        /// Raised whenever the data of the DataPresenter was updates, e.g., when the selected indices have changed or the underlying DataSet was updated.
        /// </summary>
        public event EventHandler<DataPresenterEventArgs> DataUpdated;
        /// <summary>
        /// Raised whenever the highlight of a specific index has changed.
        /// </summary>
        public event HighlightChangedHandler HighlightChanged;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of the GenericDataPresenter class.
        /// </summary>
        protected GenericDataPresenter()
        {
            _dataDimensionsIndices = new int[1];
            _dataDimensionsIndices[0] = DEFAULT_INDEX;
            SetupInitialAxisPresenters();
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Called by unity once at the start of this script.
        /// </summary>
        protected virtual void Start()
        {
            // in case the provider was set in the editor, the event handlers and non editor variables have to be initialized seperately
            if (!_isInitialized)
            {
                _dataProvider.DataChanged += Provider_DataUpdated;
                _highlightedItems = new bool[TotalItemCount];
            }
        }
        /// <summary>
        /// Generates an array of DataDimensions that this DataPresenter provides.
        /// Since the provider only stores indices to the DataSet instead of actual references,
        /// this list needs to be generated by accessing the DataSet.
        /// </summary>
        /// <returns>The resulting array of DataDimensions.</returns>
        protected virtual DataDimension[] GetDimensionsArray()
        {
            var d = new DataDimension[_dataDimensionsIndices.Length];
            for (int i = 0; i<_dataDimensionsIndices.Length; i++)
                d[i] = _dataProvider.Data[_dataDimensionsIndices[i]];
            return d;
        }
        /// <summary>
        /// Sets the axis presenters with initial values, assuming each data dimension has its own axis.
        /// </summary>
        protected virtual void SetupInitialAxisPresenters()
        {
            if (_dataProvider == null)
                return;
            _axisPresenters = new AxisPresenter[NumberOfDimensions];
            for (int i = 0; i < NumberOfDimensions; i++)
                _axisPresenters[i] = new AxisPresenter(this[i].Name);
        }
        /// <summary>
        /// Called whenever the data of the DataProvider has changed.
        /// </summary>
        /// <param name="sender">The DataProvider which data has changed.</param>
        /// <param name="e">The event arguments.</param>
        protected virtual void Provider_DataUpdated(object sender, EventArgs e)
        {
            if (_highlightedItems.Length != _dataProvider.Data.NumOfItems)
                _highlightedItems = new bool[_dataProvider.Data.NumOfItems];
            //TODO: Check dataSet dimensions, items count, etc.
            DataUpdated?.Invoke(this, new DataPresenterEventArgs(_selectedMinItem, _selectedMaxItem));
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initialize this DataPresenter and generate axis descriptions, etc.
        /// Can be called multiple times to reset the DataPresenter.
        /// </summary>
        /// <param name="provider">The DataProvider that provides the DataSet used for this DataPresenter.</param>
        /// <param name="minIndex">The minimum index of the items within the DataSet which this DataPresenter represents.</param>
        /// <param name="maxIndex">The maximum index of the items within the DataSet which this DataPresenter represents.</param>
        /// <param name="dimensionIndices">A list of indices of the DataDimensions within the DataSet which this DataPresenter should represent.</param>
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
        /// <summary>
        /// Set the minimum and maximum selected indices of the DataSet this DataPresneter represents.
        /// Note: If minimum and maximum are switched if one is larger/lower than the other.
        /// </summary>
        /// <param name="min">The new minimum selected index.</param>
        /// <param name="max">The new maximum selected index.</param>
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
        /// <summary>
        /// Reset the axis properties to their default values.
        /// </summary>
        public virtual void ResetAxisProperties()
        {
            SetupInitialAxisPresenters();
        }

        #region Highlights
        /// <summary>
        /// Toggles the highlight flag for the specified index.
        /// </summary>
        /// <param name="index">The index for which the highlight should be toggled.</param>
        public virtual void ToogleItemHighlight(int index)
        {
            _highlightedItems[index] = !_highlightedItems[index];
            HighlightChanged?.Invoke(this, index);
            if (index >= _selectedMinItem || index < _selectedMaxItem)
                DataUpdated?.Invoke(this, new DataPresenterEventArgs());
        }
        /// <summary>
        /// Indicates if the item at the specified index is currently highlightet or not.
        /// </summary>
        /// <param name="index">The index for which the highlight state should be checked.</param>
        /// <returns>true if the item is currently highlighted, otherwise false.</returns>
        public bool IsItemHighlighted(int index)
        {
            if (index < 0 || index >= _highlightedItems.Length)
                return false;
            return _highlightedItems[index];
        }
        /// <summary>
        /// Clear the highlight flags for all indices.
        /// </summary>
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
