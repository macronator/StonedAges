using System;
using System.Drawing;
using Engine;
using Newtonsoft.Json.Linq;

namespace StonedAges;

public class SpellBar
{
    private TextureManager _textureManager;

    public int _slot;

    public Sprite _sprite = new Sprite();

    public Sprite _timer = new Sprite();

    public Sprite _countDownBackground = new Sprite();

    public Text _countDown;

    private int _offset;

    public string _name;

    public Vector _position = default(Vector);

    public double _seconds;

    public string _startMsg;

    public string _reMsg;

    public string _endMsg;

    public Entity _entity;

    public double _delay;

    public bool _hover;

    private GameState _gs;

    public double _repeatTimer = 1000.0;

    public SpellBar(GameState gs, TextureManager textureManager, Engine.Font font, string name, int slot, string type, int frame, double seconds, string startMsg, string reMsg, string endMsg, Entity entity)
    {
        _gs = gs;
        _textureManager = textureManager;
        _name = name;
        _slot = slot;
        _seconds = seconds;
        _delay = seconds;
        _startMsg = startMsg;
        _reMsg = reMsg;
        _endMsg = endMsg;
        _entity = entity;
        _offset = (_slot - 1) * 23;
        _position.X = 621.0;
        _position.Y = 4 + _offset;
        _sprite.Texture = _textureManager.Get(type + "_F" + frame + "_C0");
        _sprite.SetPosition(_position);
        _sprite.SetScale(0.5, 0.5);
        _timer.SetPosition(_position.X + 1.0, _position.Y + 16.0);
        _timer.SetHeight(2f);
        _timer.SetWidth(14f);
        _timer.SetColor(Engine.Color.White);
        _countDownBackground.SetPosition(_position.X - 25.0, _position.Y + 1.0);
        _countDownBackground.SetWidth(21f);
        _countDownBackground.SetHeight(17f);
        _countDownBackground.SetColor(new Engine.Color(0f, 0f, 0f, 0.5f));
        string text = "";
        if (_seconds == 0.0)
        {
            text = "~ ";
        }
        _countDown = new Text(text, font, 18);
        _countDown.SetColor(Text.Colors(System.Drawing.Color.Red));
        _countDown.Align("right");
        _countDown.SetPosition(_position.X - 22.0, _position.Y + 5.0);
        Add();
    }

    public void Add()
    {
        foreach (JToken item in GameState._spellsDB["spells"].Children())
        {
            string text = item.Value<string>("name");
            if (text.Equals(_name, StringComparison.CurrentCultureIgnoreCase))
            {
                _startMsg = item.Value<string>("startmsg");
                if (string.IsNullOrEmpty(_startMsg))
                {
                    _startMsg = "";
                }
                _endMsg = item.Value<string>("endmsg");
                if (string.IsNullOrEmpty(_endMsg))
                {
                    _endMsg = "";
                }
                _reMsg = item.Value<string>("remsg");
                if (string.IsNullOrEmpty(_reMsg))
                {
                    _reMsg = "";
                }
            }
        }
        if (_name.ToLower() == "armor")
        {
            _entity._ac += 10;
        }
        else if (_name.ToLower() == "curse1")
        {
            _entity._ac -= 20;
        }
        else if (_name.ToLower() == "curse2")
        {
            _entity._ac -= 35;
        }
        else if (_name.ToLower() == "curse3")
        {
            _entity._ac -= 50;
        }
        else if (_name.ToLower() == "curse4")
        {
            _entity._ac -= 65;
        }
        else if (_name.ToLower() == "curse5")
        {
            _entity._ac -= 80;
        }
        else if (_name.ToLower() == "blessed1")
        {
            _entity._hit += 10;
        }
        else if (_name.ToLower() == "blessed2")
        {
            _entity._hit += 20;
        }
        else if (_name.ToLower() == "powerful")
        {
            _entity._dmg += 5;
        }
        else if (_name.ToLower() == "invisible")
        {
            _entity.Hidden = true;
            if (_entity._id == _gs.ClientID)
            {
                _gs.SendDisplayPlayer();
            }
        }
        else if (_name.ToLower() == "hpgain")
        {
            _entity._hpgain += 20;
        }
        else if (_name.ToLower() == "mpgain")
        {
            _entity._mpgain += 20;
        }
        if (_startMsg != "" && _entity._id == _gs.ClientID)
        {
            _gs.SystemMsg(_startMsg, 3);
        }
    }

