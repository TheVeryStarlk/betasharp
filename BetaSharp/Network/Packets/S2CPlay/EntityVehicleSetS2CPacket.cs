using BetaSharp.Entities;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityVehicleSetS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityVehicleSetS2CPacket).TypeHandle);

    public int entityId;
    public int vehicleEntityId;

    public EntityVehicleSetS2CPacket()
    {
    }

    public EntityVehicleSetS2CPacket(Entity entity, Entity vehicle)
    {
        entityId = entity.id;
        vehicleEntityId = vehicle != null ? vehicle.id : -1;
    }

    public override int size()
    {
        return 8;
    }

    public override void read(Stream stream)
    {
        entityId = stream.ReadInt();
        vehicleEntityId = stream.ReadInt();
    }

    public override void write(Stream stream)
    {
        stream.WriteInt(entityId);
        stream.WriteInt(vehicleEntityId);
    }

    public override void apply(NetHandler handler)
    {
        handler.onEntityVehicleSet(this);
    }
}
