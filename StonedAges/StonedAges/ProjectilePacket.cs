namespace StonedAges;

public class ProjectilePacket : PacketStructure
{
    private uint _id;

    private byte _type;

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

    public byte TYPE
    {
        get
        {
            return ReadByte(8);
        }
        set
        {
            _type = value;
            WriteByte(_type, 8);
        }
    }

    public ProjectilePacket(uint id, byte type)
        : base(9, 36)
    {
        ID = id;
        TYPE = type;
    }

    public ProjectilePacket(byte[] packet)
        : base(packet)
    {
    }
}
