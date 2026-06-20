using Engine;

namespace StonedAges;

public class DummyPlayer
{
    public PlayerBody _body;

    private TextureManager _textureManager;

    public byte _gender;

    public DummyPlayer(int gender, TextureManager textureManager)
    {
        _textureManager = textureManager;
        _gender = (byte)gender;
        _body = new PlayerBody(_textureManager, _gender);
        _body.SetPosition(464.0, 43.0);
        _body._bodyImgs["b"] = 1;
        _body._bodyImgs["a"] = 1;
    }

    public void Update(double elapsedTime)
    {
        _body.Update(elapsedTime);
    }

    public void Render(Renderer renderer)
    {
        _body.Render(renderer, swimming: false, hidden: false, ghosted: false, seethru: false, 0);
    }
}
