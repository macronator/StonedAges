namespace StonedAges;

public class Portal
{
    public int _fromMap;

    public int _toMap;

    public int _fromX;

    public int _fromY;

    public int _toX;

    public int _toY;

    public Portal(int fromMap, int toMap, int fromX, int fromY, int toX, int toY)
    {
        _fromMap = fromMap;
        _toMap = toMap;
        _fromX = fromX;
        _fromY = fromY;
        _toX = toX;
        _toY = toY;
    }
}
