using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UVis
{
    [Serializable]
    public class StringDimension : DataDimension
    {
        #region Private Fields
        [SerializeField]
        private List<string> _values;
        #endregion

        #region Public Properties
        public string this[int index] => _values[index];
        
        public override int Count => _values.Count;
        #endregion

        #region Constrcutor
        public StringDimension(string name, string[] values)
            : base(name, DataType.String)
        {
            if (_values == null)
                _values = new List<string>();
            else
                _values = new List<string>(values);
        }
        #endregion

        #region Public Properties
        public override object GetObjValue(int index)
        {
            return _values[index];
        }

        public override string GetStringValue(int index)
        {
            return _values[index];
        }

        public override void Add(object value)
        {
            if (!(value is string))
                throw new ArgumentException("StringDimension error: Added value is not of type string!");
            Add((string)value);
        }

        public void Add(string value)
        {
            _values.Add(value);
        }

        public override IEnumerator GetEnumerator()
        {
            return new StringDimensionEnumerator(this);
        }
        #endregion

        #region Enumerator
        public class StringDimensionEnumerator : IEnumerator
        {
            private int pos = -1;

            private StringDimension _dim;

            object IEnumerator.Current => Current;

            public string Current
            {
                get { return _dim._values[pos]; }
            }

            public StringDimensionEnumerator(StringDimension stringAttribute)
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
