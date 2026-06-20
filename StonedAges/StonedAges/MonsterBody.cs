using System;
using System.Collections.Generic;
using System.Linq;
using Engine;

namespace StonedAges;

public class MonsterBody
{
    public const double _idleConst = 240.0;

    public bool _flip;

    private Texture _blankTexture = default(Texture);

    public Sprite _spellAni = new Sprite();

    public int _maxSpellWidth;

    public int _maxSpellHeight;

    public bool _spellanimating;

    private double _origSpellAniDelay;

    private int _spellAniCount = -1;

    private double _spellAniDelay;

    private int _spellFrameCount;

    public int _spellAniType;

    private List<Texture> _spellAniArray = new List<Texture>();

    private TextureManager _textureManager;

    public Sprite _sprite = new Sprite();

    public Vector _position = default(Vector);

    public MPFImage _mImage;

    public int _face;

    public int _direction = 1;

    public Texture _curFrame;

    public List<Texture> _imgArr;

    public int _action = 3;

    public int _nextAction;

    public int _stay;

    public bool _hostile;

    public bool _idle = true;

    public double _delay;

    private List<Texture> _idleArr = new List<Texture>();

    public int _idleCount;

    public string _imgString;

    public double _idleDelay = 240.0;

    public string _source = "old";

    public double _walkConst = 85.0;

    public double _walkDelay = 85.0;

