namespace Engine;

public class MPFFrame
{
    private int left;

    private int top;

    private int width;

    private int height;

    private byte[] rawData;

    private int xOffset;

    private int yOffset;

    public bool IsValid
    {
        get
        {
            if (rawData != null && rawData.Length >= 1 && width >= 1 && height >= 1)
            {
                return width * height == rawData.Length;
            }
            return false;
        }
    }

    public int OffsetY => yOffset;

    public int OffsetX => xOffset;

    public byte[] RawData => rawData;

    public int Height => height;

    public int Width => width;

    public int Top
    {
        get
        {
            return top;
        }
        set
        {
            top = value;
        }
    }

    public int Left
    {
        get
        {
            return left;
        }
        set
        {
            left = value;
        }
    }

    public MPFFrame(int left, int top, int width, int height, int xOffset, int yOffset, byte[] rawData)
    {
        this.left = left;
        this.top = top;
        this.width = width;
        this.height = height;
        this.xOffset = xOffset;
        this.yOffset = yOffset;
        this.rawData = rawData;
    }

    public override string ToString()
    {
        return "{X = " + left + ", Y = " + top + ", Width = " + width + ", Height = " + height + ", Offset = (" + xOffset + ", " + yOffset + ")}";
    }
}
