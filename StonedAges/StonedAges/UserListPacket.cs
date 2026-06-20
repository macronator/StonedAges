using System.Collections.Generic;
using System.Linq;

namespace StonedAges;

public class UserListPacket : PacketStructure
{
    private List<UserS> _list;

    public List<UserS> List
    {
        get
        {
            List<UserS> list = new List<UserS>();
            int num = 4;
            int num2 = ReadUShort(num);
            num += 2;
            for (int i = 0; i < num2; i++)
            {
                UserS item = default(UserS);
                item.InGuild = ReadBool(num);
                num++;
                item.Nation = ReadByte(num);
                num++;
                item.Color = ReadByte(num);
                num++;
                byte b = ReadByte(num);
                num++;
                item.Title = ReadString(num, b);
                num += b;
                item.IsMaster = ReadBool(num);
                num++;
                byte b2 = ReadByte(num);
                num++;
                item.Name = ReadString(num, b2);
                num += b2;
                list.Add(item);
            }
            return list;
        }
        set
        {
            _list = value;
            int num = 4;
            WriteUShort((ushort)_list.Count(), num);
            num += 2;
            foreach (UserS item in _list)
            {
                WriteBool(item.InGuild, num);
                num++;
                WriteByte(item.Nation, num);
                num++;
                WriteByte(item.Color, num);
                num++;
                WriteByte((byte)item.Title.Length, num);
                num++;
                WriteString(item.Title, num);
                num += item.Title.Length;
                WriteBool(item.IsMaster, num);
                num++;
                WriteByte((byte)item.Name.Length, num);
                num++;
                WriteString(item.Name, num);
                num += item.Name.Length;
            }
        }
    }

    public UserListPacket(List<UserS> list, int length)
        : base((ushort)(4 + length), 30)
    {
        List = list;
    }

    public UserListPacket(byte[] packet)
        : base(packet)
    {
    }
}
