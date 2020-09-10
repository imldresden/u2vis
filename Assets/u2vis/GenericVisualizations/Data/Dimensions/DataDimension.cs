using System;
using System.Collections;
using UnityEngine;

namespace UVis
{
    [Serializable]
    public abstract class DataDimension : IEnumerable
    {
        #region Protected Fields
        [SerializeField]
        protected string _name = null;
        [SerializeField]
        protected DataType _dataType = DataType.Undefinded;
        #endregion

        #region Public Properties
        public string Name => _name;
        public DataType DataType => _dataType;
        public abstract int Count { get; }
        #endregion

        #region Constructors
        protected DataDimension(string name, DataType dataType)
        {
            _name = name;
            _dataType = dataType;
        }
        #endregion

        #region Public Methods
        public abstract object GetObjValue(int index);

        public virtual bool GetBooleanValue(int index)
        {
            throw new InvalidOperationException("This DataDimension does not support boolean values, check DataType first!");
        }

        public virtual int GetIntegerValue(int index)
        {
            throw new InvalidOperationException("This DataDimension does not support integer values, check DataType first!");
        }

        public virtual float GetFloatValue(int index)
        {
            throw new InvalidOperationException("This DataDimension does not support flooting point values, check DataType first!");
        }

        public virtual string GetStringValue(int index)
        {
            return GetObjValue(index).ToString();
        }

        public abstract void Add(object value);

        public abstract void Set(int index, object value);

        public abstract IEnumerator GetEnumerator();
        #endregion
    }
}
