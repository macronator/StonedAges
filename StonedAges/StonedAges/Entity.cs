using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Engine;
using Tao.OpenGl;

namespace StonedAges;

public class Entity
{
    public const int xconst = 28;

    public const int yconst = 14;

    public const double _cbOrigDelay = 5000.0;

    private const double _origHPBarDelay = 8000.0;

    public bool _assailPush;

    public EntityType _entityType = EntityType.Player;

    private Random _random = new Random();

    public bool _showInfo;

    public double _baseHitChance = 90.0;

    public double _baseCritChance = 5.0;

    public bool _moving;

    public double _oldX;

    public double _oldY;

    public bool _clicked;

    public DateTime speakTime = DateTime.UtcNow;

    public DateTime _tileTime = DateTime.UtcNow;

    public DateTime CreateTime = DateTime.UtcNow;

    public bool _hostile;

    public uint _hostileID;

    public ushort _str = 3;

    public ushort _int = 3;

    public ushort _wis = 3;

    public ushort _con = 3;

    public ushort _dex = 3;

    public int _curHP = 100000;

    public int _baseHP = 100000;

    public int _combinedHPBonus;

    public int _curMP = 100;

    public int _baseMP = 100;

    public int _combinedMPBonus;

    public byte _lev = 1;

    public uint _exp = 1u;

    public uint _tnl = 599u;

    public string _atk = "None";

    public string _def = "None";

    public byte _mr;

    public short _ac;

    public byte _dmg;

    public byte _hit;

    public byte _reg;

    public byte _hpgain;

    public byte _mpgain;

    public byte _passable;

    private static uint entityID = 100000u;

    public uint _id;

    public int _gender = 1;

    public string _name;

    public Location _location;

    public int _mapNum;

    public string _mapName;

    public Sprite _sprite = new Sprite();

    public Sprite _hpBarSprite = new Sprite();

    public Sprite _spellMeterSprite = new Sprite();

    public double _scale = 1.0;

    public string _imgString;

    public Map _map;

    public TextureManager _textureManager;

    public int _displayTag;

    public Text _nameTag;

    public Engine.Font _font;

    public Tile _tile;

    public List<string> _inventory = new List<string>();

    public List<string> _incomingChat = new List<string>();

    public Text _chatText;

    public Sprite _chatBubbleBack = new Sprite();

    public double _cbDelay = 5000.0;

    public Dictionary<string, SpellBar> _spellBar = new Dictionary<string, SpellBar>(StringComparer.CurrentCultureIgnoreCase);

    public PlayerBody _body;

    public MonsterBody _mBody;

    public Rectangle _targetBox = new Rectangle(0, 0, 27, 60);

    public bool _targeted;

    public Dictionary<string, int> _selllist = new Dictionary<string, int>();

    public List<string> _learnskills = new List<string>();

    public List<string> _learnspells = new List<string>();

    public List<string> _learnactions = new List<string>();

    public bool _dead;

    public bool _hidden;

    public bool _displayHP;

    private double _hpBarDelay = 8000.0;

    public bool _displaySpellMeter;

    public Dictionary<string, string> _dialogSpeechTriggers = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

    public GameState gs;

    public byte _unlock = 3;

    private bool _ghost;

    public int _maxHP => _baseHP + _combinedHPBonus;

    public int _maxMP => _baseMP + _combinedMPBonus;

    public int _direction
    {
        get
        {
            if (_body != null)
            {
                return _body._direction;
            }
            if (_mBody != null)
            {
                return _mBody._direction;
            }
            return 0;
        }
        set
        {
            if (_body != null)
            {
                _body._direction = value;
            }
            if (_mBody != null)
            {
                _mBody._direction = value;
            }
        }
    }

    public bool Ghost
    {
        get
        {
            return _ghost;
        }
        set
        {
            _ghost = value;
            if (_body == null)
            {
                return;
            }
            if (_ghost)
            {
                _body._bodyImgs["b"] = 2;
                _body._ghost = true;
                return;
            }
            if (_body._swimming)
            {
                if (_body._gender == 0)
                {
                    _body._bodyImgs["b"] = 5;
                }
                else
                {
                    _body._bodyImgs["b"] = 5;
                }
            }
            else
            {
                _body._bodyImgs["b"] = 1;
            }
            _body._ghost = false;
        }
    }

