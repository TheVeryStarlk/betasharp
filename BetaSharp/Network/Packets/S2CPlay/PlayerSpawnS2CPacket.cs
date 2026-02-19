using BetaSharp.Entities;
using BetaSharp.Items;
using BetaSharp.NBT;
using BetaSharp.Util.Maths;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class PlayerSpawnS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(PlayerSpawnS2CPacket).TypeHandle);

    public int entityId;
    public string name;
    public int xPosition;
    public int yPosition;
    public int zPosition;
    public sbyte rotation;
    public sbyte pitch;
    public int currentItem;

    public PlayerSpawnS2CPacket()
    {
    }

    public PlayerSpawnS2CPacket(EntityPlayer ent)
    {
        entityId = ent.id;
        name = ent.name;
        xPosition = MathHelper.floor_double(ent.x * 32.0D);
        yPosition = MathHelper.floor_double(ent.y * 32.0D);
        zPosition = MathHelper.floor_double(ent.z * 32.0D);
        rotation = (sbyte)(int)(ent.yaw * 256.0F / 360.0F);
        pitch = (sbyte)(int)(ent.pitch * 256.0F / 360.0F);
        ItemStack itemStack = ent.inventory.getSelectedItem();
        currentItem = itemStack == null ? 0 : itemStack.itemId;
    }

    public override void read(Stream stream)
    {
        entityId = stream.ReadInt();
        name = stream.ReadString(16);
        xPosition = stream.ReadInt();
        yPosition = stream.ReadInt();
        zPosition = stream.ReadInt();
        rotation = (sbyte)stream.ReadByte();
        pitch = (sbyte)stream.ReadByte();
        currentItem = stream.ReadShort();
    }

    public override void write(Stream stream)
    {
        stream.WriteInt(entityId);
        stream.WriteString(name);
        stream.WriteInt(xPosition);
        stream.WriteInt(yPosition);
        stream.WriteInt(zPosition);
        stream.WriteByte(rotation);
        stream.WriteByte(pitch);
        stream.WriteShort((short)currentItem);
    }

    public override void apply(NetHandler handler)
    {
        handler.onPlayerSpawn(this);
    }

    public override int size()
    {
        return 28;
    }
}
