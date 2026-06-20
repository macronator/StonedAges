namespace StonedAges;

public class MapInfoPacket : PacketStructure
{
    private MapInfoS _map;

    public MapInfoS Map
    {
        get
        {
            MapInfoS result = default(MapInfoS);
            int num = 4;
            result.Number = ReadUShort(num);
            num += 2;
            result.Width = ReadByte(num);
            num++;
            result.Height = ReadByte(num);
            num++;
            result.Bitmask = ReadByte(num);
            num++;
            result.CheckSum = ReadUShort(num);
            num += 2;
            int count = ReadByte(num);
            num++;
            result.Name = ReadString(num, count);
            return result;
        }
        set
        {
            _map = value;
            int num = 4;
            WriteUShort(_map.Number, num);
            num += 2;
            WriteByte(_map.Width, num);
            num++;
            WriteByte(_map.Height, num);
            num++;
            WriteByte(_map.Bitmask, num);
            num++;
            WriteUShort(_map.CheckSum, num);
            num += 2;
            WriteByte((byte)_map.Name.Length, num);
            num++;
            WriteString(_map.Name, num);
        }
    }

    public MapInfoPacket(MapInfoS map, ushort length)
        : base((ushort)(4 + length), 15)
    {
        Map = map;
    }

    public MapInfoPacket(byte[] packet)
        : base(packet)
    {
    }
}
