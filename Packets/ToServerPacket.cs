using Hkmp.Networking.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HkmpPouch.Packets
{
    internal class ToServerPacket: BasePacket, IPacketData
    {
        new public void ReadData(IPacket packet)
        {
            base.ReadData(packet);
        }

        new public void WriteData(IPacket packet)
        {
            base.WriteData(packet);
        }
    }
}
