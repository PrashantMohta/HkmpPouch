using Hkmp.Networking.Packet;

namespace HkmpPouch.Packets
{
    internal class RegisterPipePacket : IPacketData
    {
        public string modName;

        public bool IsReliable => true;

        public bool DropReliableDataIfNewerExists => false;

        public void ReadData(IPacket packet)
        {
            modName = packet.ReadString();
        }

        public void WriteData(IPacket packet)
        {
            packet.Write(modName);
        }
    }
}