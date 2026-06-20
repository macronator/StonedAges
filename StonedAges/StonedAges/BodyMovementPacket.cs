namespace StonedAges;

public class BodyMovementPacket : PacketStructure
{
    private uint _id;

    private byte _type;

    private ushort _speed;

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

    public byte AniType
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

    public ushort Speed
    {
        get
        {
            return ReadUShort(9);
        }
        set
        {
            _speed = value;
            WriteUShort(_speed, 9);
        }
    }

    public BodyMovementPacket(uint id, byte type, ushort speed)
        : base(11, 12)
    {
        ID = id;
        AniType = type;
        Speed = speed;
    }

    public BodyMovementPacket(byte[] packet)
        : base(packet)
    {
    }
}
