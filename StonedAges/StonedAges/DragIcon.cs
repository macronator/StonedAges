using Engine;

namespace StonedAges;

public class DragIcon
{
    private Input _input;

    private Sprite _sprite = new Sprite();

    public bool _clicked;

    private Point _offset = new Point(0f, 0f);

    public int _slot;

    public string _name;

    public int _amount;

    public int _durability;

    public int _bodyImgColor;

    public string _enchantment;

    public Location _loc;

    public DragIcon(Input input, Sprite sprite, string name = "", int slot = 0, int amount = 1, int durability = 0, int bodyImgColor = 0, string enchantment = "")
    {
        _input = input;
        _sprite.Texture = sprite.Texture;
        _sprite.SetColor(new Color(1f, 1f, 1f, 0.5f));
        Vector position = sprite.GetPosition();
        _sprite.SetPosition(position);
        Point position2 = _input.Mouse.Position;
        _offset.X = (float)position.X - position2.X;
        _offset.Y = (float)position.Y - position2.Y;
        _name = name;
        _slot = slot;
        _amount = amount;
        _durability = durability;
        _bodyImgColor = bodyImgColor;
        _enchantment = enchantment;
    }

    public void Update(double elapsedTime)
    {
        if (_clicked)
        {
            Point position = _input.Mouse.Position;
            _sprite.SetPosition(_offset.X + position.X, _offset.Y + position.Y);
        }
    }

    public void Render(Renderer renderer)
    {
        renderer.DrawSprite(_sprite, 0);
    }
}