    public bool Hidden
    {
        get
        {
            return _hidden;
        }
        set
        {
            _hidden = value;
            if (_body != null)
            {
                if (_hidden)
                {
                    _body._hidden = true;
                }
                else
                {
                    _body._hidden = false;
                }
            }
        }
    }

    public bool haspoison => _spellBar.ContainsKey("Poison");

    public bool hasblind => _spellBar.ContainsKey("Blind");

    public bool hasparalyze => _spellBar.ContainsKey("Paralyze");

    public bool hasslumber => _spellBar.ContainsKey("Slumber");

    public bool hassleep => _spellBar.ContainsKey("Sleep");

    public bool hasbeagcradh => _spellBar.ContainsKey("curse1");

    public bool hascradh => _spellBar.ContainsKey("curse2");

    public bool hasmorcradh => _spellBar.ContainsKey("curse3");

    public bool hasardcradh => _spellBar.ContainsKey("curse4");

    public bool hasdiacradh => _spellBar.ContainsKey("curse5");

    public bool hasaite => _spellBar.ContainsKey("Sanctuary");

    public bool hasbeagfas => _spellBar.ContainsKey("Amplify1");

    public bool hasfas => _spellBar.ContainsKey("Amplify2");

    public bool hasmorfas => _spellBar.ContainsKey("Amplify3");

    public bool hasardfas => _spellBar.ContainsKey("Amplify4");

    public bool hasdiafas => _spellBar.ContainsKey("Amplify5");

    public bool hasdion => _spellBar.ContainsKey("Invincible");

    public Entity(GameState Gs, TextureManager textureManager, Engine.Font font, string name, Location loc, Map map, string imgString, Texture img = default(Texture), uint id = 0u)
    {
        gs = Gs;
        CreateTime = DateTime.UtcNow;
        _sprite.SetWidth(57f);
        _chatBubbleBack.SetColor(new Engine.Color(0f, 0f, 0f, 0.5f));
        _chatBubbleBack.SetWidth(126f);
        _font = font;
        _chatText = gs.DrawLabel("", Engine.Color.White, 0.0, 0.0, 114, "left");
        _nameTag = new Text(name, _font, 180);
        _textureManager = textureManager;
        _map = map;
        if (id == 0)
        {
            entityID++;
            _id = entityID;
        }
        else
        {
            _id = id;
        }
        _location = loc;
        _name = name;
        _mapNum = map._number;
        _mapName = map._name;
        _imgString = imgString;
        _sprite.Texture = img;
        _spellMeterSprite.SetWidth(25f);
        _spellMeterSprite.SetHeight(4f);
        _spellMeterSprite.SetColor(Engine.Color.Red);
        _hpBarSprite.SetWidth(25f);
        _hpBarSprite.SetHeight(4f);
        _hpBarSprite.SetColor(Text.Colors(System.Drawing.Color.Green));
        if (_map._tiles.Count > _location.Y * (int)_map._width + _location.X)
        {
            _tile = _map._tiles[_location.Y * (int)_map._width + _location.X];
            _tile._entities.Add(_id, this);
            _tileTime = DateTime.UtcNow;
        }
        _map._entities.Add(_id, this);
    }

    public void Speak(string text, byte type = 0)
    {
        string text2 = "";
        string text3 = ": ";
        if (type == 1)
        {
            text2 = "{=c";
            text3 = "! ";
        }
        _cbDelay = 5000.0;
        _chatText.ChangeText(text2 + _name + text3 + text);
        speakTime = DateTime.UtcNow;
        double x = _sprite.GetPosition().X;
        double y = _sprite.GetPosition().Y;
        if (_mBody != null)
        {
            x = _mBody._position.X;
            y = _mBody._position.Y;
        }
        if (_body != null)
        {
            x = _body._position.X;
            y = _body._position.Y;
        }
        int num = _chatText._lines * 12;
        Vector vector = new Vector(x - 32.0, y - (double)(14 + num), 0.0);
        _chatText.SetPosition(vector.X + 6.0, vector.Y + 3.0, colorize: true);
        _chatBubbleBack.SetPosition(vector.X, vector.Y);
        _chatBubbleBack.SetHeight(6 + num);
    }

    public void ClearSpellBar()
    {
        KeyValuePair<string, SpellBar>[] array = _spellBar.ToArray();
        foreach (KeyValuePair<string, SpellBar> keyValuePair in array)
        {
            _spellBar[keyValuePair.Key].Remove();
        }
    }

