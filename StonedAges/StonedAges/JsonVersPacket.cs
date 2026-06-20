using System.Collections.Generic;
using System.Linq;

namespace StonedAges;

public class JsonVersPacket : PacketStructure
{
    private Dictionary<string, string> _list = new Dictionary<string, string>();

    public Dictionary<string, string> List
    {
        get
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            ushort num = 4;
            ushort num2 = ReadUShort(num);
            num += 2;
            for (int i = 0; i < num2; i++)
            {
                byte b = ReadByte(num);
                num++;
                string key = ReadString(num, b);
                num += b;
                byte b2 = ReadByte(num);
                num++;
                string value = ReadString(num, b2);
                num += b2;
                dictionary.Add(key, value);
            }
            return dictionary;
        }
        set
        {
            _list = value;
            ushort num = 4;
            WriteUShort((ushort)_list.Count(), num);
            num += 2;
            foreach (KeyValuePair<string, string> item in _list)
            {
                WriteByte((byte)item.Key.Count(), num);
                num++;
                WriteString(item.Key, num);
                num += (ushort)item.Key.Count();
                WriteByte((byte)item.Value.Count(), num);
                num++;
                WriteString(item.Value, num);
                num += (ushort)item.Value.Count();
            }
        }
    }

    public JsonVersPacket(Dictionary<string, string> list, ushort length)
        : base((ushort)(4 + length), 100)
    {
        List = list;
    }

    public JsonVersPacket(byte[] packet)
        : base(packet)
    {
    }
}
