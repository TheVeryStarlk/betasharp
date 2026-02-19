using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class GlobalEntitySpawnS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(GlobalEntitySpawnS2CPacket).TypeHandle);

    public int id;
    public int x;
    public int y;
    public int z;
    public int type;

    public GlobalEntitySpawnS2CPacket()
    {
    }

    public GlobalEntitySpawnS2CPacket(Entity ent)
    {
        id = ent.id;
        x = MathHelper.floor_double(ent.x * 32.0D);
        y = MathHelper.floor_double(ent.y * 32.0D);
        z = MathHelper.floor_double(ent.z * 32.0D);
        if (ent is EntityLightningBolt)
        {
            type = 1;
        }

    }

    public override void read(Stream stream)
    {
        id = stream.ReadInt();
        type = (sbyte)stream.ReadByte();
        x = stream.ReadInt();
        y = stream.ReadInt();
        z = stream.ReadInt();
    }

    public override void write(Stream stream)
    {
        stream.WriteInt(id);
        stream.WriteByte((byte)type);
        stream.WriteInt(x);
        stream.WriteInt(y);
        stream.WriteInt(z);
    }

    public override void apply(NetHandler handler)
    {
        handler.onLightningEntitySpawn(this);
    }

    public override int size()
    {
        return 17;
    }
}
