using System;
using System.Collections;
using UnityEngine;

namespace u2vis
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
        [SerializeField]
        protected string _name = null;
        /// <summary>
        /// The type of data stored on this data dimension.
        /// </summary>
        [SerializeField]
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

        #region Constructors
        /// <summary>
        /// Creates a new instance of the DataDimension,
        /// </summary>
        /// <param name="name">The name of this DataDimension.</param>
        /// <param name="dataType">The type of data stored in this DataDimension.</param>
        protected DataDimension(string name, DataType dataType)
        {
            _name = name;
            _dataType = dataType;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the value at the specified index of this DataDimension.
        /// </summary>
        /// <param name="index">The index of the value that should be got.</param>
        /// <returns>The resulting value.</returns>
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
        #endregion
    }
}
