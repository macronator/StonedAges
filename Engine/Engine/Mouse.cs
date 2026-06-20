using System.Drawing;
using System.Windows.Forms;

namespace Engine;

public class Mouse
{
    private bool outOfWindow;

    private TextureManager _textureManager;

    public Sprite _sprite = new Sprite();

    private Form _parentForm;

    private Control _openGLControl;

    private bool _wheelUpDetect;

    private bool _wheelDownDetect;

    private bool _leftClickDetect;

    private bool _rightClickDetect;

    private bool _middleClickDetect;

    private bool _leftDoubleClickDetect;

    private bool _rightDoubleClickDetect;

    private bool _middleDoubleClickDetect;

    private bool _leftPressDetect;

    public Point Position { get; set; }

    public bool DoubleMiddlePressed { get; private set; }

    public bool DoubleLeftPressed { get; private set; }

    public bool DoubleRightPressed { get; private set; }

    public bool MiddlePressed { get; private set; }

    public bool LeftDown { get; private set; }

    public bool LeftPressed { get; private set; }

    public bool RightPressed { get; private set; }

    public bool MiddleHeld { get; private set; }

    public bool LeftHeld { get; private set; }

    public bool RightHeld { get; set; }

    public bool WheelUp { get; set; }

    public bool WheelDown { get; set; }

    public Mouse(Form form, Control openGLControl, TextureManager textureManager)
    {
        Cursor.Hide();
        _textureManager = textureManager;
        _sprite.Texture = _textureManager.Get("mouse_F0_C0");
        _parentForm = form;
        _openGLControl = openGLControl;
        Control openGLControl2 = _openGLControl;
        MouseEventHandler value = delegate (object obj, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                _wheelUpDetect = true;
            }
            else
            {
                _wheelDownDetect = true;
            }
        };
        openGLControl2.MouseWheel += value;
        _openGLControl.MouseDoubleClick += delegate (object obj, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _leftDoubleClickDetect = true;
            }
            else if (e.Button == MouseButtons.Right)
            {
                _rightDoubleClickDetect = true;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                _middleDoubleClickDetect = true;
            }
        };
        _openGLControl.MouseClick += delegate (object obj, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _leftClickDetect = true;
            }
            else if (e.Button == MouseButtons.Right)
            {
                _rightClickDetect = true;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                _middleClickDetect = true;
            }
        };
        _openGLControl.MouseDown += delegate (object obj, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _leftPressDetect = true;
                LeftHeld = true;
            }
            else if (e.Button == MouseButtons.Right)
            {
                RightHeld = true;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                MiddleHeld = true;
            }
        };
        _openGLControl.MouseUp += delegate (object obj, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                LeftHeld = false;
            }
            else if (e.Button == MouseButtons.Right)
            {
                RightHeld = false;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                MiddleHeld = false;
            }
        };
        _openGLControl.MouseLeave += delegate
        {
            outOfWindow = true;
            Cursor.Show();
            LeftHeld = false;
            RightHeld = false;
            MiddleHeld = false;
        };
        _openGLControl.MouseEnter += delegate
        {
            if (outOfWindow)
            {
                outOfWindow = false;
                Cursor.Hide();
            }
        };
    }

    public void SetCursorDefault()
    {
        _sprite.Texture = _textureManager.Get("mouse_F0_C0");
    }

    public void SetCursorSelected()
    {
        _sprite.Texture = _textureManager.Get("mouse_F1_C0");
    }

    public void SetCursorLoading()
    {
        _sprite.Texture = _textureManager.Get("mouse_F2_C0");
    }

    public void Update(double elapsedTime)
    {
        if (!outOfWindow)
        {
            UpdateMousePosition();
        }
        UpdateMouseButtons();
    }

    private void UpdateMousePosition()
    {
        System.Drawing.Point position = Cursor.Position;
        position = _openGLControl.PointToClient(position);
        Position = new Point(position.X, position.Y);
        _sprite.SetPosition(Position.X - 13f, Position.Y - 8f);
    }

    private bool CollidesWith(int rectx, int recty)
    {
        if (rectx >= _openGLControl.Bounds.X && recty >= _openGLControl.Bounds.Y && rectx <= _openGLControl.Bounds.X + _openGLControl.ClientSize.Width && recty <= _openGLControl.Bounds.Y + _openGLControl.ClientSize.Height)
        {
            return true;
        }
        return false;
    }

    private void UpdateMouseButtons()
    {
        WheelUp = false;
        WheelDown = false;
        MiddlePressed = false;
        LeftDown = false;
        LeftPressed = false;
        RightPressed = false;
        DoubleMiddlePressed = false;
        DoubleLeftPressed = false;
        DoubleRightPressed = false;
        if (_wheelUpDetect)
        {
            WheelUp = true;
            _wheelUpDetect = false;
        }
        if (_wheelDownDetect)
        {
            WheelDown = true;
            _wheelDownDetect = false;
        }
        if (_leftPressDetect)
        {
            LeftDown = true;
            _leftPressDetect = false;
        }
        if (_leftClickDetect)
        {
            LeftPressed = true;
            _leftClickDetect = false;
        }
        if (_rightClickDetect)
        {
            RightPressed = true;
            _rightClickDetect = false;
        }
        if (_middleClickDetect)
        {
            MiddlePressed = true;
            _middleClickDetect = false;
        }
        if (_leftDoubleClickDetect)
        {
            DoubleLeftPressed = true;
            _leftDoubleClickDetect = false;
        }
        if (_rightDoubleClickDetect)
        {
            DoubleRightPressed = true;
            _rightDoubleClickDetect = false;
        }
        if (_middleDoubleClickDetect)
        {
            DoubleMiddlePressed = true;
            _middleDoubleClickDetect = false;
        }
    }

    public void Render(Renderer renderer)
    {
        renderer.DrawSprite(_sprite, 0);
    }
}
