using UnityEngine;

namespace u2vis
{
    /// <summary>
    /// Class that represents a data presenter which can be used for visualizations that aggregate a number of dimensions on a single axis (like 3d bar charts or line charts).
    /// </summary>
    public class MultiDimDataPresenter : GenericDataPresenter
    {
        #region Private Fields
        /// <summary>
        /// Determines the index of a data dimension within the data set which serves as the basis for the labels and ticks for the x axis.
        /// </summary>
        [SerializeField]
        private int _selectedIndexForXAxis = -1;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the DataDimension which serves as the basis for the labels and ticks for the x axis.
        /// </summary>
        public DataDimension CaptionDimension
        {
            get { return _dataProvider.Data[_selectedIndexForXAxis]; }
            set { _selectedIndexForXAxis = _dataProvider.Data.IndexOf(value); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of the MultiDimDataPresenter class.
        /// </summary>
        protected MultiDimDataPresenter() : base()
        {
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Called by Unity once at the start of this script.
        /// </summary>
        protected override void Start()
        {
            base.Start();
        }
        /// <summary>
        /// Sets up the axis presenters array with their default values.
        /// </summary>
        protected override void SetupInitialAxisPresenters()
        {
            _axisPresenters = new AxisPresenter[3]
            {
                new AxisPresenter("X Axis"),
                new AxisPresenter("Y Axis"),
                new AxisPresenter("Z Axis")
            };
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
        /// <param name="dimIndexForAxisLabels">The index of a data dimension within the data set which serves as the basis for the labels and ticks for the x axis.</param>
        /// <param name="dimensionIndices">A list of indices of the DataDimensions within the DataSet which this DataPresenter should represent.</param>
        public virtual void Initialize(IDataProvider provider, int minIndex, int maxIndex, int dimIndexForAxisLabels, params int[] dimensionIndices)
        {
            _selectedIndexForXAxis = dimIndexForAxisLabels;
            base.Initialize(provider, minIndex, maxIndex, dimensionIndices);
        }
        #endregion
    }
}
