using System.Collections.Generic;
using Engine;

namespace StonedAges;

public class Wall
{
    private TextureManager _textureManager;

    public Sprite _leftWall = new Sprite();

    public Sprite _rightWall = new Sprite();

    public Vector _position = default(Vector);

    public double _surfHeight;

    public Location Location;

    public int _lwall;

    public int _rwall;

    public List<int> _lIdleArr = new List<int>();

    public List<int> _rIdleArr = new List<int>();

    public int _idleSpeed;

    public Wall(TextureManager textureManager, int lwall, int rwall, Texture lwTexture, Texture rwTexture, int x, int y, double rectx, double recty)
    {
        _lwall = lwall;
        _rwall = rwall;
        _textureManager = textureManager;
        _leftWall.Texture = lwTexture;
        _rightWall.Texture = rwTexture;
        Location = new Location(x, y);
        SetPosition(rectx, recty);
    }

    public void SetPosition(double x, double y)
    {
        _position.X = x;
        _position.Y = y;
        _leftWall.SetPosition(_position.X, _position.Y - _leftWall.GetHeight() + 28.0);
        _rightWall.SetPosition(_position.X + 28.0, _position.Y - _rightWall.GetHeight() + 28.0);
    }
}
