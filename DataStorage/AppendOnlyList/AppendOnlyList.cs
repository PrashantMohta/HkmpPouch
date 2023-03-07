using System;
using System.Collections.Generic;

namespace HkmpPouch.DataStorage.AppendOnlyList
{
    internal class ListItem
    {
        internal int ttl;
        internal DateTime insertedOn;
        internal string value;
    }
    /// <summary>
    /// AppendOnlyList Update EventArgs
    /// </summary>
    public class AppendOnlyListUpdateEventArgs : EventArgs
    {
        /// <summary>
        /// The Append only list of strings
        /// </summary>
        public List<string> data;
    }

    internal class AppendOnlyListEvents
    {
        internal const string ADD = "|La"; //pipe Append Only List Add
        internal const string ADDED = "|LA"; //pipe Append Only List Added New Element
        internal const string GETALL = "|Lg"; //pipe Append Only List Get All
        internal const string GOTALL = "|LG"; //pipe Append Only List Got All (for client)
    }
}