    public bool MoveATile(int direction = 255, bool walk = true)
    {
        if (direction == 255)
        {
            direction = _mBody._direction;
        }
        int num = 0;
        int num2 = 0;
        if (direction == 0)
        {
            num2 = -1;
        }
        if (direction == 1)
        {
            num = 1;
        }
        if (direction == 2)
        {
            num2 = 1;
        }
        if (direction == 3)
        {
            num = -1;
        }
        Location location = new Location(_location.X + num, _location.Y + num2);
        int index = location.Y * (int)_map._width + location.X;
        if (location.X < (int)_map._width && location.Y < (int)_map._height && location.X >= 0 && location.Y >= 0)
        {
            Tile tile = _map._tiles[index];
            if (tile != null && tile._walkable && tile._entities.Count == 0 && _map.tileHasPortal(tile.Location.X, tile.Location.Y) == null)
            {
                if (walk)
                {
                    _moving = true;
                    _mBody.Walk();
                }
                _tile._entities.Remove(_id);
                _location = location;
                _tile = tile;
                _tile._entities.Add(_id, this);
                _tileTime = DateTime.UtcNow;
                Reactor reactor = _map.tileHasReactor((byte)_location.X, (byte)_location.Y);
                if (reactor != null && reactor._type == 0 && reactor._triggerType == 2 && reactor._trap != null)
                {
                    if (reactor._trap._dmg != 0)
                    {
                        DamageHealth(reactor._trap._dmg, null, ignoreMods: true);
                    }
                    if (reactor._trap._status != "")
                    {
                        gs.SpellBarCheck(reactor._trap, this);
                    }
                    _map._reactors.Remove(reactor);
                    _tile._spellAniRepeat = false;
                }
                _oldX = _mBody._position.X;
                _oldY = _mBody._position.Y;
                return true;
            }
        }
        return false;
    }

    public void GenderSwap()
    {
        if (_gender == 0)
        {
            _gender = 1;
            _body._gender = 1;
            _body._g = "m";
        }
        else
        {
            _gender = 0;
            _body._gender = 0;
            _body._g = "w";
        }
    }

    public void HealHealth(int heal)
    {
        if (!_dead)
        {
            _curHP += heal;
            if (_curHP > _maxHP)
            {
                _curHP = _maxHP;
            }
            _displayHP = true;
        }
    }