    public void Remove(bool replace = false)
    {
        if (_name.ToLower() == "armor")
        {
            _entity._ac -= 10;
        }
        else if (_name.ToLower() == "curse1")
        {
            _entity._ac += 20;
        }
        else if (_name.ToLower() == "curse2")
        {
            _entity._ac += 35;
        }
        else if (_name.ToLower() == "curse3")
        {
            _entity._ac += 50;
        }
        else if (_name.ToLower() == "curse4")
        {
            _entity._ac += 65;
        }
        else if (_name.ToLower() == "curse5")
        {
            _entity._ac += 80;
        }
        else if (_name.ToLower() == "blessed1")
        {
            _entity._hit -= 10;
        }
        else if (_name.ToLower() == "blessed2")
        {
            _entity._hit -= 20;
        }
        else if (_name.ToLower() == "powerful")
        {
            _entity._dmg -= 5;
        }
        else if (_name.ToLower() == "invisible")
        {
            _entity.Hidden = false;
        }
        else if (_name.ToLower() == "hpgain")
        {
            _entity._hpgain -= 20;
        }
        else if (_name.ToLower() == "mpgain")
        {
            _entity._mpgain -= 20;
        }
        else if (_name.ToLower() == "death")
        {
            if (_entity._curHP == 0)
            {
                _entity._curHP = 1;
            }
            _entity._dead = false;
        }
        if (!replace && _endMsg != "" && _entity._id == _gs.ClientID)
        {
            _gs.SystemMsg(_endMsg, 3);
        }
        _entity._spellBar.Remove(_name);
    }

    public bool Update(double elapsedTime)
    {
        _repeatTimer -= elapsedTime * 1000.0;
        if (_repeatTimer <= 0.0)
        {
            _repeatTimer = 1000.0;
            if (_entity.hassleep)
            {
                _gs.SpellAnimation(_entity, 32, 120);
            }
            else if (_entity.hasslumber)
            {
                _gs.SpellAnimation(_entity, 40, 120);
            }
            else if (_entity.hasparalyze)
            {
                _gs.SpellAnimation(_entity, 41, 120);
            }
            else if (_entity.haspoison)
            {
                _gs.SpellAnimation(_entity, 25, 120);
            }
            if (_reMsg != "" && _entity._id == _gs.ClientID)
            {
                _gs.SystemMsg(_reMsg, 3);
            }
        }
        if (_seconds != 0.0)
        {
            _delay -= elapsedTime;
            _countDown.ChangeText(((int)_delay + 1).ToString());
            if (_delay <= 0.0)
            {
                Remove();
                return false;
            }
            if (_delay <= 20.0)
            {
                _timer.SetColor(Engine.Color.LightBlue);
                _timer.SetWidth(4f);
            }
            else if (_delay <= 40.0)
            {
                _timer.SetColor(Text.Colors(System.Drawing.Color.DarkGreen));
                _timer.SetWidth(6f);
            }
            else if (_delay <= 60.0)
            {
                _timer.SetColor(Text.Colors(System.Drawing.Color.Yellow));
                _timer.SetWidth(8f);
            }
            else if (_delay <= 80.0)
            {
                _timer.SetColor(Text.Colors(System.Drawing.Color.Orange));
                _timer.SetWidth(10f);
            }
            else if (_delay <= 100.0)
            {
                _timer.SetColor(Text.Colors(System.Drawing.Color.Red));
                _timer.SetWidth(12f);
            }
        }
        return true;
    }

    public void Render(Renderer renderer)
    {
        if (_hover)
        {
            renderer.DrawSprite(_countDownBackground, 0);
        }
        renderer.DrawSprite(_sprite, 0);
        renderer.DrawSprite(_timer, 0);
        if (_hover)
        {
            renderer.DrawText(_countDown);
        }
    }

    public bool CollidesWith(Engine.Point point)
    {
        if ((double)point.X >= _position.X && (double)point.X <= _position.X + _sprite.GetWidth() && (double)point.Y >= _position.Y && (double)point.Y <= _position.Y + _sprite.GetHeight())
        {
            return true;
        }
        return false;
    }
}
