using BetaSharp.Worlds;
using BetaSharp.Worlds.Chunks;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class ChunkDeltaUpdateS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(ChunkDeltaUpdateS2CPacket).TypeHandle);

    public int x;
    public int z;
    public short[] positions;
    public byte[] blockRawIds;
    public byte[] blockMetadata;
    public int _size;

    public ChunkDeltaUpdateS2CPacket()
    {
        worldPacket = true;
    }

    public ChunkDeltaUpdateS2CPacket(int x, int z, short[] positions, int size, World world)
    {
        worldPacket = true;
        this.x = x;
        this.z = z;
        this._size = size;
        this.positions = new short[size];
        blockRawIds = new byte[size];
        blockMetadata = new byte[size];
        Chunk chunk = world.getChunk(x, z);

        for (int i = 0; i < size; i++)
        {
            int blockX = positions[i] >> 12 & 15;
            int blockZ = positions[i] >> 8 & 15;
            int blockY = positions[i] & 255;
            this.positions[i] = positions[i];
            blockRawIds[i] = (byte)chunk.getBlockId(blockX, blockY, blockZ);
            blockMetadata[i] = (byte)chunk.getBlockMeta(blockX, blockY, blockZ);
        }
    }

    public override void read(Stream stream)
    {
        x = stream.ReadInt();
        z = stream.ReadInt();
        _size = stream.ReadShort() & '\uffff';
        positions = new short[_size];

        blockRawIds = new byte[_size];
        blockMetadata = new byte[_size];

        for (int i = 0; i < _size; ++i)
        {
            positions[i] = stream.ReadShort();
        }

        stream.ReadExactly(blockRawIds);
        stream.ReadExactly(blockMetadata);
    }

    public override void write(Stream stream)
    {
        stream.WriteInt(x);
        stream.WriteInt(z);
        stream.WriteShort((short)_size);

        for (int i = 0; i < _size; ++i)
        {
            stream.WriteShort(positions[i]);
        }

        stream.Write(blockRawIds);
        stream.Write(blockMetadata);
    }

    public override void apply(NetHandler handler)
    {
        handler.onChunkDeltaUpdate(this);
    }

    public override int size()
    {
        return 10 + _size * 4;
    }
}
