using System;
using System.Drawing;
using System.Windows.Forms;

namespace Engine;

public class TextField
{
    private bool _hasFocus;

    public bool _allowClick = true;

    public int _tabIndex;

    public bool _showBack;

    public Vector _windowOffset = default(Vector);

    public Vector _position = default(Vector);

    public Sprite _background;

    public Sprite _highlightbackground;

    private Font _font;

    public Rectangle _rect;

    private string _defaulttext;

    private string _text;


    public Text _textObj;

    public Text _textObjHighlighted;

    private int _maxnum;

    public bool _allownewline;

    public bool _numbersonly;

    public bool _alphaonly;

    public bool _ignorespace;

    public bool _ignorespecial = true;

    public bool _allowApostrophe;

    public bool _hidetext;

    public EventHandler _onLoseFocus;

    public string Text
    {
        get
        {
            return _text;
        }
        set
        {
            _text = value;
            _textObj.ChangeText(_text);
        }
    }

    public void SetFocus()
    {
        _hasFocus = true;
        _textObj._drawCursor = true;
    }

    public bool CollidesWith(Point point)
    {
        if ((double)point.X >= _position.X && (double)point.X <= _position.X + (double)_rect.Width && (double)point.Y >= _position.Y && (double)point.Y <= _position.Y + (double)_rect.Height)
        {
            return true;
        }
        return false;
    }

