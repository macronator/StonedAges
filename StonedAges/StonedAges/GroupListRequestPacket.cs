namespace StonedAges;

public class GroupListRequestPacket : PacketStructure
{
    public GroupListRequestPacket()
        : base(4, 9)
    {
    }

    public GroupListRequestPacket(byte[] packet)
        : base(packet)
    {
    }
}
