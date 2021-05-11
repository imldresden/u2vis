using DataSetHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace u2vis
{
    /// <summary>
    /// AbstractDataProvider which can be used as a base class for new DataProviders.
    /// Inherit from MonoBehaviour to use DataProviders in the Editor.
    /// </summary>
    public abstract class AbstractDataProvider : MonoBehaviour, IDataProvider
    {
        /// <summary>
        /// Gets the DataSet this DataProvider provides.
        /// </summary>
        public abstract DataSet Data { get; }
        /// <summary>
        /// Raised whenever the DataSet of this provider has changed.
        /// </summary>
        public event EventHandler DataChanged;
        /// <summary>
        /// Raises the DataChanged event.
        /// </summary>
        protected virtual void OnDataChanged()
        {
            DataChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
