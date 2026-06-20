using System;
using System.Collections.Generic;
using System.Linq;
using Engine;

namespace StonedAges;

public class NPC : Entity
{
    public static Dictionary<uint, NPC> List = new Dictionary<uint, NPC>();

    public string _source;

    public NPC(GameState Gs, TextureManager textureManager, Font font, string name, Location loc, Map map, string imgString, int direction, string source = "old", uint id = 0u)
        : base(Gs, textureManager, font, name, loc, map, imgString, default(Texture), id)
    {
        _entityType = EntityType.Npc;
        _source = source;
        base._direction = base._direction;
        _nameTag.SetColor(Color.LightBlue);
        _mBody = new MonsterBody(_textureManager, imgString, source, direction);
        if (id != 0)
        {
            List.Add(id, this);
        }
    }

    public void Update(double elapsedTime)
    {
        Random random = new Random();
        int num = random.Next(1, 10);
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
            if (_inventory.Contains("Rock"))
            {
                foreach (Entity item in _map._entities.Values.OrderBy((Entity z) => z._location.DistanceFrom(_location)))
                {
                    if (item._mapNum == _mapNum && item is Player && _location.InLine(item._location, (D)_mBody._direction, 13))
                    {
                        new Projectile(_textureManager, _id, 13, _mBody._direction, _tile, 0);
                        if (GameWindow.ConnectedToServer)
                        {
                            ProjectilePacket projectilePacket = new ProjectilePacket(_id, 13);
                            GameWindow.ClientSocket.Send(projectilePacket.Data);
                        }
                        _inventory.Remove("Rock");
                        break;
                    }
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
        UpdateEntityChat(elapsedTime);
        _mBody.Update(elapsedTime);
        UpdateHealthBar(elapsedTime);
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
