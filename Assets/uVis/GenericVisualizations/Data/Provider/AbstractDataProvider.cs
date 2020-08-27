using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UVis
{
    /// <summary>
    /// Inherit from MonoBehaviour to use DataProviders in the Editor
    /// </summary>
    public abstract class AbstractDataProvider : MonoBehaviour, IDataProvider
    {
        public abstract DataSet Data { get; }

        public event EventHandler DataChanged;

        protected virtual void OnDataChanged()
        {
            DataChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
