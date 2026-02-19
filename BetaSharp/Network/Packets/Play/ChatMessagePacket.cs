using BetaSharp.NBT;
using java.io;

namespace BetaSharp.Network.Packets.Play;

public class ChatMessagePacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(ChatMessagePacket).TypeHandle);

    public string chatMessage;

    public ChatMessagePacket()
    {
    }

    public ChatMessagePacket(string msg)
    {
        if (msg.Length > 119)
        {
            msg = msg.Substring(0, 119);
        }

        chatMessage = msg;
    }

    public override void read(Stream stream)
    {
        chatMessage = stream.ReadString( 119);
    }

    public override void write(Stream stream)
    {
        stream.WriteString(chatMessage);
    }

    public override void apply(NetHandler handler)
    {
        handler.onChatMessage(this);
    }

    public override int size()
    {
        return chatMessage.Length;
    }
}