    public void DamageHealth(int dmg, Entity fromEntity, bool ignoreMods = false)
    {
        if (this is NPC && _hidden)
        {
            return;
        }
        if (fromEntity != null && !ignoreMods)
        {
            if (hasdion)
            {
                return;
            }
            int num = (int)(fromEntity._baseHitChance + (double)(fromEntity._hit / 10));
            int num2 = _random.Next(0, 101);
            if (num2 > num)
            {
                return;
            }
            short num3 = _ac;
            if (_ac < 0)
            {
                num3 = 0;
            }
            if (_ac > 190)
            {
                num3 = 190;
            }
            double num4 = 100 - num3;
            dmg = (int)((double)dmg * (num4 / 100.0 + 1.0));
            double num5 = 1.0;
            num5 = ((fromEntity._atk == "Dark") ? ((_def == "Dark") ? 0.5 : ((_def == "Light") ? 0.8 : ((_def == "Sea") ? 0.65 : ((_def == "Earth") ? 0.65 : ((_def == "Wind") ? 0.65 : ((!(_def == "Fire")) ? 1.0 : 0.65)))))) : ((fromEntity._atk == "Light") ? ((_def == "Dark") ? 0.8 : ((_def == "Light") ? 0.5 : ((_def == "Sea") ? 0.55 : ((_def == "Earth") ? 0.55 : ((_def == "Wind") ? 0.55 : ((!(_def == "Fire")) ? 1.0 : 0.55)))))) : ((fromEntity._atk == "Earth") ? ((_def == "Dark") ? 0.6 : ((_def == "Light") ? 0.65 : ((_def == "Sea") ? 0.85 : ((_def == "Earth") ? 0.5 : ((_def == "Wind") ? 0.55 : ((!(_def == "Fire")) ? 1.0 : 0.65)))))) : ((fromEntity._atk == "Fire") ? ((_def == "Dark") ? 0.6 : ((_def == "Light") ? 0.65 : ((_def == "Sea") ? 0.55 : ((_def == "Earth") ? 0.65 : ((_def == "Wind") ? 0.85 : ((!(_def == "Fire")) ? 1.0 : 0.5)))))) : ((fromEntity._atk == "Sea") ? ((_def == "Dark") ? 0.6 : ((_def == "Light") ? 0.65 : ((_def == "Sea") ? 0.5 : ((_def == "Earth") ? 0.55 : ((_def == "Wind") ? 0.65 : ((!(_def == "Fire")) ? 1.0 : 0.85)))))) : ((fromEntity._atk == "Wind") ? ((_def == "Dark") ? 0.6 : ((_def == "Light") ? 0.65 : ((_def == "Sea") ? 0.65 : ((_def == "Earth") ? 0.85 : ((_def == "Wind") ? 0.5 : ((!(_def == "Fire")) ? 1.0 : 0.55)))))) : ((_def == "Dark") ? 0.4 : ((_def == "Light") ? 0.4 : ((_def == "Sea") ? 0.4 : ((_def == "Earth") ? 0.4 : ((_def == "Wind") ? 0.4 : ((!(_def == "Fire")) ? 0.5 : 0.4))))))))))));
            if (fromEntity.hasdiafas)
            {
                num5 *= 2.5;
            }
            else if (fromEntity.hasardfas)
            {
                num5 *= 2.2;
            }
            else if (fromEntity.hasmorfas)
            {
                num5 *= 1.9;
            }
            else if (fromEntity.hasfas)
            {
                num5 *= 1.6;
            }
            else if (fromEntity.hasbeagfas)
            {
                num5 *= 1.3;
            }
            dmg = (int)((double)dmg * num5);
            if (hasaite)
            {
                dmg /= 2;
            }
            int num6 = (int)(fromEntity._baseCritChance + (double)(fromEntity._dex / 10));
            num2 = _random.Next(0, 101);
            if (num2 < num6)
            {
                dmg *= 2;
            }
            if (fromEntity._location.IsInFront(_location, (D)fromEntity._direction) && !_location.IsInFront(fromEntity._location, (D)_direction))
            {
                dmg = ((!_location.IsBehind(fromEntity._location, (D)_direction)) ? ((int)((double)dmg * 1.5)) : (dmg * 2));
            }
            if (hassleep)
            {
                dmg *= 2;
                _spellBar.Remove("Sleep");
            }
        }
        _curHP -= dmg;
        if (_curHP <= 0)
        {
            _dead = true;
            _curHP = 0;
            if (this is Player && !_spellBar.ContainsKey("Death"))
            {
                gs.NewSpellBar("Death", "spell001", 89, 0.0, "", "You are facing death.", "Restored.", this);
            }
        }
        _displayHP = true;
        _hostile = true;
        if (fromEntity != null)
        {
            _hostileID = fromEntity._id;
        }
    }

    public void UpdateEntityChat(double elapsedTime)
    {
        string[] array = _incomingChat.ToArray();
        foreach (string text in array)
        {
            if (_entityType == EntityType.Npc && !text.StartsWith(_name) && text.StartsWith(gs.ClientName))
            {
                KeyValuePair<string, string>[] array2 = _dialogSpeechTriggers.ToArray();
                for (int j = 0; j < array2.Length; j++)
                {
                    KeyValuePair<string, string> keyValuePair = array2[j];
                    if (text.ToLower().Contains(keyValuePair.Key.ToLower()))
                    {
                        gs.DialogPopup(this, keyValuePair.Value, 0, _mBody._imgArr[_mBody._face - 1]);
                        break;
                    }
                }
            }
            _incomingChat.Remove(text);
        }
        if (_chatText._text != "")
        {
            _cbDelay -= elapsedTime * 1000.0;
            if (_cbDelay <= 0.0)
            {
                _cbDelay = 5000.0;
                _chatText.ChangeText("");
            }
        }
    }

    public void RepositionTargetBox()
    {
        if (_tile != null)
        {
            _targetBox.X = (int)(_tile._position.X + _tile._width / 2.0 - (double)(_targetBox.Width / 2));
            if (_targetBox.Height == 27)
            {
                _targetBox.Y = (int)(_tile._position.Y + _tile._height - (double)_targetBox.Height - 2.0);
            }
            else
            {
                _targetBox.Y = (int)(_tile._position.Y + _tile._height - (double)_targetBox.Height - 6.0);
            }
            _hpBarSprite.SetPosition(_targetBox.X + 1, _targetBox.Y - 5);
        }
    }

