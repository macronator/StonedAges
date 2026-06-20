using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Engine;

public class Text
{
    public ushort BoardID;

    public ushort PostNumber;

    public Vector _windowOffset = default(Vector);

    public EventHandler _onPressEvent;

    public EventHandler _onDoublePressEvent;

    public Font _font;

    private Color _color = new Color(1f, 1f, 1f, 1f);

    private List<CharacterSprite> _bitmapText = new List<CharacterSprite>();

    public string _text;

    private Vector _dimensions;

    public int _maxWidth = -1;

    public Vector _position;

    public string _align = "left";

    public int _maxLines;

    public bool _drawCursor;

    public int _cursorIndex;

    public int _highlightIndex;

    public bool _shade;

    public int _lines = 1;

    public bool _colorize;

    public List<CharacterSprite> CharacterSprites => _bitmapText;

    public double Width
    {
        get
        {
            if (_text.Contains('\n'))
            {
                string[] source = _text.Split('\n');
                return source.Max((string x) => x.Length) * 6;
            }
            return _text.Length * 6;
        }
    }

    public double Height => _dimensions.Y;

    public static Color Colors(System.Drawing.Color color)
    {
        return new Color((float)(int)color.R / 255f, (float)(int)color.G / 255f, (float)(int)color.B / 255f, (float)(int)color.A / 255f);
    }

    public static Color Colors(byte R, byte G, byte B, byte A = byte.MaxValue)
    {
        return new Color((float)(int)R / 255f, (float)(int)G / 255f, (float)(int)B / 255f, (float)(int)A / 255f);
    }

    public Text(string text, Font font)
        : this(text, font, -1, 1)
    {
    }

    public Text(string text, Font font, int maxWidth, int lines = 0)
    {
        _onPressEvent = delegate
        {
        };
        _onDoublePressEvent = delegate
        {
        };
        _text = text;
        _font = font;
        _maxWidth = maxWidth;
        _maxLines = lines;
        CreateText(0.0, 0.0, _maxWidth);
    }

    public void SetColor()
    {
        foreach (CharacterSprite item in _bitmapText)
        {
            item.Sprite.SetColor(_color);
        }
    }

    public bool CollidesWith(Point point)
    {
        if ((double)point.X >= _position.X && (double)point.X <= _position.X + (double)_maxWidth && (double)point.Y >= _position.Y && (double)point.Y <= _position.Y + Height)
        {
            return true;
        }
        return false;
    }

    public void HandleInput(Input _input)
    {
        if (CollidesWith(_input.Mouse.Position))
        {
            if (_input.Mouse.LeftPressed)
            {
                OnPress();
            }
            if (_input.Mouse.DoubleLeftPressed)
            {
                OnDoublePress();
            }
        }
    }

    public void SetColor(Color color)
    {
        _color = color;
        foreach (CharacterSprite item in _bitmapText)
        {
            item.Sprite.SetColor(color);
        }
    }

    public void ChangeText(string newtext, bool colorize = false)
    {
        if (_colorize)
        {
            colorize = true;
        }
        _text = newtext;
        CreateText(_position.X, _position.Y, colorize);
    }

    public void SetPosition(double x, double y, bool colorize = false)
    {
        if (_colorize)
        {
            colorize = true;
        }
        CreateText(x, y, colorize);
    }

    public void OnPress()
    {
        _onPressEvent(this, EventArgs.Empty);
    }

    public void OnDoublePress()
    {
        _onDoublePressEvent(this, EventArgs.Empty);
    }

    public void Align(string align = "left")
    {
        _align = align;
    }

    private void CreateText(double x, double y, bool colorize = false)
    {
        if (_colorize)
        {
            colorize = true;
        }
        CreateText(x, y, _maxWidth, colorize);
    }

    public void RemoveCursor()
    {
        _drawCursor = false;
        if (_bitmapText.Count > 0)
        {
            _bitmapText.RemoveAt(_bitmapText.Count - 1);
        }
    }

