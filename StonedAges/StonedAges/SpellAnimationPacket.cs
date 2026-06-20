namespace StonedAges;

public class SpellAnimationPacket : PacketStructure
{
    private SpellAniS _spellAni;

    public SpellAniS SpellAni
    {
        get
        {
            SpellAniS result = default(SpellAniS);
            ushort num = 4;
            result.ToID = ReadUInt(num);
            num += 4;
            result.FromID = ReadUInt(num);
            num += 4;
            result.ToAni = ReadUShort(num);
            num += 2;
            result.FromAni = ReadUShort(num);
            num += 2;
            result.Delay = ReadUShort(num);
            num += 2;
            result.X = ReadByte(num);
            num++;
            result.Y = ReadByte(num);
            num++;
            return result;
        }
        set
        {
            _spellAni = value;
            ushort num = 4;
            WriteUInt(_spellAni.ToID, num);
            num += 4;
            WriteUInt(_spellAni.FromID, num);
            num += 4;
            WriteUShort(_spellAni.ToAni, num);
            num += 2;
            WriteUShort(_spellAni.FromAni, num);
            num += 2;
            WriteUShort(_spellAni.Delay, num);
            num += 2;
            WriteByte(_spellAni.X, num);
            num++;
            WriteByte(_spellAni.Y, num);
            num++;
        }
    }

    public SpellAnimationPacket(SpellAniS spellAni)
        : base(20, 35)
    {
        SpellAni = spellAni;
    }

    public SpellAnimationPacket(byte[] bytes)
        : base(bytes)
    {
    }
}
