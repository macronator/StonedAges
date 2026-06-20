namespace StonedAges;

public class LogoutPacket : PacketStructure
{
    public LogoutPacket()
        : base(4, 3)
    {
    }

    public LogoutPacket(byte[] packet)
        : base(packet)
    {
    }
}
