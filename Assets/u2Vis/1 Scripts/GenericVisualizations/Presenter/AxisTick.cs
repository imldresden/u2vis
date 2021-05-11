using System;
using UnityEngine;

namespace u2vis
{
    /// <summary>
    /// Represents a single tick of an axis.
    /// </summary>
    [Serializable]
    public class AxisTick
    {
        /// <summary>
        /// The position between 0 and 1 where the tick is located on the axis. Default is 0.0.
        /// </summary>
        [SerializeField]
        private float _position = 0.0f;
        /// <summary>
        /// The label of this tick. Default is null meaning no label.
        /// </summary>
        [SerializeField]
        private string _label = null;
        /// <summary>
        /// Gets the poosition of this tick between 0 and 1 on the axis.
        /// </summary>
        public float Position => _position;
        /// <summary>
        /// Indicates if this tick has a label.
        /// </summary>
        public bool HasLabel => !string.IsNullOrEmpty(_label);
        /// <summary>
        /// Gets the label of this tick.
        /// </summary>
        public string Label => _label;
        /// <summary>
        /// Creates a new instance of the AxisTick class.
        /// </summary>
        /// <param name="position">The position between 0 and 1 of the tick on the axis.</param>
        /// <param name="label">The label of this tick, can be null which menas no label.</param>
        public AxisTick(float position, string label = null)
        {
            _position = position;
            _label = label;
        }
    }
}
