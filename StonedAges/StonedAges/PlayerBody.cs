using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Engine;
using Tao.OpenGl;

namespace StonedAges;

public class PlayerBody
{
    private const double _origIdleBodyDelay = 240.0;

    private const double _origIdleWeaponDelay = 240.0;

    private const double _origIdleAccessoryDelay = 240.0;

    private const double _origIdleAccessorybDelay = 240.0;

    public bool _ghost;

    public bool _hidden;

    public bool _swimming;

    public bool _bloody;

    public bool _repeat;

    public bool _repeatOnce;

    public int _maxSpellWidth;

    public int _maxSpellHeight;

    public bool _attacking;

    public bool _walking;

    public bool _emoting;

    public bool _idling;

    private double _idleBodyDelay = 240.0;

    public int _idleBodyCount = -1;

    private double _idleWeaponDelay = 240.0;

    public int _idleWeaponCount = -1;

    private double _idleAccessoryDelay = 240.0;

    public int _idleAccessoryCount = -1;

    private double _idleAccessorybDelay = 240.0;

    public int _idleAccessorybCount = -1;

    private double _origAniDelay;

    private int _aniCount;

    private double _aniDelay;

    private int _frameCount = 10;

    private int _startFrame;

    public string _aniType = "01";

    public bool _spellanimating;

    private double _origSpellAniDelay;

    private int _spellAniCount = -1;

    private double _spellAniDelay;

    private int _spellFrameCount;

    public int _spellAniType;

    private List<Texture> _spellAniArray = new List<Texture>();

    private double _origEmoteAniDelay;

    private int _emoteAniCount = -1;

    private double _emoteAniDelay;

    private int _emoteFrameCount;

    public int _emoteAniType;

    public double _scale = 1.0;

    public int _gender;

    public string _g;

    public Texture _blankTexture = default(Texture);

    private TextureManager _textureManager;

    public int _face = 6;

    public Vector _position = default(Vector);

    public double _offSet;

    public int _direction = 1;

    public Sprite _spellAni = new Sprite();

    public Sprite _emoteAni = new Sprite();

    public Sprite _faceshape = new Sprite();

    public Sprite _hair = new Sprite();

    public Sprite _helm = new Sprite();

    public Sprite _body = new Sprite();

    public Sprite _arm = new Sprite();

    public Sprite _weapon = new Sprite();

    public Sprite _armor = new Sprite();

    public Sprite _shield = new Sprite();

    public Sprite _legs = new Sprite();

    public Sprite _boots = new Sprite();

    public Sprite _accessory = new Sprite();

    public Sprite _accessoryb = new Sprite();

    public Dictionary<string, int> _bodyImgs = new Dictionary<string, int>();

    public Dictionary<string, int> _bodyColors = new Dictionary<string, int>();

    public Dictionary<string, int> _bodyFolders = new Dictionary<string, int>();

    public Dictionary<string, string> _bodySource = new Dictionary<string, string>();

    public int _hairType;

    public int _hairColor;

    public int _helmType;

    public int _helmColor;

    public PlayerBody(TextureManager textureManager, int gender)
    {
        _bodyImgs.Add("o", 1);
        _bodyImgs.Add("b", 0);
        _bodyImgs.Add("a", 0);
        _bodyImgs.Add("u", 0);
        _bodyImgs.Add("h", 0);
        _bodyImgs.Add("l", 0);
        _bodyImgs.Add("n", 0);
        _bodyImgs.Add("s", 0);
        _bodyImgs.Add("w", 0);
        _bodyImgs.Add("c", 0);
        _bodyImgs.Add("g", 0);
        _bodyColors.Add("o", 0);
        _bodyColors.Add("b", 0);
        _bodyColors.Add("a", 0);
        _bodyColors.Add("u", 0);
        _bodyColors.Add("h", 0);
        _bodyColors.Add("l", 0);
        _bodyColors.Add("n", 1);
        _bodyColors.Add("s", 0);
        _bodyColors.Add("w", 0);
        _bodyColors.Add("c", 0);
        _bodyColors.Add("g", 0);
        _bodySource.Add("o", "");
        _bodySource.Add("b", "");
        _bodySource.Add("a", "");
        _bodySource.Add("u", "");
        _bodySource.Add("h", "");
        _bodySource.Add("l", "");
        _bodySource.Add("n", "");
        _bodySource.Add("s", "");
        _bodySource.Add("w", "");
        _bodySource.Add("c", "");
        _bodySource.Add("g", "");
        _textureManager = textureManager;
        _gender = gender;
        if (_gender == 0)
        {
            _g = "w";
        }
        else
        {
            _g = "m";
        }
        setTextures();
    }

