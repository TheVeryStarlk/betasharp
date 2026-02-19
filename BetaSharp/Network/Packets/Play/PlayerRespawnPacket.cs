using java.io;

namespace BetaSharp.Network.Packets.Play;

public class PlayerRespawnPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(PlayerRespawnPacket).TypeHandle);

    public sbyte dimensionId;

    public PlayerRespawnPacket()
    {
    }

    public PlayerRespawnPacket(sbyte dimensionId)
    {
        this.dimensionId = dimensionId;
    }

    public override void apply(NetHandler handler)
    {
        handler.onPlayerRespawn(this);
    }

    public override void read(Stream stream)
    {
        dimensionId = (sbyte)stream.ReadByte();
    }

    public override void write(Stream stream)
    {
        stream.WriteByte(dimensionId);
    }

    public override int size()
    {
        return 1;
    }
}
