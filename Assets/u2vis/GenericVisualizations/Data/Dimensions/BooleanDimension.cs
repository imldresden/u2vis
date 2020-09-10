using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UVis
{
    [Serializable]
    public class BooleanDimension : DataDimension, INumericalDimension
    {
        #region Private Fields
        [SerializeField]
        private List<bool> _values;
        #endregion

        #region Public Properties
        public bool this[int index] => _values[index];

        public float MinimumFloatValue => 0.0f;

        public float MaximumFloatValue => 1.0f;

        public override int Count => _values.Count;
        #endregion

        #region Constructors
        public BooleanDimension(string caption, bool[] values)
            : base(caption, DataType.Boolean)
        {
            if (values == null)
                _values = new List<bool>();
            else
                _values = new List<bool>(values);
        }
        #endregion

        #region Public Methods
        public override object GetObjValue(int index)
        {
            return _values[index];
        }

        public override bool GetBooleanValue(int index)
        {
            return _values[index];
        }

        public float ConvertToFloat(int index)
        {
            if (_values[index])
                return 1.0f;
            return 0.0f;
        }

        public float ConvertToNormalizedFloat(int index)
        {
            return ConvertToFloat(index);
        }

        public override void Add(object value)
        {
            if (!(value is bool))
                throw new ArgumentException("BoolDimension error: Added value is not of type bool!");
            Add((bool)value);
        }

        public void Add(bool value)
        {
            _values.Add(value);
        }

        public override void Set(int index, object value)
        {
            if (!(value is bool))
                throw new ArgumentException("BoolDimension error: Updated value is not of type boolean!");
            Set(index, (bool)value);
        }

        public void Set(int index, bool value)
        {
            if (index < 0 || index >= _values.Count)
                throw new IndexOutOfRangeException("BooleanDimension error: Index out of Range");
            _values[index] = value;
        }

        public override IEnumerator GetEnumerator()
        {
            return new BooleanDimensionEnumerator(this);
        }
        #endregion

        #region Enumerator
        public class BooleanDimensionEnumerator : IEnumerator
        {
            private int pos = -1;

            private BooleanDimension _dim;

            object IEnumerator.Current => Current;

            public bool Current
            {
                get { return _dim._values[pos]; }
            }

            public BooleanDimensionEnumerator(BooleanDimension stringAttribute)
            {
                _dim = stringAttribute;
            }

            public bool MoveNext()
            {
                pos++;
                return pos < _dim._values.Count;
            }

            public void Reset()
            {
                pos = -1;
            }
        }
        #endregion
    }
}
