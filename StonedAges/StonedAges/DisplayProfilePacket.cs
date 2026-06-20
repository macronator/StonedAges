using System.Collections.Generic;
using System.Linq;

namespace StonedAges;

public class DisplayProfilePacket : PacketStructure
{
    private DisplayProfileS _profile;

    public DisplayProfileS Profile
    {
        get
        {
            int num = 4;
            DisplayProfileS result = default(DisplayProfileS);
            result.ID = ReadUInt(num);
            num += 4;
            int num2 = ReadByte(num);
            num++;
            result.Name = ReadString(num, num2);
            num += num2;
            result.Nation = ReadByte(num);
            num++;
            num2 = ReadByte(num);
            num++;
            result.Title = ReadString(num, num2);
            num += num2;
            result.AllowGroup = ReadByte(num);
            num++;
            byte b = ReadByte(num);
            num++;
            result.GroupList = new List<GroupMemberS>();
            for (int i = 0; i < b; i++)
            {
                GroupMemberS item = default(GroupMemberS);
                int num3 = ReadByte(num);
                num++;
                item.Name = ReadString(num, num3);
                num += num3;
                item.Leader = ReadBool(num);
                num++;
                item.X = ReadByte(num);
                num++;
                item.Y = ReadByte(num);
                num++;
                item.Map = ReadUShort(num);
                num += 2;
                result.GroupList.Add(item);
            }
            num2 = ReadByte(num);
            num++;
            result.Rank = ReadString(num, num2);
            num += num2;
            num2 = ReadByte(num);
            num++;
            result.Level = ReadString(num, num2);
            num += num2;
            num2 = ReadByte(num);
            num++;
            result.Guild = ReadString(num, num2);
            num += num2;
            ushort num4 = ReadUShort(num);
            num += 2;
            result.Equip = new List<EquippedS>();
            for (int j = 0; j < num4; j++)
            {
                EquippedS item2 = default(EquippedS);
                item2.Slot = ReadByte(num);
                num++;
                item2.Color = ReadByte(num);
                num++;
                int num5 = ReadByte(num);
                num++;
                item2.Name = ReadString(num, num5);
                num += num5;
                item2.Dura = ReadUInt(num);
                num += 4;
                result.Equip.Add(item2);
            }
            ushort num6 = ReadUShort(num);
            num += 2;
            result.Legend = new List<LegendMarkS>();
            for (int k = 0; k < num6; k++)
            {
                LegendMarkS item3 = default(LegendMarkS);
                item3.Icon = ReadByte(num);
                num++;
                item3.Color = ReadByte(num);
                num++;
                int num7 = ReadByte(num);
                num++;
                item3.ID = ReadString(num, num7);
                num += num7;
                num7 = ReadByte(num);
                num++;
                item3.Text = ReadString(num, num7);
                num += num7;
                result.Legend.Add(item3);
            }
            return result;
        }
        set
        {
            _profile = value;
            int num = 4;
            WriteUInt(_profile.ID, num);
            num += 4;
            WriteByte((byte)_profile.Name.Length, num);
            num++;
            WriteString(_profile.Name, num);
            num += _profile.Name.Length;
            WriteByte(_profile.Nation, num);
            num++;
            WriteByte((byte)_profile.Title.Length, num);
            num++;
            WriteString(_profile.Title, num);
            num += _profile.Title.Length;
            WriteByte(_profile.AllowGroup, num);
            num++;
            WriteByte((byte)_profile.GroupList.Count(), num);
            num++;
            foreach (GroupMemberS group in _profile.GroupList)
            {
                WriteByte((byte)group.Name.Length, num);
                num++;
                WriteString(group.Name, num);
                num += group.Name.Length;
                WriteBool(group.Leader, num);
                num++;
                WriteByte(group.X, num);
                num++;
                WriteByte(group.Y, num);
                num++;
                WriteUShort(group.Map, num);
                num += 2;
            }
            WriteByte((byte)_profile.Rank.Length, num);
            num++;
            WriteString(_profile.Rank, num);
            num += _profile.Rank.Length;
            WriteByte((byte)_profile.Level.Length, num);
            num++;
            WriteString(_profile.Level, num);
            num += _profile.Level.Length;
            WriteByte((byte)_profile.Guild.Length, num);
            num++;
            WriteString(_profile.Guild, num);
            num += _profile.Guild.Length;
            WriteUShort((ushort)_profile.Equip.Count(), num);
            num += 2;
            foreach (EquippedS item in _profile.Equip)
            {
                WriteByte(item.Slot, num);
                num++;
                WriteByte(item.Color, num);
                num++;
                WriteByte((byte)item.Name.Length, num);
                num++;
                WriteString(item.Name, num);
                num += item.Name.Length;
                WriteUInt(item.Dura, num);
                num += 4;
            }
            WriteUShort((ushort)_profile.Legend.Count(), num);
            num += 2;
            foreach (LegendMarkS item2 in _profile.Legend)
            {
                WriteByte(item2.Icon, num);
                num++;
                WriteByte(item2.Color, num);
                num++;
                WriteByte((byte)item2.ID.Length, num);
                num++;
                WriteString(item2.ID, num);
                num += item2.ID.Length;
                WriteByte((byte)item2.Text.Length, num);
                num++;
                WriteString(item2.Text, num);
                num += item2.Text.Length;
            }
        }
    }

    public DisplayProfilePacket(DisplayProfileS profile, ushort length)
        : base((ushort)(4 + length), 28)
    {
        Profile = profile;
    }

    public DisplayProfilePacket(byte[] packet)
        : base(packet)
    {
    }
}
