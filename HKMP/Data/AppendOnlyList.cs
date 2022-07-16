namespace HkmpPouch{
    public class ListItem{
        public int ttl;
        public DateTime insertedOn;
        public string value;
    }
    public class AppendOnlyListUpdateEventArgs : EventArgs {
        public List<string> data;
    }

    public class AppendOnlyListEvents{
        public static string ADD = "|La"; //pipe Append Only List Add
        public static string ADDED = "|LA"; //pipe Append Only List Added New Element
        public static string GETALL = "|Lg"; //pipe Append Only List Get All
        public static string GOTALL = "|LG"; //pipe Append Only List Got All (for client)
    }

}