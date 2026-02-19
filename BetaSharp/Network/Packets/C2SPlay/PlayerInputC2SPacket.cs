using java.io;

namespace BetaSharp.Network.Packets.C2SPlay;

public class PlayerInputC2SPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(PlayerInputC2SPacket).TypeHandle);

    private float sideways;
    private float forward;
    private bool jumping;
    private bool sneaking;
    private float pitch;
    private float yaw;

    public override void read(Stream stream)
    {
        sideways = stream.ReadFloat();
        forward = stream.ReadFloat();
        pitch = stream.ReadFloat();
        yaw = stream.ReadFloat();
        jumping = stream.ReadBoolean();
        sneaking = stream.ReadBoolean();
    }

    public override void write(Stream stream)
    {
        stream.WriteFloat(sideways);
        stream.WriteFloat(forward);
        stream.WriteFloat(pitch);
        stream.WriteFloat(yaw);
        stream.WriteBoolean(jumping);
        stream.WriteBoolean(sneaking);
    }

    public override void apply(NetHandler handler)
    {
        handler.onPlayerInput(this);
    }

    public override int size()
    {
        return 18;
    }

    public float getSideways()
    {
        return sideways;
    }

    public float getPitch()
    {
        return pitch;
    }

    public float getForward()
    {
        return forward;
    }

    public float getYaw()
    {
        return yaw;
    }

    public bool isJumping()
    {
        return jumping;
    }

    public bool isSneaking()
    {
        return sneaking;
    }
}
