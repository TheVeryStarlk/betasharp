using java.io;

namespace BetaSharp.Network.Packets.C2SPlay;

public class PlayerInteractEntityC2SPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(PlayerInteractEntityC2SPacket).TypeHandle);

    public int playerId;
    public int entityId;
    public int isLeftClick;

    public PlayerInteractEntityC2SPacket()
    {
    }

    public PlayerInteractEntityC2SPacket(int playerId, int entityId, int isLeftClick)
    {
        this.playerId = playerId;
        this.entityId = entityId;
        this.isLeftClick = isLeftClick;
    }

    public override void read(Stream stream)
    {
        playerId = stream.ReadInt();
        entityId = stream.ReadInt();
        isLeftClick = (sbyte)stream.ReadByte();
    }

    public override void write(Stream stream)
    {
        stream.WriteInt(playerId);
        stream.WriteInt(entityId);
        stream.WriteByte((byte)isLeftClick);
    }

    public override void apply(NetHandler handler)
    {
        handler.handleInteractEntity(this);
    }

    public override int size()
    {
        return 9;
    }
}
