using Hkmp.Networking.Packet;

namespace HkmpPouch{
    
    public class ReceivedEventArgs : EventArgs {
        public GenericPacket packet { get; set; }
    }
    public enum Packets {
        GenericPacket
    }
    public class GenericPacket : IPacketData {
        public bool IsReliable => _isReliable;
        public bool DropReliableDataIfNewerExists => _dropReliableDataIfNewerExists;
        public bool _isReliable = false;
        public bool sameScene = false;
        public bool _dropReliableDataIfNewerExists = false;
        public bool rebroadcast = false;
        public bool broadcastToAll = false;
        public ushort toPlayer = 0;
        public ushort fromPlayer = 0;
        public string mod { get; set; }
        public string eventName { get; set; }
        public string eventData { get; set; }

        public void WriteData(IPacket packet) {
            //order of read should be same as order of write
            packet.Write(mod);
            packet.Write(rebroadcast);
            packet.Write(sameScene);
            packet.Write(broadcastToAll);
            packet.Write(fromPlayer);
            packet.Write(toPlayer);
            packet.Write(eventName);
            packet.Write(eventData);
        }

        public void ReadData(IPacket packet) {
            //order of read should be same as order of write
            mod = packet.ReadString();
            rebroadcast = packet.ReadBool();
            sameScene = packet.ReadBool();
            broadcastToAll = packet.ReadBool();
            fromPlayer = packet.ReadUShort();
            toPlayer = packet.ReadUShort();
            eventName = packet.ReadString();
            eventData = packet.ReadString();
        }
    }

}