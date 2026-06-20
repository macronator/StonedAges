namespace StonedAges;

public class SysMessagePacket : PacketStructure
{
    private byte _type;

    private string _message;

    public byte MsgType
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

    public string Message
    {
        get
        {
            ushort count = ReadUShort(5);
            return ReadString(7, count);
        }
        set
        {
            _message = value;
            WriteUShort((ushort)_message.Length, 5);
            WriteString(_message, 7);
        }
    }

    public SysMessagePacket(byte type, string message)
        : base((ushort)(7 + message.Length), 21)
    {
        MsgType = type;
        Message = message;
    }

    public SysMessagePacket(byte[] packet)
        : base(packet)
    {
    }
}
