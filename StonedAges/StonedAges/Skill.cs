using System;
using Engine;

namespace StonedAges;

public class Skill
{
    private TextureManager _textureManager;

    public byte _levReq = 1;

    public string _statsReq = "3,3,3,3,3";

    public byte _maxLevel = 100;

    public int _manaCost;

    public int _slot;

    public string _name;

    public Sprite _sprite = new Sprite();

    public byte _level;

    public DateTime _lastUse = DateTime.MinValue;

    public DateTime _lastSuccess = DateTime.MinValue;

    public double _cooldown;

    public bool _highlight;

    public string _status;

    public ushort _useCount;

    public ushort _impRate;

    public bool _checkIfHidden;

    public bool _castMsg;

    public bool _checkSpellBar;

    public bool _allowRefresh;

    public byte _bodyAniType;

    public ushort _bodyAniSpeed;

    public byte _sound;

    public int _toani;

    public int _toanispeed;

    public int _fromani;

    public int _fromanispeed;

    public byte _range = 1;

    public byte _projType;

    public int _dmg;

    public string _targettype = "";

    public string _type;

    public int _frame = -1;

    public int _seconds = -1;

    public string _startMsg = "";

    public string _reMsg = "";

    public string _endMsg = "";

    public string _element = "";

    public string _description = "";

    public Skill(TextureManager textureManager, string name, Slot slot, byte level, string type, int frame, double cooldown, byte sound, int fromani, int fromanispeed, int toani, int toanispeed, bool checkIfHidden, bool castMsg, bool checkSpellBar, bool allowRefresh, int seconds, string startMsg, string reMsg, string endMsg, byte bodyAniType, ushort bodyAniSpeed, int baseDmg, string element, string targettype, string status)
    {
        _reMsg = reMsg;
        _status = status;
        _fromani = fromani;
        _fromanispeed = fromanispeed;
        _toani = toani;
        _toanispeed = toanispeed;
        _checkIfHidden = checkIfHidden;
        _castMsg = castMsg;
        _checkSpellBar = checkSpellBar;
        _allowRefresh = allowRefresh;
        _seconds = seconds;
        _startMsg = startMsg;
        _endMsg = endMsg;
        _bodyAniType = bodyAniType;
        _bodyAniSpeed = bodyAniSpeed;
        _dmg = baseDmg;
        _element = element;
        _targettype = targettype;
        _type = type;
        _sound = sound;
        _cooldown = cooldown;
        _level = level;
        _textureManager = textureManager;
        _name = name;
        _slot = slot.Number;
        _frame = frame;
        frame--;
        _sprite.Texture = _textureManager.Get(type + "_F" + frame + "_C0");
        _sprite.SetPosition(slot._position);
    }

    public void Update(double elapsedTime)
    {
        if (_lastSuccess != DateTime.MinValue && DateTime.UtcNow.Subtract(_lastSuccess).TotalMilliseconds >= _cooldown * 1000.0)
        {
            _lastSuccess = DateTime.MinValue;
            _highlight = false;
        }
    }

    public void Render(Renderer renderer)
    {
        if (_highlight)
        {
            renderer.DrawSprite(_sprite, 1);
        }
        else
        {
            renderer.DrawSprite(_sprite, 0);
        }
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
