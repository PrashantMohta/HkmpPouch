﻿using Hkmp.Networking.Packet;

namespace HkmpPouch.Packets
{
    internal class BasePacket
    {

        public bool IsReliable => _isReliable;
        public bool DropReliableDataIfNewerExists => false; //cannot set this to true because our packets are reused between mods.

        public bool _isReliable = false;
        public string mod { get; set; }
        public string eventName { get; set; }
        public string eventData { get; set; }

        public void WriteData(IPacket packet)
        {
            //order of read should be same as order of write
            packet.Write(mod);
            packet.Write(eventName);
            packet.Write(eventData);
        }

        public void ReadData(IPacket packet)
        {
            //order of read should be same as order of write
            mod = packet.ReadString();
            eventName = packet.ReadString();
            eventData = packet.ReadString();
        }
    }
}
