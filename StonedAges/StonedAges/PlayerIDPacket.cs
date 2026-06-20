namespace StonedAges;

public class PlayerIDPacket : PacketStructure
{
    private uint _id;

    public uint ID
    {
        get
        {
            return ReadUInt(4);
        }
        set
        {
            _id = value;
            WriteUInt(_id, 4);
        }
    }

    public PlayerIDPacket(uint id)
        : base(8, 5)
    {
        ID = id;
    }

    public PlayerIDPacket(byte[] packet)
        : base(packet)
    {
    }
}
