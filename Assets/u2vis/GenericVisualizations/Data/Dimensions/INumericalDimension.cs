using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace u2vis
{
    public interface INumericalDimension
    {
        float MinimumFloatValue { get; }
        float MaximumFloatValue { get; }
        float ConvertToFloat(int index);
        float ConvertToNormalizedFloat(int index);
    }
}
