using System;

namespace UVis
{
    public interface IDataProvider
    {
        DataSet Data { get; }
        event EventHandler DataChanged;
    }
}
