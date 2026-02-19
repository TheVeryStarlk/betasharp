using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using java.io;
using java.util;

namespace BetaSharp.Network.Packets.S2CPlay;

public class LivingEntitySpawnS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(LivingEntitySpawnS2CPacket).TypeHandle);

    public int entityId;
    public sbyte type;
    public int xPosition;
    public int yPosition;
    public int zPosition;
    public sbyte yaw;
    public sbyte pitch;
    private DataWatcher metaData;
    private List receivedMetadata;

    public LivingEntitySpawnS2CPacket()
    {
    }

    public LivingEntitySpawnS2CPacket(EntityLiving ent)
    {
        entityId = ent.id;
        type = (sbyte)EntityRegistry.getRawId(ent);
        xPosition = MathHelper.floor_double(ent.x * 32.0D);
        yPosition = MathHelper.floor_double(ent.y * 32.0D);
        zPosition = MathHelper.floor_double(ent.z * 32.0D);
        yaw = (sbyte)(int)(ent.yaw * 256.0F / 360.0F);
        pitch = (sbyte)(int)(ent.pitch * 256.0F / 360.0F);
        metaData = ent.getDataWatcher();
    }

    public override void read(Stream stream)
    {
        entityId = stream.ReadInt();
        type = (sbyte)stream.ReadByte();
        xPosition = stream.ReadInt();
        yPosition = stream.ReadInt();
        zPosition = stream.ReadInt();
        yaw = (sbyte)stream.ReadByte();
        pitch = (sbyte)stream.ReadByte();
        receivedMetadata = DataWatcher.readWatchableObjects(stream);
    }

    public override void write(Stream stream)
    {
        stream.WriteInt(entityId);
        stream.WriteByte(type);
        stream.WriteInt(xPosition);
        stream.WriteInt(yPosition);
        stream.WriteInt(zPosition);
        stream.WriteByte(yaw);
        stream.WriteByte(pitch);
        metaData.writeWatchableObjects(stream);
    }

    public override void apply(NetHandler handler)
    {
        handler.onLivingEntitySpawn(this);
    }

    public override int size()
    {
        return 20;
    }

    public List getMetadata()
    {
        return receivedMetadata;
    }
}
