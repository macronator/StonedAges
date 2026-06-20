namespace Engine;

public class DATFileEntry
{
    private long endAddress;

    private string name;

    private long startAddress;

    public long EndAddress => endAddress;

    public long FileSize => endAddress - startAddress;

    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }

    public long StartAddress => startAddress;

    public DATFileEntry(string name, long startAddress, long endAddress)
    {
        this.name = name;
        this.startAddress = startAddress;
        this.endAddress = endAddress;
    }

    public override string ToString()
    {
        return "{Name = " + name + ", Size = " + FileSize.ToString("###,###,###,###,###0") + "}";
    }
}
