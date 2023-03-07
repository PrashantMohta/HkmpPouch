using Hkmp.Networking.Packet;

namespace HkmpPouch.Packets
{
    internal class ToPlayerPacket : BasePacket, IPacketData
    {
        public ushort toPlayer = 0;

        new public void ReadData(IPacket packet)
        {
            base.ReadData(packet);
            toPlayer = packet.ReadUShort();

        }

        new public void WriteData(IPacket packet)
        {
            base.WriteData(packet);
            packet.Write(toPlayer);

        }
    }
}