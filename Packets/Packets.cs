﻿using Hkmp.Networking.Packet;
using Hkmp.Networking.Packet.Data;
using System;

namespace HkmpPouch.Packets
{
    public enum PacketsEnum
    {
        ToServerPacket,
        PlayerToPlayerPacket,
        PlayerToPlayersPacket,
        ToPlayerPacket,
        ToPlayersPacket
    }
    public class RecievedEventArgs : EventArgs
    {
        public RecievedData Data { get; set; }
    }
    internal static class PacketBoi
    {
        internal static IPacketData InstantiatePacket(PacketsEnum packetId)
        {
            switch (packetId)
            {
                case PacketsEnum.ToServerPacket:
                    return new PacketDataCollection<ToServerPacket>();
                case PacketsEnum.PlayerToPlayerPacket:
                    return new PacketDataCollection<PlayerToPlayerPacket>();
                case PacketsEnum.PlayerToPlayersPacket:
                    return new PacketDataCollection<PlayerToPlayersPacket>();
                case PacketsEnum.ToPlayerPacket:
                    return new PacketDataCollection<ToPlayerPacket>();
                case PacketsEnum.ToPlayersPacket:
                    return new PacketDataCollection<ToPlayersPacket>();
            }
            return null;
        }
    }
   
}