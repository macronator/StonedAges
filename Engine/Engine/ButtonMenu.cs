using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Tao.OpenGl;

namespace Engine;

public class ButtonMenu
{
    public int _maxTabIndex;

    public int _currentTabIndex;

    private bool moving;

    public bool _moveable;

    public Vector _position = default(Vector);

    public double _width;

    public double _height;

    public int _sellitem;

    public int _catcount;

    public List<string> _catlist = new List<string>();

    public List<string> _withdrawcatlist = new List<string>();

    public Dictionary<int, string> _dyelist = new Dictionary<int, string>();

    public Dictionary<int, string> _buylist = new Dictionary<int, string>();

    public Dictionary<string, int> _selllist = new Dictionary<string, int>();

    public Dictionary<int, string> _depositlist = new Dictionary<int, string>();

    public Dictionary<string, int> _withdrawlist = new Dictionary<string, int>();

    public List<string> _learnskills = new List<string>();

    public List<string> _learnspells = new List<string>();

    public List<string> _learnactions = new List<string>();

    public Sprite _sprite = new Sprite();

    public Sprite _background = new Sprite();

    public Dictionary<string, Sprite> _backgrounds = new Dictionary<string, Sprite>();

    public string _selectedText = "";

    public ushort _selectedID;

    public ushort _boardID;

    public bool _closePost;

    private Input _input;

    private Font _font;

    public Dictionary<string, Button> _disButtons = new Dictionary<string, Button>();

    public Dictionary<string, Button> _buttons = new Dictionary<string, Button>();

    public Dictionary<string, Text> _labels = new Dictionary<string, Text>();

    public Dictionary<string, Text> _disLabels = new Dictionary<string, Text>();

    public Dictionary<string, TextField> _textFields = new Dictionary<string, TextField>();

    public Dictionary<string, TextField> _disTextFields = new Dictionary<string, TextField>();

    private Point posoff = new Point(0f, 0f);

    public bool collidesWithControls(Point mp)
    {
        foreach (TextField value in _textFields.Values)
        {
            if (value.CollidesWith(mp))
            {
                return true;
            }
        }
        foreach (TextField value2 in _disTextFields.Values)
        {
            if (value2.CollidesWith(mp))
            {
                return true;
            }
        }
        Button[] array = _disButtons.Values.ToArray();
        foreach (Button button in array)
        {
            if (button.Enabled && button.CollidesWith(mp))
            {
                return true;
            }
        }
        foreach (Button value3 in _buttons.Values)
        {
            if (value3.Enabled && value3.CollidesWith(mp))
            {
                return true;
            }
        }
        return false;
    }

