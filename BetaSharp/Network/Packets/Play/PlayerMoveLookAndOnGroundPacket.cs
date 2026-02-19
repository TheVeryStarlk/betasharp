using java.io;

namespace BetaSharp.Network.Packets.Play;

public class PlayerMoveLookAndOnGroundPacket : PlayerMovePacket
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(PlayerMoveLookAndOnGroundPacket).TypeHandle);

    public PlayerMoveLookAndOnGroundPacket()
    {
        changeLook = true;
    }

    public PlayerMoveLookAndOnGroundPacket(float yaw, float pitch, bool onGround)
    {
        base.yaw = yaw;
        base.pitch = pitch;
        base.onGround = onGround;
        changeLook = true;
    }

    public override void read(Stream stream)
    {
        yaw = stream.ReadFloat();
        pitch = stream.ReadFloat();
        base.read(stream);
    }

    public override void write(Stream stream)
    {
        stream.WriteFloat(yaw);
        stream.WriteFloat(pitch);
        base.write(stream);
    }

    public override int size()
    {
        return 9;
    }
}
