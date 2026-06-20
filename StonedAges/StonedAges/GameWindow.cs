using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Engine;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace StonedAges;

/// <summary>
///     The main client window. Hosts the OpenGL surface, owns the core subsystems
///     (input, textures, sound, and the game-state machine), and drives the game loop.
/// </summary>
public class GameWindow : Form
{
    /// <summary>ShowWindow command: hide the window.</summary>
    public const int SW_HIDE = 0;

    /// <summary>ShowWindow command: show the window.</summary>
    public const int SW_SHOW = 5;

    /// <summary>Number of indexed sound effects (0.wav .. 29.wav) loaded from DarkAges.dat.</summary>
    private const int SoundEffectCount = 30;

    /// <summary>The game databases loaded at startup, in load order.</summary>
    private static readonly string[] JsonDatabases =
    {
        "guildlist", "items", "actions", "dialogue", "maps",
        "monsters", "npcs", "skills", "spells"
    };

    #region Fields

    private IContainer components = null;
    private SimpleOpenGlControl simpleOpenGlControl1;
    private DateTime timerTest = DateTime.MinValue;

    public static int _port = 4020;
    public static Dictionary<string, DateTime> JsonVersTime = new Dictionary<string, DateTime>();
    public static object _sysObject = new object();
    public static ClientSocket ClientSocket = new ClientSocket();
    public static string _clientName = "";
    public static bool ConnectedToServer = false;
    public static DateTime _clientVersion;

    private bool _fullscreen;
    private FastLoop _fastLoop;
    private StateSystem _system = new StateSystem();
    private Input _input = new Input();
    private TextureManager _textureManager = new TextureManager();
    private SoundManager _soundManager;
    private Engine.Font _font;
    private Size _defaultSize = new Size(640, 480);

    public static float xScale = 1f;
    public static float yScale = 1f;
    public static bool newCharCreated = false;
    public static IntPtr _consoleHandle;

    #endregion

    #region Construction / startup

    public GameWindow()
    {
        FileChecks();
        AllocConsole();
        Console.Title = "Client";
        _consoleHandle = GetConsoleWindow();
        InitializeComponent();
        simpleOpenGlControl1.InitializeContexts();
        LoadClientVersion();
        JsonVersionChecks();
        ServerConnect();
        _input.Mouse = new Mouse(this, simpleOpenGlControl1, _textureManager);
        _input.Keyboard = new Keyboard(simpleOpenGlControl1);
        InitializeDisplay();
        InitializeSounds();
        Console.WriteLine("Trying to connect to server...");
        InitializeTextures();
        InitializeFonts();
        InitializeGameState();
        ShowWindow(_consoleHandle, SW_HIDE);
        _fastLoop = new FastLoop(GameLoop);
    }

    /// <summary>
    ///     Verifies the required data folders are present. A missing folder writes
    ///     Warning.txt, opens it, and exits — the client cannot run without its data.
    /// </summary>
    private void FileChecks()
    {
        if (!Directory.Exists("dats"))
        {
            DisplayWarningText("dats folder not found, please download the datsmusic.zip at http://da-items.space/downloads.htm");
        }

        if (!Directory.Exists("music"))
        {
            DisplayWarningText("music folder not found, please download the datsmusic.zip at http://da-items.space/downloads.htm");
        }
        else
        {
            // .mus is the legacy format; the client now plays .ogg. Drop any stale .mus
            // files and confirm the current music pack (identified by 68.ogg) is present.
            bool hasCurrentMusic = false;
            string[] musicFiles = Directory.GetFiles("music");
            foreach (string file in musicFiles)
            {
                if (file.EndsWith(".mus"))
                {
                    File.Delete(file);
                }
                if (file.Contains("68.ogg"))
                {
                    hasCurrentMusic = true;
                }
            }
            if (!hasCurrentMusic)
            {
                DisplayWarningText("Your music files are out of date, please download the datsmusic.zip at http://da-items.space/downloads.htm");
            }
        }

        if (!Directory.Exists("players"))
        {
            Directory.CreateDirectory("players");
        }
    }

