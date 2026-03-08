using System.Net.Sockets;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityDestroyS2CPacket() : PacketBaseEntity(PacketId.EntityDestroyS2C)
{

    public static EntityDestroyS2CPacket Get(int entityId)
    {
        var p = Get<EntityDestroyS2CPacket>(PacketId.EntityDestroyS2C);
        p.EntityId = entityId;
        return p;
    }

    public override void Apply(NetworkHandler handler)
    {
        handler.onEntityDestroy(this);
    }
}
