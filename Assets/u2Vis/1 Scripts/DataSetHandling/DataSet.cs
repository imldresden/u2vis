using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataSetHandling
{
    /// <summary>
    /// class that represents a specific data set, containing a number of DataDimensions of verious types.
    /// </summary>
    public class DataSet : IEnumerable<DataDimension>
    {
        #region Private Fields
        /// <summary>
        /// The list of data dimensions of this data set.
        /// </summary>
        protected List<DataDimension> _dimensions;
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

        #region Casting
        public static implicit operator DataObject(DataSet dataSet) => new DataObject(dataSet._dimensions.ToArray());

        public override string ToString() => $"DS [ {string.Join("; ", _dimensions)} ]";
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new instance of the DataSet class.
        /// </summary>
        public DataSet()
        {
            _dimensions = new List<DataDimension>();
        }

        /// <summary>
        /// Creates a new instance of the DataSet class.
        /// </summary>
        /// <param name="dimensions">An array of DataDimesions this dataset consists of.</param>
        public DataSet(params DataDimension[] dimensions)
        {
            _dimensions = new List<DataDimension>(dimensions);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the number of Items stored in the dataset.
        /// </summary>
        /// <returns></returns>
        private int GetNumberOfItems()
        {
            // just take the length of the first dimension, assuming that all dimensions are of equal length
            if (_dimensions.Count >= 1)
                return _dimensions[0].Count;
            return 0;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Add a new data dimension to this data set.
        /// </summary>
        /// <param name="dimension">The data dimension that should be added.</param>
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
        /// <returns>true if the dimension was removed, otherwise false.</returns>
        public bool Remove(DataDimension dimension)
        {
            return _dimensions.Remove(dimension);
        }

        /// <summary>
        /// Gets the index of the specified data dimension within the data set.
        /// </summary>
        /// <param name="dimension">The data dimension for which the index should be got.</param>
        /// <returns>The index of the specified dimension or -1 if the dimension is not part of the data set.</returns>
        /// 
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

        /// <summary>
        /// Searches for the dimension with the given name.
        /// </summary>
        /// <param name="dimensionName">The name of the dimension to search for.</param>
        /// <returns>The dimension with the given name. null if no dimension was found.</returns>
        public DataDimension GetDimension(string dimensionName)
        {
            foreach (var dimension in _dimensions.Select((v, i) => new { Key = i, Value = v }))
                if (dimension.Value.Name == dimensionName)
                    return dimension.Value;
            return null;
        }

        /// <summary>
        /// Generates a DataObject instance out of the given row of this data set.
        /// </summary>
        /// <param name="rowIndex">The index the data object should be created from.</param>
        /// <returns>The DataObject instance.</returns>
        public DataObject GetDataObject(int rowIndex)
        {
            if (rowIndex > NumOfItems)
                return null;
            return new DataObject(_dimensions.Select(dd => dd.GenerateSubSetDataDimension(rowIndex)).ToArray());

        }
        #endregion

        #region IEnumarable
        /// <summary>
        /// Get a new Enumerator for this data set.
        /// </summary>
        /// <returns>the new Enumerator.</returns>
        public IEnumerator<DataDimension> GetEnumerator()
        {
            return _dimensions.GetEnumerator();
        }

        /// <summary>
        /// Get a new Enumerator for this data set.
        /// </summary>
        /// <returns>The new Enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public class DataSetEnumerator : IEnumerator<DataDimension>
        {
            /// <summary>
            /// The data set that is traversed by this enumerator.
            /// </summary>
            private DataSet _dataSet;

            /// <summary>
            /// Index of the current data dimension.
            /// </summary>
            private int _pos = -1;

            /// <summary>
            /// Get the current DataDimension of the Enumerator.
            /// </summary>
            public DataDimension Current => _dataSet[_pos];

            /// <summary>
            /// Get the current DataDimension of the Enumerator.
            /// </summary>
            object IEnumerator.Current => _dataSet[_pos];

            /// <summary>
            /// Creates a new instace of the DataSetEnumerator class.
            /// </summary>
            /// <param name="dataSet"></param>
            DataSetEnumerator(DataSet dataSet)
            {
                _dataSet = dataSet;
            }
            /// <summary>
            /// Dispose this DataSetEnumerator.
            /// </summary>
            public void Dispose()
            {
                _dataSet = null;
            }

            /// <summary>
            /// Move to the next index of the data set.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                _pos++;
                return _pos < _dataSet.Count;
            }

            /// <summary>
            /// Reset the enumerator.
            /// </summary>
            public void Reset()
            {
                _pos = -1;
            }
        }
        #endregion
    }
}
