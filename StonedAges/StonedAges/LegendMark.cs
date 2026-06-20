using Engine;

namespace StonedAges;

public class LegendMark
{
    private TextureManager _textureManager;

    public Sprite _image = new Sprite();

    public Text _textObj;

    public string _text;

    public string _id;

    public int _color;

    public int _icon;

    public LegendMark(TextureManager textureManager, Font font, int icon, string id, string text, int color = 0)
    {
        _textureManager = textureManager;
        _text = text;
        _id = id;
        _color = color;
        _icon = icon;
        _image.Texture = _textureManager.Get("legends_F" + _icon + "_C0");
        _textObj = new Text(_text, font);
        if (color != 0)
        {
            if (_color == 1)
            {
                _textObj.SetColor(Color.LightBlue);
            }
            else if (_color == 2)
            {
                _textObj.SetColor(Color.DarkOrange);
            }
            else if (_color == 3)
            {
                _textObj.SetColor(Color.SaddleBrown);
            }
        }
    }
}
