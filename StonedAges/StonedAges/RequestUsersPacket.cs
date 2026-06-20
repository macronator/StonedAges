namespace StonedAges;

public class RequestUsersPacket : PacketStructure
{
    public RequestUsersPacket()
        : base(4, 29)
    {
    }

    public RequestUsersPacket(byte[] packet)
        : base(packet)
    {
    }
}
