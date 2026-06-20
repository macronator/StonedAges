using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Engine;
using Tao.OpenGl;

namespace StonedAges;

internal class StartMenuState : IGameObject
{
    private Sprite _background = new Sprite();

    private Renderer _renderer = new Renderer();

    private Engine.Font _font;

    private Input _input;

    private ButtonMenu _menu;

    private StateSystem _system;

    private TextureManager _textureManager;

    private ButtonMenu _logInPopup;

    private ButtonMenu _passPopup;

    public ButtonMenu _prompt;

    private bool _displayLogInPopup;

    private bool _displayPassPopup;

    public StartMenuState(Engine.Font font, TextureManager textureManager, Input input, StateSystem system)
    {
        _textureManager = textureManager;
        _system = system;
        _input = input;
        _font = font;
        _background.Texture = _textureManager.Get("lod00_F0_C0");
        InitializeMenu();
    }

    public void Update(double elapsedTime)
    {
        if (GameWindow.newCharCreated)
        {
            _prompt._labels["promptText"].ChangeText("Congratulations! Your new character has been created!");
            GameWindow.newCharCreated = false;
        }
        if (_prompt._labels["promptText"]._text != "")
        {
            if (_input.Keyboard.IsKeyPressed(Keys.Escape) || _input.Keyboard.IsKeyPressed(Keys.Space) || _input.Keyboard.IsKeyPressed(Keys.Return))
            {
                _prompt._buttons["promptOkBtn"].OnPress();
            }
            _prompt.HandleInput();
        }
        else if (_displayLogInPopup)
        {
            if (_input.Keyboard.IsKeyPressed(Keys.Escape))
            {
                _logInPopup._textFields["logNameTF"].Text = "";
                _logInPopup._textFields["logPassTF"].Text = "";
                _logInPopup._textFields["logNameTF"].SetFocus();
                _displayLogInPopup = false;
            }
            if (_input.Keyboard.IsKeyPressed(Keys.Return))
            {
                if (_logInPopup._textFields["logNameTF"]._textObj._drawCursor)
                {
                    _logInPopup._textFields["logNameTF"]._textObj.RemoveCursor();
                    _logInPopup._textFields["logPassTF"]._textObj._drawCursor = true;
                    _logInPopup._currentTabIndex = 2;
                }
                else if (_logInPopup._textFields["logPassTF"]._textObj._drawCursor)
                {
                    if (_logInPopup._textFields["logNameTF"].Text.Length >= 3)
                    {
                        _logInPopup._textFields["logPassTF"]._textObj.RemoveCursor();
                        _logInPopup._buttons["logOkBtn"].OnPress();
                    }
                }
                else if (_logInPopup._textFields["logNameTF"].Text.Length >= 3)
                {
                    _logInPopup._buttons["logOkBtn"].OnPress();
                }
            }
            _logInPopup.HandleInput();
        }
        else if (_displayPassPopup)
        {
            if (_input.Keyboard.IsKeyPressed(Keys.Escape))
            {
                if (_passPopup._textFields["passPassTF"]._textObj._drawCursor)
                {
                    _passPopup._textFields["passPassTF"]._textObj.RemoveCursor();
                }
                if (_passPopup._textFields["passNewPassTF"]._textObj._drawCursor)
                {
                    _passPopup._textFields["passNewPassTF"]._textObj.RemoveCursor();
                }
                if (_passPopup._textFields["passConfirmTF"]._textObj._drawCursor)
                {
                    _passPopup._textFields["passConfirmTF"]._textObj.RemoveCursor();
                }
                _displayPassPopup = false;
            }
            _passPopup.HandleInput();
        }
        else
        {
            if (_input.Keyboard.IsKeyPressed(Keys.Escape))
            {
                Application.Exit();
            }
            _menu.HandleInput();
        }
    }

    public void Render(double elapsedTime)
    {
        Gl.glClearColor(0f, 0f, 0f, 1f);
        Gl.glClear(16384);
        _renderer.DrawSprite(_background, 0);
        _menu.Render(_renderer);
        if (_prompt._labels["promptText"]._text != "")
        {
            _prompt.Render(_renderer);
        }
        if (_displayLogInPopup)
        {
            _logInPopup.Render(_renderer);
        }
        else if (_displayPassPopup)
        {
            _passPopup.Render(_renderer);
        }
        _input.Mouse.Render(_renderer);
        _renderer.Render();
    }

