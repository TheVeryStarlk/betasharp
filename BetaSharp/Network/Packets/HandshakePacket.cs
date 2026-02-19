using BetaSharp.NBT;
using java.io;

namespace BetaSharp.Network.Packets;

public class HandshakePacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(HandshakePacket).TypeHandle);

    public string username;

    public HandshakePacket()
    {
    }

    public HandshakePacket(string username)
    {
        this.username = username;
    }

    public override void read(Stream stream)
    {
        username = stream.ReadString( 32);
    }

    public override void write(Stream stream)
    {
        stream.WriteString(username);
    }

    public override void apply(NetHandler handler)
    {
        handler.onHandshake(this);
    }

    public override int size()
    {
        return 4 + username.Length + 4;
    }
}
