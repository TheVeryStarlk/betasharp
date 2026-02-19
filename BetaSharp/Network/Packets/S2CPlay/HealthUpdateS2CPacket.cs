using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class HealthUpdateS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(HealthUpdateS2CPacket).TypeHandle);

    public int healthMP;

    public HealthUpdateS2CPacket()
    {
    }

    public HealthUpdateS2CPacket(int health)
    {
        healthMP = health;
    }

    public override void read(Stream stream)
    {
        healthMP = stream.ReadShort();
    }

    public override void write(Stream stream)
    {
        stream.WriteShort((short)healthMP);
    }

    public override void apply(NetHandler handler)
    {
        handler.onHealthUpdate(this);
    }

    public override int size()
    {
        return 2;
    }
}
