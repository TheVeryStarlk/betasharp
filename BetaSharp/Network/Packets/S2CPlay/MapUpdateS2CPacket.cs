using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class MapUpdateS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(MapUpdateS2CPacket).TypeHandle);

    public short itemRawId;
    public short id;
    public byte[] updateData;

    public MapUpdateS2CPacket()
    {
        worldPacket = true;
    }

    public MapUpdateS2CPacket(short itemRawId, short id, byte[] updateData)
    {
        worldPacket = true;
        this.itemRawId = itemRawId;
        this.id = id;
        this.updateData = updateData;
    }

    public override void read(Stream stream)
    {
        itemRawId = stream.ReadShort();
        id = stream.ReadShort();
        updateData = new byte[(sbyte)stream.ReadByte() & 255];
        stream.ReadExactly(updateData);
    }

    public override void write(Stream stream)
    {
        stream.WriteShort(itemRawId);
        stream.WriteShort(id);
        stream.WriteByte((byte)updateData.Length);
        stream.Write(updateData);
    }

    public override void apply(NetHandler handler)
    {
        handler.onMapUpdate(this);
    }

    public override int size()
    {
        return 4 + updateData.Length;
    }
}
