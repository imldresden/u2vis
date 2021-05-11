using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataSetHandling
{
    [Serializable]
    public class StringDimension : DataDimension
    {
        #region Private Fields
        /// <summary>
        /// The string values of this DataDimension.
        /// </summary>
        private List<string> _values;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the string value at the specified index.
        /// </summary>
        /// <param name="index">The index for which the value should be gotten.</param>
        /// <returns>The value at the specified index.</returns>
        public string this[int index] => _values[index];

        /// <summary>
        /// Gets the number of items currently stored in this DataDimension.
        /// </summary>
        public override int Count => _values.Count;
        #endregion

        #region Constrcutor
        /// <summary>
        /// Creates a new instance of the StringDimension class.
        /// </summary>
        /// <param name="name">The name of this data dimension.</param>
        /// <param name="values">A list string of values that reprsent this data dimension. Can be null.</param>
        public StringDimension(string name, string[] values)
            : base(name, DataType.String)
        {
            if (values == null)
                _values = new List<string>();
            else
                _values = new List<string>(values);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Internal method for the casting of an dimension to a list of objects.
        /// </summary>
        /// <returns>This dimension as a list of objects.</returns>
        protected override List<Object> _toObjectCasting() => _values.Select((v, i) => GetObjValue(i)).ToList();

        /// <summary>
        /// Internal method for the casting of an dimension to a list of strings.
        /// </summary>
        /// <returns>This dimension as a list of strings.</returns>
        protected override List<string> _toStringCasting() => _values.Select(v => v).ToList();
        #endregion

        #region Public Properties
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
        /// Gets the string value at the specified index of this DataDimension.
        /// </summary>
        /// <param name="index">The index of the value that should be got.</param>
        /// <returns>The resulting string value.</returns>
        public override string GetStringValue(int index)
        {
            return _values[index];
        }

        /// <summary>
        /// Add a new item to this DataDimension.
        /// </summary>
        /// <param name="value">The value that should be added.</param>
        public override void Add(object value)
        {
            if (!(value is string))
                throw new ArgumentException("StringDimension error: Added value is not of type string!");
            Add((string)value);
        }

        /// <summary>
        /// Add a new item to this DataDimension.
        /// </summary>
        /// <param name="value">The value that should be added.</param>
        public void Add(string value)
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
            if (!(value is string))
                throw new ArgumentException("StringDimension error: Updated value is not of type string!");
            Set(index, (string)value);
        }

        /// <summary>
        /// Set the item at the specified index to the specified value.
        /// </summary>
        /// <param name="index">The index at which the value should be set.</param>
        /// <param name="value">the value that should be set.</param>
        public void Set(int index, string value)
        {
            if (index < 0 || index >= _values.Count)
                throw new IndexOutOfRangeException("StringDimension error: Index out of Range");
            _values[index] = value;
        }

        /// <summary>
        /// Created a new instance of the DataDimension with an subset of rows than the original.
        /// </summary>
        /// <param name="rowIndex">The indicies of the rows to be present in the new DataDimension oject.</param>
        /// <returns>The DataDimension with a smaller subset.</returns>
        public override DataDimension GenerateSubSetDataDimension(params int[] rowIndices)
        {
            return new StringDimension(
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
            return new StringDimensionEnumerator(this);
        }
        #endregion

        #region Enumerator
        /// <summary>
        /// Class representing a enumerator for the StringDimension.
        /// </summary>
        public class StringDimensionEnumerator : IEnumerator
        {
            /// <summary>
            /// Current position within the data dimension.
            /// </summary>
            private int _pos = -1;

            /// <summary>
            /// The data dimension traversed by this enumerator.
            /// </summary>
            private StringDimension _dim;

            /// <summary>
            /// Gets the current value of the trversed data dimension.
            /// </summary>
            object IEnumerator.Current => Current;

            /// <summary>
            /// Gets the current value of the trversed data dimension.
            /// </summary>
            public string Current => _dim._values[_pos];

            /// <summary>
            /// Creates a new instance of the StringDimensionEnumerator class.
            /// </summary>
            /// <param name="dimension">The StringDimension traversed by this enumerator.</param>
            public StringDimensionEnumerator(StringDimension dimension)
            {
                _dim = dimension;
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
