namespace StonedAges;

public class CharacterTurnPacket : PacketStructure
{
    private uint _id;

    private byte _direction;

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

    public byte Direction
    {
        get
        {
            return ReadByte(8);
        }
        set
        {
            _direction = value;
            WriteByte(_direction, 8);
        }
    }

    public CharacterTurnPacket(uint id, byte direction)
        : base(9, 11)
    {
        ID = id;
        Direction = direction;
    }

    public CharacterTurnPacket(byte[] packet)
        : base(packet)
    {
    }
}