    private void DisplayWarningText(string text)
    {
        File.WriteAllText("Warning.txt", text);
        Process.Start("notepad.exe", "Warning.txt");
        Environment.Exit(1);
    }

    private void UpdateClientVersion()
    {
        XDocument doc = new XDocument(new XElement("Version", $"{DateTime.UtcNow:MM.dd.yy-HH:mm:ss}"));
        doc.Save("vers.xml");
    }

    private void LoadClientVersion()
    {
        if (!File.Exists("vers.xml"))
        {
            UpdateClientVersion();
        }
        XDocument doc = XDocument.Load("vers.xml");
        _clientVersion = DateTime.ParseExact(doc.Element("Version").Value, "MM.dd.yy-HH:mm:ss", CultureInfo.InvariantCulture);
    }

    /// <summary>
    ///     Records the last-write time of each JSON database so the client can detect
    ///     when its local data is out of date relative to the server.
    /// </summary>
    private void JsonVersionChecks()
    {
        foreach (string database in JsonDatabases)
        {
            string path = "jsons\\" + database + ".json";
            JsonVersTime.Add(path, File.GetLastWriteTimeUtc(path));
        }
    }

    public void OnExit(bool d = false)
    {
        Environment.Exit(1);
    }

    private void ServerConnect()
    {
        ClientSocket.Connect("stonedages.ddns.net", _port);
    }

    #endregion

    #region Subsystem initialization

    private void InitializeFonts()
    {
        _font = new Engine.Font(_textureManager.Get("font"), FontParser.Parse("font.fnt"));
    }

    private void InitializeGameState()
    {
        GameState gameState = new GameState(this, _font, _textureManager, _soundManager, _input, _system);
        ClientSocket._gamestate = gameState;
        _system.AddState("start_menu", new StartMenuState(_font, _textureManager, _input, _system));
        _system.AddState("create_menu", new CreateMenuState(_font, _textureManager, _input, _system));
        _system.AddState("inner_game", gameState);
        _system.AddState("story", new StoryState(_font, _textureManager, _input, _system));
        _system.AddState("credits", new CreditsState(_font, _textureManager, _input, _system));
        _system.ChangeState("start_menu");
    }

    /// <summary>
    ///     Loads the loose PNG interface textures, then starts background preloading of the
    ///     (much larger) foreground tile set.
    /// </summary>
    private void InitializeTextures()
    {
        // { texture key, source PNG } for the loose interface images.
        string[][] uiTextures =
        {
            new[] { "font", "font_0.png" },
            new[] { "backgrnd", "images\\backgrnd.png" },
            new[] { "group1", "images\\group1.png" },
            new[] { "users01", "images\\users01.png" },
            new[] { "users05", "images\\users05.png" },
            new[] { "equip01", "images\\equip01.png" },
            new[] { "promptok", "images\\promptok.png" },
            new[] { "sense", "images\\sense.png" },
            new[] { "fullinvback", "images\\fullinvback.png" },
            new[] { "fullinv_F1_C0", "images\\fullinv_F1.png" },
            new[] { "fullinv_F2_C0", "images\\fullinv_F2.png" },
            new[] { "panelnums", "images\\panelnums.png" },
            new[] { "stat001", "images\\stat001.png" },
            new[] { "tilehighlight", "images\\tilehighlight.png" },
            new[] { "tiletargethighlight", "images\\tiletargethighlight.png" },
            new[] { "merbot_F2_C0", "images\\merbot_F2.png" },
            new[] { "banktab_F1_C0", "images\\banktab_F1.png" },
            new[] { "banktab_F2_C0", "images\\banktab_F2.png" },
            new[] { "item001_F1_custom_C0", "images\\item001_F1.png" },
            new[] { "item001_F2_custom_C0", "images\\item001_F2.png" },
            new[] { "item001_F3_custom_C0", "images\\item001_F3.png" },
        };
        foreach (string[] entry in uiTextures)
        {
            _textureManager.LoadTexture(entry[0], entry[1]);
        }
        _textureManager.LoadAllEPFS();

        Thread preloadThread = new Thread(PreloadFunction);
        preloadThread.Start();
    }

