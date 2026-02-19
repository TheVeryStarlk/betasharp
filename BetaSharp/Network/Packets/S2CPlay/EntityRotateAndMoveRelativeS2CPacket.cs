using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityRotateAndMoveRelativeS2CPacket : EntityS2CPacket
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityRotateAndMoveRelativeS2CPacket).TypeHandle);

    public EntityRotateAndMoveRelativeS2CPacket()
    {
        rotate = true;
    }

    public EntityRotateAndMoveRelativeS2CPacket(int entityId, byte deltaX, byte deltaY, byte deltaZ, byte yaw, byte pitch) : base(entityId)
    {
        this.deltaX = (sbyte)deltaX;
        this.deltaY = (sbyte)deltaY;
        this.deltaZ = (sbyte)deltaZ;
        this.yaw = (sbyte)yaw;
        this.pitch = (sbyte)pitch;
        rotate = true;
    }

    public override void read(Stream stream)
    {
        base.read(stream);
        deltaX = (sbyte)stream.ReadByte();
        deltaY = (sbyte)stream.ReadByte();
        deltaZ = (sbyte)stream.ReadByte();
        yaw = (sbyte)stream.ReadByte();
        pitch = (sbyte)stream.ReadByte();
    }

    public override void write(Stream stream)
    {
        base.write(stream);
        stream.WriteByte(deltaX);
        stream.WriteByte(deltaY);
        stream.WriteByte(deltaZ);
        stream.WriteByte(yaw);
        stream.WriteByte(pitch);
    }

    public override int size()
    {
        return 9;
    }
}
