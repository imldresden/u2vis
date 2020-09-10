using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace u2vis
{
    public class DataSet : IEnumerable<DataDimension>
    {
        #region Private Fields
        private List<DataDimension> _dimensions;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the data dimension with the specified index.
        /// </summary>
        /// <param name="index">The index of the data dimension.</param>
        /// <returns>The data dimension with the specified index.</returns>
        public DataDimension this[int index] => _dimensions[index];
        /// <summary>
        /// Gets the number of data dimensions.
        /// </summary>
        public int Count => _dimensions.Count;
        /// <summary>
        /// Gets the number of items a data dimension contains.
        /// </summary>
        public int NumOfItems => GetNumberOfItems();
        #endregion

        #region Constructor
        public DataSet()
        {
            _dimensions = new List<DataDimension>();
        }

        public DataSet(params DataDimension[] dimensions)
        {
            _dimensions = new List<DataDimension>(dimensions);
        }
        #endregion

        #region Private Methods
        private int GetNumberOfItems()
        {
            if (_dimensions.Count > 1)
                return _dimensions[0].Count;
            return 0;
        }
        #endregion

        #region Public Methods
        public void Add(DataDimension dimension)
        {
            if (_dimensions.Contains(dimension))
                throw new ArgumentException("Dimension is already part of the data set!");
            _dimensions.Add(dimension);
        }
        /// <summary>
        /// Removes a data dimension from the data set.
        /// </summary>
        /// <param name="dimension">The data dimension which should be removed.</param>
        /// <returns>true if the dimension was removed, otherwiese false.</returns>
        public bool Remove(DataDimension dimension)
        {
            return _dimensions.Remove(dimension);
        }
        /// <summary>
        /// Gets the index of the specified data dimension within the data set.
        /// </summary>
        /// <param name="dimension">The data dimension for which the index should be got.</param>
        /// <returns>The index of the specified dimension or -1 if the dimension is not part of the data set.</returns>
        public int IndexOf(DataDimension dimension)
        {
            return _dimensions.IndexOf(dimension);
        }
        /// <summary>
        /// Gets the index of the specified data dimension within the data set.
        /// </summary>
        /// <param name="dimensionName">The data dimension name for which the index should be got.</param>
        /// <returns>The index of the specified dimension or -1 if the dimension is not part of the data set.</returns>
        public int IndexOf(string dimensionName)
        {
            foreach (var dimension in _dimensions.Select((v, i) => new { Key = i, Value = v }))
                if (dimension.Value.Name == dimensionName)
                    return dimension.Key;
            return -1;
        }
        #endregion

        #region IEnumarable
        public IEnumerator<DataDimension> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public class DataSetEnumerator : IEnumerator<DataDimension>
        {
            private DataSet _dataSet;
            private int _pos = -1;

            public DataDimension Current => throw new NotImplementedException();

            object IEnumerator.Current => throw new NotImplementedException();

            DataSetEnumerator(DataSet dataSet)
            {
                _dataSet = dataSet;
            }

            public void Dispose()
            {
                _dataSet = null;
            }

            public bool MoveNext()
            {
                _pos++;
                return _pos < _dataSet.Count;
            }

            public void Reset()
            {
                _pos = -1;
            }
        }
        #endregion
    }
}
