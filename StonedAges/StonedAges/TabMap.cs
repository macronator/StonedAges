using System.Drawing;
using Engine;
using Tao.OpenGl;

namespace StonedAges;

public class TabMap
{
    public Map _map;

    public bool _display;

    public Player _player;

    public float _scale = 3.3000002f;

    public TabMap()
    {
    }

    public TabMap(Map map, Player player)
    {
        _map = map;
        _player = player;
    }

    public void Update(double elapsedTime)
    {
    }

    public void Render(Renderer renderer)
    {
        if (!_display || _map == null)
        {
            return;
        }
        float num = 1f + _scale;
        float num2 = 56f / num;
        float num3 = 28f / num;
        float num4 = 313f - num2 / 2f - (float)_map._toTileX(_player._location.X, _player._location.Y) / num;
        float num5 = 157f - num3 / 2f - (float)_map._toTileY(_player._location.X, _player._location.Y) / num;
        for (int i = 0; (double)i < _map._height; i++)
        {
            for (int j = 0; (double)j < _map._width; j++)
            {
                Tile tile = _map._tiles[i * (int)_map._width + j];
                float num6 = num4 + (float)j * num2 / 2f;
                float num7 = num5 + (float)j * num3 / 2f;
                if (!tile._walkable)
                {
                    DrawDiamond(new RectangleF(num6, num7, num2, num3), new Engine.Color(1f, 1f, 1f, 0.4f));
                }
                foreach (Entity value in tile._entities.Values)
                {
                    if (value._id == _player._id)
                    {
                        DrawDiamond(new RectangleF(num6 + 1f / num, num7 + 1f / num, num2 - 2f / num, num3 - 2f / num), new Engine.Color(1f, 1f, 0f, 1f));
                    }
                    else if (value is NPC && !value.Hidden)
                    {
                        DrawDiamond(new RectangleF(num6 + 1f / num, num7 + 1f / num, num2 - 2f / num, num3 - 2f / num), new Engine.Color(0f, 1f, 0f, 1f));
                    }
                    else if (value is Player && !value.Hidden)
                    {
                        DrawDiamond(new RectangleF(num6 + 1f / num, num7 + 1f / num, num2 - 2f / num, num3 - 2f / num), Text.Colors(System.Drawing.Color.DeepSkyBlue));
                    }
                    else if (value is Monster)
                    {
                        DrawDiamond(new RectangleF(num6 + 1f / num, num7 + 1f / num, num2 - 2f / num, num3 - 2f / num), new Engine.Color(1f, 0f, 0f, 1f));
                    }
                }
            }
            num4 -= 28f / num;
            num5 += 14f / num;
        }
    }

    public void DrawDiamond(RectangleF bounds, Engine.Color color)
    {
        Gl.glDisable(3553);
        Gl.glColor4d(color.R, color.G, color.B, color.A);
        Gl.glBegin(9);
        Gl.glVertex2f(bounds.Left + bounds.Width / 2f, bounds.Top);
        Gl.glVertex2f(bounds.Right, bounds.Top + bounds.Height / 2f);
        Gl.glVertex2f(bounds.Left + bounds.Width / 2f, bounds.Bottom);
        Gl.glVertex2f(bounds.Left, bounds.Top + bounds.Height / 2f);
        Gl.glEnd();
        Gl.glEnable(3553);
    }
}
