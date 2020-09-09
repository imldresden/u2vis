using System;
using UnityEngine;

namespace UVis
{
    [Serializable]
    public class AxisTick
    {
        [SerializeField]
        private float _position = 0.0f;
        [SerializeField]
        private string _label = null;

        public float Position => _position;

        public bool HasLabel => !string.IsNullOrEmpty(_label);

        public string Label => _label;

        public AxisTick(float position, string label = null)
        {
            _position = position;
            _label = label;
        }
    }
}
