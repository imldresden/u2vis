using System;

namespace u2vis.NodeLink
{
    public interface INode
    {
        int Uid { get; }
        float PosX { get; set; }
        float PosY { get; set; }
        float PosZ { get; set; }
        string Label { get; set; }

        event EventHandler Updated;
    }
}