using System;

namespace HkmpPouch.DataStorage.Counter
{
    /// <summary>
    /// Counter Update Event Args
    /// </summary>
    public class CounterUpdateEventArgs : EventArgs
    {
        /// <summary>
        /// Current Value of the counter
        /// </summary>
        public int Count;
    }

    /// <summary>
    /// Constants for Counter Event Names
    /// </summary>
    internal class CounterEvents
    {
        internal const string GET = "|CG"; // pipe counter get
        internal const string UPDATE = "|CU"; //pipe counter update
        internal const string INCREMENT = "|CI"; //pipe counter Increment
        internal const string DECREMENT = "|CD"; //pipe counter Decrement
    }
}
