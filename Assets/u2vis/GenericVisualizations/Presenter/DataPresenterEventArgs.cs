using System;

namespace UVis
{
    public class DataPresenterEventArgs : EventArgs
    {
        public readonly int PrevMinIndex;
        public readonly int PrevMaxIndex;
        public readonly int CurrMinIndex;
        public readonly int CurrMaxIndex;

        public DataPresenterEventArgs(int minIndex, int maxIndex)
            : this (minIndex, maxIndex, minIndex, maxIndex)
        {
        }

        public DataPresenterEventArgs(int prevMinIndex, int prevMaxIndex, int currMinIndex, int currMaxIndex)
        {
            PrevMinIndex = prevMinIndex;
            PrevMaxIndex = prevMaxIndex;
            CurrMinIndex = currMinIndex;
            CurrMaxIndex = currMaxIndex;
        }

        public DataPresenterEventArgs()
        {

        }
    }
}
