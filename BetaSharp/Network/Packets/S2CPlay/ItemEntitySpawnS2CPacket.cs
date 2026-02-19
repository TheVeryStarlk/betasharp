using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class ItemEntitySpawnS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(ItemEntitySpawnS2CPacket).TypeHandle);

    public int id;
    public int x;
    public int y;
    public int z;
    public sbyte velocityX;
    public sbyte velocityY;
    public sbyte velocityZ;
    public int itemRawId;
    public int itemCount;
    public int itemDamage;

    public ItemEntitySpawnS2CPacket()
    {
    }

    public ItemEntitySpawnS2CPacket(EntityItem item)
    {
        id = item.id;
        itemRawId = item.stack.itemId;
        itemCount = item.stack.count;
        itemDamage = item.stack.getDamage();
        x = MathHelper.floor_double(item.x * 32.0D);
        y = MathHelper.floor_double(item.y * 32.0D);
        z = MathHelper.floor_double(item.z * 32.0D);
        velocityX = (sbyte)(int)(item.velocityX * 128.0D);
        velocityY = (sbyte)(int)(item.velocityY * 128.0D);
        velocityZ = (sbyte)(int)(item.velocityZ * 128.0D);
    }

    public override void read(Stream stream)
    {
        id = stream.ReadInt();
        itemRawId = stream.ReadShort();
        itemCount = (sbyte)stream.ReadByte();
        itemDamage = stream.ReadShort();
        x = stream.ReadInt();
        y = stream.ReadInt();
        z = stream.ReadInt();
        velocityX = (sbyte)stream.ReadByte();
        velocityY = (sbyte)stream.ReadByte();
        velocityZ = (sbyte)stream.ReadByte();
    }

    public override void write(Stream stream)
    {
        stream.WriteInt(id);
        stream.WriteShort((short)itemRawId);
        stream.WriteByte((byte)itemCount);
        stream.WriteShort((short)itemDamage);
        stream.WriteInt(x);
        stream.WriteInt(y);
        stream.WriteInt(z);
        stream.WriteByte(velocityX);
        stream.WriteByte(velocityY);
        stream.WriteByte(velocityZ);
    }

    public override void apply(NetHandler handler)
    {
        handler.onItemEntitySpawn(this);
    }

    public override int size()
    {
        return 24;
    }
}
