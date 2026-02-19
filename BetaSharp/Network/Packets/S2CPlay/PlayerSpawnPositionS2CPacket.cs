using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class PlayerSpawnPositionS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(PlayerSpawnPositionS2CPacket).TypeHandle);

    public int x;
    public int y;
    public int z;

    public PlayerSpawnPositionS2CPacket()
    {
    }

    public PlayerSpawnPositionS2CPacket(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public override void read(Stream stream)
    {
        x = stream.ReadInt();
        y = stream.ReadInt();
        z = stream.ReadInt();
    }

    public override void write(Stream stream)
    {
        stream.WriteInt(x);
        stream.WriteInt(y);
        stream.WriteInt(z);
    }

    public override void apply(NetHandler handler)
    {
        handler.onPlayerSpawnPosition(this);
    }

    public override int size()
    {
        return 12;
    }
}
