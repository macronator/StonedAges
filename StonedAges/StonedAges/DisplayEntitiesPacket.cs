using System.Collections.Generic;
using System.Linq;

namespace StonedAges;

public class DisplayEntitiesPacket : PacketStructure
{
    private List<DisplayEntityS> _entities;

    public List<DisplayEntityS> Entities
    {
        get
        {
            List<DisplayEntityS> list = new List<DisplayEntityS>();
            int num = 4;
            ushort num2 = ReadUShort(num);
            num += 2;
            for (ushort num3 = 0; num3 < num2; num3++)
            {
                DisplayEntityS item = default(DisplayEntityS);
                item.X = ReadByte(num);
                num++;
                item.Y = ReadByte(num);
                num++;
                item.ID = ReadUInt(num);
                num += 4;
                item.Image = ReadUShort(num);
                num += 2;
                item.Source = ReadByte(num);
                num++;
                item.EntityType = (EntityType)ReadByte(num);
                num++;
                if (item.EntityType == EntityType.Item)
                {
                    item.Color = ReadByte(num);
                    num++;
                }
                else
                {
                    item.Direction = (D)ReadByte(num);
                    num++;
                    byte b = ReadByte(num);
                    num++;
                    item.Name = ReadString(num, b);
                    num += b;
                }
                list.Add(item);
            }
            return list;
        }
        set
        {
            _entities = value;
            int num = 4;
            ushort num2 = (ushort)_entities.Count();
            WriteUShort(num2, num);
            num += 2;
            for (ushort num3 = 0; num3 < num2; num3++)
            {
                WriteByte(_entities[num3].X, num);
                num++;
                WriteByte(_entities[num3].Y, num);
                num++;
                WriteUInt(_entities[num3].ID, num);
                num += 4;
                WriteUShort(_entities[num3].Image, num);
                num += 2;
                WriteByte(_entities[num3].Source, num);
                num++;
                WriteByte((byte)_entities[num3].EntityType, num);
                num++;
                if (_entities[num3].EntityType == EntityType.Item)
                {
                    WriteByte(_entities[num3].Color, num);
                    num++;
                }
                else
                {
                    WriteByte((byte)_entities[num3].Direction, num);
                    num++;
                    byte b = (byte)_entities[num3].Name.Length;
                    WriteByte(b, num);
                    num++;
                    WriteString(_entities[num3].Name, num);
                    num += b;
                }
            }
        }
    }

    public DisplayEntitiesPacket(List<DisplayEntityS> entities, ushort length)
        : base((ushort)(4 + length), 34)
    {
        Entities = entities;
    }

    public DisplayEntitiesPacket(byte[] packet)
        : base(packet)
    {
    }
}