    private void PreloadFunction()
    {
        Task.Factory.StartNew(delegate
        {
            timerTest = DateTime.UtcNow;
            _textureManager.LoadAllHPFS();
            timerTest = DateTime.MinValue;
        });
    }

    private void InitializeSounds()
    {
        _soundManager = new SoundManager();
        Console.Clear();
        DATArchive darkAgesDat = DATArchive.FromFile("dats\\DarkAges.dat");
        for (int i = 0; i < SoundEffectCount; i++)
        {
            _soundManager.LoadSound("effect" + i, i + ".wav", WAVFile.FromArchive(i + ".wav", darkAgesDat));
        }
    }

    private void InitializeDisplay()
    {
        base.ClientSize = _defaultSize;
        if (_fullscreen)
        {
            base.WindowState = FormWindowState.Normal;
            base.FormBorderStyle = FormBorderStyle.None;
            base.WindowState = FormWindowState.Maximized;
        }
        Setup2DGraphics(base.ClientSize.Width, base.ClientSize.Height);
    }

    #endregion

    #region Game loop / rendering

    private void UpdateInput(double elapsedTime)
    {
        if (base.ContainsFocus)
        {
            if (_input.Keyboard.IsKeyPressed(Keys.F11))
            {
                ToggleFullScreen();
            }
            _input.Update(elapsedTime);
        }
    }

    private void GameLoop(double elapsedTime)
    {
        UpdateInput(elapsedTime);
        _system.Update(elapsedTime);
        _system.Render(elapsedTime);
        simpleOpenGlControl1.Refresh();
    }

    private void ToggleFullScreen()
    {
        if (!_fullscreen)
        {
            _fullscreen = true;
            base.WindowState = FormWindowState.Normal;
            base.FormBorderStyle = FormBorderStyle.None;
            base.WindowState = FormWindowState.Maximized;
        }
        else
        {
            _fullscreen = false;
            base.WindowState = FormWindowState.Normal;
            base.FormBorderStyle = FormBorderStyle.Sizable;
            base.ClientSize = _defaultSize;
        }
    }

    protected override void OnClientSizeChanged(EventArgs e)
    {
        base.OnClientSizeChanged(e);
        Gl.glViewport(0, 0, base.ClientSize.Width, base.ClientSize.Height);
    }

    /// <summary>Configures an orthographic 2-D projection matching the client area.</summary>
    private void Setup2DGraphics(double width, double height)
    {
        Gl.glMatrixMode(Gl.GL_PROJECTION);
        Gl.glLoadIdentity();
        Gl.glOrtho(0.0, width, height, 0.0, 0.0, 1.0);
        Gl.glMatrixMode(Gl.GL_MODELVIEW);
        Gl.glLoadIdentity();
    }

    #endregion

    #region WinForms designer

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.simpleOpenGlControl1 = new Tao.Platform.Windows.SimpleOpenGlControl();
        base.SuspendLayout();
        this.simpleOpenGlControl1.AccumBits = 0;
        this.simpleOpenGlControl1.AutoCheckErrors = false;
        this.simpleOpenGlControl1.AutoFinish = false;
        this.simpleOpenGlControl1.AutoMakeCurrent = true;
        this.simpleOpenGlControl1.AutoSwapBuffers = true;
        this.simpleOpenGlControl1.BackColor = System.Drawing.Color.Black;
        this.simpleOpenGlControl1.ColorBits = 32;
        this.simpleOpenGlControl1.DepthBits = 16;
        this.simpleOpenGlControl1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.simpleOpenGlControl1.Location = new System.Drawing.Point(0, 0);
        this.simpleOpenGlControl1.Name = "simpleOpenGlControl1";
        this.simpleOpenGlControl1.Size = new System.Drawing.Size(640, 480);
        this.simpleOpenGlControl1.StencilBits = 0;
        this.simpleOpenGlControl1.TabIndex = 0;
        base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        base.ClientSize = new System.Drawing.Size(640, 480);
        base.Controls.Add(this.simpleOpenGlControl1);
        base.Name = "GameWindow";
        this.Text = "Stoned Ages";
        base.ResumeLayout(false);
    }

    #endregion

    #region Native methods

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    #endregion
}
