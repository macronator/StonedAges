namespace Engine;

public class PaletteTableEntry
{
    private int min;

    private int max;

    private int palette;

    public int Palette
    {
        get
        {
            return palette;
        }
        set
        {
            palette = value;
        }
    }

    public int Max
    {
        get
        {
            return max;
        }
        set
        {
            max = value;
        }
    }

    public int Min
    {
        get
        {
            return min;
        }
        set
        {
            min = value;
        }
    }

    public PaletteTableEntry(int min, int max, int palette)
    {
        this.min = min;
        this.max = max;
        this.palette = palette;
    }

    public override string ToString()
    {
        return "{Min = " + min + ", Max = " + max + ", Palette = " + palette + "}";
    }
}
