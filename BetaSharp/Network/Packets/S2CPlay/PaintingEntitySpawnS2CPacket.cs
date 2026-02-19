using BetaSharp.Entities;
using BetaSharp.NBT;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class PaintingEntitySpawnS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(PaintingEntitySpawnS2CPacket).TypeHandle);

    public int entityId;
    public int xPosition;
    public int yPosition;
    public int zPosition;
    public int direction;
    public string title;

    public PaintingEntitySpawnS2CPacket()
    {
    }

    public PaintingEntitySpawnS2CPacket(EntityPainting paint)
    {
        entityId = paint.id;
        xPosition = paint.xPosition;
        yPosition = paint.yPosition;
        zPosition = paint.zPosition;
        direction = paint.direction;
        title = paint.art.title;
    }

    public override void read(Stream stream)
    {
        entityId = stream.ReadInt();
        title = stream.ReadString(EnumArt.maxArtTitleLength);
        xPosition = stream.ReadInt();
        yPosition = stream.ReadInt();
        zPosition = stream.ReadInt();
        direction = stream.ReadInt();
    }

    public override void write(Stream stream)
    {
        stream.WriteInt(entityId);
        stream.WriteString(title);
        stream.WriteInt(xPosition);
        stream.WriteInt(yPosition);
        stream.WriteInt(zPosition);
        stream.WriteInt(direction);
    }

    public override void apply(NetHandler handler)
    {
        handler.onPaintingEntitySpawn(this);
    }

    public override int size()
    {
        return 24;
    }
}
