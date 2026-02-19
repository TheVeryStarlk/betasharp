using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityMoveRelativeS2CPacket : EntityS2CPacket
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityMoveRelativeS2CPacket).TypeHandle);

    public EntityMoveRelativeS2CPacket()
    {
    }

    public EntityMoveRelativeS2CPacket(int entityId, byte deltaX, byte deltaY, byte deltaZ) : base(entityId)
    {
        this.deltaX = (sbyte)deltaX;
        this.deltaY = (sbyte)deltaY;
        this.deltaZ = (sbyte)deltaZ;
    }

    public override void read(Stream stream)
    {
        base.read(stream);
        deltaX = (sbyte)stream.ReadByte();
        deltaY = (sbyte)stream.ReadByte();
        deltaZ = (sbyte)stream.ReadByte();
    }

    public override void write(Stream stream)
    {
        base.write(stream);
        stream.WriteByte(deltaX);
        stream.WriteByte(deltaY);
        stream.WriteByte(deltaZ);
    }

    public override int size()
    {
        return 7;
    }
}
