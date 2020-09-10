using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace u2vis
{
    [Serializable]
    public class IntegerDimension : DataDimension, INumericalDimension
    {
        #region Private Fields
        [SerializeField]
        private List<int> _values;
        [SerializeField]
        private float _minValue = float.MaxValue;
        [SerializeField]
        private float _maxValue = float.MinValue;
        [SerializeField]
        private bool _needsRecalcMinMaxValues = true;
        #endregion

        #region Public Properties
        public float MinimumFloatValue
        {
            get
            {
                if (_needsRecalcMinMaxValues)
                    RecalcMinMaxValues();
                return _minValue;
            }
        }
        public float MaximumFloatValue
        {
            get
            {
                if (_needsRecalcMinMaxValues)
                    RecalcMinMaxValues();
                return _maxValue;
            }
        }

        public int this[int index] => _values[index];
        public override int Count => _values.Count;
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
        #endregion

        #region Public Methods
        public override object GetObjValue(int index)
        {
            return _values[index];
        }

        public override int GetIntegerValue(int index)
        {
            return _values[index];
        }

        public float ConvertToFloat(int index)
        {
            return _values[index];
        }

        public float ConvertToNormalizedFloat(int index)
        {
            if (_needsRecalcMinMaxValues)
                RecalcMinMaxValues();
            return _values[index] / _maxValue;
        }

        public override void Add(object value)
        {
            if (!(value is int))
                throw new ArgumentException("IntegerDimension error: Added value is not of type int!");
            Add((int)value);
        }

        public void Add(int value)
        {
            _values.Add(value);
            if (value < _minValue)
                _minValue = value;
            else if (value > _maxValue)
                _maxValue = value;
        }

        public override void Set(int index, object value)
        {
            if (!(value is int))
                throw new ArgumentException("IntegerDimension error: Updated value is not of type int!");
            Set(index, (int)value);
        }

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

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Enumerator
        public class IntegerDimensionEnumerator : IEnumerator
        {
            private int pos = -1;

            private IntegerDimension _dim;

            object IEnumerator.Current => Current;

            public int Current
            {
                get { return _dim._values[pos]; }
            }

            public IntegerDimensionEnumerator(IntegerDimension stringAttribute)
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
