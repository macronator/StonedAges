using System;
using System.Collections.Generic;

namespace Engine;

public class Font
{
    private Texture _texture;

    private Dictionary<char, CharacterData> _characterData;

    public Font(Texture texture, Dictionary<char, CharacterData> characterData)
    {
        _texture = texture;
        _characterData = characterData;
    }

    public Vector MeasureFont(string text)
    {
        return MeasureFont(text, -1.0);
    }

    public Vector MeasureFont(string text, double maxWidth)
    {
        Vector result = default(Vector);
        foreach (char key in text)
        {
            CharacterData characterData = _characterData[key];
            result.X += characterData.XAdvance;
            result.Y = Math.Max(result.Y, characterData.Height + characterData.YOffset);
        }
        return result;
    }

    public CharacterSprite CreateSprite(char c)
    {
        CharacterData characterData = _characterData[c];
        Sprite sprite = new Sprite();
        sprite.Texture = _texture;
        Point topLeft = new Point((float)characterData.X / (float)_texture.Width, (float)characterData.Y / (float)_texture.Height);
        sprite.SetUVs(bottomRight: new Point(topLeft.X + (float)characterData.Width / (float)_texture.Width, topLeft.Y + (float)characterData.Height / (float)_texture.Height), topLeft: topLeft);
        sprite.SetWidth(characterData.Width);
        sprite.SetHeight(characterData.Height);
        sprite.SetColor(new Color(1f, 1f, 1f, 1f));
        return new CharacterSprite(sprite, characterData);
    }
}
