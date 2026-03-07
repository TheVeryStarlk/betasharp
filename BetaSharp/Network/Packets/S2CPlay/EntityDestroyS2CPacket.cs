using System.Net.Sockets;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityDestroyS2CPacket() : PacketBaseEntity(PacketId.EntityDestroyS2C)
{

    public EntityDestroyS2CPacket(int entityId) : this()
    {
        EntityId = entityId;
    }

    public override void Apply(NetworkHandler handler)
    {
        handler.onEntityDestroy(this);
    }
}