    public void HandleInput(Input _input)
    {
        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Left))
        {
            if (_input.Keyboard.IsKeyPressedOrHeld(Keys.ShiftKey))
            {
                if (_textObj._text.Length - _textObj._highlightIndex > 0)
                {
                    if (_textObj._text.Length - _textObj._cursorIndex > _textObj._highlightIndex - _textObj._cursorIndex)
                    {
                        _textObj._highlightIndex++;
                    }
                    _textObjHighlighted.ChangeText(_textObj._text.Substring(_textObj._text.Length - _textObj._highlightIndex, _textObj._highlightIndex - _textObj._cursorIndex));
                    _textObjHighlighted.SetPosition(_textObj._position.X + (double)((_textObj._text.Length - _textObj._highlightIndex) * 6), _textObj._position.Y);
                    SetHighlightBackground();
                }
            }
            else
            {
                if (_textObj._text.Length > _textObj._cursorIndex)
                {
                    _textObj._cursorIndex++;
                }
                _textObj._highlightIndex = _textObj._cursorIndex;
                _textObjHighlighted.ChangeText("");
            }
        }
        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Right))
        {
            if (_input.Keyboard.IsKeyPressedOrHeld(Keys.ShiftKey))
            {
                if (_textObj._highlightIndex > _textObj._cursorIndex)
                {
                    _textObj._highlightIndex--;
                }
                _textObjHighlighted.ChangeText(_textObj._text.Substring(_textObj._text.Length - _textObj._highlightIndex, _textObj._highlightIndex - _textObj._cursorIndex));
                _textObjHighlighted.SetPosition(_textObj._position.X + (double)((_textObj._text.Length - _textObj._highlightIndex) * 6), _textObj._position.Y);
                SetHighlightBackground();
            }
            else
            {
                if (_textObj._cursorIndex > 0)
                {
                    _textObj._cursorIndex--;
                }
                _textObj._highlightIndex = _textObj._cursorIndex;
                _textObjHighlighted.ChangeText("");
            }
        }
        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Up))
        {
            if (_input.Keyboard.IsKeyPressedOrHeld(Keys.ShiftKey))
            {
                _textObj._highlightIndex = _textObj._text.Length;
                _textObjHighlighted.ChangeText(_textObj._text.Substring(0, _textObj._highlightIndex - _textObj._cursorIndex));
                _textObjHighlighted.SetPosition(_textObj._position.X, _textObj._position.Y);
                SetHighlightBackground();
            }
            else
            {
                _textObj._cursorIndex = _textObj._text.Length;
                _textObj._highlightIndex = _textObj._cursorIndex;
                _textObjHighlighted.ChangeText("");
            }
        }
        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Down))
        {
            _textObj._cursorIndex = 0;
            _textObj._highlightIndex = _textObj._cursorIndex;
            _textObjHighlighted.ChangeText("");
        }
        if (_allowClick && _input.Mouse.LeftPressed)
        {
            if (CollidesWith(_input.Mouse.Position))
            {
                _textObj._drawCursor = true;
            }
            else if (_textObj._drawCursor)
            {
                _textObj.RemoveCursor();
            }
        }
        if (_textObj._drawCursor)
        {
            _hasFocus = true;
            Text = inputKeys(_input, Text);
        }
        else if (_hasFocus)
        {
            OnLoseFocus();
            _hasFocus = false;
        }
    }

    private void OnLoseFocus()
    {
        _onLoseFocus(this, EventArgs.Empty);
    }

    private string inputKeys(Input _input, string msg)
    {
        if (msg.Length < _maxnum || _maxnum == 0)
        {
            if (_allownewline && _input.Keyboard.IsKeyPressedAndRepeated(Keys.Return))
            {
                msg = ClearHL(msg);
                msg = msg.Insert(msg.Length - _textObj._cursorIndex, "\n");
            }
            if (!_numbersonly || _alphaonly)
            {
                if (_input.Keyboard.IsKeyPressedOrHeld(Keys.ControlKey))
                {
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.C) && _textObjHighlighted._text != "")
                    {
                        Clipboard.SetText(_textObjHighlighted._text);
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.V) && Clipboard.ContainsText())
                    {
                        string text = Clipboard.GetText();
                        if (_textObjHighlighted._text != "")
                        {
                            msg = msg.Remove(msg.Length - _textObj._highlightIndex, _textObj._highlightIndex - _textObj._cursorIndex);
                            _textObjHighlighted.ChangeText("");
                        }
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, text);
                    }
                }
                else if (_input.Keyboard.IsKeyPressedOrHeld(Keys.ShiftKey) || Control.IsKeyLocked(Keys.Capital))
                {
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.A))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "A");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.B))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "B");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.C))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "C");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.D))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "D");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.E))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "E");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.F))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "F");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.G))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "G");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.H))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "H");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.I))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "I");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.J))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "J");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.K))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "K");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.L))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "L");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.M))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "M");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.N))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "N");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.O))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "O");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.P))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "P");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Q))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "Q");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.R))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "R");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.S))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "S");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.T))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "T");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.U))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "U");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.V))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "V");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.W))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "W");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.X))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "X");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Y))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "Y");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Z))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "Z");
                    }
                    if (!_ignorespecial)
                    {
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.OemOpenBrackets))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, "{");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.OemCloseBrackets))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, "}");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.OemSemicolon))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, ":");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.OemQuotes))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, "\"");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Oemcomma))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, "<");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.OemPeriod))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, ">");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Oemtilde))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, "~");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.OemQuestion))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, "?");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.OemMinus))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, "_");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Oemplus))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, "+");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.D1) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad1))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, "!");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.D2) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad2))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, "@");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.D3) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad3))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, "#");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.D4) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad4))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, "$");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.D5) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad5))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, "%");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.D6) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad6))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, "^");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.D7) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad7))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, "&");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.D8) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad8))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, "*");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.D9) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad9))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, "(");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.D0) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad0))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, ")");
                        }
                    }
                }
                else
                {
                    if ((!_ignorespecial || _allowApostrophe) && _input.Keyboard.IsKeyPressedAndRepeated(Keys.OemQuotes))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "'");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.A))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "a");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.B))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "b");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.C))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "c");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.D))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "d");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.E))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "e");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.F))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "f");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.G))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "g");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.H))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "h");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.I))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "i");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.J))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "j");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.K))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "k");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.L))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "l");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.M))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "m");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.N))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "n");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.O))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "o");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.P))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "p");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Q))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "q");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.R))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "r");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.S))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "s");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.T))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "t");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.U))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "u");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.V))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "v");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.W))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "w");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.X))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "x");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Y))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "y");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Z))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "z");
                    }
                    if (!_ignorespecial)
                    {
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.OemOpenBrackets))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, "[");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.OemCloseBrackets))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, "]");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.OemSemicolon))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, ";");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Oemcomma))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, ",");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.OemPeriod))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, ".");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Oemtilde))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, "`");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.OemQuestion))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, "/");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.OemMinus))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, "-");
                        }
                        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Oemplus))
                        {
                            msg = ClearHL(msg);
                            msg = msg.Insert(msg.Length - _textObj._cursorIndex, "=");
                        }
                    }
                }
                if (!_ignorespace)
                {
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Space))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, " ");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Add))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "+");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Multiply))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "*");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Divide))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "/");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Subtract))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, "-");
                    }
                    if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Decimal))
                    {
                        msg = ClearHL(msg);
                        msg = msg.Insert(msg.Length - _textObj._cursorIndex, ".");
                    }
                }
            }
            if ((_numbersonly || !_alphaonly) && !_input.Keyboard.IsKeyPressedOrHeld(Keys.ShiftKey))
            {
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.D1) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad1))
                {
                    msg = ClearHL(msg);
                    msg = msg.Insert(msg.Length - _textObj._cursorIndex, "1");
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.D2) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad2))
                {
                    msg = ClearHL(msg);
                    msg = msg.Insert(msg.Length - _textObj._cursorIndex, "2");
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.D3) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad3))
                {
                    msg = ClearHL(msg);
                    msg = msg.Insert(msg.Length - _textObj._cursorIndex, "3");
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.D4) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad4))
                {
                    msg = ClearHL(msg);
                    msg = msg.Insert(msg.Length - _textObj._cursorIndex, "4");
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.D5) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad5))
                {
                    msg = ClearHL(msg);
                    msg = msg.Insert(msg.Length - _textObj._cursorIndex, "5");
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.D6) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad6))
                {
                    msg = ClearHL(msg);
                    msg = msg.Insert(msg.Length - _textObj._cursorIndex, "6");
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.D7) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad7))
                {
                    msg = ClearHL(msg);
                    msg = msg.Insert(msg.Length - _textObj._cursorIndex, "7");
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.D8) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad8))
                {
                    msg = ClearHL(msg);
                    msg = msg.Insert(msg.Length - _textObj._cursorIndex, "8");
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.D9) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad9))
                {
                    msg = ClearHL(msg);
                    msg = msg.Insert(msg.Length - _textObj._cursorIndex, "9");
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.D0) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad0))
                {
                    msg = ClearHL(msg);
                    msg = msg.Insert(msg.Length - _textObj._cursorIndex, "0");
                }
            }
        }
        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Back) && msg.Length > 0)
        {
            if (_textObjHighlighted._text != "")
            {
                msg = ClearHL(msg);
            }
            else if (msg.Length - _textObj._cursorIndex > 0)
            {
                msg = msg.Remove(msg.Length - _textObj._cursorIndex - 1, 1);
            }
        }
        if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Delete) && msg.Length > 0)
        {
            if (_textObjHighlighted._text != "")
            {
                msg = ClearHL(msg);
            }
            else if (msg.Length - _textObj._cursorIndex < msg.Length)
            {
                msg = msg.Remove(msg.Length - _textObj._cursorIndex, 1);
                _textObj._cursorIndex--;
            }
        }
        if (_hidetext && msg.Length > 8)
        {
            msg = msg.Remove(msg.Length - 1);
        }
        return msg;
    }

    private string ClearHL(string msg)
    {
        if (_textObjHighlighted._text != "")
        {
            msg = msg.Remove(msg.Length - _textObj._highlightIndex, _textObj._highlightIndex - _textObj._cursorIndex);
            _textObjHighlighted.ChangeText("");
        }
        return msg;
    }

    public void SetPosition(double x, double y)
    {
        _position = new Vector(x, y, 0.0);
        SetBackground();
    }

    public void SetBackground()
    {
        if (_showBack)
        {
            _background = new Sprite();
            _background.SetWidth(_rect.Width);
            _background.SetHeight(_rect.Height);
            _background.SetPosition(_position.X + 1.0, _position.Y + 1.0);
            _background.SetColor(Color.White);
        }
        _textObj.SetPosition(_position.X + 4.0, _position.Y + 2.0);
    }

    public void SetHighlightBackground()
    {
        _highlightbackground = new Sprite();
        _highlightbackground.SetWidth((float)_textObjHighlighted.Width);
        _highlightbackground.SetHeight(_rect.Height);
        _highlightbackground.SetPosition(_textObjHighlighted._position.X - 1.0, _textObjHighlighted._position.Y);
        _highlightbackground.SetColor(Color.White);
    }

    public TextField(Rectangle rect, Font font, string text = "", int maxnum = 0, double winPosX = 0.0, double winPosY = 0.0)
    {
        _onLoseFocus = delegate
        {
        };
        _windowOffset.X = rect.X;
        _windowOffset.Y = rect.Y;
        _position.X = (double)rect.X + winPosX;
        _position.Y = (double)rect.Y + winPosY;
        _textObj = new Text(text, font, rect.Width);
        _textObj.SetPosition(_position.X + 4.0, _position.Y + 2.0);
        _textObjHighlighted = new Text("", font, rect.Width);
        _textObjHighlighted.SetColor(new Color(0f, 0f, 0f, 1f));
        _font = font;
        _rect = rect;
        _defaulttext = text;
        _text = text;
        _maxnum = maxnum;
    }

    public void Render(Renderer renderer)
    {
        if (_background != null)
        {
            renderer.DrawSprite(_background, 0);
        }
        renderer.DrawText(_textObj);
        if (_highlightbackground != null && _textObjHighlighted._text != "")
        {
            renderer.DrawSprite(_highlightbackground, 0);
        }
        renderer.DrawText(_textObjHighlighted);
    }
}
