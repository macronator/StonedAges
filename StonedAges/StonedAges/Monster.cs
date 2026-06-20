using System;
using System.Collections.Generic;
using System.Linq;
using Engine;

namespace StonedAges;

public class Monster : Entity
{
    private const int moveFrames = 14;

    private const int moveDelay = 20;

    public static Dictionary<uint, Monster> List = new Dictionary<uint, Monster>();

    public byte _projectile;

    public bool _companion;

    public double _moveDelay = 20.0;

    public int _moveOffX;

    public int _moveOffY;

    public int _moveCount;

    public string _type;

    public int _gold;

    public List<string> _drops;

    public Dictionary<string, MSkill> _skilllist;

    public Dictionary<string, MSkill> _spelllist;

    public double _deathDelay = 2000.0;

    public string _deathChant = "";

    private Pathfind _pfa;

    public uint _hostileEntityID;

    public byte _hostileType;

    public double logicTick = 1000.0;

    public Monster(GameState Gs, TextureManager textureManager, Font font, string name, Location loc, Map map, string imgString, int direction, string type, string source = "old", uint id = 0u)
        : base(Gs, textureManager, font, name, loc, map, imgString, default(Texture), id)
    {
        _entityType = EntityType.Monster;
        _type = type;
        _mBody = new MonsterBody(_textureManager, imgString, source, direction);
        if (_mBody._mImage.WalkLength > 0)
        {
            _mBody._walkConst = 280 / _mBody._mImage.WalkLength + 1;
        }
        _mBody._walkDelay = _mBody._walkConst;
        if (id != 0)
        {
            List.Add(id, this);
        }
    }

