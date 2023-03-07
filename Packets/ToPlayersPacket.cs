using Hkmp.Networking.Packet;

namespace HkmpPouch.Packets
{
    internal class ToPlayersPacket : BasePacket, IPacketData
    {
        public string sceneName = "";

        new public void ReadData(IPacket packet)
        {
            base.ReadData(packet);
            sceneName = packet.ReadString();
        }

        new public void WriteData(IPacket packet)
        {
            base.WriteData(packet);

            packet.Write(sceneName);
        }
    }
}