using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class IncreaseStatS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(IncreaseStatS2CPacket).TypeHandle);

    public int statId;
    public int amount;

    public IncreaseStatS2CPacket()
    {
    }

    public IncreaseStatS2CPacket(int statId, int amount)
    {
        this.statId = statId;
        this.amount = amount;
    }

    public override void apply(NetHandler handler)
    {
        handler.onIncreaseStat(this);
    }

    public override void read(Stream stream)
    {
        statId = stream.ReadInt();
        amount = (sbyte)stream.ReadByte();
    }

    public override void write(Stream stream)
    {
        stream.WriteInt(statId);
        stream.WriteByte((byte)amount);
    }

    public override int size()
    {
        return 6;
    }
}
