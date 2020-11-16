using System;

namespace u2vis
{
    /// <summary>
    /// Interface that represents a DataProvider which all data providers need to implement.
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// gets the DataSet this DataProvider provides.
        /// </summary>
        DataSet Data { get; }
        /// <summary>
        /// Raised whenever the DataSet of this DataProvider has changed.
        /// </summary>
        event EventHandler DataChanged;
    }
}
