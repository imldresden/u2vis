using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSetHandling
{
    /// <summary>
    /// Represents a single data object with different datadimension.
    /// </summary>
    public class DataObject
    {        
        #region Private Fields
        /// <summary>
        /// The list of data dimensions of this data set.
        /// </summary>
        protected Dictionary<string, DataDimension> _dimensions;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the data dimension with the specified index.
        /// </summary>
        /// <param name="index">The index of the data dimension.</param>
        /// <returns>The data dimension with the specified index.</returns>
        public DataDimension this[string dimensionName] => _dimensions[dimensionName];

        /// <summary>
        /// Gets the number of data dimensions.
        /// </summary>
        public int Count => _dimensions.Count;
        #endregion

        #region Casting
        public static implicit operator DataSet(DataObject dataObject) => new DataSet(dataObject._dimensions.Values.ToArray());

        public override string ToString() => $"DO [ {string.Join("; ", _dimensions.Values)} ]";
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new instance of the DataObject class.
        /// </summary>
        public DataObject()
        {
            _dimensions = new Dictionary<string, DataDimension>();
        }

        /// <summary>
        /// Creates a new instance of the DataSet class.
        /// </summary>
        /// <param name="dimensions">An array of DataDimesions this DataObject consists of.</param>
        public DataObject(params DataDimension[] dimensions)
        {
            _dimensions = dimensions.ToDictionary(v => v.Name, v => v);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Add a new data dimension to this data object.
        /// </summary>
        /// <param name="dimension">The data dimension that should be added.</param>
        public void Add(DataDimension dimension)
        {
            if (_dimensions.ContainsKey(dimension.Name))
                throw new ArgumentException("Dimension is already part of the data object!");
            _dimensions.Add(dimension.Name, dimension);
        }

        /// <summary>
        /// Removes a data dimension from the data object.
        /// </summary>
        /// <param name="dimension">The data dimension which should be removed.</param>
        /// <returns>true if the dimension was removed, otherwiese false.</returns>
        public bool Remove(DataDimension dimension)
        {
            return _dimensions.Remove(dimension.Name);
        }
        #endregion
    }
}
