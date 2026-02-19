using java.io;

namespace BetaSharp.Network.Packets.C2SPlay;

public class UpdateSelectedSlotC2SPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(UpdateSelectedSlotC2SPacket).TypeHandle);

    public int selectedSlot;

    public UpdateSelectedSlotC2SPacket()
    {
    }

    public UpdateSelectedSlotC2SPacket(int selectedSlot)
    {
        this.selectedSlot = selectedSlot;
    }

    public override void read(Stream stream)
    {
        selectedSlot = stream.ReadShort();
    }

    public override void write(Stream stream)
    {
        stream.WriteShort((short)selectedSlot);
    }

    public override void apply(NetHandler handler)
    {
        handler.onUpdateSelectedSlot(this);
    }

    public override int size()
    {
        return 2;
    }
}