    public void Update(double elapsedTime)
    {
        if ((_walking || _attacking) && !_emoting)
        {
            _aniDelay -= elapsedTime * 1000.0;
            if (_aniDelay <= 0.0)
            {
                _aniDelay = _origAniDelay;
                if (_aniCount >= _frameCount / 2)
                {
                    _aniCount = -1;
                }
                if (_aniCount == -1)
                {
                    _aniCount++;
                    if (!_repeatOnce)
                    {
                        setDefault();
                        _walking = false;
                        _attacking = false;
                        _emoting = false;
                        _idleWeaponCount = -1;
                        _idleBodyCount = -1;
                        return;
                    }
                    _repeatOnce = false;
                }
                setTextures();
                _aniCount++;
            }
        }
        else if (!_emoting)
        {
            bool flag = false;
            _aniCount = 0;
            if (_swimming)
            {
                flag = true;
                _idleBodyDelay -= elapsedTime * 1000.0;
                if (_idleBodyDelay <= 0.0)
                {
                    _idleBodyDelay = 240.0;
                    if (_idleBodyCount >= 3)
                    {
                        _idleBodyCount = -1;
                    }
                    _idleBodyCount++;
                    flipSprites();
                    int num = ((_direction != 0 && _direction != 3) ? (_idleBodyCount + 4) : _idleBodyCount);
                    if (_g == "w")
                    {
                        _body.Texture = _textureManager.Get("wb00604_F" + num + "_new_C0", ".epf", "new", null, khan: true);
                    }
                    else
                    {
                        _body.Texture = _textureManager.Get("mb00504_F" + num + "_new_C0", ".epf", "new", null, khan: true);
                    }
                }
            }
            else
            {
                EPFImage ePFImage = _textureManager.getEPFImage(_g + "w" + _bodyImgs["w"].ToString("000") + "04.epf");
                if (ePFImage != null)
                {
                    int num2 = ePFImage.Frames.Count();
                    if (num2 > 0)
                    {
                        flag = true;
                        _idleWeaponDelay -= elapsedTime * 1000.0;
                        if (_idleWeaponDelay <= 0.0)
                        {
                            _idleWeaponDelay = 240.0;
                            if (_idleWeaponCount >= num2 / 2 - 1)
                            {
                                _idleWeaponCount = -1;
                            }
                            _idleWeaponCount++;
                            setDefault(ignoreWeapon: true);
                            string text = "";
                            if (_bloody)
                            {
                                text = "_B";
                            }
                            int num3 = ((_direction != 0 && _direction != 3) ? (_idleWeaponCount + num2 / 2) : _idleWeaponCount);
                            string text2 = "";
                            if (_bodySource["w"] != "old")
                            {
                                text2 = "_" + _bodySource["w"];
                            }
                            _weapon.Texture = _textureManager.Get(_g + "w" + _bodyImgs["w"].ToString("000") + "04_F" + num3 + text + text2 + "_C0", ".epf", _bodySource["w"], null, khan: true, _bloody);
                        }
                    }
                }
                EPFImage ePFImage2 = _textureManager.getEPFImage(_g + "c" + _bodyImgs["c"].ToString("000") + "04.epf");
                if (ePFImage2 != null)
                {
                    int num4 = ePFImage2.Frames.Count();
                    if (num4 > 0)
                    {
                        flag = true;
                        _idleAccessoryDelay -= elapsedTime * 1000.0;
                        if (_idleAccessoryDelay <= 0.0)
                        {
                            _idleAccessoryDelay = 240.0;
                            if (_idleAccessoryCount >= num4 / 2 - 1)
                            {
                                _idleAccessoryCount = -1;
                            }
                            _idleAccessoryCount++;
                            setDefault(ignoreWeapon: true);
                            int num5 = ((_direction != 0 && _direction != 3) ? (_idleAccessoryCount + num4 / 2) : _idleAccessoryCount);
                            _accessory.Texture = _textureManager.Get(_g + "c" + _bodyImgs["c"].ToString("000") + "04_F" + num5.ToString() + "_new_C" + _bodyColors["c"], ".epf", "new", null, khan: true);
                        }
                    }
                }
                EPFImage ePFImage3 = _textureManager.getEPFImage(_g + "g" + _bodyImgs["g"].ToString("000") + "04.epf");
                if (ePFImage3 != null)
                {
                    int num6 = ePFImage3.Frames.Count();
                    if (num6 > 0)
                    {
                        flag = true;
                        _idleAccessorybDelay -= elapsedTime * 1000.0;
                        if (_idleAccessorybDelay <= 0.0)
                        {
                            _idleAccessorybDelay = 240.0;
                            if (_idleAccessorybCount >= num6 / 2 - 1)
                            {
                                _idleAccessorybCount = -1;
                            }
                            _idleAccessorybCount++;
                            setDefault(ignoreWeapon: true);
                            int num7 = ((_direction != 0 && _direction != 3) ? (_idleAccessorybCount + num6 / 2) : _idleAccessorybCount);
                            _accessoryb.Texture = _textureManager.Get(_g + "g" + _bodyImgs["g"].ToString("000") + "04_F" + num7.ToString() + "_new_C" + _bodyColors["g"], ".epf", "new", null, khan: true);
                        }
                    }
                }
            }
            if (!flag)
            {
                setDefault();
            }
        }
        if (_emoting)
        {
            _emoteAniDelay -= elapsedTime * 1000.0;
            if (_emoteAniDelay <= 0.0)
            {
                _emoteAniDelay = _origEmoteAniDelay;
                if (_emoteAniCount >= _emoteFrameCount)
                {
                    _emoteAniCount = -1;
                }
                if (_emoteAniCount == -1)
                {
                    _emoteAniCount++;
                    _emoteAni.Texture = _blankTexture;
                    _emoting = false;
                    return;
                }
                if (_direction == 0 || _direction == 3)
                {
                    _emoteAni.Texture = _blankTexture;
                }
                else
                {
                    _emoteAni.Texture = _textureManager.Get("emot01_F" + (_emoteAniType + _emoteAniCount) + "_C0");
                }
                _emoteAniCount++;
            }
        }
        else
        {
            _emoteAniCount = 0;
            _emoteAni.Texture = _blankTexture;
        }
        if (_spellanimating)
        {
            _spellAniDelay -= elapsedTime * 1000.0;
            if (!(_spellAniDelay <= 0.0))
            {
                return;
            }
            _spellAniDelay = _origSpellAniDelay;
            if (_spellAniCount >= _spellFrameCount)
            {
                _spellAniCount = -1;
            }
            if (_spellAniCount == -1)
            {
                _spellAniCount++;
                if (!_repeat)
                {
                    _spellAni.Texture = _blankTexture;
                    _spellanimating = false;
                    return;
                }
            }
            _spellAni.Texture = _spellAniArray[_spellAniCount];
            _spellAniCount++;
        }
        else
        {
            _spellAniCount = 0;
            _spellAni.Texture = _blankTexture;
        }
    }

