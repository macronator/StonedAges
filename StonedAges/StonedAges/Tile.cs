using System.Collections.Generic;
using System.Drawing;
using Engine;
using Tao.OpenGl;

namespace StonedAges;

public class Tile
{
    private const double _origWallIdleSpeed = 240.0;

    public Map _map;

    public Dictionary<uint, Entity> _entities = new Dictionary<uint, Entity>();

    private Texture _blankTexture = default(Texture);

    public double _scale = 1.0;

    private TextureManager _textureManager;

    private EffectTable effectTable;

    public Vector _position = default(Vector);

    public Sprite _sprite = new Sprite();

    public Location Location;

    public double _width = 56.0;

    public double _height = 27.0;

    public bool _walkable;

    public bool _water;

    public bool _highlight;

    public int _clicked;

    public Wall _wall;

    public int _floor;

    public bool _rendered;

    public IWO _iwo;

    public int _H;

    public int _G;

    public int _F;

    public Tile _parent;

    public Rectangle _targetBox = new Rectangle(0, 0, 27, 27);

    public Sprite _highlightSprite = new Sprite();

    public Sprite _spellAni = new Sprite();

    public bool _spellanimating;

    private double _origSpellAniDelay;

    private int _spellAniCount = -1;

    private double _spellAniDelay;

    private int _spellFrameCount;

    public int _spellAniType;

    private List<Texture> _spellAniArray = new List<Texture>();

    public bool _spellAniRepeat;

    private int _spellAniWidth;

    private int _spellAniHeight;

    private double _wallIdleSpeed = 240.0;

    private int _wallIdleCount;

    public double Index => (double)Location.Y * _map._width + (double)Location.X;

    public Tile(Map map, TextureManager textureManager, int floor, int x, int y, double rectx, double recty, Texture texture, bool walkable, Wall newwall = null)
    {
        _map = map;
        _floor = floor;
        _walkable = walkable;
        _position.X = rectx;
        _position.Y = recty;
        _textureManager = textureManager;
        effectTable = _textureManager.EffectTable();
        Location = new Location(x, y);
        _sprite.Texture = texture;
        _wall = newwall;
        _sprite.SetPosition(_position.X, _position.Y);
        _targetBox.X = (int)(_position.X + _width / 2.0 - (double)(_targetBox.Width / 2));
        _targetBox.Y = (int)(_position.Y + _height - (double)_targetBox.Height - 2.0);
        _highlightSprite.Texture = _textureManager.Get("tilehighlight");
        _highlightSprite.SetPosition(_position.X + 11.0, _position.Y + 5.0);
    }

    public Entity getTopMostNonItem()
    {
        Entity result = null;
        foreach (Entity value in _entities.Values)
        {
            if (!(value is Item))
            {
                return value;
            }
        }
        return result;
    }