    public void DrawSpellMeter(Renderer renderer)
    {
        if (_displaySpellMeter)
        {
            renderer.DrawBorder(new Rectangle((int)_spellMeterSprite.GetPosition().X, (int)_spellMeterSprite.GetPosition().Y, 25, 4), Engine.Color.Black);
        }
    }

    public void UpdateHealthBar(double elapsedTime)
    {
        if (_displayHP)
        {
            _hpBarDelay -= elapsedTime * 1000.0;
            if (_hpBarDelay <= 0.0)
            {
                _hpBarDelay = 8000.0;
                _displayHP = false;
            }
        }
    }

    public void DrawHealthBar(Renderer renderer)
    {
        if (_displayHP)
        {
            bool flag = true;
            Engine.Color color = Text.Colors(System.Drawing.Color.Green);
            int num = (int)((double)_curHP / (double)_maxHP * 100.0);
            if (num == 100)
            {
                _hpBarSprite.SetWidth(25f);
            }
            else if (num > 75)
            {
                _hpBarSprite.SetWidth(20f);
            }
            else if (num > 50)
            {
                _hpBarSprite.SetWidth(15f);
            }
            else if (num > 25)
            {
                color = Text.Colors(System.Drawing.Color.Orange);
                _hpBarSprite.SetWidth(10f);
            }
            else if (num > 0)
            {
                color = Text.Colors(System.Drawing.Color.Red);
                _hpBarSprite.SetWidth(5f);
            }
            else
            {
                flag = false;
            }
            if (flag)
            {
                _hpBarSprite.SetColor(color);
                renderer.DrawSprite(_hpBarSprite, 0);
            }
            renderer.DrawBorder(new Rectangle((int)_hpBarSprite.GetPosition().X, (int)_hpBarSprite.GetPosition().Y, 25, 4), Engine.Color.Black);
        }
    }

    public void SetPosition(double x, double y, bool ignore = false)
    {
        SetPosition(new Vector(x, y, 0.0), ignore);
    }

    public void SetPosition(Vector position, bool ignore = false)
    {
        if (_tile != null)
        {
            _targetBox.X = (int)(_tile._position.X + _tile._width / 2.0 - (double)(_targetBox.Width / 2));
            if (_targetBox.Height == 27)
            {
                _targetBox.Y = (int)(_tile._position.Y + _tile._height - (double)_targetBox.Height - 2.0);
            }
            else
            {
                _targetBox.Y = (int)(_tile._position.Y + _tile._height - (double)_targetBox.Height - 6.0);
            }
            _hpBarSprite.SetPosition(_targetBox.X + 1, _targetBox.Y - 5);
            int num = (int)(_tile._position.X + _tile._width / 2.0 - _nameTag.Width / 2.0) + 2;
            int num2 = (int)(_tile._position.Y + _tile._height - (double)_targetBox.Height - 26.0);
            _nameTag.SetPosition(num, num2);
        }
        if (_mBody != null)
        {
            _mBody._sprite.SetPosition(position);
            _mBody._position = position;
            double x = Math.Abs(_mBody._sprite.GetWidth() / 2.0) + _mBody._position.X - (double)(_mBody._maxSpellWidth / 2);
            double y = _mBody._position.Y + _mBody._sprite.GetHeight() - (double)_mBody._maxSpellHeight;
            _mBody._spellAni.SetPosition(x, y);
        }
        if (_body != null)
        {
            _body.SetPosition(position.X, position.Y);
            double x2 = 28.0 + _body._position.X - (double)(_body._maxSpellWidth / 2);
            double y2 = _body._position.Y + 85.0 - (double)_body._maxSpellHeight;
            _body._spellAni.SetPosition(x2, y2);
        }
        _sprite.SetPosition(position, ignore);
        double x3 = position.X;
        double y3 = position.Y;
        if (_mBody != null)
        {
            x3 = _mBody._position.X;
            y3 = _mBody._position.Y;
        }
        if (_body != null)
        {
            x3 = _body._position.X;
            y3 = _body._position.Y;
        }
        int num3 = _chatText._lines * 12;
        Vector vector = new Vector(x3 - 32.0, y3 - (double)(14 + num3), 0.0);
        _chatText.SetPosition(vector.X + 6.0, vector.Y + 3.0, colorize: true);
        _chatBubbleBack.SetPosition(vector.X, vector.Y);
        _chatBubbleBack.SetHeight(6 + num3);
    }

