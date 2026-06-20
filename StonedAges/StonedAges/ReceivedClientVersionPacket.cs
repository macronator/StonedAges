namespace StonedAges;

public class ReceivedClientVersionPacket : PacketStructure
{
    private string _version;

    public string Version
    {
        get
        {
            byte count = ReadByte(4);
            return ReadString(5, count);
        }
        set
        {
            _version = value;
            WriteByte((byte)_version.Length, 4);
            WriteString(_version, 5);
        }
    }

    public ReceivedClientVersionPacket(string version)
        : base((ushort)(5 + version.Length), 102)
    {
        Version = version;
    }

    public ReceivedClientVersionPacket(byte[] packet)
        : base(packet)
    {
    }
}
