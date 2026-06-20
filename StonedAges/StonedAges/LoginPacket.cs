namespace StonedAges;

public class LoginPacket : PacketStructure
{
    private string _name;

    private string _pass;

    public string Name
    {
        get
        {
            int num = 4;
            int count = ReadByte(num);
            num++;
            return ReadString(num, count);
        }
        set
        {
            _name = value;
            int num = 4;
            WriteByte((byte)_name.Length, num);
            num++;
            WriteString(_name, num);
        }
    }

    public string Pass
    {
        get
        {
            int num = 5 + _name.Length;
            int count = ReadByte(num);
            num++;
            return ReadString(num, count);
        }
        set
        {
            _pass = value;
            int num = 5 + _name.Length;
            WriteByte((byte)_pass.Length, num);
            num++;
            WriteString(_pass, num);
        }
    }

    public LoginPacket(string name, string pass)
        : base((ushort)(5 + name.Length + 1 + pass.Length), 2)
    {
        Name = name;
        Pass = pass;
    }

    public LoginPacket(byte[] packet)
        : base(packet)
    {
    }
}
