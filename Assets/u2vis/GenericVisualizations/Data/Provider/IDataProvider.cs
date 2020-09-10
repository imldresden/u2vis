using System;

namespace u2vis
{
    public interface IDataProvider
    {
        DataSet Data { get; }
        event EventHandler DataChanged;
    }
}
