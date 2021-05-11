using System;
using UnityEngine;

namespace u2vis
{
    [Serializable]
    public class DataItem
    {
        [SerializeField]
        public float[] Values { get; set; }
        [SerializeField]
        public bool IsHighlighted { get; set; }

        public float this[int index] => Values[index];

        public DataItem(float value = 0.0f, bool ishighlighted = false)
        {
            Values = new float[] { value };
            IsHighlighted = ishighlighted;
        }

        public DataItem(params float[] values)
        {
            Values = values;
        }
    }
}
