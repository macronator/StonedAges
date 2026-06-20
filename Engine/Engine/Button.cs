using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Tao.OpenGl;

namespace Engine;

public class Button
{
    public int _tabIndex;

    private DateTime heldDelay = DateTime.UtcNow;

    private int repeatrate = 80;

    public static List<Button> List = new List<Button>();

    public EventHandler _onPressEvent;

    public EventHandler _onDoublePressEvent;

    public EventHandler _onHeldEvent;

    public EventHandler _onReleaseEvent;

    public EventHandler _onHoverEvent;

    public EventHandler _onStopHoverEvent;

    private bool hovering;

    private TextureManager _textureManager;

    public Text _text;

    public Vector _windowOffset = default(Vector);

    private Vector _position = default(Vector);

    public Sprite Sprite = new Sprite();

    private List<Sprite> Multiply = new List<Sprite>();

    public Texture _backupImage = default(Texture);

    public Texture _baseImage = default(Texture);

    public Texture _clickedImage = default(Texture);

    private bool _hasClickedImage;

    public Texture _focusedImage = default(Texture);

    private bool _hasFocusedImg;

    private Texture _disabledImage = default(Texture);

    public Texture _blankImage = default(Texture);

    private double _width = 1.0;

    private double _height = 1.0;

    private int _multiplyImg;

    public bool _clicked;

    private bool _selected;

    private bool _enabled = true;

    public bool _clickable = true;

    private bool _hidden;

    private string _imgString = "";

    private Texture[] _imgArr;

    private int _currentAni = -1;

    private double _origDelay;

    private double _aniDelay;

    private bool _reverseAni;

    private bool _multHorizontal;

    public Rectangle rect;

    public bool _hasfocus;

    private int _flashAni = -1;

    private bool _held;

    public double Width
    {
        get
        {
            return _width;
        }
        set
        {
            _width = value;
        }
    }

    public double Height
    {
        get
        {
            return _height;
        }
        set
        {
            _height = value;
        }
    }

    public Vector Position
    {
        get
        {
            return _position;
        }
        set
        {
            _position = value;
            UpdatePosition();
        }
    }

    public bool Held => _held;

    public bool Hidden
    {
        get
        {
            return _hidden;
        }
        set
        {
            _hidden = value;
            if (value)
            {
                Sprite.Texture = _blankImage;
            }
            else
            {
                ResetImage();
            }
        }
    }

    public bool Enabled
    {
        get
        {
            return _enabled;
        }
        set
        {
            _enabled = value;
            if (!value)
            {
                Sprite.Texture = _disabledImage;
            }
            else
            {
                ResetImage();
            }
        }
    }

    public bool Selected
    {
        get
        {
            return _selected;
        }
        set
        {
            _selected = value;
            if (value)
            {
                Sprite.Texture = _clickedImage;
            }
            else
            {
                ResetImage();
            }
        }
    }

    public Button(TextureManager textureManager, double x, double y, double width, double height, string baseImg, string clickedImg = "", string focusedImg = "", string disabledImg = "", Texture[] imgArr = null, Text text = null, int multiplyImg = 0, bool multHorzontal = false, double winPosX = 0.0, double winPosY = 0.0)
    {
        _windowOffset.X = x;
        _windowOffset.Y = y;
        _multHorizontal = multHorzontal;
        _imgArr = imgArr;
        _textureManager = textureManager;
        _onPressEvent = delegate
        {
        };
        _onDoublePressEvent = delegate
        {
        };
        _onHeldEvent = delegate
        {
        };
        _onReleaseEvent = delegate
        {
        };
        _onHoverEvent = delegate
        {
        };
        _onStopHoverEvent = delegate
        {
        };
        _imgString = baseImg;
        if (baseImg != "")
        {
            _baseImage = textureManager.Get(baseImg + "_C0");
        }
        if (clickedImg != "")
        {
            _clickedImage = textureManager.Get(clickedImg + "_C0");
            _hasClickedImage = true;
        }
        if (focusedImg != "")
        {
            _focusedImage = textureManager.Get(focusedImg + "_C0");
            _hasFocusedImg = true;
        }
        if (disabledImg != "")
        {
            _disabledImage = textureManager.Get(disabledImg + "_C0");
        }
        _multiplyImg = multiplyImg;
        ResetImage();
        Width = width;
        Height = height;
        _text = text;
        if (_text != null)
        {
            _text.SetColor(new Color(1f, 1f, 1f, 1f));
        }
        _position.X = x + winPosX;
        _position.Y = y + winPosY;
        rect = new Rectangle((int)_position.X, (int)_position.X, (int)width, (int)height);
        for (int i = 1; i < multiplyImg; i++)
        {
            Sprite sprite = new Sprite();
            if (_multHorizontal)
            {
                sprite._rotate = true;
            }
            sprite.Texture = _baseImage;
            Multiply.Add(sprite);
        }
        UpdatePosition();
        List.Add(this);
    }

    public void RepeatFlashAnimate(double delay)
    {
        _origDelay = delay;
        _aniDelay = delay;
        _flashAni = 0;
    }

    public void Animate(double delay, bool reverse = false)
    {
        _origDelay = delay;
        _aniDelay = delay;
        _reverseAni = reverse;
        _currentAni = 0;
        if (_reverseAni)
        {
            _currentAni = _imgArr.Length;
        }
    }