    public void Emote(int speed, int startframe, int endframe)
    {
        _emoteAniDelay = 0.0;
        _emoteAniType = startframe;
        _origEmoteAniDelay = speed;
        _emoteFrameCount = endframe - startframe;
        _emoting = true;
    }

    public void SpellAni(int spellType, int speed, bool repeat = false)
    {
        _spellAniDelay = 0.0;
        _repeat = repeat;
        _spellAniCount = 0;
        _spellAni.Texture = _blankTexture;
        List<int> list = _textureManager.EffectTable()[spellType - 1];
        _origSpellAniDelay = speed;
        _spellAniType = spellType;
        _spellFrameCount = list.Count;
        _spellAniArray.Clear();
        _maxSpellWidth = 0;
        _maxSpellHeight = 0;
        foreach (int item2 in list)
        {
            Texture item = _textureManager.Get("efct" + _spellAniType.ToString("000") + "_F" + item2 + "_C0");
            _spellAniArray.Add(item);
            if (item.Width > _maxSpellWidth)
            {
                _maxSpellWidth = item.Width;
            }
            if (item.Height > _maxSpellHeight)
            {
                _maxSpellHeight = item.Height;
            }
        }
        double x = 28.0 + _position.X - (double)(_maxSpellWidth / 2);
        double y = _position.Y + 85.0 - (double)_maxSpellHeight;
        _spellAni.SetPosition(x, y);
        _spellanimating = true;
    }