    public MonsterBody(TextureManager textureManager, string imgString, string source, int direction = 1)
    {
        _imgString = imgString;
        _textureManager = textureManager;
        List<Texture> list = new List<Texture>();
        int num = 1;
        MPFImage mPFImage = null;
        int num2 = int.Parse(imgString);
        if (num2 == 83 && source != "myda")
        {
            imgString = "083A";
        }
        if (num2 <= 328 && !textureManager.badMPFS().Contains(num2) && source == "old")
        {
            mPFImage = MPFImage.FromArchive("MNS" + imgString + ".mpf", ignoreCase: true, _textureManager.Dat("oldDat"));
            switch (num2)
            {
                case 83:
                    mPFImage.Attack1Start = 1;
                    mPFImage.Attack1Length = 3;
                    break;
                case 21:
                    mPFImage.IdleLength = 2;
                    break;
                case 22:
                    mPFImage.IdleLength = 10;
                    mPFImage.UnknownB = 4;
                    break;
                case 57:
                    mPFImage.IdleLength = 0;
                    break;
                case 94:
                    mPFImage.IdleLength = 1;
                    break;
                case 191:
                    mPFImage.Attack1Start = 0;
                    mPFImage.Attack1Length = 2;
                    break;
                case 195:
                    mPFImage.IdleLength = 2;
                    break;
                case 202:
                    mPFImage.WalkLength = 5;
                    mPFImage.Attack1Start = 10;
                    mPFImage.Attack1Length = 2;
                    mPFImage.Attack2Start = 12;
                    mPFImage.Attack2Length = 2;
                    break;
                case 204:
                case 241:
                    mPFImage.WalkLength = 5;
                    mPFImage.Attack1Start = 10;
                    mPFImage.Attack1Length = 2;
                    break;
                case 205:
                    mPFImage.IdleLength = 3;
                    break;
                case 234:
                    mPFImage.IdleLength = 1;
                    break;
            }
        }
        else if (source == "myda" && !textureManager.badMydaMPFS().Contains(num2))
        {
            mPFImage = MPFImage.FromArchive("MNS" + imgString + ".mpf", ignoreCase: true, _textureManager.Dat("mydaDat"));
        }
        else
        {
            mPFImage = MPFImage.FromArchive("MNS" + imgString + ".mpf", ignoreCase: true, _textureManager.Dat("hadesDat"));
            if (num2 == 83)
            {
                mPFImage.Attack1Start = 1;
                mPFImage.Attack1Length = 3;
            }
            else if (!textureManager.badNewMPFS().Contains(num2))
            {
                switch (num2)
                {
                    case 191:
                        mPFImage.Attack1Start = 0;
                        mPFImage.Attack1Length = 3;
                        break;
                    case 202:
                    case 724:
                        mPFImage.WalkLength = 5;
                        mPFImage.Attack1Length = 2;
                        mPFImage.Attack1Start = 10;
                        mPFImage.Attack2Length = 2;
                        mPFImage.Attack2Start = 12;
                        break;
                    case 241:
                        mPFImage.WalkLength = 5;
                        mPFImage.Attack1Length = 2;
                        mPFImage.Attack1Start = 10;
                        break;
                    case 380:
                        mPFImage.Attack1Length = 2;
                        mPFImage.Attack2Length = 3;
                        mPFImage.Attack2Start = 18;
                        break;
                    case 561:
                    case 562:
                        mPFImage.IdleStart = 4;
                        break;
                    default:
                        switch (num2)
                        {
                            case 561:
                            case 562:
                                mPFImage.IdleLength = 0;
                                break;
                            case 757:
                            case 758:
                                mPFImage.IdleLength = 2;
                                mPFImage.WalkLength = 2;
                                mPFImage.Attack1Length = 6;
                                mPFImage.Attack1Start = 4;
                                mPFImage.Attack2Start = 0;
                                mPFImage.Attack2Length = 0;
                                break;
                            case 785:
                                mPFImage.Attack1Start = 10;
                                mPFImage.Attack2Start = 10;
                                mPFImage.Attack3Start = 10;
                                break;
                            case 832:
                                mPFImage.IdleLength = 0;
                                mPFImage.Idle2Length = 0;
                                break;
                        }
                        break;
                }
            }
        }
        if (mPFImage == null)
        {
            mPFImage = MPFImage.FromArchive("MNS492.mpf", ignoreCase: true, _textureManager.Dat("hadesDat"));
        }
        num = mPFImage.WalkLength + 1;
        if (mPFImage.WalkStart != 0 && mPFImage.IdleStart == 0 && mPFImage.IdleLength != 0)
        {
            num = mPFImage.IdleLength + 1;
        }
        if (mPFImage.WalkStart == 0 && mPFImage.WalkLength == 0 && mPFImage.IdleStart == 0 && mPFImage.IdleLength != 0)
        {
            num = 1;
        }
        if (mPFImage.ExpectedFrames == 1 || num > mPFImage.ExpectedFrames)
        {
            num = 1;
        }
        if (mPFImage.WalkStart == 0 && mPFImage.WalkLength == 0 && mPFImage.IdleStart == 0 && mPFImage.IdleLength * 2 == mPFImage.ExpectedFrames)
        {
            num = mPFImage.IdleLength + 1;
        }
        _source = source;
        switch (source)
        {
            case "old":
                {
                    for (int j = 0; j < mPFImage.ExpectedFrames; j++)
                    {
                        list.Add(_textureManager.Get("MNS" + imgString + "_F" + j + "_C0", ".mpf", "old", mPFImage));
                    }
                    break;
                }
            case "myda":
                {
                    for (int k = 0; k < mPFImage.ExpectedFrames; k++)
                    {
                        list.Add(_textureManager.Get("MNS" + imgString + "_F" + k + "_myda_C0", ".mpf", "myda", mPFImage));
                    }
                    break;
                }
            case "new":
                {
                    for (int i = 0; i < mPFImage.ExpectedFrames; i++)
                    {
                        list.Add(_textureManager.Get("MNS" + imgString + "_F" + i + "_new_C0", ".mpf", "new", mPFImage));
                    }
                    break;
                }
        }
        if (num2 == 83 && source != "myda")
        {
            MPFImage mPFImage2 = MPFImage.FromArchive("MNS" + num2.ToString("000") + "B.mpf", ignoreCase: true, _textureManager.Dat("oldDat"));
            for (int l = 0; l < mPFImage2.ExpectedFrames; l++)
            {
                list.Add(_textureManager.Get("MNS" + num2.ToString("000") + "B_F" + l + "_C0", ".mpf", "old", mPFImage2));
            }
            mPFImage2 = MPFImage.FromArchive("MNS" + num2.ToString("000") + "C.mpf", ignoreCase: true, _textureManager.Dat("oldDat"));
            for (int m = 0; m < mPFImage2.ExpectedFrames; m++)
            {
                list.Add(_textureManager.Get("MNS" + num2.ToString("000") + "C_F" + m + "_C0", ".mpf", "old", mPFImage2));
            }
        }
        if ((source == "old" || source == "new") && _textureManager.oldFlips().Contains(num2))
        {
            _flip = true;
        }
        if (source == "myda" && _textureManager.mydaFlips().Contains(num2))
        {
            _flip = true;
        }
        _mImage = mPFImage;
        _direction = direction;
        _face = num;
        _imgArr = list;
        SetDefault();
    }

