namespace StonedAges;

public class RemoveEntityPacket : PacketStructure
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

    public RemoveEntityPacket(uint id)
        : base(8, 14)
    {
        ID = id;
    }

    public RemoveEntityPacket(byte[] packet)
        : base(packet)
    {
    }
}
