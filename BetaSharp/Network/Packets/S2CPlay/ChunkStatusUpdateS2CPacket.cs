using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class ChunkStatusUpdateS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(ChunkStatusUpdateS2CPacket).TypeHandle);

    public int x;
    public int z;
    public bool load;

    public ChunkStatusUpdateS2CPacket()
    {
        worldPacket = false;
    }

    public ChunkStatusUpdateS2CPacket(int x, int z, bool load)
    {
        worldPacket = false;
        this.x = x;
        this.z = z;
        this.load = load;
    }

    public override void read(Stream stream)
    {
        x = stream.ReadInt();
        z = stream.ReadInt();
        load = stream.ReadBoolean();
    }

    public override void write(Stream stream)
    {
        stream.WriteInt(x);
        stream.WriteInt(z);
        stream.WriteBoolean(load);
    }

    public override void apply(NetHandler handler)
    {
        handler.onChunkStatusUpdate(this);
    }

    public override int size()
    {
        return 9;
    }
}
