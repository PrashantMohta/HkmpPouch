using System;

namespace HkmpPouch.DataStorage.Counter
{
    public class CounterUpdateEventArgs : EventArgs
    {
        public int Count;
    }

    public class CounterEvents
    {
        public static string GET = "|CG"; // pipe counter get
        public static string UPDATE = "|CU"; //pipe counter update
        public static string INCREMENT = "|CI"; //pipe counter Increment
        public static string DECREMENT = "|CD"; //pipe counter Decrement
    }
}
