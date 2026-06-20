using Engine;

namespace StonedAges;

public class InventoryItem
{
    public bool _canAtkEles;

    public bool _canDefEles;

    public bool _canEnchant;

    public bool _canSmith;

    public string _enchantment = "";

    public string _displayName = "";

    public bool _clicked;

    private TextureManager _textureManager;

    public bool _bound;

    public int _slot;

    public string _name;

    public int _amount;

    public int _maxAmount = 1;

    public Sprite _sprite = new Sprite();

    public int _maxDurability;

    public int _durability;

    public int _value;

    public string _source = "";

    public string _tab = "";

    public string _description = "";

    public int _dyeable;

    public int _bodyImg;

    public int _bodyImgColor;

    public int _frame;

    public int _type;

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

    public InventoryItem(TextureManager textureManager, string name, Slot slot, string source, int type, int frame, int bodyImgColor)
    {
        if (source == "")
        {
            source = "old";
        }
        _source = source;
        _textureManager = textureManager;
        _name = name;
        _slot = slot.Number;
        _frame = frame - 1;
        _type = type;
        _bodyImgColor = bodyImgColor;
        RefreshImage();
        _sprite.SetPosition(slot._position);
    }

    public void RefreshImage()
    {
        if (_source == "new")
        {
            _sprite.Texture = _textureManager.Get("item" + _type.ToString("000") + "_F" + _frame + "_new_C" + _bodyImgColor, ".epf", _source);
        }
        else if (_source == "myda")
        {
            _sprite.Texture = _textureManager.Get("item" + _type.ToString("000") + "_F" + _frame + "_myda_C" + _bodyImgColor, ".epf", _source);
        }
        else if (_source == "custom")
        {
            _sprite.Texture = _textureManager.Get("item" + _type.ToString("000") + "_F" + (_frame + 1) + "_custom_C" + _bodyImgColor, ".epf", _source);
        }
        else
        {
            _sprite.Texture = _textureManager.Get("item" + _type.ToString("000") + "_F" + _frame + "_C" + _bodyImgColor, ".epf", _source);
        }
    }

    public void Update(double elapsedTime)
    {
    }

    public void Render(Renderer renderer)
    {
        renderer.DrawSprite(_sprite, 0);
    }

    public bool CollidesWith(Point point)
    {
        if ((double)point.X >= _sprite.GetPosition().X && (double)point.X <= _sprite.GetPosition().X + _sprite.GetWidth() && (double)point.Y >= _sprite.GetPosition().Y && (double)point.Y <= _sprite.GetPosition().Y + _sprite.GetHeight())
        {
            return true;
        }
        return false;
    }
}