    public void SetAni(string aniType, int speed, int startframe, int endframe, bool repeatOnce = false)
    {
        _repeatOnce = repeatOnce;
        _aniDelay = 0.0;
        _origAniDelay = speed;
        _aniType = aniType;
        _startFrame = startframe;
        _frameCount = endframe - startframe;
    }

    public void setDefault(bool ignoreWeapon = false)
    {
        string text = "";
        if (_bodySource["w"] != "old")
        {
            text = "_" + _bodySource["w"];
        }
        string text2 = "";
        if (_bloody)
        {
            text2 = "_B";
        }
        flipSprites();
        int num = ((_direction != 0 && _direction != 3) ? 5 : 0);
        if (!_swimming && !_ghost)
        {
            _bodyImgs["b"] = 1;
        }
        if (_swimming)
        {
            if (_g == "w")
            {
                _bodyImgs["b"] = 6;
                _body.Texture = _textureManager.Get("wb00601_F" + num + "_new_C0", ".epf", "new", null, khan: true);
            }
            else
            {
                _bodyImgs["b"] = 5;
                _body.Texture = _textureManager.Get("mb00501_F" + num + "_new_C0", ".epf", "new", null, khan: true);
            }
        }
        else if (_ghost)
        {
            _body.Texture = _textureManager.Get(_g + "b00201_F" + num + "_C0", ".epf", "old", null, khan: true);
        }
        else if (_hidden)
        {
            _body.Texture = _textureManager.Get("mb00301_F" + num + "_C0", ".epf", "old", null, khan: true);
        }
        else if (_bodySource["u"] == "new")
        {
            _body.Texture = _textureManager.Get(_g + "b" + _bodyImgs["b"].ToString("000") + "01_F" + num + "_new_C0", ".epf", "new", null, khan: true);
        }
        else
        {
            _body.Texture = _textureManager.Get(_g + "b" + _bodyImgs["b"].ToString("000") + "01_F" + num + "_C0", ".epf", "old", null, khan: true);
        }
        if (_bodyImgs["a"] == 320)
        {
            _arm.Texture = _blankTexture;
        }
        else if (_bodyImgs["a"] > 1000)
        {
            _arm.Texture = _textureManager.Get(_g + "j" + (_bodyImgs["a"] - 1000).ToString("000") + "01_F" + num + "_new_C" + _bodyColors["a"], ".epf", "new", null, khan: true);
        }
        else if (_g == "w" && _bodyImgs["u"] == 35)
        {
            _arm.Texture = _textureManager.Get(_g + "a18701_F" + num + "_C" + _bodyColors["a"], ".epf", "old", null, khan: true);
        }
        else
        {
            _arm.Texture = _textureManager.Get(_g + "a" + _bodyImgs["a"].ToString("000") + "01_F" + num + "_C" + _bodyColors["a"], ".epf", "old", null, khan: true);
        }
        _hair.Texture = _textureManager.Get(_g + "h" + _hairType.ToString("000") + "01_F" + num + "_C" + _hairColor, ".epf", "old", null, khan: true);
        _helm.Texture = _textureManager.Get(_g + "h" + _helmType.ToString("000") + "01_F" + num + "_C" + _helmColor, ".epf", "old", null, khan: true);
        if (!ignoreWeapon)
        {
            _accessoryb.Texture = _textureManager.Get(_g + "g" + _bodyImgs["g"].ToString("000") + "01_F" + num + "_new_C" + _bodyColors["g"], ".epf", "new", null, khan: true);
        }
        if (!ignoreWeapon)
        {
            _accessory.Texture = _textureManager.Get(_g + "c" + _bodyImgs["c"].ToString("000") + "01_F" + num + "_new_C" + _bodyColors["c"], ".epf", "new", null, khan: true);
        }
        if (!ignoreWeapon)
        {
            if ((_bodyImgs["w"] <= 11 || _bodyImgs["w"] == 130 || _bodyImgs["w"] == 131) && _g == "w")
            {
                _weapon.Texture = _textureManager.Get("mw" + _bodyImgs["w"].ToString("000") + "01_F" + num + text2 + text + "_C0", ".epf", _bodySource["w"], null, khan: true, _bloody);
            }
            else if (_bodyImgs["w"] == 195 || _bodyImgs["w"] == 199)
            {
                _weapon.Texture = _textureManager.Get("ww" + _bodyImgs["w"].ToString("000") + "01_F" + num + text2 + text + "_C0", ".epf", _bodySource["w"], null, khan: true, _bloody);
            }
            else
            {
                _weapon.Texture = _textureManager.Get(_g + "w" + _bodyImgs["w"].ToString("000") + "01_F" + num + text2 + text + "_C0", ".epf", _bodySource["w"], null, khan: true, _bloody);
            }
        }
        if (_bodyImgs["u"] > 1000)
        {
            _armor.Texture = _textureManager.Get(_g + "i" + (_bodyImgs["u"] - 1000).ToString("000") + "01_F" + num + "_new_C" + _bodyColors["u"], ".epf", "new", null, khan: true);
        }
        else if (_g == "w" && _bodyImgs["u"] == 35)
        {
            _armor.Texture = _textureManager.Get(_g + "u18701_F" + num + "_C" + _bodyColors["u"], ".epf", "old", null, khan: true);
        }
        else
        {
            _armor.Texture = _textureManager.Get(_g + "u" + _bodyImgs["u"].ToString("000") + "01_F" + num + "_C" + _bodyColors["u"], ".epf", "old", null, khan: true);
        }
        if (_bodyImgs["s"] == 196)
        {
            _shield.Texture = _textureManager.Get("ws19501_F" + num + "_C0", ".epf", "old", null, khan: true);
        }
        else
        {
            _shield.Texture = _textureManager.Get("ms" + _bodyImgs["s"].ToString("000") + "01_F" + num + "_C0", ".epf", "old", null, khan: true);
        }
        _legs.Texture = _textureManager.Get(_g + "n" + _bodyImgs["n"].ToString("000") + "01_F" + num + "_C" + _bodyColors["n"], ".epf", "old", null, khan: true);
        _boots.Texture = _textureManager.Get(_g + "l" + _bodyImgs["l"].ToString("000") + "01_F" + num + "_C" + _bodyColors["l"], ".epf", "old", null, khan: true);
    }

