namespace StonedAges;

public class RequestEntityMapInfoPacket : PacketStructure
{
    private string _name;

    public string Name
    {
        get
        {
            short count = ReadByte(4);
            return ReadString(5, count);
        }
        set
        {
            _name = value;
            WriteByte((byte)_name.Length, 4);
            WriteString(_name, 5);
        }
    }

    public RequestEntityMapInfoPacket(string name)
        : base((ushort)(5 + name.Length), 6)
    {
        Name = name;
    }

    public RequestEntityMapInfoPacket(byte[] packet)
        : base(packet)
    {
    }
}
