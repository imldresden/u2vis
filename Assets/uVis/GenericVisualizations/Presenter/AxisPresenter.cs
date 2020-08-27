using System;
using System.Globalization;
using UnityEngine;

namespace UVis
{
    [Serializable]
    public class AxisPresenter
    {
        #region Protected Fields
        [SerializeField]
        protected string _caption = "none";
        [SerializeField]
        protected LabelOrientation _labelOrientation = LabelOrientation.Parallel;
        [SerializeField]
        protected bool _isCategorical = false;
        [SerializeField]
        protected float _tickIntervall = 0.25f;
        [SerializeField]
        protected int _labelTickIntervall = 1;
        [SerializeField]
        protected int _decimalPlaces = 2;
        #endregion

        #region Pulic Properties
        public string Caption
        {
            get { return _caption; }
            set { _caption = value; }
        }
        public LabelOrientation LabelOrientation
        {
            get { return _labelOrientation; }
            set { _labelOrientation = value; }
        }
        public bool IsCategorical
        {
            get { return _isCategorical; }
            set { _isCategorical = value; }
        }
        public float TickIntervall
        {
            get { return _tickIntervall; }
            set { _tickIntervall = value; }
        }
        public int LabelTickIntervall
        {
            get { return _labelTickIntervall; }
            set { _labelTickIntervall = value; }
        }
        #endregion

        #region Constructors
        public AxisPresenter()
        {
        }

        public AxisPresenter(string caption)
        {
            _caption = caption;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Generates labels from the values of a single data dimension.
        /// The method will select automatically if the axis is numerical or categorical.
        /// If the label intervall was set to zero, all numerical axes will be threated as categorical.
        /// </summary>
        /// <param name="dataDim">The data dimension from which to generate the labels.</param>
        /// <param name="minItem">At which item index the axis should start. Detault is 0.</param>
        /// <param name="maxItem">At which item index the axis should end. A value of -1 means all of them. Default value is -1.</param>
        /// <returns>The generated list of axis ticks.</returns>
        public virtual AxisTick[] GenerateFromDimension(DataDimension dataDim, int minItem = 0, int maxItem = -1)
        {
            _labelTickIntervall = Mathf.Max(1, _labelTickIntervall);
            if (dataDim is INumericalDimension numDim && _tickIntervall > 0 && !_isCategorical)
            {
                // how many ticks can we fit between 0.0f and 1.0f given the tick intervall
                // if the calculation is converted directly to int in one command, there seem to be sometimes imprecision errors. Need to investigate further.
                float t = 1.0f / _tickIntervall;
                int tickCount = (int)t;
                return GenerateNumerical(0, numDim.MaximumFloatValue, tickCount);
            }
            else
                return GenerateCategorical(dataDim, minItem, maxItem);
        }
        /// <summary>
        /// Generates Axis Labels from the Captions of multiple data dimensions.
        /// NOTE: This will ignore the tick intervall value. Label intervall will still apply.
        /// </summary>
        /// <param name="dataPresenter">The data presenter which holds the data dimensions from which the axis labels should be created.</param>
        /// <param name="start">At which dimension the axis should start. Default value is 0.</param>
        /// <param name="end">At which dimension the axis should end. A value of -1 means all of them. Default value is -1.</param>
        /// <returns>The generated list of axis ticks.</returns>
        public virtual AxisTick[] GenerateFromDimensionCaptions(GenericDataPresenter dataPresenter, int start = 0, int end = -1)
        {
            _labelTickIntervall = Mathf.Max(1, _labelTickIntervall);
            if (end < 0)
                end = dataPresenter.NumberOfDimensions;
            int length = end - start;
            if (length < 0)
            {
                Debug.LogError(start + " and " + end + " are no valid number range");
                return null;
            }
            var ticks = new AxisTick[length];
            float step = 1.0f / length;
            for (int i = 0; i < length; i++)
            {
                float pos = (i + 1) * step - (0.5f * step);
                string label = null;
                if ((i + 1) % _labelTickIntervall == 0)
                    label = dataPresenter[i + start].Name;
                ticks[i] = new AxisTick(pos, label);
            }
            return ticks;
        }
        /// <summary>
        /// Generates axis labels from the given a minmum and maximum value.
        /// Ignores current value of IsCategorical.
        /// </summary>
        /// <param name="minValue">The minimum value from which the axis labels should be generated.</param>
        /// <param name="maxValue">The maximum value from which the axis labels should be generated.</param>
        /// <returns>The generated list of axis ticks.</returns>
        public virtual AxisTick[] GenerateFromMinMaxValue(float minValue, float maxValue)
        {
            if (_tickIntervall <= 0)
                return null;
            _labelTickIntervall = Mathf.Min(1, _labelTickIntervall);
            float t = 1.0f / _tickIntervall;
            int tickCount = (int)t;
            return GenerateNumerical(minValue, maxValue, tickCount);
        }
        /// <summary>
        /// Generates categorical integer axis labels between the given range.
        /// Ignores TickIntervall and IsCategorical.
        /// </summary>
        /// <param name="start">The caption and number of the first label.</param>
        /// <param name="end">The caption and number of the last label.</param>
        /// <returns>The generated list of axis ticks.</returns>
        public virtual AxisTick[] GenerateFromDiscreteRange(int start, int end)
        {
            return GenerateCategorical(null, start, end);
        }
        #endregion

        #region Protected Methods
        protected virtual AxisTick[] GenerateNumerical(float min, float max, int tickCount)
        {
            var ticks = new AxisTick[tickCount];
            float range = max - min;
            for (int i = 0; i < tickCount; i++)
            {
                // i+1 because we wont exclude zero but include values exactly at 1
                float value = (i + 1) * _tickIntervall;
                string label = null;
                // again i+1 to check if we need a label
                if ((i + 1) % _labelTickIntervall == 0)
                    //label = Math.Round(range * value + min, _decimalPlaces).ToString();
                    label = string.Format(new NumberFormatInfo() { NumberDecimalDigits = _decimalPlaces }, "{0:F}", range * value + min);
                ticks[i] = new AxisTick(value, label);

                
            }
            return ticks;
        }

        protected virtual AxisTick[] GenerateCategorical(DataDimension dim, int start, int end)
        {
            int length = end - start;
            if (length < 0)
            {
                Debug.LogError(start + " and " + end + "are no valid number range");
                return null;
            }
            var ticks = new AxisTick[length];
            float step = 1.0f / length;
            for (int i = 0; i < length; i++)
            {
                // i+1 because we wont exclude zero but include values exactly at 1
                float pos = (i + 1) * step - (0.5f * step);
                string label = null;
                // again i+1 to check if we need a label
                if ((i + 1) % _labelTickIntervall == 0)
                {
                    if (dim != null)
                        label = dim.GetStringValue(i + start);
                    else
                        label = (i + start).ToString();
                }
                ticks[i] = new AxisTick(pos, label);
            }
            return ticks;
        }
        #endregion
    }
}
