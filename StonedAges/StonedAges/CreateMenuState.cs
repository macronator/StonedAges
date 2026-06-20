using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Engine;
using Newtonsoft.Json;
using Tao.OpenGl;

namespace StonedAges;

/// <summary>
///     The character-creation screen: choose gender, hairstyle and colour, enter a name/password,
///     and write the new character's save file under players\.
/// </summary>
internal class CreateMenuState : IGameObject
{
    private Random rand = new Random();

    private Sprite _orb = new Sprite();

    private Sprite _background = new Sprite();

    private DummyPlayer _dummy;

    private int _orbCount = -1;

    private double _orbDelay = 200.0;

    private Renderer _renderer = new Renderer();

    private Engine.Font _font;

    private Input _input;

    private ButtonMenu _menu;

    private StateSystem _system;

    private TextureManager _textureManager;

    private ButtonMenu _prompt;

    public CreateMenuState(Engine.Font font, TextureManager textureManager, Input input, StateSystem system)
    {
        _textureManager = textureManager;
        _system = system;
        _input = input;
        _font = font;
        _dummy = new DummyPlayer(rand.Next(2), _textureManager);
        _dummy._body.SetScale(2.0);
        _dummy._body._hairType = rand.Next(41);
        _dummy._body._hairColor = rand.Next(72);
        _dummy._body._bodyImgs["h"] = _dummy._body._hairType;
        _dummy._body._bodyColors["h"] = _dummy._body._hairColor;
        _orb.SetPosition(468.0, 88.0);
        _background.Texture = _textureManager.Get("dlgcre00_F0_C0");
        InitializeMenu();
    }

    public void Update(double elapsedTime)
    {
        if (_prompt._labels["promptText"]._text != "")
        {
            if (_input.Keyboard.IsKeyPressed(Keys.Escape) || _input.Keyboard.IsKeyPressed(Keys.Space) || _input.Keyboard.IsKeyPressed(Keys.Return))
            {
                _prompt._labels["promptText"].ChangeText("");
            }
            _prompt.HandleInput();
        }
        else
        {
            if (_input.Keyboard.IsKeyPressed(Keys.Return))
            {
                if (_menu._textFields["nameBox"]._textObj._drawCursor || _menu._textFields["passwordBox"]._textObj._drawCursor || _menu._textFields["confirmBox"]._textObj._drawCursor)
                {
                    _menu._buttons["okBtn"].OnPress();
                }
                else
                {
                    foreach (Engine.Button value in _menu._buttons.Values)
                    {
                        if (value._hasfocus)
                        {
                            value.OnPress();
                        }
                    }
                }
            }
            if (_input.Keyboard.IsKeyPressed(Keys.Escape))
            {
                _menu._currentTabIndex = 1;
                _menu._textFields["nameBox"].SetFocus();
                _system.ChangeState("start_menu");
            }
            _menu.HandleInput();
            if (_menu._labels["curtext"]._text != _dummy._body._hairType.ToString())
            {
                _menu._labels["curtext"].ChangeText(_dummy._body._hairType.ToString());
            }
            if (_menu._labels["curcoltext"]._text != _dummy._body._hairColor.ToString())
            {
                _menu._labels["curcoltext"].ChangeText(_dummy._body._hairColor.ToString());
            }
        }
        _dummy.Update(elapsedTime);
        _orbDelay -= elapsedTime * 1000.0;
        if (_orbDelay <= 0.0)
        {
            _orbDelay = 200.0;
            if (_orbCount >= 7)
            {
                _orbCount = -1;
            }
            if (_orbCount == -1)
            {
                _orbCount++;
                _orb.Texture = default(Texture);
            }
            else
            {
                _orb.Texture = _textureManager.Get("dlgcre02_F" + _orbCount + "_C0");
                _orbCount++;
            }
        }
    }

    private Text DrawLabel(string text, System.Drawing.Color color, double x, double y, int maxWidth, string align, int lines = 0, bool shade = false, double winPosX = 0.0, double winPosY = 0.0)
    {
        Text text2 = new Text(text, _font, maxWidth, lines);
        text2._windowOffset.X = x;
        text2._windowOffset.Y = y;
        text2._shade = shade;
        text2.SetColor(Text.Colors(color));
        text2.Align(align);
        text2.SetPosition(x + winPosX, y + winPosY);
        return text2;
    }

