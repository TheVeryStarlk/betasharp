using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityPositionS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityPositionS2CPacket).TypeHandle);

    public int id;
    public int x;
    public int y;
    public int z;
    public sbyte yaw;
    public sbyte pitch;

    public EntityPositionS2CPacket()
    {
    }

    public EntityPositionS2CPacket(int entityId, int x, int y, int z, byte yaw, byte pitch)
    {
        this.id = entityId;
        this.x = x;
        this.y = y;
        this.z = z;
        this.yaw = (sbyte)yaw;
        this.pitch = (sbyte)pitch;
    }

    public EntityPositionS2CPacket(Entity var1)
    {
        id = var1.id;
        x = MathHelper.floor_double(var1.x * 32.0D);
        y = MathHelper.floor_double(var1.y * 32.0D);
        z = MathHelper.floor_double(var1.z * 32.0D);
        yaw = (sbyte)(int)(var1.yaw * 256.0F / 360.0F);
        pitch = (sbyte)(int)(var1.pitch * 256.0F / 360.0F);
    }

    public override void read(Stream stream)
    {
        id = stream.ReadInt();
        x = stream.ReadInt();
        y = stream.ReadInt();
        z = stream.ReadInt();
        yaw = (sbyte)stream.ReadByte();
        pitch = (sbyte)stream.ReadByte();
    }

    public override void write(Stream stream)
    {
        stream.WriteInt(id);
        stream.WriteInt(x);
        stream.WriteInt(y);
        stream.WriteInt(z);
        stream.WriteInt(yaw);
        stream.WriteInt(pitch);
    }

    public override void apply(NetHandler handler)
    {
        handler.onEntityPosition(this);
    }

    public override int size()
    {
        return 34;
    }
}
