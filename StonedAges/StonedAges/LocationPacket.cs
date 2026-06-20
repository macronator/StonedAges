namespace StonedAges;

public class LocationPacket : PacketStructure
{
    private uint _id;

    private Location _location;

    private byte _direction;

    public uint ID
    {
        get
        {
            return ReadUInt(4);
        }
        set
        {
            _id = value;
            WriteUInt(_id, 4);
        }
    }

    public Location Location
    {
        get
        {
            int num = 8;
            byte x = ReadByte(num);
            num++;
            byte y = ReadByte(num);
            return new Location(x, y);
        }
        set
        {
            _location = value;
            int num = 8;
            WriteByte((byte)_location.X, num);
            num++;
            WriteByte((byte)_location.Y, num);
        }
    }

    public byte Direction
    {
        get
        {
            return ReadByte(10);
        }
        set
        {
            _direction = value;
            WriteByte(_direction, 10);
        }
    }

    public LocationPacket(uint id, Location location, byte direction)
        : base(11, 4)
    {
        ID = id;
        Location = location;
        Direction = direction;
    }

    public LocationPacket(byte[] packet)
        : base(packet)
    {
    }
}
