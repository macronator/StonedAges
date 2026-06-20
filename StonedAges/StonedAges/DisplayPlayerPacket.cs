namespace StonedAges;

public class DisplayPlayerPacket : PacketStructure
{
    private DisplayPlayerS _player;

    public DisplayPlayerS Player
    {
        get
        {
            DisplayPlayerS result = default(DisplayPlayerS);
            int num = 4;
            result.X = ReadByte(num);
            num++;
            result.Y = ReadByte(num);
            num++;
            result.Direction = ReadByte(num);
            num++;
            result.ID = ReadUInt(num);
            num += 4;
            result.Form = ReadUShort(num);
            num += 2;
            result.Head = ReadUShort(num);
            num += 2;
            result.HeadColor = ReadByte(num);
            num++;
            result.HeadSource = ReadByte(num);
            num++;
            result.Body = ReadUShort(num);
            num += 2;
            result.Arms = ReadUShort(num);
            num += 2;
            result.Boots = ReadByte(num);
            num++;
            result.BootColor = ReadByte(num);
            num++;
            result.BootSource = ReadByte(num);
            num++;
            result.Armor = ReadUShort(num);
            num += 2;
            result.ArmorColor = ReadByte(num);
            num++;
            result.ArmorSource = ReadByte(num);
            num++;
            result.Shield = ReadByte(num);
            num++;
            result.ShieldSource = ReadByte(num);
            num++;
            result.Weapon = ReadUShort(num);
            num += 2;
            result.WeaponSource = ReadByte(num);
            num++;
            result.Acc = ReadUShort(num);
            num += 2;
            result.AccColor = ReadByte(num);
            num++;
            result.Hidden = ReadBool(num);
            num++;
            result.NameTagType = ReadByte(num);
            num++;
            result.Gender = ReadByte(num);
            num++;
            byte count = ReadByte(num);
            num++;
            result.Name = ReadString(num, count);
            return result;
        }
        set
        {
            _player = value;
            int num = 4;
            WriteByte(_player.X, num);
            num++;
            WriteByte(_player.Y, num);
            num++;
            WriteByte(_player.Direction, num);
            num++;
            WriteUInt(_player.ID, num);
            num += 4;
            WriteUShort(_player.Form, num);
            num += 2;
            WriteUShort(_player.Head, num);
            num += 2;
            WriteByte(_player.HeadColor, num);
            num++;
            WriteByte(_player.HeadSource, num);
            num++;
            WriteUShort(_player.Body, num);
            num += 2;
            WriteUShort(_player.Arms, num);
            num += 2;
            WriteByte(_player.Boots, num);
            num++;
            WriteByte(_player.BootColor, num);
            num++;
            WriteByte(_player.BootSource, num);
            num++;
            WriteUShort(_player.Armor, num);
            num += 2;
            WriteByte(_player.ArmorColor, num);
            num++;
            WriteByte(_player.ArmorSource, num);
            num++;
            WriteByte(_player.Shield, num);
            num++;
            WriteByte(_player.ShieldSource, num);
            num++;
            WriteUShort(_player.Weapon, num);
            num += 2;
            WriteByte(_player.WeaponSource, num);
            num++;
            WriteUShort(_player.Acc, num);
            num += 2;
            WriteByte(_player.AccColor, num);
            num++;
            WriteBool(_player.Hidden, num);
            num++;
            WriteByte(_player.NameTagType, num);
            num++;
            WriteByte(_player.Gender, num);
            num++;
            WriteByte((byte)_player.Name.Length, num);
            num++;
            WriteString(_player.Name, num);
        }
    }

    public DisplayPlayerPacket(DisplayPlayerS player, ushort length)
        : base((ushort)(4 + length), 33)
    {
        Player = player;
    }

    public DisplayPlayerPacket(byte[] packet)
        : base(packet)
    {
    }
}
