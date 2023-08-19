using Hkmp.Networking.Packet;
using Hkmp.Networking.Packet.Data;

namespace HkmpPouch.Packets
{
    internal enum PacketsEnum
    {
        ToServerPacket,
        PlayerToPlayerPacket,
        PlayerToPlayersPacket,
        ToPlayerPacket,
        ToPlayersPacket,

        GetServerMetadataPacket,
        ServerPipeListPacket,
        RegisterPipePacket
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
                case PacketsEnum.GetServerMetadataPacket:
                    return new PacketDataCollection<GetServerMetadataPacket>();
                case PacketsEnum.ServerPipeListPacket:
                    return new PacketDataCollection<ServerPipeListPacket>();
            }
            return null;
        }
    }
   
}