    public void Update(double elapsedTime)
    {
        if (_spellanimating && _walkable)
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
                if (!_spellAniRepeat)
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

    public void Render(Renderer renderer, double elapsedTime)
    {
        renderer.DrawSprite(_sprite, 0);
        if (_wall == null)
        {
            return;
        }
        _wallIdleSpeed -= elapsedTime * 1000.0;
        if (!(_wallIdleSpeed <= 0.0))
        {
            return;
        }
        _wallIdleSpeed = _wall._idleSpeed * 50;
        if (_wall._lIdleArr.Count <= 0 && _wall._rIdleArr.Count <= 0)
        {
            return;
        }
        if (_wall._lIdleArr.Count > 0)
        {
            int lwall = _wall._lIdleArr[_wallIdleCount];
            _wall._lwall = lwall;
            _wall._leftWall.Texture = _textureManager.Get("stc" + lwall.ToString("00000") + ".hpf_F0_C0", ".hpf");
        }
        if (_wall._rIdleArr.Count > 0)
        {
            int rwall = _wall._rIdleArr[_wallIdleCount];
            _wall._rwall = rwall;
            _wall._rightWall.Texture = _textureManager.Get("stc" + rwall.ToString("00000") + ".hpf_F0_C0", ".hpf");
        }
        if (_wall._lIdleArr.Count > 0)
        {
            if (_wallIdleCount == _wall._lIdleArr.Count - 1)
            {
                _wallIdleCount = 0;
            }
            else
            {
                _wallIdleCount++;
            }
        }
        else if (_wall._rIdleArr.Count > 0)
        {
            if (_wallIdleCount == _wall._rIdleArr.Count - 1)
            {
                _wallIdleCount = 0;
            }
            else
            {
                _wallIdleCount++;
            }
        }
        _wall.SetPosition(_position.X, _position.Y);
    }

    public void RenderAndUpdate(Renderer renderer, double elapsedTime)
    {
        Update(elapsedTime);
        if (Location.InView(_map._player._location))
        {
            SetPosition(_position.X, _position.Y);
            Render(renderer, elapsedTime);
        }
    }

    public void SetPosition(double x, double y)
    {
        _position.X = x;
        _position.Y = y;
        _sprite.SetPosition(_position.X, _position.Y);
        if (_wall != null)
        {
            _wall.SetPosition(_position.X, _position.Y);
        }
        double x2 = _position.X + 28.0 - (double)(_spellAniWidth / 2);
        double y2 = _position.Y + 27.0 - (double)_spellAniHeight;
        _spellAni.SetPosition(x2, y2);
        _targetBox.X = (int)(_position.X + _width / 2.0 - (double)(_targetBox.Width / 2));
        _targetBox.Y = (int)(_position.Y + _height - (double)_targetBox.Height - 2.0);
        _highlightSprite.SetPosition(_position.X + 11.0, _position.Y + 5.0);
        if (_iwo != null)
        {
            _iwo.move();
        }
    }

    public void SpellAni(int spellType, int speed, bool repeat = false)
    {
        if (repeat)
        {
            _spellAniRepeat = true;
        }
        _spellAniCount = 0;
        _spellAni.Texture = _blankTexture;
        List<int> list = effectTable[spellType - 1];
        _origSpellAniDelay = speed;
        _spellAniType = spellType;
        _spellFrameCount = list.Count;
        _spellAniArray.Clear();
        foreach (int item2 in list)
        {
            Texture item = _textureManager.Get("efct" + _spellAniType.ToString("000") + "_F" + item2 + "_C0");
            _spellAniArray.Add(item);
            if (item.Width > _spellAniWidth)
            {
                _spellAniWidth = item.Width;
            }
            if (item.Height > _spellAniHeight)
            {
                _spellAniHeight = item.Height;
            }
        }
        double x = 28.0 + _position.X - (double)(_spellAniWidth / 2);
        double y = _position.Y + 27.0 - (double)_spellAniHeight;
        _spellAni.SetPosition(x, y);
        _spellanimating = true;
    }

    public bool CollidesWith(int x, int y)
    {
        if (x >= _targetBox.X && x <= _targetBox.X + _targetBox.Width && y >= _targetBox.Y && y <= _targetBox.Y + _targetBox.Height)
        {
            return true;
        }
        return false;
    }

    public System.Drawing.Point[] ColRect()
    {
        return new System.Drawing.Point[4]
        {
            new System.Drawing.Point((int)(_position.X + _width / 2.0), (int)_position.Y),
            new System.Drawing.Point((int)(_position.X + _width), (int)(_position.Y + _height / 2.0)),
            new System.Drawing.Point((int)(_position.X + _width / 2.0), (int)(_position.Y + _height)),
            new System.Drawing.Point((int)_position.X, (int)(_position.Y + _height / 2.0))
        };
    }

    public RectangleF GetBoundingBox()
    {
        float width = (float)_width;
        float height = (float)_height;
        return new RectangleF((float)_sprite.GetPosition().X, (float)_sprite.GetPosition().Y, width, height);
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
