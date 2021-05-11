using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace u2vis
{
    /// <summary>
    /// Class that represents a vislization style that can be applied to visualizations.
    /// </summary>
    public class GenericVisualizationStyle : MonoBehaviour
    {
        #region Protected Fields
        /// <summary>
        /// The list of color gradients representing the visualization style.
        /// </summary>
        [SerializeField]
        protected Gradient[] _colorMappings = new Gradient[1];
        /// <summary>
        /// The color for highlighted values.
        /// </summary>
        [SerializeField]
        protected Color _highlightColor = Color.red;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the color for highlighted values.
        /// </summary>
        public Color HighlightColor => _highlightColor;
        #endregion

        #region Public Methods
        /// <summary>
        /// Determines the final color by multiplying the from the values resulting colors on all dimensions.
        /// Values need to be normalized between 0 and 1.
        /// </summary>
        /// <param name="values">The list of values from which the color is detemrined.</param>
        /// <returns>The resulting color.</returns>
        public virtual Color GetColorContinous(params float[] values)
        {
            Color result = Color.white;
            int length = Mathf.Min(_colorMappings.Length, values.Length);
            for (int i = 0; i < length; i++)
                result *= _colorMappings[i].Evaluate(values[i]);
            return result;
        }
        /// <summary>
        /// Determines the final color by multiplying the from the values resulting colors on all dimensions.
        /// Values need to be normalized between 0 and 1.
        /// </summary>
        /// <param name="isHighlighted">Indicates if the color is highlighted or not.</param>
        /// <param name="values">The list of values from which the color is detemrined.</param>
        /// <returns>The resulting color.</param>
        public virtual Color GetColorContinous(bool isHighlighted, params float[] values)
        {
            if (isHighlighted)
                return _highlightColor;
            return GetColorContinous(values);
        }
        /// <summary>
        /// Determines the final color by the given category index and an aoptional value.
        /// </summary>
        /// <param name="catIndex">The category for which the color should be determined.</param>
        /// <param name="value">The optional value within the category. Default is 0.5, meaning the middle of the gradient.</param>
        /// <returns>The resulting color</returns>
        public virtual Color GetColorCategorical(int catIndex, float value = 0.5f)
        {
            int index = catIndex % _colorMappings.Length;
            value = Mathf.Clamp(value, 0, 1);
            return _colorMappings[index].Evaluate(value);
        }
        /// <summary>
        /// Initialize this VisualizationStyle.
        /// </summary>
        /// <param name="colorMappings">The list of color mappings for this visualizations style.</param>
        /// <param name="highlightColor">The color used for highlighted values.</param>
        public virtual void Initialize(Gradient[] colorMappings, Color highlightColor)
        {
            _colorMappings = colorMappings;
            _highlightColor = highlightColor;
        }
        #endregion
    }
}