    public void SpellAni(int spellType, int speed)
    {
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
            if (spellType != 62 || item2 != 5)
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
                continue;
            }
            break;
        }
        double x = Math.Abs(_sprite.GetWidth() / 2.0) + _position.X - (double)(_maxSpellWidth / 2);
        double y = _position.Y + _sprite.GetHeight() - (double)_maxSpellHeight;
        _spellAni.SetPosition(x, y);
        _spellanimating = true;
    }

    public void flipSprites()
    {
        int num = 0;
        int num2 = 0;
        int num3 = int.Parse(_imgString);
        if (num3 == 2 || num3 == 143 || num3 == 144 || num3 == 145 || num3 == 146 || num3 == 147 || num3 == 148 || num3 == 149 || num3 == 150 || num3 == 171 || num3 == 178 || num3 == 182 || num3 == 186 || num3 == 237)
        {
            num2 = 7;
        }
        else if (num3 == 11)
        {
            if (_source == "new")
            {
                if (_direction == 0 || _direction == 3)
                {
                    num2 = 4;
                    num = 12;
                }
                else
                {
                    num2 = 6;
                    num = 10;
                }
            }
            else
            {
                num = 14;
            }
        }
        else if (num3 == 12 || num3 == 266)
        {
            num2 = 21;
        }
        else if (num3 == 13)
        {
            if (_source == "new")
            {
                num2 = 1;
                if (_direction == 1)
                {
                    num = -2;
                }
                else if (_direction == 2)
                {
                    num = 1;
                }
                else if (_direction == 0)
                {
                    num = -6;
                }
                else if (_direction == 3)
                {
                    num = -4;
                }
            }
            else
            {
                num = -14;
            }
        }
        else if (num3 == 83)
        {
            num2 = 27;
            num = -1;
        }
        else if (num3 == 18 || num3 == 48 || num3 == 151 || num3 == 189 || num3 == 196 || num3 == 202 || num3 == 209)
        {
            num2 = 14;
        }
        else if (num3 == 192 || num3 == 193 || num3 == 194 || num3 == 206 || num3 == 207)
        {
            num2 = ((_direction != 0 && _direction != 3) ? 14 : 28);
        }
        else if (num3 == 156 || num3 == 157)
        {
            num2 = 18;
        }
        else if (num3 == 174 || num3 == 191)
        {
            num2 = 28;
        }
        else if (num3 == 185)
        {
            num = 18;
        }
        else if (num3 == 195 || num3 == 198 || num3 == 210)
        {
            if (_direction == 0 || _direction == 3)
            {
                num2 = 21;
            }
            else
            {
                num = 18;
                num2 = 25;
            }
        }
        else if (num3 == 188)
        {
            if (_direction == 0 || _direction == 3)
            {
                num2 = 5;
                num = 8;
            }
            else
            {
                num = 18;
            }
        }
        else if (num3 == 203)
        {
            num = 10;
        }
        else if (num3 == 204 || num3 == 208)
        {
            if (_direction == 0 || _direction == 3)
            {
                num2 = 18;
                num = 8;
            }
            else
            {
                num2 = 21;
                num = 16;
            }
        }
        else if ((num3 >= 212 && num3 <= 219) || num3 == 236)
        {
            num2 = 4;
        }
        else
        {
            switch (num3)
            {
                case 220:
                    num2 = -4;
                    num = 7;
                    break;
                case 238:
                case 400:
                    num2 = 2;
                    num = 8;
                    break;
                case 245:
                    num2 = 56;
                    break;
                case 254:
                    num2 = 28;
                    break;
                case 257:
                case 258:
                case 259:
                case 260:
                case 261:
                case 262:
                    if (_direction == 0 || _direction == 3)
                    {
                        num2 = -1;
                        num = 4;
                    }
                    else
                    {
                        num2 = 2;
                        num = 8;
                    }
                    break;
                default:
                    switch (num3)
                    {
                        case 263:
                            num2 = 10;
                            break;
                        case 264:
                            num2 = ((_direction != 0 && _direction != 3) ? 12 : 8);
                            num = -3;
                            break;
                        case 394:
                            if (_direction == 0 || _direction == 3)
                            {
                                num = 10;
                                num2 = 12;
                            }
                            else
                            {
                                num = 18;
                                num2 = 18;
                            }
                            break;
                        case 401:
                            num = 18;
                            num2 = ((_direction != 0 && _direction != 3) ? 18 : 7);
                            break;
                        case 705:
                            num = -14;
                            num2 = ((_direction != 0 && _direction != 3) ? 14 : 2);
                            break;
                    }
                    break;
            }
        }
        if (_direction == 0 || _direction == 1)
        {
            if (_flip)
            {
                _sprite._flip = true;
            }
            else
            {
                _sprite._flip = false;
            }
            _sprite.SetPosition(_position.X + (double)num, _position.Y + (double)num2);
        }
        else
        {
            if (_flip)
            {
                _sprite._flip = false;
            }
            else
            {
                _sprite._flip = true;
            }
            _sprite.SetPosition(_position.X - (double)num, _position.Y + (double)num2);
        }
    }

    public void SetDefault()
    {
        flipSprites();
        if (_direction == 0 || _direction == 3)
        {
            _sprite.Texture = _imgArr[0];
        }
        else
        {
            _sprite.Texture = _imgArr[_face - 1];
        }
    }

    private void SetTexture(double elapsedTime)
    {
        flipSprites();
        if (_action == 2)
        {
            _walkDelay -= elapsedTime * 1000.0;
            if (_walkDelay <= 0.0)
            {
                _walkDelay = _walkConst;
                if (_mImage.WalkLength > 0)
                {
                    if (_direction == 0 || _direction == 3)
                    {
                        BuildArray(_idleArr, _mImage.WalkStart, _mImage.WalkStart + _mImage.WalkLength);
                        _idleArr.Add(_imgArr[0]);
                    }
                    else
                    {
                        BuildArray(_idleArr, _mImage.WalkStart + _mImage.WalkLength, _mImage.WalkStart + _mImage.WalkLength * 2);
                        _idleArr.Add(_imgArr[_face - 1]);
                    }
                    _curFrame = _idleArr[_idleCount];
                    if (_idleCount >= _idleArr.Count - 1)
                    {
                        ResetAction();
                    }
                    else
                    {
                        _idleCount++;
                    }
                    _sprite.Texture = _curFrame;
                }
                else
                {
                    ResetAction();
                }
            }
        }
        _delay -= elapsedTime * 1000.0;
        if (!(_delay <= 0.0))
        {
            return;
        }
        _delay = _idleDelay;
        if (_action == 6)
        {
            if (_mImage.Attack1Length > 0)
            {
                if (_direction == 0 || _direction == 3)
                {
                    if (_imgString == "380")
                    {
                        BuildArray(_idleArr, _mImage.Attack1Start, _mImage.Attack1Start + 2);
                    }
                    else
                    {
                        BuildArray(_idleArr, _mImage.Attack1Start, _mImage.Attack1Start + _mImage.Attack1Length);
                    }
                    _idleArr.Add(_imgArr[0]);
                }
                else if (_face == 1)
                {
                    BuildArray(_idleArr, _mImage.Attack1Start, _mImage.Attack1Start + _mImage.Attack1Length);
                    _idleArr.Add(_imgArr[_face - 1]);
                }
                else
                {
                    if (_mImage.Name == "MNS202.mpf" || _mImage.Name == "MNS724.mpf" || _mImage.Name == "MNS774.mpf" || _mImage.Name == "MNS775.mpf")
                    {
                        BuildArray(_idleArr, _mImage.Attack1Start + _mImage.Attack1Length * 2, _mImage.Attack1Start + _mImage.Attack1Length * 2 + _mImage.Attack1Length);
                    }
                    else if (_mImage.Name == "MNS380.mpf")
                    {
                        BuildArray(_idleArr, _mImage.Attack1Start + 5, _mImage.Attack1Start + 8);
                    }
                    else
                    {
                        BuildArray(_idleArr, _mImage.Attack1Start + _mImage.Attack1Length, _mImage.Attack1Start + _mImage.Attack1Length * 2);
                    }
                    if (_mImage.Name == "MNS705.mpf")
                    {
                        BuildArray(_idleArr, 33, 35, clear: false);
                    }
                    else if (_mImage.Name == "MNS693.mpf")
                    {
                        BuildArray(_idleArr, 38, 42, clear: false);
                    }
                    _idleArr.Add(_imgArr[_face - 1]);
                }
                _curFrame = _idleArr[_idleCount];
                if (_idleCount == _idleArr.Count - 1)
                {
                    ResetAction();
                }
                else
                {
                    _idleCount++;
                }
                _sprite.Texture = _curFrame;
            }
            else
            {
                ResetAction();
            }
        }
        if (_action == 5)
        {
            if (_mImage.Attack2Length > 0 && _mImage.Attack2Start != _mImage.Attack1Start)
            {
                if (_direction == 0 || _direction == 3)
                {
                    if (_imgString == "380")
                    {
                        BuildArray(_idleArr, _mImage.Attack2Start, _mImage.Attack2Start + 3);
                    }
                    else
                    {
                        BuildArray(_idleArr, _mImage.Attack2Start, _mImage.Attack2Start + _mImage.Attack2Length);
                    }
                    _idleArr.Add(_imgArr[0]);
                }
                else if (_face == 1)
                {
                    BuildArray(_idleArr, _mImage.Attack2Start, _mImage.Attack2Start + _mImage.Attack2Length);
                }
                else
                {
                    if (_mImage.Name == "MNS202.mpf" || _mImage.Name == "MNS724.mpf" || _mImage.Name == "MNS774.mpf" || _mImage.Name == "MNS775.mpf")
                    {
                        BuildArray(_idleArr, _mImage.Attack2Start + _mImage.Attack2Length * 2, _mImage.Attack2Start + _mImage.Attack2Length * 2 + _mImage.Attack2Length);
                    }
                    else if (_mImage.Name == "MNS380.mpf")
                    {
                        BuildArray(_idleArr, _mImage.Attack2Start + 5, _mImage.Attack2Start + 12);
                    }
                    else
                    {
                        BuildArray(_idleArr, _mImage.Attack2Start + _mImage.Attack2Length, _mImage.Attack2Start + _mImage.Attack2Length * 2);
                    }
                    if (_mImage.Name == "MNS705.mpf")
                    {
                        BuildArray(_idleArr, 33, 35, clear: false);
                    }
                    else if (_mImage.Name == "MNS693.mpf")
                    {
                        BuildArray(_idleArr, 38, 42, clear: false);
                    }
                    _idleArr.Add(_imgArr[_face - 1]);
                }
                _curFrame = _idleArr[_idleCount];
                if (_idleCount == _idleArr.Count - 1)
                {
                    ResetAction();
                }
                else
                {
                    _idleCount++;
                }
                _sprite.Texture = _curFrame;
            }
            else
            {
                ResetAction();
            }
        }
        if (_action == 3)
        {
            if (_mImage.IdleLength > 0)
            {
                if (_direction == 0 || _direction == 3)
                {
                    BuildArray(_idleArr, _mImage.IdleStart, _mImage.IdleStart + _mImage.IdleLength);
                }
                else if (_face == 1)
                {
                    BuildArray(_idleArr, _mImage.IdleStart, _mImage.IdleStart + _mImage.IdleLength);
                }
                else if (_mImage.IdleLength != 1 && _mImage.ExpectedFrames > _mImage.IdleLength * 2)
                {
                    BuildArray(_idleArr, _mImage.IdleStart + _mImage.IdleLength, _mImage.IdleStart + _mImage.IdleLength + _mImage.IdleLength);
                }
                else
                {
                    BuildArray(_idleArr, _face - 1, _face - 1 + _mImage.IdleLength);
                }
                _curFrame = _idleArr[_idleCount];
                if (_idleCount == _idleArr.Count - 1)
                {
                    ResetAction();
                }
                else
                {
                    _idleCount++;
                }
                _sprite.Texture = _curFrame;
            }
            else
            {
                ResetAction();
            }
        }
        if (_action != 4)
        {
            return;
        }
        if (_mImage.Idle2Length > 0 && _mImage.Idle2Length != _mImage.IdleLength)
        {
            if (_direction == 0 || _direction == 3)
            {
                BuildArray(_idleArr, _mImage.IdleStart, _mImage.IdleStart + _mImage.Idle2Length);
            }
            else if (_face == 1)
            {
                BuildArray(_idleArr, _mImage.IdleStart, _mImage.IdleStart + _mImage.Idle2Length);
            }
            else
            {
                BuildArray(_idleArr, _face - 1, _face - 1 + _mImage.Idle2Length);
            }
            _curFrame = _idleArr[_idleCount];
            if (_idleCount == _idleArr.Count - 1)
            {
                ResetAction();
            }
            else
            {
                _idleCount++;
            }
            _sprite.Texture = _curFrame;
        }
        else
        {
            ResetAction();
        }
    }

    public void Update(double elapsedTime)
    {
        SetTexture(elapsedTime);
        if (_spellanimating)
        {
            _spellAniDelay -= elapsedTime * 1000.0;
            if (_spellAniDelay <= 0.0)
            {
                _spellAniDelay = _origSpellAniDelay;
                if (_spellAniCount >= _spellFrameCount)
                {
                    _spellAniCount = -1;
                }
                if (_spellAniCount == -1)
                {
                    _spellAniCount++;
                    _spellAni.Texture = _blankTexture;
                    _spellanimating = false;
                }
                else
                {
                    _spellAni.Texture = _spellAniArray[_spellAniCount];
                    _spellAniCount++;
                }
            }
        }
        else
        {
            _spellAniCount = 0;
            _spellAni.Texture = _blankTexture;
        }
    }

    public void Render(Renderer renderer, byte colorize = 0)
    {
        renderer.DrawSprite(_sprite, colorize);
        renderer.DrawSprite(_spellAni, 0);
    }

    public void ResetAction()
    {
        _idleCount = 0;
        _nextAction = 0;
        _action = _nextAction;
    }

    public void ChangeDirection(int direction)
    {
        _direction = direction;
        SetDefault();
    }

    public void Walk()
    {
        _nextAction = 2;
        if (_action != 2 && _action != 5 && _action != 6)
        {
            _action = 2;
            _idleCount = 0;
        }
    }

    public void Attack2()
    {
        _nextAction = 5;
        if (_action != 2 && _action != 5 && _action != 6)
        {
            _action = 5;
            _idleCount = 0;
        }
    }

    public void Attack()
    {
        _nextAction = 6;
        if (_action != 2 && _action != 5 && _action != 6)
        {
            _action = 6;
            _idleCount = 0;
        }
    }

    private void BuildArray(List<Texture> thelist, int start, int end, bool clear = true)
    {
        if (clear)
        {
            thelist.Clear();
        }
        int num = 0;
        foreach (Texture item in _imgArr)
        {
            if (num >= start && num < end)
            {
                thelist.Add(item);
            }
            num++;
        }
    }
}