    private void setTextures()
    {
        string text = "";
        if (_bodySource["w"] != "old")
        {
            text = "_" + _bodySource["w"];
        }
        string text2 = "";
        if (_bloody)
        {
            text2 = "_B";
        }
        flipSprites();
        int num = ((_direction != 0 && _direction != 3) ? (_startFrame + _aniCount + _frameCount / 2) : (_startFrame + _aniCount));
        if (!_swimming && !_ghost)
        {
            _bodyImgs["b"] = 1;
        }
        if (_swimming)
        {
            if (_g == "w")
            {
                _bodyImgs["b"] = 6;
                _body.Texture = _textureManager.Get("wb006" + _aniType + "_F" + num + "_new_C0", ".epf", "new", null, khan: true);
            }
            else
            {
                _bodyImgs["b"] = 5;
                _body.Texture = _textureManager.Get("mb005" + _aniType + "_F" + num + "_new_C0", ".epf", "new", null, khan: true);
            }
        }
        else if (_ghost)
        {
            _body.Texture = _textureManager.Get(_g + "b00201_F" + num + "_C0", ".epf", "old", null, khan: true);
        }
        else if (_hidden)
        {
            _body.Texture = _textureManager.Get("mb00301_F" + num + "_C0", ".epf", "old", null, khan: true);
        }
        else if (_bodyImgs["u"] == 981)
        {
            _body.Texture = _textureManager.Get(_g + "b" + _bodyImgs["b"].ToString("000") + _aniType + "_F" + num + "_new_C0", ".epf", "new", null, khan: true);
        }
        else
        {
            _body.Texture = _textureManager.Get(_g + "b" + _bodyImgs["b"].ToString("000") + _aniType + "_F" + num + "_C0", ".epf", "old", null, khan: true);
        }
        if (_bodyImgs["a"] == 320)
        {
            _arm.Texture = _blankTexture;
        }
        else if (_bodyImgs["a"] > 1000)
        {
            _arm.Texture = _textureManager.Get(_g + "j" + (_bodyImgs["a"] - 1000).ToString("000") + _aniType + "_F" + num + "_new_C" + _bodyColors["a"], ".epf", "new", null, khan: true);
        }
        else if (_g == "w" && _bodyImgs["u"] == 35)
        {
            _arm.Texture = _textureManager.Get(_g + "a187" + _aniType + "_F" + num + "_C" + _bodyColors["a"], ".epf", "old", null, khan: true);
        }
        else
        {
            _arm.Texture = _textureManager.Get(_g + "a" + _bodyImgs["a"].ToString("000") + _aniType + "_F" + num + "_C" + _bodyColors["a"], ".epf", "old", null, khan: true);
        }
        _hair.Texture = _textureManager.Get(_g + "h" + _hairType.ToString("000") + _aniType + "_F" + num + "_C" + _hairColor, ".epf", "old", null, khan: true);
        _helm.Texture = _textureManager.Get(_g + "h" + _helmType.ToString("000") + _aniType + "_F" + num + "_C" + _helmColor, ".epf", "old", null, khan: true);
        _accessoryb.Texture = _textureManager.Get(_g + "g" + _bodyImgs["g"].ToString("000") + _aniType + "_F" + num + "_new_C" + _bodyColors["g"], ".epf", "new", null, khan: true);
        _accessory.Texture = _textureManager.Get(_g + "c" + _bodyImgs["c"].ToString("000") + _aniType + "_F" + num + "_new_C" + _bodyColors["c"], ".epf", "new", null, khan: true);
        if ((_bodyImgs["w"] <= 11 || _bodyImgs["w"] == 130 || _bodyImgs["w"] == 131) && _g == "w")
        {
            _weapon.Texture = _textureManager.Get("mw" + _bodyImgs["w"].ToString("000") + _aniType + "_F" + num + text2 + text + "_C0", ".epf", _bodySource["w"], null, khan: true, _bloody);
        }
        else if (_bodyImgs["w"] == 195 || _bodyImgs["w"] == 199)
        {
            _weapon.Texture = _textureManager.Get("ww" + _bodyImgs["w"].ToString("000") + _aniType + "_F" + num + text2 + text + "_C0", ".epf", _bodySource["w"], null, khan: true, _bloody);
        }
        else
        {
            _weapon.Texture = _textureManager.Get(_g + "w" + _bodyImgs["w"].ToString("000") + _aniType + "_F" + num + text2 + text + "_C0", ".epf", _bodySource["w"], null, khan: true, _bloody);
        }
        if (_bodyImgs["u"] > 1000)
        {
            _armor.Texture = _textureManager.Get(_g + "i" + (_bodyImgs["u"] - 1000).ToString("000") + _aniType + "_F" + num + "_new_C" + _bodyColors["u"], ".epf", "new", null, khan: true);
        }
        else if (_g == "w" && _bodyImgs["u"] == 35)
        {
            _armor.Texture = _textureManager.Get(_g + "u187" + _aniType + "_F" + num + "_C" + _bodyColors["u"], ".epf", "old", null, khan: true);
        }
        else
        {
            _armor.Texture = _textureManager.Get(_g + "u" + _bodyImgs["u"].ToString("000") + _aniType + "_F" + num + "_C" + _bodyColors["u"], ".epf", "old", null, khan: true);
        }
        if (_bodyImgs["s"] == 196)
        {
            _shield.Texture = _textureManager.Get("ws195" + _aniType + "_F" + num + "_C0", ".epf", "old", null, khan: true);
        }
        else
        {
            _shield.Texture = _textureManager.Get("ms" + _bodyImgs["s"].ToString("000") + _aniType + "_F" + num + "_C0", ".epf", "old", null, khan: true);
        }
        _legs.Texture = _textureManager.Get(_g + "n" + _bodyImgs["n"].ToString("000") + _aniType + "_F" + num + "_C" + _bodyColors["n"], ".epf", "old", null, khan: true);
        _boots.Texture = _textureManager.Get(_g + "l" + _bodyImgs["l"].ToString("000") + _aniType + "_F" + num + "_C" + _bodyColors["l"], ".epf", "old", null, khan: true);
    }

