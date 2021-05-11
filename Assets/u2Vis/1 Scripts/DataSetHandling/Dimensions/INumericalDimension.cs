using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSetHandling
{
    /// <summary>
    /// Interface representing a DataDimension with can be converted to a numeric value.
    /// </summary>
    public interface INumericalDimension
    {
        /// <summary>
        /// A float representing the minimum value stored in this DataDimension.
        /// </summary>
        float MinimumFloatValue { get; }

        /// <summary>
        /// A float representing the maximum value stored in this DataDimension.
        /// </summary>
        float MaximumFloatValue { get; }

        /// <summary>
        /// Convertes the value stored at the specified index to a float.
        /// </summary>
        /// <param name="index">The index of value which should be converted.</param>
        /// <returns>A flot representing the converted value.</returns>
        float ConvertToFloat(int index);

        /// <summary>
        /// Convertes the value stored at the specified index to a float which is normalized between 0 and 1 in relation to the minimum and maximum float value of this DataDimension.
        /// </summary>
        /// <param name="index">The index of value which should be converted.</param>
        /// <returns>A flot representing the converted value.</returns>
        float ConvertToNormalizedFloat(int index);
    }
}
