using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityStatusS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityStatusS2CPacket).TypeHandle);

    public int entityId;
    public sbyte entityStatus;

    public EntityStatusS2CPacket()
    {
    }

    public EntityStatusS2CPacket(int entityId, byte status)
    {
        this.entityId = entityId;
        entityStatus = (sbyte)status;
    }

    public override void read(Stream stream)
    {
        entityId = stream.ReadInt();
        entityStatus = (sbyte)stream.ReadByte();
    }

    public override void write(Stream stream)
    {
        stream.WriteInt(entityId);
        stream.WriteByte(entityStatus);
    }

    public override void apply(NetHandler handler)
    {
        handler.onEntityStatus(this);
    }

    public override int size()
    {
        return 5;
    }
}
