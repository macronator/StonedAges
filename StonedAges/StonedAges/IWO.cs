using System.Drawing;
using Engine;
using Tao.OpenGl;

namespace StonedAges;

public class IWO
{
    public bool _clicked;

    public ushort _id;

    public string _type;

    public int _direction;

    public Location _location;

    public int _map;

    public string _text;

    public Rectangle _rect = default(Rectangle);

    public bool _enabled;

    public Tile _tile;

    public IWO(ushort id, string type, int direction, Location loc, int map, string text, Tile tile)
    {
        _id = id;
        _type = type;
        _direction = direction;
        _location = loc;
        _map = map;
        _text = text;
        _tile = tile;
        _tile._iwo = this;
        move();
        _tile._map._iwos.Add(this);
    }

    public void move()
    {
        if (_type == "Board" || _type == "Sign")
        {
            _rect = new Rectangle((int)_tile._position.X + 10, (int)_tile._position.Y - 33, 37, 37);
        }
        else if (_type == "DoorE" || _type == "DoorA" || _type == "Door")
        {
            if (_direction == 0)
            {
                _rect = new Rectangle((int)_tile._position.X - 8, (int)_tile._position.Y - 37, 37, 52);
            }
            else
            {
                _rect = new Rectangle((int)_tile._position.X + 27, (int)_tile._position.Y - 37, 37, 52);
            }
        }
        else if (_type == "DoorD")
        {
            if (_direction == 0)
            {
                _rect = new Rectangle((int)_tile._position.X - 3, (int)_tile._position.Y - 32, 32, 52);
            }
            else
            {
                _rect = new Rectangle((int)_tile._position.X + 27, (int)_tile._position.Y - 32, 32, 52);
            }
        }
        else if (_type == "DoorC")
        {
            if (_direction == 0)
            {
                _rect = new Rectangle((int)_tile._position.X - 8, (int)_tile._position.Y - 37, 37, 52);
            }
            else
            {
                _rect = new Rectangle((int)_tile._position.X + 22, (int)_tile._position.Y - 32, 37, 52);
            }
        }
        else if (_type == "DoorB")
        {
            if (_direction == 0)
            {
                _rect = new Rectangle((int)_tile._position.X + 7, (int)_tile._position.Y - 33, 37, 57);
            }
            else
            {
                _rect = new Rectangle((int)_tile._position.X + 13, (int)_tile._position.Y - 33, 37, 57);
            }
        }
        else if (_type == "JailDoor")
        {
            if (_direction == 0)
            {
                _rect = new Rectangle((int)_tile._position.X + 5, (int)_tile._position.Y - 32, 37, 52);
            }
            else
            {
                _rect = new Rectangle((int)_tile._position.X + 27, (int)_tile._position.Y - 37, 37, 52);
            }
        }
        else if (_type == "Chest")
        {
            if (_direction == 0)
            {
                _rect = new Rectangle((int)_tile._position.X + 10, (int)_tile._position.Y - 18, 45, 37);
            }
            else
            {
                _rect = new Rectangle((int)_tile._position.X - 10, (int)_tile._position.Y - 15, 45, 37);
            }
        }
    }

    public bool CollidesWith(Engine.Point point)
    {
        if (point.X >= (float)_rect.X && point.X <= (float)(_rect.X + _rect.Width) && point.Y >= (float)_rect.Y && point.Y <= (float)(_rect.Y + _rect.Height))
        {
            return true;
        }
        return false;
    }

    public void Render_Debug()
    {
        Gl.glDisable(3553);
        Gl.glBegin(2);
        Gl.glColor3f(1f, 0f, 0f);
        Gl.glVertex2i(_rect.Left, _rect.Top);
        Gl.glVertex2i(_rect.Right, _rect.Top);
        Gl.glVertex2i(_rect.Right, _rect.Bottom);
        Gl.glVertex2i(_rect.Left, _rect.Bottom);
        Gl.glEnd();
        Gl.glEnable(3553);
    }
}