    public void HandleInput()
    {
        Button[] array = _disButtons.Values.ToArray();
        foreach (Button button in array)
        {
            button.HandleInput(_input);
        }
        foreach (Button value in _buttons.Values)
        {
            value.HandleInput(_input);
        }
        foreach (TextField value2 in _textFields.Values)
        {
            value2.HandleInput(_input);
        }
        foreach (TextField value3 in _disTextFields.Values)
        {
            value3.HandleInput(_input);
        }
        foreach (Text value4 in _labels.Values)
        {
            value4.HandleInput(_input);
        }
        foreach (Text value5 in _disLabels.Values)
        {
            value5.HandleInput(_input);
        }
        if (_maxTabIndex > 0)
        {
            if (_input.Mouse.LeftPressed)
            {
                _currentTabIndex = 0;
                foreach (Button value6 in _buttons.Values)
                {
                    if (value6.CollidesWith(_input.Mouse.Position))
                    {
                        _currentTabIndex = value6._tabIndex;
                        ClearButtonFocus();
                        ClearTextFields();
                        SetFocus(value6);
                        break;
                    }
                }
                foreach (TextField value7 in _textFields.Values)
                {
                    if (value7._allowClick && value7.CollidesWith(_input.Mouse.Position))
                    {
                        _currentTabIndex = value7._tabIndex;
                        ClearButtonFocus();
                        ClearTextFields();
                        SetFocus(value7);
                        break;
                    }
                }
            }
            if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Tab))
            {
                if (!_input.Keyboard.IsKeyPressedOrHeld(Keys.ShiftKey))
                {
                    if (_currentTabIndex >= _maxTabIndex)
                    {
                        _currentTabIndex = 0;
                    }
                    _currentTabIndex++;
                }
                else
                {
                    _currentTabIndex--;
                    if (_currentTabIndex <= 0)
                    {
                        _currentTabIndex = _maxTabIndex;
                    }
                }
                ClearButtonFocus();
                ClearTextFields();
                Button[] array2 = _buttons.Values.ToArray();
                foreach (Button button2 in array2)
                {
                    if (button2._tabIndex != 0 && button2._tabIndex == _currentTabIndex)
                    {
                        SetFocus(button2);
                    }
                }
                TextField[] array3 = _textFields.Values.ToArray();
                foreach (TextField textField in array3)
                {
                    if (textField._tabIndex != 0 && textField._tabIndex == _currentTabIndex)
                    {
                        SetFocus(textField);
                    }
                }
            }
        }
        if (!_moveable)
        {
            return;
        }
        Point position = _input.Mouse.Position;
        if (CollidesWith(position) && !collidesWithControls(position))
        {
            if (_input.Mouse.LeftDown)
            {
                posoff = new Point((float)_position.X - position.X, (float)_position.Y - position.Y);
                moving = true;
            }
            if (!_input.Mouse.LeftHeld)
            {
                moving = false;
            }
            if (_input.Mouse.LeftHeld && moving)
            {
                Move(position);
            }
        }
    }

    public void Move(Point mp, bool resetposoff = false)
    {
        if (resetposoff)
        {
            posoff = new Point(0f, 0f);
        }
        _position.X = posoff.X + mp.X;
        _position.Y = posoff.Y + mp.Y;
        _background.SetPosition(_position);
        foreach (Sprite value in _backgrounds.Values)
        {
            value.SetPosition(_position.X + value._windowOffset.X, _position.Y + value._windowOffset.Y);
        }
        _sprite.SetPosition(_position.X + _sprite._windowOffset.X, _position.Y + _sprite._windowOffset.Y);
        Button[] array = _disButtons.Values.ToArray();
        foreach (Button button in array)
        {
            if (button._text != null && button._text._text != "")
            {
                button._text.SetPosition(_position.X + button._text._windowOffset.X, _position.Y + button._text._windowOffset.Y);
            }
            button.Position = new Vector(_position.X + button._windowOffset.X, _position.Y + button._windowOffset.Y, 0.0);
        }
        foreach (Button value2 in _buttons.Values)
        {
            value2.Position = new Vector(_position.X + value2._windowOffset.X, _position.Y + value2._windowOffset.Y, 0.0);
        }
        foreach (TextField value3 in _textFields.Values)
        {
            value3._position = new Vector(_position.X + value3._windowOffset.X, _position.Y + value3._windowOffset.Y, 0.0);
            value3.SetBackground();
        }
        foreach (TextField value4 in _disTextFields.Values)
        {
            value4._position = new Vector(_position.X + value4._windowOffset.X, _position.Y + value4._windowOffset.Y, 0.0);
            value4.SetBackground();
        }
        Text[] array2 = _disLabels.Values.ToArray();
        foreach (Text text in array2)
        {
            text.SetPosition(_position.X + text._windowOffset.X, _position.Y + text._windowOffset.Y, colorize: true);
        }
        foreach (Text value5 in _labels.Values)
        {
            value5.SetPosition(_position.X + value5._windowOffset.X, _position.Y + value5._windowOffset.Y, colorize: true);
        }
    }

    public ButtonMenu(Input input, Font font, double x = 0.0, double y = 0.0, double width = 640.0, double height = 480.0, bool moveable = false)
    {
        _moveable = moveable;
        _position.X = x;
        _position.Y = y;
        _width = width;
        _height = height;
        _font = font;
        _input = input;
    }

    public void AddButton(string name, Button button)
    {
        _buttons.Add(name, button);
    }

    public void AddLabel(string name, Text label)
    {
        _labels.Add(name, label);
    }

    public void AddTextField(string name, TextField textField)
    {
        _textFields.Add(name, textField);
    }

    public bool CollidesWith(Point point)
    {
        if ((double)point.X >= _position.X && (double)point.X <= _position.X + _width && (double)point.Y >= _position.Y && (double)point.Y <= _position.Y + _height)
        {
            return true;
        }
        return false;
    }

    public void Update(double elapsedTime)
    {
        Button[] array = _buttons.Values.ToArray();
        foreach (Button button in array)
        {
            button.Update(elapsedTime);
        }
    }

    public void Render(Renderer renderer)
    {
        renderer.DrawSprite(_background, 0);
        foreach (Sprite value in _backgrounds.Values)
        {
            renderer.DrawSprite(value, 0);
        }
        renderer.DrawSprite(_sprite, 0);
        Button[] array = _disButtons.Values.ToArray();
        foreach (Button button in array)
        {
            button.Render(renderer);
        }
        foreach (Button value2 in _buttons.Values)
        {
            value2.Render(renderer);
        }
        foreach (TextField value3 in _textFields.Values)
        {
            value3.Render(renderer);
        }
        foreach (TextField value4 in _disTextFields.Values)
        {
            value4.Render(renderer);
        }
        foreach (Text value5 in _disLabels.Values)
        {
            renderer.DrawText(value5, value5._shade);
        }
        foreach (Text value6 in _labels.Values)
        {
            renderer.DrawText(value6, value6._shade);
        }
    }

    public void Render_Debug(Rectangle bounds)
    {
        Gl.glDisable(3553);
        Gl.glBegin(2);
        Gl.glColor3f(1f, 0f, 0f);
        Gl.glVertex2i(bounds.Left, bounds.Top);
        Gl.glVertex2i(bounds.Right, bounds.Top);
        Gl.glVertex2i(bounds.Right, bounds.Bottom);
        Gl.glVertex2i(bounds.Left, bounds.Bottom);
        Gl.glEnd();
        Gl.glEnable(3553);
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

    public void SetFocus(Button b)
    {
        _currentTabIndex = b._tabIndex;
        b._hasfocus = true;
    }

    public void SetFocus(TextField t)
    {
        _currentTabIndex = t._tabIndex;
        t.SetFocus();
    }

    public void ClearButtonFocus()
    {
        Button[] array = _buttons.Values.ToArray();
        foreach (Button button in array)
        {
            if (button._hasfocus)
            {
                button._hasfocus = false;
            }
        }
    }

    public void ClearTextFields()
    {
        TextField[] array = _textFields.Values.ToArray();
        foreach (TextField textField in array)
        {
            if (textField._textObj._drawCursor)
            {
                textField._textObj.RemoveCursor();
            }
        }
    }
}
