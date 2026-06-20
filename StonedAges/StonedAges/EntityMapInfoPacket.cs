namespace StonedAges;

public class EntityMapInfoPacket : PacketStructure
{
    private ushort _num;

    private byte _x;

    private byte _y;

    public ushort Number
    {
        get
        {
            return ReadUShort(4);
        }
        set
        {
            _num = value;
            WriteUShort(_num, 4);
        }
    }

    public byte X
    {
        get
        {
            return ReadByte(6);
        }
        set
        {
            _x = value;
            WriteByte(_x, 6);
        }
    }

    public byte Y
    {
        get
        {
            return ReadByte(7);
        }
        set
        {
            _y = value;
            WriteByte(_y, 7);
        }
    }

    public EntityMapInfoPacket(ushort num, byte x, byte y)
        : base(8, 7)
    {
        Number = num;
        X = x;
        Y = y;
    }

    public EntityMapInfoPacket(byte[] packet)
        : base(packet)
    {
    }
}
