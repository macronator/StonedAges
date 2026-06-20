using System.Drawing;
using Engine;
using Tao.OpenGl;

namespace StonedAges;

public class Slot
{
    public Vector _windowOffset = default(Vector);

    public Vector _position = default(Vector);

    public double _width = 33.0;

    public double _height = 33.0;

    public int Number;

    public InventoryItem _item;

    public Slot(int number, double rectx, double recty, double winOffx = 0.0, double winOffy = 0.0)
    {
        _windowOffset.X = rectx;
        _windowOffset.Y = recty;
        Number = number;
        _position.X = rectx + winOffx;
        _position.Y = recty + winOffy;
    }

    public void Update(double elapsedTime)
    {
    }

    public void Render(Renderer renderer)
    {
    }

    public bool CollidesWith(Engine.Point point)
    {
        if ((double)point.X >= _position.X && (double)point.X <= _position.X + _width && (double)point.Y >= _position.Y && (double)point.Y <= _position.Y + _height)
        {
            return true;
        }
        return false;
    }

    public RectangleF GetBoundingBox()
    {
        float width = (float)_width;
        float height = (float)_height;
        return new RectangleF((float)_position.X, (float)_position.Y, width, height);
    }

    public void Render_Debug()
    {
        Gl.glDisable(3553);
        RectangleF boundingBox = GetBoundingBox();
        Gl.glBegin(2);
        Gl.glColor3f(1f, 1f, 0f);
        Gl.glVertex2f(boundingBox.Left, boundingBox.Top);
        Gl.glVertex2f(boundingBox.Right, boundingBox.Top);
        Gl.glVertex2f(boundingBox.Right, boundingBox.Bottom);
        Gl.glVertex2f(boundingBox.Left, boundingBox.Bottom);
        Gl.glEnd();
        Gl.glEnable(3553);
    }
}
