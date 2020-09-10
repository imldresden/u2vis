using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace u2vis
{
    public class GenericVisualizationStyle : MonoBehaviour
    {
        [SerializeField]
        protected Gradient[] _colorMappings = new Gradient[1];
        [SerializeField]
        protected Color _highlightColor = Color.red;

        public Color HighlightColor => _highlightColor;

        public virtual Color GetColorContinous(params float[] values)
        {
            Color result = Color.white;
            int length = Mathf.Min(_colorMappings.Length, values.Length);
            for (int i = 0; i < length; i++)
                result *= _colorMappings[i].Evaluate(values[i]);
            return result;
        }

        public virtual Color GetColorContinous(bool isHighlighted, params float[] values)
        {
            if (isHighlighted)
                return _highlightColor;
            return GetColorContinous(values);
        }

        public virtual Color GetColorCategorical(int catIndex, float value = 0.5f)
        {
            int index = catIndex % _colorMappings.Length;
            value = Mathf.Clamp(value, 0, 1);
            return _colorMappings[index].Evaluate(value);
        }

        public virtual void Initialize(Gradient[] colorMappings, Color highlightColor)
        {
            _colorMappings = colorMappings;
            _highlightColor = highlightColor;
        }
    }
}