    private void ColorCodedChars(char c, out Color lcolor)
    {
        switch (c)
        {
            case 'a':
                lcolor = Colors(112, 180, 168);
                break;
            case 'b':
                lcolor = Colors(200, 0, 16);
                break;
            case 'c':
                lcolor = Colors(248, 228, 56);
                break;
            case 'd':
                lcolor = Colors(0, 96, 0);
                break;
            case 'e':
                lcolor = Colors(120, 164, 240);
                break;
            case 'f':
                lcolor = Colors(32, 24, 152);
                break;
            case 'g':
                lcolor = Colors(216, 216, 216);
                break;
            case 'h':
                lcolor = Colors(184, 184, 184);
                break;
            case 'i':
                lcolor = Colors(144, 148, 144);
                break;
            case 'j':
                lcolor = Colors(112, 116, 112);
                break;
            case 'k':
                lcolor = Colors(80, 80, 80);
                break;
            case 'l':
                lcolor = Colors(48, 48, 48);
                break;
            case 'm':
                lcolor = Colors(8, 12, 8);
                break;
            case 'n':
                lcolor = Colors(0, 0, 8);
                break;
            case 'o':
                lcolor = Colors(240, 88, 136);
                break;
            case 'p':
                lcolor = Colors(112, 24, 112);
                break;
            case 'q':
                lcolor = Colors(0, 252, 0);
                break;
            case 'r':
                lcolor = Colors(0, 132, 112);
                break;
            case 's':
                lcolor = Colors(240, 140, 24);
                break;
            case 't':
                lcolor = Colors(96, 52, 24);
                break;
            case 'u':
                lcolor = Colors(248, 252, 248);
                break;
            case 'v':
                lcolor = Colors(160, 64, 0);
                break;
            case 'w':
                lcolor = Colors(232, 108, 8);
                break;
            case 'x':
                lcolor = Colors(248, 252, 248, 0);
                break;
            case 'y':
                lcolor = Colors(136, 148, 48);
                break;
            case 'z':
                lcolor = Colors(248, 252, 248);
                break;
            default:
                lcolor = _color;
                break;
        }
    }

