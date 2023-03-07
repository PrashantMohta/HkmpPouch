using Hkmp.Networking.Packet;

namespace HkmpPouch.Packets
{
    internal class PlayerToPlayersPacket : BasePacket, IPacketData
    {
        public ushort fromPlayer = 0;

        public string sceneName = "";
        new public void ReadData(IPacket packet)
        {
            base.ReadData(packet);
            fromPlayer = packet.ReadUShort();
            sceneName = packet.ReadString();


        }

        new public void WriteData(IPacket packet)
        {
            base.WriteData(packet);
            packet.Write(fromPlayer);
            packet.Write(sceneName);

        }
    }
}