    private void flipSprites()
    {
        if (_direction == 0 || _direction == 1)
        {
            _faceshape._flip = false;
            _body._flip = false;
            _arm._flip = false;
            _hair._flip = false;
            _helm._flip = false;
            _weapon._flip = false;
            _armor._flip = false;
            _shield._flip = false;
            _legs._flip = false;
            _boots._flip = false;
            _emoteAni._flip = false;
            _accessory._flip = false;
            _accessoryb._flip = false;
        }
        else
        {
            _faceshape._flip = true;
            _body._flip = true;
            _arm._flip = true;
            _hair._flip = true;
            _helm._flip = true;
            _weapon._flip = true;
            _armor._flip = true;
            _shield._flip = true;
            _legs._flip = true;
            _boots._flip = true;
            _emoteAni._flip = true;
            _accessory._flip = true;
            _accessoryb._flip = true;
        }
    }

    public void Render(Renderer renderer, bool swimming = false, bool hidden = false, bool ghosted = false, bool seethru = false, byte colorize = 0)
    {
        if (_hidden || _swimming)
        {
            if (_hidden && !_walking)
            {
                setDefault();
            }
            renderer.DrawSprite(_body, 0);
            return;
        }
        if (_ghost)
        {
            renderer.DrawSprite(_body, 0);
            renderer.DrawSprite(_hair, 0);
            return;
        }
        if (_bodyImgs["u"] == 184 || _bodyImgs["u"] == 42 || _bodyImgs["u"] == 104 || _bodyImgs["u"] == 320)
        {
            renderer.DrawSprite(_body, colorize);
            renderer.DrawSprite(_armor, colorize);
            return;
        }
        if (_bodyImgs["g"] != 0)
        {
            renderer.DrawSprite(_accessoryb, colorize);
        }
        if ((_direction == 3 || _direction == 0) && _bodyImgs["s"] != 0)
        {
            renderer.DrawSprite(_shield, colorize);
        }
        renderer.DrawSprite(_body, colorize);
        renderer.DrawSprite(_emoteAni, colorize);
        if (_hairType != 0 && _helmType == 0)
        {
            renderer.DrawSprite(_hair, colorize);
        }
        if (_g == "m" && (_bodyImgs["u"] == 1 || _bodyImgs["u"] == 2 || _bodyImgs["u"] == 5 || _bodyImgs["u"] == 6 || _bodyImgs["u"] == 7 || _bodyImgs["u"] == 8 || _bodyImgs["u"] == 12 || _bodyImgs["u"] == 186 || _bodyImgs["u"] == 195 || _bodyImgs["u"] == 196 || _bodyImgs["u"] == 204))
        {
            renderer.DrawSprite(_legs, colorize);
        }
        if (_helmType != 0)
        {
            renderer.DrawSprite(_helm, colorize);
        }
        if (_bodyImgs["u"] != 196 && _bodyImgs["u"] != 195 && _bodyImgs["l"] != 0)
        {
            renderer.DrawSprite(_boots, colorize);
        }
        if (_bodyImgs["u"] != 0)
        {
            renderer.DrawSprite(_armor, colorize);
        }
        if ((_bodyImgs["u"] == 196 || _bodyImgs["u"] == 195) && _bodyImgs["l"] != 0)
        {
            renderer.DrawSprite(_boots, colorize);
        }
        renderer.DrawSprite(_arm, colorize);
        if (_bodyImgs["c"] != 0)
        {
            renderer.DrawSprite(_accessory, colorize);
        }
        if (_bodyImgs["w"] != 0)
        {
            renderer.DrawSprite(_weapon, colorize);
        }
        if ((_direction == 1 || _direction == 2) && _bodyImgs["s"] != 0)
        {
            renderer.DrawSprite(_shield, colorize);
        }
        renderer.DrawSprite(_spellAni, 0);
    }

