using UnityEngine;

namespace u2vis
{
    public class MultiDimDataPresenter : GenericDataPresenter
    {
        [SerializeField]
        private int _selectedIndexForXAxis = -1;
        public DataDimension CaptionDimension
        {
            get { return _dataProvider.Data[_selectedIndexForXAxis]; }
            set { _selectedIndexForXAxis = _dataProvider.Data.IndexOf(value); }
        }

        protected MultiDimDataPresenter() : base()
        {
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void SetupInitialAxisPresenters()
        {
            _axisPresenters = new AxisPresenter[3]
            {
                new AxisPresenter("X Axis"),
                new AxisPresenter("Y Axis"),
                new AxisPresenter("Z Axis")
            };
        }

        public virtual void Initialize(IDataProvider provider, int minIndex, int maxIndex, int dimIndexForAxisLabels, params int[] dimensionIndices)
        {
            _selectedIndexForXAxis = dimIndexForAxisLabels;
            base.Initialize(provider, minIndex, maxIndex, dimensionIndices);
        }
    }
}
