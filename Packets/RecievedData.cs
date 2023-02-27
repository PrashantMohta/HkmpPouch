namespace HkmpPouch.Packets
{
    public class RecievedData
    {
        //metadata
        public bool IsReliable = false;
        public bool DropReliableDataIfNewerExists = false;

        // server side metadata
        public bool Rebroadcast = true;

        //actual data
        public string SceneName = "";
        public ushort ToPlayer = 0;
        public ushort FromPlayer = 0;
        public string ModName = "";
        public string EventName = "";
        public string EventData = "";
    }
}