    public void SetScale(double scale)
    {
        _scale = scale;
        _faceshape.SetScale(_scale, _scale);
        _body.SetScale(_scale, _scale);
        _arm.SetScale(_scale, _scale);
        _hair.SetScale(_scale, _scale);
        _helm.SetScale(_scale, _scale);
        _weapon.SetScale(_scale, _scale);
        _armor.SetScale(_scale, _scale);
        _shield.SetScale(_scale, _scale);
        _legs.SetScale(_scale, _scale);
        _boots.SetScale(_scale, _scale);
        _emoteAni.SetScale(_scale, _scale);
        _accessory.SetScale(_scale, _scale);
    }

    public void SetPosition(double x, double y, bool ignore = false)
    {
        if (!ignore)
        {
            _position.X = x;
            _position.Y = y;
        }
        _faceshape.SetPosition(x, y);
        _hair.SetPosition(x, y);
        _helm.SetPosition(x, y);
        _body.SetPosition(x, y);
        _arm.SetPosition(x, y);
        _accessoryb.SetPosition(x - 27.0, y);
        _accessory.SetPosition(x - 27.0, y);
        _weapon.SetPosition(x - 27.0, y);
        _armor.SetPosition(x, y);
        _shield.SetPosition(x, y);
        _legs.SetPosition(x, y);
        _boots.SetPosition(x, y);
        _emoteAni.SetPosition(x, y);
    }

    public RectangleF GetBoundingBox()
    {
        float width = (float)((double)_body.Texture.Width * _scale);
        float height = (float)((double)_body.Texture.Height * _scale);
        return new RectangleF((float)_body.GetPosition().X, (float)_body.GetPosition().Y, width, height);
    }

    public void Render_Debug()
    {
        Gl.glDisable(3553);
        RectangleF boundingBox = GetBoundingBox();
        Gl.glBegin(2);
        Gl.glColor3f(1f, 0f, 0f);
        Gl.glVertex2f(boundingBox.Left, boundingBox.Top);
        Gl.glVertex2f(boundingBox.Right, boundingBox.Top);
        Gl.glVertex2f(boundingBox.Right, boundingBox.Bottom);
        Gl.glVertex2f(boundingBox.Left, boundingBox.Bottom);
        Gl.glEnd();
        Gl.glEnable(3553);
    }
}
