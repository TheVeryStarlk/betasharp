using BetaSharp.Items;
using java.io;

namespace BetaSharp.Network.Packets.C2SPlay;

public class PlayerInteractBlockC2SPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(PlayerInteractBlockC2SPacket).TypeHandle);

    public int x;
    public int y;
    public int z;
    public int side;
    public ItemStack stack;

    public PlayerInteractBlockC2SPacket()
    {
    }

    public PlayerInteractBlockC2SPacket(int x, int y, int z, int side, ItemStack stack)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.side = side;
        this.stack = stack;
    }

    public override void read(Stream stream)
    {
        x = stream.ReadInt();
        y = stream.ReadInt();
        z = stream.ReadInt();
        side = stream.ReadInt();
        short itemId = stream.ReadShort();
        if (itemId >= 0)
        {
            sbyte count = (sbyte)stream.ReadByte();
            short damage = stream.ReadShort();
            stack = new ItemStack(itemId, count, damage);
        }
        else
        {
            stack = null;
        }

    }

    public override void write(Stream stream)
    {
        stream.WriteInt(x);
        stream.WriteInt(y);
        stream.WriteInt(z);
        stream.WriteInt(side);
        if (stack == null)
        {
            stream.WriteShort(-1);
        }
        else
        {
            stream.WriteShort((short)stack.itemId);
            stream.WriteByte((byte)stack.count);
            stream.WriteShort((short)stack.getDamage());
        }

    }

    public override void apply(NetHandler handler)
    {
        handler.onPlayerInteractBlock(this);
    }

    public override int size()
    {
        return 15;
    }
}
