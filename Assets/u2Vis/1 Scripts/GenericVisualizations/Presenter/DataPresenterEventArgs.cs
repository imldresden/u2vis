using System;

namespace u2vis
{
    /// <summary>
    /// Event Arguments which represents changed data for a DataPresenter.
    /// </summary>
    public class DataPresenterEventArgs : EventArgs
    {
        /// <summary>
        /// The previous minimum index before the change.
        /// </summary>
        public readonly int PrevMinIndex;
        /// <summary>
        /// The previous maximum index before the change.
        /// </summary>
        public readonly int PrevMaxIndex;
        /// <summary>
        /// The current minimum index after the change.
        /// </summary>
        public readonly int CurrMinIndex;
        /// <summary>
        /// The current maximum index after the change.
        /// </summary>
        public readonly int CurrMaxIndex;
        /// <summary>
        /// Creates a new instance of the DataPresenterEventArgs class.
        /// </summary>
        public DataPresenterEventArgs()
        {
        }
        /// <summary>
        /// Creates a new instance of the DataPresenterEventArgs class.
        /// </summary>
        /// <param name="minIndex">The current minimum index.</param>
        /// <param name="maxIndex">The current maximum index.</param>
        public DataPresenterEventArgs(int minIndex, int maxIndex)
            : this (minIndex, maxIndex, minIndex, maxIndex)
        {
        }
        /// <summary>
        /// Creates a new instance of the DataPresenterEventArgs class.
        /// </summary>
        /// <param name="prevMinIndex">The previous minimum index.</param>
        /// <param name="prevMaxIndex">The previous maximum index.</param>
        /// <param name="currMinIndex">The current minimum index.</param>
        /// <param name="currMaxIndex">The current maximum index.</param>
        public DataPresenterEventArgs(int prevMinIndex, int prevMaxIndex, int currMinIndex, int currMaxIndex)
        {
            PrevMinIndex = prevMinIndex;
            PrevMaxIndex = prevMaxIndex;
            CurrMinIndex = currMinIndex;
            CurrMaxIndex = currMaxIndex;
        }
    }
}