    public void Update(double elapsedTime)
    {
        if (_imgArr != null && _flashAni > -1 && Enabled)
        {
            _aniDelay -= elapsedTime * 1000.0;
            if (_aniDelay <= 0.0)
            {
                _aniDelay = _origDelay;
                if (_flashAni == _imgArr.Length - 1)
                {
                    if (!_clicked)
                    {
                        Sprite.Texture = _blankImage;
                    }
                    _flashAni = -1;
                }
                _flashAni++;
                if (!_clicked)
                {
                    Sprite.Texture = _imgArr[_flashAni];
                }
            }
        }
        if (_imgArr == null || _currentAni <= -1)
        {
            return;
        }
        _aniDelay -= elapsedTime * 1000.0;
        if (!(_aniDelay <= 0.0))
        {
            return;
        }
        _aniDelay = _origDelay;
        if (!_reverseAni)
        {
            if (_currentAni == _imgArr.Length - 1)
            {
                Sprite.Texture = _blankImage;
                Selected = true;
                _currentAni = -1;
                return;
            }
            _currentAni++;
        }
        else
        {
            if (_currentAni == 0)
            {
                Selected = false;
                _currentAni = -1;
                return;
            }
            _currentAni--;
        }
        Sprite.Texture = _imgArr[_currentAni];
    }

    public void HandleInput(Input _input)
    {
        if (!Enabled || Hidden)
        {
            return;
        }
        if (_hasfocus && _input.Keyboard.IsKeyPressed(Keys.Return))
        {
            OnPress();
        }
        if (CollidesWith(_input.Mouse.Position))
        {
            hovering = true;
            OnGainFocus();
            if (_input.Mouse.LeftDown)
            {
                repeatrate = 200;
                heldDelay = DateTime.UtcNow;
                _clicked = true;
                if (_hasClickedImage)
                {
                    Sprite.Texture = _clickedImage;
                }
            }
            if (_input.Mouse.LeftPressed)
            {
                repeatrate = 200;
                heldDelay = DateTime.UtcNow;
                OnPress();
            }
            if (_input.Mouse.DoubleLeftPressed)
            {
                repeatrate = 200;
                heldDelay = DateTime.UtcNow;
                OnDoublePress();
            }
            OnHover();
        }
        else
        {
            OnStopHover();
            OnLoseFocus();
        }
        if (!_input.Mouse.LeftHeld)
        {
            if (_held)
            {
                OnRelease();
            }
            {
                foreach (Button item in List)
                {
                    if (item._clicked && !item.Selected)
                    {
                        item._clicked = false;
                        item.ResetImage();
                    }
                }
                return;
            }
        }
        if (_clicked && DateTime.UtcNow.Subtract(heldDelay).TotalMilliseconds > (double)repeatrate)
        {
            repeatrate = 80;
            heldDelay = DateTime.UtcNow;
            OnHeld();
        }
    }

    public bool CollidesWith(Point point)
    {
        if ((double)point.X >= _position.X && (double)point.X <= _position.X + Width && (double)point.Y >= _position.Y && (double)point.Y <= _position.Y + Height)
        {
            return true;
        }
        return false;
    }

    public void Render_Debug()
    {
        Gl.glDisable(3553);
        Rectangle rectangle = new Rectangle((int)_position.X, (int)_position.Y, (int)_width, (int)_height);
        Gl.glBegin(2);
        Gl.glColor3f(1f, 0f, 0f);
        Gl.glVertex2i(rectangle.Left, rectangle.Top);
        Gl.glVertex2i(rectangle.Right, rectangle.Top);
        Gl.glVertex2i(rectangle.Right, rectangle.Bottom);
        Gl.glVertex2i(rectangle.Left, rectangle.Bottom);
        Gl.glEnd();
        Gl.glEnable(3553);
    }

    public void UpdatePosition()
    {
        Sprite.SetPosition(_position.X + Sprite._windowOffset.X, _position.Y + Sprite._windowOffset.Y);
        int num = 0;
        foreach (Sprite item in Multiply)
        {
            num++;
            if (_multHorizontal)
            {
                item.SetPosition(_position.X + (double)((_baseImage.Width - 1) * num), _position.Y);
            }
            else
            {
                item.SetPosition(_position.X, _position.Y + (double)(_baseImage.Height * num));
            }
        }
    }

    public void ResetImage()
    {
        Sprite.Texture = _baseImage;
    }

    public void OnGainFocus()
    {
        if (_hasFocusedImg)
        {
            Sprite.Texture = _focusedImage;
        }
    }

    public void OnLoseFocus()
    {
        if (_hasFocusedImg)
        {
            ResetImage();
        }
    }

    public void OnStopHover()
    {
        if (hovering)
        {
            hovering = false;
            _onStopHoverEvent(this, EventArgs.Empty);
        }
    }

    public void OnPress()
    {
        _onPressEvent(this, EventArgs.Empty);
    }

    public void OnDoublePress()
    {
        _onDoublePressEvent(this, EventArgs.Empty);
    }

    public void OnRelease()
    {
        _held = false;
        _onReleaseEvent(this, EventArgs.Empty);
    }

    public void OnHeld()
    {
        _held = true;
        _onHeldEvent(this, EventArgs.Empty);
    }

    public void OnHover()
    {
        _onHoverEvent(this, EventArgs.Empty);
    }

    public void Render(Renderer renderer)
    {
        renderer.DrawSprite(Sprite, 0);
        foreach (Sprite item in Multiply)
        {
            renderer.DrawSprite(item, 0);
        }
        if (_text != null)
        {
            renderer.DrawText(_text);
        }
        if (_hasfocus)
        {
            renderer.DrawBorder(new Rectangle((int)_position.X - 1, (int)_position.Y - 1, (int)_width + 2, (int)_height + 2), Text.Colors(System.Drawing.Color.Gray));
            renderer.DrawBorder(new Rectangle((int)_position.X - 2, (int)_position.Y - 2, (int)_width + 4, (int)_height + 4), Text.Colors(System.Drawing.Color.Gray));
        }
    }
}
