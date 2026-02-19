using BetaSharp.Items;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityEquipmentUpdateS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityEquipmentUpdateS2CPacket).TypeHandle);

    public int id;
    public int slot;
    public int itemRawId;
    public int itemDamage;

    public EntityEquipmentUpdateS2CPacket()
    {
    }

    public EntityEquipmentUpdateS2CPacket(int id, int slot, ItemStack itemStack)
    {
        this.id = id;
        this.slot = slot;
        if (itemStack == null)
        {
            itemRawId = -1;
            itemDamage = 0;
        }
        else
        {
            itemRawId = itemStack.itemId;
            itemDamage = itemStack.getDamage();
        }
    }

    public override void read(Stream stream)
    {
        id = stream.ReadInt();
        slot = stream.ReadShort();
        itemRawId = stream.ReadShort();
        itemDamage = stream.ReadShort();
    }

    public override void write(Stream stream)
    {
        stream.WriteInt(id);
        stream.WriteShort((short)slot);
        stream.WriteShort((short)itemRawId);
        stream.WriteShort((short)itemDamage);
    }

    public override void apply(NetHandler handler)
    {
        handler.onEntityEquipmentUpdate(this);
    }

    public override int size()
    {
        return 8;
    }
}
