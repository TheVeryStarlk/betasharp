using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityDestroyS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityDestroyS2CPacket).TypeHandle);

    public int entityId;

    public EntityDestroyS2CPacket()
    {
    }

    public EntityDestroyS2CPacket(int id)
    {
        entityId = id;
    }

    public override void read(Stream stream)
    {
        entityId = stream.ReadInt();
    }

    public override void write(Stream stream)
    {
        stream.WriteInt(entityId);
    }

    public override void apply(NetHandler handler)
    {
        handler.onEntityDestroy(this);
    }

    public override int size()
    {
        return 4;
    }
}
