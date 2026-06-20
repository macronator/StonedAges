using System.Collections.Generic;

namespace StonedAges;

public struct DisplayProfileS
{
    public uint ID;

    public string Name;

    public byte Nation;

    public string Title;

    public byte AllowGroup;

    public List<GroupMemberS> GroupList;

    public string Rank;

    public string Level;

    public string Guild;

    public List<EquippedS> Equip;

    public List<LegendMarkS> Legend;
}
