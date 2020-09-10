using UnityEngine;

namespace u2vis
{
    public static class VisViewHelper
    {
        public static float GetItemValue(GenericDataPresenter presenter, int dimIndex, int itemIndex, bool normalize = false, bool relative = false)
        {
            if (relative)
                return GetItemValueRelative(presenter, dimIndex, itemIndex, normalize);
            return GetItemValueAbsolute(presenter, dimIndex, itemIndex, normalize);
        }

        public static float GetItemValueAbsolute(GenericDataPresenter presenter, int dimIndex, int itemIndex, bool normalize = false)
        {
            var dim = presenter[dimIndex];
            var numDim = dim as INumericalDimension;
            if (numDim != null)
            {
                if (normalize)
                    return numDim.ConvertToNormalizedFloat(itemIndex);
                return numDim.ConvertToFloat(itemIndex);
            }
            return (itemIndex - presenter.SelectedMinItem) / (presenter.SelectedMaxItem - presenter.SelectedMinItem);
        }

        public static float GetItemValueRelative(GenericDataPresenter presenter, int dimIndex, int itemIndex, bool normalize = false)
        {
            var dim = presenter[dimIndex];
            var numDim = dim as INumericalDimension;
            if (numDim != null)
            {
                float v = numDim.ConvertToFloat(itemIndex);
                v -= numDim.MinimumFloatValue;
                if (normalize)
                    return v / (numDim.MaximumFloatValue - numDim.MinimumFloatValue);
                return v;
            }
            return (itemIndex - presenter.SelectedMinItem) / (presenter.SelectedMaxItem - presenter.SelectedMinItem);
        }

        public static string GetItemString(GenericDataPresenter presenter, int dimIndex, int itemIndex)
        {
            var dim = presenter[dimIndex];
            var numDim = dim as INumericalDimension;
            if (numDim != null)
            {
                return numDim.ConvertToFloat(itemIndex).ToString();
            }
            return dim.GetStringValue(itemIndex);
        }
        public static float GetGlobalMaximum(GenericDataPresenter presenter)
        {
            float max = float.MinValue;
            foreach (var dim in presenter.Dimensions)
            {
                var numDim = dim as INumericalDimension;
                if (numDim == null)
                    continue;
                if (numDim.MaximumFloatValue > max)
                    max = numDim.MaximumFloatValue;
            }
            return max;
        }

        public static Color BlendColorsTotal(params Color[] colors)
        {
            var result = Color.black;
            foreach (var color in colors)
                result += color;
            result.r = Mathf.Max(1.0f, result.r);
            result.g = Mathf.Max(1.0f, result.g);
            result.b = Mathf.Max(1.0f, result.b);
            result.a = Mathf.Max(1.0f, result.a);
            return result;
        }

        public static Color BlendColorsAverage(params Color[] colors)
        {
            var result = Color.black;
            foreach (var color in colors)
                result += color;
            return result / colors.Length;
        }

        public static Color BlendColorsMaximum(params Color[] colors)
        {
            var result = Color.black;
            foreach (var color in colors)
            {
                result.r = Mathf.Max(result.r, color.r);
                result.g = Mathf.Max(result.r, color.g);
                result.b = Mathf.Max(result.r, color.b);
                result.a = Mathf.Max(result.r, color.a);
            }
            return result;
        }
    }
}
