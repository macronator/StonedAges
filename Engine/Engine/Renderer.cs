using System.Drawing;
using Tao.OpenGl;

namespace Engine;

public class Renderer
{
    private Batch _batch = new Batch();

    private int _currentTextureId = -1;

    public void Render()
    {
        _batch.Draw();
    }

    public Renderer()
    {
        Gl.glEnable(3553);
        Gl.glEnable(3042);
        Gl.glBlendFunc(770, 771);
    }

    public void DrawSprite(Sprite sprite, byte colorize = 0)
    {
        if (sprite.Texture.Id == _currentTextureId)
        {
            _batch.AddSprite(sprite, colorize);
            return;
        }
        _batch.Draw();
        _currentTextureId = sprite.Texture.Id;
        Gl.glBindTexture(3553, _currentTextureId);
        _batch.AddSprite(sprite, colorize);
    }

    public void DrawImmediateModeVertex(Vector position, Color color, Point uvs)
    {
        Gl.glColor4f(color.R, color.G, color.B, color.A);
        Gl.glTexCoord2f(uvs.X, uvs.Y);
        Gl.glVertex3d(position.X, position.Y, position.Z);
    }

    public void DrawText(Text text, bool shade = false)
    {
        if (text == null)
        {
            return;
        }
        if (shade)
        {
            Text text2 = new Text(text._text, text._font);
            Text text3 = new Text(text._text, text._font);
            text2.SetColor(Color.Black);
            text3.SetColor(Color.Black);
            if (text._align == "center")
            {
                double num = text._maxWidth / 2;
                double num2 = text.Width / 2.0;
                double num3 = text._position.X - 1.0;
                num3 = num3 - num2 + num;
                text2.SetPosition(num3, text._position.Y);
                text3.SetPosition(num3 + 2.0, text._position.Y);
            }
            else
            {
                text2.SetPosition(text._position.X - 1.0, text._position.Y);
                text3.SetPosition(text._position.X + 1.0, text._position.Y);
            }
            foreach (CharacterSprite characterSprite in text2.CharacterSprites)
            {
                DrawSprite(characterSprite.Sprite, 0);
            }
            foreach (CharacterSprite characterSprite2 in text3.CharacterSprites)
            {
                DrawSprite(characterSprite2.Sprite, 0);
            }
        }
        foreach (CharacterSprite characterSprite3 in text.CharacterSprites)
        {
            DrawSprite(characterSprite3.Sprite, 0);
        }
    }

    public void DrawPixel(double x, double y, Color color)
    {
        Sprite sprite = new Sprite();
        sprite.SetPosition(x, y);
        sprite.SetWidth(1f);
        sprite.SetHeight(1f);
        sprite.SetColor(color);
        DrawSprite(sprite, 0);
    }

    public void DrawBorder(Rectangle rect, Color color, bool ignorebottommid = false)
    {
        Sprite sprite = new Sprite();
        sprite.SetPosition(rect.X - 1, rect.Y - 1);
        sprite.SetWidth(rect.Width + 2);
        sprite.SetHeight(1f);
        sprite.SetColor(color);
        DrawSprite(sprite, 0);
        sprite = new Sprite();
        sprite.SetPosition(rect.X + rect.Width, rect.Y);
        sprite.SetWidth(1f);
        sprite.SetHeight(rect.Height);
        sprite.SetColor(color);
        DrawSprite(sprite, 0);
        if (ignorebottommid)
        {
            sprite = new Sprite();
            sprite.SetPosition(rect.X - 1, rect.Y + rect.Height);
            sprite.SetWidth(rect.Width / 2 - 4);
            sprite.SetHeight(1f);
            sprite.SetColor(color);
            DrawSprite(sprite, 0);
            sprite = new Sprite();
            sprite.SetPosition(rect.X - 1 + rect.Width / 2 + 3, rect.Y + rect.Height);
            sprite.SetWidth(rect.Width / 2 - 1);
            sprite.SetHeight(1f);
            sprite.SetColor(color);
            DrawSprite(sprite, 0);
        }
        else
        {
            sprite = new Sprite();
            sprite.SetPosition(rect.X - 1, rect.Y + rect.Height);
            sprite.SetWidth(rect.Width + 2);
            sprite.SetHeight(1f);
            sprite.SetColor(color);
            DrawSprite(sprite, 0);
        }
        sprite = new Sprite();
        sprite.SetPosition(rect.X - 1, rect.Y);
        sprite.SetWidth(1f);
        sprite.SetHeight(rect.Height);
        sprite.SetColor(color);
        DrawSprite(sprite, 0);
    }
}
