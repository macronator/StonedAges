namespace Engine;

public class ScrollingBackground
{
    private Sprite _background = new Sprite();

    private Point _topLeft = new Point(0f, 0f);

    private Point _bottomRight = new Point(1f, 1f);

    public float Speed { get; set; }

    public Vector Direction { get; set; }

    public void SetScale(double x, double y)
    {
        _background.SetScale(x, y);
    }

    public ScrollingBackground(Texture background)
    {
        _background.Texture = background;
        Speed = 0.15f;
        Direction = new Vector(1.0, 0.0, 0.0);
    }

    public void Update(float elapsedTime)
    {
        _background.SetUVs(_topLeft, _bottomRight);
        _topLeft.X += (float)((double)Speed * Direction.X * (double)elapsedTime);
        _bottomRight.X += (float)((double)Speed * Direction.X * (double)elapsedTime);
        _topLeft.Y += (float)((double)Speed * Direction.Y * (double)elapsedTime);
        _bottomRight.Y += (float)((double)Speed * Direction.Y * (double)elapsedTime);
    }

    public void Render(Renderer renderer)
    {
        renderer.DrawSprite(_background, 0);
    }
}