    public void Update(double elapsedTime)
    {
        if (!_tile._map._loaded)
        {
            return;
        }
        Random random = new Random();
        int num = random.Next(1, 10);
        if (_map._loaded)
        {
            _moveDelay -= elapsedTime * 1000.0;
            if (_moveDelay <= 0.0)
            {
                _moveDelay = 20.0;
                if (_moving)
                {
                    if (_moveCount >= 14)
                    {
                        _moveOffX = 0;
                        _moveOffY = 0;
                        _moveCount = 0;
                        _moving = false;
                        CenterEntity();
                    }
                    else
                    {
                        _moveCount++;
                        if (_mBody._direction == 0)
                        {
                            _moveOffX = 2;
                            _moveOffY = -1;
                        }
                        else if (_mBody._direction == 1)
                        {
                            _moveOffX = 2;
                            _moveOffY = 1;
                        }
                        else if (_mBody._direction == 2)
                        {
                            _moveOffX = -2;
                            _moveOffY = 1;
                        }
                        else if (_mBody._direction == 3)
                        {
                            _moveOffX = -2;
                            _moveOffY = -1;
                        }
                        _targetBox.X += _moveOffX;
                        _targetBox.Y += _moveOffY;
                        _hpBarSprite.SetPosition(_targetBox.X + 1, _targetBox.Y - 5);
                        _nameTag.SetPosition(_nameTag._position.X + (double)_moveOffX, _nameTag._position.Y + (double)_moveOffY);
                        _mBody._position.X += _moveOffX;
                        _mBody._position.Y += _moveOffY;
                        _mBody._sprite.SetPosition(_mBody._position.X, _mBody._position.Y);
                        _mBody._spellAni.SetPosition(_mBody._spellAni.GetPosition().X + (double)_moveOffX, _mBody._spellAni.GetPosition().Y + (double)_moveOffY);
                    }
                }
            }
        }
        if (_dead)
        {
            _pfa = null;
            if (_deathChant != "")
            {
                gs.DisplayChat(0, _id, _name + ": " + _deathChant);
                _deathChant = "";
            }
            _deathDelay -= elapsedTime * 1000.0;
            if (_deathDelay <= 0.0)
            {
                _map._entities.Remove(_id);
                _tile._entities.Remove(_id);
                if (_map._spawns.ContainsKey(_type))
                {
                    _map._spawns[_type].LastDeath = DateTime.UtcNow;
                }
                if (_exp != 0)
                {
                    gs.GainExp(_exp);
                }
                _map.SpawnItem("Gold", _location, _gold);
                foreach (string drop in _drops)
                {
                    _map.SpawnItem(drop, _location);
                }
            }
        }
        if (_spellBar.Count > 0)
        {
            _spellBar.Values.ToList().ForEach(delegate (SpellBar s)
            {
                s.Update(elapsedTime);
            });
        }
        if (!base.hassleep && !_dead && !base.hasslumber && !base.hasparalyze && !base.hasblind && _mBody._action == 0 && !_moving)
        {
            if (_mBody._stay == 1)
            {
                if (!_hostile)
                {
                }
            }
            else
            {
                if (_hostile)
                {
                    Entity entity = null;
                    foreach (Entity item in _map._entities.Values.OrderBy((Entity z) => z._location.DistanceFrom(_location)))
                    {
                        if (item is Player)
                        {
                            entity = item;
                            break;
                        }
                    }
                    if (entity != null)
                    {
                        _hostileEntityID = entity._id;
                        if (entity._dead || entity._location.DistanceFrom(_location) >= 13)
                        {
                            _pfa = null;
                            _hostile = false;
                        }
                        if (_location.DistanceFrom(entity._location) <= 1)
                        {
                            _pfa = null;
                            if (!_location.IsInFront(entity._location, (D)_mBody._direction))
                            {
                                if (entity._location.Y == _location.Y - 1)
                                {
                                    _mBody.ChangeDirection(0);
                                }
                                if (entity._location.Y == _location.Y + 1)
                                {
                                    _mBody.ChangeDirection(2);
                                }
                                if (entity._location.X == _location.X + 1)
                                {
                                    _mBody.ChangeDirection(1);
                                }
                                if (entity._location.X == _location.X - 1)
                                {
                                    _mBody.ChangeDirection(3);
                                }
                            }
                        }
                        else
                        {
                            if (_pfa == null || _pfa._thePath.Count() == 0)
                            {
                                _pfa = new Pathfind(_location.X, _location.Y, entity._location.X, entity._location.Y, _map);
                            }
                            if (_pfa != null)
                            {
                                if (_pfa._thePath.Count() > 0)
                                {
                                    Tile tile = _pfa._thePath.Last();
                                    if (tile.Location.Y == _location.Y + 1)
                                    {
                                        _mBody.ChangeDirection(2);
                                        MoveATile();
                                    }
                                    else if (tile.Location.Y == _location.Y - 1)
                                    {
                                        _mBody.ChangeDirection(0);
                                        MoveATile();
                                    }
                                    else if (tile.Location.X == _location.X + 1)
                                    {
                                        _mBody.ChangeDirection(1);
                                        MoveATile();
                                    }
                                    else if (tile.Location.X == _location.X - 1)
                                    {
                                        _mBody.ChangeDirection(3);
                                        MoveATile();
                                    }
                                    _pfa._thePath.Remove(tile);
                                }
                                else
                                {
                                    _pfa = null;
                                }
                            }
                        }
                        if (_location.IsInFront(entity._location, (D)_mBody._direction))
                        {
                            _mBody.Attack();
                            int dmg = (int)((double)(int)_str * 0.8 * 6.0) + _dmg * 5;
                            entity.DamageHealth(dmg, this);
                        }
                    }
                    else
                    {
                        _pfa = null;
                    }
                }
                else
                {
                    if (_hostileEntityID != 0)
                    {
                        _hostileEntityID = 0u;
                    }
                    _pfa = null;
                    if (_projectile > 0)
                    {
                        if (MoveATile())
                        {
                            _projectile--;
                            if (_projectile == 0)
                            {
                                _deathDelay = 0.0;
                                _dead = true;
                            }
                        }
                    }
                    else
                    {
                        num = random.Next(0, 100);
                        if (num <= 70)
                        {
                            MoveATile();
                        }
                        else
                        {
                            for (num = _mBody._direction; num == _mBody._direction; num = random.Next(0, 4))
                            {
                            }
                            _mBody.ChangeDirection(num);
                        }
                    }
                }
                _mBody._idleCount = 0;
            }
        }
        if (_mBody._action == 0 && !_hostile)
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
        UpdateEntityChat(elapsedTime);
        _mBody.Update(elapsedTime);
        UpdateHealthBar(elapsedTime);
    }

    public void ResetHostile()
    {
        if (_hostileType == 1)
        {
            _hostile = true;
        }
        else if (_hostileType == 0)
        {
            _hostile = false;
        }
    }

    public void Render(Renderer renderer)
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
}
