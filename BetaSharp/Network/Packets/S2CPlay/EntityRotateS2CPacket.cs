using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityRotateS2CPacket : EntityS2CPacket
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityRotateS2CPacket).TypeHandle);

    public EntityRotateS2CPacket()
    {
        rotate = true;
    }

    public EntityRotateS2CPacket(int entityId, byte yaw, byte pitch) : base(entityId)
    {
        this.yaw = (sbyte)yaw;
        this.pitch = (sbyte)pitch;
        rotate = true;
    }

    public override void read(Stream stream)
    {
        base.read(stream);
        yaw = (sbyte)stream.ReadByte();
        pitch = (sbyte)stream.ReadByte();
    }

    public override void write(Stream stream)
    {
        base.write(stream);
        stream.WriteByte(yaw);
        stream.WriteByte(pitch);
    }

    public override int size()
    {
        return 6;
    }
}
