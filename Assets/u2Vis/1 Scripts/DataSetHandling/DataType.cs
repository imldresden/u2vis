using System;
using System.Linq;

namespace DataSetHandling
{
    /// <summary>
    /// All data types a DataDimension can contain.
    /// </summary>
    public enum DataType
    {
        Undefinded,
        Boolean,
        Integer,
        Float,
        String,
    }

    public static class DataTypeExtension
    {
        public static DataDimension GenerateDataDimension(this DataType dataType, string name, string[] values)
        {
            switch (dataType)
            {
                case DataType.Boolean:      return new BooleanDimension(name, values.Select(s => Boolean.Parse(s.Trim())).ToArray());
                case DataType.Integer:      return new IntegerDimension(name, values.Select(s => Int32.Parse(s.Trim())).ToArray());
                case DataType.Float:        return new FloatDimension(name, values.Select(s => Single.Parse(s.Trim())).ToArray());
                case DataType.String:       return new StringDimension(name, values.Select(s => s.Trim()).ToArray());
                default:                    return null;
            }
        }

        /// <summary>
        /// Generates the DataType from a given string for one of the enum values.
        /// </summary>
        /// <param name="_"></param>
        /// <param name="name">The string value to transform from.</param>
        /// <returns>The DataType based on the given string. If the string does not match any enum value, this function returns the 'Undefined' value.</returns>
        public static DataType GetFromString(this DataType _, string name)
        {
            try
            {
                return (DataType)Enum.Parse(typeof(DataType), name.Trim());
            }
            catch
            {
                return DataType.Undefinded;
            }
        }

        /// <summary>
        /// Generate the DataType from a data string that should be corresponding to the DataType.
        /// </summary>
        /// <param name="_"></param>
        /// <param name="dataValue">The DataType to check for as a string.</param>
        /// <returns>Returns the best fitting DataType for the given data string.</returns>
        public static DataType GetTypeFromDataString(this DataType _, string dataValue)
        {
            if (Boolean.TryParse(dataValue, out bool b))
                return DataType.Boolean;
            if (Int32.TryParse(dataValue, out int i))
                return DataType.Integer;
            if (Single.TryParse(dataValue, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float f))
                return DataType.Float;
            return DataType.String;
        }
    }
}
