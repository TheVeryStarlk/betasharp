using java.io;

namespace BetaSharp.Network.Packets.Play;

public class PlayerMoveFullPacket : PlayerMovePacket
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(PlayerMoveFullPacket).TypeHandle);

    public PlayerMoveFullPacket()
    {
        changeLook = true;
        changePosition = true;
    }

    public PlayerMoveFullPacket(double x, double y, double eyeHeight, double z, float yaw, float pitch, bool onGround)
    {
        base.x = x;
        base.y = y;
        base.z = z;
        base.eyeHeight = eyeHeight;
        base.yaw = yaw;
        base.pitch = pitch;
        base.onGround = onGround;
        changeLook = true;
        changePosition = true;
    }

    public override void read(Stream stream)
    {
        x = stream.ReadDouble();
        y = stream.ReadDouble();
        eyeHeight = stream.ReadDouble();
        z = stream.ReadDouble();
        yaw = stream.ReadFloat();
        pitch = stream.ReadFloat();
        base.read(stream);
    }

    public override void write(Stream stream)
    {
        stream.WriteDouble(x);
        stream.WriteDouble(y);
        stream.WriteDouble(eyeHeight);
        stream.WriteDouble(z);
        stream.WriteFloat(yaw);
        stream.WriteFloat(pitch);
        base.write(stream);
    }

    public override int size()
    {
        return 41;
    }
}
