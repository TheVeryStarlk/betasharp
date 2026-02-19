using java.io;

namespace BetaSharp.Network.Packets.Play;

public class PlayerMovePositionAndOnGroundPacket : PlayerMovePacket
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(PlayerMovePositionAndOnGroundPacket).TypeHandle);

    public PlayerMovePositionAndOnGroundPacket()
    {
        changePosition = true;
    }

    public PlayerMovePositionAndOnGroundPacket(double x, double y, double eyeHeight, double z, bool onGround)
    {
        base.x = x;
        base.y = y;
        base.eyeHeight = eyeHeight;
        base.z = z;
        base.onGround = onGround;
        changePosition = true;
    }

    public override void read(Stream stream)
    {
        x = stream.ReadDouble();
        y = stream.ReadDouble();
        eyeHeight = stream.ReadDouble();
        z = stream.ReadDouble();
        base.read(stream);
    }

    public override void write(Stream stream)
    {
        stream.WriteDouble(x);
        stream.WriteDouble(y);
        stream.WriteDouble(eyeHeight);
        stream.WriteDouble(z);
        base.write(stream);
    }

    public override int size()
    {
        return 33;
    }
}
