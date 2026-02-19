using BetaSharp.Entities;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class PlayerSleepUpdateS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(PlayerSleepUpdateS2CPacket).TypeHandle);

    public int id;
    public int x;
    public int y;
    public int z;
    public int status;

    public PlayerSleepUpdateS2CPacket()
    {
    }

    public PlayerSleepUpdateS2CPacket(Entity player, int status, int x, int y, int z)
    {
        this.status = status;
        this.x = x;
        this.y = y;
        this.z = z;
        this.id = player.id;
    }

    public override void read(Stream stream)
    {
        id = stream.ReadInt();
        status = (sbyte)stream.ReadByte();
        x = stream.ReadInt();
        y = (sbyte)stream.ReadByte();
        z = stream.ReadInt();
    }

    public override void write(Stream stream)
    {
        stream.WriteInt(id);
        stream.WriteByte((sbyte)status);
        stream.WriteInt(x);
        stream.WriteByte((sbyte)y);
        stream.WriteInt(z);
    }

    public override void apply(NetHandler handler)
    {
        handler.onPlayerSleepUpdate(this);
    }

    public override int size()
    {
        return 14;
    }
}
