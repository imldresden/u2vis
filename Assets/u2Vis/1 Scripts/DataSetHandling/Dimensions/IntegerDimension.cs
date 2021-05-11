using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataSetHandling
{
    [Serializable]
    public class IntegerDimension : DataDimension, INumericalDimension
    {
        #region Private Fields
        /// <summary>
        /// The integer values stored in this data dimension.
        /// </summary>
        private List<int> _values;

        /// <summary>
        /// The minimum value stored in this data dimension.
        /// </summary>
        private float _minValue = float.MaxValue;

        /// <summary>
        /// The maximum value stored in this data dimension.
        /// </summary>
        private float _maxValue = float.MinValue;

        /// <summary>
        /// Indicates if the minimum and maximum values needs to be recalculated before the next access.
        /// </summary>
        private bool _needsRecalcMinMaxValues = true;
        #endregion

        #region Public Properties
        /// <summary>
        /// A float representing the minimum value stored in this DataDimension.
        /// </summary>
        public float MinimumFloatValue
        {
            get
            {
                if (_needsRecalcMinMaxValues)
                    RecalcMinMaxValues();
                return _minValue;
            }
        }

        /// <summary>
        /// A float representing the maximum value stored in this DataDimension.
        /// </summary>
        public float MaximumFloatValue
        {
            get
            {
                if (_needsRecalcMinMaxValues)
                    RecalcMinMaxValues();
                return _maxValue;
            }
        }

        /// <summary>
        /// Gets the integer value at the specified index.
        /// </summary>
        /// <param name="index">The index for which the value should be gotten.</param>
        /// <returns>The value at the specified index.</returns>
        public int this[int index] => _values[index];

        /// <summary>
        /// Gets the number of items currently stored in this DataDimension.
        /// </summary>
        public override int Count => _values.Count;
        #endregion

        #region Casting
        public static implicit operator List<int>(IntegerDimension dimension) => dimension._values.Select(v => v).ToList();

        public static implicit operator List<float>(IntegerDimension dimension) => dimension._values.Select((v, i) => dimension.ConvertToFloat(i)).ToList();
        #endregion

        #region Constructors
        public IntegerDimension(string name, int[] values)
            : base(name, DataType.Integer)
        {
            if (values == null)
                _values = new List<int>();
            else
                _values = new List<int>(values);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Recalculate the minium and maximum values of this data dimension.
        /// </summary>
        private void RecalcMinMaxValues()
        {
            _minValue = Single.MaxValue;
            _maxValue = Single.MinValue;
            for (int i = 0; i < _values.Count; i++)
            {
                if (_values[i] < _minValue)
                    _minValue = _values[i];
                if (_values[i] > _maxValue)
                    _maxValue = _values[i];
            }
            _needsRecalcMinMaxValues = false;
        }

        /// <summary>
        /// Internal method for the casting of an dimension to a list of objects.
        /// </summary>
        /// <returns>This dimension as a list of objects.</returns>
        protected override List<Object> _toObjectCasting() => _values.Select((v, i) => GetObjValue(i)).ToList();

        /// <summary>
        /// Internal method for the casting of an dimension to a list of strings.
        /// </summary>
        /// <returns>This dimension as a list of strings.</returns>
        protected override List<string> _toStringCasting() => _values.Select((v, i) => GetStringValue(i)).ToList();
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the value at the specified index of this DataDimension.
        /// </summary>
        /// <param name="index">The index of the value that should be got.</param>
        /// <returns>The resulting value.</returns>
        public override object GetObjValue(int index)
        {
            return _values[index];
        }

        /// <summary>
        /// Gets the integer value at the specified index of this DataDimension.
        /// </summary>
        /// <param name="index">The index of the value that should be got.</param>
        /// <returns>The resulting integer value.</returns>
        public int GetIntegerValue(int index)
        {
            return _values[index];
        }

        /// <summary>
        /// Convertes the value stored at the specified index to a float.
        /// </summary>
        /// <param name="index">The index of value which should be converted.</param>
        /// <returns>A flot representing the converted value.</returns>
        public float ConvertToFloat(int index)
        {
            return _values[index];
        }

        /// <summary>
        /// Convertes the value stored at the specified index to a float which is normalized between 0 and 1 in relation to the minimum and maximum float value of this DataDimension.
        /// </summary>
        /// <param name="index">The index of value which should be converted.</param>
        /// <returns>A flot representing the converted value.</returns>
        public float ConvertToNormalizedFloat(int index)
        {
            if (_needsRecalcMinMaxValues)
                RecalcMinMaxValues();
            return _values[index] / _maxValue;
        }

        /// <summary>
        /// Add a new item to this DataDimension.
        /// </summary>
        /// <param name="value">The value that should be added.</param>
        public override void Add(object value)
        {
            if (!(value is int))
                throw new ArgumentException("IntegerDimension error: Added value is not of type int!");
            Add((int)value);
        }

        /// <summary>
        /// Add a new item to this DataDimension.
        /// </summary>
        /// <param name="value">The value that should be added.</param>
        public void Add(int value)
        {
            _values.Add(value);
            if (value < _minValue)
                _minValue = value;
            else if (value > _maxValue)
                _maxValue = value;
        }

        /// <summary>
        /// Set the item at the specified index to the specified value.
        /// </summary>
        /// <param name="index">The index at which the value should be set.</param>
        /// <param name="value">the value that should be set.</param>
        public override void Set(int index, object value)
        {
            if (!(value is int))
                throw new ArgumentException("IntegerDimension error: Updated value is not of type int!");
            Set(index, (int)value);
        }

        /// <summary>
        /// Set the item at the specified index to the specified value.
        /// </summary>
        /// <param name="index">The index at which the value should be set.</param>
        /// <param name="value">the value that should be set.</param>
        public void Set(int index, int value)
        {
            if (index < 0 || index >= _values.Count)
                throw new IndexOutOfRangeException("IntegerDimension error: Index out of Range");
            _values[index] = value;
            if (value < _minValue)
                _minValue = value;
            else if (value > _maxValue)
                _maxValue = value;
        }

        /// <summary>
        /// Created a new instance of the DataDimension with an subset of rows than the original.
        /// </summary>
        /// <param name="rowIndex">The indicies of the rows to be present in the new DataDimension oject.</param>
        /// <returns>The DataDimension with a smaller subset.</returns>
        public override DataDimension GenerateSubSetDataDimension(params int[] rowIndices)
        {
            return new IntegerDimension(
                name: this.Name,
                values: _values.Where((v, i) => rowIndices.Contains(i)).ToArray()
            );
        }

        /// <summary>
        /// Get a new Enumerator for this DataDimension.
        /// </summary>
        /// <returns>The new Enumerator.</returns>
        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Enumerator
        /// <summary>
        /// Class representing a enumerator for the IntegerDimension.
        /// </summary>
        public class IntegerDimensionEnumerator : IEnumerator
        {
            /// <summary>
            /// Current position within the data dimension.
            /// </summary>
            private int _pos = -1;

            /// <summary>
            /// The data dimension traversed by this enumerator.
            /// </summary>
            private IntegerDimension _dim;

            /// <summary>
            /// Gets the current value of the trversed data dimension.
            /// </summary>
            object IEnumerator.Current => Current;

            /// <summary>
            /// Gets the current value of the trversed data dimension.
            /// </summary>
            public int Current => _dim._values[_pos];

            /// <summary>
            /// Creates a new instance of the IntegerDimensionEnumerator class.
            /// </summary>
            /// <param name="dimension">The IntegerDimension traversed by this enumerator.</param>
            public IntegerDimensionEnumerator(IntegerDimension stringAttribute)
            {
                _dim = stringAttribute;
            }
            /// <summary>
            /// Move to the next item of the trversed data dimension.
            /// </summary>
            /// <returns>true if there is a next value, otherwiese false.</returns>
            public bool MoveNext()
            {
                _pos++;
                return _pos < _dim._values.Count;
            }

            /// <summary>
            /// Reset this enumerator so the data dimension can be traversed again.
            /// </summary>
            public void Reset()
            {
                _pos = -1;
            }
        }
        #endregion
    }
}
