using System.Windows.Forms;
using Engine;
using Tao.OpenGl;

namespace StonedAges;

/// <summary>The lore/story screen. Escape or the button returns to the start menu.</summary>
internal class StoryState : IGameObject
{
    private Sprite _background = new Sprite();

    private Renderer _renderer = new Renderer();

    private Font _font;

    private Input _input;

    private ButtonMenu _menu;

    private StateSystem _system;

    private TextureManager _textureManager;

    public StoryState(Font font, TextureManager textureManager, Input input, StateSystem system)
    {
        _textureManager = textureManager;
        _system = system;
        _input = input;
        _font = font;
        _background.Texture = _textureManager.Get("setup01_F0_C0");
        InitializeMenu();
    }

    public void Update(double elapsedTime)
    {
        if (_input.Keyboard.IsKeyPressed(Keys.Escape))
        {
            _system.ChangeState("start_menu");
        }
        _menu.HandleInput();
    }

    public void Render(double elapsedTime)
    {
        Gl.glClearColor(0f, 0f, 0f, 1f);
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
        _renderer.DrawSprite(_background, 0);
        _menu.Render(_renderer);
        _input.Mouse.Render(_renderer);
        _renderer.Render();
    }

    private void InitializeMenu()
    {
        _menu = new ButtonMenu(_input, _font);
        Engine.Button button = new Engine.Button(_textureManager, 552.0, 438.0, 70.0, 22.0, "");
        button._onPressEvent = delegate
        {
            _system.ChangeState("start_menu");
        };
        _menu.AddButton("closeMenu", button);
    }
}
