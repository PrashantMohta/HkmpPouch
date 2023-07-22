using Hkmp.Networking.Packet;
using System;

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
        public bool hasExtraBytes { get => extraBytes != null && extraBytes.Length > 0; }
        public ulong extraBytesSize { get; set; }
        public byte[] extraBytes { get; set; }

        public void WriteData(IPacket packet)
        {
            //order of read should be same as order of write
            packet.Write(mod);
            packet.Write(eventName);
            packet.Write(eventData);
            packet.Write(hasExtraBytes);
            if (hasExtraBytes)
            {
                extraBytesSize = (ulong)extraBytes.Length;
                packet.Write(extraBytesSize);
                for (ulong i = 0; i < extraBytesSize; i++)
                {
                    packet.Write(extraBytes[i]);
                }
            }
        }

        public void ReadData(IPacket packet)
        {
            //order of read should be same as order of write
            mod = packet.ReadString();
            eventName = packet.ReadString();
            eventData = packet.ReadString();
            try
            {
                var hasExtraBytes = packet.ReadBool();
                if (hasExtraBytes)
                {
                    extraBytesSize = packet.ReadULong();
                    for (ulong i = 0; i < extraBytesSize; i++)
                    {
                        packet.Write(extraBytes[i]);
                    }
                }
            }
            catch(Exception ex){
                // eat the error here that will happen when ReadBool tries to read from a packet that does not have data to read
                // this will happen when a player with old pouch connects to new servers, i don't know if we are on the server or unity so here goes
#pragma warning disable CS0618 // Type or member is obsolete but only for others
                Logger.StupidError(ex.ToString());
#pragma warning restore CS0618 // Type or member is obsolete but only for others
            }
        }

    }
}
