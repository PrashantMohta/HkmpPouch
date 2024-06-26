﻿using Hkmp.Networking.Packet;

namespace HkmpPouch.Packets
{
    internal class ToServerPacket: BasePacket, IPacketData
    {
        new public void ReadData(IPacket packet)
        {
            base.ReadData(packet);
            base.ReadExtraBytes(packet);
        }

        new public void WriteData(IPacket packet)
        {
            base.WriteData(packet);
            base.WriteExtraBytes(packet);
        }
    }
}
