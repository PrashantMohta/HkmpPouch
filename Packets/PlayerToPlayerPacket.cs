using Hkmp.Networking.Packet;

namespace HkmpPouch.Packets
{
    internal class PlayerToPlayerPacket : BasePacket, IPacketData
    {
        public ushort toPlayer = 0;
        public ushort fromPlayer = 0;
        public string sceneName = "";
        new public void ReadData(IPacket packet)
        {
            base.ReadData(packet);
            fromPlayer = packet.ReadUShort();
            toPlayer = packet.ReadUShort();
            sceneName = packet.ReadString();
        }

        new public void WriteData(IPacket packet)
        {
            base.WriteData(packet);
            packet.Write(fromPlayer);
            packet.Write(toPlayer);
            packet.Write(sceneName);
        }
    }
}