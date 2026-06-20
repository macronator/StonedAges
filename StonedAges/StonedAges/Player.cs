using System;
using System.Collections.Generic;
using System.Linq;
using Engine;

namespace StonedAges;

public class Player : Entity
{
    private const double _orighpRegenDelay = 10000.0;

    private const double _origmpRegenDelay = 10000.0;

    public static Dictionary<uint, Player> List = new Dictionary<uint, Player>();

    public Dictionary<int, uint> TNIS = new Dictionary<int, uint>();

    public bool _master;

    public string _rank = "";

    public string _title = "";

    public string _guild = "";

    public byte _nation;

    public bool _allowGrouping = true;

    public byte _availstats;

    public List<LegendMark> _legendMarks = new List<LegendMark>();

    public List<Equipment> _equipment = new List<Equipment>();

    public bool _inMonsterForm;

    public int _monsterForm;

    private double _hpRegenDelay = 10000.0;

    private double _mpRegenDelay = 10000.0;

    public Player(GameState Gs, TextureManager textureManager, Font font, string name, Location loc, Map map, string imgString, int gender, uint id = 0u)
        : base(Gs, textureManager, font, name, loc, map, imgString, default(Texture), id)
    {
        PopulateTNIS();
        _gender = gender;
        _body = new PlayerBody(textureManager, _gender);
        if (id != 0)
        {
            List.Add(id, this);
        }
    }

    private void PopulateTNIS()
    {
        TNIS.Clear();
        uint num = 600u;
        uint num2 = 0u;
        for (int i = 0; i <= 99; i++)
        {
            uint num3 = (uint)((int)num + (int)(i * (num * 2))) + num2;
            TNIS.Add(i, num3);
            num2 = num3;
        }
    }

    public void AnimateMovement(int feetShuffleDelay)
    {
        if (_inMonsterForm)
        {
            _mBody.Walk();
        }
        else if (!_body._walking)
        {
            _body.SetAni("01", feetShuffleDelay, 0, 10);
            _body._walking = true;
        }
    }

    public void MonsterForm(int form, string source = "old")
    {
        if (!_textureManager.badNewMPFS().Contains(form))
        {
            if (form != 1 || !_inMonsterForm)
            {
                _inMonsterForm = true;
                _monsterForm = form;
                _mBody = new MonsterBody(_textureManager, _monsterForm.ToString("000"), source, _body._direction);
                _mBody._idleCount = 0;
                CenterEntity();
                _mBody.flipSprites();
            }
            else
            {
                _inMonsterForm = false;
            }
        }
    }

    public void Update(double elapsedTime)
    {
        Random random = new Random();
        int num = random.Next(1, 10);
        UpdateEntityChat(elapsedTime);
        if (_dead)
        {
            if (!_body._spellanimating)
            {
                gs.SpellAnimation(this, 24, 120);
            }
        }
        else if (!base.Ghost)
        {
            _hpRegenDelay -= elapsedTime * 1000.0;
            if (_hpRegenDelay <= 0.0)
            {
                _hpRegenDelay = 10000.0;
                if (_curHP != base._maxHP)
                {
                    int num2 = base._maxHP / 10;
                    if (_curHP + num2 <= base._maxHP)
                    {
                        _curHP += num2;
                        if (_curHP <= 0)
                        {
                            _curHP = 1;
                        }
                    }
                    else
                    {
                        _curHP = base._maxHP;
                    }
                }
            }
            _mpRegenDelay -= elapsedTime * 1000.0;
            if (_mpRegenDelay <= 0.0)
            {
                _mpRegenDelay = 10000.0;
                if (_curMP != base._maxMP)
                {
                    int num3 = base._maxMP / 10;
                    if (_curMP + num3 <= base._maxMP)
                    {
                        _curMP += num3;
                        if (_curMP <= 0)
                        {
                            _curMP = 1;
                        }
                    }
                    else
                    {
                        _curMP = base._maxMP;
                    }
                }
            }
        }
        if (_inMonsterForm)
        {
            if (_mBody._action == 0)
            {
                num = random.Next(0, 100);
                if (num <= 80)
                {
                    _mBody._action = 3;
                }
                else
                {
                    _mBody._action = 4;
                }
                _mBody._idleCount = 0;
            }
            _mBody.Update(elapsedTime);
        }
        _body.Update(elapsedTime);
        UpdateHealthBar(elapsedTime);
    }

    public void Render(Renderer renderer)
    {
        if (_inMonsterForm)
        {
            _mBody.Render(renderer, 0);
            if (_targeted)
            {
                _mBody.Render(renderer, 1);
            }
            else if (base.hasbeagcradh || base.hascradh || base.hasmorcradh || base.hasardcradh || base.hasdiacradh)
            {
                _mBody.Render(renderer, 2);
            }
        }
        else
        {
            _body.Render(renderer, _tile._water, _hidden, ghosted: false, seethru: false, 0);
            if (_targeted)
            {
                _body.Render(renderer, _tile._water, _hidden, ghosted: false, seethru: false, 1);
            }
            else if (base.hasbeagcradh || base.hascradh || base.hasmorcradh || base.hasardcradh || base.hasdiacradh)
            {
                _body.Render(renderer, _tile._water, _hidden, ghosted: false, seethru: false, 2);
            }
        }
    }

    public void RenderSeeThru(Renderer renderer)
    {
        if (_inMonsterForm)
        {
            renderer.DrawSprite(_mBody._sprite, 0);
        }
        else
        {
            _body.Render(renderer, _tile._water, _hidden, ghosted: false, seethru: true, 0);
        }
    }
}
