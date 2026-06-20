namespace StonedAges;

public class MessagePacket : PacketStructure
{
    private byte _type;

    private uint _id;

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

    public uint ID
    {
        get
        {
            return ReadUInt(5);
        }
        set
        {
            _id = value;
            WriteUInt(_id, 5);
        }
    }

    public string Message
    {
        get
        {
            ushort count = ReadUShort(9);
            return ReadString(11, count);
        }
        set
        {
            _message = value;
            WriteUShort((ushort)_message.Length, 9);
            WriteString(_message, 11);
        }
    }

    public MessagePacket(byte type, uint id, string message)
        : base((ushort)(11 + message.Length), 20)
    {
        MsgType = type;
        ID = id;
        Message = message;
    }

    public MessagePacket(byte[] packet)
        : base(packet)
    {
    }
}
