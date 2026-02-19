using BetaSharp.Worlds;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class BlockUpdateS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(BlockUpdateS2CPacket).TypeHandle);

    public int x;
    public int y;
    public int z;
    public int blockRawId;
    public int blockMetadata;

    public BlockUpdateS2CPacket()
    {
        worldPacket = false;
    }

    public BlockUpdateS2CPacket(int x, int y, int z, World world)
    {
        worldPacket = false;
        this.x = x;
        this.y = y;
        this.z = z;
        blockRawId = world.getBlockId(x, y, z);
        blockMetadata = world.getBlockMeta(x, y, z);
    }

    public override void read(Stream stream)
    {
        x = stream.ReadInt();
        y = stream.ReadInt();
        z = stream.ReadInt();
        blockRawId = stream.ReadInt();
        blockMetadata = stream.ReadInt();
    }

    public override void write(Stream stream)
    {
        stream.WriteInt(x);
        stream.WriteInt(y);
        stream.WriteInt(z);
        stream.WriteInt(blockRawId);
        stream.WriteInt(blockMetadata);
    }

    public override void apply(NetHandler handler)
    {
        handler.onBlockUpdate(this);
    }

    public override int size()
    {
        return 11;
    }
}