    public void CenterEntity()
    {
        if (_tile != null)
        {
            _targetBox.X = (int)(_tile._position.X + _tile._width / 2.0 - (double)(_targetBox.Width / 2));
            if (_targetBox.Height == 27)
            {
                _targetBox.Y = (int)(_tile._position.Y + _tile._height - (double)_targetBox.Height - 2.0);
            }
            else
            {
                _targetBox.Y = (int)(_tile._position.Y + _tile._height - (double)_targetBox.Height - 6.0);
            }
            _hpBarSprite.SetPosition(_targetBox.X + 1, _targetBox.Y - 5);
            int num = (int)(_tile._position.X + _tile._width / 2.0 - _nameTag.Width / 2.0) + 2;
            int num2 = (int)(_tile._position.Y + _tile._height - (double)_targetBox.Height - 26.0);
            _nameTag.SetPosition(num, num2);
            if (_body != null)
            {
                _body.SetPosition(_tile._position.X + _tile._width / 2.0 - Math.Abs(_body._body.GetWidth()) / 2.0 + 1.0, _tile._position.Y + _tile._height - _body._body.GetHeight() + 1.0);
                double x = 28.0 + _body._position.X - (double)(_body._maxSpellWidth / 2);
                double y = _body._position.Y + 85.0 - (double)_body._maxSpellHeight;
                _body._spellAni.SetPosition(x, y);
            }
            if (_mBody != null)
            {
                Vector position = new Vector(_tile._position.X + _tile._width / 2.0 - Math.Abs(_mBody._sprite.GetWidth()) / 2.0 + 1.0, _tile._position.Y + _tile._height - _mBody._sprite.GetHeight() + 1.0, 0.0);
                _mBody._sprite.SetPosition(position);
                _mBody._position = position;
                double x2 = Math.Abs(_mBody._sprite.GetWidth() / 2.0) + _mBody._position.X - (double)(_mBody._maxSpellWidth / 2);
                double y2 = _mBody._position.Y + _mBody._sprite.GetHeight() - (double)_mBody._maxSpellHeight;
                _mBody._spellAni.SetPosition(x2, y2);
            }
            _sprite.SetPosition(_tile._position.X + _tile._width / 2.0 - Math.Abs(_sprite.GetWidth()) / 2.0 + 1.0, _tile._position.Y + _tile._height - _sprite.GetHeight() + 1.0);
            double num3 = 0.0;
            double num4 = 0.0;
            if (_mBody != null)
            {
                num3 = _mBody._position.X;
                num4 = _mBody._position.Y;
            }
            if (_body != null)
            {
                num3 = _body._position.X;
                num4 = _body._position.Y;
            }
            int num5 = _chatText._lines * 12;
            Vector vector = new Vector(num3 - 32.0, num4 - (double)(14 + num5), 0.0);
            _chatText.SetPosition(vector.X + 6.0, vector.Y + 3.0, colorize: true);
            _chatBubbleBack.SetPosition(vector.X, vector.Y);
            _chatBubbleBack.SetHeight(6 + num5);
        }
    }

    public bool CollidesWith(Engine.Point point)
    {
        if (point.X >= (float)_targetBox.X && point.X <= (float)(_targetBox.X + _targetBox.Width) && point.Y >= (float)_targetBox.Y && point.Y <= (float)(_targetBox.Y + _targetBox.Height))
        {
            return true;
        }
        return false;
    }

    public void Render_Debug()
    {
        Gl.glDisable(3553);
        Rectangle targetBox = _targetBox;
        Gl.glBegin(2);
        Gl.glColor3f(1f, 0f, 0f);
        Gl.glVertex2i(targetBox.Left, targetBox.Top);
        Gl.glVertex2i(targetBox.Right, targetBox.Top);
        Gl.glVertex2i(targetBox.Right, targetBox.Bottom);
        Gl.glVertex2i(targetBox.Left, targetBox.Bottom);
        Gl.glEnd();
        Gl.glEnable(3553);
    }
}
