using System.Collections.Generic;
using System.Drawing;
using Engine;

namespace StonedAges;

public class Town
{
    public static Dictionary<string, Town> _List = new Dictionary<string, Town>();

    private string _name = "";

    public int _map;

    public Rectangle _rect;

    private bool _hover;

    private Text _text;

    private Engine.Font _font;

    public Town(string name, int map, Rectangle rect, Engine.Font font)
    {
        _name = name;
        _map = map;
        _rect = rect;
        _font = font;
        _text = new Text(_name, _font, 120);
        _text.SetPosition(_rect.X + 20, _rect.Y);
        _List.Add(_name, this);
    }

    public bool CollidesWith(Engine.Point point)
    {
        if (point.X >= (float)_rect.X && point.X <= (float)(_rect.X + _rect.Width) && point.Y >= (float)_rect.Y && point.Y <= (float)(_rect.Y + _rect.Height))
        {
            _text.SetColor(Text.Colors(System.Drawing.Color.Orange));
            _hover = true;
            return true;
        }
        _text.SetColor(Engine.Color.White);
        _hover = false;
        return false;
    }

    public void Update(double elapsedTime)
    {
    }

    public void Render(Renderer renderer)
    {
        if (_hover)
        {
            renderer.DrawBorder(new Rectangle(_rect.X + 2, _rect.Y + 2, _rect.Width - 4, _rect.Height - 4), Engine.Color.LightBlue);
        }
        renderer.DrawBorder(new Rectangle(_rect.X + 1, _rect.Y + 1, _rect.Width - 2, _rect.Height - 2), Engine.Color.LightBlue);
        if (!_hover)
        {
            renderer.DrawBorder(new Rectangle(_rect.X, _rect.Y, _rect.Width, _rect.Height), Engine.Color.LightBlue);
        }
        renderer.DrawText(_text);
    }
}
