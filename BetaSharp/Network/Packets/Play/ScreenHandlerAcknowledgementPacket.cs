using java.io;

namespace BetaSharp.Network.Packets.Play;

public class ScreenHandlerAcknowledgementPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(ScreenHandlerAcknowledgementPacket).TypeHandle);

    public int syncId;
    public short actionType;
    public bool accepted;

    public ScreenHandlerAcknowledgementPacket()
    {
    }

    public ScreenHandlerAcknowledgementPacket(int syncId, short actionType, bool accepted)
    {
        this.syncId = syncId;
        this.actionType = actionType;
        this.accepted = accepted;
    }

    public override void apply(NetHandler handler)
    {
        handler.onScreenHandlerAcknowledgement(this);
    }

    public override void read(Stream stream)
    {
        syncId = (sbyte)stream.ReadByte();
        actionType = stream.ReadShort();
        accepted = (sbyte)stream.ReadByte() != 0;
    }

    public override void write(Stream stream)
    {
        stream.WriteByte((sbyte)syncId);
        stream.WriteShort(actionType);
        stream.WriteBoolean(accepted);
    }

    public override int size()
    {
        return 4;
    }
}
