using java.io;

namespace BetaSharp.Network.Packets.Play;

public class PlayerMovePacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(PlayerMovePacket).TypeHandle);

    public double x;
    public double y;
    public double z;
    public double eyeHeight;
    public float yaw;
    public float pitch;
    public bool onGround;
    public bool changePosition;
    public bool changeLook;

    public PlayerMovePacket()
    {
    }

    public PlayerMovePacket(bool onGround)
    {
        this.onGround = onGround;
    }

    public override void apply(NetHandler handler)
    {
        handler.onPlayerMove(this);
    }

    public override void read(Stream stream)
    {
        onGround = stream.ReadBoolean();
    }

    public override void write(Stream stream)
    {
        stream.WriteBoolean(onGround);
    }

    public override int size()
    {
        return 1;
    }
}
