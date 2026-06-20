namespace Engine;

public struct Point
{
    public float X { get; set; }

    public float Y { get; set; }

    public Point(float x, float y)
    {
        this = default(Point);
        X = x;
        Y = y;
    }
}