    private void CreateText(double x, double y, double maxWidth, bool colorize = false)
    {
        if (_colorize)
        {
            colorize = true;
        }
        _lines = 1;
        _position = new Vector(x, y, 0.0);
        double num = x;
        if (_maxWidth != -1)
        {
            if (_align == "center")
            {
                double num2 = _maxWidth / 2;
                double num3 = Width / 2.0;
                x = x - num3 + num2;
            }
            if (_align == "right")
            {
                x = x + (double)_maxWidth - Width;
            }
            _ = _align == "left";
        }
        y += 10.0;
        double num4 = 0.0;
        double num5 = 0.0;
        _bitmapText.Clear();
        double num6 = 0.0;
        double num7 = 0.0;
        string[] array = _text.Split('\n');
        double num8 = 0.0;
        if (_text.Contains('\n'))
        {
            string[] array2 = array;
            foreach (string text in array2)
            {
                Color lcolor = _color;
                double num9 = 0.0;
                if (colorize)
                {
                    for (int j = 0; j < text.Length; j++)
                    {
                        if (text[j] == '{' && text.Length > j + 1 && text[j + 1] == '=')
                        {
                            num9 += 3.0;
                        }
                    }
                }
                double num10 = 12.0;
                double num11 = ((double)text.Length - num9) * 6.0;
                if (num11 > num8)
                {
                    num8 = num11;
                }
                string[] array3 = text.Split(' ');
                string[] array4 = array3;
                foreach (string text2 in array4)
                {
                    Vector vector = _font.MeasureFont(text2);
                    if (maxWidth != -1.0 && num6 + vector.X > maxWidth)
                    {
                        num6 = 0.0;
                        num7 -= num10;
                        _lines++;
                    }
                    string text3 = text2 + " ";
                    string text4 = "";
                    string text5 = text3;
                    foreach (char c in text5)
                    {
                        bool flag = false;
                        if (colorize)
                        {
                            if (c == '{')
                            {
                                text4 = "{";
                            }
                            else if (text4 == "{" && c == '=')
                            {
                                text4 = "{=";
                            }
                            else if (text4 == "{=")
                            {
                                flag = true;
                                ColorCodedChars(c, out lcolor);
                                text4 = "";
                            }
                        }
                        CharacterSprite characterSprite = _font.CreateSprite(c);
                        float num12 = characterSprite.Data.XOffset;
                        float num13 = characterSprite.Data.Height;
                        if (c == '[' || c == ']' || c == '{' || c == '}' || c == '(' || c == ')')
                        {
                            num13 += -2f;
                        }
                        if (c == 'j' || c == 'g' || c == 'p' || c == 'y' || c == 'q' || c == ',' || c == 'Q')
                        {
                            num13 += -1f;
                        }
                        if (c == '^' || c == '`' || c == '"' || c == '\'')
                        {
                            num13 += 7f;
                        }
                        if (c == '-')
                        {
                            num13 += 3f;
                        }
                        if (c == '=' || c == '~')
                        {
                            num13 += 2f;
                        }
                        if (c == '*' || c == '+' || c == '<' || c == '>')
                        {
                            num13 += 1f;
                        }
                        double num14 = x;
                        if (_align == "center")
                        {
                            double num15 = _maxWidth / 2;
                            double num16 = num11 / 2.0;
                            num14 = num - num16 + num15;
                        }
                        if (_align == "right")
                        {
                            num14 = num + (double)_maxWidth - num11;
                        }
                        characterSprite.Sprite.SetPosition(num14 + num6 + (double)num12, y - num7 - (double)num13);
                        characterSprite.Sprite.SetColor(lcolor);
                        num4 = num6;
                        num5 = num7;
                        if (flag)
                        {
                            _bitmapText.Remove(_bitmapText.Last());
                            _bitmapText.Remove(_bitmapText.Last());
                            num6 -= (double)(characterSprite.Data.XAdvance * 2);
                            flag = false;
                        }
                        else
                        {
                            num6 += (double)characterSprite.Data.XAdvance;
                            _bitmapText.Add(characterSprite);
                        }
                    }
                }
                num6 = 0.0;
                num7 += 0.0 - num10;
                _lines++;
            }
            _dimensions.X = num8;
            _dimensions.Y = num7;
            if (_drawCursor)
            {
                CharacterSprite characterSprite2 = _font.CreateSprite('|');
                float num17 = (float)characterSprite2.Data.XOffset - 3f;
                float num18 = (float)characterSprite2.Data.Height - 2f;
                characterSprite2.Sprite.SetPosition(x + num4 + (double)num17, y - num5 - (double)num18);
                characterSprite2.Sprite.SetColor(_color);
                _bitmapText.Add(characterSprite2);
            }
            return;
        }
        double num19 = 0.0;
        if (colorize)
        {
            for (int m = 0; m < _text.Length; m++)
            {
                if (_text[m] == '{' && _text.Length > m + 1 && _text[m + 1] == '=')
                {
                    num19 += 3.0;
                }
            }
        }
        Color lcolor2 = _color;
        double num20 = ((double)_text.Length - num19) * 6.0;
        if (num20 > num8)
        {
            num8 = num20;
        }
        string[] array5 = _text.Split(' ');
        string[] array6 = array5;
        foreach (string text6 in array6)
        {
            Vector vector2 = _font.MeasureFont(text6);
            if (_maxLines != 1 && maxWidth != -1.0 && num6 + vector2.X > maxWidth)
            {
                num6 = 0.0;
                num7 -= 12.0;
                _lines++;
            }
            string text7 = text6 + " ";
            string text8 = "";
            string text9 = text7;
            foreach (char c2 in text9)
            {
                bool flag2 = false;
                if (colorize)
                {
                    if (c2 == '{')
                    {
                        text8 = "{";
                    }
                    else if (text8 == "{" && c2 == '=')
                    {
                        text8 = "{=";
                    }
                    else if (text8 == "{=")
                    {
                        flag2 = true;
                        ColorCodedChars(c2, out lcolor2);
                        text8 = "";
                    }
                }
                CharacterSprite characterSprite3 = _font.CreateSprite(c2);
                float num22 = characterSprite3.Data.XOffset;
                float num23 = characterSprite3.Data.Height;
                if (c2 == '[' || c2 == ']' || c2 == '{' || c2 == '}' || c2 == '(' || c2 == ')')
                {
                    num23 += -2f;
                }
                if (c2 == 'j' || c2 == 'g' || c2 == 'p' || c2 == 'y' || c2 == 'q' || c2 == ',' || c2 == 'Q')
                {
                    num23 += -1f;
                }
                if (c2 == '^' || c2 == '`' || c2 == '"' || c2 == '\'')
                {
                    num23 += 7f;
                }
                if (c2 == '-')
                {
                    num23 += 3f;
                }
                if (c2 == '=' || c2 == '~')
                {
                    num23 += 2f;
                }
                if (c2 == '*' || c2 == '+' || c2 == '<' || c2 == '>')
                {
                    num23 += 1f;
                }
                characterSprite3.Sprite.SetPosition(x + num6 + (double)num22, y - num7 - (double)num23);
                characterSprite3.Sprite.SetColor(lcolor2);
                num4 = num6;
                num5 = num7;
                if (flag2)
                {
                    _bitmapText.Remove(_bitmapText.Last());
                    _bitmapText.Remove(_bitmapText.Last());
                    num6 -= (double)(characterSprite3.Data.XAdvance * 2);
                    flag2 = false;
                }
                else
                {
                    num6 += (double)characterSprite3.Data.XAdvance;
                    _bitmapText.Add(characterSprite3);
                }
            }
        }
        _dimensions.X = num8;
        _dimensions.Y = _lines * 12;
        if (_drawCursor)
        {
            CharacterSprite characterSprite4 = _font.CreateSprite('|');
            float num24 = (float)characterSprite4.Data.XOffset - (float)(_cursorIndex * 6) - 3f;
            float num25 = (float)characterSprite4.Data.Height - 2f;
            characterSprite4.Sprite.SetPosition(x + num4 + (double)num24, y - num5 - (double)num25);
            characterSprite4.Sprite.SetColor(_color);
            _bitmapText.Add(characterSprite4);
        }
    }
}
