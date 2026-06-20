using System.Collections.Generic;

namespace StonedAges;

public struct PlayerDataS
{
    public string name;

    public string password;

    public byte gender;

    public byte hairstyle;

    public byte haircolor;

    public byte direction;

    public int mapnum;

    public byte x;

    public byte y;

    public byte ac;

    public ushort STR;

    public ushort INT;

    public ushort WIS;

    public ushort CON;

    public ushort DEX;

    public byte availstats;

    public int basehp;

    public int basemp;

    public int curhp;

    public int curmp;

    public uint exp;

    public uint next;

    public byte lev;

    public bool master;

    public string guild;

    public byte national;

    public string rank;

    public string title;

    public List<QuestS> quest;

    public List<LegendS> legend;

    public List<InventoryS> inv;

    public List<EquipS> equip;

    public List<SkillS> skills;

    public List<SpellS> spells;

    public List<ActionS> actions;
}
