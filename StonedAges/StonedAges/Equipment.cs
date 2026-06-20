using Engine;

namespace StonedAges;

public class Equipment
{
    private TextureManager _textureManager;

    public Sprite _image = new Sprite();

    public string _name;

    public int _color;

    public byte _slot;

    public string _source;

    public int _type;

    public int _frame;

    public int _amount;

    public int _maxAmount = 1;

    public int _durability;

    public int _maxDurability;

    public string _displayName = "";

    public string _enchantment = "";

    public int _value;

    public string _tab = "";

    public string _description = "";

    public int _dyeable;

    public int _bodyImg;

    public int _bodyImgColor;

    public bool _bound;

    public string _level = "";

    public string _gender = "";

    public string _weaponDmg = "";

    public string _atk = "";

    public string _def = "";

    public int _hp;

    public int _mp;

    public short _str;

    public short _int;

    public short _wis;

    public short _con;

    public short _dex;

    public sbyte _mr;

    public sbyte _ac;

    public sbyte _dmg;

    public sbyte _hit;

    public sbyte _reg;

    public Equipment(TextureManager textureManager, string name, byte slot, byte color, string source, int type, int frame)
    {
        if (source == "")
        {
            source = "old";
        }
        _textureManager = textureManager;
        _source = source;
        _type = type;
        _frame = frame - 1;
        _name = name;
        _displayName = name;
        _slot = slot;
        _color = color;
        RefreshImage();
        SetOffset();
    }

    public void RefreshImage()
    {
        if (_source == "new")
        {
            _image.Texture = _textureManager.Get("item" + _type.ToString("000") + "_F" + _frame + "_new_C" + _color, ".epf", _source);
        }
        else if (_source == "myda")
        {
            _image.Texture = _textureManager.Get("item" + _type.ToString("000") + "_F" + _frame + "_myda_C" + _color, ".epf", _source);
        }
        else if (_source == "custom")
        {
            _image.Texture = _textureManager.Get("item" + _type.ToString("000") + "_F" + (_frame + 1) + "_custom_C" + _color, ".epf", _source);
        }
        else
        {
            _image.Texture = _textureManager.Get("item" + _type.ToString("000") + "_F" + _frame + "_C" + _color, ".epf", _source);
        }
    }

    public void SetOffset()
    {
        if (_slot == 1)
        {
            _image._windowOffset = new Vector(105.0, 114.0, 0.0);
        }
        else if (_slot == 2)
        {
            _image._windowOffset = new Vector(170.0, 126.0, 0.0);
        }
        else if (_slot == 3)
        {
            _image._windowOffset = new Vector(194.0, 153.0, 0.0);
        }
        else if (_slot == 4)
        {
            _image._windowOffset = new Vector(78.0, 196.0, 0.0);
        }
        else if (_slot == 5)
        {
            _image._windowOffset = new Vector(106.0, 209.0, 0.0);
        }
        else if (_slot == 6)
        {
            _image._windowOffset = new Vector(136.0, 194.0, 0.0);
        }
        else if (_slot == 7)
        {
            _image._windowOffset = new Vector(106.0, 235.0, 0.0);
        }
        else if (_slot == 8)
        {
            _image._windowOffset = new Vector(149.0, 256.0, 0.0);
        }
        else if (_slot == 9)
        {
            _image._windowOffset = new Vector(60.0, 153.0, 0.0);
        }
        else if (_slot == 10)
        {
            _image._windowOffset = new Vector(137.0, 92.0, 0.0);
        }
        else if (_slot == 11)
        {
            _image._windowOffset = new Vector(127.0, 153.0, 0.0);
        }
        else if (_slot == 12)
        {
            _image._windowOffset = new Vector(193.0, 196.0, 0.0);
        }
        else if (_slot == 13)
        {
            _image._windowOffset = new Vector(168.0, 217.0, 0.0);
        }
        else if (_slot == 14)
        {
            _image._windowOffset = new Vector(202.0, 92.0, 0.0);
        }
        if (_slot != 3 && _slot != 9 && _slot != 11)
        {
            _image.SetScale(0.5, 0.5);
        }
    }

    public bool CollidesWith(Point point)
    {
        if ((double)point.X >= _image.GetPosition().X && (double)point.X <= _image.GetPosition().X + _image.GetWidth() && (double)point.Y >= _image.GetPosition().Y && (double)point.Y <= _image.GetPosition().Y + _image.GetHeight())
        {
            return true;
        }
        return false;
    }
}
