using System;
using Engine;

namespace StonedAges;

public class Projectile
{
    private TextureManager _textureManager;

    public Texture _tex;

    public uint _fromID;

    public int _direction;

    public Vector _position;

    public int _dmg;

    public Sprite _sprite = new Sprite();

    public int _distance = 13;

    public bool _moving = true;

    public Tile _tile;

    public int _type;

    public double xconst = 28.0;

    public double yconst = 14.0;

    public double moveFrames = 3.5;

    public double _moveDelay;

    public double moveDelay = 50.0;

    public int _moveCount;

    public double _moveOffX;

    public double _moveOffY;

    public Projectile(TextureManager textureManager, uint fromID, int type, int direction, Tile tile, int dmg)
    {
        _textureManager = textureManager;
        _dmg = dmg;
        _fromID = fromID;
        _type = type;
        _tile = tile;
        _direction = direction;
        _tex = _textureManager.Get("mefc" + _type.ToString("000") + "_F" + _direction + "_new_C0", ".epf", "new");
        _sprite.Texture = _tex;
        _position = new Vector(_tile._position.X + _tile._width / 2.0 - Math.Abs(_sprite.GetWidth()) / 2.0 + 1.0, _tile._position.Y + _tile._height - _sprite.GetHeight() + 1.0, 0.0);
        _sprite.SetPosition(_position);
        _tile._map._projectiles.Add(this);
    }

    public void selfDestruct(Entity en)
    {
        if (_type == 13)
        {
            en._inventory.Add("Rock");
            if (en.gs != null && en._id == en.gs.ClientID)
            {
                en.gs.NewItem("Rock");
            }
        }
        else
        {
            en.DamageHealth(_dmg, null);
        }
        discard();
    }

    public void discard()
    {
        _tile._map._projectiles.Remove(this);
    }

    public void Update(double elapsedTime)
    {
        if (!_tile._map._loaded)
        {
            return;
        }
        _moveDelay -= elapsedTime * 1000.0;
        if (!(_moveDelay <= 0.0))
        {
            return;
        }
        _moveDelay = moveDelay;
        int num = 0;
        int num2 = 0;
        if (_direction == 0)
        {
            num2 = -1;
            _moveOffX = xconst / moveFrames;
            _moveOffY = 0.0 - yconst / moveFrames;
        }
        else if (_direction == 1)
        {
            num = 1;
            _moveOffX = xconst / moveFrames;
            _moveOffY = yconst / moveFrames;
        }
        else if (_direction == 2)
        {
            num2 = 1;
            _moveOffX = 0.0 - xconst / moveFrames;
            _moveOffY = yconst / moveFrames;
        }
        else if (_direction == 3)
        {
            num = -1;
            _moveOffX = 0.0 - xconst / moveFrames;
            _moveOffY = 0.0 - yconst / moveFrames;
        }
        if ((double)_moveCount >= moveFrames - 1.0)
        {
            int index = (_tile.Location.Y + num2) * (int)_tile._map._width + _tile.Location.X + num;
            if (_tile.Location.Y + num2 < 0 || _tile.Location.X + num < 0 || (double)(_tile.Location.Y + num2) >= _tile._map._height || (double)(_tile.Location.X + num) >= _tile._map._width)
            {
                discard();
                return;
            }
            Tile tile = _tile._map._tiles[index];
            if (tile == null)
            {
                discard();
                return;
            }
            if (tile.getTopMostNonItem() != null)
            {
                selfDestruct(tile.getTopMostNonItem());
                return;
            }
            _tile = tile;
            _moveCount = 0;
        }
        _moveCount++;
        _position.X += _moveOffX;
        _position.Y += _moveOffY;
        _sprite.SetPosition(_position);
    }

    public void Render(Renderer renderer)
    {
        renderer.DrawSprite(_sprite, 0);
    }
}
