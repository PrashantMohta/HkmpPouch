using Hkmp.Networking.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HkmpPouch.Packets
{
    internal enum METADATA
    {
        INSTALLED_PIPES = 0
    }
    internal class GetServerMetadataPacket : IPacketData
    {
        public bool IsReliable => true;
        public bool DropReliableDataIfNewerExists => true;

        public byte MetadataKey = 0;

        public void ReadData(IPacket packet)
        {
            MetadataKey = packet.ReadByte();
        }

        public void WriteData(IPacket packet)
        {
            packet.Write(MetadataKey);
        }
    }
    internal class ServerPipeListPacket : IPacketData
    {
        public bool IsReliable => true;
        public bool DropReliableDataIfNewerExists => true;

        private byte Count;
        public List<string> PipeList = new();

        public void ReadData(IPacket packet)
        {

            Count = packet.ReadByte();
            for (var i = 0; i < Count; i++)
            {
                var x = packet.ReadString();
                Debug.Log(x);
                PipeList.Add(x);
            }
        }

        public void WriteData(IPacket packet)
        {
            packet.Write((byte)PipeList.Count);
            for (var i = 0; i < PipeList.Count && i < byte.MaxValue; i++)
            {
                packet.Write(PipeList[i]);
            }
        }
    }
}
