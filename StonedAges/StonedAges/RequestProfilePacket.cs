namespace StonedAges;

public class RequestProfilePacket : PacketStructure
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

    public RequestProfilePacket(uint id)
        : base(8, 27)
    {
        ID = id;
    }

    public RequestProfilePacket(byte[] packet)
        : base(packet)
    {
    }
}
