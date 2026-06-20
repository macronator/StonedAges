using System.Collections.Generic;

namespace StonedAges;

public struct BoardS
{
    public byte Permission;

    public ushort BoardID;

    public string BoardName;

    public List<PostS> PostList;
}
