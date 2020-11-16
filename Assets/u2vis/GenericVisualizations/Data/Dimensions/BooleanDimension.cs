using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace u2vis
{
    /// <summary>
    /// Class which represents a DataDimension that stores boolean values.
    /// </summary>
    [Serializable]
    public class BooleanDimension : DataDimension, INumericalDimension
    {
        #region Private Fields
        /// <summary>
        /// The boolean values of this DataDimension.
        /// </summary>
        [SerializeField]
        private List<bool> _values;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the boolean value at the specified index.
        /// </summary>
        /// <param name="index">The index for which the value should be gotten.</param>
        /// <returns>The value at the specified index.</returns>
        public bool this[int index] => _values[index];
        /// <summary>
        /// Gets the minium float value, which is always 0.0 for a BooleanDimension.
        /// </summary>
        public float MinimumFloatValue => 0.0f;
        /// <summary>
        /// Gets the maximum float value, which is always 1.0 for a BooleanDimension.
        /// </summary>
        public float MaximumFloatValue => 1.0f;
        /// <summary>
        /// Gets the number of items currently stored in this BooleanDimension.
        /// </summary>
        public override int Count => _values.Count;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of the BoleanDimension class.
        /// </summary>
        /// <param name="name">The name of this data dimension.</param>
        /// <param name="values">A list boolean of values that reprsent this data dimension. Can be null.</param>
        public BooleanDimension(string name, bool[] values)
            : base(name, DataType.Boolean)
        {
            if (values == null)
                _values = new List<bool>();
            else
                _values = new List<bool>(values);
        }
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
        /// Gets the boolean value at the specified index of this DataDimension.
        /// </summary>
        /// <param name="index">The index of the value that should be got.</param>
        /// <returns>The resulting boolean value.</returns>
        public bool GetBooleanValue(int index)
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
            if (_values[index])
                return 1.0f;
            return 0.0f;
        }
        /// <summary>
        /// Convertes the value stored at the specified index to a float which is normalized between 0 and 1 in relation to the minimum and maximum float value of this DataDimension.
        /// </summary>
        /// <param name="index">The index of value which should be converted.</param>
        /// <returns>A flot representing the converted value.</returns>
        public float ConvertToNormalizedFloat(int index)
        {
            return ConvertToFloat(index);
        }
        /// <summary>
        /// Add a new item to this DataDimension.
        /// </summary>
        /// <param name="value">The value that should be added.</param>
        public override void Add(object value)
        {
            if (!(value is bool))
                throw new ArgumentException("BoolDimension error: Added value is not of type bool!");
            Add((bool)value);
        }
        /// <summary>
        /// Add a new item to this DataDimension.
        /// </summary>
        /// <param name="value">The value that should be added.</param>
        public void Add(bool value)
        {
            _values.Add(value);
        }
        /// <summary>
        /// Set the item at the specified index to the specified value.
        /// </summary>
        /// <param name="index">The index at which the value should be set.</param>
        /// <param name="value">the value that should be set.</param>
        public override void Set(int index, object value)
        {
            if (!(value is bool))
                throw new ArgumentException("BoolDimension error: Updated value is not of type boolean!");
            Set(index, (bool)value);
        }
        /// <summary>
        /// Set the item at the specified index to the specified value.
        /// </summary>
        /// <param name="index">The index at which the value should be set.</param>
        /// <param name="value">the value that should be set.</param>
        public void Set(int index, bool value)
        {
            if (index < 0 || index >= _values.Count)
                throw new IndexOutOfRangeException("BooleanDimension error: Index out of Range");
            _values[index] = value;
        }
        /// <summary>
        /// Get a new Enumerator for this DataDimension.
        /// </summary>
        /// <returns>The new Enumerator.</returns>
        public override IEnumerator GetEnumerator()
        {
            return new BooleanDimensionEnumerator(this);
        }
        #endregion

        #region Enumerator
        /// <summary>
        /// Class representing a enumerator for the BooleanDimension.
        /// </summary>
        public class BooleanDimensionEnumerator : IEnumerator
        {
            /// <summary>
            /// Current position within the data dimension.
            /// </summary>
            private int pos = -1;
            /// <summary>
            /// The data dimension traversed by this enumerator.
            /// </summary>
            private BooleanDimension _dim;
            /// <summary>
            /// Gets the current value of the trversed data dimension.
            /// </summary>
            object IEnumerator.Current => Current;
            /// <summary>
            /// Gets the current value of the trversed data dimension.
            /// </summary>
            public bool Current => _dim._values[pos];
            /// <summary>
            /// Creates a new instance of the BooleanDimensionEnumerator class.
            /// </summary>
            /// <param name="dimension">The BooleanDimension traversed by this enumerator.</param>
            public BooleanDimensionEnumerator(BooleanDimension stringAttribute)
            {
                _dim = stringAttribute;
            }
            /// <summary>
            /// Move to the next item of the trversed data dimension.
            /// </summary>
            /// <returns>true if there is a next value, otherwiese false.</returns>
            public bool MoveNext()
            {
                pos++;
                return pos < _dim._values.Count;
            }
            /// <summary>
            /// Reset this enumerator so the data dimension can be traversed again.
            /// </summary>
            public void Reset()
            {
                pos = -1;
            }
        }
        #endregion
    }
}
