using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UVis
{
    [Serializable]
    public class FloatDimension : DataDimension, INumericalDimension
    {
        #region Private Fields
        [SerializeField]
        private List<float> _values;
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
        public float this[int index] => _values[index];
        public override int Count => _values.Count;
        #endregion

        #region Constructors
        public FloatDimension(string name, float[] values)
            : base(name, DataType.Float)
        {
            if (values == null)
                _values = new List<float>();
            else
                _values = new List<float>(values);
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

        public override float GetFloatValue(int index)
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

        public int GetLength()
        {
            return _values.Count;
        }

        public override void Add(object value)
        {
            if (!(value is float))
                throw new ArgumentException("FloatDimension error: Added value is not of type float!");
            Add((float)value);
        }

        public void Add(float value)
        {
            _values.Add(value);
            _needsRecalcMinMaxValues = true;
        }

        public override IEnumerator GetEnumerator()
        {
            return new FloatDimensionEnumerator(this);
        }
        #endregion

        #region Enumerator
        public class FloatDimensionEnumerator : IEnumerator
        {
            private int pos = -1;

            private FloatDimension _dim;

            object IEnumerator.Current => Current;

            public float Current
            {
                get { return _dim._values[pos]; }
            }

            public FloatDimensionEnumerator(FloatDimension stringAttribute)
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
