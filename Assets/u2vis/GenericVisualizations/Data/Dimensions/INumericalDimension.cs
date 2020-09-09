using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UVis
{
    public interface INumericalDimension
    {
        float MinimumFloatValue { get; }
        float MaximumFloatValue { get; }
        float ConvertToFloat(int index);
        float ConvertToNormalizedFloat(int index);
    }
}
