namespace StonedAges;

public class RequestClientVersionPacket : PacketStructure
{
    public RequestClientVersionPacket()
        : base(4, 101)
    {
    }

    public RequestClientVersionPacket(byte[] packet)
        : base(packet)
    {
    }
}