    public void Render(double elapsedTime)
    {
        Gl.glClearColor(0f, 0f, 0f, 1f);
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
        _renderer.DrawSprite(_background, 0);
        _renderer.DrawSprite(_orb, 0);
        _dummy.Render(_renderer);
        _menu.Render(_renderer);
        if (_prompt._labels["promptText"]._text != "")
        {
            _prompt.Render(_renderer);
        }
        _input.Mouse.Render(_renderer);
        _renderer.Render();
    }

    private void InitializeMenu()
    {
        Vector vector = default(Vector);
        vector = new Vector(190.0, 183.0, 0.0);
        _prompt = new ButtonMenu(_input, _font, vector.X, vector.Y, 263.0, 74.0, moveable: true);
        _prompt._sprite.Texture = _textureManager.Get("promptok");
        _prompt._sprite.SetPosition(vector);
        Engine.Button button = new Engine.Button(_textureManager, 184.0, 43.0, 70.0, 22.0, "butt001_F15", "butt001_F16", "", "butt001_F17", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button._onPressEvent = delegate
        {
            _prompt._labels["promptText"].ChangeText("");
        };
        Text value = DrawLabel("", System.Drawing.Color.White, 6.0, 6.0, 250, "left", 5, shade: false, vector.X, vector.Y);
        _prompt._labels.Add("promptText", value);
        _prompt._buttons.Add("promptOkBtn", button);
        _menu = new ButtonMenu(_input, _font);
        _menu._maxTabIndex = 23;
        TextField nameBox = new TextField(new Rectangle(125, 28, 120, 15), _font);
        nameBox._tabIndex = 1;
        _menu._currentTabIndex = 1;
        nameBox._alphaonly = true;
        nameBox._ignorespace = true;
        nameBox.SetFocus();
        TextField textField = new TextField(new Rectangle(135, 80, 108, 15), _font);
        textField._tabIndex = 2;
        textField._ignorespace = true;
        TextField textField2 = new TextField(new Rectangle(135, 132, 108, 15), _font);
        textField2._tabIndex = 3;
        textField2._ignorespace = true;
        TextField textField3 = new TextField(new Rectangle(52, 273, 363, 13), _font, "You have my gratitude");
        textField3._tabIndex = 4;
        TextField textField4 = new TextField(new Rectangle(52, 292, 363, 13), _font, "Please teach me things");
        textField4._tabIndex = 5;
        TextField textField5 = new TextField(new Rectangle(52, 311, 363, 13), _font, "Adventure with me?");
        textField5._tabIndex = 6;
        TextField textField6 = new TextField(new Rectangle(52, 330, 363, 13), _font, "Group me, please");
        textField6._tabIndex = 7;
        TextField textField7 = new TextField(new Rectangle(52, 349, 363, 13), _font, "I'll group you");
        textField7._tabIndex = 8;
        TextField textField8 = new TextField(new Rectangle(52, 368, 363, 13), _font, "Help!");
        textField8._tabIndex = 9;
        TextField textField9 = new TextField(new Rectangle(52, 387, 363, 13), _font, "Be careful");
        textField9._tabIndex = 10;
        TextField textField10 = new TextField(new Rectangle(52, 406, 363, 13), _font, "Fair day");
        textField10._tabIndex = 11;
        TextField textField11 = new TextField(new Rectangle(52, 425, 363, 13), _font, "Farewell, Aisling");
        textField11._tabIndex = 12;
        TextField textField12 = new TextField(new Rectangle(52, 444, 363, 13), _font, "Greetings, Aisling");
        textField12._tabIndex = 13;
        Engine.Button helpBtn = new Engine.Button(_textureManager, 328.0, 190.0, 26.0, 25.0, "helpbtn1_F0", "helpbtn1_F1");
        helpBtn._tabIndex = 14;
        Engine.Button button2 = new Engine.Button(_textureManager, 467.0, 23.0, 37.0, 29.0, "dlgcre01_F0", "dlgcre01_F1");
        button2._tabIndex = 15;
        Engine.Button button3 = new Engine.Button(_textureManager, 537.0, 23.0, 37.0, 29.0, "dlgcre01_F3", "dlgcre01_F4");
        button3._tabIndex = 16;
        Engine.Button button4 = new Engine.Button(_textureManager, 417.0, 132.0, 19.0, 15.0, "dlgcre03_F0", "dlgcre03_F1");
        button4._tabIndex = 17;
        Engine.Button button5 = new Engine.Button(_textureManager, 603.0, 132.0, 19.0, 15.0, "dlgcre03_F3", "dlgcre03_F4");
        button5._tabIndex = 18;
        Engine.Button button6 = new Engine.Button(_textureManager, 469.0, 230.0, 19.0, 15.0, "dlgcre03_F0", "dlgcre03_F1");
        button6._tabIndex = 19;
        Engine.Button button7 = new Engine.Button(_textureManager, 555.0, 230.0, 19.0, 15.0, "dlgcre03_F3", "dlgcre03_F4");
        button7._tabIndex = 20;
        Engine.Button button8 = new Engine.Button(_textureManager, 469.0, 262.0, 19.0, 15.0, "dlgcre03_F0", "dlgcre03_F1");
        button8._tabIndex = 21;
        Engine.Button button9 = new Engine.Button(_textureManager, 555.0, 262.0, 19.0, 15.0, "dlgcre03_F3", "dlgcre03_F4");
        button9._tabIndex = 22;
        Engine.Button button10 = new Engine.Button(_textureManager, 452.0, 438.0, 70.0, 22.0, "butt001_F15", "butt001_F16");
        button10._tabIndex = 23;
        Engine.Button button11 = new Engine.Button(_textureManager, 552.0, 438.0, 70.0, 22.0, "butt001_F21", "butt001_F22");
        Text value2 = DrawLabel(_dummy._body._hairType.ToString(), System.Drawing.Color.Black, 582.0, 232.0, 230, "left", 1, shade: true);
        _menu._labels.Add("curtext", value2);
        Text value3 = DrawLabel(_dummy._body._hairColor.ToString(), System.Drawing.Color.Black, 582.0, 264.0, 230, "left", 1, shade: true);
        _menu._labels.Add("curcoltext", value3);
        button4._onPressEvent = delegate
        {
            if (_dummy._body._direction > 0)
            {
                _dummy._body._direction = _dummy._body._direction - 1;
            }
            else
            {
                _dummy._body._direction = 3;
            }
        };
        button5._onPressEvent = delegate
        {
            if (_dummy._body._direction < 3)
            {
                _dummy._body._direction = _dummy._body._direction + 1;
            }
            else
            {
                _dummy._body._direction = 0;
            }
        };
        button8._onPressEvent = delegate
        {
            if (_dummy._body._hairColor > 0)
            {
                _dummy._body._hairColor = _dummy._body._hairColor - 1;
                _dummy._body._bodyColors["h"] = _dummy._body._hairColor;
            }
            else
            {
                _dummy._body._hairColor = 71;
                _dummy._body._bodyColors["h"] = _dummy._body._hairColor;
            }
        };
        button9._onPressEvent = delegate
        {
            if (_dummy._body._hairColor < 71)
            {
                _dummy._body._hairColor = _dummy._body._hairColor + 1;
                _dummy._body._bodyColors["h"] = _dummy._body._hairColor;
            }
            else
            {
                _dummy._body._hairColor = 0;
                _dummy._body._bodyColors["h"] = _dummy._body._hairColor;
            }
        };
        button6._onPressEvent = delegate
        {
            if (_dummy._body._hairType > 0)
            {
                _dummy._body._hairType = _dummy._body._hairType - 1;
                _dummy._body._bodyImgs["h"] = _dummy._body._hairType;
            }
            else
            {
                _dummy._body._hairType = 57;
                _dummy._body._bodyImgs["h"] = _dummy._body._hairType;
            }
        };
        button7._onPressEvent = delegate
        {
            if (_dummy._body._hairType < 57)
            {
                _dummy._body._hairType = _dummy._body._hairType + 1;
                _dummy._body._bodyImgs["h"] = _dummy._body._hairType;
            }
            else
            {
                _dummy._body._hairType = 0;
                _dummy._body._bodyImgs["h"] = _dummy._body._hairType;
            }
        };
        button2._onPressEvent = delegate
        {
            _dummy = new DummyPlayer(1, _textureManager);
            _dummy._body.SetScale(2.0);
            _dummy._body._hairType = rand.Next(58);
            _dummy._body._hairColor = rand.Next(72);
            _dummy._body._bodyImgs["h"] = _dummy._body._hairType;
            _dummy._body._bodyColors["h"] = _dummy._body._hairColor;
        };
        button3._onPressEvent = delegate
        {
            _dummy = new DummyPlayer(0, _textureManager);
            _dummy._body.SetScale(2.0);
            _dummy._body._hairType = rand.Next(58);
            _dummy._body._hairColor = rand.Next(72);
            _dummy._body._bodyImgs["h"] = _dummy._body._hairType;
            _dummy._body._bodyColors["h"] = _dummy._body._hairColor;
        };
        button10._onPressEvent = delegate
        {
            string text = _menu._textFields["nameBox"].Text;
            string text2 = _menu._textFields["passwordBox"].Text;
            if (text.Count() < 3)
            {
                _prompt._labels["promptText"].ChangeText("Player name must be between 3 and 16 letters.");
            }
            else if (text2.Count() < 4)
            {
                _prompt._labels["promptText"].ChangeText("Password must be between 4 and 16 characters.");
            }
            else if (_menu._textFields["confirmBox"].Text != text2)
            {
                _prompt._labels["promptText"].ChangeText("Password and Confirm have to match.");
            }
            else
            {
                if (!Directory.Exists("players\\" + text.ToLower()))
                {
                    Directory.CreateDirectory("players\\" + text.ToLower());
                }
                bool flag = false;
                string path = "players\\" + text.ToLower() + "\\" + text.ToLower() + ".txt";
                if (File.Exists(path))
                {
                    flag = true;
                }
                if (flag)
                {
                    _prompt._labels["promptText"].ChangeText("That name already exists.");
                }
                else
                {
                    bool flag2 = false;
                    if (text.ToLower().Contains("hitler"))
                    {
                        flag2 = true;
                    }
                    if (flag2)
                    {
                        _prompt._labels["promptText"].ChangeText("Choose a name that doesn't suck.");
                    }
                    else
                    {
                        PlayerDataS playerDataS = default(PlayerDataS);
                        playerDataS.name = text.ToLower();
                        playerDataS.password = text2;
                        playerDataS.gender = _dummy._gender;
                        playerDataS.hairstyle = (byte)_dummy._body._hairType;
                        playerDataS.haircolor = (byte)_dummy._body._hairColor;
                        playerDataS.direction = 1;
                        playerDataS.mapnum = 136;
                        playerDataS.x = 5;
                        playerDataS.y = 7;
                        playerDataS.ac = 0;
                        playerDataS.STR = 3;
                        playerDataS.INT = 3;
                        playerDataS.WIS = 3;
                        playerDataS.CON = 3;
                        playerDataS.DEX = 3;
                        playerDataS.availstats = 0;
                        playerDataS.basehp = 100;
                        playerDataS.basemp = 100;
                        playerDataS.curhp = 100;
                        playerDataS.curmp = 100;
                        playerDataS.exp = 1u;
                        playerDataS.next = 599u;
                        playerDataS.lev = 1;
                        playerDataS.master = false;
                        playerDataS.guild = "";
                        playerDataS.national = 0;
                        playerDataS.rank = "";
                        playerDataS.title = "";
                        playerDataS.quest = new List<QuestS>();
                        playerDataS.legend = new List<LegendS>
                        {
                            new LegendS
                            {
                                color = 0,
                                icon = 0,
                                text = "Chaos Age Aisling - Deoch 1, Spring",
                                type = "A1"
                            }
                        };
                        playerDataS.inv = new List<InventoryS>
                        {
                            new InventoryS
                            {
                                name = "Gold",
                                slot = 72,
                                stack = 0
                            }
                        };
                        playerDataS.equip = new List<EquipS>();
                        playerDataS.skills = new List<SkillS>
                        {
                            new SkillS
                            {
                                name = "Assail",
                                slot = 1,
                                level = 0
                            }
                        };
                        playerDataS.spells = new List<SpellS>();
                        playerDataS.actions = new List<ActionS>();
                        JsonSerializer jsonSerializer = new JsonSerializer
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        };
                        using (StreamWriter textWriter = new StreamWriter(path))
                        {
                            using JsonWriter jsonWriter = new JsonTextWriter(textWriter);
                            jsonWriter.Formatting = Formatting.Indented;
                            jsonSerializer.Serialize(jsonWriter, playerDataS);
                        }
                        GameWindow.newCharCreated = true;
                        _menu._textFields["nameBox"]._textObj.RemoveCursor();
                        _menu._textFields["nameBox"].Text = "";
                        _menu._textFields["passwordBox"]._textObj.RemoveCursor();
                        _menu._textFields["passwordBox"].Text = "";
                        _menu._textFields["confirmBox"]._textObj.RemoveCursor();
                        _menu._textFields["confirmBox"].Text = "";
                        _dummy = new DummyPlayer(rand.Next(2), _textureManager);
                        _dummy._body.SetScale(2.0);
                        _dummy._body._hairType = rand.Next(58);
                        _dummy._body._hairColor = rand.Next(72);
                        _dummy._body._bodyImgs["h"] = _dummy._body._hairType;
                        _dummy._body._bodyColors["h"] = _dummy._body._hairColor;
                        _system.ChangeState("start_menu");
                        if (helpBtn.Selected)
                        {
                            helpBtn.Selected = false;
                        }
                        _menu._currentTabIndex = 1;
                        _menu._textFields["nameBox"].SetFocus();
                    }
                }
            }
        };
        button11._onPressEvent = delegate
        {
            _dummy = new DummyPlayer(rand.Next(2), _textureManager);
            _dummy._body.SetScale(2.0);
            _dummy._body._hairType = rand.Next(58);
            _dummy._body._hairColor = rand.Next(72);
            _dummy._body._bodyImgs["h"] = _dummy._body._hairType;
            _dummy._body._bodyColors["h"] = _dummy._body._hairColor;
            _system.ChangeState("start_menu");
            if (helpBtn.Selected)
            {
                helpBtn.Selected = false;
            }
            _menu._currentTabIndex = 1;
            nameBox.SetFocus();
        };
        _menu.AddTextField("nameBox", nameBox);
        _menu.AddTextField("passwordBox", textField);
        _menu.AddTextField("confirmBox", textField2);
        _menu.AddTextField("phrase1Box", textField3);
        _menu.AddTextField("phrase2Box", textField4);
        _menu.AddTextField("phrase3Box", textField5);
        _menu.AddTextField("phrase4Box", textField6);
        _menu.AddTextField("phrase5Box", textField7);
        _menu.AddTextField("phrase6Box", textField8);
        _menu.AddTextField("phrase7Box", textField9);
        _menu.AddTextField("phrase8Box", textField10);
        _menu.AddTextField("phrase9Box", textField11);
        _menu.AddTextField("phrase10Box", textField12);
        _menu.AddButton("helpBtn", helpBtn);
        _menu.AddButton("maleBtn", button2);
        _menu.AddButton("femaleBtn", button3);
        _menu.AddButton("turnLbtn", button4);
        _menu.AddButton("turnRbtn", button5);
        _menu.AddButton("hairLbtn", button6);
        _menu.AddButton("hairRbtn", button7);
        _menu.AddButton("hairColorLbtn", button8);
        _menu.AddButton("hairColorRbtn", button9);
        _menu.AddButton("okBtn", button10);
        _menu.AddButton("closeMenu", button11);
    }
}
