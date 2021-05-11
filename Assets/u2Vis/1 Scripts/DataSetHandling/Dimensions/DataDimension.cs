using System;
using System.Collections;
using System.Collections.Generic;

namespace DataSetHandling
{
    /// <summary>
    /// The base class for all DataDimensions.
    /// </summary>
    [Serializable]
    public abstract class DataDimension : IEnumerable
    {
        #region Protected Fields
        /// <summary>
        /// The name of this data dimension.
        /// </summary>
        protected string _name = null;

        /// <summary>
        /// The type of data stored on this data dimension.
        /// </summary>
        protected DataType _dataType = DataType.Undefinded;
        #endregion

        #region Public Properties
        /// <summary>
        /// The name of this data dimension.
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// The type of data stored in this data dimension.
        /// </summary>
        public DataType DataType => _dataType;

        /// <summary>
        /// The number of items contained in this data dimension.
        /// </summary>
        public abstract int Count { get; }
        #endregion

        #region Casting
        public static implicit operator List<string>(DataDimension dimension) => dimension._toStringCasting();

        public static implicit operator List<object>(DataDimension dimension) => dimension._toObjectCasting();

        public override string ToString() => $"({Name} | {DataType}): {string.Join(", ", (List<string>)this)}";
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of the DataDimension.
        /// </summary>
        /// <param name="name">The name of this DataDimension.</param>
        /// <param name="dataType">The type of data stored in this DataDimension.</param>
        protected DataDimension(string name, DataType dataType)
        {
            _name = name;
            _dataType = dataType;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Internal method for the casting of an dimension to a list of objects.
        /// </summary>
        /// <returns>This dimension as a list of objects.</returns>
        protected abstract List<Object> _toObjectCasting();

        /// <summary>
        /// Internal method for the casting of an dimension to a list of strings.
        /// </summary>
        /// <returns>This dimension as a list of strings.</returns>
        protected abstract List<string> _toStringCasting();
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the value at the specified index of this DataDimension.
        /// </summary>
        /// <param name="index">The index of the value that should be got.</param>
        /// <returns>The resulting value.</returns>
        /// 
        public abstract object GetObjValue(int index);

        /// <summary>
        /// Get a string representation of the value at the specified index of this DataDimension.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual string GetStringValue(int index)
        {
            return GetObjValue(index).ToString();
        }

        /// <summary>
        /// Add a new item to this DataDimension.
        /// </summary>
        /// <param name="value">The value that should be added.</param>
        public abstract void Add(object value);

        /// <summary>
        /// Set the item at the specified index to the specified value.
        /// </summary>
        /// <param name="index">The index at which the value should be set.</param>
        /// <param name="value">the value that should be set.</param>
        public abstract void Set(int index, object value);

        /// <summary>
        /// Get a new Enumerator for this DataDimension.
        /// </summary>
        /// <returns>The new Enumerator.</returns>
        public abstract IEnumerator GetEnumerator();

        /// <summary>
        /// Created a new instance of the DataDimension with an subset of rows than the original.
        /// </summary>
        /// <param name="dimension">The original DataDimenstion to use as a base.</param>
        /// <param name="rowIndex">The indicies of the rows to be present in the new DataDimension oject.</param>
        /// <returns>The DataDimension with a smaller subset.</returns>
        public abstract DataDimension GenerateSubSetDataDimension(params int[] rowIndices);
        #endregion
    }
}
