namespace StonedAges;

public class GroupRequestPacket : PacketStructure
{
    private string _name;

    private byte _type;

    public byte Type
    {
        get
        {
            return ReadByte(4);
        }
        set
        {
            _type = value;
            WriteByte(_type, 4);
        }
    }

    public string Name
    {
        get
        {
            byte count = ReadByte(5);
            return ReadString(6, count);
        }
        set
        {
            _name = value;
            WriteByte((byte)_name.Length, 5);
            WriteString(_name, 6);
        }
    }

    public GroupRequestPacket(byte type, string name)
        : base((ushort)(6 + name.Length), 8)
    {
        Type = type;
        Name = name;
    }

    public GroupRequestPacket(byte[] packet)
        : base(packet)
    {
    }
}