    private Text DrawLabel(string text, Engine.Color color, double x, double y, int maxWidth, string align, int lines = 0, bool shade = false, double winPosX = 0.0, double winPosY = 0.0)
    {
        Text text2 = new Text(text, _font, maxWidth, lines);
        text2._windowOffset.X = x;
        text2._windowOffset.Y = y;
        text2._shade = shade;
        text2.SetColor(color);
        text2.Align(align);
        text2.SetPosition(x + winPosX, y + winPosY);
        return text2;
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
        Text value = DrawLabel("", Engine.Color.White, 6.0, 6.0, 250, "left", 5, shade: false, vector.X, vector.Y);
        _prompt._labels.Add("promptText", value);
        _prompt._buttons.Add("promptOkBtn", button);
        _menu = new ButtonMenu(_input, _font);
        Engine.Button button2 = new Engine.Button(_textureManager, 457.0, 321.0, 97.0, 21.0, "", "menubtn_F0", "menubtn_F0");
        Engine.Button button3 = new Engine.Button(_textureManager, 457.0, 343.0, 97.0, 21.0, "", "menubtn_F1", "menubtn_F1");
        Engine.Button button4 = new Engine.Button(_textureManager, 457.0, 365.0, 97.0, 21.0, "", "menubtn_F2", "menubtn_F2");
        Engine.Button button5 = new Engine.Button(_textureManager, 457.0, 387.0, 97.0, 21.0, "", "menubtn_F3", "menubtn_F3");
        Engine.Button button6 = new Engine.Button(_textureManager, 457.0, 409.0, 97.0, 21.0, "", "menubtn_F4", "menubtn_F4");
        Engine.Button button7 = new Engine.Button(_textureManager, 457.0, 431.0, 97.0, 21.0, "", "menubtn_F5", "menubtn_F5");
        button2._onPressEvent = delegate
        {
            _logInPopup._textFields["logNameTF"].Text = "";
            _logInPopup._textFields["logPassTF"].Text = "";
            _logInPopup._textFields["logNameTF"].SetFocus();
            _system.ChangeState("create_menu");
        };
        button3._onPressEvent = delegate
        {
            _logInPopup.ClearButtonFocus();
            _logInPopup.ClearTextFields();
            _logInPopup._currentTabIndex = 1;
            _logInPopup.SetFocus(_logInPopup._textFields["logNameTF"]);
            _displayLogInPopup = true;
        };
        button4._onPressEvent = delegate
        {
            _passPopup.ClearButtonFocus();
            _passPopup.ClearTextFields();
            _passPopup._currentTabIndex = 1;
            _passPopup.SetFocus(_passPopup._textFields["passNameTF"]);
            _displayPassPopup = true;
        };
        button5._onPressEvent = delegate
        {
            _system.ChangeState("story");
        };
        button6._onPressEvent = delegate
        {
            _system.ChangeState("credits");
        };
        button7._onPressEvent = delegate
        {
            Application.Exit();
        };
        _menu.AddButton("create", button2);
        _menu.AddButton("startGame", button3);
        _menu.AddButton("changePass", button4);
        _menu.AddButton("story", button5);
        _menu.AddButton("credits", button6);
        _menu.AddButton("exitGame", button7);
        vector = new Vector(446.0, 322.0, 0.0);
        _logInPopup = new ButtonMenu(_input, _font, vector.X, vector.Y, 116.0, 158.0);
        _logInPopup._background.SetHeight(158f);
        _logInPopup._background.SetWidth(116f);
        _logInPopup._background.SetColor(Engine.Color.Black);
        _logInPopup._background.SetPosition(vector);
        _logInPopup._sprite.Texture = _textureManager.Get("dlglogin_F0_C0");
        _logInPopup._sprite.SetPosition(vector);
        TextField logNameTF = new TextField(new Rectangle(3, 17, 110, 13), _font, "", 17, vector.X, vector.Y);
        logNameTF._tabIndex = 1;
        _logInPopup._currentTabIndex = 1;
        logNameTF._alphaonly = true;
        logNameTF._ignorespace = true;
        logNameTF._textObj.SetColor(Engine.Color.White);
        TextField logPassTF = new TextField(new Rectangle(3, 47, 110, 13), _font, "", 17, vector.X, vector.Y);
        logPassTF._tabIndex = 2;
        logPassTF._alphaonly = true;
        logPassTF._ignorespace = true;
        logPassTF._textObj.SetColor(Engine.Color.White);
        Engine.Button button8 = new Engine.Button(_textureManager, 2.0, 134.0, 27.0, 16.0, "menuok_F0", "menuok_F1", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button8._tabIndex = 3;
        button8._onPressEvent = delegate
        {
            string text = logNameTF.Text;
            string pass = "";
            if (text.Count() >= 3)
            {
                bool flag = false;
                string path = "players\\" + text.ToLower() + "\\" + text.ToLower() + ".txt";
                if (File.Exists(path))
                {
                    flag = true;
                }
                if (flag)
                {
                    logNameTF.Text = "";
                    logPassTF.Text = "";
                    logNameTF.SetFocus();
                    GameWindow._clientName = text;
                    if (GameWindow.ConnectedToServer)
                    {
                        LoginPacket loginPacket = new LoginPacket(text, pass);
                        GameWindow.ClientSocket.Send(loginPacket.Data);
                    }
                    GameState._initPlayer = true;
                    _system.ChangeState("inner_game");
                    _displayLogInPopup = false;
                }
                else
                {
                    logPassTF.Text = "";
                    logNameTF.Text = "";
                    logNameTF.SetFocus();
                    _prompt._labels["promptText"].ChangeText("That character does not exist.");
                }
            }
        };
        Engine.Button button9 = new Engine.Button(_textureManager, 44.0, 134.0, 69.0, 16.0, "menucncl_F0", "menucncl_F1", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button9._onPressEvent = delegate
        {
            logNameTF.Text = "";
            logPassTF.Text = "";
            logNameTF.SetFocus();
            _displayLogInPopup = false;
        };
        _logInPopup._textFields.Add("logNameTF", logNameTF);
        _logInPopup._textFields.Add("logPassTF", logPassTF);
        _logInPopup._buttons.Add("logOkBtn", button8);
        _logInPopup._buttons.Add("logCancelBtn", button9);
        _logInPopup._maxTabIndex = 3;
        vector = new Vector(439.0, 315.0, 0.0);
        _passPopup = new ButtonMenu(_input, _font, vector.X, vector.Y, 132.0, 164.0);
        _passPopup._background.SetHeight(164f);
        _passPopup._background.SetWidth(132f);
        _passPopup._background.SetColor(Engine.Color.Black);
        _passPopup._background.SetPosition(vector);
        _passPopup._sprite.Texture = _textureManager.Get("dlgpass_F0_C0");
        _passPopup._sprite.SetPosition(vector);
        TextField passNameTF = new TextField(new Rectangle(10, 24, 110, 13), _font, "", 17, vector.X, vector.Y);
        passNameTF._tabIndex = 1;
        _passPopup._currentTabIndex = 1;
        passNameTF._alphaonly = true;
        passNameTF._ignorespace = true;
        passNameTF._textObj.SetColor(Engine.Color.White);
        TextField textField = new TextField(new Rectangle(10, 54, 110, 13), _font, "", 17, vector.X, vector.Y);
        textField._tabIndex = 2;
        textField._alphaonly = true;
        textField._ignorespace = true;
        textField._textObj.SetColor(Engine.Color.White);
        TextField textField2 = new TextField(new Rectangle(10, 84, 110, 13), _font, "", 17, vector.X, vector.Y);
        textField2._tabIndex = 3;
        textField2._alphaonly = true;
        textField2._ignorespace = true;
        textField2._textObj.SetColor(Engine.Color.White);
        TextField textField3 = new TextField(new Rectangle(10, 114, 110, 13), _font, "", 17, vector.X, vector.Y);
        textField3._tabIndex = 4;
        textField3._alphaonly = true;
        textField3._ignorespace = true;
        textField3._textObj.SetColor(Engine.Color.White);
        Engine.Button button10 = new Engine.Button(_textureManager, 9.0, 141.0, 27.0, 16.0, "menuok_F0", "menuok_F1", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button10._tabIndex = 5;
        button10._onPressEvent = delegate
        {
            if (passNameTF.Text.Length >= 3)
            {
                _displayPassPopup = false;
            }
        };
        Engine.Button button11 = new Engine.Button(_textureManager, 51.0, 141.0, 69.0, 16.0, "menucncl_F0", "menucncl_F1", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button11._onPressEvent = delegate
        {
            _displayPassPopup = false;
        };
        _passPopup._textFields.Add("passNameTF", passNameTF);
        _passPopup._textFields.Add("passPassTF", textField);
        _passPopup._textFields.Add("passNewPassTF", textField2);
        _passPopup._textFields.Add("passConfirmTF", textField3);
        _passPopup._buttons.Add("passOkBtn", button10);
        _passPopup._buttons.Add("passCancelBtn", button11);
        _passPopup._maxTabIndex = 5;
    }
}
