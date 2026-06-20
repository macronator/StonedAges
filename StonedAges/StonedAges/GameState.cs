using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using Engine;
using IrrKlang;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tao.OpenGl;

namespace StonedAges;

public class GameState : IGameObject
{
    private const int moveFrames = 2;

    private const double _xConst = 28.0;

    private const double _yConst = 14.0;

    public List<DisplayPlayerPacket> displayPlayerPackets = new List<DisplayPlayerPacket>();

    public List<BodyMovementPacket> bodyMovementPackets = new List<BodyMovementPacket>();

    public List<SpellAnimationPacket> spellAnimationPackets = new List<SpellAnimationPacket>();

    public List<ProjectilePacket> projectilePackets = new List<ProjectilePacket>();

    public List<DisplayProfilePacket> displayProfilePackets = new List<DisplayProfilePacket>();

    public List<RemoveEntityPacket> removeEntityPackets = new List<RemoveEntityPacket>();

    public List<LocationPacket> locationPackets = new List<LocationPacket>();

    public List<DisplayEntitiesPacket> displayEntitiesPackets = new List<DisplayEntitiesPacket>();

    public List<DisplayBoardPacket> displayBoardPackets = new List<DisplayBoardPacket>();

    public List<MessagePacket> messagePackets = new List<MessagePacket>();

    public Dictionary<ushort, BoardS> personalBoards = new Dictionary<ushort, BoardS>();

    public Dictionary<ushort, PostS> mailPosts = new Dictionary<ushort, PostS>();

    public Dictionary<ushort, PostS> boardPosts = new Dictionary<ushort, PostS>();

    public List<int> wallaninums = new List<int>();

    private Sprite _loadingBarSprite = new Sprite();

    private Entity _autoAttackEntity;

    private Engine.Point dDP = new Engine.Point(187f, 303f);

    public static List<UserS> _userList = new List<UserS>();

    private Sprite _blindedSprite = new Sprite();

    private Sprite _blackScreen = new Sprite();

    private int _soundNodeIndex = 50;

    private int _musicNodeIndex = 50;

    private ISound curTrack;

    private byte _infoBarMenuIndex;

    private List<GroupMemberS> GroupList = new List<GroupMemberS>();

    private DateTime lastArrowKeyPressDT = DateTime.MinValue;

    private byte lastArrowKeyDirection = byte.MaxValue;

    private List<Text> commandBodyText = new List<Text>();

    private List<Text> keyboardBodyText = new List<Text>();

    private bool debug;

    private byte castCount;

    private DateTime lastSpellUse = DateTime.MinValue;

    private bool _userMsgPrompt;

    private byte _curFlag;

    private bool _viewingGroupList;

    private bool _useGroupWindow = true;

    private bool _showGroupRequestPrompt;

    private List<string> _friendList = new List<string>
    {
        "", "", "", "", "", "", "", "", "", "",
        "", "", "", "", "", "", "", "", "", ""
    };

    private List<string> _ignoreList = new List<string>();

    private bool _enableExchange = true;

    private bool _allowGrouping = true;

    private bool _listenToNPCs = true;

    private bool _enableSpells = true;

    private bool _listenToWhispers = true;

    private bool _listenToGuild = true;

    private bool _listenToShouts = true;

    private bool _enableWeather;

    private bool _portraitFlap;

    private List<string> _whisperList = new List<string>();

    private int _whisperListIndex;

    public static bool _initPlayer = false;

    private PlayerDataS pdata = default(PlayerDataS);

    private int _feetShuffleDelay = 70;

    private double _origMoveAniDelay = 50.0;

    private double origKeyboardMoveDelay = 70.0;

    private Pathfind _pfa;

    private Spell _loadedSpell;

    private Dictionary<string, string> questvars = new Dictionary<string, string>();

    private bool dyeingequipment;

    private int selectedequipmentdyeslot;

    private int selectedequipmentdye;

    private int _depositItemDialogIndex;

    private int _withdrawItemDialogIndex;

    private int _withdrawItemDialogCatIndex;

    private string _selectedWithdrawDialogCat = "";

    private int _dyehairDialogIndex;

    private int _hairstyleDialogIndex;

    private int _frgtActionDialogIndex;

    private int _frgtSpellDialogIndex;

    private int _frgtSkillDialogIndex;

    private int _lrnActionDialogIndex;

    private int _lrnSpellDialogIndex;

    private int _lrnSkillDialogIndex;

    private string _selectedBuyDialogCat = "";

    private int _buyDialogCatIndex;

    private int _buyDialogIndex;

    private int _sellDialogIndex;

    private int _dyeDialogIndex;

    private int _dye2DialogIndex;

    private Random rand = new Random();

    private Dictionary<int, Map> _maps = new Dictionary<int, Map>();

    private TabMap _tabMap = new TabMap();

    public Map _map = new Map();

    private static Player _player;

    private Player _profilePlayer;

    private ScrollingBackground _weatherEffect;

    private Sprite _loadingUserPopup = new Sprite();

    private Sprite _worldMapSprite = new Sprite();

    private Sprite _spellBarBack = new Sprite();

    private Sprite _healthOrb = new Sprite();

    private Sprite _manaOrb = new Sprite();

    private Sprite _fullInvPanel = new Sprite();

    private Sprite _toolTipBack = new Sprite();

    private Sprite _entityToolTipBack = new Sprite();

    private Renderer _renderer = new Renderer();

    private Engine.Font _font;

    private Input _input;

    private StateSystem _system;

    private TextureManager _textureManager;

    private SoundManager _soundManager;

    private Text _toolTipText;

    private string _tTT = "";

    private Text _entityToolTip;

    private string _eTT = "";

    private DragIcon _dragIcon;

    private DateTime _upperLeftMsgTimer = DateTime.MinValue;

    private DateTime _assailCD = DateTime.UtcNow;

    private DateTime _itemUseKeysDelay = DateTime.UtcNow;

    private double _itemCDConst = 333.0;

    private DateTime _movementKeysDelay = DateTime.UtcNow;

    private DateTime _chatKeysDelay = DateTime.UtcNow;

    private bool _moving;

    private double _moveAniDelay;

    private int _moveAniCount = -1;

    private double _xOffset;

    private double _yOffset;

    private int _playerXOffset;

    private int _playerYOffset;

    private int _panelNum = 3;

    private double _infoBarDelay;

    private double _origInfoBarDelay = 15000.0;

    private int _othersLegendIndex;

    private int _legendIndex;

    private int _userIndex;

    private bool _viewKeyboards;

    private int _commandIndex;

    private int _keyboardIndex;

    private int _infoBarIndex;

    private string _infoBarText = "";

    private List<string> _infoBarList = new List<string>();

    private static int _chatIndex = 0;

    private string _userMsg = "";

    private static List<string> _chatList = new List<string>();

    private bool _chatMode;

    private List<Slot> _slots = new List<Slot>();

    private List<Slot> _inventorySlots = new List<Slot>();

    private List<Slot> _equipmentSlots = new List<Slot>();

    private List<InventoryItem> _equipment = new List<InventoryItem>();

    private List<InventoryItem> _inventory = new List<InventoryItem>();

    private int _playerbankedgold;

    private Dictionary<string, BankItem> _playerbank = new Dictionary<string, BankItem>();

    private List<Skill> _skills = new List<Skill>();

    private List<Spell> _spells = new List<Spell>();

    private List<Action> _actions = new List<Action>();

    private Dictionary<string, NPC> NPCDict = new Dictionary<string, NPC>();

    private bool _worldMap;

    private bool _viewCommands;

    private bool _viewingOthersProfile;

    private bool _viewingOthersLegend;

    private bool _viewingProfile;

    private bool _viewingLegend;

    private bool _viewingPersonalBoard;

    private bool _viewingMailList;

    private bool _viewingBoardList;

    private bool _viewingPost;

    private bool _composingPost;

    private bool _viewingSense;

    private bool _viewingSign;

    private bool _viewingDialog;

    private bool _viewingOptions;

    private bool _viewingSettings;

    private bool _viewingFriends;

    private bool _viewingMacros;

    private bool _viewingUsers;

    private List<string> wallaniarr = new List<string>();

    public static JObject _mapsDB;

    public static JObject _npcsDB;

    public static JObject _monstersDB;

    public static JObject _itemsDB;

    public static JObject _skillsDB;

    public static JObject _spellsDB;

    public static JObject _actionsDB;

    public static JObject _dialogDB;

    public static JObject _guildsDB;

    private ButtonMenu _deletePrompt;

    private ButtonMenu _prompt;

    private ButtonMenu _groupListMenu;

    private ButtonMenu _groupRequestPrompt;

    private ButtonMenu _infoBarMenu;

    private ButtonMenu _commandMenu;

    private ButtonMenu _keyboardMenu;

    private ButtonMenu _miscMenu;

    private ButtonMenu _skillMenu;

    private ButtonMenu _spellMenu;

    private ButtonMenu _chatMenu;

    private ButtonMenu _statMenu;

    private ButtonMenu _actionMenu;

    private ButtonMenu _infoMenu;

    private ButtonMenu _inventoryMenu;

    private ButtonMenu _panelMenu;

    private ButtonMenu _menu;

    private ButtonMenu _optionsMenu;

    private ButtonMenu _friendsMenu;

    private ButtonMenu _macroMenu;

    private ButtonMenu _settingsMenu;

    private ButtonMenu _boardMenu;

    private ButtonMenu _mailListMenu;

    private ButtonMenu _boardListMenu;

    private ButtonMenu _composePostMenu;

    private ButtonMenu _viewPostMenu;

    private ButtonMenu _usersMenu;

    private ButtonMenu _othersProfileMenu;

    private ButtonMenu _othersLegendMenu;

    private ButtonMenu _profileMenu;

    private ButtonMenu _legendMenu;

    private ButtonMenu _senseMenu;

    private ButtonMenu _signMenu;

    private ButtonMenu _standardDialogPopup;

    private int _debugSound;

    private int _debugMusic = 1;

    private int _debugMonImg = 1;

    private int _debugSpellAni = 1;

    private string _debugMonSource = "old";

    private bool _GM;

    private GameWindow _gw;

    private int weaponDmg
    {
        get
        {
            InventoryItem equipmentSlot = getEquipmentSlot(9);
            if (equipmentSlot != null && equipmentSlot._weaponDmg.Contains("m"))
            {
                string[] array = equipmentSlot._weaponDmg.Split('m');
                return rand.Next(int.Parse(array[0]), int.Parse(array[1]) + 1);
            }
            return 0;
        }
    }

    public string ClientName => _player._name;

    public uint ClientID
    {
        get
        {
            return _player._id;
        }
        set
        {
            _player._id = value;
        }
    }

    public GameState(GameWindow gw, Engine.Font font, TextureManager textureManager, SoundManager soundManager, Input input, StateSystem system)
    {
        _soundManager = soundManager;
        _gw = gw;
        _feetShuffleDelay = (int)(2.0 * _origMoveAniDelay / 5.0 * 2.5);
        LoadClientSettings();
        _equipmentSlots.Add(new Slot(1, 121.0, 114.0));
        _equipmentSlots.Add(new Slot(2, 186.0, 126.0));
        _equipmentSlots.Add(new Slot(3, 210.0, 153.0));
        _equipmentSlots.Add(new Slot(4, 94.0, 196.0));
        _equipmentSlots.Add(new Slot(5, 122.0, 209.0));
        _equipmentSlots.Add(new Slot(6, 152.0, 194.0));
        _equipmentSlots.Add(new Slot(7, 122.0, 235.0));
        _equipmentSlots.Add(new Slot(8, 165.0, 256.0));
        _equipmentSlots.Add(new Slot(9, 76.0, 153.0));
        _equipmentSlots.Add(new Slot(10, 153.0, 92.0));
        _equipmentSlots.Add(new Slot(11, 143.0, 153.0));
        _equipmentSlots.Add(new Slot(12, 209.0, 196.0));
        _equipmentSlots.Add(new Slot(13, 184.0, 217.0));
        _equipmentSlots.Add(new Slot(14, 218.0, 92.0));
        _inventorySlots.Add(new Slot(1, 94.0, 336.0));
        _inventorySlots.Add(new Slot(2, 130.0, 336.0));
        _inventorySlots.Add(new Slot(3, 166.0, 336.0));
        _inventorySlots.Add(new Slot(4, 202.0, 336.0));
        _inventorySlots.Add(new Slot(5, 238.0, 336.0));
        _inventorySlots.Add(new Slot(6, 274.0, 336.0));
        _inventorySlots.Add(new Slot(7, 310.0, 336.0));
        _inventorySlots.Add(new Slot(8, 346.0, 336.0));
        _inventorySlots.Add(new Slot(9, 382.0, 336.0));
        _inventorySlots.Add(new Slot(10, 418.0, 336.0));
        _inventorySlots.Add(new Slot(11, 454.0, 336.0));
        _inventorySlots.Add(new Slot(12, 490.0, 336.0));
        _inventorySlots.Add(new Slot(13, 94.0, 370.0));
        _inventorySlots.Add(new Slot(14, 130.0, 370.0));
        _inventorySlots.Add(new Slot(15, 166.0, 370.0));
        _inventorySlots.Add(new Slot(16, 202.0, 370.0));
        _inventorySlots.Add(new Slot(17, 238.0, 370.0));
        _inventorySlots.Add(new Slot(18, 274.0, 370.0));
        _inventorySlots.Add(new Slot(19, 310.0, 370.0));
        _inventorySlots.Add(new Slot(20, 346.0, 370.0));
        _inventorySlots.Add(new Slot(21, 382.0, 370.0));
        _inventorySlots.Add(new Slot(22, 418.0, 370.0));
        _inventorySlots.Add(new Slot(23, 454.0, 370.0));
        _inventorySlots.Add(new Slot(24, 490.0, 370.0));
        _inventorySlots.Add(new Slot(25, 94.0, 404.0));
        _inventorySlots.Add(new Slot(26, 130.0, 404.0));
        _inventorySlots.Add(new Slot(27, 166.0, 404.0));
        _inventorySlots.Add(new Slot(28, 202.0, 404.0));
        _inventorySlots.Add(new Slot(29, 238.0, 404.0));
        _inventorySlots.Add(new Slot(30, 274.0, 404.0));
        _inventorySlots.Add(new Slot(31, 310.0, 404.0));
        _inventorySlots.Add(new Slot(32, 346.0, 404.0));
        _inventorySlots.Add(new Slot(33, 382.0, 404.0));
        _inventorySlots.Add(new Slot(34, 418.0, 404.0));
        _inventorySlots.Add(new Slot(35, 454.0, 404.0));
        _inventorySlots.Add(new Slot(36, 490.0, 404.0));
        _inventorySlots.Add(new Slot(37, 94.0, 336.0));
        _inventorySlots.Add(new Slot(38, 130.0, 336.0));
        _inventorySlots.Add(new Slot(39, 166.0, 336.0));
        _inventorySlots.Add(new Slot(40, 202.0, 336.0));
        _inventorySlots.Add(new Slot(41, 238.0, 336.0));
        _inventorySlots.Add(new Slot(42, 274.0, 336.0));
        _inventorySlots.Add(new Slot(43, 310.0, 336.0));
        _inventorySlots.Add(new Slot(44, 346.0, 336.0));
        _inventorySlots.Add(new Slot(45, 382.0, 336.0));
        _inventorySlots.Add(new Slot(46, 418.0, 336.0));
        _inventorySlots.Add(new Slot(47, 454.0, 336.0));
        _inventorySlots.Add(new Slot(48, 490.0, 336.0));
        _inventorySlots.Add(new Slot(49, 94.0, 370.0));
        _inventorySlots.Add(new Slot(50, 130.0, 370.0));
        _inventorySlots.Add(new Slot(51, 166.0, 370.0));
        _inventorySlots.Add(new Slot(52, 202.0, 370.0));
        _inventorySlots.Add(new Slot(53, 238.0, 370.0));
        _inventorySlots.Add(new Slot(54, 274.0, 370.0));
        _inventorySlots.Add(new Slot(55, 310.0, 370.0));
        _inventorySlots.Add(new Slot(56, 346.0, 370.0));
        _inventorySlots.Add(new Slot(57, 382.0, 370.0));
        _inventorySlots.Add(new Slot(58, 418.0, 370.0));
        _inventorySlots.Add(new Slot(59, 454.0, 370.0));
        _inventorySlots.Add(new Slot(60, 490.0, 370.0));
        _inventorySlots.Add(new Slot(61, 94.0, 404.0));
        _inventorySlots.Add(new Slot(62, 130.0, 404.0));
        _inventorySlots.Add(new Slot(63, 166.0, 404.0));
        _inventorySlots.Add(new Slot(64, 202.0, 404.0));
        _inventorySlots.Add(new Slot(65, 238.0, 404.0));
        _inventorySlots.Add(new Slot(66, 274.0, 404.0));
        _inventorySlots.Add(new Slot(67, 310.0, 404.0));
        _inventorySlots.Add(new Slot(68, 346.0, 404.0));
        _inventorySlots.Add(new Slot(69, 382.0, 404.0));
        _inventorySlots.Add(new Slot(70, 418.0, 404.0));
        _inventorySlots.Add(new Slot(71, 454.0, 404.0));
        _inventorySlots.Add(new Slot(72, 490.0, 404.0));
        _slots.Add(new Slot(1, 94.0, 336.0));
        _slots.Add(new Slot(2, 130.0, 336.0));
        _slots.Add(new Slot(3, 166.0, 336.0));
        _slots.Add(new Slot(4, 202.0, 336.0));
        _slots.Add(new Slot(5, 238.0, 336.0));
        _slots.Add(new Slot(6, 274.0, 336.0));
        _slots.Add(new Slot(7, 310.0, 336.0));
        _slots.Add(new Slot(8, 346.0, 336.0));
        _slots.Add(new Slot(9, 382.0, 336.0));
        _slots.Add(new Slot(10, 418.0, 336.0));
        _slots.Add(new Slot(11, 454.0, 336.0));
        _slots.Add(new Slot(12, 490.0, 336.0));
        _slots.Add(new Slot(13, 94.0, 370.0));
        _slots.Add(new Slot(14, 130.0, 370.0));
        _slots.Add(new Slot(15, 166.0, 370.0));
        _slots.Add(new Slot(16, 202.0, 370.0));
        _slots.Add(new Slot(17, 238.0, 370.0));
        _slots.Add(new Slot(18, 274.0, 370.0));
        _slots.Add(new Slot(19, 310.0, 370.0));
        _slots.Add(new Slot(20, 346.0, 370.0));
        _slots.Add(new Slot(21, 382.0, 370.0));
        _slots.Add(new Slot(22, 418.0, 370.0));
        _slots.Add(new Slot(23, 454.0, 370.0));
        _slots.Add(new Slot(24, 490.0, 370.0));
        _slots.Add(new Slot(25, 94.0, 404.0));
        _slots.Add(new Slot(26, 130.0, 404.0));
        _slots.Add(new Slot(27, 166.0, 404.0));
        _slots.Add(new Slot(28, 202.0, 404.0));
        _slots.Add(new Slot(29, 238.0, 404.0));
        _slots.Add(new Slot(30, 274.0, 404.0));
        _slots.Add(new Slot(31, 310.0, 404.0));
        _slots.Add(new Slot(32, 346.0, 404.0));
        _slots.Add(new Slot(33, 382.0, 404.0));
        _slots.Add(new Slot(34, 418.0, 404.0));
        _slots.Add(new Slot(35, 454.0, 404.0));
        _slots.Add(new Slot(36, 490.0, 404.0));
        _slots.Add(new Slot(37, 94.0, 336.0));
        _slots.Add(new Slot(38, 130.0, 336.0));
        _slots.Add(new Slot(39, 166.0, 336.0));
        _slots.Add(new Slot(40, 202.0, 336.0));
        _slots.Add(new Slot(41, 238.0, 336.0));
        _slots.Add(new Slot(42, 274.0, 336.0));
        _slots.Add(new Slot(43, 310.0, 336.0));
        _slots.Add(new Slot(44, 346.0, 336.0));
        _slots.Add(new Slot(45, 382.0, 336.0));
        _slots.Add(new Slot(46, 418.0, 336.0));
        _slots.Add(new Slot(47, 454.0, 336.0));
        _slots.Add(new Slot(48, 490.0, 336.0));
        _slots.Add(new Slot(49, 94.0, 370.0));
        _slots.Add(new Slot(50, 130.0, 370.0));
        _slots.Add(new Slot(51, 166.0, 370.0));
        _slots.Add(new Slot(52, 202.0, 370.0));
        _slots.Add(new Slot(53, 238.0, 370.0));
        _slots.Add(new Slot(54, 274.0, 370.0));
        _slots.Add(new Slot(55, 310.0, 370.0));
        _slots.Add(new Slot(56, 346.0, 370.0));
        _slots.Add(new Slot(57, 382.0, 370.0));
        _slots.Add(new Slot(58, 418.0, 370.0));
        _slots.Add(new Slot(59, 454.0, 370.0));
        _slots.Add(new Slot(60, 490.0, 370.0));
        _slots.Add(new Slot(61, 94.0, 404.0));
        _slots.Add(new Slot(62, 130.0, 404.0));
        _slots.Add(new Slot(63, 166.0, 404.0));
        _slots.Add(new Slot(64, 202.0, 404.0));
        _slots.Add(new Slot(65, 238.0, 404.0));
        _slots.Add(new Slot(66, 274.0, 404.0));
        _slots.Add(new Slot(67, 310.0, 404.0));
        _slots.Add(new Slot(68, 346.0, 404.0));
        _slots.Add(new Slot(69, 382.0, 404.0));
        _slots.Add(new Slot(70, 418.0, 404.0));
        _slots.Add(new Slot(71, 454.0, 404.0));
        _slots.Add(new Slot(72, 490.0, 404.0));
        _textureManager = textureManager;
        _system = system;
        _input = input;
        _font = font;
        using (StreamReader streamReader = new StreamReader("dats\\stcani.tbl"))
        {
            string text;
            while ((text = streamReader.ReadLine()) != null)
            {
                string[] array = text.Split();
                for (int i = 0; i < array.Count() - 1; i++)
                {
                    wallaninums.Add(int.Parse(array[i]));
                }
                wallaniarr.Add(text);
            }
        }
        using (StreamReader streamReader2 = new StreamReader("jsons\\maps.json"))
        {
            _mapsDB = JObject.Parse(streamReader2.ReadToEnd());
        }
        using (StreamReader streamReader3 = new StreamReader("jsons\\npcs.json"))
        {
            _npcsDB = JObject.Parse(streamReader3.ReadToEnd());
        }
        using (StreamReader streamReader4 = new StreamReader("jsons\\monsters.json"))
        {
            _monstersDB = JObject.Parse(streamReader4.ReadToEnd());
        }
        using (StreamReader streamReader5 = new StreamReader("jsons\\items.json"))
        {
            _itemsDB = JObject.Parse(streamReader5.ReadToEnd());
        }
        using (StreamReader streamReader6 = new StreamReader("jsons\\skills.json"))
        {
            _skillsDB = JObject.Parse(streamReader6.ReadToEnd());
        }
        using (StreamReader streamReader7 = new StreamReader("jsons\\spells.json"))
        {
            _spellsDB = JObject.Parse(streamReader7.ReadToEnd());
        }
        using (StreamReader streamReader8 = new StreamReader("jsons\\actions.json"))
        {
            _actionsDB = JObject.Parse(streamReader8.ReadToEnd());
        }
        using (StreamReader streamReader9 = new StreamReader("jsons\\dialogue.json"))
        {
            _dialogDB = JObject.Parse(streamReader9.ReadToEnd());
        }
        using (StreamReader streamReader10 = new StreamReader("jsons\\guildlist.json"))
        {
            _guildsDB = JObject.Parse(streamReader10.ReadToEnd());
        }
        PlayMusicFile("music\\" + rand.Next(1, 27) + ".ogg");
        _blackScreen.Texture = DrawBlackScreen();
        _blindedSprite.Texture = _textureManager.Get("mask001_F0_C0");
        _blindedSprite.SetPosition(255.0, 153.0);
        _loadingUserPopup.Texture = _textureManager.Get("lodusr_F0_C0");
        _loadingUserPopup.SetPosition(155.0, 135.0);
        _loadingBarSprite.SetPosition(185.0, 170.0);
        _loadingBarSprite.SetHeight(11f);
        _loadingBarSprite.SetColor(Engine.Color.White);
        _worldMapSprite.Texture = _textureManager.Get("field001_F0_C0");
        _spellBarBack.Texture = _textureManager.Get("spelled_F0_C0");
        _spellBarBack.SetPosition(617.0, 0.0);
        _toolTipBack.SetColor(new Engine.Color(0f, 0f, 0f, 0.5f));
        _entityToolTipBack.SetColor(new Engine.Color(0f, 0f, 0f, 0.5f));
        _weatherEffect = new ScrollingBackground(textureManager.Get("snow01_F0_C0"));
        _weatherEffect.Direction = new Vector(-0.4, -1.0, 0.0);
        _weatherEffect.Speed = 0.1f;
        _player = new Player(this, _textureManager, _font, "", new Location(0, 0), _map, "", 0);
        NewItem("Gold", 72, 0);
        InitializeMenu();
        SlashCommands();
        Hotkeys();
        _toolTipText = DrawLabel("", Engine.Color.White, 0.0, 0.0, 146, "left");
        _entityToolTip = DrawLabel("", Engine.Color.White, 0.0, 0.0, 146, "left");
        _dragIcon = new DragIcon(_input, new Sprite());
    }

    /// <summary>Writes client settings (volumes, F4 chat/group toggles, friends, ignore list) to ClientSettings.xml.</summary>
    private void SaveClientSettings()
    {
        XDocument doc = new XDocument(new XElement("Settings", new XElement("MusicVolume", _musicNodeIndex), new XElement("SoundVolume", _soundNodeIndex), new XElement("PortraitFlap", _portraitFlap), new XElement("F4Settings", new XElement("ListenToWhisper", _listenToWhispers), new XElement("ListenToShout", _listenToShouts), new XElement("ListenToGuild", _listenToGuild), new XElement("RecordNPCs", _listenToNPCs), new XElement("JoinAGroup", _allowGrouping), new XElement("SpellAnimations", _enableSpells), new XElement("WeatherEffects", _enableWeather), new XElement("Exchange", _enableExchange), new XElement("GroupWindow", _useGroupWindow)), new XElement("Friends", from x in _friendList
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     where x != ""
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     select new XElement("Name", x)), new XElement("Ignore", _ignoreList.Select((string x) => new XElement("Name", x)))));
        doc.Save("ClientSettings.xml");
    }

    /// <summary>Loads client settings from ClientSettings.xml (creating defaults if missing): volumes, F4 chat/group toggles, friends, ignore list.</summary>
    private void LoadClientSettings()
    {
        if (!File.Exists("ClientSettings.xml"))
        {
            SaveClientSettings();
        }
        XDocument doc = XDocument.Load("ClientSettings.xml");
        foreach (XElement element in doc.Root.Elements())
        {
            if (element.Name.ToString() == "MusicVolume")
            {
                _musicNodeIndex = int.Parse(element.Value);
            }
            else if (element.Name.ToString() == "SoundVolume")
            {
                _soundNodeIndex = int.Parse(element.Value);
            }
            else if (element.Name.ToString() == "PortraitFlap")
            {
                _portraitFlap = bool.Parse(element.Value);
            }
            else if (element.Name.ToString() == "F4Settings")
            {
                foreach (XElement f4Element in element.Elements())
                {
                    if (f4Element.Name.ToString() == "ListenToWhisper")
                    {
                        _listenToWhispers = bool.Parse(f4Element.Value);
                    }
                    else if (f4Element.Name.ToString() == "ListenToShout")
                    {
                        _listenToShouts = bool.Parse(f4Element.Value);
                    }
                    else if (f4Element.Name.ToString() == "ListenToGuild")
                    {
                        _listenToGuild = bool.Parse(f4Element.Value);
                    }
                    else if (f4Element.Name.ToString() == "RecordNPCs")
                    {
                        _listenToNPCs = bool.Parse(f4Element.Value);
                    }
                    else if (f4Element.Name.ToString() == "JoinAGroup")
                    {
                        _allowGrouping = bool.Parse(f4Element.Value);
                    }
                    else if (f4Element.Name.ToString() == "SpellAnimations")
                    {
                        _enableSpells = bool.Parse(f4Element.Value);
                    }
                    else if (f4Element.Name.ToString() == "WeatherEffects")
                    {
                        _enableWeather = bool.Parse(f4Element.Value);
                    }
                    else if (f4Element.Name.ToString() == "Exchange")
                    {
                        _enableExchange = bool.Parse(f4Element.Value);
                    }
                    else if (f4Element.Name.ToString() == "GroupWindow")
                    {
                        _useGroupWindow = bool.Parse(f4Element.Value);
                    }
                }
            }
            else if (element.Name.ToString() == "Friends")
            {
                int friendIndex = 0;
                foreach (XElement friendElement in element.Elements())
                {
                    _friendList[friendIndex] = friendElement.Value;
                    friendIndex++;
                }
            }
            else
            {
                if (!(element.Name.ToString() == "Ignore"))
                {
                    continue;
                }
                foreach (XElement ignoreElement in element.Elements())
                {
                    _ignoreList.Add(ignoreElement.Value);
                }
            }
        }
    }

    /// <summary>
    ///     The per-frame game tick. First drains the queued server packets (chat, board, player/entity
    ///     display, location, movement, spell animation, projectile, profile, remove) onto game state,
    ///     then advances the world (map/weather/spawns), handles any modal prompt or the world map,
    ///     processes menu/panel/window input, refreshes the HUD labels, runs movement + mouse-hover
    ///     targeting/tooltips (slots, tiles, doors/chests, entities), and finally delegates to UpdateInput.
    /// </summary>
    public void Update(double elapsedTime)
    {
        if (_initPlayer)
        {
            InitializePlayer();
        }
        // --- Drain queued server packets onto game state (the receive thread enqueues; we apply here) ---
        MessagePacket[] pendingMessages = messagePackets.ToArray();
        foreach (MessagePacket messagePacket in pendingMessages)
        {
            DisplayChat(messagePacket.MsgType, messagePacket.ID, messagePacket.Message);
            messagePackets.Remove(messagePacket);
        }
        DisplayBoardPacket[] pendingBoards = displayBoardPackets.ToArray();
        foreach (DisplayBoardPacket displayBoardPacket in pendingBoards)
        {
            UpdateBoardDisplay(displayBoardPacket.Board);
            displayBoardPackets.Remove(displayBoardPacket);
        }
        DisplayPlayerPacket[] pendingPlayers = displayPlayerPackets.ToArray();
        foreach (DisplayPlayerPacket displayPlayerPacket in pendingPlayers)
        {
            UpdatePlayerDisplay(displayPlayerPacket.Player);
            displayPlayerPackets.Remove(displayPlayerPacket);
        }
        DisplayEntitiesPacket[] pendingEntities = displayEntitiesPackets.ToArray();
        foreach (DisplayEntitiesPacket displayEntitiesPacket in pendingEntities)
        {
            UpdateEntitiesDisplay(displayEntitiesPacket.Entities);
            displayEntitiesPackets.Remove(displayEntitiesPacket);
        }
        LocationPacket[] pendingLocations = locationPackets.ToArray();
        foreach (LocationPacket locationPacket in pendingLocations)
        {
            Location(locationPacket.ID, locationPacket.Location, locationPacket.Direction);
            locationPackets.Remove(locationPacket);
        }
        BodyMovementPacket[] pendingMoves = bodyMovementPackets.ToArray();
        foreach (BodyMovementPacket bodyMovementPacket in pendingMoves)
        {
            if (bodyMovementPacket.AniType == 3 || bodyMovementPacket.AniType == 4 || bodyMovementPacket.AniType > 54)
            {
                EmoteP(bodyMovementPacket.ID, bodyMovementPacket.AniType, bodyMovementPacket.Speed);
            }
            else
            {
                BodyMoveP(bodyMovementPacket.ID, bodyMovementPacket.AniType, bodyMovementPacket.Speed);
            }
            bodyMovementPackets.Remove(bodyMovementPacket);
        }
        SpellAnimationPacket[] pendingSpellAnims = spellAnimationPackets.ToArray();
        foreach (SpellAnimationPacket spellAnimationPacket in pendingSpellAnims)
        {
            SpellAnimationP(spellAnimationPacket.SpellAni);
            spellAnimationPackets.Remove(spellAnimationPacket);
        }
        ProjectilePacket[] pendingProjectiles = projectilePackets.ToArray();
        foreach (ProjectilePacket projectilePacket in pendingProjectiles)
        {
            Projectile(projectilePacket.ID, projectilePacket.TYPE);
            projectilePackets.Remove(projectilePacket);
        }
        DisplayProfilePacket[] pendingProfiles = displayProfilePackets.ToArray();
        foreach (DisplayProfilePacket displayProfilePacket in pendingProfiles)
        {
            DisplayProfile(displayProfilePacket.Profile);
            displayProfilePackets.Remove(displayProfilePacket);
        }
        RemoveEntityPacket[] pendingRemoves = removeEntityPackets.ToArray();
        foreach (RemoveEntityPacket removeEntityPacket in pendingRemoves)
        {
            RemoveEntityP(removeEntityPacket.ID);
            removeEntityPackets.Remove(removeEntityPacket);
        }
        // --- Advance the world: map, weather, monster spawns, NPC tile effects, cast-cooldown reset ---
        _map.Update(elapsedTime);
        _weatherEffect.Update((float)elapsedTime);
        Engine.Point position = _input.Mouse.Position;
        if (_map != null)
        {
            KeyValuePair<string, Spawn>[] array11 = _map._spawns.ToArray();
            for (int i = 0; i < array11.Length; i++)
            {
                KeyValuePair<string, Spawn> keyValuePair = array11[i];
                if (MonsterCount(keyValuePair.Key, _map._number) < keyValuePair.Value.SpawnCap && DateTime.UtcNow.Subtract(keyValuePair.Value.LastDeath).TotalMilliseconds > 10000.0)
                {
                    SpawnMonster(_map._number, keyValuePair.Key);
                }
            }
        }
        if (DateTime.UtcNow.Subtract(lastSpellUse).TotalMilliseconds >= 1000.0)
        {
            castCount = 0;
            lastSpellUse = DateTime.UtcNow;
        }
        NPC[] array12 = NPC.List.Values.ToArray();
        foreach (NPC nPC in array12)
        {
            if (nPC != null)
            {
                Tile tile = TileImFacing(nPC);
                if (tile != null && !tile._spellanimating)
                {
                    tile.SpellAni(96, 80);
                }
            }
        }
        array12 = NPCDict.Values.ToArray();
        foreach (NPC nPC2 in array12)
        {
            if (nPC2 != null)
            {
                Tile tile2 = TileImFacing(nPC2);
                if (tile2 != null && !tile2._spellanimating)
                {
                    tile2.SpellAni(96, 80);
                }
            }
        }
        // --- Modal prompts: while one is open, consume input and return early ---
        if (_deletePrompt._labels["delpromptText"]._text != "")
        {
            if (_input.Keyboard.IsKeyPressed(Keys.Escape))
            {
                _deletePrompt._buttons["delpromptCancelBtn"].OnPress();
            }
            if (_input.Keyboard.IsKeyPressed(Keys.Space) || _input.Keyboard.IsKeyPressed(Keys.Return))
            {
                _deletePrompt._buttons["delpromptDeleteBtn"].OnPress();
            }
            _deletePrompt.HandleInput();
            return;
        }
        if (_prompt._labels["promptText"]._text != "")
        {
            if (_input.Keyboard.IsKeyPressed(Keys.Escape) || _input.Keyboard.IsKeyPressed(Keys.Space) || _input.Keyboard.IsKeyPressed(Keys.Return))
            {
                _prompt._buttons["promptOkBtn"].OnPress();
            }
            _prompt.HandleInput();
            return;
        }
        // --- World-map screen ---
        if (_worldMap)
        {
            Town[] array13 = Town._List.Values.ToArray();
            foreach (Town town in array13)
            {
                if (town.CollidesWith(position) && _input.Mouse.LeftPressed)
                {
                    NewMap(1001, 5, 5);
                    NewTown(town);
                    Town._List.Clear();
                    _worldMap = false;
                }
            }
            if (_input.Keyboard.IsKeyPressed(Keys.Escape) && _worldMap)
            {
                Town._List.Clear();
                _worldMap = false;
            }
            return;
        }
        // --- Menu / panel / window input + per-frame skill/spell/action updates ---
        if (_input.Mouse.LeftPressed && _eTT != "")
        {
            _eTT = "";
        }
        if (!_viewingStuff())
        {
            _menu.HandleInput();
            _panelMenu.HandleInput();
            _miscMenu.HandleInput();
        }
        foreach (Skill skill in _skills)
        {
            skill.Update(elapsedTime);
        }
        foreach (Spell spell in _spells)
        {
            spell.Update(elapsedTime);
        }
        foreach (Action action in _actions)
        {
            action.Update(elapsedTime);
        }
        if (!_viewingStuff())
        {
            if (_viewingLegend)
            {
                _legendMenu.HandleInput();
            }
            else if (_viewingProfile)
            {
                _profileMenu.HandleInput();
            }
            else if (_viewingOthersLegend)
            {
                _othersLegendMenu.HandleInput();
            }
            else if (_viewingOthersProfile)
            {
                _othersProfileMenu.HandleInput();
            }
        }
        if (_viewingGroupList)
        {
            _groupListMenu.HandleInput();
        }
        if (_showGroupRequestPrompt)
        {
            _groupRequestPrompt.HandleInput();
        }
        if (!_miscMenu._buttons["fullInvBtn"].Selected)
        {
            _infoBarMenu.HandleInput();
        }
        if (_viewCommands)
        {
            _commandMenu.HandleInput();
        }
        if (_viewKeyboards)
        {
            _keyboardMenu.HandleInput();
        }
        if (_viewingSign)
        {
            _signMenu.HandleInput();
        }
        else if (_viewingSense)
        {
            _senseMenu.HandleInput();
        }
        if (_viewingDialog)
        {
            _standardDialogPopup.HandleInput();
            if (_standardDialogPopup._disTextFields.ContainsKey("inputTF"))
            {
                if (_standardDialogPopup._disTextFields["inputTF"].Text == "")
                {
                    _standardDialogPopup._buttons["sdpNextBtn"].Enabled = false;
                }
                else
                {
                    _standardDialogPopup._buttons["sdpNextBtn"].Enabled = true;
                }
            }
        }
        if (_player._spellBar.Count > 0)
        {
            _player._spellBar.Values.ToList().ForEach(delegate (SpellBar s)
            {
                s.Update(elapsedTime);
            });
        }
        // --- Refresh the HUD: orbs, info bar, name/location/stat labels, group/users lists ---
        UpdateHPMPOrbs();
        ClearInfoBar(elapsedTime);
        _miscMenu._labels["nameLabel"].ChangeText(_player._name);
        _miscMenu._labels["infoLabel"].ChangeText(_infoBarText, colorize: true);
        if (_upperLeftMsgTimer != DateTime.MinValue)
        {
            if (DateTime.UtcNow.Subtract(_upperLeftMsgTimer).TotalSeconds < 5.0)
            {
                _miscMenu._labels["upperLeft1Label"].ChangeText(_infoBarList[_infoBarList.LastIndexOf(_infoBarList.Last()) - 2], colorize: true);
                _miscMenu._labels["upperLeft2Label"].ChangeText(_infoBarList[_infoBarList.LastIndexOf(_infoBarList.Last()) - 1], colorize: true);
                _miscMenu._labels["upperLeft3Label"].ChangeText(_infoBarList.Last(), colorize: true);
            }
            else if (DateTime.UtcNow.Subtract(_upperLeftMsgTimer).TotalSeconds >= 5.0)
            {
                _upperLeftMsgTimer = DateTime.MinValue;
                _miscMenu._labels["upperLeft1Label"].ChangeText("");
                _miscMenu._labels["upperLeft2Label"].ChangeText("");
                _miscMenu._labels["upperLeft3Label"].ChangeText("");
            }
        }
        if (!_chatMode)
        {
            if (_panelNum == 4 && _player._availstats > 0)
            {
                _miscMenu._labels["userMsgLabel"].ChangeText("Level Up Point : " + _player._availstats);
            }
            else
            {
                _miscMenu._labels["userMsgLabel"].ChangeText(_userMsg);
            }
        }
        else if (_miscMenu._labels["userMsgLabel"]._text != "")
        {
            _miscMenu._labels["userMsgLabel"].ChangeText("");
        }
        if (_panelNum == 3)
        {
            string tmp2 = "";
            int count2 = 0;
            int endCount2 = _chatIndex + 8;
            _chatList.ForEach(delegate (string x)
            {
                if (count2 >= _chatIndex && count2 < endCount2)
                {
                    tmp2 = tmp2 + x + "\n";
                }
                count2++;
            });
            _chatMenu._labels["chatPanel"].ChangeText(tmp2, colorize: true);
        }
        if (_panelNum == 8)
        {
            string tmp1 = "";
            int count = 0;
            int endCount = _infoBarIndex + 8;
            _infoBarList.ForEach(delegate (string x)
            {
                if (count >= _infoBarIndex && count < endCount)
                {
                    tmp1 = tmp1 + x + "\n";
                }
                count++;
            });
            _infoMenu._labels["infoPanel"].ChangeText(tmp1, colorize: true);
        }
        if (_viewingOthersProfile && _profilePlayer != null)
        {
            _othersProfileMenu._labels["othersprofileNameLabel"].ChangeText(_profilePlayer._name);
            string newtext = "Master";
            if (!_profilePlayer._master)
            {
                newtext = "Lv. " + _profilePlayer._lev;
            }
            _othersProfileMenu._labels["othersprofileLevelLabel"].ChangeText(newtext);
            _othersProfileMenu._labels["othersprofileGuildLabel"].ChangeText(_profilePlayer._guild);
            _othersProfileMenu._labels["othersprofileRankLabel"].ChangeText(_profilePlayer._rank);
            _othersProfileMenu._labels["othersprofileTitleLabel"].ChangeText(_profilePlayer._title);
        }
        if (_viewingOthersLegend && _profilePlayer != null)
        {
            _othersLegendMenu._labels["otherslegendNameLabel"].ChangeText(_profilePlayer._name);
        }
        if (_viewingProfile)
        {
            _profileMenu._labels["profileNameLabel"].ChangeText(_player._name);
            string newtext2 = "Master";
            if (!_player._master)
            {
                newtext2 = "Lv. " + _player._lev;
            }
            _profileMenu._labels["profileLevelLabel"].ChangeText(newtext2);
            _profileMenu._labels["profileGuildLabel"].ChangeText(_player._guild);
            _profileMenu._labels["profileRankLabel"].ChangeText(_player._rank);
            _profileMenu._labels["profileTitleLabel"].ChangeText(_player._title);
        }
        if (_viewingLegend)
        {
            _legendMenu._labels["legendNameLabel"].ChangeText(_player._name);
        }
        if (_viewingGroupList)
        {
            for (int l = 1; l <= 10; l++)
            {
                if (GroupList.Count() > l - 1)
                {
                    _groupListMenu._labels["groupName" + l + "Lab"].ChangeText(GroupList[l - 1].Name);
                    _groupListMenu._labels["groupMap" + l + "Lab"].ChangeText(GroupList[l - 1].X + "," + GroupList[l - 1].Y + " - " + GroupList[l - 1].Map);
                    if (GroupList[l - 1].Leader)
                    {
                        _groupListMenu._labels["groupName" + l + "Lab"].SetColor(Engine.Color.White);
                        _groupListMenu._buttons["groupQuit" + l + "Btn"].Enabled = true;
                    }
                    else
                    {
                        _groupListMenu._labels["groupName" + l + "Lab"].SetColor(Engine.Color.Gray2);
                        _groupListMenu._buttons["groupQuit" + l + "Btn"].Enabled = false;
                    }
                }
                else
                {
                    _groupListMenu._labels["groupName" + l + "Lab"].ChangeText("");
                    _groupListMenu._labels["groupMap" + l + "Lab"].ChangeText("");
                    _groupListMenu._buttons["groupQuit" + l + "Btn"].Enabled = false;
                }
            }
        }
        _miscMenu._labels["mapLabel"].ChangeText(_map._name);
        _miscMenu._labels["locationLabel"].ChangeText("XY: " + _player._location.X + ", " + _player._location.Y);
        if (_panelNum == 4)
        {
            _statMenu._labels["strLabel"].ChangeText(_player._str.ToString());
            _statMenu._labels["intLabel"].ChangeText(_player._int.ToString());
            _statMenu._labels["wisLabel"].ChangeText(_player._wis.ToString());
            _statMenu._labels["conLabel"].ChangeText(_player._con.ToString());
            _statMenu._labels["dexLabel"].ChangeText(_player._dex.ToString());
            _statMenu._labels["curHpLabel"].ChangeText(_player._curHP.ToString("#,0"));
            _statMenu._labels["maxHpLabel"].ChangeText(_player._maxHP.ToString("#,0"));
            _statMenu._labels["curMpLabel"].ChangeText(_player._curMP.ToString("#,0"));
            _statMenu._labels["maxMpLabel"].ChangeText(_player._maxMP.ToString("#,0"));
            _statMenu._labels["expLabel"].ChangeText(_player._exp.ToString("#,0"));
            _statMenu._labels["goldLabel"].ChangeText(getItemSlot(72)._amount.ToString("#,0"));
            _statMenu._labels["levLabel"].ChangeText(_player._lev.ToString());
            _statMenu._labels["tnlLabel"].ChangeText(_player._tnl.ToString("#,0"));
            _statMenu._labels["atkLabel"].ChangeText(_player._atk);
            _statMenu._labels["defLabel"].ChangeText(_player._def);
            _statMenu._labels["mrLabel"].ChangeText(_player._mr.ToString());
            if (_player._ac < 0)
            {
                _statMenu._labels["acLabel"].ChangeText("0");
            }
            else
            {
                _statMenu._labels["acLabel"].ChangeText(_player._ac.ToString());
            }
            _statMenu._labels["dmgLabel"].ChangeText(_player._dmg.ToString());
            _statMenu._labels["hitLabel"].ChangeText(_player._hit.ToString());
        }
        if (_optionsMenu._buttons["soundBtn"].Selected)
        {
            _optionsMenu._labels["keyboardLabel"].ChangeText("sound " + _debugSound);
        }
        else if (_optionsMenu._buttons["musicBtn"].Selected)
        {
            _optionsMenu._labels["keyboardLabel"].ChangeText("music " + _debugMusic);
        }
        else if (_player._inMonsterForm && _menu._buttons["optionsBtn"].Selected)
        {
            _optionsMenu._labels["keyboardLabel"].ChangeText(_debugMonImg.ToString());
        }
        else if (_menu._buttons["optionsBtn"].Selected)
        {
            _optionsMenu._labels["keyboardLabel"].ChangeText(_debugSpellAni.ToString());
        }
        else
        {
            _optionsMenu._labels["keyboardLabel"].ChangeText("");
        }
        if (_viewingUsers)
        {
            string ntmp = "";
            string ttmp = "";
            _userList.ForEach(delegate (UserS x)
            {
                string text = "";
                if (_friendList.Contains(x.Name, StringComparer.CurrentCultureIgnoreCase))
                {
                    text = "{=q";
                }
                else if (x.Color == 1)
                {
                    text = "{=s";
                }
                else if (x.Color == 2)
                {
                    text = "{=a";
                }
                if (_usersMenu._buttons["countryBtn"].Selected)
                {
                    ntmp = ntmp + text + x.Name + " \n";
                    ttmp = ttmp + text + x.Title + " \n";
                }
                else if (_usersMenu._buttons["masterBtn"].Selected && x.IsMaster)
                {
                    ntmp = ntmp + text + x.Name + " \n";
                    ttmp = ttmp + text + x.Title + " \n";
                }
                else if (_usersMenu._buttons["guildBtn"].Selected && x.InGuild)
                {
                    ntmp = ntmp + text + x.Name + " \n";
                    ttmp = ttmp + text + x.Title + " \n";
                }
                else if (_usersMenu._buttons["friendUBtn"].Selected && _friendList.Contains(x.Name, StringComparer.CurrentCultureIgnoreCase))
                {
                    ntmp = ntmp + text + x.Name + " \n";
                    ttmp = ttmp + text + x.Title + " \n";
                }
            });
            _usersMenu._labels["namesLabel"].ChangeText(ntmp, colorize: true);
            _usersMenu._labels["titlesLabel"].ChangeText(ttmp, colorize: true);
            _usersMenu._labels["worldLabel"].ChangeText("Temuair");
            _usersMenu._labels["totalLabel"].ChangeText(_userList.Count.ToString());
            _usersMenu._labels["masterLabel"].ChangeText(PlayerCount("master").ToString());
            _usersMenu._labels["warriorLabel"].ChangeText(PlayerCount("warrior").ToString());
            _usersMenu._labels["rogueLabel"].ChangeText(PlayerCount("rogue").ToString());
            _usersMenu._labels["wizardLabel"].ChangeText(PlayerCount("wizard").ToString());
            _usersMenu._labels["priestLabel"].ChangeText(PlayerCount("priest").ToString());
            _usersMenu._labels["monkLabel"].ChangeText(PlayerCount("monk").ToString());
            _usersMenu._labels["peasantLabel"].ChangeText(PlayerCount("peasant").ToString());
            _usersMenu._labels["guildLabel"].ChangeText(PlayerCount("guild").ToString());
            _usersMenu._labels["friendLabel"].ChangeText(PlayerCount("friend").ToString());
        }
        _miscMenu._buttons["portraitFlap"].Update(elapsedTime);
        _statMenu.Update(elapsedTime);
        if (_panelMenu._buttons["inventoryBtn"].Selected)
        {
            _inventoryMenu.HandleInput();
        }
        if (_panelMenu._buttons["skillBtn"].Selected)
        {
            _skillMenu.HandleInput();
        }
        if (_panelMenu._buttons["spellBtn"].Selected)
        {
            _spellMenu.HandleInput();
        }
        if (_panelNum == 3)
        {
            _chatMenu.HandleInput();
        }
        if (_panelMenu._buttons["statBtn"].Selected)
        {
            _statMenu.HandleInput();
        }
        if (_panelMenu._buttons["actionBtn"].Selected)
        {
            _actionMenu.HandleInput();
        }
        if (_panelNum == 8)
        {
            _infoMenu.HandleInput();
        }
        if (_viewingOptions)
        {
            _optionsMenu.HandleInput();
        }
        if (_viewingFriends)
        {
            _friendsMenu.HandleInput();
        }
        if (_viewingMacros)
        {
            _macroMenu.HandleInput();
        }
        if (_viewingSettings)
        {
            _settingsMenu.HandleInput();
        }
        if (_composingPost)
        {
            if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Return))
            {
                if (_composePostMenu._textFields["composeToName"]._textObj._drawCursor)
                {
                    _composePostMenu._textFields["composeToName"]._textObj.RemoveCursor();
                    _composePostMenu._textFields["composeTitle"].SetFocus();
                    _composePostMenu._currentTabIndex = 2;
                }
                else if (_composePostMenu._textFields["composeTitle"]._textObj._drawCursor)
                {
                    _composePostMenu._textFields["composeTitle"]._textObj.RemoveCursor();
                    _composePostMenu._textFields["composeBody"].SetFocus();
                    _composePostMenu._currentTabIndex = 3;
                }
            }
            _composePostMenu.HandleInput();
        }
        else if (_viewingPost)
        {
            _viewPostMenu.HandleInput();
        }
        else if (_viewingMailList)
        {
            _mailListMenu.HandleInput();
        }
        else if (_viewingBoardList)
        {
            _boardListMenu.HandleInput();
        }
        else if (_viewingPersonalBoard)
        {
            _boardMenu.HandleInput();
        }
        if (_viewingUsers)
        {
            _usersMenu.HandleInput();
        }
        // --- Player movement, then mouse-hover targeting/tooltips over slots, tiles, objects, entities ---
        Move(elapsedTime);
        bool flag = false;
        if (!_chatMode && !_viewingStuff() && !_viewingLegend && !_infoBarMenu._buttons["sysmsgBtn"].Held)
        {
            _tTT = "";
            int xYSlot = getXYSlot((int)position.X, (int)position.Y);
            if (xYSlot != 0 && !CollidesWithProfiles(position))
            {
                if (_panelNum == 0)
                {
                    InventoryItem itemSlot = getItemSlot(xYSlot);
                    if (itemSlot != null)
                    {
                        flag = true;
                        ItemToolTip(itemSlot, position);
                    }
                }
                if (_panelNum == 1 || _panelNum == 6)
                {
                    Skill skillSlot = getSkillSlot(xYSlot);
                    if (skillSlot != null)
                    {
                        flag = true;
                        _tTT = skillSlot._name;
                        _toolTipText.ChangeText(_tTT);
                        UpdateToolTip(position);
                        _input.Mouse.SetCursorSelected();
                    }
                }
                if (_panelNum == 2 || _panelNum == 7)
                {
                    Spell spellSlot = getSpellSlot(xYSlot);
                    if (spellSlot != null)
                    {
                        flag = true;
                        _tTT = spellSlot._name;
                        _toolTipText.ChangeText(_tTT);
                        UpdateToolTip(position);
                        _input.Mouse.SetCursorSelected();
                    }
                }
                if (_panelNum == 5 || _panelNum == 10)
                {
                    Action actionSlot = getActionSlot(xYSlot);
                    if (actionSlot != null)
                    {
                        flag = true;
                        _tTT = actionSlot._name;
                        _toolTipText.ChangeText(_tTT);
                        UpdateToolTip(position);
                        _input.Mouse.SetCursorSelected();
                    }
                }
            }
            else
            {
                foreach (Tile tile7 in _map._tiles)
                {
                    if (tile7.CollidesWith((int)position.X, (int)position.Y))
                    {
                        if (debug && _input.Mouse.LeftPressed)
                        {
                            if (tile7._wall != null)
                            {
                                SystemMsg("Tile " + tile7.Location.X + "," + tile7.Location.Y + " - Floor: " + tile7._floor + " - LWall: " + tile7._wall._lwall + " - RWall: " + tile7._wall._rwall, 3);
                            }
                            else
                            {
                                SystemMsg("Tile " + tile7.Location.X + "," + tile7.Location.Y + " - Floor: " + tile7._floor, 3);
                            }
                        }
                        if (_loadedSpell != null && _loadedSpell._targettype == "tile")
                        {
                            if (_input.Mouse.LeftPressed)
                            {
                                CastLoadedSpellOnTile(tile7);
                            }
                            tile7._highlightSprite.Texture = _textureManager.Get("tiletargethighlight");
                        }
                        else
                        {
                            tile7._highlightSprite.Texture = _textureManager.Get("tilehighlight");
                        }
                        tile7._highlight = true;
                    }
                    else if (tile7._highlight)
                    {
                        tile7._highlight = false;
                    }
                }
                if (!_viewingLegend && !_viewingProfile && !_viewingOthersLegend && !_viewingOthersProfile)
                {
                    // --- Interactive world objects: doors (open/close + walls), chests, signs, boards ---
                    IWO[] array14 = _map._iwos.ToArray();
                    foreach (IWO iWO in array14)
                    {
                        if (!iWO._tile.Location.InView(_player._location) || !iWO.CollidesWith(position) || !(iWO._type != "Chest"))
                        {
                            continue;
                        }
                        if (_input.Mouse.LeftDown)
                        {
                            iWO._clicked = true;
                        }
                        flag = true;
                        _input.Mouse.SetCursorSelected();
                        if (!_input.Mouse.LeftPressed || !iWO._clicked)
                        {
                            continue;
                        }
                        if (iWO._enabled)
                        {
                            iWO._enabled = false;
                            List<int> list = new List<int>();
                            int rwall = iWO._tile._wall._rwall;
                            int lwall = iWO._tile._wall._lwall;
                            if (list.Contains(rwall) || _textureManager.openDoors(rwall))
                            {
                                int num = _textureManager.AltDoor(rwall);
                                iWO._tile._wall._rwall = num;
                                iWO._tile._wall._rightWall.Texture = _textureManager.Get("stc" + num.ToString("00000") + ".hpf_F0_C0", ".hpf");
                                if (iWO._type == "DoorE")
                                {
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X + 1]);
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X - 1]);
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y + 1) * (int)_map._width + iWO._location.X]);
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y - 1) * (int)_map._width + iWO._location.X]);
                                }
                                else if (iWO._type == "Door")
                                {
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y - 1) * (int)_map._width + iWO._location.X]);
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X - 1]);
                                }
                                else if (iWO._type == "DoorA")
                                {
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X - 1]);
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y - 1) * (int)_map._width + iWO._location.X]);
                                }
                                else if (iWO._type == "DoorB" || iWO._type == "Chest")
                                {
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X + 1]);
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y + 1) * (int)_map._width + iWO._location.X]);
                                    if (num == 924 || num == 921)
                                    {
                                        Tile tile3 = _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X];
                                        tile3._wall.SetPosition(tile3._position.X, tile3._position.Y);
                                    }
                                }
                                else if (iWO._type == "DoorC")
                                {
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X - 1]);
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X + 1]);
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y + 1) * (int)_map._width + iWO._location.X]);
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y - 1) * (int)_map._width + iWO._location.X]);
                                }
                                else if (iWO._type == "JailDoor")
                                {
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X + 1]);
                                }
                                if (iWO._type == "Door" || iWO._type == "DoorC" || iWO._type == "DoorD" || iWO._type == "DoorE" || iWO._type == "DoorA" || iWO._type == "DoorB" || iWO._type == "JailDoor")
                                {
                                    iWO._tile._walkable = false;
                                    SystemMsg("Door closed.", 3);
                                }
                            }
                            if (list.Contains(lwall) || _textureManager.openDoors(lwall))
                            {
                                int num2 = _textureManager.AltDoor(lwall);
                                iWO._tile._wall._lwall = num2;
                                iWO._tile._wall._leftWall.Texture = _textureManager.Get("stc" + num2.ToString("00000") + ".hpf_F0_C0", ".hpf");
                                if (iWO._type == "DoorE")
                                {
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X + 1]);
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X - 1]);
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y + 1) * (int)_map._width + iWO._location.X]);
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y - 1) * (int)_map._width + iWO._location.X]);
                                }
                                else if (iWO._type == "Door")
                                {
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y - 1) * (int)_map._width + iWO._location.X]);
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X - 1]);
                                }
                                else if (iWO._type == "DoorA")
                                {
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X - 1]);
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y - 1) * (int)_map._width + iWO._location.X]);
                                }
                                else if (iWO._type == "DoorB" || iWO._type == "Chest")
                                {
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X + 1]);
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y + 1) * (int)_map._width + iWO._location.X]);
                                    if (num2 == 924 || num2 == 921)
                                    {
                                        Tile tile4 = _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X];
                                        tile4._wall.SetPosition(tile4._position.X, tile4._position.Y);
                                    }
                                }
                                else if (iWO._type == "DoorC")
                                {
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X - 1]);
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X + 1]);
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y + 1) * (int)_map._width + iWO._location.X]);
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y - 1) * (int)_map._width + iWO._location.X]);
                                }
                                else if (iWO._type == "JailDoor")
                                {
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X + 1]);
                                }
                                if (iWO._type == "Door" || iWO._type == "DoorC" || iWO._type == "DoorD" || iWO._type == "DoorE" || iWO._type == "DoorA" || iWO._type == "DoorB" || iWO._type == "JailDoor")
                                {
                                    iWO._tile._walkable = false;
                                    SystemMsg("Door closed.", 3);
                                }
                            }
                        }
                        else
                        {
                            iWO._enabled = true;
                            List<int> list2 = new List<int>();
                            int rwall2 = iWO._tile._wall._rwall;
                            int lwall2 = iWO._tile._wall._lwall;
                            if (list2.Contains(rwall2) || _textureManager.closedDoors(rwall2))
                            {
                                int num3 = _textureManager.AltDoor(rwall2);
                                iWO._tile._wall._rwall = num3;
                                iWO._tile._wall._rightWall.Texture = _textureManager.Get("stc" + num3.ToString("00000") + ".hpf_F0_C0", ".hpf");
                                if (iWO._type == "DoorE")
                                {
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X + 1], walkable: true);
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X - 1], walkable: true);
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y + 1) * (int)_map._width + iWO._location.X], walkable: true);
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y - 1) * (int)_map._width + iWO._location.X], walkable: true);
                                }
                                else if (iWO._type == "Door")
                                {
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y - 1) * (int)_map._width + iWO._location.X], walkable: true);
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X - 1], walkable: true);
                                }
                                else if (iWO._type == "DoorA")
                                {
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X - 1], walkable: true);
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y - 1) * (int)_map._width + iWO._location.X], walkable: true);
                                }
                                else if (iWO._type == "DoorB" || iWO._type == "Chest")
                                {
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X + 1], walkable: true);
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y + 1) * (int)_map._width + iWO._location.X], walkable: true);
                                    if (num3 == 924 || num3 == 921)
                                    {
                                        Tile tile5 = _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X];
                                        tile5._wall.SetPosition(tile5._position.X, tile5._position.Y);
                                    }
                                }
                                else if (iWO._type == "DoorC")
                                {
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X - 1], walkable: true);
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X + 1], walkable: true);
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y + 1) * (int)_map._width + iWO._location.X], walkable: true);
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y - 1) * (int)_map._width + iWO._location.X], walkable: true);
                                }
                                else if (iWO._type == "JailDoor")
                                {
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X + 1]);
                                }
                                if (iWO._type == "Door" || iWO._type == "DoorC" || iWO._type == "DoorD" || iWO._type == "DoorE" || iWO._type == "DoorA" || iWO._type == "DoorB" || iWO._type == "JailDoor")
                                {
                                    iWO._tile._walkable = true;
                                    SystemMsg("Door opened.", 3);
                                }
                            }
                            if (list2.Contains(lwall2) || _textureManager.closedDoors(lwall2))
                            {
                                int num4 = _textureManager.AltDoor(lwall2);
                                iWO._tile._wall._lwall = num4;
                                iWO._tile._wall._leftWall.Texture = _textureManager.Get("stc" + num4.ToString("00000") + ".hpf_F0_C0", ".hpf");
                                if (iWO._type == "DoorE")
                                {
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X + 1], walkable: true);
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X - 1], walkable: true);
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y + 1) * (int)_map._width + iWO._location.X], walkable: true);
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y - 1) * (int)_map._width + iWO._location.X], walkable: true);
                                }
                                else if (iWO._type == "Door")
                                {
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y - 1) * (int)_map._width + iWO._location.X], walkable: true);
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X - 1], walkable: true);
                                }
                                else if (iWO._type == "DoorA")
                                {
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X - 1], walkable: true);
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y - 1) * (int)_map._width + iWO._location.X], walkable: true);
                                }
                                else if (iWO._type == "DoorB" || iWO._type == "Chest")
                                {
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X + 1], walkable: true);
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y + 1) * (int)_map._width + iWO._location.X], walkable: true);
                                    if (num4 == 924 || num4 == 921)
                                    {
                                        Tile tile6 = _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X];
                                        tile6._wall.SetPosition(tile6._position.X, tile6._position.Y);
                                    }
                                }
                                else if (iWO._type == "DoorC")
                                {
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X - 1], walkable: true);
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X + 1], walkable: true);
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y + 1) * (int)_map._width + iWO._location.X], walkable: true);
                                    UpdateAltDoors(elapsedTime, _map._tiles[(iWO._location.Y - 1) * (int)_map._width + iWO._location.X], walkable: true);
                                }
                                else if (iWO._type == "JailDoor")
                                {
                                    UpdateAltDoors(elapsedTime, _map._tiles[iWO._location.Y * (int)_map._width + iWO._location.X + 1]);
                                }
                                if (iWO._type == "Door" || iWO._type == "DoorC" || iWO._type == "DoorD" || iWO._type == "DoorE" || iWO._type == "DoorA" || iWO._type == "DoorB" || iWO._type == "JailDoor")
                                {
                                    iWO._tile._walkable = true;
                                    SystemMsg("Door opened.", 3);
                                }
                            }
                        }
                        if (iWO._type == "Sign" && !_viewingSign)
                        {
                            _signMenu._labels["signLabel"].ChangeText(iWO._text);
                            _viewingSign = true;
                        }
                        if (iWO._type == "Board" && !_viewingBoardList)
                        {
                            RequestBoard(iWO._id, 2, 0, 0);
                            _viewingBoardList = true;
                        }
                        iWO._clicked = false;
                    }
                }
                SpellBar[] array15 = _player._spellBar.Values.ToArray();
                foreach (SpellBar spellBar in array15)
                {
                    if (spellBar.CollidesWith(position))
                    {
                        spellBar._hover = true;
                    }
                    else if (spellBar._hover)
                    {
                        spellBar._hover = false;
                    }
                }
                // --- Hover/click entities: name tags, tooltips, targeting, cast, NPC dialog, profile ---
                foreach (Entity item in from z in _map._entities.Values.ToArray()
                                        orderby z._tileTime
                                        select z)
                {
                    if (item._location.InView(_player._location) && item.CollidesWith(position))
                    {
                        if (item is Item || (item._id != _player._id && item.Hidden))
                        {
                            continue;
                        }
                        if (_input.Mouse.LeftDown)
                        {
                            item._clicked = true;
                        }
                        flag = true;
                        if (item._id != _player._id)
                        {
                            item._displayTag = 1;
                        }
                        if (_loadedSpell != null && (_loadedSpell._targettype == "target" || _loadedSpell._targettype == "meall"))
                        {
                            item._targeted = true;
                        }
                        else
                        {
                            item._targeted = false;
                        }
                        _input.Mouse.SetCursorSelected();
                        if (item is Monster && item._showInfo)
                        {
                            _tTT = "Name: " + item._name;
                            _tTT = _tTT + "\nEXP:  " + item._exp.ToString("#,0");
                            _tTT = _tTT + "\nHP:   " + item._curHP.ToString("#,0") + " / " + item._maxHP.ToString("#,0");
                            _tTT = _tTT + "\nLev:  " + item._lev;
                            _tTT = _tTT + "\nATTACK NATURE:  " + item._atk;
                            _tTT = _tTT + "\nDEFENSE NATURE:  " + item._def;
                            _toolTipText.ChangeText(_tTT);
                            UpdateToolTip(position);
                        }
                        if (!_input.Mouse.DoubleRightPressed && _input.Mouse.RightPressed && debug && item._mBody != null)
                        {
                            _eTT = "Image: " + item._mBody._mImage.Name;
                            _eTT = _eTT + "\nWalk Start: " + item._mBody._mImage.WalkStart;
                            _eTT = _eTT + "\nWalk Length: " + item._mBody._mImage.WalkLength;
                            _eTT = _eTT + "\nIdle Start: " + item._mBody._mImage.IdleStart;
                            _eTT = _eTT + "\nIdle Length: " + item._mBody._mImage.IdleLength;
                            _eTT = _eTT + "\nIdle2 Length: " + item._mBody._mImage.Idle2Length;
                            _eTT = _eTT + "\nAttack1 Start: " + item._mBody._mImage.Attack1Start;
                            _eTT = _eTT + "\nAttack1 Length: " + item._mBody._mImage.Attack1Length;
                            _eTT = _eTT + "\nAttack2 Start: " + item._mBody._mImage.Attack2Start;
                            _eTT = _eTT + "\nAttack2 Length: " + item._mBody._mImage.Attack2Length;
                            _eTT = _eTT + "\nAttack3 Start: " + item._mBody._mImage.Attack3Start;
                            _eTT = _eTT + "\nAttack3 Length: " + item._mBody._mImage.Attack3Length;
                            _entityToolTip.ChangeText(_eTT);
                            UpdateEntityToolTip(position);
                        }
                        if (!_input.Mouse.LeftPressed || _viewingLegend || _viewingProfile || _viewingOthersLegend || _viewingOthersProfile)
                        {
                            continue;
                        }
                        if (_loadedSpell != null && (_loadedSpell._targettype == "target" || _loadedSpell._targettype == "meall"))
                        {
                            CastLoadedSpellOnTarget(item);
                        }
                        else if (item is Monster)
                        {
                            SystemMsg(item._name, 3);
                        }
                        else if (item is NPC && item._clicked && !item.Hidden)
                        {
                            DialogPopup(item, item._name, 0, item._mBody._imgArr[item._mBody._face - 1]);
                        }
                        else if (item is Player && _input.Keyboard.IsKeyPressedOrHeld(Keys.ControlKey) && !_viewingProfile && !_viewingOthersProfile)
                        {
                            if (item._id == _player._id)
                            {
                                _viewingProfile = true;
                            }
                            else if (!item.Hidden)
                            {
                                RequestProfilePacket requestProfilePacket = new RequestProfilePacket(item._id);
                                GameWindow.ClientSocket.Send(requestProfilePacket.Data);
                                _viewingOthersProfile = true;
                            }
                        }
                        item._clicked = false;
                    }
                    else
                    {
                        if (item._displayTag == 1)
                        {
                            item._displayTag = 0;
                        }
                        if (item._targeted)
                        {
                            item._targeted = false;
                        }
                    }
                }
                if (_viewingProfile)
                {
                    foreach (InventoryItem item2 in _equipment)
                    {
                        if (item2.CollidesWith(position))
                        {
                            flag = true;
                            ItemToolTip(item2, position);
                            break;
                        }
                    }
                }
                if (_viewingOthersProfile && _profilePlayer != null)
                {
                    foreach (Equipment item3 in _profilePlayer._equipment)
                    {
                        if (item3.CollidesWith(position))
                        {
                            flag = true;
                            ItemToolTip(item3, position);
                            break;
                        }
                    }
                }
            }
        }
        // --- Nothing hovered: clear tooltip + cursor; update drag icon; then process keyboard input ---
        if (!flag && !_viewingDialog)
        {
            _tTT = "";
            _toolTipText.ChangeText("");
            _input.Mouse.SetCursorDefault();
        }
        if (!_infoBarMenu._buttons["sysmsgBtn"].Held)
        {
            _dragIcon.Update(elapsedTime);
        }
        UpdateInput(elapsedTime);
    }

    /// <summary>
    ///     Processes keyboard input each frame. First the always-on keys: Escape (close), Return/Space
    ///     (send chat / advance dialog), the arrow keys (facing), and whisper/shout entry. While the player
    ///     is typing (chat mode or a number-entry prompt) it returns here; otherwise it handles the in-world
    ///     hotkeys: function keys (F1-F12), letter hotkeys (bag, panels, group, scrolling), and the
    ///     spell/skill hotbar (number keys modified by Ctrl/Alt), plus held-key actions and list navigation.
    /// </summary>
    private void UpdateInput(double elapsedTime)
    {
        Engine.Point mp = _input.Mouse.Position;
        EscapeHotkey();
        // === Chat / dialog confirm (Return, Space), facing (arrows), whisper & shout entry ===
        if (_input.Keyboard.IsKeyPressed(Keys.Return))
        {
            if (_viewingSense)
            {
                _viewingSense = false;
            }
            else if (_viewingSign)
            {
                _viewingSign = false;
            }
            else if (_viewingDialog)
            {
                if (_standardDialogPopup._disButtons.ContainsKey("btn0"))
                {
                    _standardDialogPopup._disButtons["btn0"].OnPress();
                }
                else if (_standardDialogPopup._buttons["sdpNextBtn"].Enabled)
                {
                    _standardDialogPopup._buttons["sdpNextBtn"].OnPress();
                }
                else if (!_standardDialogPopup._disTextFields.ContainsKey("inputTF"))
                {
                    _standardDialogPopup._buttons["sdpQuitBtn"].OnPress();
                }
            }
            else if (!_viewingStuff())
            {
                if (!_chatMode)
                {
                    if (!_userMsgPrompt)
                    {
                        _miscMenu._labels["chatLabel"].ChangeText(_player._name + ": ");
                        _miscMenu._labels["chatLabel"].SetColor(Engine.Color.White);
                        _miscMenu._textFields["chatTF"].SetPosition(_miscMenu._labels["chatLabel"]._position.X - 4.0 + (double)(_miscMenu._labels["chatLabel"]._text.Length * 6), _miscMenu._labels["chatLabel"]._position.Y - 2.0);
                        _miscMenu._textFields["chatTF"].SetFocus();
                        _miscMenu._textFields["chatTF"]._textObj.SetColor(Engine.Color.White);
                        _chatMode = true;
                    }
                    else
                    {
                        _miscMenu._textFields["userMsgTF"]._textObj.RemoveCursor();
                        _userMsgPrompt = false;
                        _userMsg = "";
                        _miscMenu._labels["userMsgLabel"].ChangeText("");
                        if (_miscMenu._textFields["userMsgTF"].Text != "")
                        {
                            if (int.TryParse(_miscMenu._textFields["userMsgTF"].Text, out var result))
                            {
                                if (result > _dragIcon._amount)
                                {
                                    result = _dragIcon._amount;
                                }
                                Reactor reactor = _map.tileHasReactor((byte)_dragIcon._loc.X, (byte)_dragIcon._loc.Y);
                                if (reactor != null && reactor._type == 1)
                                {
                                    StepOnAReactor(1, (byte)_dragIcon._loc.X, (byte)_dragIcon._loc.Y);
                                }
                                else
                                {
                                    SpawnItem(_dragIcon._name, _dragIcon._loc, result, _dragIcon._durability, _dragIcon._bodyImgColor, _dragIcon._enchantment);
                                }
                                RemoveItem(_dragIcon._slot, result);
                            }
                            _miscMenu._textFields["userMsgTF"].Text = "";
                        }
                    }
                }
                else if (_miscMenu._labels["chatLabel"]._text.Equals("whisper who? "))
                {
                    string text = _miscMenu._textFields["chatTF"].Text;
                    if (!_whisperList.Contains(text))
                    {
                        _whisperListIndex++;
                        _whisperList.Add(text);
                    }
                    else
                    {
                        _whisperList.Remove(text);
                        _whisperList.Add(text);
                    }
                    _miscMenu._labels["chatLabel"].ChangeText("-> " + text + ": ");
                    _miscMenu._labels["chatLabel"].SetColor(Engine.Color.LightBlue);
                    _miscMenu._textFields["chatTF"].SetPosition(_miscMenu._labels["chatLabel"]._position.X - 4.0 + (double)(_miscMenu._labels["chatLabel"]._text.Length * 6), _miscMenu._labels["chatLabel"]._position.Y - 2.0);
                    _miscMenu._textFields["chatTF"].SetFocus();
                    _miscMenu._textFields["chatTF"]._textObj.SetColor(Engine.Color.LightBlue);
                    _miscMenu._textFields["chatTF"].Text = "";
                    _chatMode = true;
                }
                else
                {
                    _chatMode = false;
                    _miscMenu._textFields["chatTF"]._textObj.RemoveCursor();
                    ChatMsg(_miscMenu._labels["chatLabel"]._text + _miscMenu._textFields["chatTF"].Text);
                    _miscMenu._textFields["chatTF"].Text = "";
                    _miscMenu._labels["chatLabel"].ChangeText("");
                }
            }
        }
        if (_input.Keyboard.IsKeyPressed(Keys.Up))
        {
            lastArrowKeyPressDT = DateTime.UtcNow;
            lastArrowKeyDirection = 0;
        }
        else if (_input.Keyboard.IsKeyPressed(Keys.Down))
        {
            lastArrowKeyPressDT = DateTime.UtcNow;
            lastArrowKeyDirection = 2;
        }
        else if (_input.Keyboard.IsKeyPressed(Keys.Left))
        {
            lastArrowKeyPressDT = DateTime.UtcNow;
            lastArrowKeyDirection = 3;
        }
        else if (_input.Keyboard.IsKeyPressed(Keys.Right))
        {
            lastArrowKeyPressDT = DateTime.UtcNow;
            lastArrowKeyDirection = 1;
        }
        if (_chatMode && _miscMenu._labels["chatLabel"]._text.StartsWith("whisper who?") && _whisperList.Count() > 1)
        {
            if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Up))
            {
                if (_whisperListIndex >= _whisperList.Count())
                {
                    _whisperListIndex = 0;
                }
                _whisperListIndex++;
                _miscMenu._textFields["chatTF"].Text = _whisperList[_whisperListIndex - 1];
            }
            else if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Down))
            {
                if (_whisperListIndex <= 1)
                {
                    _whisperListIndex = _whisperList.Count();
                }
                else
                {
                    _whisperListIndex--;
                }
                _miscMenu._textFields["chatTF"].Text = _whisperList[_whisperListIndex - 1];
            }
        }
        if (_input.Keyboard.IsKeyPressed(Keys.OemQuotes) && !_viewingStuff() && !_userMsgPrompt && !_chatMode)
        {
            _chatMode = true;
            _miscMenu._labels["chatLabel"].ChangeText("whisper who? ");
            _miscMenu._labels["chatLabel"].SetColor(Engine.Color.LightBlue);
            _miscMenu._textFields["chatTF"].SetPosition(_miscMenu._labels["chatLabel"]._position.X - 4.0 + (double)(_miscMenu._labels["chatLabel"]._text.Length * 6), _miscMenu._labels["chatLabel"]._position.Y - 2.0);
            _miscMenu._textFields["chatTF"].SetFocus();
            _miscMenu._textFields["chatTF"]._textObj.SetColor(Engine.Color.LightBlue);
            if (_whisperList.Count() > 0)
            {
                _miscMenu._textFields["chatTF"].Text = _whisperList[_whisperListIndex - 1];
            }
        }
        if (_input.Keyboard.IsKeyHeld(Keys.ShiftKey) && _input.Keyboard.IsKeyPressed(Keys.D1) && !_viewingStuff() && !_chatMode)
        {
            _chatMode = true;
            _miscMenu._labels["chatLabel"].ChangeText(_player._name + "! ");
            _miscMenu._labels["chatLabel"].SetColor(Engine.Color.Yellow);
            _miscMenu._textFields["chatTF"].SetPosition(_miscMenu._labels["chatLabel"]._position.X - 4.0 + (double)(_miscMenu._labels["chatLabel"]._text.Length * 6), _miscMenu._labels["chatLabel"]._position.Y - 2.0);
            _miscMenu._textFields["chatTF"].SetFocus();
            _miscMenu._textFields["chatTF"]._textObj.SetColor(Engine.Color.Yellow);
        }
        if (_input.Keyboard.IsKeyPressed(Keys.Space))
        {
            if (_viewingSense)
            {
                _viewingSense = false;
            }
            else if (_viewingSign)
            {
                _viewingSign = false;
            }
            else if (_viewingDialog && !_standardDialogPopup._disTextFields.ContainsKey("inputTF"))
            {
                if (_standardDialogPopup._disButtons.ContainsKey("btn0"))
                {
                    _standardDialogPopup._disButtons["btn0"].OnPress();
                }
                else if (_standardDialogPopup._buttons["sdpNextBtn"].Enabled)
                {
                    _standardDialogPopup._buttons["sdpNextBtn"].OnPress();
                }
                else if (!_standardDialogPopup._disTextFields.ContainsKey("inputTF"))
                {
                    _standardDialogPopup._buttons["sdpQuitBtn"].OnPress();
                }
            }
        }
        // === Past this gate: in-world hotkeys (skipped while typing in chat or a number-entry prompt) ===
        if (_chatMode || _userMsgPrompt)
        {
            return;
        }
        // === Function keys: F1 create-NPC dialog, F3 world map, F4 settings, F5 recall, F12 screenshot ===
        if (_input.Keyboard.IsKeyPressed(Keys.F1))
        {
            foreach (Entity value in _map._entities.Values)
            {
                if (value._name == "Create" && value is NPC)
                {
                    DialogPopup(value, value._name, 0, value._mBody._sprite.Texture);
                }
            }
        }
        if (_input.Keyboard.IsKeyPressed(Keys.F3) && !_worldMap)
        {
            _worldMap = true;
            AddTowns();
        }
        if (_input.Keyboard.IsKeyPressed(Keys.F4))
        {
            _viewingSettings = true;
        }
        if (_input.Keyboard.IsKeyPressed(Keys.F5))
        {
            Teleport(_player._location.X, _player._location.Y);
        }
        if (_input.Keyboard.IsKeyPressed(Keys.F12))
        {
            string text2 = "screenshots/";
            if (!Directory.Exists(text2))
            {
                Directory.CreateDirectory(text2);
            }
            Size size = new Size(_gw.ClientSize.Width, _gw.ClientSize.Height);
            Bitmap bitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, size.Width, size.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Gl.glReadPixels(0, 0, size.Width, size.Height, 32993, 5121, bitmapData.Scan0);
            bitmap.UnlockBits(bitmapData);
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
            int num = 0;
            string[] files = Directory.GetFiles(text2);
            string[] array = files;
            foreach (string text3 in array)
            {
                if (text3.StartsWith(text2 + "ss") && text3.EndsWith(".png"))
                {
                    int num2 = int.Parse(text3.Remove(text3.Length - 4).Substring(14));
                    if (num < num2)
                    {
                        num = num2;
                    }
                }
            }
            string text4 = "ss" + (num + 1) + ".png";
            bitmap.Save(text2 + text4, ImageFormat.Png);
            bitmap.Dispose();
            SystemMsg("(( " + text4 + " saved to " + text2.Remove(text2.Count() - 1) + ". ))", 3);
        }
        if (!_viewingStuff())
        {
            // === Letter hotkeys: bag, panel toggles, group, Tab, page scrolling, examine ===
            if (_input.Keyboard.IsKeyPressed(Keys.B))
            {
                bool flag = false;
                Tile tile = _player._tile;
                foreach (Entity item in tile._entities.Values.OrderByDescending((Entity z) => z._tileTime))
                {
                    if (item is Item)
                    {
                        flag = true;
                        NewItem(item._name, 0, (item as Item)._amount, (item as Item)._durability, (item as Item)._bodyImgColor, (item as Item)._enchantment);
                        RemoveEntity(item._id, item._tile);
                        break;
                    }
                }
                if (!flag)
                {
                    tile = TileImFacing();
                    foreach (Entity item2 in tile._entities.Values.OrderByDescending((Entity z) => z._tileTime))
                    {
                        if (item2 is Item)
                        {
                            NewItem(item2._name, 0, (item2 as Item)._amount, (item2 as Item)._durability, (item2 as Item)._bodyImgColor, (item2 as Item)._enchantment);
                            RemoveEntity(item2._id, item2._tile);
                            break;
                        }
                    }
                }
            }
            if (_input.Mouse.DoubleLeftPressed)
            {
                Tile xYTile = getXYTile((int)mp.X, (int)mp.Y);
                if (xYTile != null && xYTile.Location.WithinRange(_player._location, 3))
                {
                    foreach (Entity item3 in from z in xYTile._entities.Values.ToArray()
                                             orderby z._tileTime descending
                                             select z)
                    {
                        if (item3 is Item)
                        {
                            NewItem(item3._name, 0, (item3 as Item)._amount, (item3 as Item)._durability, (item3 as Item)._bodyImgColor, (item3 as Item)._enchantment);
                            RemoveEntity(item3._id, item3._tile);
                            break;
                        }
                    }
                }
            }
            if (_input.Keyboard.IsKeyPressed(Keys.Q))
            {
                _menu._buttons["optionsBtn"].OnPress();
            }
            if (_input.Keyboard.IsKeyPressed(Keys.W))
            {
                _menu._buttons["mail1Btn"].OnPress();
            }
            if (_input.Keyboard.IsKeyPressed(Keys.E))
            {
                _menu._buttons["usersBtn"].OnPress();
            }
            if (_input.Keyboard.IsKeyPressed(Keys.Y))
            {
                if (GameWindow.ConnectedToServer)
                {
                    GroupListRequestPacket groupListRequestPacket = new GroupListRequestPacket();
                    GameWindow.ClientSocket.Send(groupListRequestPacket.Data);
                }
                CloseMenus();
                _viewingGroupList = true;
            }
            if (_tabMap != null && _input.Keyboard.IsKeyPressed(Keys.Tab))
            {
                if (_tabMap._display)
                {
                    _tabMap._display = false;
                }
                else
                {
                    _tabMap._display = true;
                }
            }
            if (_tabMap != null && _tabMap._display && _input.Keyboard.IsKeyPressedAndRepeated(Keys.Prior) && _tabMap._scale > -0.3f)
            {
                _tabMap._scale -= 0.3f;
            }
            if (_tabMap != null && _tabMap._display && _input.Keyboard.IsKeyPressedAndRepeated(Keys.Next) && _tabMap._scale < 6.3f)
            {
                _tabMap._scale += 0.3f;
            }
            if (_input.Keyboard.IsKeyPressed(Keys.X) && _viewingOptions)
            {
                ToggleSelectedButtons(_optionsMenu);
                ToggleSelectedButtons(_menu);
                _system.ChangeState("start_menu");
            }
            if (_input.Keyboard.IsKeyPressedOrHeld(Keys.ShiftKey))
            {
                if (_input.Keyboard.IsKeyPressed(Keys.A))
                {
                    if (_panelNum != 0)
                    {
                        _panelNum = 0;
                        ToggleSelectedButtons(_panelMenu, _panelMenu._buttons["inventoryBtn"], toggle: false);
                        _miscMenu._buttons["fullInvBtn"].Enabled = true;
                    }
                    if (!_miscMenu._buttons["fullInvBtn"].Selected)
                    {
                        _inventoryMenu._background.SetPosition(96.0, 234.0);
                        _miscMenu._buttons["fullInvBtn"].Selected = true;
                    }
                }
                if (_input.Keyboard.IsKeyPressed(Keys.S) && _panelNum != 6)
                {
                    _panelNum = 6;
                    ToggleSelectedButtons(_panelMenu, _panelMenu._buttons["skillBtn"], toggle: false);
                    _inventoryMenu._background.SetPosition(96.0, 336.0);
                    _miscMenu._buttons["fullInvBtn"].Enabled = false;
                    _miscMenu._buttons["fullInvBtn"].Selected = false;
                }
                if (_input.Keyboard.IsKeyPressed(Keys.D) && _panelNum != 7)
                {
                    _panelNum = 7;
                    ToggleSelectedButtons(_panelMenu, _panelMenu._buttons["spellBtn"], toggle: false);
                    _inventoryMenu._background.SetPosition(96.0, 336.0);
                    _miscMenu._buttons["fullInvBtn"].Enabled = false;
                    _miscMenu._buttons["fullInvBtn"].Selected = false;
                }
                if (_input.Keyboard.IsKeyPressed(Keys.F) && _panelNum != 8)
                {
                    _panelNum = 8;
                    ToggleSelectedButtons(_panelMenu, _panelMenu._buttons["chatBtn"], toggle: false);
                    _inventoryMenu._background.SetPosition(96.0, 336.0);
                    _miscMenu._buttons["fullInvBtn"].Enabled = false;
                    _miscMenu._buttons["fullInvBtn"].Selected = false;
                }
                if (_input.Keyboard.IsKeyPressed(Keys.G) && _panelNum != 9)
                {
                    _panelNum = 9;
                    ToggleSelectedButtons(_panelMenu, _panelMenu._buttons["statBtn"], toggle: false);
                    _inventoryMenu._background.SetPosition(96.0, 336.0);
                    _miscMenu._buttons["fullInvBtn"].Enabled = false;
                    _miscMenu._buttons["fullInvBtn"].Selected = false;
                }
                if (_input.Keyboard.IsKeyPressed(Keys.H) && _panelNum != 10)
                {
                    _panelNum = 10;
                    ToggleSelectedButtons(_panelMenu, _panelMenu._buttons["actionBtn"], toggle: false);
                    _inventoryMenu._background.SetPosition(96.0, 336.0);
                    _miscMenu._buttons["fullInvBtn"].Enabled = false;
                    _miscMenu._buttons["fullInvBtn"].Selected = false;
                }
            }
            else
            {
                if (_input.Keyboard.IsKeyPressed(Keys.A))
                {
                    if (_panelNum != 0)
                    {
                        _panelNum = 0;
                        ToggleSelectedButtons(_panelMenu, _panelMenu._buttons["inventoryBtn"], toggle: false);
                        _miscMenu._buttons["fullInvBtn"].Enabled = true;
                    }
                    else if (_miscMenu._buttons["fullInvBtn"].Selected)
                    {
                        _inventoryMenu._background.SetPosition(96.0, 336.0);
                        _miscMenu._buttons["fullInvBtn"].Selected = false;
                    }
                    else if (_panelNum == 0 && !_viewingProfile)
                    {
                        _viewingProfile = true;
                    }
                }
                if (_input.Keyboard.IsKeyPressed(Keys.S) && _panelNum != 1)
                {
                    _panelNum = 1;
                    ToggleSelectedButtons(_panelMenu, _panelMenu._buttons["skillBtn"], toggle: false);
                    _inventoryMenu._background.SetPosition(96.0, 336.0);
                    _miscMenu._buttons["fullInvBtn"].Enabled = false;
                    _miscMenu._buttons["fullInvBtn"].Selected = false;
                }
                if (_input.Keyboard.IsKeyPressed(Keys.D) && _panelNum != 2)
                {
                    _panelNum = 2;
                    ToggleSelectedButtons(_panelMenu, _panelMenu._buttons["spellBtn"], toggle: false);
                    _inventoryMenu._background.SetPosition(96.0, 336.0);
                    _miscMenu._buttons["fullInvBtn"].Enabled = false;
                    _miscMenu._buttons["fullInvBtn"].Selected = false;
                }
                if (_input.Keyboard.IsKeyPressed(Keys.F) && _panelNum != 3)
                {
                    _panelNum = 3;
                    ToggleSelectedButtons(_panelMenu, _panelMenu._buttons["chatBtn"], toggle: false);
                    _inventoryMenu._background.SetPosition(96.0, 336.0);
                    _miscMenu._buttons["fullInvBtn"].Enabled = false;
                    _miscMenu._buttons["fullInvBtn"].Selected = false;
                }
                if (_input.Keyboard.IsKeyPressed(Keys.G) && _panelNum != 4)
                {
                    _panelNum = 4;
                    ToggleSelectedButtons(_panelMenu, _panelMenu._buttons["statBtn"], toggle: false);
                    _inventoryMenu._background.SetPosition(96.0, 336.0);
                    _miscMenu._buttons["fullInvBtn"].Enabled = false;
                    _miscMenu._buttons["fullInvBtn"].Selected = false;
                }
                if (_input.Keyboard.IsKeyPressed(Keys.H) && _panelNum != 5)
                {
                    _panelNum = 5;
                    ToggleSelectedButtons(_panelMenu, _panelMenu._buttons["actionBtn"], toggle: false);
                    _inventoryMenu._background.SetPosition(96.0, 336.0);
                    _miscMenu._buttons["fullInvBtn"].Enabled = false;
                    _miscMenu._buttons["fullInvBtn"].Selected = false;
                }
            }
            // === Spell/skill hotbar: number keys cast/use a slot; Ctrl/Alt select the hotbar row ===
            if (_input.Keyboard.IsKeyPressedOrHeld(Keys.ControlKey) && _input.Keyboard.IsKeyPressedOrHeld(Keys.LMenu))
            {
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad1) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D1))
                {
                    Emote(64, 1500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad2) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D2))
                {
                    Emote(65, 1500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad3) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D3))
                {
                    Emote(66, 1500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad4) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D4))
                {
                    Emote(67, 1500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad5) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D5))
                {
                    Emote(68, 500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad6) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D6))
                {
                    Emote(69, 1500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad7) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D7))
                {
                    Emote(70, 1500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad8) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D8))
                {
                    Emote(71, 1500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad9) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D9))
                {
                    Emote(72, 1500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad0) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D0))
                {
                    Emote(73, 1500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Subtract) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.OemMinus))
                {
                    Emote(74, 1500);
                }
            }
            else if (_input.Keyboard.IsKeyPressedOrHeld(Keys.ControlKey))
            {
                if (_input.Keyboard.IsKeyPressed(Keys.Oemtilde))
                {
                    _dragIcon._clicked = false;
                    InventoryItem[] array2 = _equipment.ToArray();
                    foreach (InventoryItem inventoryItem in array2)
                    {
                        NewItem(inventoryItem._name, 0, 1, inventoryItem._durability, inventoryItem._bodyImgColor, inventoryItem._enchantment);
                        RemoveEquip(inventoryItem._slot);
                    }
                    SendProfileData();
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad1) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D1))
                {
                    Emote(55, 1500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad2) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D2))
                {
                    Emote(56, 1500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad3) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D3))
                {
                    Emote(57, 1500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad4) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D4))
                {
                    Emote(58, 1500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad5) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D5))
                {
                    Emote(59, 1500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad6) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D6))
                {
                    Emote(60, 1500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad7) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D7))
                {
                    Emote(61, 1500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad8) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D8))
                {
                    Emote(62, 1000);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad9) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D9))
                {
                    Emote(63, 1000);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad0) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D0))
                {
                    Emote(3, 1000);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Subtract) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.OemMinus))
                {
                    Emote(4, 400, repeatOnce: true);
                }
            }
            else if (_input.Keyboard.IsKeyPressedOrHeld(Keys.LMenu))
            {
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad1) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D1))
                {
                    Emote(75, 1500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad2) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D2))
                {
                    Emote(76, 1500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad3) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D3))
                {
                    Emote(77, 1500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad4) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D4))
                {
                    Emote(78, 1500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad5) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D5))
                {
                    Emote(79, 1500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad6) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D6))
                {
                    Emote(80, 1500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad7) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D7))
                {
                    Emote(81, 1500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad8) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D8))
                {
                    Emote(82, 1500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad9) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D9))
                {
                    Emote(83, 500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad0) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D0))
                {
                    Emote(84, 500);
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Subtract) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.OemMinus))
                {
                    Emote(85, 500);
                }
            }
            else
            {
                if (_input.Keyboard.IsKeyPressed(Keys.Oemtilde))
                {
                    _dragIcon._clicked = false;
                    InventoryItem[] array3 = _equipment.ToArray();
                    foreach (InventoryItem inventoryItem2 in array3)
                    {
                        if (inventoryItem2._slot == 9 || inventoryItem2._slot == 3)
                        {
                            NewItem(inventoryItem2._name, 0, 1, inventoryItem2._durability, inventoryItem2._bodyImgColor, inventoryItem2._enchantment);
                            RemoveEquip(inventoryItem2._slot);
                        }
                    }
                    SendProfileData();
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad1) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D1))
                {
                    if (_panelNum == 0)
                    {
                        UseItem(1);
                    }
                    else if (_panelNum == 1)
                    {
                        UseSkill(1);
                    }
                    else if (_panelNum == 2)
                    {
                        UseSpell(1);
                    }
                    else if (_panelNum == 3 || _panelNum == 4 || _panelNum == 8 || _panelNum == 9)
                    {
                        UseSpeechMacro(1);
                    }
                    else if (_panelNum == 5)
                    {
                        UseAction(1);
                    }
                    else if (_panelNum == 6)
                    {
                        UseSkill(37);
                    }
                    else if (_panelNum == 7)
                    {
                        UseSpell(37);
                    }
                    else if (_panelNum == 10)
                    {
                        UseAction(37);
                    }
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad2) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D2))
                {
                    if (_panelNum == 0)
                    {
                        UseItem(2);
                    }
                    else if (_panelNum == 1)
                    {
                        UseSkill(2);
                    }
                    else if (_panelNum == 2)
                    {
                        UseSpell(2);
                    }
                    else if (_panelNum == 3 || _panelNum == 4 || _panelNum == 8 || _panelNum == 9)
                    {
                        UseSpeechMacro(2);
                    }
                    else if (_panelNum == 5)
                    {
                        UseAction(2);
                    }
                    else if (_panelNum == 6)
                    {
                        UseSkill(38);
                    }
                    else if (_panelNum == 7)
                    {
                        UseSpell(38);
                    }
                    else if (_panelNum == 10)
                    {
                        UseAction(38);
                    }
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad3) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D3))
                {
                    if (_panelNum == 0)
                    {
                        UseItem(3);
                    }
                    else if (_panelNum == 1)
                    {
                        UseSkill(3);
                    }
                    else if (_panelNum == 2)
                    {
                        UseSpell(3);
                    }
                    else if (_panelNum == 3 || _panelNum == 4 || _panelNum == 8 || _panelNum == 9)
                    {
                        UseSpeechMacro(3);
                    }
                    else if (_panelNum == 5)
                    {
                        UseAction(3);
                    }
                    else if (_panelNum == 6)
                    {
                        UseSkill(39);
                    }
                    else if (_panelNum == 7)
                    {
                        UseSpell(39);
                    }
                    else if (_panelNum == 10)
                    {
                        UseAction(39);
                    }
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad4) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D4))
                {
                    if (_panelNum == 0)
                    {
                        UseItem(4);
                    }
                    else if (_panelNum == 1)
                    {
                        UseSkill(4);
                    }
                    else if (_panelNum == 2)
                    {
                        UseSpell(4);
                    }
                    else if (_panelNum == 3 || _panelNum == 4 || _panelNum == 8 || _panelNum == 9)
                    {
                        UseSpeechMacro(4);
                    }
                    else if (_panelNum == 5)
                    {
                        UseAction(4);
                    }
                    else if (_panelNum == 6)
                    {
                        UseSkill(40);
                    }
                    else if (_panelNum == 7)
                    {
                        UseSpell(40);
                    }
                    else if (_panelNum == 10)
                    {
                        UseAction(40);
                    }
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad5) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D5))
                {
                    if (_panelNum == 0)
                    {
                        UseItem(5);
                    }
                    else if (_panelNum == 1)
                    {
                        UseSkill(5);
                    }
                    else if (_panelNum == 2)
                    {
                        UseSpell(5);
                    }
                    else if (_panelNum == 3 || _panelNum == 4 || _panelNum == 8 || _panelNum == 9)
                    {
                        UseSpeechMacro(5);
                    }
                    else if (_panelNum == 5)
                    {
                        UseAction(5);
                    }
                    else if (_panelNum == 6)
                    {
                        UseSkill(41);
                    }
                    else if (_panelNum == 7)
                    {
                        UseSpell(41);
                    }
                    else if (_panelNum == 10)
                    {
                        UseAction(41);
                    }
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad6) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D6))
                {
                    if (_panelNum == 0)
                    {
                        UseItem(6);
                    }
                    else if (_panelNum == 1)
                    {
                        UseSkill(6);
                    }
                    else if (_panelNum == 2)
                    {
                        UseSpell(6);
                    }
                    else if (_panelNum == 3 || _panelNum == 4 || _panelNum == 8 || _panelNum == 9)
                    {
                        UseSpeechMacro(6);
                    }
                    else if (_panelNum == 5)
                    {
                        UseAction(6);
                    }
                    else if (_panelNum == 6)
                    {
                        UseSkill(42);
                    }
                    else if (_panelNum == 7)
                    {
                        UseSpell(42);
                    }
                    else if (_panelNum == 10)
                    {
                        UseAction(42);
                    }
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad7) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D7))
                {
                    if (_panelNum == 0)
                    {
                        UseItem(7);
                    }
                    else if (_panelNum == 1)
                    {
                        UseSkill(7);
                    }
                    else if (_panelNum == 2)
                    {
                        UseSpell(7);
                    }
                    else if (_panelNum == 3 || _panelNum == 4 || _panelNum == 8 || _panelNum == 9)
                    {
                        UseSpeechMacro(7);
                    }
                    else if (_panelNum == 5)
                    {
                        UseAction(7);
                    }
                    else if (_panelNum == 6)
                    {
                        UseSkill(43);
                    }
                    else if (_panelNum == 7)
                    {
                        UseSpell(43);
                    }
                    else if (_panelNum == 10)
                    {
                        UseAction(43);
                    }
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad8) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D8))
                {
                    if (_panelNum == 0)
                    {
                        UseItem(8);
                    }
                    else if (_panelNum == 1)
                    {
                        UseSkill(8);
                    }
                    else if (_panelNum == 2)
                    {
                        UseSpell(8);
                    }
                    else if (_panelNum == 3 || _panelNum == 4 || _panelNum == 8 || _panelNum == 9)
                    {
                        UseSpeechMacro(8);
                    }
                    else if (_panelNum == 5)
                    {
                        UseAction(8);
                    }
                    else if (_panelNum == 6)
                    {
                        UseSkill(44);
                    }
                    else if (_panelNum == 7)
                    {
                        UseSpell(44);
                    }
                    else if (_panelNum == 10)
                    {
                        UseAction(44);
                    }
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad9) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D9))
                {
                    if (_panelNum == 0)
                    {
                        UseItem(9);
                    }
                    else if (_panelNum == 1)
                    {
                        UseSkill(9);
                    }
                    else if (_panelNum == 2)
                    {
                        UseSpell(9);
                    }
                    else if (_panelNum == 3 || _panelNum == 4 || _panelNum == 8 || _panelNum == 9)
                    {
                        UseSpeechMacro(9);
                    }
                    else if (_panelNum == 5)
                    {
                        UseAction(9);
                    }
                    else if (_panelNum == 6)
                    {
                        UseSkill(45);
                    }
                    else if (_panelNum == 7)
                    {
                        UseSpell(45);
                    }
                    else if (_panelNum == 10)
                    {
                        UseAction(45);
                    }
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.NumPad0) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.D0))
                {
                    if (_panelNum == 0)
                    {
                        UseItem(10);
                    }
                    else if (_panelNum == 1)
                    {
                        UseSkill(10);
                    }
                    else if (_panelNum == 2)
                    {
                        UseSpell(10);
                    }
                    else if (_panelNum == 3 || _panelNum == 4 || _panelNum == 8 || _panelNum == 9)
                    {
                        UseSpeechMacro(10);
                    }
                    else if (_panelNum == 5)
                    {
                        UseAction(10);
                    }
                    else if (_panelNum == 6)
                    {
                        UseSkill(46);
                    }
                    else if (_panelNum == 7)
                    {
                        UseSpell(46);
                    }
                    else if (_panelNum == 10)
                    {
                        UseAction(46);
                    }
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Subtract) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.OemMinus))
                {
                    if (_panelNum == 0)
                    {
                        UseItem(11);
                    }
                    else if (_panelNum == 1)
                    {
                        UseSkill(11);
                    }
                    else if (_panelNum == 2)
                    {
                        UseSpell(11);
                    }
                    else if (_panelNum == 5)
                    {
                        UseAction(11);
                    }
                    else if (_panelNum == 6)
                    {
                        UseSkill(47);
                    }
                    else if (_panelNum == 7)
                    {
                        UseSpell(47);
                    }
                    else if (_panelNum == 10)
                    {
                        UseAction(47);
                    }
                }
                if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Add) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.Oemplus))
                {
                    if (_panelNum == 0)
                    {
                        UseItem(12);
                    }
                    else if (_panelNum == 1)
                    {
                        UseSkill(12);
                    }
                    else if (_panelNum == 2)
                    {
                        UseSpell(12);
                    }
                    else if (_panelNum == 5)
                    {
                        UseAction(12);
                    }
                    else if (_panelNum == 6)
                    {
                        UseSkill(48);
                    }
                    else if (_panelNum == 7)
                    {
                        UseSpell(48);
                    }
                    else if (_panelNum == 10)
                    {
                        UseAction(48);
                    }
                }
            }
        }
        if (_input.Mouse.WheelUp)
        {
            if (_viewingDialog)
            {
                if (_standardDialogPopup._disButtons.ContainsKey("chatScrollUpBtn"))
                {
                    Rectangle rect = new Rectangle((int)_standardDialogPopup._position.X + 136, (int)_standardDialogPopup._position.Y + 13, 303, 82);
                    if (CollidesWith(rect, mp) && _standardDialogPopup._disButtons.ContainsKey("catScrollUpBtn"))
                    {
                        _standardDialogPopup._disButtons["catScrollUpBtn"].OnPress();
                    }
                    else
                    {
                        _standardDialogPopup._disButtons["chatScrollUpBtn"].OnPress();
                    }
                }
            }
            else if (_viewingLegend)
            {
                _legendMenu._buttons["legendScrollUpBtn"].OnPress();
            }
            else if (_viewCommands)
            {
                _commandMenu._buttons["commandScrollUpBtn"].OnPress();
            }
            else if (_viewKeyboards)
            {
                _keyboardMenu._buttons["keyboardScrollUpBtn"].OnPress();
            }
            else if (_panelNum == 3)
            {
                _chatMenu._buttons["chatScrollUpBtn"].OnPress();
            }
            else if (_panelNum == 8)
            {
                _infoMenu._buttons["infoScrollUpBtn"].OnPress();
            }
        }
        if (_input.Mouse.WheelDown)
        {
            if (_viewingDialog)
            {
                if (_standardDialogPopup._disButtons.ContainsKey("chatScrollDownBtn"))
                {
                    Rectangle rect2 = new Rectangle((int)_standardDialogPopup._position.X + 136, (int)_standardDialogPopup._position.Y + 13, 303, 82);
                    if (CollidesWith(rect2, mp) && _standardDialogPopup._disButtons.ContainsKey("catScrollDownBtn"))
                    {
                        _standardDialogPopup._disButtons["catScrollDownBtn"].OnPress();
                    }
                    else
                    {
                        _standardDialogPopup._disButtons["chatScrollDownBtn"].OnPress();
                    }
                }
            }
            else if (_viewingLegend)
            {
                _legendMenu._buttons["legendScrollDownBtn"].OnPress();
            }
            else if (_viewCommands)
            {
                _commandMenu._buttons["commandScrollDownBtn"].OnPress();
            }
            else if (_viewKeyboards)
            {
                _keyboardMenu._buttons["keyboardScrollDownBtn"].OnPress();
            }
            else if (_panelNum == 3)
            {
                _chatMenu._buttons["chatScrollDownBtn"].OnPress();
            }
            else if (_panelNum == 8)
            {
                _infoMenu._buttons["infoScrollDownBtn"].OnPress();
            }
        }
        if (_input.Mouse.RightPressed)
        {
            _autoAttackEntity = null;
            if (_loadedSpell != null)
            {
                _loadedSpell = null;
            }
            else if (_viewingDialog && _standardDialogPopup.CollidesWith(mp))
            {
                _viewingDialog = false;
            }
            else if (_viewKeyboards)
            {
                _viewKeyboards = false;
            }
            else if (_viewCommands)
            {
                _viewCommands = false;
            }
            else if (_viewingSign && _signMenu.CollidesWith(mp))
            {
                _viewingSign = false;
            }
            else if (_viewingSense && _senseMenu.CollidesWith(mp))
            {
                _viewingSense = false;
            }
            else if (_viewingSettings)
            {
                _viewingSettings = false;
                ToggleSelectedButtons(_optionsMenu);
            }
            else if (_viewingMacros)
            {
                _viewingMacros = false;
                ToggleSelectedButtons(_optionsMenu);
            }
            else if (_viewingFriends)
            {
                _viewingFriends = false;
                ToggleSelectedButtons(_optionsMenu);
            }
            else if (_viewingOptions)
            {
                _viewingOptions = false;
                ToggleSelectedButtons(_optionsMenu);
                ToggleSelectedButtons(_menu);
            }
            else if (_composingPost && _composePostMenu.CollidesWith(mp))
            {
                _composePostMenu._buttons["cancelComposedBtn"].OnPress();
            }
            else if (_viewingPost && _viewPostMenu.CollidesWith(mp))
            {
                _viewPostMenu._buttons["upViewBtn"].OnPress();
            }
            else if (_viewingMailList && _mailListMenu.CollidesWith(mp))
            {
                _viewingMailList = false;
            }
            else if (_viewingBoardList && _boardListMenu.CollidesWith(mp))
            {
                _viewingBoardList = false;
            }
            else if (_viewingPersonalBoard && _boardMenu.CollidesWith(mp))
            {
                _viewingPersonalBoard = false;
                ToggleSelectedButtons(_boardMenu);
                ToggleSelectedButtons(_menu);
            }
            else if (_viewingUsers && _usersMenu.CollidesWith(mp))
            {
                _viewingUsers = false;
                ToggleSelectedButtons(_usersMenu);
                ToggleSelectedButtons(_menu);
            }
            else if (!_viewingStuff() && mp.Y < 320f && !_player._dead && (!_miscMenu._buttons["fullInvBtn"].Selected || !_fullInvPanel.CollidesWith(mp.X, mp.Y)))
            {
                Tile xYTile2 = getXYTile((int)mp.X, (int)mp.Y, 13);
                if (xYTile2 != null)
                {
                    _pfa = new Pathfind(_player._location.X, _player._location.Y, xYTile2.Location.X, xYTile2.Location.Y, _map);
                }
            }
        }
        if (_input.Mouse.DoubleRightPressed)
        {
            Tile tile2 = null;
            tile2 = ((_pfa == null || _pfa._thePath.Count() <= 0) ? getXYTile((int)mp.X, (int)mp.Y, 13) : _map._tiles[(int)((double)_pfa._endy * _map._width + (double)_pfa._endx)]);
            if (tile2 != null)
            {
                Entity topMostNonItem = tile2.getTopMostNonItem();
                if (topMostNonItem != null)
                {
                    _autoAttackEntity = topMostNonItem;
                }
            }
        }
        if (dyeingequipment && _input.Mouse.LeftPressed && _panelNum == 0)
        {
            int xYSlot = getXYSlot((int)mp.X, (int)mp.Y);
            if (xYSlot != 0)
            {
                InventoryItem itemSlot = getItemSlot(xYSlot);
                if (itemSlot != null)
                {
                    if (itemSlot._dyeable == 1)
                    {
                        itemSlot._bodyImgColor = selectedequipmentdye;
                        dyeingequipment = false;
                        _userMsg = "";
                        RemoveItem(selectedequipmentdyeslot);
                        SystemMsg(itemSlot._name + " was dyed " + getDye(selectedequipmentdye.ToString()) + ".", 3);
                        itemSlot.RefreshImage();
                    }
                    else
                    {
                        SystemMsg(itemSlot._name + " cannot be dyed.", 3);
                    }
                }
            }
        }
        if (!_viewingStuff() && !_viewingLegend && !_infoBarMenu._buttons["sysmsgBtn"].Held)
        {
            if (_input.Mouse.LeftPressed && _viewingProfile)
            {
                _equipment.ForEach(delegate (InventoryItem x)
                {
                    if (x.CollidesWith(mp))
                    {
                        NewItem(x._name, 0, 1, x._durability, x._bodyImgColor, x._enchantment);
                        RemoveEquip(x._slot);
                    }
                });
            }
            if (_input.Mouse.LeftDown && !_dragIcon._clicked)
            {
                int xYSlot2 = getXYSlot((int)mp.X, (int)mp.Y);
                if (xYSlot2 != 0 && !CollidesWithProfiles(mp))
                {
                    if (_panelNum == 0)
                    {
                        InventoryItem itemSlot2 = getItemSlot(xYSlot2);
                        if (itemSlot2 != null)
                        {
                            _dragIcon = new DragIcon(_input, itemSlot2._sprite, itemSlot2._name, itemSlot2._slot, itemSlot2._amount, itemSlot2._durability, itemSlot2._bodyImgColor, itemSlot2._enchantment);
                        }
                        else
                        {
                            _dragIcon = new DragIcon(_input, new Sprite(), "", xYSlot2);
                        }
                        _dragIcon._clicked = true;
                    }
                    else if (_panelNum == 1 || _panelNum == 6)
                    {
                        Skill skillSlot = getSkillSlot(xYSlot2);
                        if (skillSlot != null)
                        {
                            if (_GM || (skillSlot._cooldown != 0.0 && DateTime.UtcNow.Subtract(skillSlot._lastSuccess).TotalMilliseconds >= skillSlot._cooldown * 1000.0))
                            {
                                _dragIcon = new DragIcon(_input, skillSlot._sprite, skillSlot._name, skillSlot._slot);
                                _dragIcon._clicked = true;
                            }
                        }
                        else
                        {
                            _dragIcon = new DragIcon(_input, new Sprite(), "", xYSlot2);
                            _dragIcon._clicked = true;
                        }
                    }
                    else if (_panelNum == 2 || _panelNum == 7)
                    {
                        Spell spellSlot = getSpellSlot(xYSlot2);
                        if (spellSlot != null)
                        {
                            _dragIcon = new DragIcon(_input, spellSlot._sprite, spellSlot._name, spellSlot._slot);
                        }
                        else
                        {
                            _dragIcon = new DragIcon(_input, new Sprite(), "", xYSlot2);
                        }
                        _dragIcon._clicked = true;
                    }
                    else if (_panelNum == 5 || _panelNum == 10)
                    {
                        Action actionSlot = getActionSlot(xYSlot2);
                        if (actionSlot != null)
                        {
                            _dragIcon = new DragIcon(_input, actionSlot._sprite, actionSlot._name, actionSlot._slot);
                        }
                        else
                        {
                            _dragIcon = new DragIcon(_input, new Sprite(), "", xYSlot2);
                        }
                        _dragIcon._clicked = true;
                    }
                }
            }
            else if (!_input.Mouse.LeftHeld && _dragIcon._clicked)
            {
                int xYSlot3 = getXYSlot((int)mp.X, (int)mp.Y);
                if (xYSlot3 != 0 && !CollidesWithProfiles(mp))
                {
                    if (_panelNum == 0)
                    {
                        if (xYSlot3 != 72 && _dragIcon._slot != 72 && xYSlot3 != _dragIcon._slot)
                        {
                            InventoryItem itemSlot3 = getItemSlot(xYSlot3);
                            RemoveItem(_dragIcon._slot);
                            if (itemSlot3 != null && itemSlot3._slot != _dragIcon._slot)
                            {
                                RemoveItem(itemSlot3._slot);
                                NewItem(itemSlot3._name, _dragIcon._slot, itemSlot3._amount, itemSlot3._durability, itemSlot3._bodyImgColor, itemSlot3._enchantment);
                            }
                            NewItem(_dragIcon._name, xYSlot3, _dragIcon._amount, _dragIcon._durability, _dragIcon._bodyImgColor, _dragIcon._enchantment);
                        }
                    }
                    else if (_panelNum == 1 || _panelNum == 6)
                    {
                        if (xYSlot3 != 72 && _dragIcon._slot != 72)
                        {
                            Skill skillSlot2 = getSkillSlot(xYSlot3);
                            bool num3;
                            if (skillSlot2 == null)
                            {
                                num3 = skillSlot2 == null;
                            }
                            else
                            {
                                if (skillSlot2._cooldown == 0.0)
                                {
                                    goto IL_4091;
                                }
                                num3 = DateTime.UtcNow.Subtract(skillSlot2._lastSuccess).TotalMilliseconds >= skillSlot2._cooldown * 1000.0;
                            }
                            if (num3)
                            {
                                RemoveSkill(_dragIcon._slot);
                                if (skillSlot2 != null && skillSlot2._slot != _dragIcon._slot)
                                {
                                    RemoveSkill(skillSlot2._slot);
                                    NewSkill(skillSlot2._name, _dragIcon._slot, 0);
                                }
                                NewSkill(_dragIcon._name, xYSlot3, 0);
                            }
                        }
                    }
                    else if (_panelNum == 2 || _panelNum == 7)
                    {
                        if (xYSlot3 != 72 && _dragIcon._slot != 72)
                        {
                            Spell spellSlot2 = getSpellSlot(xYSlot3);
                            RemoveSpell(_dragIcon._slot);
                            if (spellSlot2 != null && spellSlot2._slot != _dragIcon._slot)
                            {
                                RemoveSpell(spellSlot2._slot);
                                NewSpell(spellSlot2._name, _dragIcon._slot, 0);
                            }
                            NewSpell(_dragIcon._name, xYSlot3, 0);
                        }
                    }
                    else if ((_panelNum == 5 || _panelNum == 10) && xYSlot3 != 72 && _dragIcon._slot != 72)
                    {
                        Action actionSlot2 = getActionSlot(xYSlot3);
                        RemoveAction(_dragIcon._slot);
                        if (actionSlot2 != null && actionSlot2._slot != _dragIcon._slot)
                        {
                            RemoveAction(actionSlot2._slot);
                            NewAction(actionSlot2._name, _dragIcon._slot, 0);
                        }
                        NewAction(_dragIcon._name, xYSlot3, 0);
                    }
                }
                else
                {
                    Tile xYTile3 = getXYTile((int)mp.X, (int)mp.Y);
                    if (_panelNum == 0 && !CollidesWithProfiles(mp) && xYTile3 != null)
                    {
                        Entity topMostNonItem2 = xYTile3.getTopMostNonItem();
                        if (topMostNonItem2 != null && topMostNonItem2.CollidesWith(mp))
                        {
                            if (!(topMostNonItem2 is Player))
                            {
                                topMostNonItem2._inventory.Add(_dragIcon._name);
                                _dragIcon._clicked = false;
                            }
                        }
                        else
                        {
                            int amount = _dragIcon._amount;
                            if (amount > 1)
                            {
                                _dragIcon._loc = new Location(xYTile3.Location.X, xYTile3.Location.Y);
                                _userMsg = "Drop how many " + _dragIcon._name + " [ " + _dragIcon._amount + " ] ? : ";
                                _userMsgPrompt = true;
                                _miscMenu._textFields["userMsgTF"].SetPosition(94 + _userMsg.Length * 6, _miscMenu._labels["userMsgLabel"]._position.Y - 2.0);
                                _miscMenu._textFields["userMsgTF"].SetFocus();
                            }
                            else
                            {
                                Reactor reactor2 = _map.tileHasReactor((byte)xYTile3.Location.X, (byte)xYTile3.Location.Y);
                                if (reactor2 != null && reactor2._type == 1)
                                {
                                    StepOnAReactor(1, (byte)xYTile3.Location.X, (byte)xYTile3.Location.Y);
                                }
                                else
                                {
                                    SpawnItem(_dragIcon._name, new Location(xYTile3.Location.X, xYTile3.Location.Y), amount, _dragIcon._durability, _dragIcon._bodyImgColor, _dragIcon._enchantment);
                                }
                                RemoveItem(_dragIcon._slot, amount);
                            }
                        }
                    }
                }
                goto IL_4091;
            }
            goto IL_409d;
        }
        goto IL_4228;
    IL_4091:
        _dragIcon._clicked = false;
        goto IL_409d;
    IL_409d:
        if (_input.Mouse.DoubleLeftPressed)
        {
            int xYSlot4 = getXYSlot((int)mp.X, (int)mp.Y);
            if (xYSlot4 != 0 && !CollidesWithProfiles(mp))
            {
                if (_panelNum == 0)
                {
                    UseItem(xYSlot4);
                }
                else if (_panelNum == 1 || _panelNum == 6)
                {
                    UseSkill(xYSlot4);
                }
                else if (_panelNum == 2 || _panelNum == 7)
                {
                    UseSpell(xYSlot4);
                }
                else if (_panelNum == 5 || _panelNum == 10)
                {
                    UseAction(xYSlot4);
                }
            }
        }
        // === Held-key actions (assail on Space) + arrow/Return navigation in open lists & dialogs ===
        if (_input.Keyboard.IsKeyPressedOrHeld(Keys.Space))
        {
            Assail();
        }
        if (_autoAttackEntity != null)
        {
            if (_autoAttackEntity._location.DistanceFrom(_player._location) == 1)
            {
                if (_player._location.IsInFront(_autoAttackEntity._location, (D)_player._direction))
                {
                    Assail();
                }
            }
            else if (_autoAttackEntity._location.DistanceFrom(_player._location) != 0)
            {
                _pfa = new Pathfind(_player._location.X, _player._location.Y, _autoAttackEntity._location.X, _autoAttackEntity._location.Y, _map);
            }
        }
        goto IL_4228;
    IL_4228:
        if (_optionsMenu._buttons["soundBtn"].Selected)
        {
            if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Up) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.Right))
            {
                _debugSound++;
                if (_debugSound > 29)
                {
                    _debugSound = 0;
                }
                PlaySound("effect" + _debugSound);
            }
            else if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Down) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.Left))
            {
                _debugSound--;
                if (_debugSound < 0)
                {
                    _debugSound = 29;
                }
                PlaySound("effect" + _debugSound);
            }
            if (_input.Keyboard.IsKeyPressed(Keys.Return))
            {
                PlaySound("effect" + _debugSound);
            }
        }
        else if (_optionsMenu._buttons["musicBtn"].Selected)
        {
            if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Up) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.Right))
            {
                _debugMusic++;
                if (_debugMusic > 68)
                {
                    _debugMusic = 1;
                }
                PlayMusicFile("music\\" + _debugMusic + ".ogg");
            }
            else if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Down) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.Left))
            {
                _debugMusic--;
                if (_debugMusic < 1)
                {
                    _debugMusic = 68;
                }
                PlayMusicFile("music\\" + _debugMusic + ".ogg");
            }
            if (_input.Keyboard.IsKeyPressed(Keys.Return))
            {
                PlayMusicFile("music\\" + _debugMusic + ".ogg");
            }
        }
        else if (_player._inMonsterForm && _menu._buttons["optionsBtn"].Selected)
        {
            if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Up) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.Right))
            {
                _debugMonImg++;
                if (_debugMonImg > 965)
                {
                    _debugMonImg = 1;
                }
                _player.MonsterForm(_debugMonImg, _debugMonSource);
            }
            else if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Down) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.Left))
            {
                _debugMonImg--;
                if (_debugMonImg < 1)
                {
                    _debugMonImg = 965;
                }
                _player.MonsterForm(_debugMonImg, _debugMonSource);
            }
        }
        else if (_menu._buttons["optionsBtn"].Selected)
        {
            if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Up) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.Right))
            {
                _debugSpellAni++;
                if (_debugSpellAni > 358)
                {
                    _debugSpellAni = 1;
                }
                SpellAnimation(_player, _debugSpellAni, 120);
            }
            else if (_input.Keyboard.IsKeyPressedAndRepeated(Keys.Down) || _input.Keyboard.IsKeyPressedAndRepeated(Keys.Left))
            {
                _debugSpellAni--;
                if (_debugSpellAni < 1)
                {
                    _debugSpellAni = 358;
                }
                SpellAnimation(_player, _debugSpellAni, 120);
            }
            if (_input.Keyboard.IsKeyPressed(Keys.Return))
            {
                SpellAnimation(_player, _debugSpellAni, 120);
            }
        }
        else
        {
            if (_viewingStuff() || _viewingLegend || _player._dead || !(DateTime.UtcNow.Subtract(_movementKeysDelay).TotalMilliseconds >= origKeyboardMoveDelay) || !_map._loaded)
            {
                return;
            }
            if ((_input.Keyboard.IsKeyPressedOrHeld(Keys.Right) || _input.Keyboard.IsKeyPressedOrHeld(Keys.V)) && _player._body._direction == 1)
            {
                _pfa = null;
                _autoAttackEntity = null;
                if (!_moving)
                {
                    _xOffset = -14.0;
                    _yOffset = -7.0;
                    _playerXOffset = 1;
                    _playerYOffset = 0;
                    _moving = true;
                    _map.useOffs = true;
                    _map.xOffset = _xOffset;
                    _map.yOffset = _yOffset;
                    _movementKeysDelay = DateTime.UtcNow;
                    origKeyboardMoveDelay = 50.0;
                }
            }
            else if ((_input.Keyboard.IsKeyPressedOrHeld(Keys.Left) || _input.Keyboard.IsKeyPressedOrHeld(Keys.Z)) && _player._body._direction == 3)
            {
                _pfa = null;
                _autoAttackEntity = null;
                if (!_moving)
                {
                    _xOffset = 14.0;
                    _yOffset = 7.0;
                    _playerXOffset = -1;
                    _playerYOffset = 0;
                    _moving = true;
                    _map.useOffs = true;
                    _map.xOffset = _xOffset;
                    _map.yOffset = _yOffset;
                    _movementKeysDelay = DateTime.UtcNow;
                    origKeyboardMoveDelay = 50.0;
                }
            }
            else if ((_input.Keyboard.IsKeyPressedOrHeld(Keys.Up) || _input.Keyboard.IsKeyPressedOrHeld(Keys.C)) && _player._body._direction == 0)
            {
                _pfa = null;
                _autoAttackEntity = null;
                if (!_moving)
                {
                    _xOffset = -14.0;
                    _yOffset = 7.0;
                    _playerXOffset = 0;
                    _playerYOffset = -1;
                    _moving = true;
                    _map.useOffs = true;
                    _map.xOffset = _xOffset;
                    _map.yOffset = _yOffset;
                    _movementKeysDelay = DateTime.UtcNow;
                    origKeyboardMoveDelay = 50.0;
                }
            }
            else if ((_input.Keyboard.IsKeyPressedOrHeld(Keys.Down) || _input.Keyboard.IsKeyPressedOrHeld(Keys.X)) && _player._body._direction == 2)
            {
                _pfa = null;
                _autoAttackEntity = null;
                if (!_moving)
                {
                    _xOffset = 14.0;
                    _yOffset = -7.0;
                    _playerXOffset = 0;
                    _playerYOffset = 1;
                    _moving = true;
                    _map.useOffs = true;
                    _map.xOffset = _xOffset;
                    _map.yOffset = _yOffset;
                    _movementKeysDelay = DateTime.UtcNow;
                    origKeyboardMoveDelay = 50.0;
                }
            }
            if ((_input.Keyboard.IsKeyPressedOrHeld(Keys.Right) || _input.Keyboard.IsKeyPressedOrHeld(Keys.V)) && _player._body._direction != 1 && !_moving)
            {
                _pfa = null;
                _autoAttackEntity = null;
                ChangeDirection(1);
            }
            else if ((_input.Keyboard.IsKeyPressedOrHeld(Keys.Left) || _input.Keyboard.IsKeyPressedOrHeld(Keys.Z)) && _player._body._direction != 3 && !_moving)
            {
                _pfa = null;
                _autoAttackEntity = null;
                ChangeDirection(3);
            }
            else if ((_input.Keyboard.IsKeyPressedOrHeld(Keys.Up) || _input.Keyboard.IsKeyPressedOrHeld(Keys.C)) && _player._body._direction != 0 && !_moving)
            {
                _pfa = null;
                _autoAttackEntity = null;
                ChangeDirection(0);
            }
            else if ((_input.Keyboard.IsKeyPressedOrHeld(Keys.Down) || _input.Keyboard.IsKeyPressedOrHeld(Keys.X)) && _player._body._direction != 2 && !_moving)
            {
                _pfa = null;
                _autoAttackEntity = null;
                ChangeDirection(2);
            }
        }
    }

    /// <summary>
    ///     Draws one frame: clears the screen, then either the world map or the in-game world (map,
    ///     weather, HUD menus, orbs, the active side panel, and per-entity name tags / health bars /
    ///     chat bubbles) plus any open floating windows (profile, legend, board, tooltips, prompts),
    ///     and finally the cursor.
    /// </summary>
    public void Render(double elapsedTime)
    {
        Gl.glClearColor(0f, 0f, 0f, 1f);
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
        // --- World-map screen vs. the in-game world ---
        if (_worldMap)
        {
            _renderer.DrawSprite(_worldMapSprite, 0);
            foreach (Town town in Town._List.Values)
            {
                town.Render(_renderer);
            }
        }
        else
        {
            // The game world, weather, and the HUD menus / orbs / spell bar.
            _map.Render(_renderer, elapsedTime);
            if (_enableWeather && !_initPlayer)
            {
                _weatherEffect.Render(_renderer);
            }
            _tabMap.Render(_renderer);
            _menu.Render(_renderer);
            _panelMenu.Render(_renderer);
            _miscMenu.Render(_renderer);
            if (_miscMenu._buttons["fullInvBtn"].Selected)
            {
                _renderer.DrawSprite(_fullInvPanel, 0);
            }
            if (_player._spellBar.Count > 0)
            {
                _renderer.DrawSprite(_spellBarBack, 0);
                foreach (SpellBar spellBar in _player._spellBar.Values)
                {
                    spellBar.Render(_renderer);
                }
            }
            _renderer.DrawSprite(_healthOrb, 0);
            _renderer.DrawSprite(_manaOrb, 0);
            // Active side panel (0=inventory, 1/6=skills, 2/7=spells, 3=chat, 4=stats, 5/10=actions, 8=info).
            if (_panelNum == 0)
            {
                if (_miscMenu._buttons["fullInvBtn"].Selected)
                {
                    _inventorySlots.ForEach(delegate (Slot x)
                    {
                        if (x.Number <= 12)
                        {
                            x._position.Y = 234.0;
                        }
                        else if (x.Number <= 24)
                        {
                            x._position.Y = 268.0;
                        }
                        else if (x.Number <= 36)
                        {
                            x._position.Y = 302.0;
                        }
                    });
                    _inventory.ForEach(delegate (InventoryItem x)
                    {
                        if (x._slot <= 36)
                        {
                            x._sprite.SetPosition(_inventorySlots[x._slot - 1]._position);
                        }
                        x.Render(_renderer);
                    });
                }
                else
                {
                    _inventorySlots.ForEach(delegate (Slot x)
                    {
                        if (x.Number <= 12)
                        {
                            x._position.Y = 336.0;
                        }
                        else if (x.Number <= 24)
                        {
                            x._position.Y = 370.0;
                        }
                        else if (x.Number <= 36)
                        {
                            x._position.Y = 404.0;
                        }
                    });
                    _inventory.ForEach(delegate (InventoryItem x)
                    {
                        if (x._slot <= 36)
                        {
                            x._sprite.SetPosition(_inventorySlots[x._slot - 1]._position);
                        }
                        if (x._slot < 36 || x._slot == 72)
                        {
                            x.Render(_renderer);
                        }
                    });
                }
                _inventoryMenu.Render(_renderer);
            }
            if (_panelNum == 1)
            {
                _skills.ForEach(delegate (Skill x)
                {
                    if (x._slot <= 36)
                    {
                        x.Render(_renderer);
                    }
                });
                _skillMenu.Render(_renderer);
            }
            if (_panelNum == 2)
            {
                _spells.ForEach(delegate (Spell x)
                {
                    if (x._slot <= 36)
                    {
                        x.Render(_renderer);
                    }
                });
                _spellMenu.Render(_renderer);
            }
            if (_panelNum == 3)
            {
                _chatMenu.Render(_renderer);
            }
            if (_panelNum == 4)
            {
                _statMenu.Render(_renderer);
            }
            if (_panelNum == 5)
            {
                _actions.ForEach(delegate (Action x)
                {
                    if (x._slot <= 36)
                    {
                        x.Render(_renderer);
                    }
                });
                _actionMenu.Render(_renderer);
            }
            if (_panelNum == 6)
            {
                _skills.ForEach(delegate (Skill x)
                {
                    if (x._slot > 36)
                    {
                        x.Render(_renderer);
                    }
                });
                _skillMenu.Render(_renderer);
            }
            if (_panelNum == 7)
            {
                _spells.ForEach(delegate (Spell x)
                {
                    if (x._slot > 36)
                    {
                        x.Render(_renderer);
                    }
                });
                _spellMenu.Render(_renderer);
            }
            if (_panelNum == 8)
            {
                _infoMenu.Render(_renderer);
            }
            if (_panelNum == 10)
            {
                _actions.ForEach(delegate (Action x)
                {
                    if (x._slot > 36)
                    {
                        x.Render(_renderer);
                    }
                });
                _actionMenu.Render(_renderer);
            }
            if (!_miscMenu._buttons["fullInvBtn"].Selected)
            {
                _infoBarMenu.Render(_renderer);
            }
            if (_initPlayer)
            {
                _input.Mouse.SetCursorLoading();
                _renderer.DrawSprite(_loadingUserPopup, 0);
            }
            if (_player.hasblind)
            {
                _renderer.DrawSprite(_blackScreen, 0);
                _renderer.DrawSprite(_blindedSprite, 0);
            }
            // Per-entity overlays: name tags, health bars, and chat bubbles.
            foreach (Entity entity in from z in _map._entities.Values.ToArray()
                                    orderby z.speakTime
                                    select z)
            {
                if (entity._displayTag != 0 && (entity is NPC || entity is Player))
                {
                    _renderer.DrawText(entity._nameTag, shade: true);
                }
                entity.DrawHealthBar(_renderer);
                if (entity._chatText._text != "")
                {
                    int bubbleHeight = entity._chatText._lines * 12;
                    Vector bubblePos = new Vector(entity._chatBubbleBack.GetPosition().X, entity._chatBubbleBack.GetPosition().Y, 0.0);
                    _renderer.DrawBorder(new Rectangle((int)bubblePos.X, (int)bubblePos.Y, (int)entity._chatBubbleBack.GetWidth(), (int)entity._chatBubbleBack.GetHeight()), Engine.Color.White, ignorebottommid: true);
                    _renderer.DrawPixel(bubblePos.X + 58.0, bubblePos.Y + (double)bubbleHeight + 6.0, new Engine.Color(0f, 0f, 0f, 0.5f));
                    _renderer.DrawPixel(bubblePos.X + 59.0, bubblePos.Y + (double)bubbleHeight + 6.0, new Engine.Color(0f, 0f, 0f, 0.5f));
                    _renderer.DrawPixel(bubblePos.X + 60.0, bubblePos.Y + (double)bubbleHeight + 6.0, new Engine.Color(0f, 0f, 0f, 0.5f));
                    _renderer.DrawPixel(bubblePos.X + 61.0, bubblePos.Y + (double)bubbleHeight + 6.0, new Engine.Color(0f, 0f, 0f, 0.5f));
                    _renderer.DrawPixel(bubblePos.X + 62.0, bubblePos.Y + (double)bubbleHeight + 6.0, new Engine.Color(0f, 0f, 0f, 0.5f));
                    _renderer.DrawPixel(bubblePos.X + 63.0, bubblePos.Y + (double)bubbleHeight + 6.0, new Engine.Color(0f, 0f, 0f, 0.5f));
                    _renderer.DrawPixel(bubblePos.X + 64.0, bubblePos.Y + (double)bubbleHeight + 6.0, new Engine.Color(0f, 0f, 0f, 0.5f));
                    _renderer.DrawPixel(bubblePos.X + 60.0, bubblePos.Y + (double)bubbleHeight + 7.0, new Engine.Color(0f, 0f, 0f, 0.5f));
                    _renderer.DrawPixel(bubblePos.X + 61.0, bubblePos.Y + (double)bubbleHeight + 7.0, new Engine.Color(0f, 0f, 0f, 0.5f));
                    _renderer.DrawPixel(bubblePos.X + 62.0, bubblePos.Y + (double)bubbleHeight + 7.0, new Engine.Color(0f, 0f, 0f, 0.5f));
                    _renderer.DrawPixel(bubblePos.X + 61.0, bubblePos.Y + (double)bubbleHeight + 8.0, new Engine.Color(0f, 0f, 0f, 0.5f));
                    _renderer.DrawPixel(bubblePos.X + 58.0, bubblePos.Y + (double)bubbleHeight + 7.0, Engine.Color.White);
                    _renderer.DrawPixel(bubblePos.X + 59.0, bubblePos.Y + (double)bubbleHeight + 7.0, Engine.Color.White);
                    _renderer.DrawPixel(bubblePos.X + 60.0, bubblePos.Y + (double)bubbleHeight + 8.0, Engine.Color.White);
                    _renderer.DrawPixel(bubblePos.X + 61.0, bubblePos.Y + (double)bubbleHeight + 9.0, Engine.Color.White);
                    _renderer.DrawPixel(bubblePos.X + 61.0, bubblePos.Y + (double)bubbleHeight + 10.0, Engine.Color.White);
                    _renderer.DrawPixel(bubblePos.X + 62.0, bubblePos.Y + (double)bubbleHeight + 8.0, Engine.Color.White);
                    _renderer.DrawPixel(bubblePos.X + 63.0, bubblePos.Y + (double)bubbleHeight + 7.0, Engine.Color.White);
                    _renderer.DrawPixel(bubblePos.X + 64.0, bubblePos.Y + (double)bubbleHeight + 7.0, Engine.Color.White);
                    _renderer.DrawSprite(entity._chatBubbleBack, 0);
                    _renderer.DrawText(entity._chatText);
                }
            }
            // Floating windows: drag icon, profiles, legends, menus, tooltips, prompts.
            if (_dragIcon._clicked && !_infoBarMenu._buttons["sysmsgBtn"].Held)
            {
                _dragIcon.Render(_renderer);
            }
            if (_viewingOthersProfile)
            {
                _othersProfileMenu.Render(_renderer);
                if (_profilePlayer != null)
                {
                    foreach (Equipment equipItem in _profilePlayer._equipment)
                    {
                        equipItem._image.SetPosition(_othersProfileMenu._background.GetPosition().X + equipItem._image._windowOffset.X, _othersProfileMenu._background.GetPosition().Y + equipItem._image._windowOffset.Y);
                        _renderer.DrawSprite(equipItem._image, 0);
                    }
                }
            }
            if (_viewingOthersLegend)
            {
                _othersLegendMenu.Render(_renderer);
                if (_profilePlayer != null)
                {
                    int othLegendShown = 0;
                    int othLegendCount = 0;
                    foreach (LegendMark legendMark in _profilePlayer._legendMarks)
                    {
                        othLegendCount++;
                        if (othLegendCount > _othersLegendIndex)
                        {
                            othLegendShown++;
                            if (othLegendShown > 14)
                            {
                                break;
                            }
                            legendMark._image.SetPosition(_othersLegendMenu._background.GetPosition().X + 30.0, _othersLegendMenu._background.GetPosition().Y + 43.0 + (double)(othLegendShown * 21));
                            legendMark._textObj.SetPosition(_othersLegendMenu._background.GetPosition().X + 60.0, _othersLegendMenu._background.GetPosition().Y + 47.0 + (double)(othLegendShown * 21));
                            _renderer.DrawSprite(legendMark._image, 0);
                            _renderer.DrawText(legendMark._textObj);
                        }
                    }
                }
            }
            if (_viewingProfile)
            {
                _profileMenu.Render(_renderer);
                _equipmentSlots.ForEach(delegate (Slot x)
                {
                    x._position.X = _profileMenu._position.X + x._windowOffset.X;
                    x._position.Y = _profileMenu._position.Y + x._windowOffset.Y;
                });
                _equipment.ForEach(delegate (InventoryItem x)
                {
                    x._sprite.SetPosition(_equipmentSlots[x._slot - 1]._position);
                    x.Render(_renderer);
                });
            }
            if (_viewingLegend)
            {
                _legendMenu.Render(_renderer);
                int legendShown = 0;
                int legendCount = 0;
                foreach (LegendMark legendMark2 in _player._legendMarks)
                {
                    legendCount++;
                    if (legendCount > _legendIndex)
                    {
                        legendShown++;
                        if (legendShown > 14)
                        {
                            break;
                        }
                        legendMark2._image.SetPosition(_legendMenu._background.GetPosition().X + 30.0, _legendMenu._background.GetPosition().Y + 43.0 + (double)(legendShown * 21));
                        legendMark2._textObj.SetPosition(_legendMenu._background.GetPosition().X + 60.0, _legendMenu._background.GetPosition().Y + 47.0 + (double)(legendShown * 21));
                        _renderer.DrawSprite(legendMark2._image, 0);
                        _renderer.DrawText(legendMark2._textObj);
                    }
                }
            }
            if (_menu._buttons["optionsBtn"].Selected)
            {
                _optionsMenu.Render(_renderer);
                if (_optionsMenu._buttons["macroBtn"].Selected)
                {
                    _macroMenu.Render(_renderer);
                }
                if (_optionsMenu._buttons["friendBtn"].Selected)
                {
                    _friendsMenu.Render(_renderer);
                }
            }
            if (_viewingSettings)
            {
                _settingsMenu.Render(_renderer);
            }
            if (_menu._buttons["mail1Btn"].Selected)
            {
                _boardMenu.Render(_renderer);
            }
            if (_viewingBoardList)
            {
                _boardListMenu.Render(_renderer);
            }
            if (_viewingMailList)
            {
                _mailListMenu.Render(_renderer);
            }
            if (_viewingPost)
            {
                _viewPostMenu.Render(_renderer);
            }
            if (_composingPost)
            {
                _composePostMenu.Render(_renderer);
            }
            if (_viewingUsers)
            {
                _usersMenu.Render(_renderer);
            }
            if (_viewCommands)
            {
                _commandMenu.Render(_renderer);
                int cmdShown = 0;
                int cmdCount = 0;
                foreach (Text cmdLine in commandBodyText)
                {
                    cmdCount++;
                    if (cmdCount > _commandIndex)
                    {
                        cmdShown++;
                        if (cmdShown > 25)
                        {
                            break;
                        }
                        cmdLine.SetPosition(_commandMenu._background.GetPosition().X + 28.0, _commandMenu._background.GetPosition().Y + 48.0 + (double)(cmdShown * 12));
                        _renderer.DrawText(cmdLine);
                    }
                }
            }
            if (_viewKeyboards)
            {
                _keyboardMenu.Render(_renderer);
                int keyShown = 0;
                int keyCount = 0;
                foreach (Text keyLine in keyboardBodyText)
                {
                    keyCount++;
                    if (keyCount > _keyboardIndex)
                    {
                        keyShown++;
                        if (keyShown > 25)
                        {
                            break;
                        }
                        keyLine.SetPosition(_keyboardMenu._background.GetPosition().X + 28.0, _keyboardMenu._background.GetPosition().Y + 48.0 + (double)(keyShown * 12));
                        _renderer.DrawText(keyLine);
                    }
                }
            }
            if (_viewingSense)
            {
                _senseMenu.Render(_renderer);
            }
            if (_viewingSign)
            {
                _signMenu.Render(_renderer);
            }
            if (_viewingDialog)
            {
                _standardDialogPopup.Render(_renderer);
            }
            if (_showGroupRequestPrompt)
            {
                _groupRequestPrompt.Render(_renderer);
            }
            if (_viewingGroupList)
            {
                _groupListMenu.Render(_renderer);
            }
            if (_tTT != "")
            {
                _renderer.DrawBorder(new Rectangle((int)_toolTipBack.GetPosition().X, (int)_toolTipBack.GetPosition().Y, (int)_toolTipBack.GetWidth(), (int)_toolTipBack.GetHeight()), new Engine.Color(1f, 1f, 1f, 1f));
                _renderer.DrawSprite(_toolTipBack, 0);
                _renderer.DrawText(_toolTipText);
            }
            if (_eTT != "")
            {
                _renderer.DrawBorder(new Rectangle((int)_entityToolTipBack.GetPosition().X, (int)_entityToolTipBack.GetPosition().Y, (int)_entityToolTipBack.GetWidth(), (int)_entityToolTipBack.GetHeight()), Engine.Color.Orange);
                _renderer.DrawSprite(_entityToolTipBack, 0);
                _renderer.DrawText(_entityToolTip);
            }
            if (_prompt._labels["promptText"]._text != "")
            {
                _prompt.Render(_renderer);
            }
            if (_deletePrompt._labels["delpromptText"]._text != "")
            {
                _deletePrompt.Render(_renderer);
            }
        }
        _input.Mouse.Render(_renderer);
        _renderer.Render();
    }

    /// <summary>
    ///     Builds every in-game UI surface once at startup: the HUD (orbs, side panels, info bar, misc
    ///     labels), the panels (inventory/skills/spells/chat/stats/actions/info), the menu bar and its
    ///     sub-menus (options/friends/macros/settings/board/mail/users), the profile/legend windows, and
    ///     the popups (prompts, sense/sign, NPC dialog, commands, group). The `// === ... ===` markers
    ///     below delimit each menu's construction.
    /// </summary>
    private void InitializeMenu()
    {
        Vector vector = default(Vector);
        vector = new Vector(190.0, 133.0, 0.0);
        // === Generic OK prompt ===
        _prompt = new ButtonMenu(_input, _font, vector.X, vector.Y, 263.0, 74.0, moveable: true);
        _prompt._sprite.Texture = _textureManager.Get("promptok");
        _prompt._sprite.SetPosition(vector);
        Engine.Button button = new Engine.Button(_textureManager, 184.0, 43.0, 70.0, 22.0, "butt001_F15", "butt001_F16", "", "butt001_F17", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button._onPressEvent = delegate
        {
            _prompt._labels["promptText"].ChangeText("");
            if (_prompt._closePost)
            {
                if (_viewingPost)
                {
                    _viewPostMenu._buttons["upViewBtn"].OnPress();
                }
                else if (_composingPost)
                {
                    _composePostMenu._buttons["cancelComposedBtn"].OnPress();
                }
                else if (_viewingBoardList)
                {
                    RequestBoard(_boardListMenu._boardID, 2, 0, 0);
                }
            }
        };
        Text value = DrawLabel("", Engine.Color.White, 6.0, 6.0, 250, "left", 5, shade: false, vector.X, vector.Y);
        _prompt._labels.Add("promptText", value);
        _prompt._buttons.Add("promptOkBtn", button);
        vector = new Vector(190.0, 133.0, 0.0);
        // === Delete prompt (delete / cancel) ===
        _deletePrompt = new ButtonMenu(_input, _font, vector.X, vector.Y, 263.0, 74.0, moveable: true);
        _deletePrompt._sprite.Texture = _textureManager.Get("promptok");
        _deletePrompt._sprite.SetPosition(vector);
        Engine.Button button2 = new Engine.Button(_textureManager, 184.0, 43.0, 70.0, 22.0, "butt001_F21", "butt001_F22", "", "butt001_F23", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button2._onPressEvent = delegate
        {
            _deletePrompt._labels["delpromptText"].ChangeText("");
        };
        Engine.Button button3 = new Engine.Button(_textureManager, 104.0, 43.0, 70.0, 22.0, "butt001_F18", "butt001_F19", "", "butt001_F20", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button3._onPressEvent = delegate
        {
            _deletePrompt._labels["delpromptText"].ChangeText("");
            if (_viewingPost)
            {
                RequestBoard(_viewPostMenu._boardID, 5, _viewPostMenu._selectedID, 0);
            }
            else if (_viewingBoardList)
            {
                RequestBoard(_boardListMenu._boardID, 5, _boardListMenu._selectedID, 0);
            }
        };
        Text value2 = DrawLabel("", Engine.Color.White, 6.0, 6.0, 250, "left", 5, shade: false, vector.X, vector.Y);
        _deletePrompt._labels.Add("delpromptText", value2);
        _deletePrompt._buttons.Add("delpromptDeleteBtn", button3);
        _deletePrompt._buttons.Add("delpromptCancelBtn", button2);
        // === HUD: health/mana orbs, portrait, status labels ===
        _healthOrb.Texture = _textureManager.Get("orb001_F0_C0");
        _healthOrb.SetPosition(-1.0, 299.0);
        _manaOrb.Texture = _textureManager.Get("orb002_F0_C0");
        _manaOrb.SetPosition(-1.0, 382.0);
        _fullInvPanel.Texture = _textureManager.Get("fullinvback");
        _fullInvPanel.SetPosition(87.0, 230.0);
        Text label = DrawLabel("", Engine.Color.Gray2, 561.0, 429.0, 72, "center");
        Text label2 = DrawLabel("", Engine.Color.Orange, 94.0, 317.0, 420, "left");
        TextField textField = new TextField(new Rectangle(94, 445, 490, 13), _font);
        textField._allowClick = false;
        TextField textField2 = new TextField(new Rectangle(94, 445, 490, 13), _font);
        textField2._allowClick = false;
        textField2._ignorespecial = false;
        Text label3 = DrawLabel("", Engine.Color.White, 94.0, 445.0, 420, "left");
        Text label4 = DrawLabel("", Engine.Color.White, 94.0, 445.0, 420, "left");
        Text label5 = DrawLabel("", Engine.Color.White, 287.0, 464.0, 120, "center", 1);
        Text label6 = DrawLabel("Quests", Engine.Color.BrightGreen, 467.0, 464.0, 65, "center");
        Text label7 = DrawLabel("XY: 0, 0", Engine.Color.Gray2, 157.0, 464.0, 65, "center");
        Text label8 = DrawLabel("", Engine.Color.White, 4.0, 4.0, 200, "left", 1);
        Text label9 = DrawLabel("", Engine.Color.White, 4.0, 20.0, 200, "left", 1);
        Text label10 = DrawLabel("", Engine.Color.White, 4.0, 36.0, 200, "left", 1);
        Engine.Button portraitFlap = new Engine.Button(_textureManager, 556.0, 310.0, 75.0, 106.0, "portrait_F0", "", "", "", new Texture[3]
        {
            _textureManager.Get("portrait_F0_C0"),
            _textureManager.Get("portrait_F1_C0"),
            _textureManager.Get("portrait_F2_C0")
        });
        if (_portraitFlap)
        {
            portraitFlap.Animate(0.0);
        }
        Engine.Button fullInvBtn = new Engine.Button(_textureManager, 534.0, 313.0, 16.0, 15.0, "fullinv_F1", "fullinv_F2", "", "fullinv_F1");
        fullInvBtn.Enabled = false;
        // === Inventory panel ===
        _inventoryMenu = new ButtonMenu(_input, _font);
        _inventoryMenu._background.Texture = _textureManager.Get("panelnums");
        _inventoryMenu._background.SetPosition(96.0, 336.0);
        // === Skill panel ===
        _skillMenu = new ButtonMenu(_input, _font);
        _skillMenu._background.Texture = _textureManager.Get("panelnums");
        _skillMenu._background.SetPosition(96.0, 336.0);
        // === Spell panel ===
        _spellMenu = new ButtonMenu(_input, _font);
        _spellMenu._background.Texture = _textureManager.Get("panelnums");
        _spellMenu._background.SetPosition(96.0, 336.0);
        // === Chat panel ===
        _chatMenu = new ButtonMenu(_input, _font);
        Text label11 = DrawLabel("", Engine.Color.White, 94.0, 339.0, 420, "left");
        Engine.Button button4 = new Engine.Button(_textureManager, 514.0, 338.0, 13.0, 12.0, "scroll_F0", "scroll_F1", "", "scroll_F1");
        button4.Enabled = false;
        Engine.Button button5 = new Engine.Button(_textureManager, 514.0, 422.0, 13.0, 12.0, "scroll_F2", "scroll_F3", "", "scroll_F3");
        button5.Enabled = false;
        Engine.Button button6 = new Engine.Button(_textureManager, 514.0, 410.0, 13.0, 12.0, "scroll_F4", "scroll_F4");
        button6.Hidden = true;
        Engine.Button button7 = new Engine.Button(_textureManager, 514.0, 350.0, 13.0, 72.0, "scroll_F5", "scroll_F5", "", "", null, null, 6);
        button4._onHeldEvent = delegate
        {
            if (_chatIndex > 0)
            {
                int count42 = _chatList.Count;
                _chatIndex--;
                RepositionScroller(_chatIndex, count42 - 8, 60, _chatMenu, _chatMenu._buttons["chatScrollerBtn"], 514, 350);
            }
        };
        button4._onPressEvent = delegate
        {
            if (_chatIndex > 0)
            {
                int count41 = _chatList.Count;
                _chatIndex--;
                RepositionScroller(_chatIndex, count41 - 8, 60, _chatMenu, _chatMenu._buttons["chatScrollerBtn"], 514, 350);
            }
        };
        button5._onHeldEvent = delegate
        {
            if (_chatList.Count - 8 > 0 && _chatIndex < _chatList.Count - 8)
            {
                int count40 = _chatList.Count;
                _chatIndex++;
                RepositionScroller(_chatIndex, count40 - 8, 60, _chatMenu, _chatMenu._buttons["chatScrollerBtn"], 514, 350);
            }
        };
        button5._onPressEvent = delegate
        {
            if (_chatList.Count - 8 > 0 && _chatIndex < _chatList.Count - 8)
            {
                int count39 = _chatList.Count;
                _chatIndex++;
                RepositionScroller(_chatIndex, count39 - 8, 60, _chatMenu, _chatMenu._buttons["chatScrollerBtn"], 514, 350);
            }
        };
        button6._onHeldEvent = delegate
        {
            int count38 = _chatList.Count;
            double num32 = (double)(_input.Mouse.Position.Y - 350f - 6f) / 66.0;
            int num33 = (int)Math.Round(num32 * (double)(count38 - 8));
            _chatIndex = ((num33 >= 0) ? ((num33 > count38 - 8) ? (count38 - 8) : num33) : 0);
            RepositionScroller(_chatIndex, count38 - 8, 60, _chatMenu, _chatMenu._buttons["chatScrollerBtn"], 514, 350);
        };
        button7._onPressEvent = delegate
        {
            int count37 = _chatList.Count;
            double num30 = (double)(_input.Mouse.Position.Y - 350f - 6f) / 66.0;
            int num31 = (int)Math.Round(num30 * (double)(count37 - 8));
            _chatIndex = ((num31 >= 0) ? ((num31 > count37 - 8) ? (count37 - 8) : num31) : 0);
            RepositionScroller(_chatIndex, count37 - 8, 60, _chatMenu, _chatMenu._buttons["chatScrollerBtn"], 514, 350);
        };
        _chatMenu.AddLabel("chatPanel", label11);
        _chatMenu.AddButton("chatScrollBackBtn", button7);
        _chatMenu.AddButton("chatScrollerBtn", button6);
        _chatMenu.AddButton("chatScrollUpBtn", button4);
        _chatMenu.AddButton("chatScrollDownBtn", button5);
        // === Stat panel ===
        _statMenu = new ButtonMenu(_input, _font);
        _statMenu._background.Texture = _textureManager.Get("stat001");
        _statMenu._background.SetPosition(87.0, 332.0);
        Text label12 = DrawLabel(_player._str.ToString(), Engine.Color.Gray2, 144.0, 340.0, 18, "right");
        Text label13 = DrawLabel(_player._int.ToString(), Engine.Color.Gray2, 144.0, 360.0, 18, "right");
        Text label14 = DrawLabel(_player._wis.ToString(), Engine.Color.Gray2, 144.0, 380.0, 18, "right");
        Text label15 = DrawLabel(_player._con.ToString(), Engine.Color.Gray2, 144.0, 400.0, 18, "right");
        Text label16 = DrawLabel(_player._dex.ToString(), Engine.Color.Gray2, 144.0, 420.0, 18, "right");
        Engine.Button button8 = new Engine.Button(_textureManager, 166.0, 337.0, 15.0, 18.0, "levelup_F0", "levelup_F2", "", "levelup_F0", new Texture[2]
        {
            _textureManager.Get("levelup_F0_C0"),
            _textureManager.Get("levelup_F1_C0")
        });
        button8.RepeatFlashAnimate(420.0);
        button8.Enabled = false;
        button8._onPressEvent = delegate
        {
            if (_player._availstats > 0)
            {
                AddStat(0);
            }
        };
        Engine.Button button9 = new Engine.Button(_textureManager, 166.0, 357.0, 15.0, 18.0, "levelup_F0", "levelup_F2", "", "levelup_F0", new Texture[2]
        {
            _textureManager.Get("levelup_F0_C0"),
            _textureManager.Get("levelup_F1_C0")
        });
        button9.RepeatFlashAnimate(420.0);
        button9.Enabled = false;
        button9._onPressEvent = delegate
        {
            if (_player._availstats > 0)
            {
                AddStat(1);
            }
        };
        Engine.Button button10 = new Engine.Button(_textureManager, 166.0, 377.0, 15.0, 18.0, "levelup_F0", "levelup_F2", "", "levelup_F0", new Texture[2]
        {
            _textureManager.Get("levelup_F0_C0"),
            _textureManager.Get("levelup_F1_C0")
        });
        button10.RepeatFlashAnimate(420.0);
        button10.Enabled = false;
        button10._onPressEvent = delegate
        {
            if (_player._availstats > 0)
            {
                AddStat(2);
            }
        };
        Engine.Button button11 = new Engine.Button(_textureManager, 166.0, 397.0, 15.0, 18.0, "levelup_F0", "levelup_F2", "", "levelup_F0", new Texture[2]
        {
            _textureManager.Get("levelup_F0_C0"),
            _textureManager.Get("levelup_F1_C0")
        });
        button11.RepeatFlashAnimate(420.0);
        button11.Enabled = false;
        button11._onPressEvent = delegate
        {
            if (_player._availstats > 0)
            {
                AddStat(3);
            }
        };
        Engine.Button button12 = new Engine.Button(_textureManager, 166.0, 417.0, 15.0, 18.0, "levelup_F0", "levelup_F2", "", "levelup_F0", new Texture[2]
        {
            _textureManager.Get("levelup_F0_C0"),
            _textureManager.Get("levelup_F1_C0")
        });
        button12.RepeatFlashAnimate(420.0);
        button12.Enabled = false;
        button12._onPressEvent = delegate
        {
            if (_player._availstats > 0)
            {
                AddStat(4);
            }
        };
        Text label17 = DrawLabel(_player._curHP.ToString("#,0"), Engine.Color.Gray2, 225.0, 345.0, 55, "right");
        Text label18 = DrawLabel(_player._maxHP.ToString("#,0"), Engine.Color.Gray2, 289.0, 345.0, 55, "right");
        Text label19 = DrawLabel(_player._curMP.ToString("#,0"), Engine.Color.Gray2, 225.0, 366.0, 55, "right");
        Text label20 = DrawLabel(_player._maxMP.ToString("#,0"), Engine.Color.Gray2, 289.0, 366.0, 55, "right");
        Text label21 = DrawLabel(_player._exp.ToString("#,0"), Engine.Color.Gray2, 232.0, 396.0, 85, "right");
        Text label22 = DrawLabel(getItemSlot(72)._amount.ToString("#,0"), Engine.Color.Gray2, 237.0, 417.0, 80, "right");
        Text label23 = DrawLabel(_player._lev.ToString(), Engine.Color.Gray2, 410.0, 396.0, 20, "right");
        Text label24 = DrawLabel(_player._tnl.ToString("#,0"), Engine.Color.Gray2, 375.0, 417.0, 55, "right");
        Text label25 = DrawLabel(_player._atk, Engine.Color.Gray2, 385.0, 345.0, 45, "right");
        Text label26 = DrawLabel(_player._def, Engine.Color.Gray2, 385.0, 366.0, 45, "right");
        Text label27 = DrawLabel(_player._mr.ToString(), Engine.Color.Gray2, 495.0, 347.0, 20, "right");
        Text label28 = DrawLabel(_player._ac.ToString(), Engine.Color.Gray2, 495.0, 369.0, 20, "right");
        Text label29 = DrawLabel(_player._dmg.ToString(), Engine.Color.Gray2, 495.0, 391.0, 20, "right");
        Text label30 = DrawLabel(_player._hit.ToString(), Engine.Color.Gray2, 495.0, 413.0, 20, "right");
        _statMenu.AddLabel("expLabel", label21);
        _statMenu.AddLabel("goldLabel", label22);
        _statMenu.AddLabel("levLabel", label23);
        _statMenu.AddLabel("tnlLabel", label24);
        _statMenu.AddLabel("atkLabel", label25);
        _statMenu.AddLabel("defLabel", label26);
        _statMenu.AddLabel("mrLabel", label27);
        _statMenu.AddLabel("acLabel", label28);
        _statMenu.AddLabel("dmgLabel", label29);
        _statMenu.AddLabel("hitLabel", label30);
        _statMenu.AddLabel("curHpLabel", label17);
        _statMenu.AddLabel("maxHpLabel", label18);
        _statMenu.AddLabel("curMpLabel", label19);
        _statMenu.AddLabel("maxMpLabel", label20);
        _statMenu.AddLabel("strLabel", label12);
        _statMenu.AddLabel("intLabel", label13);
        _statMenu.AddLabel("wisLabel", label14);
        _statMenu.AddLabel("conLabel", label15);
        _statMenu.AddLabel("dexLabel", label16);
        _statMenu.AddButton("strBtn", button8);
        _statMenu.AddButton("intBtn", button9);
        _statMenu.AddButton("wisBtn", button10);
        _statMenu.AddButton("conBtn", button11);
        _statMenu.AddButton("dexBtn", button12);
        // === Action panel ===
        _actionMenu = new ButtonMenu(_input, _font);
        _actionMenu._background.Texture = _textureManager.Get("panelnums");
        _actionMenu._background.SetPosition(96.0, 336.0);
        // === Info panel ===
        _infoMenu = new ButtonMenu(_input, _font);
        Text label31 = DrawLabel("", Engine.Color.Orange, 94.0, 339.0, 420, "left");
        Engine.Button button13 = new Engine.Button(_textureManager, 514.0, 338.0, 13.0, 12.0, "scroll_F0", "scroll_F1", "", "scroll_F1");
        button13.Enabled = false;
        Engine.Button button14 = new Engine.Button(_textureManager, 514.0, 422.0, 13.0, 12.0, "scroll_F2", "scroll_F3", "", "scroll_F3");
        button14.Enabled = false;
        Engine.Button button15 = new Engine.Button(_textureManager, 514.0, 410.0, 13.0, 12.0, "scroll_F4", "scroll_F4");
        button15.Hidden = true;
        Engine.Button button16 = new Engine.Button(_textureManager, 514.0, 350.0, 13.0, 72.0, "scroll_F5", "scroll_F5", "", "", null, null, 6);
        button13._onHeldEvent = delegate
        {
            if (_infoBarIndex > 0)
            {
                int count36 = _infoBarList.Count;
                _infoBarIndex--;
                RepositionScroller(_infoBarIndex, count36 - 8, 60, _infoMenu, _infoMenu._buttons["infoScrollerBtn"], 514, 350);
            }
        };
        button13._onPressEvent = delegate
        {
            if (_infoBarIndex > 0)
            {
                int count35 = _infoBarList.Count;
                _infoBarIndex--;
                RepositionScroller(_infoBarIndex, count35 - 8, 60, _infoMenu, _infoMenu._buttons["infoScrollerBtn"], 514, 350);
            }
        };
        button14._onHeldEvent = delegate
        {
            if (_infoBarList.Count - 8 > 0 && _infoBarIndex < _infoBarList.Count - 8)
            {
                int count34 = _infoBarList.Count;
                _infoBarIndex++;
                RepositionScroller(_infoBarIndex, count34 - 8, 60, _infoMenu, _infoMenu._buttons["infoScrollerBtn"], 514, 350);
            }
        };
        button14._onPressEvent = delegate
        {
            if (_infoBarList.Count - 8 > 0 && _infoBarIndex < _infoBarList.Count - 8)
            {
                int count33 = _infoBarList.Count;
                _infoBarIndex++;
                RepositionScroller(_infoBarIndex, count33 - 8, 60, _infoMenu, _infoMenu._buttons["infoScrollerBtn"], 514, 350);
            }
        };
        button15._onHeldEvent = delegate
        {
            int count32 = _infoBarList.Count;
            double num28 = (double)(_input.Mouse.Position.Y - 350f - 6f) / 66.0;
            int num29 = (int)Math.Round(num28 * (double)(count32 - 8));
            _infoBarIndex = ((num29 >= 0) ? ((num29 > count32 - 8) ? (count32 - 8) : num29) : 0);
            RepositionScroller(_infoBarIndex, count32 - 8, 60, _infoMenu, _infoMenu._buttons["infoScrollerBtn"], 514, 350);
        };
        button16._onPressEvent = delegate
        {
            int count31 = _infoBarList.Count;
            double num26 = (double)(_input.Mouse.Position.Y - 350f - 6f) / 66.0;
            int num27 = (int)Math.Round(num26 * (double)(count31 - 8));
            _infoBarIndex = ((num27 >= 0) ? ((num27 > count31 - 8) ? (count31 - 8) : num27) : 0);
            RepositionScroller(_infoBarIndex, count31 - 8, 60, _infoMenu, _infoMenu._buttons["infoScrollerBtn"], 514, 350);
        };
        _infoMenu.AddLabel("infoPanel", label31);
        _infoMenu.AddButton("infoScrollBackBtn", button16);
        _infoMenu.AddButton("infoScrollerBtn", button15);
        _infoMenu.AddButton("infoScrollUpBtn", button13);
        _infoMenu.AddButton("infoScrollDownBtn", button14);
        // === Panel selector (the panel-tab row) ===
        _panelMenu = new ButtonMenu(_input, _font);
        Engine.Button inventoryBtn = new Engine.Button(_textureManager, 531.0, 330.0, 22.0, 23.0, "", "gbicon01_F0");
        Engine.Button skillBtn = new Engine.Button(_textureManager, 531.0, 353.0, 22.0, 23.0, "", "gbicon01_F1");
        Engine.Button spellBtn = new Engine.Button(_textureManager, 531.0, 376.0, 22.0, 23.0, "", "gbicon01_F2");
        Engine.Button chatBtn = new Engine.Button(_textureManager, 531.0, 399.0, 22.0, 23.0, "", "gbicon01_F3");
        chatBtn.Selected = true;
        Engine.Button statBtn = new Engine.Button(_textureManager, 531.0, 422.0, 22.0, 23.0, "", "gbicon01_F4");
        Engine.Button actionBtn = new Engine.Button(_textureManager, 531.0, 445.0, 22.0, 23.0, "", "gbicon01_F1");
        inventoryBtn._onPressEvent = delegate
        {
            if (_panelNum != 0)
            {
                _panelNum = 0;
                ToggleSelectedButtons(_panelMenu, inventoryBtn);
                fullInvBtn.Enabled = true;
            }
        };
        skillBtn._onPressEvent = delegate
        {
            if (_panelNum != 1)
            {
                _panelNum = 1;
                ToggleSelectedButtons(_panelMenu, skillBtn);
                _inventoryMenu._background.SetPosition(96.0, 336.0);
                fullInvBtn.Enabled = false;
                fullInvBtn.Selected = false;
            }
        };
        spellBtn._onPressEvent = delegate
        {
            if (_panelNum != 2)
            {
                _panelNum = 2;
                ToggleSelectedButtons(_panelMenu, spellBtn);
                _inventoryMenu._background.SetPosition(96.0, 336.0);
                fullInvBtn.Enabled = false;
                fullInvBtn.Selected = false;
            }
        };
        chatBtn._onPressEvent = delegate
        {
            if (_panelNum != 3)
            {
                _panelNum = 3;
                ToggleSelectedButtons(_panelMenu, chatBtn);
                _inventoryMenu._background.SetPosition(96.0, 336.0);
                fullInvBtn.Enabled = false;
                fullInvBtn.Selected = false;
            }
        };
        statBtn._onPressEvent = delegate
        {
            if (_panelNum != 4)
            {
                _panelNum = 4;
                ToggleSelectedButtons(_panelMenu, statBtn);
                _inventoryMenu._background.SetPosition(96.0, 336.0);
                fullInvBtn.Enabled = false;
                fullInvBtn.Selected = false;
            }
        };
        actionBtn._onPressEvent = delegate
        {
            if (_panelNum != 5)
            {
                _panelNum = 5;
                ToggleSelectedButtons(_panelMenu, actionBtn);
                _inventoryMenu._background.SetPosition(96.0, 336.0);
                fullInvBtn.Enabled = false;
                fullInvBtn.Selected = false;
            }
        };
        _panelMenu.AddButton("inventoryBtn", inventoryBtn);
        _panelMenu.AddButton("skillBtn", skillBtn);
        _panelMenu.AddButton("spellBtn", spellBtn);
        _panelMenu.AddButton("chatBtn", chatBtn);
        _panelMenu.AddButton("statBtn", statBtn);
        _panelMenu.AddButton("actionBtn", actionBtn);
        vector = new Vector(4.0, 4.0, 0.0);
        // === Other player's profile window ===
        _othersProfileMenu = new ButtonMenu(_input, _font, vector.X, vector.Y, 276.0, 309.0, moveable: true);
        _othersProfileMenu._background.Texture = _textureManager.Get("equip04_F0_C0");
        _othersProfileMenu._background.SetPosition(vector);
        Text label32 = DrawLabel("", Engine.Color.Gray2, 128.0, 22.0, 115, "center", 0, shade: false, vector.X, vector.Y);
        Text label33 = DrawLabel("", Engine.Color.Gray2, 32.0, 22.0, 55, "center", 0, shade: false, vector.X, vector.Y);
        Text label34 = DrawLabel("", Engine.Color.Gray2, 64.0, 54.0, 85, "left", 0, shade: false, vector.X, vector.Y);
        Text label35 = DrawLabel("", Engine.Color.Gray2, 161.0, 54.0, 115, "left", 0, shade: false, vector.X, vector.Y);
        Text label36 = DrawLabel("", Engine.Color.Gray2, 64.0, 72.0, 115, "left", 0, shade: false, vector.X, vector.Y);
        Engine.Button otherslegendProfileMenuBtn = new Engine.Button(_textureManager, 51.0, 110.0, 35.0, 27.0, "legendi_F0", "legendi_F1", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        Engine.Button button17 = new Engine.Button(_textureManager, 213.0, 228.0, 35.0, 27.0, "equip05_F0", "equip05_F1", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        Engine.Button button18 = new Engine.Button(_textureManager, 36.0, 228.0, 35.0, 27.0, "equip06_F0", "equip06_F2", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        Engine.Button button19 = new Engine.Button(_textureManager, 191.0, 270.0, 70.0, 22.0, "butt001_F33", "butt001_F34", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button17._onPressEvent = delegate
        {
            if (_profilePlayer != null)
            {
                if (_profilePlayer._allowGrouping)
                {
                    GroupRequestPacket groupRequestPacket2 = new GroupRequestPacket(0, _profilePlayer._name);
                    GameWindow.ClientSocket.Send(groupRequestPacket2.Data);
                }
                else
                {
                    SystemMsg("Refuses to join a group.", 3);
                }
            }
        };
        otherslegendProfileMenuBtn._onPressEvent = delegate
        {
            if (!_viewingOthersLegend)
            {
                otherslegendProfileMenuBtn.Selected = true;
                _viewingOthersLegend = true;
            }
        };
        button19._onPressEvent = delegate
        {
            ResetOthersProfileUI();
        };
        _othersProfileMenu.AddLabel("othersprofileNameLabel", label32);
        _othersProfileMenu.AddLabel("othersprofileLevelLabel", label33);
        _othersProfileMenu.AddLabel("othersprofileGuildLabel", label34);
        _othersProfileMenu.AddLabel("othersprofileRankLabel", label35);
        _othersProfileMenu.AddLabel("othersprofileTitleLabel", label36);
        _othersProfileMenu.AddButton("otherscloseProfileMenuBtn", button19);
        _othersProfileMenu.AddButton("othersgroupProfileMenuBtn", button17);
        _othersProfileMenu.AddButton("othersexchangeProfileMenuBtn", button18);
        _othersProfileMenu.AddButton("otherslegendProfileMenuBtn", otherslegendProfileMenuBtn);
        vector = new Vector(100.0, 4.0, 0.0);
        // === Other player's legend window ===
        _othersLegendMenu = new ButtonMenu(_input, _font, vector.X, vector.Y, 451.0, 418.0, moveable: true);
        _othersLegendMenu._background.Texture = _textureManager.Get("legend_F0_C0");
        _othersLegendMenu._background.SetPosition(vector);
        Text label37 = DrawLabel("", Engine.Color.Gray2, 178.0, 30.0, 115, "center", 0, shade: false, vector.X, vector.Y);
        Engine.Button button20 = new Engine.Button(_textureManager, 365.0, 377.0, 70.0, 22.0, "butt001_F33", "butt001_F34", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button20._onPressEvent = delegate
        {
            ResetOthersLegendUI();
        };
        Engine.Button button21 = new Engine.Button(_textureManager, 421.0, 60.0, 13.0, 12.0, "scroll_F0", "scroll_F1", "", "scroll_F1", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button21.Enabled = false;
        Engine.Button button22 = new Engine.Button(_textureManager, 421.0, 348.0, 13.0, 12.0, "scroll_F2", "scroll_F3", "", "scroll_F3", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button22.Enabled = false;
        Engine.Button button23 = new Engine.Button(_textureManager, 421.0, 72.0, 13.0, 12.0, "scroll_F4", "scroll_F4", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button23.Hidden = true;
        Engine.Button button24 = new Engine.Button(_textureManager, 421.0, 72.0, 13.0, 276.0, "scroll_F5", "scroll_F5", "", "", null, null, 23, multHorzontal: false, vector.X, vector.Y);
        button21._onHeldEvent = delegate
        {
            if (_othersLegendIndex > 0)
            {
                int count30 = _profilePlayer._legendMarks.Count;
                _othersLegendIndex--;
                RepositionScroller(_othersLegendIndex, count30 - 14, 264, _othersLegendMenu, _othersLegendMenu._buttons["otherslegendScrollerBtn"], 421, 72);
            }
        };
        button21._onPressEvent = delegate
        {
            if (_othersLegendIndex > 0)
            {
                int count29 = _profilePlayer._legendMarks.Count;
                _othersLegendIndex--;
                RepositionScroller(_othersLegendIndex, count29 - 14, 264, _othersLegendMenu, _othersLegendMenu._buttons["otherslegendScrollerBtn"], 421, 72);
            }
        };
        button22._onHeldEvent = delegate
        {
            if (_profilePlayer._legendMarks.Count - 14 > 0 && _othersLegendIndex < _profilePlayer._legendMarks.Count - 14)
            {
                int count28 = _profilePlayer._legendMarks.Count;
                _othersLegendIndex++;
                RepositionScroller(_othersLegendIndex, count28 - 14, 264, _othersLegendMenu, _othersLegendMenu._buttons["otherslegendScrollerBtn"], 421, 72);
            }
        };
        button22._onPressEvent = delegate
        {
            if (_profilePlayer._legendMarks.Count - 14 > 0 && _othersLegendIndex < _profilePlayer._legendMarks.Count - 14)
            {
                int count27 = _profilePlayer._legendMarks.Count;
                _othersLegendIndex++;
                RepositionScroller(_othersLegendIndex, count27 - 14, 264, _othersLegendMenu, _othersLegendMenu._buttons["otherslegendScrollerBtn"], 421, 72);
            }
        };
        button23._onHeldEvent = delegate
        {
            int count26 = _profilePlayer._legendMarks.Count;
            double num24 = ((double)_input.Mouse.Position.Y - _othersLegendMenu._position.Y - 72.0 - 6.0) / 270.0;
            int num25 = (int)Math.Round(num24 * (double)(count26 - 14));
            _othersLegendIndex = ((num25 >= 0) ? ((num25 > count26 - 14) ? (count26 - 14) : num25) : 0);
            RepositionScroller(_othersLegendIndex, count26 - 14, 264, _othersLegendMenu, _othersLegendMenu._buttons["otherslegendScrollerBtn"], 421, 72);
        };
        button24._onPressEvent = delegate
        {
            int count25 = _profilePlayer._legendMarks.Count;
            double num22 = ((double)_input.Mouse.Position.Y - _othersLegendMenu._position.Y - 72.0 - 6.0) / 270.0;
            int num23 = (int)Math.Round(num22 * (double)(count25 - 14));
            _othersLegendIndex = ((num23 >= 0) ? ((num23 > count25 - 14) ? (count25 - 14) : num23) : 0);
            RepositionScroller(_othersLegendIndex, count25 - 14, 264, _othersLegendMenu, _othersLegendMenu._buttons["otherslegendScrollerBtn"], 421, 72);
        };
        _othersLegendMenu.AddButton("otherslegendScrollBackBtn", button24);
        _othersLegendMenu.AddButton("otherslegendScrollerBtn", button23);
        _othersLegendMenu.AddButton("otherslegendScrollUpBtn", button21);
        _othersLegendMenu.AddButton("otherslegendScrollDownBtn", button22);
        _othersLegendMenu.AddLabel("otherslegendNameLabel", label37);
        _othersLegendMenu.AddButton("closeothersLegendMenuBtn", button20);
        vector = new Vector(4.0, 4.0, 0.0);
        // === Own profile window ===
        _profileMenu = new ButtonMenu(_input, _font, vector.X, vector.Y, 292.0, 308.0, moveable: true);
        _profileMenu._background.Texture = _textureManager.Get("equip01");
        _profileMenu._background.SetPosition(vector);
        Engine.Button nationFlagBtn = new Engine.Button(_textureManager, 34.0, 47.0, 41.0, 51.0, "nation_F0", "", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        nationFlagBtn._onPressEvent = delegate
        {
            if (_curFlag == 10)
            {
                _curFlag = 0;
            }
            else
            {
                _curFlag++;
            }
            nationFlagBtn._baseImage = _textureManager.Get("nation_F" + _curFlag + "_C0");
            nationFlagBtn.Sprite.Texture = nationFlagBtn._baseImage;
        };
        _profileMenu._buttons.Add("nationFlagBtn", nationFlagBtn);
        Text label38 = DrawLabel("", Engine.Color.Gray2, 144.0, 22.0, 115, "center", 0, shade: false, vector.X, vector.Y);
        Text label39 = DrawLabel("", Engine.Color.Gray2, 48.0, 22.0, 55, "center", 0, shade: false, vector.X, vector.Y);
        Text label40 = DrawLabel("", Engine.Color.Gray2, 80.0, 54.0, 85, "left", 0, shade: false, vector.X, vector.Y);
        Text label41 = DrawLabel("", Engine.Color.Gray2, 177.0, 54.0, 115, "left", 0, shade: false, vector.X, vector.Y);
        Text label42 = DrawLabel("", Engine.Color.Gray2, 80.0, 72.0, 115, "left", 0, shade: false, vector.X, vector.Y);
        Engine.Button legendProfileMenuBtn = new Engine.Button(_textureManager, 67.0, 110.0, 35.0, 27.0, "legendi_F0", "legendi_F1", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        Engine.Button groupProfileMenuBtn = new Engine.Button(_textureManager, 229.0, 228.0, 35.0, 27.0, "equip05_F0", "equip05_F1", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        if (_allowGrouping)
        {
            groupProfileMenuBtn._baseImage = _textureManager.Get("equip05_F2_C0");
            groupProfileMenuBtn._clickedImage = _textureManager.Get("equip05_F3_C0");
            groupProfileMenuBtn.Sprite.Texture = groupProfileMenuBtn._baseImage;
        }
        Engine.Button button25 = new Engine.Button(_textureManager, 207.0, 270.0, 70.0, 22.0, "butt001_F33", "butt001_F34", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        legendProfileMenuBtn._onPressEvent = delegate
        {
            if (!_viewingLegend)
            {
                legendProfileMenuBtn.Selected = true;
                _viewingLegend = true;
            }
        };
        groupProfileMenuBtn._onPressEvent = delegate
        {
            if (_allowGrouping)
            {
                groupProfileMenuBtn._baseImage = _textureManager.Get("equip05_F0_C0");
                groupProfileMenuBtn._clickedImage = _textureManager.Get("equip05_F1_C0");
                groupProfileMenuBtn.Sprite.Texture = groupProfileMenuBtn._baseImage;
                _allowGrouping = false;
                SendProfileData();
                SaveClientSettings();
                _settingsMenu._labels["settings5Lab"].ChangeText(_settingsMenu._labels["settings5Lab"]._text.Remove(_settingsMenu._labels["settings5Lab"]._text.IndexOf(" : ") + 3) + "OFF");
            }
            else
            {
                groupProfileMenuBtn._baseImage = _textureManager.Get("equip05_F2_C0");
                groupProfileMenuBtn._clickedImage = _textureManager.Get("equip05_F3_C0");
                groupProfileMenuBtn.Sprite.Texture = groupProfileMenuBtn._baseImage;
                _allowGrouping = true;
                SendProfileData();
                SaveClientSettings();
                _settingsMenu._labels["settings5Lab"].ChangeText(_settingsMenu._labels["settings5Lab"]._text.Remove(_settingsMenu._labels["settings5Lab"]._text.IndexOf(" : ") + 3) + "ON");
            }
        };
        button25._onPressEvent = delegate
        {
            ResetProfileUI();
        };
        Engine.Button button26 = new Engine.Button(_textureManager, 121.0, 114.0, 17.0, 17.0, "", "", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        Engine.Button button27 = new Engine.Button(_textureManager, 186.0, 126.0, 17.0, 17.0, "", "", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        Engine.Button button28 = new Engine.Button(_textureManager, 210.0, 153.0, 33.0, 33.0, "", "", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        Engine.Button button29 = new Engine.Button(_textureManager, 94.0, 196.0, 17.0, 17.0, "", "", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        Engine.Button button30 = new Engine.Button(_textureManager, 122.0, 209.0, 17.0, 17.0, "", "", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        Engine.Button button31 = new Engine.Button(_textureManager, 152.0, 194.0, 17.0, 17.0, "", "", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        Engine.Button button32 = new Engine.Button(_textureManager, 122.0, 235.0, 17.0, 17.0, "", "", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        Engine.Button button33 = new Engine.Button(_textureManager, 165.0, 256.0, 17.0, 17.0, "", "", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        Engine.Button button34 = new Engine.Button(_textureManager, 76.0, 153.0, 33.0, 33.0, "", "", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        Engine.Button button35 = new Engine.Button(_textureManager, 153.0, 92.0, 17.0, 17.0, "", "", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        Engine.Button button36 = new Engine.Button(_textureManager, 143.0, 153.0, 33.0, 33.0, "", "", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        Engine.Button button37 = new Engine.Button(_textureManager, 209.0, 196.0, 17.0, 17.0, "", "", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        Engine.Button button38 = new Engine.Button(_textureManager, 184.0, 217.0, 17.0, 17.0, "", "", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        Engine.Button button39 = new Engine.Button(_textureManager, 218.0, 92.0, 17.0, 17.0, "", "", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        _profileMenu.AddButton("earringsSlot", button26);
        _profileMenu.AddButton("necklaceSlot", button27);
        _profileMenu.AddButton("shieldSlot", button28);
        _profileMenu.AddButton("gauntletLSlot", button29);
        _profileMenu.AddButton("ringLSlot", button30);
        _profileMenu.AddButton("beltSlot", button31);
        _profileMenu.AddButton("greavesSlot", button32);
        _profileMenu.AddButton("bootsSlot", button33);
        _profileMenu.AddButton("weaponSlot", button34);
        _profileMenu.AddButton("helmetSlot", button35);
        _profileMenu.AddButton("armorSlot", button36);
        _profileMenu.AddButton("gauntletRSlot", button37);
        _profileMenu.AddButton("ringRSlot", button38);
        _profileMenu.AddButton("accessorySlot", button39);
        _profileMenu.AddLabel("profileNameLabel", label38);
        _profileMenu.AddLabel("profileLevelLabel", label39);
        _profileMenu.AddLabel("profileGuildLabel", label40);
        _profileMenu.AddLabel("profileRankLabel", label41);
        _profileMenu.AddLabel("profileTitleLabel", label42);
        _profileMenu.AddButton("closeProfileMenuBtn", button25);
        _profileMenu.AddButton("groupProfileMenuBtn", groupProfileMenuBtn);
        _profileMenu.AddButton("legendProfileMenuBtn", legendProfileMenuBtn);
        vector = new Vector(100.0, 4.0, 0.0);
        // === Own legend window ===
        _legendMenu = new ButtonMenu(_input, _font, vector.X, vector.Y, 451.0, 418.0, moveable: true);
        _legendMenu._background.Texture = _textureManager.Get("legend_F0_C0");
        _legendMenu._background.SetPosition(vector);
        Text label43 = DrawLabel("", Engine.Color.Gray2, 178.0, 30.0, 115, "center", 0, shade: false, vector.X, vector.Y);
        Engine.Button button40 = new Engine.Button(_textureManager, 365.0, 377.0, 70.0, 22.0, "butt001_F33", "butt001_F34", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button40._onPressEvent = delegate
        {
            ResetLegendUI();
        };
        Engine.Button button41 = new Engine.Button(_textureManager, 421.0, 60.0, 13.0, 12.0, "scroll_F0", "scroll_F1", "", "scroll_F1", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button41.Enabled = false;
        Engine.Button button42 = new Engine.Button(_textureManager, 421.0, 348.0, 13.0, 12.0, "scroll_F2", "scroll_F3", "", "scroll_F3", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button42.Enabled = false;
        Engine.Button button43 = new Engine.Button(_textureManager, 421.0, 72.0, 13.0, 12.0, "scroll_F4", "scroll_F4", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button43.Hidden = true;
        Engine.Button button44 = new Engine.Button(_textureManager, 421.0, 72.0, 13.0, 276.0, "scroll_F5", "scroll_F5", "", "", null, null, 23, multHorzontal: false, vector.X, vector.Y);
        button41._onHeldEvent = delegate
        {
            if (_legendIndex > 0)
            {
                int count24 = _player._legendMarks.Count;
                _legendIndex--;
                RepositionScroller(_legendIndex, count24 - 14, 264, _legendMenu, _legendMenu._buttons["legendScrollerBtn"], 421, 72);
            }
        };
        button41._onPressEvent = delegate
        {
            if (_legendIndex > 0)
            {
                int count23 = _player._legendMarks.Count;
                _legendIndex--;
                RepositionScroller(_legendIndex, count23 - 14, 264, _legendMenu, _legendMenu._buttons["legendScrollerBtn"], 421, 72);
            }
        };
        button42._onHeldEvent = delegate
        {
            if (_player._legendMarks.Count - 14 > 0 && _legendIndex < _player._legendMarks.Count - 14)
            {
                int count22 = _player._legendMarks.Count;
                _legendIndex++;
                RepositionScroller(_legendIndex, count22 - 14, 264, _legendMenu, _legendMenu._buttons["legendScrollerBtn"], 421, 72);
            }
        };
        button42._onPressEvent = delegate
        {
            if (_player._legendMarks.Count - 14 > 0 && _legendIndex < _player._legendMarks.Count - 14)
            {
                int count21 = _player._legendMarks.Count;
                _legendIndex++;
                RepositionScroller(_legendIndex, count21 - 14, 264, _legendMenu, _legendMenu._buttons["legendScrollerBtn"], 421, 72);
            }
        };
        button43._onHeldEvent = delegate
        {
            int count20 = _player._legendMarks.Count;
            double num20 = ((double)_input.Mouse.Position.Y - _legendMenu._position.Y - 72.0 - 6.0) / 270.0;
            int num21 = (int)Math.Round(num20 * (double)(count20 - 14));
            _legendIndex = ((num21 >= 0) ? ((num21 > count20 - 14) ? (count20 - 14) : num21) : 0);
            RepositionScroller(_legendIndex, count20 - 14, 264, _legendMenu, _legendMenu._buttons["legendScrollerBtn"], 421, 72);
        };
        button44._onPressEvent = delegate
        {
            int count19 = _player._legendMarks.Count;
            double num18 = ((double)_input.Mouse.Position.Y - _legendMenu._position.Y - 72.0 - 6.0) / 270.0;
            int num19 = (int)Math.Round(num18 * (double)(count19 - 14));
            _legendIndex = ((num19 >= 0) ? ((num19 > count19 - 14) ? (count19 - 14) : num19) : 0);
            RepositionScroller(_legendIndex, count19 - 14, 264, _legendMenu, _legendMenu._buttons["legendScrollerBtn"], 421, 72);
        };
        _legendMenu.AddButton("legendScrollBackBtn", button44);
        _legendMenu.AddButton("legendScrollerBtn", button43);
        _legendMenu.AddButton("legendScrollUpBtn", button41);
        _legendMenu.AddButton("legendScrollDownBtn", button42);
        _legendMenu.AddLabel("legendNameLabel", label43);
        _legendMenu.AddButton("closeLegendMenuBtn", button40);
        // === Main menu bar ===
        _menu = new ButtonMenu(_input, _font);
        _menu._background.Texture = _textureManager.Get("backgrnd");
        Engine.Button optionsBtn = new Engine.Button(_textureManager, 619.0, 233.0, 22.0, 23.0, "", "gbicon02_F0");
        Engine.Button mail1Btn = new Engine.Button(_textureManager, 619.0, 256.0, 22.0, 23.0, "", "gbicon02_F1");
        Engine.Button usersBtn = new Engine.Button(_textureManager, 619.0, 279.0, 22.0, 23.0, "", "gbicon02_F2");
        mail1Btn._onPressEvent = delegate
        {
            CloseMenus();
            _viewingPersonalBoard = true;
            ToggleSelectedButtons(_menu, mail1Btn);
            RequestBoard(0, 1, 0, 0);
        };
        usersBtn._onPressEvent = delegate
        {
            _userList.Clear();
            if (GameWindow.ConnectedToServer)
            {
                RequestUsersPacket requestUsersPacket = new RequestUsersPacket();
                GameWindow.ClientSocket.Send(requestUsersPacket.Data);
            }
            CloseMenus();
            _viewingUsers = true;
            _usersMenu._buttons["countryBtn"].Selected = true;
            ToggleSelectedButtons(_menu, usersBtn);
        };
        optionsBtn._onPressEvent = delegate
        {
            CloseMenus();
            _viewingOptions = true;
            ToggleSelectedButtons(_menu, optionsBtn);
        };
        _menu.AddButton("optionsBtn", optionsBtn);
        _menu.AddButton("mail1Btn", mail1Btn);
        _menu.AddButton("usersBtn", usersBtn);
        // === Options menu ===
        _optionsMenu = new ButtonMenu(_input, _font);
        _optionsMenu._background.Texture = _textureManager.Get("option01_F0_C0");
        _optionsMenu._background.SetPosition(444.0, 4.0);
        Text label44 = DrawLabel("", Engine.Color.White, 539.0, 75.0, 48, "center");
        Engine.Button button45 = new Engine.Button(_textureManager, 457.0, 73.0, 68.0, 20.0, "", "option02_F0");
        Engine.Button soundBtn = new Engine.Button(_textureManager, 457.0, 99.0, 68.0, 20.0, "", "option02_F1");
        Engine.Button soundNodeBtn = new Engine.Button(_textureManager, 534.0, 104.0, 9.0, 9.0, "option04_F0");
        soundNodeBtn._windowOffset = new Vector(534.0, 104.0, 0.0);
        Engine.Button musicBtn = new Engine.Button(_textureManager, 457.0, 125.0, 68.0, 20.0, "", "option02_F2");
        Engine.Button musicNodeBtn = new Engine.Button(_textureManager, 534.0, 130.0, 9.0, 9.0, "option04_F0");
        musicNodeBtn._windowOffset = new Vector(534.0, 130.0, 0.0);
        Engine.Button friendBtn = new Engine.Button(_textureManager, 457.0, 151.0, 68.0, 20.0, "", "option02_F3");
        Engine.Button macroBtn = new Engine.Button(_textureManager, 457.0, 177.0, 68.0, 20.0, "", "option02_F4");
        Engine.Button settingBtn = new Engine.Button(_textureManager, 457.0, 203.0, 68.0, 20.0, "", "option02_F5");
        Engine.Button button46 = new Engine.Button(_textureManager, 489.0, 236.0, 96.0, 20.0, "", "option03_F0");
        Engine.Button button47 = new Engine.Button(_textureManager, 531.0, 274.0, 70.0, 22.0, "butt001_F33", "butt001_F34");
        RepositionHorizontalScroller(_soundNodeIndex, 100, 49, _optionsMenu, soundNodeBtn, 534, 104);
        RepositionHorizontalScroller(_musicNodeIndex, 100, 49, _optionsMenu, musicNodeBtn, 534, 130);
        button45._onPressEvent = delegate
        {
            if (_viewKeyboards)
            {
                _viewKeyboards = false;
            }
            else
            {
                _viewKeyboards = true;
            }
        };
        soundBtn._onPressEvent = delegate
        {
            ToggleSelectedButtons(_optionsMenu, soundBtn);
        };
        soundNodeBtn._onHeldEvent = delegate
        {
            double num16 = ((double)(_input.Mouse.Position.X - 534f) - 4.5) / 50.5;
            int num17 = (int)Math.Round(num16 * 100.0);
            _soundNodeIndex = ((num17 >= 0) ? ((num17 > 100) ? 100 : num17) : 0);
            RepositionHorizontalScroller(_soundNodeIndex, 100, 49, _optionsMenu, soundNodeBtn, 534, 104);
            SoundVolume();
            SaveClientSettings();
        };
        musicBtn._onPressEvent = delegate
        {
            ToggleSelectedButtons(_optionsMenu, musicBtn);
        };
        musicNodeBtn._onHeldEvent = delegate
        {
            double num14 = ((double)(_input.Mouse.Position.X - 534f) - 4.5) / 50.5;
            int num15 = (int)Math.Round(num14 * 100.0);
            _musicNodeIndex = ((num15 >= 0) ? ((num15 > 100) ? 100 : num15) : 0);
            RepositionHorizontalScroller(_musicNodeIndex, 100, 49, _optionsMenu, musicNodeBtn, 534, 130);
            MusicVolume();
            SaveClientSettings();
        };
        friendBtn._onPressEvent = delegate
        {
            _viewingFriends = true;
            ToggleSelectedButtons(_optionsMenu, friendBtn);
        };
        macroBtn._onPressEvent = delegate
        {
            _viewingMacros = true;
            ToggleSelectedButtons(_optionsMenu, macroBtn);
        };
        settingBtn._onPressEvent = delegate
        {
            _viewingSettings = true;
            ToggleSelectedButtons(_optionsMenu, settingBtn);
        };
        button47._onPressEvent = delegate
        {
            CloseMenus();
        };
        button46._onPressEvent = delegate
        {
            LogOut();
        };
        _optionsMenu.AddLabel("keyboardLabel", label44);
        _optionsMenu.AddButton("closeOptionsMenuBtn", button47);
        _optionsMenu.AddButton("exitGameBtn", button46);
        _optionsMenu.AddButton("keyboardBtn", button45);
        _optionsMenu.AddButton("soundBtn", soundBtn);
        _optionsMenu.AddButton("soundNodeBtn", soundNodeBtn);
        _optionsMenu.AddButton("musicBtn", musicBtn);
        _optionsMenu.AddButton("musicNodeBtn", musicNodeBtn);
        _optionsMenu.AddButton("friendBtn", friendBtn);
        _optionsMenu.AddButton("macroBtn", macroBtn);
        _optionsMenu.AddButton("settingBtn", settingBtn);
        // === Friends menu ===
        _friendsMenu = new ButtonMenu(_input, _font);
        _friendsMenu._background.Texture = _textureManager.Get("friend_F0_C0");
        _friendsMenu._background.SetPosition(0.0, 4.0);
        TextField friend1Box = new TextField(new Rectangle(67, 61, 150, 13), _font, _friendList[0]);
        friend1Box._onLoseFocus = delegate
        {
            if (_friendList[0] != friend1Box.Text)
            {
                _friendList[0] = friend1Box.Text;
                SaveClientSettings();
            }
        };
        TextField friend2Box = new TextField(new Rectangle(67, 80, 150, 13), _font, _friendList[1]);
        friend2Box._onLoseFocus = delegate
        {
            if (_friendList[1] != friend2Box.Text)
            {
                _friendList[1] = friend2Box.Text;
                SaveClientSettings();
            }
        };
        TextField friend3Box = new TextField(new Rectangle(67, 99, 150, 13), _font, _friendList[2]);
        friend3Box._onLoseFocus = delegate
        {
            if (_friendList[2] != friend3Box.Text)
            {
                _friendList[2] = friend3Box.Text;
                SaveClientSettings();
            }
        };
        TextField friend4Box = new TextField(new Rectangle(67, 118, 150, 13), _font, _friendList[3]);
        friend4Box._onLoseFocus = delegate
        {
            if (_friendList[3] != friend4Box.Text)
            {
                _friendList[3] = friend4Box.Text;
                SaveClientSettings();
            }
        };
        TextField friend5Box = new TextField(new Rectangle(67, 137, 150, 13), _font, _friendList[4]);
        friend5Box._onLoseFocus = delegate
        {
            if (_friendList[4] != friend5Box.Text)
            {
                _friendList[4] = friend5Box.Text;
                SaveClientSettings();
            }
        };
        TextField friend6Box = new TextField(new Rectangle(67, 156, 150, 13), _font, _friendList[5]);
        friend6Box._onLoseFocus = delegate
        {
            if (_friendList[5] != friend6Box.Text)
            {
                _friendList[5] = friend6Box.Text;
                SaveClientSettings();
            }
        };
        TextField friend7Box = new TextField(new Rectangle(67, 175, 150, 13), _font, _friendList[6]);
        friend7Box._onLoseFocus = delegate
        {
            if (_friendList[6] != friend7Box.Text)
            {
                _friendList[6] = friend7Box.Text;
                SaveClientSettings();
            }
        };
        TextField friend8Box = new TextField(new Rectangle(67, 194, 150, 13), _font, _friendList[7]);
        friend8Box._onLoseFocus = delegate
        {
            if (_friendList[7] != friend8Box.Text)
            {
                _friendList[7] = friend8Box.Text;
                SaveClientSettings();
            }
        };
        TextField friend9Box = new TextField(new Rectangle(67, 213, 150, 13), _font, _friendList[8]);
        friend9Box._onLoseFocus = delegate
        {
            if (_friendList[8] != friend9Box.Text)
            {
                _friendList[8] = friend9Box.Text;
                SaveClientSettings();
            }
        };
        TextField friend10Box = new TextField(new Rectangle(67, 232, 150, 13), _font, _friendList[9]);
        friend10Box._onLoseFocus = delegate
        {
            if (_friendList[9] != friend10Box.Text)
            {
                _friendList[9] = friend10Box.Text;
                SaveClientSettings();
            }
        };
        TextField friend11Box = new TextField(new Rectangle(285, 61, 150, 13), _font, _friendList[10]);
        friend11Box._onLoseFocus = delegate
        {
            if (_friendList[10] != friend11Box.Text)
            {
                _friendList[10] = friend11Box.Text;
                SaveClientSettings();
            }
        };
        TextField friend12Box = new TextField(new Rectangle(285, 80, 150, 13), _font, _friendList[11]);
        friend12Box._onLoseFocus = delegate
        {
            if (_friendList[11] != friend12Box.Text)
            {
                _friendList[11] = friend12Box.Text;
                SaveClientSettings();
            }
        };
        TextField friend13Box = new TextField(new Rectangle(285, 99, 150, 13), _font, _friendList[12]);
        friend13Box._onLoseFocus = delegate
        {
            if (_friendList[12] != friend13Box.Text)
            {
                _friendList[12] = friend13Box.Text;
                SaveClientSettings();
            }
        };
        TextField friend14Box = new TextField(new Rectangle(285, 118, 150, 13), _font, _friendList[13]);
        friend14Box._onLoseFocus = delegate
        {
            if (_friendList[13] != friend14Box.Text)
            {
                _friendList[13] = friend14Box.Text;
                SaveClientSettings();
            }
        };
        TextField friend15Box = new TextField(new Rectangle(285, 137, 150, 13), _font, _friendList[14]);
        friend15Box._onLoseFocus = delegate
        {
            if (_friendList[14] != friend15Box.Text)
            {
                _friendList[14] = friend15Box.Text;
                SaveClientSettings();
            }
        };
        TextField friend16Box = new TextField(new Rectangle(285, 156, 150, 13), _font, _friendList[15]);
        friend16Box._onLoseFocus = delegate
        {
            if (_friendList[15] != friend16Box.Text)
            {
                _friendList[15] = friend16Box.Text;
                SaveClientSettings();
            }
        };
        TextField friend17Box = new TextField(new Rectangle(285, 175, 150, 13), _font, _friendList[16]);
        friend17Box._onLoseFocus = delegate
        {
            if (_friendList[16] != friend17Box.Text)
            {
                _friendList[16] = friend17Box.Text;
                SaveClientSettings();
            }
        };
        TextField friend18Box = new TextField(new Rectangle(285, 194, 150, 13), _font, _friendList[17]);
        friend18Box._onLoseFocus = delegate
        {
            if (_friendList[17] != friend18Box.Text)
            {
                _friendList[17] = friend18Box.Text;
                SaveClientSettings();
            }
        };
        TextField friend19Box = new TextField(new Rectangle(285, 213, 150, 13), _font, _friendList[18]);
        friend19Box._onLoseFocus = delegate
        {
            if (_friendList[18] != friend19Box.Text)
            {
                _friendList[18] = friend19Box.Text;
                SaveClientSettings();
            }
        };
        TextField friend20Box = new TextField(new Rectangle(285, 232, 150, 13), _font, _friendList[19]);
        friend20Box._onLoseFocus = delegate
        {
            if (_friendList[19] != friend20Box.Text)
            {
                _friendList[19] = friend20Box.Text;
                SaveClientSettings();
            }
        };
        Engine.Button button48 = new Engine.Button(_textureManager, 370.0, 274.0, 70.0, 22.0, "butt001_F33", "butt001_F34");
        button48._onPressEvent = delegate
        {
            _viewingFriends = false;
            ToggleSelectedButtons(_optionsMenu);
        };
        _friendsMenu.AddTextField("friend1Box", friend1Box);
        _friendsMenu.AddTextField("friend2Box", friend2Box);
        _friendsMenu.AddTextField("friend3Box", friend3Box);
        _friendsMenu.AddTextField("friend4Box", friend4Box);
        _friendsMenu.AddTextField("friend5Box", friend5Box);
        _friendsMenu.AddTextField("friend6Box", friend6Box);
        _friendsMenu.AddTextField("friend7Box", friend7Box);
        _friendsMenu.AddTextField("friend8Box", friend8Box);
        _friendsMenu.AddTextField("friend9Box", friend9Box);
        _friendsMenu.AddTextField("friend10Box", friend10Box);
        _friendsMenu.AddTextField("friend11Box", friend11Box);
        _friendsMenu.AddTextField("friend12Box", friend12Box);
        _friendsMenu.AddTextField("friend13Box", friend13Box);
        _friendsMenu.AddTextField("friend14Box", friend14Box);
        _friendsMenu.AddTextField("friend15Box", friend15Box);
        _friendsMenu.AddTextField("friend16Box", friend16Box);
        _friendsMenu.AddTextField("friend17Box", friend17Box);
        _friendsMenu.AddTextField("friend18Box", friend18Box);
        _friendsMenu.AddTextField("friend19Box", friend19Box);
        _friendsMenu.AddTextField("friend20Box", friend20Box);
        _friendsMenu.AddButton("closeFriendsBtn", button48);
        // === Macro menu ===
        _macroMenu = new ButtonMenu(_input, _font);
        _macroMenu._background.Texture = _textureManager.Get("macro01_F0_C0");
        _macroMenu._background.SetPosition(0.0, 4.0);
        TextField textField3 = new TextField(new Rectangle(67, 61, 363, 13), _font, "You have my gratitude");
        TextField textField4 = new TextField(new Rectangle(67, 80, 363, 13), _font, "Please teach me things");
        TextField textField5 = new TextField(new Rectangle(67, 99, 363, 13), _font, "Adventure with me?");
        TextField textField6 = new TextField(new Rectangle(67, 118, 363, 13), _font, "Group me, please");
        TextField textField7 = new TextField(new Rectangle(67, 137, 363, 13), _font, "I'll group you");
        TextField textField8 = new TextField(new Rectangle(67, 156, 363, 13), _font, "Help!");
        TextField textField9 = new TextField(new Rectangle(67, 175, 363, 13), _font, "Be careful");
        TextField textField10 = new TextField(new Rectangle(67, 194, 363, 13), _font, "Fair day");
        TextField textField11 = new TextField(new Rectangle(67, 213, 363, 13), _font, "Farewell, Aisling");
        TextField textField12 = new TextField(new Rectangle(67, 232, 363, 13), _font, "Greetings, Aisling");
        Engine.Button button49 = new Engine.Button(_textureManager, 370.0, 274.0, 70.0, 22.0, "butt001_F33", "butt001_F34");
        button49._onPressEvent = delegate
        {
            _viewingMacros = false;
            ToggleSelectedButtons(_optionsMenu);
        };
        _macroMenu.AddTextField("phrase1Box", textField3);
        _macroMenu.AddTextField("phrase2Box", textField4);
        _macroMenu.AddTextField("phrase3Box", textField5);
        _macroMenu.AddTextField("phrase4Box", textField6);
        _macroMenu.AddTextField("phrase5Box", textField7);
        _macroMenu.AddTextField("phrase6Box", textField8);
        _macroMenu.AddTextField("phrase7Box", textField9);
        _macroMenu.AddTextField("phrase8Box", textField10);
        _macroMenu.AddTextField("phrase9Box", textField11);
        _macroMenu.AddTextField("phrase10Box", textField12);
        _macroMenu.AddButton("closeMacroBtn", button49);
        // === Settings (F4) menu ===
        _settingsMenu = new ButtonMenu(_input, _font);
        _settingsMenu._background.Texture = _textureManager.Get("gset01_F0_C0");
        _settingsMenu._background.SetPosition(0.0, 4.0);
        Engine.Button button50 = new Engine.Button(_textureManager, 370.0, 274.0, 70.0, 22.0, "butt001_F33", "butt001_F34");
        string text = "ON";
        text = (_listenToWhispers ? "ON" : "OFF");
        Text settings1Lab = DrawLabel("Listen to whisper : " + text, Engine.Color.White, 68.0, 62.0, 363, "left");
        settings1Lab._onPressEvent = delegate
        {
            string text11 = settings1Lab._text.Substring(settings1Lab._text.IndexOf(" : ") + 3);
            if (text11 == "ON")
            {
                _listenToWhispers = false;
                SaveClientSettings();
                settings1Lab.ChangeText(settings1Lab._text.Remove(settings1Lab._text.IndexOf(" : ") + 3) + "OFF");
            }
            else
            {
                _listenToWhispers = true;
                SaveClientSettings();
                settings1Lab.ChangeText(settings1Lab._text.Remove(settings1Lab._text.IndexOf(" : ") + 3) + "ON");
            }
        };
        text = (_listenToShouts ? "ON" : "OFF");
        Text settings2Lab = DrawLabel("Listen to shout : " + text, Engine.Color.White, 68.0, 81.0, 363, "left");
        settings2Lab._onPressEvent = delegate
        {
            string text10 = settings2Lab._text.Substring(settings2Lab._text.IndexOf(" : ") + 3);
            if (text10 == "ON")
            {
                _listenToShouts = false;
                SaveClientSettings();
                settings2Lab.ChangeText(settings2Lab._text.Remove(settings2Lab._text.IndexOf(" : ") + 3) + "OFF");
            }
            else
            {
                _listenToShouts = true;
                SaveClientSettings();
                settings2Lab.ChangeText(settings2Lab._text.Remove(settings2Lab._text.IndexOf(" : ") + 3) + "ON");
            }
        };
        text = (_listenToGuild ? "ON" : "OFF");
        Text settings3Lab = DrawLabel("Listen to guild : " + text, Engine.Color.White, 68.0, 100.0, 363, "left");
        settings3Lab._onPressEvent = delegate
        {
            string text9 = settings3Lab._text.Substring(settings3Lab._text.IndexOf(" : ") + 3);
            if (text9 == "ON")
            {
                _listenToGuild = false;
                SaveClientSettings();
                settings3Lab.ChangeText(settings3Lab._text.Remove(settings3Lab._text.IndexOf(" : ") + 3) + "OFF");
            }
            else
            {
                _listenToGuild = true;
                SaveClientSettings();
                settings3Lab.ChangeText(settings3Lab._text.Remove(settings3Lab._text.IndexOf(" : ") + 3) + "ON");
            }
        };
        text = (_listenToNPCs ? "ON" : "OFF");
        Text settings4Lab = DrawLabel("Record NPC Chat : " + text, Engine.Color.White, 68.0, 119.0, 363, "left");
        settings4Lab._onPressEvent = delegate
        {
            string text8 = settings4Lab._text.Substring(settings4Lab._text.IndexOf(" : ") + 3);
            if (text8 == "ON")
            {
                _listenToNPCs = false;
                SaveClientSettings();
                settings4Lab.ChangeText(settings4Lab._text.Remove(settings4Lab._text.IndexOf(" : ") + 3) + "OFF");
            }
            else
            {
                _listenToNPCs = true;
                SaveClientSettings();
                settings4Lab.ChangeText(settings4Lab._text.Remove(settings4Lab._text.IndexOf(" : ") + 3) + "ON");
            }
        };
        text = (_allowGrouping ? "ON" : "OFF");
        Text settings5Lab = DrawLabel("Join a group : " + text, Engine.Color.White, 68.0, 138.0, 363, "left");
        settings5Lab._onPressEvent = delegate
        {
            string text7 = settings5Lab._text.Substring(settings5Lab._text.IndexOf(" : ") + 3);
            if (text7 == "ON")
            {
                _profileMenu._buttons["groupProfileMenuBtn"]._baseImage = _textureManager.Get("equip05_F0_C0");
                _profileMenu._buttons["groupProfileMenuBtn"]._clickedImage = _textureManager.Get("equip05_F1_C0");
                _profileMenu._buttons["groupProfileMenuBtn"].Sprite.Texture = _profileMenu._buttons["groupProfileMenuBtn"]._baseImage;
                _allowGrouping = false;
                SendProfileData();
                SaveClientSettings();
                settings5Lab.ChangeText(settings5Lab._text.Remove(settings5Lab._text.IndexOf(" : ") + 3) + "OFF");
            }
            else
            {
                _profileMenu._buttons["groupProfileMenuBtn"]._baseImage = _textureManager.Get("equip05_F2_C0");
                _profileMenu._buttons["groupProfileMenuBtn"]._clickedImage = _textureManager.Get("equip05_F3_C0");
                _profileMenu._buttons["groupProfileMenuBtn"].Sprite.Texture = _profileMenu._buttons["groupProfileMenuBtn"]._baseImage;
                _allowGrouping = true;
                SendProfileData();
                SaveClientSettings();
                settings5Lab.ChangeText(settings5Lab._text.Remove(settings5Lab._text.IndexOf(" : ") + 3) + "ON");
            }
        };
        text = (_enableSpells ? "ON" : "OFF");
        Text settings6Lab = DrawLabel("Spell animations : " + text, Engine.Color.White, 68.0, 157.0, 363, "left");
        settings6Lab._onPressEvent = delegate
        {
            string text6 = settings6Lab._text.Substring(settings6Lab._text.IndexOf(" : ") + 3);
            if (text6 == "ON")
            {
                _enableSpells = false;
                SaveClientSettings();
                settings6Lab.ChangeText(settings6Lab._text.Remove(settings6Lab._text.IndexOf(" : ") + 3) + "OFF");
            }
            else
            {
                _enableSpells = true;
                SaveClientSettings();
                settings6Lab.ChangeText(settings6Lab._text.Remove(settings6Lab._text.IndexOf(" : ") + 3) + "ON");
            }
        };
        text = (_enableWeather ? "ON" : "OFF");
        Text settings7Lab = DrawLabel("Weather effects : " + text, Engine.Color.White, 68.0, 176.0, 363, "left");
        settings7Lab._onPressEvent = delegate
        {
            string text5 = settings7Lab._text.Substring(settings7Lab._text.IndexOf(" : ") + 3);
            if (text5 == "ON")
            {
                _enableWeather = false;
                SaveClientSettings();
                settings7Lab.ChangeText(settings7Lab._text.Remove(settings7Lab._text.IndexOf(" : ") + 3) + "OFF");
            }
            else
            {
                _enableWeather = true;
                SaveClientSettings();
                settings7Lab.ChangeText(settings7Lab._text.Remove(settings7Lab._text.IndexOf(" : ") + 3) + "ON");
            }
        };
        text = (_enableExchange ? "ON" : "OFF");
        Text settings8Lab = DrawLabel("Exchange : " + text, Engine.Color.White, 68.0, 195.0, 363, "left");
        settings8Lab._onPressEvent = delegate
        {
            string text4 = settings8Lab._text.Substring(settings8Lab._text.IndexOf(" : ") + 3);
            if (text4 == "ON")
            {
                _enableExchange = false;
                SaveClientSettings();
                settings8Lab.ChangeText(settings8Lab._text.Remove(settings8Lab._text.IndexOf(" : ") + 3) + "OFF");
            }
            else
            {
                _enableExchange = true;
                SaveClientSettings();
                settings8Lab.ChangeText(settings8Lab._text.Remove(settings8Lab._text.IndexOf(" : ") + 3) + "ON");
            }
        };
        text = (_useGroupWindow ? "ON" : "OFF");
        Text settings9Lab = DrawLabel("Use group window : " + text, Engine.Color.White, 68.0, 214.0, 363, "left");
        settings9Lab._onPressEvent = delegate
        {
            string text3 = settings9Lab._text.Substring(settings9Lab._text.IndexOf(" : ") + 3);
            if (text3 == "ON")
            {
                settings9Lab.ChangeText(settings9Lab._text.Remove(settings9Lab._text.IndexOf(" : ") + 3) + "OFF");
                _useGroupWindow = false;
                SaveClientSettings();
            }
            else
            {
                settings9Lab.ChangeText(settings9Lab._text.Remove(settings9Lab._text.IndexOf(" : ") + 3) + "ON");
                _useGroupWindow = true;
                SaveClientSettings();
            }
        };
        Text settings10Lab = DrawLabel(" : ON", Engine.Color.White, 68.0, 233.0, 363, "left");
        settings10Lab._onPressEvent = delegate
        {
            string text2 = settings10Lab._text.Substring(settings10Lab._text.IndexOf(" : ") + 3);
            if (text2 == "ON")
            {
                settings10Lab.ChangeText(settings10Lab._text.Remove(settings10Lab._text.IndexOf(" : ") + 3) + "OFF");
            }
            else
            {
                settings10Lab.ChangeText(settings10Lab._text.Remove(settings10Lab._text.IndexOf(" : ") + 3) + "ON");
            }
        };
        button50._onPressEvent = delegate
        {
            _viewingSettings = false;
            ToggleSelectedButtons(_optionsMenu);
        };
        _settingsMenu.AddLabel("settings1Lab", settings1Lab);
        _settingsMenu.AddLabel("settings2Lab", settings2Lab);
        _settingsMenu.AddLabel("settings3Lab", settings3Lab);
        _settingsMenu.AddLabel("settings4Lab", settings4Lab);
        _settingsMenu.AddLabel("settings5Lab", settings5Lab);
        _settingsMenu.AddLabel("settings6Lab", settings6Lab);
        _settingsMenu.AddLabel("settings7Lab", settings7Lab);
        _settingsMenu.AddLabel("settings8Lab", settings8Lab);
        _settingsMenu.AddLabel("settings9Lab", settings9Lab);
        _settingsMenu.AddLabel("settings10Lab", settings10Lab);
        _settingsMenu.AddButton("closeSettingsBtn", button50);
        // === Board / mail menu ===
        _boardMenu = new ButtonMenu(_input, _font);
        _boardMenu._background.Texture = _textureManager.Get("dlgbbs01_F0_C0");
        _boardMenu._background.SetPosition(0.0, 4.0);
        Engine.Button viewBoardBtn = new Engine.Button(_textureManager, 527.0, 55.0, 70.0, 22.0, "butt001_F0", "butt001_F1", "", "butt001_F2");
        viewBoardBtn.Enabled = false;
        viewBoardBtn._onPressEvent = delegate
        {
            if (personalBoards.Count > 0 && _boardMenu._selectedID != 0 && personalBoards.ContainsKey(_boardMenu._selectedID))
            {
                RequestBoard(_boardMenu._selectedID, 2, 0, 0);
                _boardMenu._selectedID = 0;
                ResetBoardLabelColors(_boardMenu);
            }
        };
        _boardMenu.AddButton("viewBtn", viewBoardBtn);
        double num = 25.0;
        for (int i = 1; i <= 22; i++)
        {
            Text boardLabel3 = DrawLabel("", Engine.Color.Gray2, 28.0, num, 450, "left");
            boardLabel3._onPressEvent = delegate
            {
                ResetBoardLabelColors(_boardMenu);
                if (boardLabel3.BoardID != 0)
                {
                    _boardMenu._selectedID = boardLabel3.BoardID;
                    boardLabel3.SetColor(Engine.Color.LightBlue);
                    viewBoardBtn.Enabled = true;
                }
            };
            boardLabel3._onDoublePressEvent = delegate
            {
                if (boardLabel3.BoardID != 0)
                {
                    viewBoardBtn.OnPress();
                }
            };
            _boardMenu.AddLabel("boardLabel" + i, boardLabel3);
            num += 12.0;
        }
        Engine.Button button51 = new Engine.Button(_textureManager, 531.0, 274.0, 70.0, 22.0, "butt001_F33", "butt001_F34");
        button51._onPressEvent = delegate
        {
            ResetBoardLabelColors(_boardMenu);
            ToggleSelectedButtons(_menu);
            _viewingPersonalBoard = false;
            _boardMenu._selectedText = "";
            viewBoardBtn.Enabled = false;
        };
        _boardMenu.AddButton("closeBoardBtn", button51);
        // === Mail list ===
        _mailListMenu = new ButtonMenu(_input, _font);
        _mailListMenu._background.Texture = _textureManager.Get("dlgbbs01_F0_C0");
        _mailListMenu._background.SetPosition(0.0, 4.0);
        Engine.Button viewMailBtn = new Engine.Button(_textureManager, 527.0, 81.0, 70.0, 22.0, "butt001_F0", "butt001_F1", "", "butt001_F2");
        viewMailBtn._onPressEvent = delegate
        {
            _viewingPost = true;
        };
        Engine.Button button52 = new Engine.Button(_textureManager, 527.0, 133.0, 70.0, 22.0, "butt001_F6", "butt001_F7");
        button52._onPressEvent = delegate
        {
            _composingPost = true;
        };
        Engine.Button button53 = new Engine.Button(_textureManager, 527.0, 159.0, 70.0, 22.0, "butt001_F27", "butt001_F28", "", "butt001_F29");
        button53.Enabled = false;
        Engine.Button button54 = new Engine.Button(_textureManager, 527.0, 185.0, 70.0, 22.0, "butt001_F18", "butt001_F19", "", "butt001_F20");
        button54.Enabled = false;
        Engine.Button button55 = new Engine.Button(_textureManager, 527.0, 237.0, 70.0, 22.0, "butt001_F12", "butt001_F13");
        button55._onPressEvent = delegate
        {
            _viewingMailList = false;
        };
        Engine.Button button56 = new Engine.Button(_textureManager, 531.0, 274.0, 70.0, 22.0, "butt001_F33", "butt001_F34");
        button56._onPressEvent = delegate
        {
            _viewingMailList = false;
            _mailListMenu._buttons["closeMailBtn"].OnPress();
        };
        num = 25.0;
        for (int j = 1; j <= 22; j++)
        {
            Text boardLabel2 = DrawLabel("", Engine.Color.Gray2, 28.0, num, 450, "left");
            boardLabel2._onPressEvent = delegate
            {
                ResetBoardLabelColors(_mailListMenu);
                if (boardLabel2.BoardID != 0)
                {
                    _boardMenu._selectedID = boardLabel2.BoardID;
                    boardLabel2.SetColor(Engine.Color.LightBlue);
                    viewMailBtn.Enabled = true;
                }
            };
            boardLabel2._onDoublePressEvent = delegate
            {
                if (boardLabel2.BoardID != 0)
                {
                    viewMailBtn.OnPress();
                }
            };
            _mailListMenu.AddLabel("boardLabel" + j, boardLabel2);
            num += 12.0;
        }
        _mailListMenu.AddButton("viewBtn", viewMailBtn);
        _mailListMenu.AddButton("newMailBtn", button52);
        _mailListMenu.AddButton("replyMailBtn", button53);
        _mailListMenu.AddButton("deleteMailBtn", button54);
        _mailListMenu.AddButton("upMailBtn", button55);
        _mailListMenu.AddButton("closeMailBtn", button56);
        // === Board list ===
        _boardListMenu = new ButtonMenu(_input, _font);
        _boardListMenu._background.Texture = _textureManager.Get("dlgbbs01_F0_C0");
        _boardListMenu._background.SetPosition(0.0, 4.0);
        Engine.Button viewBoardListBtn = new Engine.Button(_textureManager, 527.0, 55.0, 70.0, 22.0, "butt001_F0", "butt001_F1", "", "butt001_F2");
        viewBoardListBtn.Enabled = false;
        viewBoardListBtn._onPressEvent = delegate
        {
            if (boardPosts.Count > 0 && _boardListMenu._selectedID != 0 && boardPosts.ContainsKey(_boardListMenu._selectedID))
            {
                RequestBoard(_boardListMenu._boardID, 3, _boardListMenu._selectedID, 0);
                ResetBoardLabelColors(_boardListMenu);
            }
        };
        Engine.Button button57 = new Engine.Button(_textureManager, 527.0, 133.0, 70.0, 22.0, "butt001_F6", "butt001_F7");
        button57._onPressEvent = delegate
        {
            _composePostMenu._boardID = _boardListMenu._boardID;
            _composePostMenu._textFields["composeToName"].Text = _player._name;
            _composePostMenu._textFields["composeToName"]._allowClick = false;
            _composePostMenu._textFields["composeToName"]._textObj.SetColor(Engine.Color.Gray2);
            _composePostMenu._textFields["composeTitle"].SetFocus();
            _composePostMenu._currentTabIndex = 2;
            _composingPost = true;
        };
        Engine.Button deleteBoardListBtn = new Engine.Button(_textureManager, 527.0, 185.0, 70.0, 22.0, "butt001_F18", "butt001_F19", "", "butt001_F20");
        deleteBoardListBtn.Enabled = false;
        deleteBoardListBtn._onPressEvent = delegate
        {
            if (_boardListMenu._selectedID != 0)
            {
                _deletePrompt._labels["delpromptText"].ChangeText("Would you like to delete?");
            }
        };
        _boardListMenu.AddButton("deleteBoardListBtn", deleteBoardListBtn);
        _boardListMenu.AddButton("newBoardListBtn", button57);
        _boardListMenu.AddButton("viewBtn", viewBoardListBtn);
        num = 25.0;
        for (int k = 1; k <= 22; k++)
        {
            Text boardLabel = DrawLabel("", Engine.Color.Gray2, 28.0, num, 450, "left");
            boardLabel._onPressEvent = delegate
            {
                ResetBoardLabelColors(_boardListMenu);
                if (boardLabel.PostNumber != 0)
                {
                    _boardListMenu._boardID = boardLabel.BoardID;
                    _boardListMenu._selectedID = boardLabel.PostNumber;
                    boardLabel.SetColor(Engine.Color.LightBlue);
                    viewBoardListBtn.Enabled = true;
                    deleteBoardListBtn.Enabled = true;
                }
                else
                {
                    _boardListMenu._selectedID = 0;
                }
            };
            boardLabel._onDoublePressEvent = delegate
            {
                if (boardLabel.PostNumber != 0)
                {
                    viewBoardListBtn.OnPress();
                }
            };
            _boardListMenu.AddLabel("boardLabel" + k, boardLabel);
            num += 12.0;
        }
        Engine.Button button58 = new Engine.Button(_textureManager, 527.0, 237.0, 70.0, 22.0, "butt001_F12", "butt001_F13");
        button58._onPressEvent = delegate
        {
            _viewingBoardList = false;
        };
        Engine.Button button59 = new Engine.Button(_textureManager, 531.0, 274.0, 70.0, 22.0, "butt001_F33", "butt001_F34");
        button59._onPressEvent = delegate
        {
            _viewingBoardList = false;
            _boardMenu._buttons["closeBoardBtn"].OnPress();
        };
        _boardListMenu.AddButton("closeBoardListBtn", button59);
        _boardListMenu.AddButton("upBoardListBtn", button58);
        // === Compose-post window ===
        _composePostMenu = new ButtonMenu(_input, _font);
        _composePostMenu._background.Texture = _textureManager.Get("dlgbbs03_F0_C0");
        _composePostMenu._background.SetPosition(0.0, 4.0);
        TextField textField13 = new TextField(new Rectangle(114, 23, 220, 13), _font, "", 12);
        textField13._tabIndex = 1;
        TextField textField14 = new TextField(new Rectangle(114, 47, 390, 13), _font, "", 100);
        textField14._allowApostrophe = true;
        textField14._ignorespecial = false;
        textField14._tabIndex = 2;
        TextField textField15 = new TextField(new Rectangle(25, 81, 450, 210), _font, "", 65535);
        textField15._allownewline = true;
        textField15._allowApostrophe = true;
        textField15._ignorespecial = false;
        textField15._tabIndex = 3;
        _composePostMenu._currentTabIndex = 2;
        _composePostMenu._maxTabIndex = 3;
        Engine.Button button60 = new Engine.Button(_textureManager, 527.0, 81.0, 70.0, 22.0, "butt001_F36", "butt001_F37");
        button60._onPressEvent = delegate
        {
            SendNewPost(_composePostMenu._boardID, _composePostMenu._textFields["composeToName"].Text, _composePostMenu._textFields["composeTitle"].Text, _composePostMenu._textFields["composeBody"].Text);
        };
        Engine.Button button61 = new Engine.Button(_textureManager, 527.0, 107.0, 70.0, 22.0, "butt001_F21", "butt001_F22");
        button61._onPressEvent = delegate
        {
            RequestBoard(_composePostMenu._boardID, 2, 0, 0);
            _composePostMenu._textFields["composeToName"].Text = "";
            _composePostMenu._textFields["composeTitle"].Text = "";
            _composePostMenu._textFields["composeBody"].Text = "";
            _composingPost = false;
        };
        _composePostMenu.AddTextField("composeToName", textField13);
        _composePostMenu.AddTextField("composeTitle", textField14);
        _composePostMenu.AddTextField("composeBody", textField15);
        _composePostMenu.AddButton("sendComposedBtn", button60);
        _composePostMenu.AddButton("cancelComposedBtn", button61);
        // === View-post window ===
        _viewPostMenu = new ButtonMenu(_input, _font);
        _viewPostMenu._background.Texture = _textureManager.Get("dlgbbs02_F0_C0");
        _viewPostMenu._background.SetPosition(0.0, 4.0);
        Text label45 = DrawLabel("", Engine.Color.White, 118.0, 25.0, 220, "left");
        Text label46 = DrawLabel("", Engine.Color.White, 385.0, 25.0, 90, "center");
        Text label47 = DrawLabel("", Engine.Color.White, 118.0, 49.0, 390, "left");
        TextField textField16 = new TextField(new Rectangle(25, 81, 450, 210), _font, "", 65535);
        textField16._textObj.SetColor(Engine.Color.Gray2);
        Engine.Button button62 = new Engine.Button(_textureManager, 527.0, 81.0, 70.0, 22.0, "butt001_F30", "butt001_F31", "", "butt001_F32");
        button62._onPressEvent = delegate
        {
            RequestBoard(_viewPostMenu._boardID, 3, _viewPostMenu._selectedID, 2);
        };
        Engine.Button button63 = new Engine.Button(_textureManager, 527.0, 107.0, 70.0, 22.0, "butt001_F9", "butt001_F10", "", "butt001_F11");
        button63._onPressEvent = delegate
        {
            RequestBoard(_viewPostMenu._boardID, 3, _viewPostMenu._selectedID, 1);
        };
        Engine.Button button64 = new Engine.Button(_textureManager, 527.0, 133.0, 70.0, 22.0, "butt001_F6", "butt001_F7");
        button64._onPressEvent = delegate
        {
            _composePostMenu._boardID = _viewPostMenu._boardID;
            _composePostMenu._textFields["composeToName"].Text = _player._name;
            _composePostMenu._textFields["composeToName"]._allowClick = false;
            _composePostMenu._textFields["composeToName"]._textObj.SetColor(Engine.Color.Gray2);
            _composePostMenu._textFields["composeTitle"].SetFocus();
            _composePostMenu._currentTabIndex = 2;
            _composingPost = true;
            _viewingPost = false;
        };
        Engine.Button button65 = new Engine.Button(_textureManager, 527.0, 159.0, 70.0, 22.0, "butt001_F27", "butt001_F28", "", "butt001_F29");
        button65._onPressEvent = delegate
        {
            _composePostMenu._boardID = _viewPostMenu._boardID;
            _composePostMenu._textFields["composeToName"].Text = boardPosts[_viewPostMenu._selectedID].PosterName;
            _composePostMenu._textFields["composeToName"]._allowClick = false;
            _composePostMenu._textFields["composeToName"]._textObj.SetColor(Engine.Color.Gray2);
            _composePostMenu._textFields["composeTitle"].SetFocus();
            _composePostMenu._currentTabIndex = 2;
            _composingPost = true;
            _viewingPost = false;
        };
        Engine.Button button66 = new Engine.Button(_textureManager, 527.0, 185.0, 70.0, 22.0, "butt001_F18", "butt001_F19", "", "butt001_F20");
        button66._onPressEvent = delegate
        {
            if (_viewPostMenu._selectedID != 0)
            {
                _deletePrompt._labels["delpromptText"].ChangeText("Would you like to delete?");
            }
        };
        Engine.Button button67 = new Engine.Button(_textureManager, 527.0, 237.0, 70.0, 22.0, "butt001_F12", "butt001_F13");
        button67._onPressEvent = delegate
        {
            RequestBoard(_viewPostMenu._boardID, 2, 0, 0);
            _viewingPost = false;
        };
        Engine.Button button68 = new Engine.Button(_textureManager, 531.0, 274.0, 70.0, 22.0, "butt001_F33", "butt001_F34");
        button68._onPressEvent = delegate
        {
            _viewingPost = false;
            _viewingMailList = false;
            _viewingBoardList = false;
            _boardMenu._buttons["closeBoardBtn"].OnPress();
        };
        _viewPostMenu.AddTextField("bodyViewTF", textField16);
        _viewPostMenu.AddLabel("nameViewLabel", label45);
        _viewPostMenu.AddLabel("dateViewLabel", label46);
        _viewPostMenu.AddLabel("titleViewLabel", label47);
        _viewPostMenu.AddButton("prevViewBtn", button63);
        _viewPostMenu.AddButton("nextViewBtn", button62);
        _viewPostMenu.AddButton("newViewBtn", button64);
        _viewPostMenu.AddButton("replyViewBtn", button65);
        _viewPostMenu.AddButton("deleteViewBtn", button66);
        _viewPostMenu.AddButton("upViewBtn", button67);
        _viewPostMenu.AddButton("closeViewBtn", button68);
        // === Online users window ===
        _usersMenu = new ButtonMenu(_input, _font);
        _usersMenu._background.Texture = _textureManager.Get("users01");
        _usersMenu._background.SetPosition(150.0, 4.0);
        Text label48 = DrawLabel("", Engine.Color.Gray2, 310.0, 27.0, 90, "right");
        Text label49 = DrawLabel("", Engine.Color.Gray2, 178.0, 27.0, 132, "right");
        Text label50 = DrawLabel("Temuair", Engine.Color.Gray2, 484.0, 21.0, 65, "center");
        Text label51 = DrawLabel("", Engine.Color.Gray2, 548.0, 52.0, 20, "center");
        Text label52 = DrawLabel("", Engine.Color.Gray2, 548.0, 79.0, 20, "center");
        Text label53 = DrawLabel("", Engine.Color.Gray2, 548.0, 99.0, 20, "center");
        Text label54 = DrawLabel("", Engine.Color.Gray2, 548.0, 119.0, 20, "center");
        Text label55 = DrawLabel("", Engine.Color.Gray2, 548.0, 139.0, 20, "center");
        Text label56 = DrawLabel("", Engine.Color.Gray2, 548.0, 159.0, 20, "center");
        Text label57 = DrawLabel("", Engine.Color.Gray2, 548.0, 179.0, 20, "center");
        Text label58 = DrawLabel("", Engine.Color.Gray2, 548.0, 199.0, 20, "center");
        Text label59 = DrawLabel("", Engine.Color.Gray2, 548.0, 219.0, 20, "center");
        Text label60 = DrawLabel("", Engine.Color.Gray2, 548.0, 239.0, 20, "center");
        Engine.Button countryBtn = new Engine.Button(_textureManager, 450.0, 45.0, 68.0, 22.0, "", "users04_F0");
        countryBtn.Selected = true;
        Engine.Button masterBtn = new Engine.Button(_textureManager, 450.0, 72.0, 68.0, 22.0, "", "users04_F1");
        Engine.Button warriorBtn = new Engine.Button(_textureManager, 450.0, 92.0, 68.0, 22.0, "", "users04_F2");
        Engine.Button rogueBtn = new Engine.Button(_textureManager, 450.0, 112.0, 68.0, 22.0, "", "users04_F3");
        Engine.Button wizardBtn = new Engine.Button(_textureManager, 450.0, 132.0, 68.0, 22.0, "", "users04_F4");
        Engine.Button priestBtn = new Engine.Button(_textureManager, 450.0, 152.0, 68.0, 22.0, "", "users04_F5");
        Engine.Button monkBtn = new Engine.Button(_textureManager, 450.0, 172.0, 68.0, 22.0, "", "users04_F6");
        Engine.Button peasantBtn = new Engine.Button(_textureManager, 450.0, 192.0, 68.0, 22.0, "", "users04_F7");
        Engine.Button guildBtn = new Engine.Button(_textureManager, 450.0, 212.0, 68.0, 22.0, "", "users04_F8");
        Engine.Button friendUBtn = new Engine.Button(_textureManager, 450.0, 232.0, 68.0, 22.0, "");
        friendUBtn._clickedImage = _textureManager.Get("users05");
        Engine.Button button69 = new Engine.Button(_textureManager, 531.0, 274.0, 70.0, 22.0, "butt001_F33", "butt001_F34");
        countryBtn._onPressEvent = delegate
        {
            if (!countryBtn.Selected)
            {
                ToggleSelectedButtons(_usersMenu, countryBtn);
            }
        };
        masterBtn._onPressEvent = delegate
        {
            if (!masterBtn.Selected)
            {
                ToggleSelectedButtons(_usersMenu, masterBtn);
            }
        };
        warriorBtn._onPressEvent = delegate
        {
            if (!warriorBtn.Selected)
            {
                ToggleSelectedButtons(_usersMenu, warriorBtn);
            }
        };
        rogueBtn._onPressEvent = delegate
        {
            if (!rogueBtn.Selected)
            {
                ToggleSelectedButtons(_usersMenu, rogueBtn);
            }
        };
        wizardBtn._onPressEvent = delegate
        {
            if (!wizardBtn.Selected)
            {
                ToggleSelectedButtons(_usersMenu, wizardBtn);
            }
        };
        priestBtn._onPressEvent = delegate
        {
            if (!priestBtn.Selected)
            {
                ToggleSelectedButtons(_usersMenu, priestBtn);
            }
        };
        monkBtn._onPressEvent = delegate
        {
            if (!monkBtn.Selected)
            {
                ToggleSelectedButtons(_usersMenu, monkBtn);
            }
        };
        peasantBtn._onPressEvent = delegate
        {
            if (!peasantBtn.Selected)
            {
                ToggleSelectedButtons(_usersMenu, peasantBtn);
            }
        };
        guildBtn._onPressEvent = delegate
        {
            if (!guildBtn.Selected)
            {
                ToggleSelectedButtons(_usersMenu, guildBtn);
            }
        };
        friendUBtn._onPressEvent = delegate
        {
            if (!friendUBtn.Selected)
            {
                ToggleSelectedButtons(_usersMenu, friendUBtn);
            }
        };
        button69._onPressEvent = delegate
        {
            _viewingUsers = false;
            ToggleSelectedButtons(_usersMenu);
            ToggleSelectedButtons(_menu);
        };
        Engine.Button button70 = new Engine.Button(_textureManager, 406.0, 28.0, 13.0, 12.0, "scroll_F0", "scroll_F1", "", "scroll_F1");
        button70.Enabled = false;
        Engine.Button button71 = new Engine.Button(_textureManager, 406.0, 268.0, 13.0, 12.0, "scroll_F2", "scroll_F3", "", "scroll_F3");
        button71.Enabled = false;
        Engine.Button button72 = new Engine.Button(_textureManager, 406.0, 40.0, 13.0, 12.0, "scroll_F4", "scroll_F4");
        button72.Hidden = true;
        Engine.Button button73 = new Engine.Button(_textureManager, 406.0, 40.0, 13.0, 228.0, "scroll_F5", "scroll_F5", "", "", null, null, 19);
        button70._onHeldEvent = delegate
        {
            if (_userIndex > 0)
            {
                int count18 = _userList.Count;
                _userIndex--;
                RepositionScroller(_userIndex, count18 - 14, 216, _usersMenu, _usersMenu._buttons["usersScrollerBtn"], 406, 40);
            }
        };
        button70._onPressEvent = delegate
        {
            if (_userIndex > 0)
            {
                int count17 = _userList.Count;
                _userIndex--;
                RepositionScroller(_userIndex, count17 - 14, 216, _usersMenu, _usersMenu._buttons["usersScrollerBtn"], 406, 40);
            }
        };
        button71._onHeldEvent = delegate
        {
            if (_userList.Count - 14 > 0 && _userIndex < _userList.Count - 14)
            {
                int count16 = _userList.Count;
                _userIndex++;
                RepositionScroller(_userIndex, count16 - 14, 216, _usersMenu, _usersMenu._buttons["usersScrollerBtn"], 406, 40);
            }
        };
        button71._onPressEvent = delegate
        {
            if (_userList.Count - 14 > 0 && _userIndex < _userList.Count - 14)
            {
                int count15 = _userList.Count;
                _userIndex++;
                RepositionScroller(_userIndex, count15 - 14, 216, _usersMenu, _usersMenu._buttons["usersScrollerBtn"], 406, 40);
            }
        };
        button72._onHeldEvent = delegate
        {
            int count14 = _userList.Count;
            double num12 = ((double)_input.Mouse.Position.Y - _usersMenu._position.Y - 40.0 - 6.0) / 222.0;
            int num13 = (int)Math.Round(num12 * (double)(count14 - 14));
            _userIndex = ((num13 >= 0) ? ((num13 > count14 - 14) ? (count14 - 14) : num13) : 0);
            RepositionScroller(_userIndex, count14 - 14, 216, _usersMenu, _usersMenu._buttons["usersScrollerBtn"], 406, 40);
        };
        button73._onPressEvent = delegate
        {
            int count13 = _userList.Count;
            double num10 = ((double)_input.Mouse.Position.Y - _usersMenu._position.Y - 40.0 - 6.0) / 222.0;
            int num11 = (int)Math.Round(num10 * (double)(count13 - 14));
            _userIndex = ((num11 >= 0) ? ((num11 > count13 - 14) ? (count13 - 14) : num11) : 0);
            RepositionScroller(_userIndex, count13 - 14, 216, _usersMenu, _usersMenu._buttons["usersScrollerBtn"], 406, 40);
        };
        _usersMenu.AddButton("usersScrollBackBtn", button73);
        _usersMenu.AddButton("usersScrollerBtn", button72);
        _usersMenu.AddButton("usersScrollUpBtn", button70);
        _usersMenu.AddButton("usersScrollDownBtn", button71);
        _usersMenu.AddLabel("namesLabel", label48);
        _usersMenu.AddLabel("titlesLabel", label49);
        _usersMenu.AddLabel("worldLabel", label50);
        _usersMenu.AddLabel("totalLabel", label51);
        _usersMenu.AddLabel("masterLabel", label52);
        _usersMenu.AddLabel("warriorLabel", label53);
        _usersMenu.AddLabel("rogueLabel", label54);
        _usersMenu.AddLabel("wizardLabel", label55);
        _usersMenu.AddLabel("priestLabel", label56);
        _usersMenu.AddLabel("monkLabel", label57);
        _usersMenu.AddLabel("peasantLabel", label58);
        _usersMenu.AddLabel("guildLabel", label59);
        _usersMenu.AddLabel("friendLabel", label60);
        _usersMenu.AddButton("countryBtn", countryBtn);
        _usersMenu.AddButton("masterBtn", masterBtn);
        _usersMenu.AddButton("warriorBtn", warriorBtn);
        _usersMenu.AddButton("rogueBtn", rogueBtn);
        _usersMenu.AddButton("wizardBtn", wizardBtn);
        _usersMenu.AddButton("priestBtn", priestBtn);
        _usersMenu.AddButton("monkBtn", monkBtn);
        _usersMenu.AddButton("peasantBtn", peasantBtn);
        _usersMenu.AddButton("guildBtn", guildBtn);
        _usersMenu.AddButton("friendUBtn", friendUBtn);
        _usersMenu.AddButton("closeUsersBtn", button69);
        // === Misc HUD (chat input + status labels) ===
        _miscMenu = new ButtonMenu(_input, _font);
        portraitFlap._onPressEvent = delegate
        {
            if (_portraitFlap)
            {
                _portraitFlap = false;
                SaveClientSettings();
                portraitFlap.Animate(35.0, reverse: true);
            }
            else
            {
                _portraitFlap = true;
                SaveClientSettings();
                portraitFlap.Animate(35.0);
            }
        };
        fullInvBtn._onPressEvent = delegate
        {
            if (fullInvBtn.Selected)
            {
                fullInvBtn.Selected = false;
                _inventoryMenu._background.SetPosition(96.0, 336.0);
            }
            else
            {
                _inventoryMenu._background.SetPosition(96.0, 234.0);
                fullInvBtn.Selected = true;
            }
        };
        _miscMenu.AddButton("fullInvBtn", fullInvBtn);
        _miscMenu.AddButton("portraitFlap", portraitFlap);
        _miscMenu.AddLabel("upperLeft1Label", label8);
        _miscMenu.AddLabel("upperLeft2Label", label9);
        _miscMenu.AddLabel("upperLeft3Label", label10);
        _miscMenu.AddLabel("nameLabel", label);
        _miscMenu.AddLabel("infoLabel", label2);
        _miscMenu.AddLabel("userMsgLabel", label4);
        _miscMenu.AddTextField("userMsgTF", textField);
        _miscMenu.AddTextField("chatTF", textField2);
        _miscMenu.AddLabel("chatLabel", label3);
        _miscMenu.AddLabel("locationLabel", label7);
        _miscMenu.AddLabel("mapLabel", label5);
        _miscMenu.AddLabel("questLabel", label6);
        vector = new Vector(141.0, 74.0, 0.0);
        // === Sense / examine popup ===
        _senseMenu = new ButtonMenu(_input, _font, vector.X, vector.Y, 360.0, 180.0, moveable: true);
        _senseMenu._background.Texture = _textureManager.Get("sense");
        _senseMenu._background.SetPosition(vector);
        Text label61 = DrawLabel("", Engine.Color.Gray2, 20.0, 20.0, 320, "left", 9, shade: false, vector.X, vector.Y);
        Engine.Button button74 = new Engine.Button(_textureManager, 280.0, 147.0, 70.0, 22.0, "butt001_F3", "butt001_F4", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button74._onPressEvent = delegate
        {
            _viewingSense = false;
        };
        _senseMenu.AddLabel("senseLabel", label61);
        _senseMenu.AddButton("quitSenseBtn", button74);
        vector = new Vector(141.0, 74.0, 0.0);
        // === Sign popup ===
        _signMenu = new ButtonMenu(_input, _font, vector.X, vector.Y, 360.0, 180.0, moveable: true);
        _signMenu._background.Texture = _textureManager.Get("woodbk_F0_C0");
        _signMenu._background.SetPosition(vector);
        Text label62 = DrawLabel("", Engine.Color.White, 20.0, 20.0, 340, "left", 9, shade: false, vector.X, vector.Y);
        Engine.Button button75 = new Engine.Button(_textureManager, 280.0, 147.0, 70.0, 22.0, "", "", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button75._onPressEvent = delegate
        {
            _viewingSign = false;
        };
        _signMenu.AddLabel("signLabel", label62);
        _signMenu.AddButton("quitSignBtn", button75);
        vector = new Vector(dDP.X, dDP.Y, 0.0);
        // === Standard NPC dialog popup ===
        _standardDialogPopup = new ButtonMenu(_input, _font, vector.X, vector.Y, 454.0, 180.0, moveable: true);
        _standardDialogPopup._background.SetPosition(vector);
        Engine.Button button76 = new Engine.Button(_textureManager, 20.0, 143.0, 70.0, 22.0, "butt001_F9", "butt001_F10", "", "butt001_F11", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button76.Enabled = false;
        Engine.Button button77 = new Engine.Button(_textureManager, 20.0, 143.0, 70.0, 22.0, "butt001_F24", "butt001_F25", "", "butt001_F26", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button77.Enabled = false;
        button77.Hidden = true;
        Engine.Button button78 = new Engine.Button(_textureManager, 90.0, 143.0, 70.0, 22.0, "butt001_F30", "butt001_F31", "", "butt001_F32", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button78.Enabled = false;
        Engine.Button button79 = new Engine.Button(_textureManager, 364.0, 143.0, 70.0, 22.0, "butt001_F3", "butt001_F4", "", "butt001_F5", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button79._onPressEvent = delegate
        {
            _viewingDialog = false;
        };
        Text label63 = DrawLabel("", Engine.Color.LightBlue, 18.0, 86.0, 100, "center", 1, shade: true, vector.X, vector.Y);
        Text label64 = DrawLabel("", Engine.Color.White, 134.0, 17.0, 300, "left", 9, shade: false, vector.X, vector.Y);
        _standardDialogPopup.AddButton("sdpTopBtn", button77);
        _standardDialogPopup.AddButton("sdpPreviousBtn", button76);
        _standardDialogPopup.AddButton("sdpNextBtn", button78);
        _standardDialogPopup.AddButton("sdpQuitBtn", button79);
        _standardDialogPopup.AddLabel("sdpNameLabel", label63);
        _standardDialogPopup.AddLabel("sdpBodyLabel", label64);
        vector = new Vector(100.0, 4.0, 0.0);
        // === Commands-list window ===
        _commandMenu = new ButtonMenu(_input, _font, vector.X, vector.Y, 451.0, 418.0, moveable: true);
        _commandMenu._background.Texture = _textureManager.Get("legend_F0_C0");
        _commandMenu._background.SetPosition(vector);
        Text label65 = DrawLabel("Slash Commands", Engine.Color.Gray2, 178.0, 30.0, 115, "center", 0, shade: false, vector.X, vector.Y);
        Engine.Button button80 = new Engine.Button(_textureManager, 365.0, 377.0, 70.0, 22.0, "butt001_F33", "butt001_F34", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button80._onPressEvent = delegate
        {
            _viewCommands = false;
        };
        Engine.Button button81 = new Engine.Button(_textureManager, 421.0, 60.0, 13.0, 12.0, "scroll_F0", "scroll_F1", "", "scroll_F1", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button81.Enabled = false;
        Engine.Button button82 = new Engine.Button(_textureManager, 421.0, 348.0, 13.0, 12.0, "scroll_F2", "scroll_F3", "", "scroll_F3", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button82.Enabled = false;
        Engine.Button button83 = new Engine.Button(_textureManager, 421.0, 72.0, 13.0, 12.0, "scroll_F4", "scroll_F4", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button83.Hidden = true;
        Engine.Button button84 = new Engine.Button(_textureManager, 421.0, 72.0, 13.0, 276.0, "scroll_F5", "scroll_F5", "", "", null, null, 23, multHorzontal: false, vector.X, vector.Y);
        button81._onHeldEvent = delegate
        {
            if (_commandIndex > 0)
            {
                int count12 = commandBodyText.Count;
                _commandIndex--;
                RepositionScroller(_commandIndex, count12 - 25, 264, _commandMenu, _commandMenu._buttons["commandScrollerBtn"], 421, 72);
            }
        };
        button81._onPressEvent = delegate
        {
            if (_commandIndex > 0)
            {
                int count11 = commandBodyText.Count;
                _commandIndex--;
                RepositionScroller(_commandIndex, count11 - 25, 264, _commandMenu, _commandMenu._buttons["commandScrollerBtn"], 421, 72);
            }
        };
        button82._onHeldEvent = delegate
        {
            if (commandBodyText.Count - 25 > 0 && _commandIndex < commandBodyText.Count - 25)
            {
                int count10 = commandBodyText.Count;
                _commandIndex++;
                RepositionScroller(_commandIndex, count10 - 25, 264, _commandMenu, _commandMenu._buttons["commandScrollerBtn"], 421, 72);
            }
        };
        button82._onPressEvent = delegate
        {
            if (commandBodyText.Count - 25 > 0 && _commandIndex < commandBodyText.Count - 25)
            {
                int count9 = commandBodyText.Count;
                _commandIndex++;
                RepositionScroller(_commandIndex, count9 - 25, 264, _commandMenu, _commandMenu._buttons["commandScrollerBtn"], 421, 72);
            }
        };
        button83._onHeldEvent = delegate
        {
            int count8 = commandBodyText.Count;
            double num8 = ((double)_input.Mouse.Position.Y - _commandMenu._position.Y - 72.0 - 6.0) / 270.0;
            int num9 = (int)Math.Round(num8 * (double)(count8 - 25));
            _commandIndex = ((num9 >= 0) ? ((num9 > count8 - 25) ? (count8 - 25) : num9) : 0);
            RepositionScroller(_commandIndex, count8 - 25, 264, _commandMenu, _commandMenu._buttons["commandScrollerBtn"], 421, 72);
        };
        button84._onPressEvent = delegate
        {
            int count7 = commandBodyText.Count;
            double num6 = ((double)_input.Mouse.Position.Y - _commandMenu._position.Y - 72.0 - 6.0) / 270.0;
            int num7 = (int)Math.Round(num6 * (double)(count7 - 25));
            _commandIndex = ((num7 >= 0) ? ((num7 > count7 - 25) ? (count7 - 25) : num7) : 0);
            RepositionScroller(_commandIndex, count7 - 25, 264, _commandMenu, _commandMenu._buttons["commandScrollerBtn"], 421, 72);
        };
        _commandMenu.AddButton("commandScrollBackBtn", button84);
        _commandMenu.AddButton("commandScrollerBtn", button83);
        _commandMenu.AddButton("commandScrollUpBtn", button81);
        _commandMenu.AddButton("commandScrollDownBtn", button82);
        _commandMenu.AddLabel("commandNameLabel", label65);
        _commandMenu.AddButton("closeCommandMenuBtn", button80);
        vector = new Vector(100.0, 4.0, 0.0);
        // === Keyboard-shortcuts list window ===
        _keyboardMenu = new ButtonMenu(_input, _font, vector.X, vector.Y, 451.0, 418.0, moveable: true);
        _keyboardMenu._background.Texture = _textureManager.Get("legend_F0_C0");
        _keyboardMenu._background.SetPosition(vector);
        Text label66 = DrawLabel("Key Commands", Engine.Color.Gray2, 178.0, 30.0, 115, "center", 0, shade: false, vector.X, vector.Y);
        Engine.Button button85 = new Engine.Button(_textureManager, 365.0, 377.0, 70.0, 22.0, "butt001_F33", "butt001_F34", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button85._onPressEvent = delegate
        {
            _viewKeyboards = false;
        };
        Engine.Button button86 = new Engine.Button(_textureManager, 421.0, 60.0, 13.0, 12.0, "scroll_F0", "scroll_F1", "", "scroll_F1", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button86.Enabled = true;
        Engine.Button button87 = new Engine.Button(_textureManager, 421.0, 348.0, 13.0, 12.0, "scroll_F2", "scroll_F3", "", "scroll_F3", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button87.Enabled = true;
        Engine.Button button88 = new Engine.Button(_textureManager, 421.0, 72.0, 13.0, 12.0, "scroll_F4", "scroll_F4", "", "", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button88.Enabled = true;
        Engine.Button button89 = new Engine.Button(_textureManager, 421.0, 72.0, 13.0, 276.0, "scroll_F5", "scroll_F5", "", "", null, null, 23, multHorzontal: false, vector.X, vector.Y);
        button86._onHeldEvent = delegate
        {
            if (_keyboardIndex > 0)
            {
                int count6 = keyboardBodyText.Count;
                _keyboardIndex--;
                RepositionScroller(_keyboardIndex, count6 - 25, 264, _keyboardMenu, _keyboardMenu._buttons["keyboardScrollerBtn"], 421, 72);
            }
        };
        button86._onPressEvent = delegate
        {
            if (_keyboardIndex > 0)
            {
                int count5 = keyboardBodyText.Count;
                _keyboardIndex--;
                RepositionScroller(_keyboardIndex, count5 - 25, 264, _keyboardMenu, _keyboardMenu._buttons["keyboardScrollerBtn"], 421, 72);
            }
        };
        button87._onHeldEvent = delegate
        {
            if (keyboardBodyText.Count - 25 > 0 && _keyboardIndex < keyboardBodyText.Count - 25)
            {
                int count4 = keyboardBodyText.Count;
                _keyboardIndex++;
                RepositionScroller(_keyboardIndex, count4 - 25, 264, _keyboardMenu, _keyboardMenu._buttons["keyboardScrollerBtn"], 421, 72);
            }
        };
        button87._onPressEvent = delegate
        {
            if (keyboardBodyText.Count - 25 > 0 && _keyboardIndex < keyboardBodyText.Count - 25)
            {
                int count3 = keyboardBodyText.Count;
                _keyboardIndex++;
                RepositionScroller(_keyboardIndex, count3 - 25, 264, _keyboardMenu, _keyboardMenu._buttons["keyboardScrollerBtn"], 421, 72);
            }
        };
        button88._onHeldEvent = delegate
        {
            int count2 = keyboardBodyText.Count;
            double num4 = ((double)_input.Mouse.Position.Y - _keyboardMenu._position.Y - 72.0 - 6.0) / 270.0;
            int num5 = (int)Math.Round(num4 * (double)(count2 - 25));
            _keyboardIndex = ((num5 >= 0) ? ((num5 > count2 - 25) ? (count2 - 25) : num5) : 0);
            RepositionScroller(_keyboardIndex, count2 - 25, 264, _keyboardMenu, _keyboardMenu._buttons["keyboardScrollerBtn"], 421, 72);
        };
        button89._onPressEvent = delegate
        {
            int count = keyboardBodyText.Count;
            double num2 = ((double)_input.Mouse.Position.Y - _keyboardMenu._position.Y - 72.0 - 6.0) / 270.0;
            int num3 = (int)Math.Round(num2 * (double)(count - 25));
            _keyboardIndex = ((num3 >= 0) ? ((num3 > count - 25) ? (count - 25) : num3) : 0);
            RepositionScroller(_keyboardIndex, count - 25, 264, _keyboardMenu, _keyboardMenu._buttons["keyboardScrollerBtn"], 421, 72);
        };
        _keyboardMenu.AddButton("keyboardScrollBackBtn", button89);
        _keyboardMenu.AddButton("keyboardScrollerBtn", button88);
        _keyboardMenu.AddButton("keyboardScrollUpBtn", button86);
        _keyboardMenu.AddButton("keyboardScrollDownBtn", button87);
        _keyboardMenu.AddLabel("keyboardNameLabel", label66);
        _keyboardMenu.AddButton("closeKeyboardMenuBtn", button85);
        // === Info bar (scrolling system messages) ===
        _infoBarMenu = new ButtonMenu(_input, _font, 88.0, 317.0, 443.0, 12.0);
        Engine.Button sysmsgBtn = new Engine.Button(_textureManager, 88.0, 317.0, 433.0, 12.0, "");
        Text label67 = DrawLabel("", Engine.Color.Orange, 94.0, 318.0, 420, "left");
        Text label68 = DrawLabel("", Engine.Color.Orange, 94.0, 330.0, 420, "left");
        Text label69 = DrawLabel("", Engine.Color.Orange, 94.0, 342.0, 420, "left");
        Text label70 = DrawLabel("", Engine.Color.Orange, 94.0, 354.0, 420, "left");
        Text label71 = DrawLabel("", Engine.Color.Orange, 94.0, 366.0, 420, "left");
        Text label72 = DrawLabel("", Engine.Color.Orange, 94.0, 378.0, 420, "left");
        Text label73 = DrawLabel("", Engine.Color.Orange, 94.0, 390.0, 420, "left");
        Text label74 = DrawLabel("", Engine.Color.Orange, 94.0, 402.0, 420, "left");
        Text label75 = DrawLabel("", Engine.Color.Orange, 94.0, 414.0, 420, "left");
        Text label76 = DrawLabel("", Engine.Color.Orange, 94.0, 426.0, 420, "left");
        LoadSysMsgImages();
        sysmsgBtn._onReleaseEvent = delegate
        {
            _infoBarMenuIndex = 0;
            sysmsgBtn.Sprite.Texture = sysmsgBtn._blankImage;
            ClearSysMsgLabels();
        };
        sysmsgBtn._onHeldEvent = delegate
        {
            Engine.Point position = _input.Mouse.Position;
            if (position.Y < 329f && _infoBarMenuIndex != 0)
            {
                _infoBarMenuIndex = 0;
                sysmsgBtn.Sprite.Texture = sysmsgBtn._blankImage;
            }
            else if (position.Y >= 329f && position.Y < 341f && _infoBarMenuIndex != 1)
            {
                _infoBarMenuIndex = 1;
                sysmsgBtn.Sprite.Texture = _textureManager.Get("sysmsg_F1_C0");
            }
            else if (position.Y >= 341f && position.Y < 353f && _infoBarMenuIndex != 2)
            {
                _infoBarMenuIndex = 2;
                sysmsgBtn.Sprite.Texture = _textureManager.Get("sysmsg_F2_C0");
            }
            else if (position.Y >= 353f && position.Y < 365f && _infoBarMenuIndex != 3)
            {
                _infoBarMenuIndex = 3;
                sysmsgBtn.Sprite.Texture = _textureManager.Get("sysmsg_F3_C0");
            }
            else if (position.Y >= 365f && position.Y < 377f && _infoBarMenuIndex != 4)
            {
                _infoBarMenuIndex = 4;
                sysmsgBtn.Sprite.Texture = _textureManager.Get("sysmsg_F4_C0");
            }
            else if (position.Y >= 377f && position.Y < 389f && _infoBarMenuIndex != 5)
            {
                _infoBarMenuIndex = 5;
                sysmsgBtn.Sprite.Texture = _textureManager.Get("sysmsg_F5_C0");
            }
            else if (position.Y >= 389f && position.Y < 401f && _infoBarMenuIndex != 6)
            {
                _infoBarMenuIndex = 6;
                sysmsgBtn.Sprite.Texture = _textureManager.Get("sysmsg_F6_C0");
            }
            else if (position.Y >= 401f && position.Y < 413f && _infoBarMenuIndex != 7)
            {
                _infoBarMenuIndex = 7;
                sysmsgBtn.Sprite.Texture = _textureManager.Get("sysmsg_F7_C0");
            }
            else if (position.Y >= 413f && position.Y < 425f && _infoBarMenuIndex != 8)
            {
                _infoBarMenuIndex = 8;
                sysmsgBtn.Sprite.Texture = _textureManager.Get("sysmsg_F8_C0");
            }
            else if (position.Y >= 425f && _infoBarMenuIndex != 9)
            {
                _infoBarMenuIndex = 9;
                sysmsgBtn.Sprite.Texture = _textureManager.Get("sysmsg_F0_C0");
            }
            ClearSysMsgLabels();
        };
        _infoBarMenu.AddLabel("sysmsg1", label67);
        _infoBarMenu.AddLabel("sysmsg2", label68);
        _infoBarMenu.AddLabel("sysmsg3", label69);
        _infoBarMenu.AddLabel("sysmsg4", label70);
        _infoBarMenu.AddLabel("sysmsg5", label71);
        _infoBarMenu.AddLabel("sysmsg6", label72);
        _infoBarMenu.AddLabel("sysmsg7", label73);
        _infoBarMenu.AddLabel("sysmsg8", label74);
        _infoBarMenu.AddLabel("sysmsg9", label75);
        _infoBarMenu.AddLabel("sysmsg10", label76);
        _infoBarMenu.AddButton("sysmsgBtn", sysmsgBtn);
        vector = new Vector(190.0, 183.0, 0.0);
        // === Group-request prompt ===
        _groupRequestPrompt = new ButtonMenu(_input, _font, vector.X, vector.Y, 263.0, 74.0, moveable: true);
        _groupRequestPrompt._sprite.Texture = _textureManager.Get("promptok");
        _groupRequestPrompt._sprite.SetPosition(vector);
        Engine.Button button90 = new Engine.Button(_textureManager, 104.0, 43.0, 70.0, 22.0, "butt001_F15", "butt001_F16", "", "butt001_F17", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        Engine.Button button91 = new Engine.Button(_textureManager, 184.0, 43.0, 70.0, 22.0, "butt001_F21", "butt001_F22", "", "butt001_F23", null, null, 0, multHorzontal: false, vector.X, vector.Y);
        button91._onPressEvent = delegate
        {
            _showGroupRequestPrompt = false;
        };
        button90._onPressEvent = delegate
        {
            if (GameWindow.ConnectedToServer)
            {
                string[] array = _groupRequestPrompt._labels["promptText"]._text.Split(' ');
                string name = array[0];
                GroupRequestPacket groupRequestPacket = new GroupRequestPacket(1, name);
                GameWindow.ClientSocket.Send(groupRequestPacket.Data);
            }
            _showGroupRequestPrompt = false;
        };
        Text value3 = DrawLabel("", Engine.Color.White, 6.0, 6.0, 250, "left", 5, shade: false, vector.X, vector.Y);
        _groupRequestPrompt._labels.Add("groupPromptText", value3);
        _groupRequestPrompt._buttons.Add("groupPromptOkBtn", button90);
        _groupRequestPrompt._buttons.Add("groupPromptCloseBtn", button91);
        // === Group-list window ===
        _groupListMenu = new ButtonMenu(_input, _font);
        _groupListMenu._background.Texture = _textureManager.Get("group1");
        _groupListMenu._background.SetPosition(0.0, 4.0);
        Engine.Button button92 = new Engine.Button(_textureManager, 370.0, 274.0, 70.0, 22.0, "butt001_F33", "butt001_F34");
        Text value4 = DrawLabel("", Engine.Color.White, 68.0, 62.0, 72, "left");
        Text value5 = DrawLabel("", Engine.Color.Gray2, 68.0, 81.0, 72, "left");
        Text value6 = DrawLabel("", Engine.Color.Gray2, 68.0, 100.0, 72, "left");
        Text value7 = DrawLabel("", Engine.Color.Gray2, 68.0, 119.0, 72, "left");
        Text value8 = DrawLabel("", Engine.Color.Gray2, 68.0, 138.0, 72, "left");
        Text value9 = DrawLabel("", Engine.Color.Gray2, 68.0, 157.0, 72, "left");
        Text value10 = DrawLabel("", Engine.Color.Gray2, 68.0, 176.0, 72, "left");
        Text value11 = DrawLabel("", Engine.Color.Gray2, 68.0, 195.0, 72, "left");
        Text value12 = DrawLabel("", Engine.Color.Gray2, 68.0, 214.0, 72, "left");
        Text value13 = DrawLabel("", Engine.Color.Gray2, 68.0, 233.0, 72, "left");
        Text value14 = DrawLabel("", Engine.Color.Gray2, 158.0, 62.0, 270, "left");
        Text value15 = DrawLabel("", Engine.Color.Gray2, 158.0, 81.0, 270, "left");
        Text value16 = DrawLabel("", Engine.Color.Gray2, 158.0, 100.0, 270, "left");
        Text value17 = DrawLabel("", Engine.Color.Gray2, 158.0, 119.0, 270, "left");
        Text value18 = DrawLabel("", Engine.Color.Gray2, 158.0, 138.0, 270, "left");
        Text value19 = DrawLabel("", Engine.Color.Gray2, 158.0, 157.0, 270, "left");
        Text value20 = DrawLabel("", Engine.Color.Gray2, 158.0, 176.0, 270, "left");
        Text value21 = DrawLabel("", Engine.Color.Gray2, 158.0, 195.0, 270, "left");
        Text value22 = DrawLabel("", Engine.Color.Gray2, 158.0, 214.0, 270, "left");
        Text value23 = DrawLabel("", Engine.Color.Gray2, 158.0, 233.0, 270, "left");
        Engine.Button groupQuit1Btn = new Engine.Button(_textureManager, 29.0, 60.0, 21.0, 17.0, "");
        groupQuit1Btn.Enabled = false;
        Engine.Button groupQuit2Btn = new Engine.Button(_textureManager, 29.0, 79.0, 21.0, 17.0, "");
        groupQuit2Btn.Enabled = false;
        Engine.Button button93 = new Engine.Button(_textureManager, 29.0, 98.0, 21.0, 17.0, "");
        button93.Enabled = false;
        Engine.Button button94 = new Engine.Button(_textureManager, 29.0, 117.0, 21.0, 17.0, "");
        button94.Enabled = false;
        Engine.Button button95 = new Engine.Button(_textureManager, 29.0, 136.0, 21.0, 17.0, "");
        button95.Enabled = false;
        Engine.Button button96 = new Engine.Button(_textureManager, 29.0, 155.0, 21.0, 17.0, "");
        button96.Enabled = false;
        Engine.Button button97 = new Engine.Button(_textureManager, 29.0, 174.0, 21.0, 17.0, "");
        button97.Enabled = false;
        Engine.Button button98 = new Engine.Button(_textureManager, 29.0, 193.0, 21.0, 17.0, "");
        button98.Enabled = false;
        Engine.Button button99 = new Engine.Button(_textureManager, 29.0, 212.0, 21.0, 17.0, "");
        button99.Enabled = false;
        Engine.Button button100 = new Engine.Button(_textureManager, 29.0, 231.0, 21.0, 17.0, "");
        button100.Enabled = false;
        groupQuit1Btn._onPressEvent = delegate
        {
            _ = groupQuit1Btn.Enabled;
        };
        groupQuit2Btn._onPressEvent = delegate
        {
            _ = groupQuit2Btn.Enabled;
        };
        button92._onPressEvent = delegate
        {
            _viewingGroupList = false;
        };
        _groupListMenu._labels.Add("groupMap1Lab", value14);
        _groupListMenu._labels.Add("groupMap2Lab", value15);
        _groupListMenu._labels.Add("groupMap3Lab", value16);
        _groupListMenu._labels.Add("groupMap4Lab", value17);
        _groupListMenu._labels.Add("groupMap5Lab", value18);
        _groupListMenu._labels.Add("groupMap6Lab", value19);
        _groupListMenu._labels.Add("groupMap7Lab", value20);
        _groupListMenu._labels.Add("groupMap8Lab", value21);
        _groupListMenu._labels.Add("groupMap9Lab", value22);
        _groupListMenu._labels.Add("groupMap10Lab", value23);
        _groupListMenu._labels.Add("groupName1Lab", value4);
        _groupListMenu._labels.Add("groupName2Lab", value5);
        _groupListMenu._labels.Add("groupName3Lab", value6);
        _groupListMenu._labels.Add("groupName4Lab", value7);
        _groupListMenu._labels.Add("groupName5Lab", value8);
        _groupListMenu._labels.Add("groupName6Lab", value9);
        _groupListMenu._labels.Add("groupName7Lab", value10);
        _groupListMenu._labels.Add("groupName8Lab", value11);
        _groupListMenu._labels.Add("groupName9Lab", value12);
        _groupListMenu._labels.Add("groupName10Lab", value13);
        _groupListMenu._buttons.Add("groupQuit1Btn", groupQuit1Btn);
        _groupListMenu._buttons.Add("groupQuit2Btn", groupQuit2Btn);
        _groupListMenu._buttons.Add("groupQuit3Btn", button93);
        _groupListMenu._buttons.Add("groupQuit4Btn", button94);
        _groupListMenu._buttons.Add("groupQuit5Btn", button95);
        _groupListMenu._buttons.Add("groupQuit6Btn", button96);
        _groupListMenu._buttons.Add("groupQuit7Btn", button97);
        _groupListMenu._buttons.Add("groupQuit8Btn", button98);
        _groupListMenu._buttons.Add("groupQuit9Btn", button99);
        _groupListMenu._buttons.Add("groupQuit10Btn", button100);
        _groupListMenu._buttons.Add("closeGroupListBtn", button92);
    }

    /// <summary>Refreshes the info-bar's sysmsg labels (sysmsg1..10) from the recent-messages list, scrolled by <c>_infoBarMenuIndex</c>.</summary>
    private void ClearSysMsgLabels()
    {
        for (int i = 1; i <= _infoBarMenuIndex + 1; i++)
        {
            if (_infoBarList.Count() >= _infoBarMenuIndex + 2 - i)
            {
                _infoBarMenu._labels["sysmsg" + i].ChangeText(_infoBarList[_infoBarList.Count() - (_infoBarMenuIndex + 2 - i)], colorize: true);
            }
            else
            {
                _infoBarMenu._labels["sysmsg" + i].ChangeText("");
            }
        }
        for (int j = _infoBarMenuIndex + 2; j <= 10; j++)
        {
            _infoBarMenu._labels["sysmsg" + j].ChangeText("");
        }
        if (_infoBarMenuIndex == 0)
        {
            for (int k = 1; k <= 10; k++)
            {
                _infoBarMenu._labels["sysmsg" + k].ChangeText("");
            }
        }
    }

    /// <summary>Pre-renders the 8 info-bar background textures (sysmsg_F1..F8) by cropping sysmsg.epf at heights 26..110.</summary>
    private void LoadSysMsgImages()
    {
        EPFImage sysmsgEpf = EPFImage.FromArchive("sysmsg.epf", _textureManager.Dat("oldDat"));
        Image fullImage = DAGraphics.RenderImage(sysmsgEpf[0], _textureManager.Pal("oldPal"), new Size(sysmsgEpf.Width, sysmsgEpf.Height));
        int barHeight = 26;
        Image cropped = new Bitmap(443, barHeight);
        Graphics graphics = Graphics.FromImage(cropped);
        graphics.DrawImage(fullImage, 0, 0, new Rectangle(0, 123 - barHeight, 443, barHeight), GraphicsUnit.Pixel);
        _textureManager.LoadTexture("sysmsg_F1_C0", cropped);
        graphics.Dispose();
        barHeight = 38;
        cropped = new Bitmap(443, barHeight);
        graphics = Graphics.FromImage(cropped);
        graphics.DrawImage(fullImage, 0, 0, new Rectangle(0, 123 - barHeight, 443, barHeight), GraphicsUnit.Pixel);
        _textureManager.LoadTexture("sysmsg_F2_C0", cropped);
        graphics.Dispose();
        barHeight = 50;
        cropped = new Bitmap(443, barHeight);
        graphics = Graphics.FromImage(cropped);
        graphics.DrawImage(fullImage, 0, 0, new Rectangle(0, 123 - barHeight, 443, barHeight), GraphicsUnit.Pixel);
        _textureManager.LoadTexture("sysmsg_F3_C0", cropped);
        graphics.Dispose();
        barHeight = 62;
        cropped = new Bitmap(443, barHeight);
        graphics = Graphics.FromImage(cropped);
        graphics.DrawImage(fullImage, 0, 0, new Rectangle(0, 123 - barHeight, 443, barHeight), GraphicsUnit.Pixel);
        _textureManager.LoadTexture("sysmsg_F4_C0", cropped);
        graphics.Dispose();
        barHeight = 74;
        cropped = new Bitmap(443, barHeight);
        graphics = Graphics.FromImage(cropped);
        graphics.DrawImage(fullImage, 0, 0, new Rectangle(0, 123 - barHeight, 443, barHeight), GraphicsUnit.Pixel);
        _textureManager.LoadTexture("sysmsg_F5_C0", cropped);
        graphics.Dispose();
        barHeight = 86;
        cropped = new Bitmap(443, barHeight);
        graphics = Graphics.FromImage(cropped);
        graphics.DrawImage(fullImage, 0, 0, new Rectangle(0, 123 - barHeight, 443, barHeight), GraphicsUnit.Pixel);
        _textureManager.LoadTexture("sysmsg_F6_C0", cropped);
        graphics.Dispose();
        barHeight = 98;
        cropped = new Bitmap(443, barHeight);
        graphics = Graphics.FromImage(cropped);
        graphics.DrawImage(fullImage, 0, 0, new Rectangle(0, 123 - barHeight, 443, barHeight), GraphicsUnit.Pixel);
        _textureManager.LoadTexture("sysmsg_F7_C0", cropped);
        graphics.Dispose();
        barHeight = 110;
        cropped = new Bitmap(443, barHeight);
        graphics = Graphics.FromImage(cropped);
        graphics.DrawImage(fullImage, 0, 0, new Rectangle(0, 123 - barHeight, 443, barHeight), GraphicsUnit.Pixel);
        _textureManager.LoadTexture("sysmsg_F8_C0", cropped);
        graphics.Dispose();
    }

    /// <summary>Closes all secondary menus (group/settings/macros/friends/users/options/board) and clears their button selections.</summary>
    private void CloseMenus()
    {
        _viewingGroupList = false;
        _viewingSettings = false;
        _viewingMacros = false;
        _viewingFriends = false;
        _viewingUsers = false;
        _viewingOptions = false;
        _viewingPersonalBoard = false;
        ToggleSelectedButtons(_optionsMenu);
        ToggleSelectedButtons(_usersMenu);
        ToggleSelectedButtons(_boardMenu);
        ToggleSelectedButtons(_menu);
    }

    /// <summary>True if any modal/secondary view is currently open (dialog, board, settings, macros, dyeing, prompts, …).</summary>
    private bool _viewingStuff()
    {
        if (!_viewingDialog && !_viewingSense && !_viewingSign && !_viewingPersonalBoard && !_viewingMailList && !_viewingBoardList && !_viewingPost && !_composingPost && !_viewingOptions && !_viewingSettings && !_viewingFriends && !_viewingMacros && !_viewingUsers && !dyeingequipment && !_viewCommands && !_viewKeyboards && !_viewingGroupList && !_showGroupRequestPrompt)
        {
            return false;
        }
        return true;
    }

    /// <summary>On ESC, closes whatever is open, in priority order: chat/dye/keyboard-help/commands/loaded-spell, then each modal view (dialog/sign/sense/settings/macros/friends/options/board/mail/users/group/profile/legend/map).</summary>
    private void EscapeHotkey()
    {
        if (_input.Keyboard.IsKeyPressed(Keys.Escape))
        {
            _miscMenu._textFields["chatTF"]._textObj.RemoveCursor();
            _chatMode = false;
            _miscMenu._textFields["chatTF"].Text = "";
            _miscMenu._textFields["chatTF"]._textObjHighlighted.ChangeText("");
            _miscMenu._labels["chatLabel"].ChangeText("");
            if (!dyeingequipment)
            {
                _miscMenu._textFields["userMsgTF"]._textObj.RemoveCursor();
                _userMsgPrompt = false;
                _userMsg = "";
                _miscMenu._labels["userMsgLabel"].ChangeText("");
            }
            if (dyeingequipment)
            {
                dyeingequipment = false;
                selectedequipmentdye = 0;
                selectedequipmentdyeslot = 0;
                _userMsg = "";
            }
            else if (_viewKeyboards)
            {
                _viewKeyboards = false;
            }
            else if (_viewCommands)
            {
                _viewCommands = false;
            }
            else if (_loadedSpell != null)
            {
                _loadedSpell = null;
            }
            else if (_viewingDialog)
            {
                _viewingDialog = false;
            }
            else if (_viewingSign)
            {
                _viewingSign = false;
            }
            else if (_viewingSense)
            {
                _viewingSense = false;
            }
            else if (_viewingSettings)
            {
                _viewingSettings = false;
                ToggleSelectedButtons(_optionsMenu);
            }
            else if (_viewingMacros)
            {
                _viewingMacros = false;
                ToggleSelectedButtons(_optionsMenu);
            }
            else if (_viewingFriends)
            {
                _viewingFriends = false;
                ToggleSelectedButtons(_optionsMenu);
            }
            else if (_viewingOptions)
            {
                _viewingOptions = false;
                ToggleSelectedButtons(_optionsMenu);
                ToggleSelectedButtons(_menu);
            }
            else if (_composingPost)
            {
                _composePostMenu._buttons["cancelComposedBtn"].OnPress();
            }
            else if (_viewingPost)
            {
                _viewPostMenu._buttons["upViewBtn"].OnPress();
            }
            else if (_viewingMailList)
            {
                _viewingMailList = false;
            }
            else if (_viewingBoardList)
            {
                _viewingBoardList = false;
            }
            else if (_viewingPersonalBoard)
            {
                _viewingPersonalBoard = false;
                ToggleSelectedButtons(_boardMenu);
                ToggleSelectedButtons(_menu);
            }
            else if (_viewingUsers)
            {
                _viewingUsers = false;
                ToggleSelectedButtons(_usersMenu);
                ToggleSelectedButtons(_menu);
            }
            else if (_showGroupRequestPrompt)
            {
                _showGroupRequestPrompt = false;
            }
            else if (_viewingGroupList)
            {
                _viewingGroupList = false;
            }
            else if (_viewingOthersLegend)
            {
                ResetOthersLegendUI();
            }
            else if (_viewingOthersProfile)
            {
                ResetOthersProfileUI();
            }
            else if (_viewingLegend)
            {
                ResetLegendUI();
            }
            else if (_viewingProfile)
            {
                ResetProfileUI();
            }
            else if (_tabMap._display)
            {
                _tabMap._display = false;
            }
        }
    }

    /// <summary>True if <paramref name="point"/> is inside <paramref name="rect"/>.</summary>
    private bool CollidesWith(Rectangle rect, Engine.Point point)
    {
        return point.X >= (float)rect.X && point.X <= (float)(rect.X + rect.Width) && point.Y >= (float)rect.Y && point.Y <= (float)(rect.Y + rect.Height);
    }

    /// <summary>Draws a yellow debug outline (line loop) around <paramref name="bounds"/>.</summary>
    private void Render_Debug(Rectangle bounds)
    {
        Gl.glDisable(Gl.GL_TEXTURE_2D);
        Gl.glBegin(Gl.GL_LINE_LOOP);
        Gl.glColor3f(1f, 1f, 0f);
        Gl.glVertex2i(bounds.Left, bounds.Top);
        Gl.glVertex2i(bounds.Right, bounds.Top);
        Gl.glVertex2i(bounds.Right, bounds.Bottom);
        Gl.glVertex2i(bounds.Left, bounds.Bottom);
        Gl.glEnd();
        Gl.glEnable(3553);
    }

    /// <summary>Animates a tile's "alternate door" walls: toggles walkability on portal tiles and swaps the left/right wall textures to their alt-door frame.</summary>
    private void UpdateAltDoors(double elapsedTime, Tile t, bool walkable = false)
    {
        if (t._wall == null)
        {
            return;
        }
        if (_map.tileHasPortal(t.Location.X, t.Location.Y) != null)
        {
            t._walkable = walkable;
        }
        int altDoorTex = _textureManager.AltDoor(t._wall._rwall);
        if (altDoorTex > 0)
        {
            t._wall._rwall = altDoorTex;
            t._wall._rightWall.Texture = _textureManager.Get("stc" + altDoorTex.ToString("00000") + ".hpf_F0_C0", ".hpf");
        }
        altDoorTex = _textureManager.AltDoor(t._wall._lwall);
        if (altDoorTex > 0)
        {
            t._wall._lwall = altDoorTex;
            t._wall._leftWall.Texture = _textureManager.Get("stc" + altDoorTex.ToString("00000") + ".hpf_F0_C0", ".hpf");
        }
        t._wall.SetPosition(t._position.X, t._position.Y);
    }

    /// <summary>Builds (and caches) a "blackScreen" texture — opaque black panels framing the play area, used for the blind effect.</summary>
    private Texture DrawBlackScreen()
    {
        Image image = new Bitmap(618, 310);
        Graphics graphics = Graphics.FromImage(image);
        graphics.FillRectangles(new SolidBrush(System.Drawing.Color.Black), new Rectangle[4]
        {
            new Rectangle(2, 2, 616, 153),
            new Rectangle(2, 153, 255, 65),
            new Rectangle(368, 153, 616, 65),
            new Rectangle(2, 218, 616, 153)
        });
        _textureManager.LoadTexture("blackScreen", image);
        return _textureManager.Get("blackScreen");
    }

    /// <summary>Fires the Assail skill if the player has it.</summary>
    private void Assail()
    {
        foreach (Skill skill in _skills)
        {
            if (skill._name == "Assail")
            {
                UseSkill(skill._slot);
                break;
            }
        }
    }

    /// <summary>Turns the player to face <paramref name="direction"/> (unless emoting), cancels any pathing, and saves.</summary>
    private void ChangeDirection(int direction)
    {
        if (!_player._body._emoting)
        {
            _pfa = null;
            if (_player._mBody != null)
            {
                _player._mBody.ChangeDirection(direction);
            }
            _player._body._direction = direction;
            _player._body.setDefault();
            SaveLocation();
            _movementKeysDelay = DateTime.UtcNow;
            origKeyboardMoveDelay = 150.0;
        }
    }

    /// <summary>
    ///     Loads the local player's save (players/&lt;name&gt;/&lt;name&gt;.txt JSON) into the live Player:
    ///     stats, equipment, inventory, skills/spells/actions, legend marks and quest flags; builds the bank
    ///     catalog from the items DB; then enters the player's map.
    /// </summary>
    public void InitializePlayer()
    {
        _initPlayer = false;
        string clientName = GameWindow._clientName;
        string path = "players\\" + clientName.ToLower() + "\\" + clientName.ToLower() + ".txt";
        using (StreamReader streamReader = new StreamReader(path))
        {
            pdata = JsonConvert.DeserializeObject<PlayerDataS>(streamReader.ReadToEnd());
        }
        _panelNum = 3;
        uint id = _player._id;
        _player = new Player(this, _textureManager, _font, clientName, new Location(pdata.x, pdata.y), _map, "", pdata.gender);
        _player.gs = this;
        _player._id = id;
        _player._body._hairType = pdata.hairstyle;
        _player._body._hairColor = pdata.haircolor;
        _player._body._bodyImgs["b"] = 1;
        _player._body._bodyImgs["a"] = 1;
        _player._body._bodyImgs["n"] = 1;
        _player._body._bodyImgs["h"] = _player._body._hairType;
        _player._body._bodyColors["h"] = _player._body._hairColor;
        _player._body._direction = pdata.direction;
        _player._lev = pdata.lev;
        _player._exp = pdata.exp;
        _player._tnl = pdata.next;
        _player._availstats = pdata.availstats;
        if (_player._availstats > 0)
        {
            _statMenu._buttons["strBtn"].Enabled = true;
            _statMenu._buttons["intBtn"].Enabled = true;
            _statMenu._buttons["wisBtn"].Enabled = true;
            _statMenu._buttons["conBtn"].Enabled = true;
            _statMenu._buttons["dexBtn"].Enabled = true;
        }
        _player._str = pdata.STR;
        _player._int = pdata.INT;
        _player._wis = pdata.WIS;
        _player._con = pdata.CON;
        _player._dex = pdata.DEX;
        _player._baseHP = pdata.basehp;
        _player._baseMP = pdata.basemp;
        _player._curHP = pdata.curhp;
        _player._curMP = pdata.curmp;
        _player._ac = pdata.ac;
        _player._master = pdata.master;
        _player._guild = pdata.guild;
        _player._rank = pdata.rank;
        _player._title = pdata.title;
        _player._nation = pdata.national;
        _player._mapNum = pdata.mapnum;
        foreach (QuestS questEntry in pdata.quest)
        {
            questvars.Add(questEntry.id, questEntry.value);
        }
        foreach (InventoryS invEntry in pdata.inv)
        {
            NewItem(invEntry.name, invEntry.slot, invEntry.stack, invEntry.dura, invEntry.color, invEntry.enchantment, save: false);
        }
        foreach (EquipS equipEntry in pdata.equip)
        {
            NewEquip(equipEntry.name, equipEntry.tab, equipEntry.dura, equipEntry.color, null, equipEntry.slot, equipEntry.enchantment, save: false);
        }
        foreach (LegendS legendEntry in pdata.legend)
        {
            NewLegendMark(legendEntry.icon, legendEntry.type, legendEntry.text, legendEntry.color, save: false);
        }
        foreach (SkillS skill in pdata.skills)
        {
            NewSkill(skill.name, skill.slot, skill.level, save: false);
        }
        foreach (SpellS spell in pdata.spells)
        {
            NewSpell(spell.name, spell.slot, spell.level, save: false);
        }
        foreach (ActionS action in pdata.actions)
        {
            NewAction(action.name, action.slot, action.level, save: false);
        }
        foreach (JToken itemDef in _itemsDB["items"].Children())
        {
            string itemName = itemDef.Value<string>("name");
            if (!(itemName == "Gold"))
            {
                string tab = itemDef.Value<string>("tab");
                int imageFolder = itemDef.Value<int>("imagefolder");
                int frame = itemDef.Value<int>("frame");
                string imageType = itemDef.Value<string>("imagetype");
                if (string.IsNullOrEmpty(imageType))
                {
                    imageType = "old";
                }
                int bodyImgColor = itemDef.Value<int>("bodyimgcolor");
                BankItem bankItem = new BankItem(itemName, tab, 30, imageType switch
                {
                    "new" => _textureManager.Get("item" + imageFolder.ToString("000") + "_F" + (frame - 1) + "_new_C" + bodyImgColor, ".epf", imageType),
                    "myda" => _textureManager.Get("item" + imageFolder.ToString("000") + "_F" + (frame - 1) + "_myda_C" + bodyImgColor, ".epf", imageType),
                    "custom" => _textureManager.Get("item" + imageFolder.ToString("000") + "_F" + frame + "_custom_C" + bodyImgColor, ".epf", imageType),
                    _ => _textureManager.Get("item" + imageFolder.ToString("000") + "_F" + (frame - 1) + "_C" + bodyImgColor, ".epf", imageType),
                });
                bankItem._maxAmount = itemDef.Value<int>("stack");
                bankItem._value = itemDef.Value<int>("value");
                bankItem._description = itemDef.Value<string>("desc");
                bankItem._maxDurability = itemDef.Value<int>("dura");
                bankItem._dyeable = itemDef.Value<int>("dyeable");
                JToken statsNode = itemDef["stats"];
                if (statsNode != null)
                {
                    bankItem._level = statsNode.Value<string>("lev");
                    bankItem._gender = statsNode.Value<string>("gender");
                    bankItem._weaponDmg = statsNode.Value<string>("w");
                    bankItem._atk = statsNode.Value<string>("atk");
                    bankItem._def = statsNode.Value<string>("def");
                    bankItem._hp = statsNode.Value<int>("hp");
                    bankItem._mp = statsNode.Value<int>("mp");
                    bankItem._str = statsNode.Value<int>("str");
                    bankItem._int = statsNode.Value<int>("int");
                    bankItem._wis = statsNode.Value<int>("wis");
                    bankItem._con = statsNode.Value<int>("con");
                    bankItem._dex = statsNode.Value<int>("dex");
                    bankItem._mr = statsNode.Value<int>("mr");
                    bankItem._ac = statsNode.Value<int>("ac");
                    bankItem._dmg = statsNode.Value<int>("dmg");
                    bankItem._hit = statsNode.Value<int>("hit");
                    bankItem._reg = statsNode.Value<int>("reg");
                }
                _playerbank.Add(itemName, bankItem);
            }
        }
        if (GameWindow.ConnectedToServer)
        {
            SendProfileData();
            Console.WriteLine("Logged in as " + _player._name);
        }
        NewMap(_player._mapNum);
        SystemMsg("Type \"/commands\" to see a list of slash commands.", 3);
        SystemMsg("Press \"F3\" Key to open the world map anywhere.", 3);
        SystemMsg("Press \"F1\" Key to learn skills and withdraw items.", 3);
    }

    /// <summary>
    ///     Logs out: sends the logout packet, resets the map/player to empty, and clears the on-screen
    ///     name/location/map/info/chat labels.
    /// </summary>
    private void LogOut()
    {
        if (GameWindow.ConnectedToServer)
        {
            LogoutPacket logoutPacket = new LogoutPacket();
            GameWindow.ClientSocket.Send(logoutPacket.Data);
        }
        _map = new Map();
        _player = new Player(this, _textureManager, _font, "", new Location(0, 0), _map, "", 0);
        NewItem("Gold", 72, 0);
        _tabMap._display = false;
        _miscMenu._labels["nameLabel"].ChangeText("");
        _miscMenu._labels["locationLabel"].ChangeText("XY: 0, 0");
        _miscMenu._labels["mapLabel"].ChangeText("");
        _miscMenu._labels["infoLabel"].ChangeText("");
        _miscMenu._labels["userMsgLabel"].ChangeText("");
        _miscMenu._textFields["chatTF"]._textObj.RemoveCursor();
        _chatMode = false;
        _miscMenu._textFields["chatTF"].Text = "";
        _miscMenu._textFields["chatTF"]._textObjHighlighted.ChangeText("");
        _miscMenu._labels["chatLabel"].ChangeText("");
        _chatIndex = 0;
        _chatList.Clear();
        _infoBarIndex = 0;
        _infoBarList.Clear();
        _chatMenu._labels["chatPanel"].ChangeText("");
        _infoMenu._labels["infoPanel"].ChangeText("");
        _miscMenu._buttons["fullInvBtn"].Selected = false;
        _inventoryMenu._background.SetPosition(96.0, 336.0);
        GroupList.Clear();
        _panelNum = 3;
        ToggleSelectedButtons(_panelMenu);
        _panelMenu._buttons["chatBtn"].Selected = true;
        ResetProfileUI();
        ResetLegendUI();
        ResetOthersProfileUI();
        ResetOthersLegendUI();
        _viewingSense = false;
        _viewingSign = false;
        _viewingDialog = false;
        CloseMenus();
        Player.List.Clear();
        _playerbankedgold = 0;
        _playerbank.Clear();
        _maps.Clear();
        foreach (Slot equipmentSlot in _equipmentSlots)
        {
            equipmentSlot._item = null;
        }
        foreach (Slot inventorySlot in _inventorySlots)
        {
            inventorySlot._item = null;
        }
        foreach (Slot slot in _slots)
        {
            slot._item = null;
        }
        _equipment.Clear();
        _inventory.Clear();
        _skills.Clear();
        _spells.Clear();
        _statMenu._buttons["strBtn"].Enabled = false;
        _statMenu._buttons["intBtn"].Enabled = false;
        _statMenu._buttons["wisBtn"].Enabled = false;
        _statMenu._buttons["conBtn"].Enabled = false;
        _statMenu._buttons["dexBtn"].Enabled = false;
        _actions.Clear();
        questvars.Clear();
        Console.WriteLine("Logged off " + GameWindow._clientName);
        _system.ChangeState("start_menu");
    }

    #region Profile / legend panel reset

    /// <summary>Hides the profile panel and clears its labels.</summary>
    private void ResetProfileUI()
    {
        _viewingProfile = false;
        _profileMenu._buttons["legendProfileMenuBtn"].Selected = false;
        foreach (Text label in _profileMenu._labels.Values)
        {
            label.ChangeText("");
        }
    }

    /// <summary>Hides the legend panel and clears its labels.</summary>
    private void ResetLegendUI()
    {
        _profileMenu._buttons["legendProfileMenuBtn"].Selected = false;
        _viewingLegend = false;
        foreach (Text label in _legendMenu._labels.Values)
        {
            label.ChangeText("");
        }
    }

    /// <summary>Hides another player's profile panel and clears its labels.</summary>
    private void ResetOthersProfileUI()
    {
        _profilePlayer = null;
        _viewingOthersProfile = false;
        _othersProfileMenu._buttons["otherslegendProfileMenuBtn"].Selected = false;
        foreach (Text label in _othersProfileMenu._labels.Values)
        {
            label.ChangeText("");
        }
    }

    /// <summary>Hides another player's legend panel and clears its labels.</summary>
    private void ResetOthersLegendUI()
    {
        _othersProfileMenu._buttons["otherslegendProfileMenuBtn"].Selected = false;
        _viewingOthersLegend = false;
        foreach (Text label in _othersLegendMenu._labels.Values)
        {
            label.ChangeText("");
        }
    }

    #endregion

    private void Hotkeys()
    {
        keyboardBodyText.Add(DrawLabel("{=eESC {=h= Close opened menus.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=eF1 {=h= Help Menu.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=eF3 {=h= World Map.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=eF4 {=h= Settings Menu.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=eF5 {=h= Refresh.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=eF11 {=h= Fullscreen Toggle.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=eF12 {=h= Screenshot.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=e` {=h= Unequip Weapon and Shield, {=e~ {=h= Unequip All Gear.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=e1 {=hto {=e0 {=hand {=e-+ {=h= Item/Skill/Spell/Action Hotkeys.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=eTab {=h= Object Map.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=eq {=h= Game Options.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=ew {=h= Board and Mailbox.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=ee {=h= User List.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=ey {=h= Group Management.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=ea {=h= Item Panel, {=eA {=h= Full Inventory Toggle, {=eaa {=h= Profile Window.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=es {=h= Skill Panel, {=eS {=h= Secondary Skill Panel.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=ed {=h= Spell Panel, {=eD {=h= Secondary Spell Panel.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=ef {=h= Chat Panel, {=eF {=h= System Info Panel.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=eg {=h= Stat Panel.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=eh {=h= Action Panel, {=eH {=h= Secondary Action Panel.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=ez {=hor {=eLeft Key {=h= Move West.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=ex {=hor {=eDown Key {=h= Move South.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=ec {=hor {=eUp Key {=h= Move North.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=ev {=hor {=eRight Key {=h= Move East.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=eb {=h= Loot item under/in front of you.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=eSpace {=h= Basic attack.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=ePage Up {=h= Zoom In Object Map.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=ePage Down {=h= Zoom Out Object Map.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=eEnter {=h= Toggle Chat Mode.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=e' {=h= Whisper Player (Type their name, then hit {=eEnter{=h).", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("(Type in {=c! {=hto whisper Group, {=c!! {=hfor Guild, {=c!!! {=hfor World.)", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=eRight Click {=h= Auto-Walk (Pathfind).", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        keyboardBodyText.Add(DrawLabel("{=eDouble Right Click {=h= Auto-Attack Entity.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
    }

    /// <summary>Populates the in-game help panel with the list of available slash commands.</summary>
    private void SlashCommands()
    {
        commandBodyText.Add(DrawLabel("/port <#> (x,y) - Port to a map number, xy optional.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        commandBodyText.Add(DrawLabel("/where <name> - Get a players map location.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        commandBodyText.Add(DrawLabel("/maps - Prints a list of map numbers/names to desktop.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        commandBodyText.Add(DrawLabel(" ", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        commandBodyText.Add(DrawLabel("/create item <name> (#) - Adds the item to inventory.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        commandBodyText.Add(DrawLabel("/create skill <name> - Adds the skill to skill list.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        commandBodyText.Add(DrawLabel("/create spell <name> - Adds the spell to spell list.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        commandBodyText.Add(DrawLabel("/create action <name> - Adds the action to action list.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        commandBodyText.Add(DrawLabel(" ", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        commandBodyText.Add(DrawLabel("/spawn npc (new) <#> - Spawn a NPC by image #.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        commandBodyText.Add(DrawLabel("/spawn monster (new) <name or #> - Spawn a Monster.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        commandBodyText.Add(DrawLabel("/spawn item <name> - Spawn item entity on ground.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        commandBodyText.Add(DrawLabel(" ", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        commandBodyText.Add(DrawLabel("/gm - Enable GM mode (for now just walk through walls).", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        commandBodyText.Add(DrawLabel("/gender - Switch gender.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        commandBodyText.Add(DrawLabel("/title <text> - Give yourself a title.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        commandBodyText.Add(DrawLabel("/reset - Reset to level 1 (and stats).", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        commandBodyText.Add(DrawLabel(" ", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        commandBodyText.Add(DrawLabel("/script <name> - Trigger a script by name.", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        commandBodyText.Add(DrawLabel("/spellani - Debug Spell Animations (Open options menu).", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
        commandBodyText.Add(DrawLabel("/m <#> - Debug Monster Forms (Open options menu).", Engine.Color.Gray2, 0.0, 0.0, 400, "left", 0, shade: false, 0.0, 0.0, true));
    }

    /// <summary>
    ///     Clears the Selected flag on every button in <paramref name="menu"/> except
    ///     <paramref name="swap"/>, then either toggles <paramref name="swap"/> (default) or forces it on.
    /// </summary>
    private void ToggleSelectedButtons(ButtonMenu menu, Engine.Button swap = null, bool toggle = true)
    {
        foreach (Engine.Button button in menu._buttons.Values)
        {
            if (button.Selected && button != swap)
            {
                button.Selected = false;
            }
        }
        if (swap == null)
        {
            return;
        }
        if (toggle)
        {
            swap.Selected = !swap.Selected;
        }
        else
        {
            swap.Selected = true;
        }
    }

    public Text DrawLabel(string text, Engine.Color color, double x, double y, int maxWidth, string align, int lines = 0, bool shade = false, double winPosX = 0.0, double winPosY = 0.0, bool colorize = false)
    {
        Text label = new Text(text, _font, maxWidth, lines);
        label._colorize = colorize;
        label._windowOffset.X = x;
        label._windowOffset.Y = y;
        label._shade = shade;
        label.SetColor(color);
        label.Align(align);
        label.SetPosition(x + winPosX, y + winPosY);
        return label;
    }

    /// <summary>Greys out all board-list labels (boardLabel1..22) and disables the view/delete buttons in <paramref name="menu"/>.</summary>
    private void ResetBoardLabelColors(ButtonMenu menu)
    {
        for (int i = 1; i <= 22; i++)
        {
            if (menu._buttons.ContainsKey("viewBtn"))
            {
                menu._buttons["viewBtn"].Enabled = false;
            }
            if (menu._buttons.ContainsKey("deleteBoardListBtn"))
            {
                menu._buttons["deleteBoardListBtn"].Enabled = false;
            }
            if (menu._labels.ContainsKey("boardLabel" + i))
            {
                menu._labels["boardLabel" + i].SetColor(Engine.Color.Gray2);
            }
        }
    }

    /// <summary>True if the mouse is over an open profile / other-player-profile panel.</summary>
    private bool CollidesWithProfiles(Engine.Point mp)
    {
        return (_viewingProfile && _profileMenu.CollidesWith(mp)) || (_viewingOthersProfile && _othersProfileMenu.CollidesWith(mp));
    }

    /// <summary>Counts down the info-bar message timer and clears the text when it elapses.</summary>
    private void ClearInfoBar(double elapsedTime)
    {
        if (_infoBarText != "")
        {
            _infoBarDelay -= elapsedTime * 1000.0;
            if (_infoBarDelay <= 0.0)
            {
                _infoBarDelay = _origInfoBarDelay;
                _infoBarText = "";
            }
        }
    }

    /// <summary>Builds the hover tooltip for an equipped item (name, stack, durability, level, weapon/stats, description) and shows it at the cursor.</summary>
    private void ItemToolTip(Equipment item, Engine.Point mp)
    {
        _tTT = item._displayName;
        if (item._maxAmount > 1)
        {
            if (item._name == "Gold")
            {
                _tTT = _tTT + " ( " + item._amount.ToString("#,0") + " ) ";
            }
            else
            {
                _tTT = _tTT + " [ " + item._amount.ToString("#,0") + " ] ";
            }
        }
        if (item._maxDurability > 0)
        {
            _tTT = _tTT + "\n" + item._maxDurability.ToString("#,0") + " / " + item._maxDurability.ToString("#,0");
            if (item._dyeable == 1)
            {
                _tTT = _tTT + "\nDye: " + getDye("0");
            }
            _tTT = _tTT + "\nLevel: " + item._level.ToString();
            if (!string.IsNullOrEmpty(item._weaponDmg))
            {
                _tTT = _tTT + "\nWeapon: " + item._weaponDmg;
            }
            if (!string.IsNullOrEmpty(item._atk))
            {
                _tTT = _tTT + "\nAttack: " + item._atk;
            }
            if (!string.IsNullOrEmpty(item._def))
            {
                _tTT = _tTT + "\nDefense: " + item._def;
            }
            if (item._hp != 0)
            {
                _tTT = _tTT + "\nHealth: " + item._hp.ToString("#,0");
            }
            if (item._mp != 0)
            {
                _tTT = _tTT + "\nMana: " + item._mp.ToString("#,0");
            }
            if (item._str != 0)
            {
                _tTT = _tTT + "\nSTR: " + item._str;
            }
            if (item._int != 0)
            {
                _tTT = _tTT + "\nINT: " + item._int;
            }
            if (item._wis != 0)
            {
                _tTT = _tTT + "\nWIS: " + item._wis;
            }
            if (item._con != 0)
            {
                _tTT = _tTT + "\nCON: " + item._con;
            }
            if (item._dex != 0)
            {
                _tTT = _tTT + "\nDEX: " + item._dex;
            }
            if (item._mr != 0)
            {
                _tTT = _tTT + "\nMR: " + item._mr;
            }
            if (item._reg != 0)
            {
                _tTT = _tTT + "\nRegen: " + item._reg;
            }
            if (item._ac != 0)
            {
                _tTT = _tTT + "\nAC: " + item._ac;
            }
            if (item._dmg != 0)
            {
                _tTT = _tTT + "\nDMG: " + item._dmg;
            }
            if (item._hit != 0)
            {
                _tTT = _tTT + "\nHIT: " + item._hit;
            }
        }
        if (!string.IsNullOrEmpty(item._description))
        {
            _tTT = _tTT + "\n - " + item._description;
        }
        _toolTipText.ChangeText(_tTT);
        UpdateToolTip(mp);
        _input.Mouse.SetCursorSelected();
    }

    /// <summary>Builds the hover tooltip for a bank item (name, stack, durability, level, weapon/stats, description) and shows it at the cursor.</summary>
    private void ItemToolTip(BankItem item, Engine.Point mp)
    {
        _tTT = item._displayName;
        if (item._maxAmount > 1)
        {
            if (item._name == "Gold")
            {
                _tTT = _tTT + " ( " + item._amount.ToString("#,0") + " ) ";
            }
            else
            {
                _tTT = _tTT + " [ " + item._amount.ToString("#,0") + " ] ";
            }
        }
        if (item._maxDurability > 0)
        {
            _tTT = _tTT + "\n" + item._maxDurability.ToString("#,0") + " / " + item._maxDurability.ToString("#,0");
            if (item._dyeable == 1)
            {
                _tTT = _tTT + "\nDye: " + getDye("0");
            }
            _tTT = _tTT + "\nLevel: " + item._level.ToString();
            if (!string.IsNullOrEmpty(item._weaponDmg))
            {
                _tTT = _tTT + "\nWeapon: " + item._weaponDmg;
            }
            if (!string.IsNullOrEmpty(item._atk))
            {
                _tTT = _tTT + "\nAttack: " + item._atk;
            }
            if (!string.IsNullOrEmpty(item._def))
            {
                _tTT = _tTT + "\nDefense: " + item._def;
            }
            if (item._hp != 0)
            {
                _tTT = _tTT + "\nHealth: " + item._hp.ToString("#,0");
            }
            if (item._mp != 0)
            {
                _tTT = _tTT + "\nMana: " + item._mp.ToString("#,0");
            }
            if (item._str != 0)
            {
                _tTT = _tTT + "\nSTR: " + item._str;
            }
            if (item._int != 0)
            {
                _tTT = _tTT + "\nINT: " + item._int;
            }
            if (item._wis != 0)
            {
                _tTT = _tTT + "\nWIS: " + item._wis;
            }
            if (item._con != 0)
            {
                _tTT = _tTT + "\nCON: " + item._con;
            }
            if (item._dex != 0)
            {
                _tTT = _tTT + "\nDEX: " + item._dex;
            }
            if (item._mr != 0)
            {
                _tTT = _tTT + "\nMR: " + item._mr;
            }
            if (item._reg != 0)
            {
                _tTT = _tTT + "\nRegen: " + item._reg;
            }
            if (item._ac != 0)
            {
                _tTT = _tTT + "\nAC: " + item._ac;
            }
            if (item._dmg != 0)
            {
                _tTT = _tTT + "\nDMG: " + item._dmg;
            }
            if (item._hit != 0)
            {
                _tTT = _tTT + "\nHIT: " + item._hit;
            }
        }
        if (!string.IsNullOrEmpty(item._description))
        {
            _tTT = _tTT + "\n - " + item._description;
        }
        _toolTipText.ChangeText(_tTT);
        UpdateToolTip(mp);
        _input.Mouse.SetCursorSelected();
    }

    /// <summary>Builds the hover tooltip for an inventory item (name, stack, durability, level, weapon/stats, description) and shows it at the cursor.</summary>
    private void ItemToolTip(InventoryItem item, Engine.Point mp)
    {
        _tTT = item._displayName;
        if (item._maxAmount > 1)
        {
            if (item._name == "Gold")
            {
                _tTT = _tTT + " ( " + item._amount.ToString("#,0") + " ) ";
            }
            else
            {
                _tTT = _tTT + " [ " + item._amount.ToString("#,0") + " ] ";
            }
        }
        if (item._maxDurability > 0)
        {
            _tTT = _tTT + "\n" + item._durability.ToString("#,0") + " / " + item._maxDurability.ToString("#,0");
            if (item._dyeable == 1)
            {
                _tTT = _tTT + "\nDye: " + getDye(item._bodyImgColor.ToString());
            }
            _tTT = _tTT + "\nLevel: " + item._level.ToString();
            if (!string.IsNullOrEmpty(item._weaponDmg))
            {
                _tTT = _tTT + "\nWeapon: " + item._weaponDmg;
            }
            if (!string.IsNullOrEmpty(item._atk))
            {
                _tTT = _tTT + "\nAttack: " + item._atk;
            }
            if (!string.IsNullOrEmpty(item._def))
            {
                _tTT = _tTT + "\nDefense: " + item._def;
            }
            if (item._hp != 0)
            {
                _tTT = _tTT + "\nHealth: " + item._hp.ToString("#,0");
            }
            if (item._mp != 0)
            {
                _tTT = _tTT + "\nMana: " + item._mp.ToString("#,0");
            }
            if (item._str != 0)
            {
                _tTT = _tTT + "\nSTR: " + item._str;
            }
            if (item._int != 0)
            {
                _tTT = _tTT + "\nINT: " + item._int;
            }
            if (item._wis != 0)
            {
                _tTT = _tTT + "\nWIS: " + item._wis;
            }
            if (item._con != 0)
            {
                _tTT = _tTT + "\nCON: " + item._con;
            }
            if (item._dex != 0)
            {
                _tTT = _tTT + "\nDEX: " + item._dex;
            }
            if (item._mr != 0)
            {
                _tTT = _tTT + "\nMR: " + item._mr;
            }
            if (item._reg != 0)
            {
                _tTT = _tTT + "\nRegen: " + item._reg;
            }
            if (item._ac != 0)
            {
                _tTT = _tTT + "\nAC: " + item._ac;
            }
            if (item._dmg != 0)
            {
                _tTT = _tTT + "\nDMG: " + item._dmg;
            }
            if (item._hit != 0)
            {
                _tTT = _tTT + "\nHIT: " + item._hit;
            }
        }
        if (!string.IsNullOrEmpty(item._description))
        {
            _tTT = _tTT + "\n - " + item._description;
        }
        _toolTipText.ChangeText(_tTT);
        UpdateToolTip(mp);
        _input.Mouse.SetCursorSelected();
    }

    /// <summary>Positions the item tooltip + its background near the cursor, sized to the text and clamped to the 640×480 screen.</summary>
    private void UpdateToolTip(Engine.Point mp)
    {
        string[] lines = _toolTipText._text.Split('\n');
        float width = (float)_toolTipText.Width + 10f;
        float height = 17 + (lines.Count() - 1) * 12;
        _toolTipBack.SetWidth(width);
        _toolTipBack.SetHeight(height);
        double posX = ((mp.X + width > 640f) ? (mp.X - 20f - (mp.X + width - 640f)) : mp.X);
        double posY = ((mp.Y + height > 480f) ? (mp.Y - 3f - (mp.Y + height - 480f)) : mp.Y);
        _toolTipText.SetPosition(posX + 20.0, posY + 3.0);
        _toolTipBack.SetPosition(posX + 15.0, posY);
    }

    /// <summary>Positions the entity (monster/NPC) tooltip + its background near the cursor, sized to the text and clamped to the screen.</summary>
    private void UpdateEntityToolTip(Engine.Point mp)
    {
        string[] lines = _entityToolTip._text.Split('\n');
        float width = 150f;
        float height = 17 + (lines.Count() - 1) * 12;
        _entityToolTipBack.SetWidth(width);
        _entityToolTipBack.SetHeight(height);
        double posX = ((mp.X + width > 640f) ? (mp.X - 20f - (mp.X + width - 640f)) : mp.X);
        double posY = ((mp.Y + height > 480f) ? (mp.Y - 3f - (mp.Y + height - 480f)) : mp.Y);
        _entityToolTip.SetPosition(posX + 20.0, posY + 3.0);
        _entityToolTipBack.SetPosition(posX + 15.0, posY);
    }

    /// <summary>
    ///     Updates the HP and MP orb sprites: computes each as a 0-100% of current/max (clamped), maps the
    ///     percent to one of 16 orb frames (0 = full ... 15 = empty), and applies the orb001/orb002 textures.
    /// </summary>
    private void UpdateHPMPOrbs()
    {
        int hpPct = 100;
        int mpPct = 100;
        if (_player._maxHP >= _player._curHP)
        {
            int hp = _player._curHP;
            int maxHp = _player._maxHP;
            if (_player._curHP <= 0)
            {
                hp = 1;
            }
            if (_player._maxHP <= 0)
            {
                maxHp = 1;
            }
            hpPct = (int)((float)hp / (float)maxHp * 100f);
        }
        if (_player._maxMP >= _player._curMP)
        {
            int mp = _player._curMP;
            int maxMp = _player._maxMP;
            if (_player._curMP <= 0)
            {
                mp = 1;
            }
            if (_player._maxMP <= 0)
            {
                maxMp = 1;
            }
            mpPct = (int)((float)mp / (float)maxMp * 100f);
        }
        int orbFrame = 0;
        if (hpPct < 100)
        {
            if (hpPct >= 99)
            {
                orbFrame = 0;
            }
            else if (hpPct >= 92)
            {
                orbFrame = 1;
            }
            else if (hpPct >= 85)
            {
                orbFrame = 2;
            }
            else if (hpPct >= 78)
            {
                orbFrame = 3;
            }
            else if (hpPct >= 71)
            {
                orbFrame = 4;
            }
            else if (hpPct >= 64)
            {
                orbFrame = 5;
            }
            else if (hpPct >= 57)
            {
                orbFrame = 6;
            }
            else if (hpPct >= 50)
            {
                orbFrame = 7;
            }
            else if (hpPct >= 43)
            {
                orbFrame = 8;
            }
            else if (hpPct >= 36)
            {
                orbFrame = 9;
            }
            else if (hpPct >= 29)
            {
                orbFrame = 10;
            }
            else if (hpPct >= 22)
            {
                orbFrame = 11;
            }
            else if (hpPct >= 15)
            {
                orbFrame = 12;
            }
            else if (hpPct >= 8)
            {
                orbFrame = 13;
            }
            else if (hpPct >= 1)
            {
                orbFrame = 14;
            }
            else if (hpPct < 1)
            {
                orbFrame = 15;
            }
        }
        _healthOrb.Texture = _textureManager.Get("orb001_F" + orbFrame + "_C0");
        orbFrame = 0;
        if (mpPct < 100)
        {
            if (mpPct >= 99)
            {
                orbFrame = 0;
            }
            else if (mpPct >= 92)
            {
                orbFrame = 1;
            }
            else if (mpPct >= 85)
            {
                orbFrame = 2;
            }
            else if (mpPct >= 78)
            {
                orbFrame = 3;
            }
            else if (mpPct >= 71)
            {
                orbFrame = 4;
            }
            else if (mpPct >= 64)
            {
                orbFrame = 5;
            }
            else if (mpPct >= 57)
            {
                orbFrame = 6;
            }
            else if (mpPct >= 50)
            {
                orbFrame = 7;
            }
            else if (mpPct >= 43)
            {
                orbFrame = 8;
            }
            else if (mpPct >= 36)
            {
                orbFrame = 9;
            }
            else if (mpPct >= 29)
            {
                orbFrame = 10;
            }
            else if (mpPct >= 22)
            {
                orbFrame = 11;
            }
            else if (mpPct >= 15)
            {
                orbFrame = 12;
            }
            else if (mpPct >= 8)
            {
                orbFrame = 13;
            }
            else if (mpPct >= 1)
            {
                orbFrame = 14;
            }
            else if (mpPct < 1)
            {
                orbFrame = 15;
            }
        }
        _manaOrb.Texture = _textureManager.Get("orb002_F" + orbFrame + "_C0");
    }

    /// <summary>Counts online users matching <paramref name="type"/> ("master" / "guild" / "friend").</summary>
    private int PlayerCount(string type)
    {
        int count = 0;
        foreach (UserS user in _userList)
        {
            if (type == "master" && user.IsMaster)
            {
                count++;
            }
            else if (type == "guild" && user.InGuild)
            {
                count++;
            }
            else if (type == "friend" && _friendList.Contains(user.Name, StringComparer.CurrentCultureIgnoreCase))
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>Maps a dye id string ("1".."35") to its display name; "Default" for unknown ids.</summary>
    private string getDye(string dyeId)
    {
        return dyeId switch
        {
            "1" => "Black",
            "2" => "Strawberry",
            "3" => "Auburn",
            "4" => "Gold",
            "5" => "Cyan",
            "6" => "Blueberry",
            "7" => "Violet",
            "8" => "Mustardseed",
            "9" => "Evergreen",
            "10" => "Copper",
            "11" => "Brown",
            "12" => "Grey",
            "13" => "Night",
            "14" => "Tan",
            "15" => "White",
            "16" => "Pink",
            "17" => "Light Green",
            "18" => "Orange",
            "19" => "Platinum",
            "20" => "Dark Blue",
            "21" => "Silver",
            "22" => "?",
            "23" => "??",
            "24" => "Rose Pink",
            "25" => "Royal Blue",
            "26" => "Blush",
            "27" => "Turqoise",
            "28" => "Purple",
            "29" => "Dark Orange",
            "30" => "Sky Blue",
            "31" => "Lime Green",
            "32" => "Jade Green",
            "33" => "Dirty Blonde",
            "34" => "Ocean Blue",
            "35" => "Light Brown",
            _ => "Default",
        };
    }

    #region Movement, tiles & portals

    /// <summary>
    ///     Advances the player's step animation and, on the final frame, commits the move: updates the
    ///     player's tile, toggles swimming on water, fires reactor/portal triggers, and scrolls every
    ///     tile / entity / projectile by the camera offset. Bails if the destination tile is unwalkable
    ///     (non-GM) or occupied by a visible blocker.
    /// </summary>
    private void Move(double elapsedTime)
    {
        if (_player._body._emoting || !_map._loaded)
        {
            return;
        }
        if (_pfa != null && _pfa._thePath.Count() > 0)
        {
            RightClickMovement();
        }
        Tile destTile = null;
        if (_player._location.X + _playerXOffset >= 0 && (double)(_player._location.X + _playerXOffset) < _map._width && _player._location.Y + _playerYOffset >= 0 && (double)(_player._location.Y + _playerYOffset) < _map._height)
        {
            int index = (_player._location.Y + _playerYOffset) * (int)_map._width + (_player._location.X + _playerXOffset);
            destTile = _map._tiles[index];
        }
        if (destTile == null)
        {
            _moving = false;
        }
        else
        {
            if (!destTile._walkable && !_GM)
            {
                _moving = false;
            }
            int blockerCount = 0;
            foreach (Entity occupant in destTile._entities.Values)
            {
                if (!(occupant is Item) && !occupant.Hidden)
                {
                    blockerCount++;
                }
            }
            if (blockerCount > 0)
            {
                _moving = false;
            }
        }
        if (_moving)
        {
            _player.AnimateMovement(_feetShuffleDelay);
            _moveAniDelay -= elapsedTime * 1000.0;
            if (!(_moveAniDelay <= 0.0))
            {
                return;
            }
            _moveAniDelay = _origMoveAniDelay;
            if (_moveAniCount >= moveFrames)
            {
                _moveAniCount = -1;
            }
            if (_moveAniCount == -1)
            {
                _moveAniCount++;
                _player._location.X += _playerXOffset;
                _player._location.Y += _playerYOffset;
                SaveLocation();
                _player._tile._entities.Remove(_player._id);
                _player._tile = _map._tiles[_player._location.Y * (int)_map._width + _player._location.X];
                _player._tile._entities.Add(_player._id, _player);
                _player._tileTime = DateTime.UtcNow;
                if (_player._tile._water && !_player._body._swimming && !_player.Hidden)
                {
                    _player._body._swimming = true;
                    SendDisplayPlayer();
                }
                else if (_player._body._swimming && !_player._tile._water)
                {
                    _player._body._swimming = false;
                    SendDisplayPlayer();
                }
                _moving = false;
                _player.RepositionTargetBox();
                StepOnAReactor(0, 0, 0);
                StepOnAPortal();
                _map.useOffs = false;
                return;
            }
            foreach (Projectile projectile in _map._projectiles)
            {
                projectile._position.X += _xOffset;
                projectile._position.Y += _yOffset;
            }
            Tile[] tiles = _map._tiles.ToArray();
            foreach (Tile tile in tiles)
            {
                tile._position.X += _xOffset;
                tile._position.Y += _yOffset;
                Entity[] entities = tile._entities.Values.ToArray();
                foreach (Entity entity in entities)
                {
                    if (entity._id != _player._id)
                    {
                        entity._targetBox.X += (int)_xOffset;
                        entity._targetBox.Y += (int)_yOffset;
                        entity._hpBarSprite.SetPosition(entity._targetBox.X + 1, entity._targetBox.Y - 5);
                        entity._nameTag.SetPosition(entity._nameTag._position.X + _xOffset, entity._nameTag._position.Y + _yOffset);
                        if (entity._mBody != null)
                        {
                            entity._mBody._position.X += _xOffset;
                            entity._mBody._position.Y += _yOffset;
                            entity._mBody._sprite.SetPosition(entity._mBody._position.X, entity._mBody._position.Y);
                            entity._mBody._spellAni.SetPosition(entity._mBody._spellAni.GetPosition().X + _xOffset, entity._mBody._spellAni.GetPosition().Y + _yOffset);
                        }
                        else if (entity is Player)
                        {
                            entity.SetPosition(new Vector(tile._position.X + tile._width / 2.0 - 28.0 + 1.0, tile._position.Y + tile._height - 85.0 + 1.0, 0.0));
                        }
                        else
                        {
                            entity.SetPosition(tile._position.X + tile._width / 2.0 - Math.Abs(entity._sprite.GetWidth()) / 2.0, tile._position.Y + tile._height - entity._sprite.GetHeight());
                        }
                    }
                }
            }
            _moveAniCount++;
        }
        else
        {
            _moveAniCount = 0;
        }
    }

    /// <summary>
    ///     Consumes the next tile on the right-click pathfinding path (<c>_pfa</c>): turns the player to
    ///     face it (for one tick), then sets the camera/player step offsets to walk onto it.
    /// </summary>
    private void RightClickMovement()
    {
        if (_moving)
        {
            return;
        }
        Tile tile = _pfa._thePath.Last();
        if (tile.Location.Y == _player._location.Y + 1)
        {
            if (_player._body._direction != 2)
            {
                if (_player._mBody != null)
                {
                    _player._mBody.ChangeDirection(2);
                }
                _player._body._direction = 2;
                _player._body.setDefault();
                SaveLocation();
                _movementKeysDelay = DateTime.UtcNow;
                origKeyboardMoveDelay = 150.0;
            }
            if (_player._body._direction == 2)
            {
                _xOffset = 14.0;
                _yOffset = -7.0;
                _playerXOffset = 0;
                _playerYOffset = 1;
                _moving = true;
                _map.useOffs = true;
                _map.xOffset = _xOffset;
                _map.yOffset = _yOffset;
                _movementKeysDelay = DateTime.UtcNow;
                origKeyboardMoveDelay = 50.0;
            }
        }
        else if (tile.Location.Y == _player._location.Y - 1)
        {
            if (_player._body._direction != 0)
            {
                if (_player._mBody != null)
                {
                    _player._mBody.ChangeDirection(0);
                }
                _player._body._direction = 0;
                _player._body.setDefault();
                SaveLocation();
                _movementKeysDelay = DateTime.UtcNow;
                origKeyboardMoveDelay = 150.0;
            }
            if (_player._body._direction == 0)
            {
                _xOffset = -14.0;
                _yOffset = 7.0;
                _playerXOffset = 0;
                _playerYOffset = -1;
                _moving = true;
                _map.useOffs = true;
                _map.xOffset = _xOffset;
                _map.yOffset = _yOffset;
                _movementKeysDelay = DateTime.UtcNow;
                origKeyboardMoveDelay = 50.0;
            }
        }
        else if (tile.Location.X == _player._location.X + 1)
        {
            if (_player._body._direction != 1)
            {
                if (_player._mBody != null)
                {
                    _player._mBody.ChangeDirection(1);
                }
                _player._body._direction = 1;
                _player._body.setDefault();
                SaveLocation();
                _movementKeysDelay = DateTime.UtcNow;
                origKeyboardMoveDelay = 150.0;
            }
            if (_player._body._direction == 1)
            {
                _xOffset = -14.0;
                _yOffset = -7.0;
                _playerXOffset = 1;
                _playerYOffset = 0;
                _moving = true;
                _map.useOffs = true;
                _map.xOffset = _xOffset;
                _map.yOffset = _yOffset;
                _movementKeysDelay = DateTime.UtcNow;
                origKeyboardMoveDelay = 50.0;
            }
        }
        else if (tile.Location.X == _player._location.X - 1)
        {
            if (_player._body._direction != 3)
            {
                if (_player._mBody != null)
                {
                    _player._mBody.ChangeDirection(3);
                }
                _player._body._direction = 3;
                _player._body.setDefault();
                SaveLocation();
                _movementKeysDelay = DateTime.UtcNow;
                origKeyboardMoveDelay = 150.0;
            }
            if (_player._body._direction == 3)
            {
                _xOffset = 14.0;
                _yOffset = 7.0;
                _playerXOffset = -1;
                _playerYOffset = 0;
                _moving = true;
                _map.useOffs = true;
                _map.xOffset = _xOffset;
                _map.yOffset = _yOffset;
                _movementKeysDelay = DateTime.UtcNow;
                origKeyboardMoveDelay = 50.0;
            }
        }
        _pfa._thePath.Remove(tile);
    }

    /// <summary>Isometric X pixel offset for tile (x, y) (half-tile width 28).</summary>
    private int toTileX(int x, int y)
    {
        return x * 28 + y * 28 * -1;
    }

    /// <summary>Isometric Y pixel offset for tile (x, y) (half-tile height 14).</summary>
    private int toTileY(int x, int y)
    {
        return x * 14 + y * 14;
    }

    /// <summary>The on-screen tile containing pixel (x, y) within <paramref name="range"/> of the player, or null.</summary>
    private Tile getXYTile(int x, int y, int range = 3)
    {
        foreach (Tile tile in _map._tiles)
        {
            if (tile.CollidesWith(x, y) && tile.Location.WithinRange(_player._location, range))
            {
                return tile;
            }
        }
        return null;
    }

    /// <summary>The tile directly in front of <paramref name="entity"/> (defaults to the player), or null if off-map.</summary>
    private Tile TileImFacing(Entity entity = null)
    {
        if (entity == null)
        {
            entity = _player;
        }
        Tile facingTile = null;
        Location location = new Location(0, 0);
        if (entity._direction == 0)
        {
            location = new Location(entity._location.X, entity._location.Y - 1);
        }
        else if (entity._direction == 1)
        {
            location = new Location(entity._location.X + 1, entity._location.Y);
        }
        else if (entity._direction == 2)
        {
            location = new Location(entity._location.X, entity._location.Y + 1);
        }
        else if (entity._direction == 3)
        {
            location = new Location(entity._location.X - 1, entity._location.Y);
        }
        if (location.X < (int)_map._width && location.Y < (int)_map._height && location.X >= 0 && location.Y >= 0)
        {
            facingTile = _map._tiles[location.Y * (int)_map._width + location.X];
        }
        return facingTile;
    }

    /// <summary>The tile directly behind the player (opposite their facing), or null if off-map.</summary>
    private Tile TileBehindMe()
    {
        Tile behindTile = null;
        Location location = new Location(0, 0);
        if (_player._body._direction == 0)
        {
            location = new Location(_player._location.X, _player._location.Y + 1);
        }
        else if (_player._body._direction == 1)
        {
            location = new Location(_player._location.X - 1, _player._location.Y);
        }
        else if (_player._body._direction == 2)
        {
            location = new Location(_player._location.X, _player._location.Y - 1);
        }
        else if (_player._body._direction == 3)
        {
            location = new Location(_player._location.X + 1, _player._location.Y);
        }
        if (location.X < (int)_map._width && location.Y < (int)_map._height && location.X >= 0 && location.Y >= 0)
        {
            behindTile = _map._tiles[location.Y * (int)_map._width + location.X];
        }
        return behindTile;
    }

    /// <summary>
    ///     If the player (or the given x,y) stands on a reactor of <paramref name="_type"/>, fires its
    ///     trigger: 0 = open a dialog, 1 = spawn a monster. (Types 2/3/4+ have no client-side effect.)
    /// </summary>
    private void StepOnAReactor(byte _type = 0, byte x = 0, byte y = 0)
    {
        if (x == 0)
        {
            x = (byte)_player._location.X;
        }
        if (y == 0)
        {
            y = (byte)_player._location.Y;
        }
        Reactor reactor = _map.tileHasReactor(x, y);
        if (reactor != null && reactor._type == _type)
        {
            if (reactor._triggerType == 0)
            {
                DialogPopup(null, reactor._monType, 0);
            }
            else if (reactor._triggerType == 1)
            {
                SpawnMonster(reactor._monType, reactor._location, "old", 1);
            }
        }
    }

    /// <summary>If the player stands on a portal, transfers to its destination map (revealing the world map for map 1001).</summary>
    private void StepOnAPortal()
    {
        Portal portal = _map.tileHasPortal(_player._location.X, _player._location.Y);
        if (portal != null)
        {
            if (portal._toMap == 1001 && !_worldMap)
            {
                _worldMap = true;
                AddTowns();
            }
            NewMap(portal._toMap, portal._toX, portal._toY);
        }
    }

    #endregion

    #region Stats & leveling

    /// <summary>Spends one available stat point on the chosen stat (0=Str, 1=Int, 2=Wis, 3=Con, 4=Dex; Wis/Con also raise MP/HP), then persists it.</summary>
    private void AddStat(byte stat)
    {
        if (_player._availstats > 0)
        {
            switch (stat)
            {
                case 0:
                    _player._str++;
                    break;
                case 1:
                    _player._int++;
                    break;
                case 2:
                    _player._wis++;
                    _player._baseMP += 50;
                    SaveStat(7, 50);
                    break;
                case 3:
                    _player._con++;
                    _player._baseHP += 50;
                    SaveStat(6, 50);
                    break;
                case 4:
                    _player._dex++;
                    break;
            }
            _player._availstats--;
            if (_player._availstats == 0)
            {
                _statMenu._buttons["strBtn"].Enabled = false;
                _statMenu._buttons["intBtn"].Enabled = false;
                _statMenu._buttons["wisBtn"].Enabled = false;
                _statMenu._buttons["conBtn"].Enabled = false;
                _statMenu._buttons["dexBtn"].Enabled = false;
            }
            SaveStat(stat);
        }
    }

    /// <summary>
    ///     Raises the player's level: +2 stat points, +51 base HP and MP (both refilled to max), +1 AC
    ///     every third level, plays the level-up animation, and re-enables the stat-allocation buttons.
    /// </summary>
    private void LevelUp()
    {
        _player._lev++;
        SaveLevel();
        _player._availstats += 2;
        SaveStat(5);
        int hpGain = 51;
        SaveStat(6, hpGain);
        _player._baseHP += hpGain;
        _player._curHP = _player._maxHP;
        int mpGain = 51;
        SaveStat(7, mpGain);
        _player._baseMP += mpGain;
        _player._curMP = _player._maxMP;
        if (_player._lev % 3 == 0)
        {
            _player._ac++;
            SaveStat(8);
        }
        SystemMsg("Your level has increased!", 3);
        SpellAnimation(_player, 79, 200);
        _statMenu._buttons["strBtn"].Enabled = true;
        _statMenu._buttons["intBtn"].Enabled = true;
        _statMenu._buttons["wisBtn"].Enabled = true;
        _statMenu._buttons["conBtn"].Enabled = true;
        _statMenu._buttons["dexBtn"].Enabled = true;
    }

    /// <summary>
    ///     Awards experience, then levels the player up (possibly several times, up to 99) for as long as
    ///     the gained <paramref name="exp"/> covers the to-next-level thresholds, carrying the remainder.
    /// </summary>
    public void GainExp(uint exp)
    {
        SystemMsg(exp.ToString("#,0") + " experience!", 3);
        _player._exp += exp;
        if (_player._lev == 99)
        {
            _player._tnl = 0u;
        }
        if (_player._tnl != 0)
        {
            if ((int)(_player._tnl - exp) <= 0)
            {
                uint cumulativeTnl = _player._tnl;
                for (int level = _player._lev + 1; level <= 99; level++)
                {
                    LevelUp();
                    cumulativeTnl += _player.TNIS[level];
                    if (cumulativeTnl >= exp)
                    {
                        _player._tnl = cumulativeTnl - exp;
                        break;
                    }
                }
            }
            else
            {
                _player._tnl -= exp;
            }
        }
        if (_player._lev == 99 && _player._tnl != 0)
        {
            _player._tnl = 0u;
        }
        SaveExp();
    }

    #endregion

    /// <summary>
    ///     Loads (or revisits) map <paramref name="number"/> at (tox, toy): reads its size from the
    ///     maps DB, builds it once (spawning NPCs/monsters) or re-lays its tiles/entities on return,
    ///     moves the player onto it, updates swim state, plays a random track, and saves.
    /// </summary>
    private void NewMap(int number, int tox = -1, int toy = -1)
    {
        double mapWidth = 0.0;
        double mapHeight = 0.0;
        bool found = false;
        foreach (JToken mapEntry in _mapsDB["maps"].Children())
        {
            if (mapEntry.Value<int>("mapnum") == number)
            {
                found = true;
                mapWidth = mapEntry.Value<double>("width");
                mapHeight = mapEntry.Value<double>("height");
            }
        }
        if (!found)
        {
            SystemMsg("Map " + number + " does not exist in maps database.", 3);
            return;
        }
        MAPFile mapFile = null;
        try
        {
            mapFile = MAPFile.FromFile("maps\\lod" + number + ".map", (int)mapWidth, (int)mapHeight);
        }
        catch
        {
            SystemMsg("Map file 'lod" + number + ".map' does not exist in maps folder.", 3);
            return;
        }
        _pfa = null;
        _map._loaded = false;
        _playerXOffset = 0;
        _playerYOffset = 0;
        _yOffset = 0.0;
        _xOffset = 0.0;
        if (_player._tile != null && _player._tile._entities.ContainsKey(_player._id))
        {
            _player._tile._entities.Remove(_player._id);
        }
        if (tox == -1)
        {
            tox = _player._location.X;
            toy = _player._location.Y;
        }
        _player._location.X = tox;
        _player._location.Y = toy;
        if (!_maps.ContainsKey(number))
        {
            _map = new Map(_itemsDB, _font, _input, this, _textureManager, _player, number, _miscMenu, _mapsDB, wallaniarr, mapFile);
            _player._tile = _map._tiles[_player._location.Y * (int)_map._width + _player._location.X];
            _maps.Add(number, _map);
            SpawnNpcs(number);
            SpawnMonsters(number);
        }
        else
        {
            _map = _maps[number];
            _player._tile = _map._tiles[_player._location.Y * (int)mapWidth + _player._location.X];
            double originX = 283 - _map._toTileX(_player._location.X, _player._location.Y);
            double originY = 181 - _map._toTileY(_player._location.X, _player._location.Y);
            for (int row = 0; (double)row < mapHeight; row++)
            {
                for (int col = 0; (double)col < mapWidth; col++)
                {
                    Tile tile = _map._tiles[row * (int)mapWidth + col];
                    tile.SetPosition(originX + (double)(col * 56 / 2), originY + (double)(col * 28 / 2));
                    Entity[] entities = tile._entities.Values.ToArray();
                    foreach (Entity entity in entities)
                    {
                        if (entity._id != _player._id)
                        {
                            if (entity._mBody != null)
                            {
                                entity._mBody.ResetAction();
                                entity._mBody.SetDefault();
                            }
                            if (entity is Item)
                            {
                                entity.SetPosition(new Vector(tile._position.X + tile._width / 2.0 - Math.Abs(entity._sprite.GetWidth()) / 2.0 + 1.0, tile._position.Y + tile._height - entity._sprite.GetHeight() + 1.0, 0.0));
                            }
                            else if (entity is Monster || entity is NPC)
                            {
                                entity.SetPosition(new Vector(tile._position.X + tile._width / 2.0 - Math.Abs(entity._mBody._sprite.GetWidth()) / 2.0 + 1.0, tile._position.Y + tile._height - entity._mBody._sprite.GetHeight() + 1.0, 0.0));
                            }
                            else if (entity is Player)
                            {
                                entity.SetPosition(new Vector(tile._position.X + tile._width / 2.0 - 28.0 + 1.0, tile._position.Y + tile._height - 85.0 + 1.0, 0.0));
                            }
                        }
                    }
                }
                originX -= 28.0;
                originY += 14.0;
            }
            _map._loaded = true;
        }
        PlayMusicFile("music\\" + rand.Next(1, 27) + ".ogg");
        SaveMap();
        SaveLocation(ignore: true);
        _tabMap._player = _player;
        _tabMap._map = _map;
        foreach (IWO iwo in _map._iwos)
        {
            iwo.move();
        }
        _player._map = _map;
        _player._mapNum = _map._number;
        _player._mapName = _map._name;
        if (_player._tile._water && !_player._body._swimming)
        {
            _player._body._swimming = true;
        }
        else if (_player._body._swimming && !_player._tile._water)
        {
            _player._body._swimming = false;
        }
        _player._body._walking = false;
        _player._body.setDefault();
        _player.RepositionTargetBox();
        if (_player._mBody != null)
        {
            _player._mBody.flipSprites();
        }
        if (_player._body._position.Y != 124.0)
        {
            _player._body.SetPosition(283.0, 124.0);
        }
        _player._tile._entities.Add(_player._id, _player);
        _player._tileTime = DateTime.UtcNow;
        _player._chatText.ChangeText("");
        _player._cbDelay = 0.0;
        SendDisplayPlayer();
    }

    #region Map / town transitions

    /// <summary>Re-enters the current map at (x, y) — repositions the player without changing maps.</summary>
    private void Teleport(int x, int y)
    {
        _pfa = null;
        NewMap(_player._mapNum, x, y);
        StepOnAReactor(0, 0, 0);
    }

    /// <summary>Registers every world-map town marker (name, map number, clickable rect) for the world map screen.</summary>
    private void AddTowns()
    {
        new Town("Abel", 3014, new Rectangle(299, 86, 12, 12), _font);
        new Town("Mileth", 3006, new Rectangle(255, 101, 12, 12), _font);
        new Town("East Woodland", 600, new Rectangle(219, 118, 12, 12), _font);
        new Town("Pravat Cave", 3052, new Rectangle(299, 138, 12, 12), _font);
        new Town("Kasmanium Mine", 660, new Rectangle(228, 153, 12, 12), _font);
        new Town("Piet", 3020, new Rectangle(297, 196, 12, 12), _font);
        new Town("Mehadi Swamp", 3071, new Rectangle(369, 208, 12, 12), _font);
        new Town("Grass Field", 701, new Rectangle(435, 288, 12, 12), _font);
        new Town("Dubhaim Castle", 340, new Rectangle(422, 144, 12, 12), _font);
        new Town("Rucesion", 3010, new Rectangle(494, 202, 12, 12), _font);
        new Town("Undine", 3008, new Rectangle(100, 130, 12, 12), _font);
        new Town("Astrid", 3060, new Rectangle(109, 167, 12, 12), _font);
        new Town("Suomi", 3016, new Rectangle(50, 181, 12, 12), _font);
        new Town("Flamel", 21400, new Rectangle(7, 212, 12, 12), _font);
        new Town("Asal", 20108, new Rectangle(160, 250, 12, 12), _font);
        new Town("Loures", 664, new Rectangle(332, 265, 12, 12), _font);
        new Town("House Macabre", 2052, new Rectangle(92, 269, 12, 12), _font);
        new Town("Finvarra", 25730, new Rectangle(15, 281, 12, 12), _font);
        new Town("Tagor", 662, new Rectangle(111, 308, 12, 12), _font);
        new Town("Zarmuth", 21001, new Rectangle(497, 339, 12, 12), _font);
        new Town("Mt Merry", 7050, new Rectangle(135, 91, 12, 12), _font);
        new Town("Coliseum Arena", 5232, new Rectangle(312, 169, 12, 12), _font);
    }

    /// <summary>
    ///     Ports the player toward town <paramref name="t"/>: finds the exit on the current map that leads
    ///     to that town's map and enters it at a random tile within the exit's destination range.
    /// </summary>
    private void NewTown(Town t)
    {
        int destX = 0;
        int destY = 0;
        foreach (JToken mapEntry in _mapsDB["maps"].Children())
        {
            if (mapEntry.Value<int>("mapnum") != _player._mapNum)
            {
                continue;
            }
            foreach (JToken exit in mapEntry["to"].Children())
            {
                int destMap = exit.Value<int>("map");
                if (destMap == t._map)
                {
                    destX = rand.Next(exit["dest"]["lowx"].Value<int>(), exit["dest"]["highx"].Value<int>() + 1);
                    destY = rand.Next(exit["dest"]["lowy"].Value<int>(), exit["dest"]["highy"].Value<int>() + 1);
                    NewMap(destMap, destX, destY);
                }
            }
        }
    }

    #endregion

    #region Guild membership

    /// <summary>Founds a new guild led by the player and stamps the founder + leader legend marks.</summary>
    private void createGuild(string guildname)
    {
        _player._guild = guildname;
        _player._rank = "Leader";
        SaveProfile();
        NewLegendMark(7, "GldFnd", "Founded guild " + guildname + " - Deoch 1, Spring", 1);
        NewLegendMark(7, "GldLd", "Guild Leader - Deoch 1, Spring", 1);
    }

    /// <summary>Sets the player's guild (matching an existing guild's canonical casing when found) and stamps the join legend mark.</summary>
    private void joinGuild(string guildname)
    {
        string guild = guildname;
        foreach (JToken guildEntry in _guildsDB["guild"].Children())
        {
            string name = guildEntry.Value<string>("name");
            if (name.ToLower() == guildname.ToLower())
            {
                guild = name;
            }
        }
        _player._guild = guild;
        SaveProfile();
        NewLegendMark(7, "GldOath", "Guild by oath of Aricin - Deoch 1, Spring");
    }

    /// <summary>True if a guild with this name (case-insensitive) exists in the guilds DB.</summary>
    private bool guildExists(string guildname)
    {
        foreach (JToken guildEntry in _guildsDB["guild"].Children())
        {
            string name = guildEntry.Value<string>("name");
            if (name.ToLower() == guildname.ToLower())
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>True if the player appears as a member of any guild in the guilds DB. (The <paramref name="guildname"/> argument is currently unused.)</summary>
    private bool playerInGuild(string guildname)
    {
        foreach (JToken guildEntry in _guildsDB["guild"].Children())
        {
            foreach (JToken member in guildEntry["members"].Children())
            {
                if (member.Value<string>("name").ToLower() == _player._name.ToLower())
                {
                    return true;
                }
            }
        }
        return false;
    }

    #endregion

    #region Inventory slot geometry, lookup & allocation

    /// <summary>
    ///     Maps a screen (x, y) pixel to the inventory/equipment slot beneath it. Slots are laid out
    ///     as rows of 36px-wide cells starting at x=94; which rows are live depends on the active
    ///     panel (<c>_panelNum</c>) and whether the full-inventory view is open. Returns the slot
    ///     number, or 0 if (x, y) lands on no cell.
    /// </summary>
    private int getXYSlot(int x, int y)
    {
        const int gridLeft = 94;
        const int cellWidth = 36;
        int cellX = gridLeft;
        if (_miscMenu._buttons["fullInvBtn"].Selected && _panelNum == 0)
        {
            if (y >= 234 && y < 268)
            {
                for (int slot = 1; slot < 13; slot++)
                {
                    if (x >= cellX && x <= cellX + cellWidth)
                    {
                        return slot;
                    }
                    cellX += cellWidth;
                }
            }
            else if (y >= 268 && y < 302)
            {
                for (int slot = 13; slot < 25; slot++)
                {
                    if (x >= cellX && x <= cellX + cellWidth)
                    {
                        return slot;
                    }
                    cellX += cellWidth;
                }
            }
            else if (y >= 302 && y <= 336)
            {
                for (int slot = 25; slot < 37; slot++)
                {
                    if (x >= cellX && x <= cellX + cellWidth)
                    {
                        return slot;
                    }
                    cellX += cellWidth;
                }
            }
            else if (y >= 336 && y < 370)
            {
                for (int slot = 37; slot < 49; slot++)
                {
                    if (x >= cellX && x <= cellX + cellWidth)
                    {
                        return slot;
                    }
                    cellX += cellWidth;
                }
            }
            else if (y >= 370 && y < 404)
            {
                for (int slot = 49; slot < 61; slot++)
                {
                    if (x >= cellX && x <= cellX + cellWidth)
                    {
                        return slot;
                    }
                    cellX += cellWidth;
                }
            }
            else if (y >= 404 && y <= 434)
            {
                for (int slot = 61; slot < 73; slot++)
                {
                    if (x >= cellX && x <= cellX + cellWidth)
                    {
                        return slot;
                    }
                    cellX += cellWidth;
                }
            }
        }
        else if (_panelNum == 6 || _panelNum == 7 || _panelNum == 10)
        {
            if (y >= 336 && y < 370)
            {
                for (int slot = 37; slot < 49; slot++)
                {
                    if (x >= cellX && x <= cellX + cellWidth)
                    {
                        return slot;
                    }
                    cellX += cellWidth;
                }
            }
            else if (y >= 370 && y < 404)
            {
                for (int slot = 49; slot < 61; slot++)
                {
                    if (x >= cellX && x <= cellX + cellWidth)
                    {
                        return slot;
                    }
                    cellX += cellWidth;
                }
            }
            else if (y >= 404 && y <= 434)
            {
                for (int slot = 61; slot < 73; slot++)
                {
                    if (x >= cellX && x <= cellX + cellWidth)
                    {
                        return slot;
                    }
                    cellX += cellWidth;
                }
                if (x >= cellX && x <= cellX + cellWidth)
                {
                    return 72;
                }
            }
        }
        else if (_panelNum == 0)
        {
            if (y >= 336 && y < 370)
            {
                for (int slot = 1; slot < 13; slot++)
                {
                    if (x >= cellX && x <= cellX + cellWidth)
                    {
                        return slot;
                    }
                    cellX += cellWidth;
                }
            }
            else if (y >= 370 && y < 404)
            {
                for (int slot = 13; slot < 25; slot++)
                {
                    if (x >= cellX && x <= cellX + cellWidth)
                    {
                        return slot;
                    }
                    cellX += cellWidth;
                }
            }
            else if (y >= 404 && y <= 434)
            {
                for (int slot = 25; slot < 36; slot++)
                {
                    if (x >= cellX && x <= cellX + cellWidth)
                    {
                        return slot;
                    }
                    cellX += cellWidth;
                }
                if (x >= cellX && x <= cellX + cellWidth)
                {
                    return 72;
                }
            }
        }
        else if (y >= 336 && y < 370)
        {
            for (int slot = 1; slot < 13; slot++)
            {
                if (x >= cellX && x <= cellX + cellWidth)
                {
                    return slot;
                }
                cellX += cellWidth;
            }
        }
        else if (y >= 370 && y < 404)
        {
            for (int slot = 13; slot < 25; slot++)
            {
                if (x >= cellX && x <= cellX + cellWidth)
                {
                    return slot;
                }
                cellX += cellWidth;
            }
        }
        else if (y >= 404 && y <= 434)
        {
            for (int slot = 25; slot < 37; slot++)
            {
                if (x >= cellX && x <= cellX + cellWidth)
                {
                    return slot;
                }
                cellX += cellWidth;
            }
            if (x >= cellX && x <= cellX + cellWidth)
            {
                return 72;
            }
        }
        return 0;
    }

    private InventoryItem getItemSlot(int slot)
    {
        foreach (InventoryItem item in _inventory)
        {
            if (item._slot == slot)
            {
                return item;
            }
        }
        return null;
    }

    private Skill getSkillSlot(int slot)
    {
        foreach (Skill skill in _skills)
        {
            if (skill._slot == slot)
            {
                return skill;
            }
        }
        return null;
    }

    private Spell getSpellSlot(int slot)
    {
        foreach (Spell spell in _spells)
        {
            if (spell._slot == slot)
            {
                return spell;
            }
        }
        return null;
    }

    private Action getActionSlot(int slot)
    {
        foreach (Action action in _actions)
        {
            if (action._slot == slot)
            {
                return action;
            }
        }
        return null;
    }

    private InventoryItem getEquipmentSlot(int slot)
    {
        foreach (InventoryItem item in _equipment)
        {
            if (item._slot == slot)
            {
                return item;
            }
        }
        return null;
    }

    /// <summary>First unused skill slot in [1, 71), or 0 if all are taken.</summary>
    private int FirstAvailableSkillSlot()
    {
        const int maxSlots = 71;
        int firstFree = 1;
        for (int slot = 1; slot < maxSlots; slot++)
        {
            bool taken = false;
            foreach (Skill occupant in _skills.OrderBy((Skill s) => s._slot))
            {
                if (slot == occupant._slot)
                {
                    taken = true;
                }
            }
            if (!taken)
            {
                firstFree = slot;
                if (firstFree < maxSlots)
                {
                    return firstFree;
                }
            }
        }
        if (firstFree < maxSlots)
        {
            return firstFree;
        }
        return 0;
    }

    /// <summary>First unused spell slot in [1, 71), or 0 if all are taken.</summary>
    private int FirstAvailableSpellSlot()
    {
        const int maxSlots = 71;
        int firstFree = 1;
        for (int slot = 1; slot < maxSlots; slot++)
        {
            bool taken = false;
            foreach (Spell occupant in _spells.OrderBy((Spell s) => s._slot))
            {
                if (slot == occupant._slot)
                {
                    taken = true;
                }
            }
            if (!taken)
            {
                firstFree = slot;
                if (firstFree < maxSlots)
                {
                    return firstFree;
                }
            }
        }
        if (firstFree < maxSlots)
        {
            return firstFree;
        }
        return 0;
    }

    /// <summary>First unused action slot in [1, 71), or 0 if all are taken.</summary>
    private int FirstAvailableActionSlot()
    {
        const int maxSlots = 71;
        int firstFree = 1;
        for (int slot = 1; slot < maxSlots; slot++)
        {
            bool taken = false;
            foreach (Action occupant in _actions.OrderBy((Action s) => s._slot))
            {
                if (slot == occupant._slot)
                {
                    taken = true;
                }
            }
            if (!taken)
            {
                firstFree = slot;
                if (firstFree < maxSlots)
                {
                    return firstFree;
                }
            }
        }
        if (firstFree < maxSlots)
        {
            return firstFree;
        }
        return 0;
    }

    /// <summary>First unused inventory slot in [1, 71), or 0 if all are taken.</summary>
    private int FirstAvailableInventorySlot()
    {
        const int maxSlots = 71;
        int firstFree = 1;
        for (int slot = 1; slot < maxSlots; slot++)
        {
            bool taken = false;
            foreach (InventoryItem occupant in _inventory.OrderBy((InventoryItem s) => s._slot))
            {
                if (slot == occupant._slot)
                {
                    taken = true;
                }
            }
            if (!taken)
            {
                firstFree = slot;
                if (firstFree < maxSlots)
                {
                    return firstFree;
                }
            }
        }
        if (firstFree < maxSlots)
        {
            return firstFree;
        }
        return 0;
    }

    /// <summary>First unused spell-bar slot in [1, 20), or 0 if all are taken.</summary>
    private int AvailableSlot(List<SpellBar> bars)
    {
        const int maxSlots = 20;
        int firstFree = 1;
        for (int slot = 1; slot < maxSlots; slot++)
        {
            bool taken = false;
            foreach (SpellBar occupant in bars)
            {
                if (slot == occupant._slot)
                {
                    taken = true;
                }
            }
            if (!taken)
            {
                firstFree = slot;
                if (firstFree < maxSlots)
                {
                    return firstFree;
                }
            }
        }
        if (firstFree < maxSlots)
        {
            return firstFree;
        }
        return 0;
    }

    #endregion

    #region Entity spawning

    /// <summary>Spawns a projectile from <paramref name="en"/> in its facing direction, and notifies the server when connected.</summary>
    private void SpawnProjectile(Entity en, int type, int dmg)
    {
        new Projectile(_textureManager, en._id, type, en._body._direction, en._tile, dmg);
        if (GameWindow.ConnectedToServer)
        {
            ProjectilePacket projectilePacket = new ProjectilePacket(en._id, (byte)type);
            GameWindow.ClientSocket.Send(projectilePacket.Data);
        }
    }

    /// <summary>
    ///     Spawns every NPC for map <paramref name="mapnum"/> from the NPC DB: builds each NPC, loads
    ///     its sell list, dialogue speech-triggers, and learnable skills/spells/actions ("Create" NPCs
    ///     learn everything), positions it on its tile, and registers it in NPCDict.
    /// </summary>
    private void SpawnNpcs(int mapnum)
    {
        NPCDict.Clear();
        foreach (JToken npcEntry in _npcsDB["npc"].Children())
        {
            int entryMapnum = npcEntry.Value<int>("mapnum");
            if (entryMapnum != mapnum && entryMapnum != 0)
            {
                continue;
            }
            string text = npcEntry.Value<string>("name");
            if (NPCDict.ContainsKey(text) || (!(text == "Create") && GameWindow.ConnectedToServer))
            {
                continue;
            }
            Location location = new Location(npcEntry.Value<int>("x"), npcEntry.Value<int>("y"));
            string npcSource = npcEntry.Value<string>("source");
            if (string.IsNullOrEmpty(npcSource))
            {
                npcSource = "old";
            }
            string imgString = npcEntry.Value<string>("img");
            int direction = npcEntry.Value<int>("direction");
            byte hidden = npcEntry.Value<byte>("hidden");
            bool assailPush = npcEntry.Value<bool>("assailpush");
            JToken sellList = npcEntry["sell"];
            NPC npc = new NPC(this, _textureManager, _font, text, location, _map, imgString, direction, npcSource);
            npc._assailPush = assailPush;
            npc._baseHP = 60000000;
            npc._curHP = npc._baseHP;
            if (hidden == 1)
            {
                npc.Hidden = true;
            }
            if (sellList != null)
            {
                foreach (JToken sellEntry in sellList.Children())
                {
                    npc._selllist.Add(sellEntry.Value<string>("text"), sellEntry.Value<int>("value"));
                }
            }
            foreach (JToken dialogueEntry in _dialogDB["dialogue"].Children())
            {
                string dialogueName = dialogueEntry.Value<string>("name");
                if (!dialogueName.Equals(text, StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }
                for (int i = 0; i < dialogueEntry.Children().Count() - 1; i++)
                {
                    JToken dialoguePage = dialogueEntry[i.ToString()];
                    if (dialoguePage == null)
                    {
                        continue;
                    }
                    JToken options = dialoguePage["options"];
                    if (options == null)
                    {
                        continue;
                    }
                    for (int j = 0; j < options.Children().Count(); j++)
                    {
                        string value = options[(j + 1).ToString()].Value<string>("text");
                        JToken speechList = options[(j + 1).ToString()]["speech"];
                        if (speechList != null)
                        {
                            for (int k = 1; k <= speechList.Children().Count(); k++)
                            {
                                string speechKey = speechList.Value<string>(k.ToString());
                                npc._dialogSpeechTriggers.Add(speechKey, value);
                            }
                        }
                    }
                }
            }
            if (text == "Create")
            {
                foreach (JToken skillEntry in _skillsDB["skills"].Children())
                {
                    JToken skillMaxlev = skillEntry["maxlev"];
                    if (skillMaxlev != null)
                    {
                        npc._learnskills.Add(skillEntry.Value<string>("name"));
                    }
                }
                foreach (JToken spellEntry in _spellsDB["spells"].Children())
                {
                    JToken spellMaxlev = spellEntry["maxlev"];
                    if (spellMaxlev != null)
                    {
                        npc._learnspells.Add(spellEntry.Value<string>("name"));
                    }
                }
                foreach (JToken actionEntry in _actionsDB["actions"].Children())
                {
                    npc._learnactions.Add(actionEntry.Value<string>("name"));
                }
            }
            else
            {
                JToken learnSkillList = npcEntry["learnskill"];
                if (learnSkillList != null)
                {
                    foreach (JToken learnSkillEntry in learnSkillList.Children())
                    {
                        npc._learnskills.Add(learnSkillEntry.Value<string>("name"));
                    }
                }
                JToken learnSpellList = npcEntry["learnspell"];
                if (learnSpellList != null)
                {
                    foreach (JToken learnSpellEntry in learnSpellList.Children())
                    {
                        npc._learnspells.Add(learnSpellEntry.Value<string>("name"));
                    }
                }
                JToken learnActionList = npcEntry["learnaction"];
                if (learnActionList != null)
                {
                    foreach (JToken learnActionEntry in learnActionList.Children())
                    {
                        npc._learnactions.Add(learnActionEntry.Value<string>("name"));
                    }
                }
            }
            Tile tile = _map._tiles[location.Y * (int)_map._width + location.X];
            npc.SetPosition(new Vector(tile._position.X + tile._width / 2.0 - Math.Abs(npc._mBody._sprite.GetWidth()) / 2.0 + 1.0, tile._position.Y + tile._height - npc._mBody._sprite.GetHeight() + 1.0, 0.0));
            NPCDict.Add(text, npc);
        }
    }

    /// <summary>Spawns a single NPC by image number at <paramref name="loc"/> (ad-hoc/debug spawn; not registered in NPCDict).</summary>
    private void SpawnNpc(int img, Location loc, bool useNew = false)
    {
        string imgName = img.ToString("000");
        int direction = 1;
        NPC npc = new NPC(this, _textureManager, _font, imgName, loc, _map, imgName, direction);
        npc._baseHP = 60000000;
        npc._curHP = npc._baseHP;
        Tile tile = _map._tiles[loc.Y * (int)_map._width + loc.X];
        npc.SetPosition(new Vector(tile._position.X + tile._width / 2.0 - Math.Abs(npc._mBody._sprite.GetWidth()) / 2.0 + 1.0, tile._position.Y + tile._height - npc._mBody._sprite.GetHeight() + 1.0, 0.0));
    }

    /// <summary>
    ///     Spawns monsters for map <paramref name="mapnum"/> from the monster DB: for each monster whose
    ///     spawn list includes this map, registers a Spawn (cap + rate) and instantiates up to the cap —
    ///     rolling a position (within spawn areas, or anywhere), HP/MP, atk/def, gold, drops
    ///     (weapon/gear/loot/potion/special) and skills/spells — placing each on a walkable tile.
    /// </summary>
    private void SpawnMonsters(int mapnum)
    {
        foreach (JToken monsterEntry in _monstersDB["monsters"].Children())
        {
            JToken mapsNode = monsterEntry["maps"];
            if (mapsNode == null)
            {
                continue;
            }
            foreach (JToken mapEntry in mapsNode.Children())
            {
                int spawnMapnum = mapEntry.Value<int>("num");
                if (spawnMapnum != mapnum)
                {
                    continue;
                }
                string monsterType = monsterEntry.Value<string>("type");
                int spawnRate = mapEntry.Value<int>("spawnrate");
                int spawnCap = mapEntry.Value<int>("spawncap");
                _map._spawns.Add(monsterType, new Spawn
                {
                    SpawnCap = spawnCap,
                    SpawnRate = spawnRate,
                    LastSpawn = DateTime.UtcNow
                });
                if (MonsterCount(monsterType, spawnMapnum) >= spawnCap)
                {
                    break;
                }
                JToken spawnAreasNode = mapEntry["spawnareas"];
                string monsterName = monsterEntry.Value<string>("name");
                int stay = monsterEntry.Value<int>("stay");
                if (stay != 1)
                {
                    stay = 0;
                }
                string imgString = monsterEntry.Value<string>("img");
                int direction = monsterEntry.Value<int>("direction");
                byte passable = monsterEntry.Value<byte>("passable");
                string source = monsterEntry.Value<string>("source");
                if (string.IsNullOrEmpty(source))
                {
                    source = "old";
                }
                if (direction == 0)
                {
                    direction = 1;
                }
                for (int i = 0; i < spawnCap; i++)
                {
                    int x;
                    int y;
                    if (spawnAreasNode != null)
                    {
                        List<Location> spawnTiles = new List<Location>();
                        foreach (JToken spawnArea in spawnAreasNode.Children())
                        {
                            for (int j = spawnArea.Value<int>("lowy"); j <= spawnArea.Value<int>("highy"); j++)
                            {
                                for (int k = spawnArea.Value<int>("lowx"); k <= spawnArea.Value<int>("highx"); k++)
                                {
                                    spawnTiles.Add(new Location(k, j));
                                }
                            }
                        }
                        int index = rand.Next(0, spawnTiles.Count - 1);
                        x = spawnTiles[index].X;
                        y = spawnTiles[index].Y;
                    }
                    else
                    {
                        x = rand.Next(0, (int)_map._width - 1);
                        y = rand.Next(0, (int)_map._height - 1);
                    }
                    Location location = new Location(x, y);
                    Tile tile = _map._tiles[location.Y * (int)_map._width + location.X];
                    if (tile == null || tile.getTopMostNonItem() != null || !tile._walkable)
                    {
                        continue;
                    }
                    Monster monster = new Monster(this, _textureManager, _font, monsterName, location, _map, imgString, direction, monsterType);
                    monster._passable = passable;
                    monster._mBody._stay = stay;
                    monster._hostileType = monsterEntry.Value<byte>("hostile");
                    monster.ResetHostile();
                    monster._baseHP = rand.Next(monsterEntry.Value<int>("minhp"), monsterEntry.Value<int>("maxhp"));
                    monster._baseMP = rand.Next(monsterEntry.Value<int>("minmp"), monsterEntry.Value<int>("maxmp"));
                    monster._curHP = monster._baseHP;
                    monster._curMP = monster._baseMP;
                    JToken atkNode = monsterEntry["atk"];
                    monster._atk = atkNode.Value<string>(rand.Next(1, atkNode.Count()).ToString());
                    JToken defNode = monsterEntry["def"];
                    monster._def = defNode.Value<string>(rand.Next(1, defNode.Count()).ToString());
                    monster._lev = monsterEntry.Value<byte>("lev");
                    monster._exp = monsterEntry.Value<uint>("exp");
                    monster._ac = monsterEntry.Value<byte>("ac");
                    monster._mr = monsterEntry.Value<byte>("mr");
                    monster._drops = new List<string>();
                    monster._skilllist = new Dictionary<string, MSkill>();
                    monster._spelllist = new Dictionary<string, MSkill>();
                    JToken goldNode = monsterEntry["gold"];
                    if (goldNode != null)
                    {
                        foreach (JToken goldEntry in goldNode.Children())
                        {
                            int perc = goldEntry.Value<int>("perc");
                            int roll = rand.Next(1, 100);
                            if (roll <= perc)
                            {
                                monster._gold = goldEntry.Value<int>("amount");
                            }
                        }
                    }
                    JToken dropsNode = monsterEntry["drops"];
                    if (dropsNode != null)
                    {
                        foreach (JToken dropEntry in dropsNode.Children())
                        {
                            string dropType = dropEntry.Value<string>("type");
                            if (dropType == "weapon")
                            {
                                int perc = dropEntry.Value<int>("perc");
                                int roll = rand.Next(1, 100);
                                if (roll <= perc)
                                {
                                    monster._drops.Add(dropEntry.Value<string>("name"));
                                    break;
                                }
                            }
                        }
                        foreach (JToken dropEntry in dropsNode.Children())
                        {
                            string dropType = dropEntry.Value<string>("type");
                            if (dropType == "gear")
                            {
                                int perc = dropEntry.Value<int>("perc");
                                int roll = rand.Next(1, 100);
                                if (roll <= perc)
                                {
                                    monster._drops.Add(dropEntry.Value<string>("name"));
                                    break;
                                }
                            }
                        }
                        foreach (JToken dropEntry in dropsNode.Children())
                        {
                            string dropType = dropEntry.Value<string>("type");
                            if (dropType == "loot")
                            {
                                int perc = dropEntry.Value<int>("perc");
                                int roll = rand.Next(1, 100);
                                if (roll <= perc)
                                {
                                    monster._drops.Add(dropEntry.Value<string>("name"));
                                    break;
                                }
                            }
                        }
                        foreach (JToken dropEntry in dropsNode.Children())
                        {
                            string dropType = dropEntry.Value<string>("type");
                            if (dropType == "potion")
                            {
                                int perc = dropEntry.Value<int>("perc");
                                int roll = rand.Next(1, 100);
                                if (roll <= perc)
                                {
                                    monster._drops.Add(dropEntry.Value<string>("name"));
                                    break;
                                }
                            }
                        }
                        foreach (JToken dropEntry in dropsNode.Children())
                        {
                            string dropType = dropEntry.Value<string>("type");
                            if (dropType == "special")
                            {
                                int perc = dropEntry.Value<int>("perc");
                                int roll = rand.Next(1, 100);
                                if (roll <= perc)
                                {
                                    monster._drops.Add(dropEntry.Value<string>("name"));
                                    break;
                                }
                            }
                        }
                    }
                    JToken skillsNode = monsterEntry["skills"];
                    if (skillsNode != null)
                    {
                        foreach (JToken skillEntry in skillsNode.Children())
                        {
                            monster._skilllist.Add(skillEntry.Value<string>("name"), new MSkill(skillEntry.Value<string>("name"), skillEntry.Value<string>("trigger"), skillEntry.Value<int>("cdperc"), skillEntry.Value<byte>("min"), skillEntry.Value<byte>("max")));
                        }
                    }
                    JToken spellsNode = monsterEntry["spells"];
                    if (spellsNode != null)
                    {
                        foreach (JToken spellEntry in spellsNode.Children())
                        {
                            monster._spelllist.Add(spellEntry.Value<string>("name"), new MSkill(spellEntry.Value<string>("name"), spellEntry.Value<string>("trigger"), spellEntry.Value<int>("cdperc"), spellEntry.Value<byte>("min"), spellEntry.Value<byte>("max")));
                        }
                    }
                    monster.SetPosition(new Vector(tile._position.X + tile._width / 2.0 - Math.Abs(monster._mBody._sprite.GetWidth()) / 2.0 + 1.0, tile._position.Y + tile._height - monster._mBody._sprite.GetHeight() + 1.0, 0.0));
                }
                break;
            }
        }
    }

    /// <summary>
    ///     Spawns a single monster of the given <paramref name="type"/> on map <paramref name="mapnum"/>
    ///     from the monster DB (same build as SpawnMonsters: position, HP/MP, atk/def, gold, drops,
    ///     skills/spells), placing it on the first walkable tile found.
    /// </summary>
    private void SpawnMonster(int mapnum, string type)
    {
        foreach (JToken monsterEntry in _monstersDB["monsters"].Children())
        {
            JToken mapsNode = monsterEntry["maps"];
            if (mapsNode == null)
            {
                continue;
            }
            foreach (JToken mapEntry in mapsNode.Children())
            {
                int spawnMapnum = mapEntry.Value<int>("num");
                if (spawnMapnum != mapnum)
                {
                    continue;
                }
                string monsterType = monsterEntry.Value<string>("type");
                JToken spawnAreasNode = mapEntry["spawnareas"];
                string monsterName = monsterEntry.Value<string>("name");
                if (!(type.ToLower() == monsterType.ToLower()))
                {
                    continue;
                }
                int stay = monsterEntry.Value<int>("stay");
                if (stay != 1)
                {
                    stay = 0;
                }
                string imgString = monsterEntry.Value<string>("img");
                int direction = monsterEntry.Value<int>("direction");
                byte passable = monsterEntry.Value<byte>("passable");
                string source = monsterEntry.Value<string>("source");
                if (string.IsNullOrEmpty(source))
                {
                    source = "old";
                }
                if (direction == 0)
                {
                    direction = 1;
                }
                int x;
                int y;
                if (spawnAreasNode != null)
                {
                    List<Location> spawnTiles = new List<Location>();
                    foreach (JToken spawnArea in spawnAreasNode.Children())
                    {
                        for (int i = spawnArea.Value<int>("lowy"); i <= spawnArea.Value<int>("highy"); i++)
                        {
                            for (int j = spawnArea.Value<int>("lowx"); j <= spawnArea.Value<int>("highx"); j++)
                            {
                                spawnTiles.Add(new Location(j, i));
                            }
                        }
                    }
                    int index = rand.Next(0, spawnTiles.Count - 1);
                    x = spawnTiles[index].X;
                    y = spawnTiles[index].Y;
                }
                else
                {
                    x = rand.Next(0, (int)_map._width - 1);
                    y = rand.Next(0, (int)_map._height - 1);
                }
                Location location = new Location(x, y);
                Tile tile = _map._tiles[location.Y * (int)_map._width + location.X];
                if (tile == null || tile.getTopMostNonItem() != null || !tile._walkable)
                {
                    continue;
                }
                Monster monster = new Monster(this, _textureManager, _font, monsterName, location, _map, imgString, direction, monsterType);
                monster._passable = passable;
                monster._mBody._stay = stay;
                monster._hostileType = monsterEntry.Value<byte>("hostile");
                monster.ResetHostile();
                monster._baseHP = rand.Next(monsterEntry.Value<int>("minhp"), monsterEntry.Value<int>("maxhp"));
                monster._baseMP = rand.Next(monsterEntry.Value<int>("minmp"), monsterEntry.Value<int>("maxmp"));
                monster._curHP = monster._baseHP;
                monster._curMP = monster._baseMP;
                JToken atkNode = monsterEntry["atk"];
                monster._atk = atkNode.Value<string>(rand.Next(1, atkNode.Count()).ToString());
                JToken defNode = monsterEntry["def"];
                monster._def = defNode.Value<string>(rand.Next(1, defNode.Count()).ToString());
                monster._lev = monsterEntry.Value<byte>("lev");
                monster._exp = monsterEntry.Value<uint>("exp");
                monster._ac = monsterEntry.Value<byte>("ac");
                monster._mr = monsterEntry.Value<byte>("mr");
                monster._drops = new List<string>();
                monster._skilllist = new Dictionary<string, MSkill>();
                monster._spelllist = new Dictionary<string, MSkill>();
                JToken goldNode = monsterEntry["gold"];
                if (goldNode != null)
                {
                    foreach (JToken goldEntry in goldNode.Children())
                    {
                        int perc = goldEntry.Value<int>("perc");
                        int roll = rand.Next(1, 100);
                        if (roll <= perc)
                        {
                            monster._gold = goldEntry.Value<int>("amount");
                        }
                    }
                }
                JToken dropsNode = monsterEntry["drops"];
                if (dropsNode != null)
                {
                    foreach (JToken dropEntry in dropsNode.Children())
                    {
                        string dropType = dropEntry.Value<string>("type");
                        if (dropType == "weapon")
                        {
                            int perc = dropEntry.Value<int>("perc");
                            int roll = rand.Next(1, 100);
                            if (roll <= perc)
                            {
                                monster._drops.Add(dropEntry.Value<string>("name"));
                                break;
                            }
                        }
                    }
                    foreach (JToken dropEntry in dropsNode.Children())
                    {
                        string dropType = dropEntry.Value<string>("type");
                        if (dropType == "gear")
                        {
                            int perc = dropEntry.Value<int>("perc");
                            int roll = rand.Next(1, 100);
                            if (roll <= perc)
                            {
                                monster._drops.Add(dropEntry.Value<string>("name"));
                                break;
                            }
                        }
                    }
                    foreach (JToken dropEntry in dropsNode.Children())
                    {
                        string dropType = dropEntry.Value<string>("type");
                        if (dropType == "loot")
                        {
                            int perc = dropEntry.Value<int>("perc");
                            int roll = rand.Next(1, 100);
                            if (roll <= perc)
                            {
                                monster._drops.Add(dropEntry.Value<string>("name"));
                                break;
                            }
                        }
                    }
                    foreach (JToken dropEntry in dropsNode.Children())
                    {
                        string dropType = dropEntry.Value<string>("type");
                        if (dropType == "potion")
                        {
                            int perc = dropEntry.Value<int>("perc");
                            int roll = rand.Next(1, 100);
                            if (roll <= perc)
                            {
                                monster._drops.Add(dropEntry.Value<string>("name"));
                                break;
                            }
                        }
                    }
                    foreach (JToken dropEntry in dropsNode.Children())
                    {
                        string dropType = dropEntry.Value<string>("type");
                        if (dropType == "special")
                        {
                            int perc = dropEntry.Value<int>("perc");
                            int roll = rand.Next(1, 100);
                            if (roll <= perc)
                            {
                                monster._drops.Add(dropEntry.Value<string>("name"));
                                break;
                            }
                        }
                    }
                }
                JToken skillsNode = monsterEntry["skills"];
                if (skillsNode != null)
                {
                    foreach (JToken skillEntry in skillsNode.Children())
                    {
                        monster._skilllist.Add(skillEntry.Value<string>("name"), new MSkill(skillEntry.Value<string>("name"), skillEntry.Value<string>("trigger"), skillEntry.Value<int>("cdperc"), skillEntry.Value<byte>("min"), skillEntry.Value<byte>("max")));
                    }
                }
                JToken spellsNode = monsterEntry["spells"];
                if (spellsNode != null)
                {
                    foreach (JToken spellEntry in spellsNode.Children())
                    {
                        monster._spelllist.Add(spellEntry.Value<string>("name"), new MSkill(spellEntry.Value<string>("name"), spellEntry.Value<string>("trigger"), spellEntry.Value<int>("cdperc"), spellEntry.Value<byte>("min"), spellEntry.Value<byte>("max")));
                    }
                }
                monster.SetPosition(new Vector(tile._position.X + tile._width / 2.0 - Math.Abs(monster._mBody._sprite.GetWidth()) / 2.0 + 1.0, tile._position.Y + tile._height - monster._mBody._sprite.GetHeight() + 1.0, 0.0));
                return;
            }
        }
    }

    /// <summary>
    ///     Spawns one monster of <paramref name="type"/> (DB name or type) at <paramref name="loc"/>
    ///     with the given source/direction/companion. Falls back to a generic level-99 monster when
    ///     <paramref name="type"/> is a bare image number (1-965); reports if neither matches.
    /// </summary>
    private void SpawnMonster(string type, Location loc, string source = "old", byte direction = 1, bool companion = false)
    {
        bool found = false;
        foreach (JToken monsterEntry in _monstersDB["monsters"].Children())
        {
            string monsterType = monsterEntry.Value<string>("type");
            string monsterName = monsterEntry.Value<string>("name");
            if (!(type.ToLower() == monsterType.ToLower()) && !(type.ToLower() == monsterName.ToLower()))
            {
                continue;
            }
            found = true;
            int stay = monsterEntry.Value<int>("stay");
            if (stay != 1)
            {
                stay = 0;
            }
            string imgString = monsterEntry.Value<string>("img");
            byte passable = monsterEntry.Value<byte>("passable");
            byte projectile = monsterEntry.Value<byte>("projectile");
            Tile tile = _map._tiles[loc.Y * (int)_map._width + loc.X];
            Monster monster = new Monster(this, _textureManager, _font, monsterName, loc, _map, imgString, direction, monsterType, source);
            monster._companion = companion;
            monster._projectile = projectile;
            monster._passable = passable;
            monster._mBody._stay = stay;
            monster._hostileType = monsterEntry.Value<byte>("hostile");
            monster.ResetHostile();
            monster._baseHP = rand.Next(monsterEntry.Value<int>("minhp"), monsterEntry.Value<int>("maxhp"));
            monster._baseMP = rand.Next(monsterEntry.Value<int>("minmp"), monsterEntry.Value<int>("maxmp"));
            monster._curHP = monster._baseHP;
            monster._curMP = monster._baseMP;
            JToken atkNode = monsterEntry["atk"];
            monster._atk = atkNode.Value<string>(rand.Next(1, atkNode.Count()).ToString());
            JToken defNode = monsterEntry["def"];
            monster._def = defNode.Value<string>(rand.Next(1, defNode.Count()).ToString());
            monster._lev = monsterEntry.Value<byte>("lev");
            monster._exp = monsterEntry.Value<uint>("exp");
            monster._ac = monsterEntry.Value<byte>("ac");
            monster._mr = monsterEntry.Value<byte>("mr");
            monster._drops = new List<string>();
            monster._skilllist = new Dictionary<string, MSkill>();
            monster._spelllist = new Dictionary<string, MSkill>();
            JToken goldNode = monsterEntry["gold"];
            if (goldNode != null)
            {
                foreach (JToken goldEntry in goldNode.Children())
                {
                    int perc = goldEntry.Value<int>("perc");
                    int roll = rand.Next(1, 100);
                    if (roll <= perc)
                    {
                        monster._gold = goldEntry.Value<int>("amount");
                    }
                }
            }
            JToken dropsNode = monsterEntry["drops"];
            if (dropsNode != null)
            {
                foreach (JToken dropEntry in dropsNode.Children())
                {
                    string dropType = dropEntry.Value<string>("type");
                    if (dropType == "weapon")
                    {
                        int perc = dropEntry.Value<int>("perc");
                        int roll = rand.Next(1, 100);
                        if (roll <= perc)
                        {
                            monster._drops.Add(dropEntry.Value<string>("name"));
                            break;
                        }
                    }
                }
                foreach (JToken dropEntry in dropsNode.Children())
                {
                    string dropType = dropEntry.Value<string>("type");
                    if (dropType == "gear")
                    {
                        int perc = dropEntry.Value<int>("perc");
                        int roll = rand.Next(1, 100);
                        if (roll <= perc)
                        {
                            monster._drops.Add(dropEntry.Value<string>("name"));
                            break;
                        }
                    }
                }
                foreach (JToken dropEntry in dropsNode.Children())
                {
                    string dropType = dropEntry.Value<string>("type");
                    if (dropType == "loot")
                    {
                        int perc = dropEntry.Value<int>("perc");
                        int roll = rand.Next(1, 100);
                        if (roll <= perc)
                        {
                            monster._drops.Add(dropEntry.Value<string>("name"));
                            break;
                        }
                    }
                }
                foreach (JToken dropEntry in dropsNode.Children())
                {
                    string dropType = dropEntry.Value<string>("type");
                    if (dropType == "potion")
                    {
                        int perc = dropEntry.Value<int>("perc");
                        int roll = rand.Next(1, 100);
                        if (roll <= perc)
                        {
                            monster._drops.Add(dropEntry.Value<string>("name"));
                            break;
                        }
                    }
                }
                foreach (JToken dropEntry in dropsNode.Children())
                {
                    string dropType = dropEntry.Value<string>("type");
                    if (dropType == "special")
                    {
                        int perc = dropEntry.Value<int>("perc");
                        int roll = rand.Next(1, 100);
                        if (roll <= perc)
                        {
                            monster._drops.Add(dropEntry.Value<string>("name"));
                            break;
                        }
                    }
                }
            }
            JToken skillsNode = monsterEntry["skills"];
            if (skillsNode != null)
            {
                foreach (JToken skillEntry in skillsNode.Children())
                {
                    monster._skilllist.Add(skillEntry.Value<string>("name"), new MSkill(skillEntry.Value<string>("name"), skillEntry.Value<string>("trigger"), skillEntry.Value<int>("cdperc"), skillEntry.Value<byte>("min"), skillEntry.Value<byte>("max")));
                }
            }
            JToken spellsNode = monsterEntry["spells"];
            if (spellsNode != null)
            {
                foreach (JToken spellEntry in spellsNode.Children())
                {
                    monster._spelllist.Add(spellEntry.Value<string>("name"), new MSkill(spellEntry.Value<string>("name"), spellEntry.Value<string>("trigger"), spellEntry.Value<int>("cdperc"), spellEntry.Value<byte>("min"), spellEntry.Value<byte>("max")));
                }
            }
            monster.SetPosition(new Vector(tile._position.X + tile._width / 2.0 - Math.Abs(monster._mBody._sprite.GetWidth()) / 2.0 + 1.0, tile._position.Y + tile._height - monster._mBody._sprite.GetHeight() + 1.0, 0.0));
            return;
        }
        if (!found)
        {
            if (int.TryParse(type, out var monsterNum))
            {
                Tile tile2 = _map._tiles[loc.Y * (int)_map._width + loc.X];
                Monster monster2 = new Monster(this, _textureManager, _font, type, loc, _map, monsterNum.ToString("000"), 1, type, source);
                monster2._passable = 0;
                monster2._mBody._stay = 0;
                monster2._hostileType = 0;
                monster2.ResetHostile();
                monster2._baseHP = rand.Next(350000, 600000);
                monster2._baseMP = rand.Next(100, 1000);
                monster2._curHP = monster2._baseHP;
                monster2._curMP = monster2._baseMP;
                monster2._atk = "Dark";
                monster2._def = "Dark";
                monster2._lev = 99;
                monster2._exp = 100000u;
                monster2._ac = 50;
                monster2._mr = 30;
                monster2._drops = new List<string>();
                monster2._skilllist = new Dictionary<string, MSkill>();
                monster2._spelllist = new Dictionary<string, MSkill>();
                monster2._gold = 10000;
                monster2._skilllist.Add("Assail", new MSkill("Assail", "normal", 95, 1, 2));
                monster2.SetPosition(new Vector(tile2._position.X + tile2._width / 2.0 - Math.Abs(monster2._mBody._sprite.GetWidth()) / 2.0 + 1.0, tile2._position.Y + tile2._height - monster2._mBody._sprite.GetHeight() + 1.0, 0.0));
            }
            else
            {
                SystemMsg("Monster name not found in database; Note: #s 1-965 work.", 3);
            }
        }
    }

    /// <summary>
    ///     Drops an item entity named <paramref name="name"/> on the ground at <paramref name="loc"/>:
    ///     looks it up in the items DB and resolves its sprite (handling Gold -> coin/pile variants,
    ///     the old/new/myda/custom image sets, and " e:" enchantment suffixes), then places it on its tile.
    /// </summary>
    private void SpawnItem(string name, Location loc, int amount = 1, int durability = 0, int bodyImgColor = 0, string enchantment = "")
    {
        if (name.Contains(" e:"))
        {
            enchantment = name.Substring(name.IndexOf(" e:") + 3);
            name = name.Remove(name.IndexOf(" e:"));
        }
        foreach (JToken itemEntry in _itemsDB["items"].Children())
        {
            string itemName = itemEntry.Value<string>("name");
            if (!itemName.Equals(name, StringComparison.CurrentCultureIgnoreCase))
            {
                continue;
            }
            int imageFolder = itemEntry.Value<int>("imagefolder");
            int frame = itemEntry.Value<int>("frame");
            if (itemName == "Gold")
            {
                if (amount <= 0)
                {
                    break;
                }
                if (amount == 1)
                {
                    itemName = "Silver Coin";
                    frame = 138;
                }
                else if (amount < 10)
                {
                    itemName = "Gold Coin";
                    frame = 137;
                }
                else if (amount < 100)
                {
                    itemName = "Silver Pile";
                    frame = 141;
                }
                else
                {
                    itemName = "Gold Pile";
                    frame = 140;
                }
            }
            string imageType = itemEntry.Value<string>("imagetype");
            if (string.IsNullOrEmpty(imageType))
            {
                imageType = "old";
            }
            string imageSuffix = "";
            if (imageType == "new")
            {
                imageSuffix = "_new";
            }
            if (imageType == "myda")
            {
                imageSuffix = "_myda";
            }
            if (imageType == "custom")
            {
                imageSuffix = "_custom";
            }
            string spriteName = "item" + imageFolder.ToString("000") + "_F" + (frame - 1).ToString() + imageSuffix + "_C" + bodyImgColor;
            if (imageType == "custom")
            {
                spriteName = "item" + imageFolder.ToString("000") + "_F" + frame.ToString() + imageSuffix + "_C" + bodyImgColor;
            }
            Texture img = _textureManager.Get(spriteName, ".epf", imageType);
            Item item = new Item(this, _textureManager, _font, itemName, loc, _map, spriteName, img, amount, durability, bodyImgColor, enchantment);
            Tile tile = _map._tiles[loc.Y * (int)_map._width + loc.X];
            item.SetPosition(new Vector(tile._position.X + tile._width / 2.0 - item._sprite.GetWidth() / 2.0 + 1.0, tile._position.Y + tile._height - item._sprite.GetHeight() + 1.0, 0.0));
            break;
        }
    }

    #endregion

    /// <summary>Counts live monsters of <paramref name="montype"/> currently on the active map.</summary>
    private int MonsterCount(string montype, int mapnum)
    {
        if (mapnum != _map._number)
        {
            return 0;
        }
        int count = 0;
        foreach (Entity entity in _map._entities.Values)
        {
            if (entity is Monster && (entity as Monster)._type == montype && entity._mapNum == mapnum)
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    ///     Adds or refreshes a legend mark: substitutes $birthtown/$birthseason/$birthage tokens, and if a
    ///     same-id mark already exists with a "(n)" counter, bumps it to (n+1). Enables the legend scrollbar
    ///     past 14 marks; saves unless <paramref name="save"/> is false.
    /// </summary>
    private void NewLegendMark(int icon, string id, string text, int color = 0, bool save = true)
    {
        if (text.Contains("$birthtown") && questvars.ContainsKey("birthtown"))
        {
            text = text.Replace("$birthtown", questvars["birthtown"]);
        }
        if (text.Contains("$birthseason") && questvars.ContainsKey("birthseason"))
        {
            text = text.Replace("$birthseason", questvars["birthseason"]);
        }
        if (text.Contains("$birthage") && questvars.ContainsKey("birthage"))
        {
            text = text.Replace("$birthage", questvars["birthage"]);
        }
        string updatedText = "";
        LegendMark[] marks = _player._legendMarks.ToArray();
        foreach (LegendMark legendMark in marks)
        {
            if (!(legendMark._id == id))
            {
                continue;
            }
            if (legendMark._text.Contains("(") && legendMark._text.Contains(")"))
            {
                string countStr = legendMark._text.Remove(legendMark._text.IndexOf(")"));
                countStr = countStr.Substring(countStr.IndexOf("(") + 1);
                Console.WriteLine(countStr);
                if (int.TryParse(countStr, out var parsedCount))
                {
                    parsedCount++;
                    updatedText = legendMark._text.Substring(0, legendMark._text.IndexOf("(") + 1) + parsedCount + legendMark._text.Substring(legendMark._text.IndexOf(")"));
                }
            }
            _player._legendMarks.Remove(legendMark);
        }
        if (updatedText != "")
        {
            text = updatedText;
        }
        _player._legendMarks.Add(new LegendMark(_textureManager, _font, icon, id, text, color));
        if (_player._legendMarks.Count > 14 && !_legendMenu._buttons["legendScrollUpBtn"].Enabled)
        {
            _legendMenu._buttons["legendScrollUpBtn"].Enabled = true;
            _legendMenu._buttons["legendScrollDownBtn"].Enabled = true;
            _legendMenu._buttons["legendScrollerBtn"].Hidden = false;
            _legendMenu._buttons["legendScrollerBtn"].Enabled = true;
        }
        if (save)
        {
            SaveLegend();
        }
    }

    /// <summary>
    ///     Equips the named gear: maps its tab to an equipment slot, builds it from the items DB
    ///     (applying any enchantment / smith bonuses), swaps out whatever was in the slot, updates the
    ///     player's body sprite + combined stat bonuses, and saves. Rejects gear that doesn't fit the gender.
    /// </summary>
    private void NewEquip(string name, string tab, int durability = 0, int bodyImgColor = 0, InventoryItem inventoryItem = null, int slot = 0, string enchantment = "", bool save = true)
    {
        if (string.IsNullOrEmpty(enchantment))
        {
            enchantment = "";
        }
        string appendage = "";
        switch (tab)
        {
            case "Earrings":
                slot = 1;
                break;
            case "Necklace":
                slot = 2;
                break;
            case "Shield":
                slot = 3;
                appendage = "s";
                break;
            case "Gauntlet":
            case "Bracer":
                if (slot == 0)
                {
                    slot = 4;
                    if (_equipmentSlots[slot - 1]._item != null)
                    {
                        slot = 12;
                    }
                }
                break;
            case "Ring":
                if (slot == 0)
                {
                    slot = 5;
                    if (_equipmentSlots[slot - 1]._item != null)
                    {
                        slot = 13;
                    }
                }
                break;
            case "Belt":
                slot = 6;
                break;
            case "Greaves":
                slot = 7;
                break;
            case "Boots":
                slot = 8;
                appendage = "l";
                break;
            case "Weapon":
                slot = 9;
                appendage = "w";
                break;
            default:
                if (tab.StartsWith("Helmet"))
                {
                    slot = 10;
                    appendage = "h";
                }
                else if (tab.StartsWith("Armor"))
                {
                    slot = 11;
                    appendage = "u";
                }
                else if (tab == "Accessory")
                {
                    slot = 14;
                    appendage = "c";
                }
                break;
        }
        Slot equipSlot = _equipmentSlots[slot - 1];
        InventoryItem equip = null;
        foreach (JToken itemEntry in _itemsDB["items"].Children())
        {
            if (!itemEntry.Value<string>("name").Equals(name, StringComparison.CurrentCultureIgnoreCase))
            {
                continue;
            }
            int imageFolder = itemEntry.Value<int>("imagefolder");
            int frame = itemEntry.Value<int>("frame");
            string imageType = itemEntry.Value<string>("imagetype");
            if (string.IsNullOrEmpty(imageType))
            {
                imageType = "old";
            }
            int finalColor = itemEntry.Value<int>("bodyimgcolor");
            if (bodyImgColor > 0)
            {
                finalColor = bodyImgColor;
            }
            equip = new InventoryItem(_textureManager, name, _equipmentSlots[slot - 1], imageType, imageFolder, frame, finalColor);
            int boundFlag = itemEntry.Value<int>("bound");
            if (boundFlag == 1)
            {
                equip._bound = true;
            }
            equip._maxAmount = itemEntry.Value<int>("stack");
            equip._source = imageType;
            equip._tab = itemEntry.Value<string>("tab");
            equip._value = itemEntry.Value<int>("value");
            equip._description = itemEntry.Value<string>("desc");
            equip._maxDurability = itemEntry.Value<int>("dura");
            equip._durability = equip._maxDurability;
            equip._dyeable = itemEntry.Value<int>("dyeable");
            equip._bodyImg = itemEntry.Value<int>("bodyimg");
            equip._canAtkEles = Convert.ToBoolean(itemEntry.Value<byte>("canatkele"));
            equip._canDefEles = Convert.ToBoolean(itemEntry.Value<byte>("candefele"));
            equip._canEnchant = Convert.ToBoolean(itemEntry.Value<byte>("canenchant"));
            equip._canSmith = Convert.ToBoolean(itemEntry.Value<byte>("cansmith"));
            JToken statsNode = itemEntry["stats"];
            if (statsNode != null)
            {
                equip._level = statsNode.Value<string>("lev");
                equip._gender = statsNode.Value<string>("gender");
                equip._weaponDmg = statsNode.Value<string>("w");
                equip._atk = statsNode.Value<string>("atk");
                equip._def = statsNode.Value<string>("def");
                equip._hp = statsNode.Value<int>("hp");
                equip._mp = statsNode.Value<int>("mp");
                equip._str = statsNode.Value<short>("str");
                equip._int = statsNode.Value<short>("int");
                equip._wis = statsNode.Value<short>("wis");
                equip._con = statsNode.Value<short>("con");
                equip._dex = statsNode.Value<short>("dex");
                equip._mr = statsNode.Value<sbyte>("mr");
                equip._ac = statsNode.Value<sbyte>("ac");
                equip._dmg = statsNode.Value<sbyte>("dmg");
                equip._hit = statsNode.Value<sbyte>("hit");
                equip._reg = statsNode.Value<sbyte>("reg");
            }
            equip._displayName = name;
            if (inventoryItem != null && enchantment == "")
            {
                enchantment = inventoryItem._enchantment;
            }
            if (enchantment != "")
            {
                equip._enchantment = enchantment;
                equip._displayName = enchantment + " " + name;
                if (equip._canAtkEles)
                {
                    equip._atk = enchantment;
                }
                if (equip._canDefEles)
                {
                    equip._def = enchantment;
                }
                if (equip._canEnchant)
                {
                    switch (enchantment)
                    {
                        case "Deoch":
                            equip._reg += 5;
                            break;
                        case "Sgrios":
                            equip._ac++;
                            equip._maxDurability /= 2;
                            break;
                        case "Gramail":
                            equip._mr += 10;
                            break;
                        case "Ceannlaidir":
                            equip._str++;
                            break;
                        case "Luathas":
                            equip._int++;
                            break;
                        case "Glioca":
                            equip._wis++;
                            break;
                        case "Cail":
                            equip._con++;
                            break;
                        case "Fiosachd":
                            equip._dex++;
                            break;
                        case "Magic":
                            equip._mp += 100;
                            break;
                        case "Might":
                            equip._hp += 100;
                            break;
                        case "Blessed":
                            equip._hit += 5;
                            break;
                        case "Abundance":
                            equip._dmg++;
                            break;
                    }
                }
                if (equip._canSmith)
                {
                    if (equip._tab.Contains("Armor"))
                    {
                        switch (enchantment)
                        {
                            case "Good":
                                if (equip._level == "1")
                                {
                                    equip._level = "3";
                                }
                                else if (equip._level == "11")
                                {
                                    equip._level = "17";
                                }
                                else if (equip._level == "41")
                                {
                                    equip._level = "47";
                                }
                                else if (equip._level == "71")
                                {
                                    equip._level = "77";
                                }
                                equip._ac++;
                                if (equip._str > 0)
                                {
                                    equip._str++;
                                }
                                else if (equip._int > 0)
                                {
                                    equip._int++;
                                }
                                else if (equip._wis > 0)
                                {
                                    equip._wis++;
                                }
                                else if (equip._con > 0)
                                {
                                    equip._con++;
                                }
                                else if (equip._dex > 0)
                                {
                                    equip._dex++;
                                }
                                break;
                            case "Fine":
                                if (equip._level == "1")
                                {
                                    equip._level = "5";
                                }
                                else if (equip._level == "11")
                                {
                                    equip._level = "23";
                                }
                                else if (equip._level == "41")
                                {
                                    equip._level = "53";
                                }
                                else if (equip._level == "71")
                                {
                                    equip._level = "83";
                                }
                                equip._ac += 2;
                                if (equip._str > 0)
                                {
                                    equip._str += 2;
                                }
                                else if (equip._int > 0)
                                {
                                    equip._int += 2;
                                }
                                else if (equip._wis > 0)
                                {
                                    equip._wis += 2;
                                }
                                else if (equip._con > 0)
                                {
                                    equip._con += 2;
                                }
                                else if (equip._dex > 0)
                                {
                                    equip._dex += 2;
                                }
                                break;
                            case "Grand":
                                if (equip._level == "1")
                                {
                                    equip._level = "7";
                                }
                                else if (equip._level == "11")
                                {
                                    equip._level = "29";
                                }
                                else if (equip._level == "41")
                                {
                                    equip._level = "59";
                                }
                                else if (equip._level == "71")
                                {
                                    equip._level = "89";
                                }
                                equip._ac += 3;
                                if (equip._str > 0)
                                {
                                    equip._str += 3;
                                }
                                else if (equip._int > 0)
                                {
                                    equip._int += 3;
                                }
                                else if (equip._wis > 0)
                                {
                                    equip._wis += 3;
                                }
                                else if (equip._con > 0)
                                {
                                    equip._con += 3;
                                }
                                else if (equip._dex > 0)
                                {
                                    equip._dex += 3;
                                }
                                break;
                            case "Great":
                                if (equip._level == "1")
                                {
                                    equip._level = "9";
                                }
                                else if (equip._level == "11")
                                {
                                    equip._level = "35";
                                }
                                else if (equip._level == "41")
                                {
                                    equip._level = "65";
                                }
                                else if (equip._level == "71")
                                {
                                    equip._level = "95";
                                }
                                equip._ac += 4;
                                if (equip._str > 0)
                                {
                                    equip._str += 4;
                                }
                                else if (equip._int > 0)
                                {
                                    equip._int += 4;
                                }
                                else if (equip._wis > 0)
                                {
                                    equip._wis += 4;
                                }
                                else if (equip._con > 0)
                                {
                                    equip._con += 4;
                                }
                                else if (equip._dex > 0)
                                {
                                    equip._dex += 4;
                                }
                                break;
                        }
                    }
                    else
                    {
                        string[] dmgParts = equip._weaponDmg.Split('m');
                        int minDmg = int.Parse(dmgParts[0]);
                        int maxDmg = int.Parse(dmgParts[1]);
                        switch (enchantment)
                        {
                            default:
                                        break;
                            case "Good":
                            case "Fine":
                            case "Grand":
                                break;
                        }
                        equip._weaponDmg = minDmg + "m" + maxDmg;
                    }
                }
            }
            if (durability > 0)
            {
                equip._durability = durability;
            }
            if (equip._tab != "Weapon" && !equip._tab.StartsWith("Armor") && equip._tab != "Shield")
            {
                equip._sprite.SetScale(0.5, 0.5);
            }
            break;
        }
        if (equip == null)
        {
            SystemMsg(name + " not found in database.", 3);
        }
        else if (string.IsNullOrEmpty(equip._gender) || int.Parse(equip._gender) == _player._gender)
        {
            if (inventoryItem != null)
            {
                _inventory.Remove(inventoryItem);
                SaveItems();
            }
            if (equipSlot._item != null)
            {
                NewItem(equipSlot._item._name, 0, 1, equipSlot._item._durability, equipSlot._item._bodyImgColor, equipSlot._item._enchantment);
                RemoveEquip(slot);
            }
            _equipmentSlots[slot - 1]._item = equip;
            _equipment.Add(equip);
            if (appendage != "")
            {
                if (appendage == "c")
                {
                    _player._body._bodySource["g"] = equip._source;
                    _player._body._bodyImgs["g"] = equip._bodyImg;
                    if (equip._bodyImgColor != 0)
                    {
                        _player._body._bodyColors["g"] = equip._bodyImgColor;
                    }
                }
                if (appendage == "u")
                {
                    _player._body._bodySource["a"] = equip._source;
                    _player._body._bodyImgs["a"] = equip._bodyImg;
                    if (equip._bodyImgColor != 0)
                    {
                        _player._body._bodyColors["a"] = equip._bodyImgColor;
                    }
                }
                _player._body._bodySource[appendage] = equip._source;
                if (appendage == "h")
                {
                    _player._body._helmType = ConvertMydaNum(equip._bodyImg);
                    _player._body._helmColor = equip._bodyImgColor;
                    _player._body._bodyImgs[appendage] = ConvertMydaNum(equip._bodyImg);
                    if (equip._bodyImgColor != 0)
                    {
                        _player._body._bodyColors[appendage] = equip._bodyImgColor;
                    }
                }
                else
                {
                    _player._body._bodyImgs[appendage] = equip._bodyImg;
                    if (equip._bodyImgColor != 0)
                    {
                        _player._body._bodyColors[appendage] = equip._bodyImgColor;
                    }
                }
                if (appendage == "w")
                {
                    _player._body._bloody = false;
                }
                _player._body._idleBodyCount = -1;
                _player._body._idleWeaponCount = -1;
                _player._body._idleAccessoryCount = -1;
                _player._body._idleAccessorybCount = -1;
                _player._body.setDefault();
            }
            if (equip._hp != 0)
            {
                _player._combinedHPBonus += equip._hp;
            }
            if (equip._mp != 0)
            {
                _player._combinedMPBonus += equip._mp;
            }
            if (equip._str != 0)
            {
                _player._str += (ushort)equip._str;
            }
            if (equip._int != 0)
            {
                _player._int += (ushort)equip._int;
            }
            if (equip._wis != 0)
            {
                _player._wis += (ushort)equip._wis;
            }
            if (equip._con != 0)
            {
                _player._con += (ushort)equip._con;
            }
            if (equip._dex != 0)
            {
                _player._dex += (ushort)equip._dex;
            }
            if (equip._ac != 0)
            {
                Player player = _player;
                player._ac += (byte)equip._ac;
            }
            if (equip._mr != 0)
            {
                _player._mr += (byte)equip._mr;
            }
            if (equip._hit != 0)
            {
                _player._hit += (byte)equip._hit;
            }
            if (equip._dmg != 0)
            {
                _player._dmg += (byte)equip._dmg;
            }
            if (equip._reg != 0)
            {
                _player._reg += (byte)equip._reg;
            }
            if (!string.IsNullOrEmpty(equip._atk))
            {
                _player._atk = equip._atk;
            }
            if (!string.IsNullOrEmpty(equip._def))
            {
                _player._def = equip._def;
            }
            if (_player._curHP >= _player._baseHP && _player._baseHP < _player._maxHP)
            {
                _player._curHP = _player._maxHP;
            }
            if (_player._curHP > _player._maxHP)
            {
                _player._curHP = _player._maxHP;
            }
            if (_player._curMP >= _player._baseMP && _player._baseMP < _player._maxMP)
            {
                _player._curMP = _player._maxMP;
            }
            if (_player._curMP > _player._maxMP)
            {
                _player._curMP = _player._maxMP;
            }
            if (save)
            {
                SaveEquip();
            }
            SendProfileData();
        }
        else
        {
            SystemMsg("This does not fit you.", 3);
        }
    }

    /// <summary>
    ///     Adds the named item to the inventory: stacks onto an existing non-durable item (spilling any
    ///     overflow on the ground), else looks it up in the items DB, applies any enchantment (stat /
    ///     element / smith bonuses), sets amount + durability, and saves. False if it stacked or is unknown.
    /// </summary>
    public bool NewItem(string name, int slot = 0, int amount = 1, int durability = 0, int bodyImgColor = 0, string enchantment = "", bool save = true)
    {
        if (string.IsNullOrEmpty(enchantment))
        {
            enchantment = "";
        }
        if (amount == 0)
        {
            amount = 1;
        }
        if (name.Equals("Silver Coin", StringComparison.CurrentCultureIgnoreCase) || name.Equals("Gold Coin", StringComparison.CurrentCultureIgnoreCase) || name.Equals("Silver Pile", StringComparison.CurrentCultureIgnoreCase) || name.Equals("Gold Pile", StringComparison.CurrentCultureIgnoreCase))
        {
            name = "Gold";
        }
        foreach (InventoryItem stackItem in _inventory)
        {
            if (stackItem._name == name && stackItem._maxDurability == 0)
            {
                stackItem._amount += amount;
                if (stackItem._amount > stackItem._maxAmount)
                {
                    int overflow = stackItem._amount - stackItem._maxAmount;
                    stackItem._amount = stackItem._maxAmount;
                    SpawnItem(stackItem._name, new Location(_player._location.X, _player._location.Y), overflow);
                }
                return false;
            }
        }
        if (slot == 0)
        {
            slot = FirstAvailableInventorySlot();
        }
        if (slot == 0)
        {
            return false;
        }
        foreach (JToken itemEntry in _itemsDB["items"].Children())
        {
            string dbName = itemEntry.Value<string>("name");
            if (!dbName.Equals(name, StringComparison.CurrentCultureIgnoreCase))
            {
                continue;
            }
            int imageFolder = itemEntry.Value<int>("imagefolder");
            int frame = itemEntry.Value<int>("frame");
            string imageType = itemEntry.Value<string>("imagetype");
            if (string.IsNullOrEmpty(imageType))
            {
                imageType = "";
            }
            int finalColor = itemEntry.Value<int>("bodyimgcolor");
            if (bodyImgColor > 0)
            {
                finalColor = bodyImgColor;
            }
            InventoryItem inventoryItem = new InventoryItem(_textureManager, dbName, _inventorySlots[slot - 1], imageType, imageFolder, frame, finalColor);
            int boundFlag = itemEntry.Value<int>("bound");
            if (boundFlag == 1)
            {
                inventoryItem._bound = true;
            }
            inventoryItem._maxAmount = itemEntry.Value<int>("stack");
            inventoryItem._tab = itemEntry.Value<string>("tab");
            inventoryItem._value = itemEntry.Value<int>("value");
            inventoryItem._description = itemEntry.Value<string>("desc");
            inventoryItem._maxDurability = itemEntry.Value<int>("dura");
            inventoryItem._durability = inventoryItem._maxDurability;
            inventoryItem._dyeable = itemEntry.Value<int>("dyeable");
            inventoryItem._bodyImg = itemEntry.Value<int>("bodyimg");
            inventoryItem._canAtkEles = Convert.ToBoolean(itemEntry.Value<byte>("canatkele"));
            inventoryItem._canDefEles = Convert.ToBoolean(itemEntry.Value<byte>("candefele"));
            inventoryItem._canEnchant = Convert.ToBoolean(itemEntry.Value<byte>("canenchant"));
            inventoryItem._canSmith = Convert.ToBoolean(itemEntry.Value<byte>("cansmith"));
            JToken statsNode = itemEntry["stats"];
            if (statsNode != null)
            {
                inventoryItem._level = statsNode.Value<string>("lev");
                inventoryItem._gender = statsNode.Value<string>("gender");
                inventoryItem._weaponDmg = statsNode.Value<string>("w");
                inventoryItem._atk = statsNode.Value<string>("atk");
                inventoryItem._def = statsNode.Value<string>("def");
                inventoryItem._hp = statsNode.Value<int>("hp");
                inventoryItem._mp = statsNode.Value<int>("mp");
                inventoryItem._str = statsNode.Value<short>("str");
                inventoryItem._int = statsNode.Value<short>("int");
                inventoryItem._wis = statsNode.Value<short>("wis");
                inventoryItem._con = statsNode.Value<short>("con");
                inventoryItem._dex = statsNode.Value<short>("dex");
                inventoryItem._mr = statsNode.Value<sbyte>("mr");
                inventoryItem._ac = statsNode.Value<sbyte>("ac");
                inventoryItem._dmg = statsNode.Value<sbyte>("dmg");
                inventoryItem._hit = statsNode.Value<sbyte>("hit");
                inventoryItem._reg = statsNode.Value<sbyte>("reg");
            }
            inventoryItem._displayName = dbName;
            if (enchantment != "")
            {
                inventoryItem._enchantment = enchantment;
                inventoryItem._displayName = enchantment + " " + dbName;
                if (inventoryItem._canAtkEles)
                {
                    inventoryItem._atk = enchantment;
                }
                if (inventoryItem._canDefEles)
                {
                    inventoryItem._def = enchantment;
                }
                if (inventoryItem._canEnchant)
                {
                    switch (enchantment)
                    {
                        case "Deoch":
                            inventoryItem._reg += 5;
                            break;
                        case "Sgrios":
                            inventoryItem._ac++;
                            inventoryItem._maxDurability /= 2;
                            inventoryItem._durability = inventoryItem._maxDurability;
                            break;
                        case "Gramail":
                            inventoryItem._mr += 10;
                            break;
                        case "Ceannlaidir":
                            inventoryItem._str++;
                            break;
                        case "Luathas":
                            inventoryItem._int++;
                            break;
                        case "Glioca":
                            inventoryItem._wis++;
                            break;
                        case "Cail":
                            inventoryItem._con++;
                            break;
                        case "Fiosachd":
                            inventoryItem._dex++;
                            break;
                        case "Magic":
                            inventoryItem._mp += 100;
                            break;
                        case "Might":
                            inventoryItem._hp += 100;
                            break;
                        case "Blessed":
                            inventoryItem._hit += 5;
                            break;
                        case "Abundance":
                            inventoryItem._dmg++;
                            break;
                    }
                }
                if (inventoryItem._canSmith)
                {
                    if (inventoryItem._tab.Contains("Armor"))
                    {
                        switch (enchantment)
                        {
                            case "Good":
                                if (inventoryItem._level == "1")
                                {
                                    inventoryItem._level = "3";
                                }
                                else if (inventoryItem._level == "11")
                                {
                                    inventoryItem._level = "17";
                                }
                                else if (inventoryItem._level == "41")
                                {
                                    inventoryItem._level = "47";
                                }
                                else if (inventoryItem._level == "71")
                                {
                                    inventoryItem._level = "77";
                                }
                                inventoryItem._ac++;
                                if (inventoryItem._str > 0)
                                {
                                    inventoryItem._str++;
                                }
                                else if (inventoryItem._int > 0)
                                {
                                    inventoryItem._int++;
                                }
                                else if (inventoryItem._wis > 0)
                                {
                                    inventoryItem._wis++;
                                }
                                else if (inventoryItem._con > 0)
                                {
                                    inventoryItem._con++;
                                }
                                else if (inventoryItem._dex > 0)
                                {
                                    inventoryItem._dex++;
                                }
                                break;
                            case "Fine":
                                if (inventoryItem._level == "1")
                                {
                                    inventoryItem._level = "5";
                                }
                                else if (inventoryItem._level == "11")
                                {
                                    inventoryItem._level = "23";
                                }
                                else if (inventoryItem._level == "41")
                                {
                                    inventoryItem._level = "53";
                                }
                                else if (inventoryItem._level == "71")
                                {
                                    inventoryItem._level = "83";
                                }
                                inventoryItem._ac += 2;
                                if (inventoryItem._str > 0)
                                {
                                    inventoryItem._str += 2;
                                }
                                else if (inventoryItem._int > 0)
                                {
                                    inventoryItem._int += 2;
                                }
                                else if (inventoryItem._wis > 0)
                                {
                                    inventoryItem._wis += 2;
                                }
                                else if (inventoryItem._con > 0)
                                {
                                    inventoryItem._con += 2;
                                }
                                else if (inventoryItem._dex > 0)
                                {
                                    inventoryItem._dex += 2;
                                }
                                break;
                            case "Grand":
                                if (inventoryItem._level == "1")
                                {
                                    inventoryItem._level = "7";
                                }
                                else if (inventoryItem._level == "11")
                                {
                                    inventoryItem._level = "29";
                                }
                                else if (inventoryItem._level == "41")
                                {
                                    inventoryItem._level = "59";
                                }
                                else if (inventoryItem._level == "71")
                                {
                                    inventoryItem._level = "89";
                                }
                                inventoryItem._ac += 3;
                                if (inventoryItem._str > 0)
                                {
                                    inventoryItem._str += 3;
                                }
                                else if (inventoryItem._int > 0)
                                {
                                    inventoryItem._int += 3;
                                }
                                else if (inventoryItem._wis > 0)
                                {
                                    inventoryItem._wis += 3;
                                }
                                else if (inventoryItem._con > 0)
                                {
                                    inventoryItem._con += 3;
                                }
                                else if (inventoryItem._dex > 0)
                                {
                                    inventoryItem._dex += 3;
                                }
                                break;
                            case "Great":
                                if (inventoryItem._level == "1")
                                {
                                    inventoryItem._level = "9";
                                }
                                else if (inventoryItem._level == "11")
                                {
                                    inventoryItem._level = "35";
                                }
                                else if (inventoryItem._level == "41")
                                {
                                    inventoryItem._level = "65";
                                }
                                else if (inventoryItem._level == "71")
                                {
                                    inventoryItem._level = "95";
                                }
                                inventoryItem._ac += 4;
                                if (inventoryItem._str > 0)
                                {
                                    inventoryItem._str += 4;
                                }
                                else if (inventoryItem._int > 0)
                                {
                                    inventoryItem._int += 4;
                                }
                                else if (inventoryItem._wis > 0)
                                {
                                    inventoryItem._wis += 4;
                                }
                                else if (inventoryItem._con > 0)
                                {
                                    inventoryItem._con += 4;
                                }
                                else if (inventoryItem._dex > 0)
                                {
                                    inventoryItem._dex += 4;
                                }
                                break;
                        }
                    }
                    else
                    {
                        string[] dmgParts = inventoryItem._weaponDmg.Split('m');
                        int minDmg = int.Parse(dmgParts[0]);
                        int maxDmg = int.Parse(dmgParts[1]);
                        switch (enchantment)
                        {
                            default:
                                                break;
                            case "Good":
                            case "Fine":
                            case "Grand":
                                break;
                        }
                        inventoryItem._weaponDmg = minDmg + "m" + maxDmg;
                    }
                }
            }
            inventoryItem._amount = amount;
            if (durability > 0)
            {
                inventoryItem._durability = durability;
            }
            _inventorySlots[slot - 1]._item = inventoryItem;
            _inventory.Add(inventoryItem);
            if (save)
            {
                SaveItems();
            }
            return true;
        }
        return false;
    }

    /// <summary>
    ///     Adds the named skill to the player's skill book (into <paramref name="slot"/> or the first
    ///     free one): looks it up in the skills DB, reads its animation/cooldown/damage/range/message/
    ///     requirement fields (with defaults), builds the Skill, and saves. No-op + false if unknown or known.
    /// </summary>
    private bool NewSkill(string name, int slot = 0, byte level = 0, bool save = true)
    {
        if (slot == 0)
        {
            slot = FirstAvailableSkillSlot();
        }
        if (slot == 0)
        {
            return false;
        }
        foreach (JToken skillEntry in _skillsDB["skills"].Children())
        {
            string entryName = skillEntry.Value<string>("name");
            if (entryName.Equals(name, StringComparison.CurrentCultureIgnoreCase))
            {
                if (HasSkill(name))
                {
                    SystemMsg("You already know that skill.", 3);
                    return false;
                }
                string imgPath = skillEntry.Value<string>("imgpath");
                int frame = skillEntry.Value<int>("image");
                double cooldown = skillEntry.Value<double>("cooldown");
                if (cooldown == 0.0)
                {
                    cooldown = ((!entryName.Equals("assail", StringComparison.CurrentCultureIgnoreCase)) ? 0.333 : 0.6);
                }
                if (string.IsNullOrEmpty(imgPath))
                {
                    imgPath = "skill001";
                }
                int toani = skillEntry.Value<int>("toani");
                int toAniSpeed = skillEntry.Value<int>("toanispeed");
                if (toAniSpeed == 0)
                {
                    toAniSpeed = 120;
                }
                int fromani = skillEntry.Value<int>("fromani");
                int fromAniSpeed = skillEntry.Value<int>("fromanispeed");
                if (fromAniSpeed == 0)
                {
                    fromAniSpeed = 120;
                }
                string targetType = skillEntry.Value<string>("targettype");
                if (string.IsNullOrEmpty(targetType))
                {
                    targetType = "facing";
                }
                byte range = skillEntry.Value<byte>("range");
                if (range == 0)
                {
                    range = 1;
                }
                int baseDmg = skillEntry.Value<int>("dmg");
                int seconds = skillEntry.Value<int>("seconds");
                string startMsg = skillEntry.Value<string>("startmsg");
                string endMsg = skillEntry.Value<string>("endmsg");
                if (string.IsNullOrEmpty(startMsg))
                {
                    startMsg = "";
                }
                if (string.IsNullOrEmpty(endMsg))
                {
                    endMsg = "";
                }
                string reMsg = skillEntry.Value<string>("remsg");
                if (string.IsNullOrEmpty(reMsg))
                {
                    reMsg = "";
                }
                string soundStr = skillEntry.Value<string>("sound");
                byte sound = ((!string.IsNullOrEmpty(soundStr)) ? byte.Parse(soundStr) : byte.MaxValue);
                string element = skillEntry.Value<string>("element");
                if (string.IsNullOrEmpty(element))
                {
                    element = "None";
                }
                byte bodyAniType = skillEntry.Value<byte>("bodyanitype");
                ushort bodyAniSpeed = skillEntry.Value<ushort>("bodyanispeed");
                if (bodyAniSpeed == 0)
                {
                    bodyAniSpeed = 200;
                }
                bool checkIfHidden = skillEntry.Value<bool>("checkifhidden");
                bool castMsg = skillEntry.Value<bool>("castmsg");
                bool checkSpellBar = skillEntry.Value<bool>("checkspellbar");
                bool allowRefresh = skillEntry.Value<bool>("allowrefresh");
                byte projType = skillEntry.Value<byte>("projtype");
                string desc = skillEntry.Value<string>("desc");
                if (string.IsNullOrEmpty(desc))
                {
                    desc = "";
                }
                string status = skillEntry.Value<string>("status");
                if (string.IsNullOrEmpty(status))
                {
                    status = "";
                }
                int manaCost = skillEntry.Value<int>("mp");
                JToken reqs = skillEntry["reqs"];
                byte levReq = 0;
                string statsReq = "";
                if (reqs != null)
                {
                    levReq = reqs.Value<byte>("lev");
                    statsReq = reqs.Value<string>("stats");
                }
                if (levReq == 0)
                {
                    levReq = 1;
                }
                if (string.IsNullOrEmpty(statsReq))
                {
                    statsReq = "3,3,3,3,3";
                }
                Skill skill = new Skill(_textureManager, entryName, _slots[slot - 1], level, imgPath, frame, cooldown, sound, fromani, fromAniSpeed, toani, toAniSpeed, checkIfHidden, castMsg, checkSpellBar, allowRefresh, seconds, startMsg, reMsg, endMsg, bodyAniType, bodyAniSpeed, baseDmg, element, targetType, status);
                skill._projType = projType;
                skill._range = range;
                skill._description = desc;
                skill._manaCost = manaCost;
                skill._levReq = levReq;
                skill._statsReq = statsReq;
                _skills.Add(skill);
                if (save)
                {
                    SaveSkill();
                }
                return true;
            }
        }
        return false;
    }

    /// <summary>Adds the named spell to the spell book (given slot or first free): reads its DB fields
    /// (incl. heal / remove-status / mana / requirements), builds the Spell, and saves. No-op + false if unknown/known.</summary>
    private bool NewSpell(string name, int slot = 0, byte level = 0, bool save = true)
    {
        if (slot == 0)
        {
            slot = FirstAvailableSpellSlot();
        }
        if (slot == 0)
        {
            return false;
        }
        foreach (JToken spellEntry in _spellsDB["spells"].Children())
        {
            string entryName = spellEntry.Value<string>("name");
            if (!entryName.Equals(name, StringComparison.CurrentCultureIgnoreCase))
            {
                continue;
            }
            if (HasSpell(name))
            {
                SystemMsg("You already know that spell.", 3);
                return false;
            }
            string imgPath = spellEntry.Value<string>("imgpath");
            int frame = spellEntry.Value<int>("image");
            double cooldown = spellEntry.Value<double>("cooldown");
            if (cooldown == 0.0)
            {
                cooldown = 0.333;
            }
            if (string.IsNullOrEmpty(imgPath))
            {
                imgPath = "spell001";
            }
            int toani = spellEntry.Value<int>("toani");
            int toanispeed = 120;
            int fromani = spellEntry.Value<int>("fromani");
            int fromanispeed = 120;
            string targetType = spellEntry.Value<string>("targettype");
            if (string.IsNullOrEmpty(targetType))
            {
                targetType = "self";
            }
            int seconds = spellEntry.Value<int>("seconds");
            string startMsg = spellEntry.Value<string>("startmsg");
            if (string.IsNullOrEmpty(startMsg))
            {
                startMsg = "";
            }
            string endMsg = spellEntry.Value<string>("endmsg");
            if (string.IsNullOrEmpty(endMsg))
            {
                endMsg = "";
            }
            string reMsg = spellEntry.Value<string>("remsg");
            if (string.IsNullOrEmpty(reMsg))
            {
                reMsg = "";
            }
            string status = spellEntry.Value<string>("status");
            if (string.IsNullOrEmpty(status))
            {
                status = "";
            }
            string desc = spellEntry.Value<string>("desc");
            if (string.IsNullOrEmpty(desc))
            {
                desc = "";
            }
            string soundStr = spellEntry.Value<string>("sound");
            byte sound = ((!string.IsNullOrEmpty(soundStr)) ? byte.Parse(soundStr) : byte.MaxValue);
            int baseDmg = spellEntry.Value<int>("dmg");
            string element = spellEntry.Value<string>("element");
            if (string.IsNullOrEmpty(element))
            {
                element = "None";
            }
            byte bodyAniType = spellEntry.Value<byte>("bodyanitype");
            if (bodyAniType == 0)
            {
                bodyAniType = 2;
            }
            ushort bodyAniSpeed = spellEntry.Value<ushort>("bodyanispeed");
            if (bodyAniSpeed == 0)
            {
                bodyAniSpeed = 200;
            }
            if (bodyAniType == 2)
            {
                bodyAniSpeed = 350;
            }
            bool checkIfHidden = spellEntry.Value<bool>("checkifhidden");
            bool castMsg = spellEntry.Value<bool>("castmsg");
            bool checkSpellBar = spellEntry.Value<bool>("checkspellbar");
            bool allowRefresh = spellEntry.Value<bool>("allowrefresh");
            int heal = spellEntry.Value<int>("heal");
            byte range = spellEntry.Value<byte>("range");
            if (range == 0)
            {
                range = 1;
            }
            int manaCost = spellEntry.Value<int>("mp");
            JToken reqs = spellEntry["reqs"];
            byte levReq = 0;
            string statsReq = "";
            if (reqs != null)
            {
                levReq = reqs.Value<byte>("lev");
                statsReq = reqs.Value<string>("stats");
            }
            if (levReq == 0)
            {
                levReq = 1;
            }
            if (string.IsNullOrEmpty(statsReq))
            {
                statsReq = "3,3,3,3,3";
            }
            JToken removeStatusNode = spellEntry["removestatus"];
            List<string> removeStatus = new List<string>();
            if (removeStatusNode != null)
            {
                foreach (JProperty statusProp in removeStatusNode.Children())
                {
                    string statusName = statusProp.Value.ToString();
                    removeStatus.Add(statusName);
                }
            }
            bool successOnly = spellEntry.Value<bool>("successonly");
            Spell spell = new Spell(_textureManager, entryName, _slots[slot - 1], level, imgPath, frame, cooldown, sound, fromani, fromanispeed, toani, toanispeed, checkIfHidden, castMsg, checkSpellBar, allowRefresh, seconds, startMsg, reMsg, endMsg, bodyAniType, bodyAniSpeed, baseDmg, element, heal, targetType, status);
            spell._successOnly = successOnly;
            spell._removeStatus = removeStatus;
            spell._description = desc;
            spell._range = range;
            spell._manaCost = manaCost;
            spell._levReq = levReq;
            spell._statsReq = statsReq;
            _spells.Add(spell);
            if (save)
            {
                SaveSpell();
            }
            return true;
        }
        return false;
    }

    /// <summary>Adds the named action to the action book (given slot or first free): reads its DB
    /// fields, builds the Action, and saves. No-op + false if unknown or already known.</summary>
    private bool NewAction(string name, int slot = 0, byte level = 0, bool save = true)
    {
        if (slot == 0)
        {
            slot = FirstAvailableActionSlot();
        }
        if (slot == 0)
        {
            return false;
        }
        foreach (JToken actionEntry in _actionsDB["actions"].Children())
        {
            string entryName = actionEntry.Value<string>("name");
            if (entryName.Equals(name, StringComparison.CurrentCultureIgnoreCase))
            {
                if (HasAction(name))
                {
                    SystemMsg("You already know that action.", 3);
                    return false;
                }
                string imgPath = actionEntry.Value<string>("imgpath");
                int frame = actionEntry.Value<int>("image");
                if (string.IsNullOrEmpty(imgPath))
                {
                    imgPath = "skill001";
                }
                double cooldown = actionEntry.Value<double>("cooldown");
                if (cooldown == 0.0)
                {
                    cooldown = 0.333;
                }
                string source = actionEntry.Value<string>("source");
                if (string.IsNullOrEmpty(source))
                {
                    source = "old";
                }
                int toani = actionEntry.Value<int>("toani");
                int toAniSpeed = actionEntry.Value<int>("toanispeed");
                if (toAniSpeed == 0)
                {
                    toAniSpeed = 120;
                }
                int fromani = actionEntry.Value<int>("fromani");
                int fromAniSpeed = actionEntry.Value<int>("fromanispeed");
                if (fromAniSpeed == 0)
                {
                    fromAniSpeed = 120;
                }
                string targetType = actionEntry.Value<string>("targettype");
                if (string.IsNullOrEmpty(targetType))
                {
                    targetType = "self";
                }
                byte range = actionEntry.Value<byte>("range");
                if (range == 0)
                {
                    range = 1;
                }
                int baseDmg = actionEntry.Value<int>("dmg");
                int seconds = actionEntry.Value<int>("seconds");
                string startMsg = actionEntry.Value<string>("startmsg");
                string endMsg = actionEntry.Value<string>("endmsg");
                if (string.IsNullOrEmpty(startMsg))
                {
                    startMsg = "";
                }
                if (string.IsNullOrEmpty(endMsg))
                {
                    endMsg = "";
                }
                string soundStr = actionEntry.Value<string>("sound");
                byte sound = ((!string.IsNullOrEmpty(soundStr)) ? byte.Parse(soundStr) : byte.MaxValue);
                string element = actionEntry.Value<string>("element");
                if (string.IsNullOrEmpty(element))
                {
                    element = "None";
                }
                byte bodyAniType = actionEntry.Value<byte>("bodyanitype");
                ushort bodyAniSpeed = actionEntry.Value<ushort>("bodyanispeed");
                if (bodyAniSpeed == 0)
                {
                    bodyAniSpeed = 200;
                }
                bool checkIfHidden = actionEntry.Value<bool>("checkifhidden");
                bool castMsg = actionEntry.Value<bool>("castmsg");
                bool checkSpellBar = actionEntry.Value<bool>("checkspellbar");
                bool allowRefresh = actionEntry.Value<bool>("allowrefresh");
                string desc = actionEntry.Value<string>("desc");
                if (string.IsNullOrEmpty(desc))
                {
                    desc = "";
                }
                string status = actionEntry.Value<string>("status");
                if (string.IsNullOrEmpty(status))
                {
                    status = "";
                }
                int manaCost = actionEntry.Value<int>("mp");
                JToken reqs = actionEntry["reqs"];
                byte levReq = 0;
                string statsReq = "";
                if (reqs != null)
                {
                    levReq = reqs.Value<byte>("lev");
                    statsReq = reqs.Value<string>("stats");
                }
                if (levReq == 0)
                {
                    levReq = 1;
                }
                if (string.IsNullOrEmpty(statsReq))
                {
                    statsReq = "3,3,3,3,3";
                }
                Action action = new Action(_textureManager, entryName, _slots[slot - 1], level, imgPath, frame, source, cooldown, sound, fromani, fromAniSpeed, toani, toAniSpeed, checkIfHidden, castMsg, checkSpellBar, allowRefresh, seconds, startMsg, endMsg, bodyAniType, bodyAniSpeed, baseDmg, element, targetType, status);
                action._description = desc;
                action._range = range;
                action._manaCost = manaCost;
                action._levReq = levReq;
                action._statsReq = statsReq;
                _actions.Add(action);
                if (save)
                {
                    SaveAction();
                }
                return true;
            }
        }
        return false;
    }

    public void NewSpellBar(string name, string type, int frame, double seconds, string startMsg, string reMsg, string endMsg, Entity entity)
    {
        if (entity._spellBar.ContainsKey(name))
        {
            entity._spellBar[name].Remove(replace: true);
        }
        entity._spellBar.Add(name, new SpellBar(this, _textureManager, _font, name, AvailableSlot(entity._spellBar.Values.ToList()), type, frame, seconds, startMsg, reMsg, endMsg, entity));
    }

    /// <summary>
    ///     Remaps a "MYDA"-set sprite/appendage number to the equivalent in this client's sprite set,
    ///     adjusting for gender (female bodies use a different range). Returns <paramref name="num"/>
    ///     unchanged when it has no mapping. The big switch is a hand-authored lookup table.
    /// </summary>
    private int ConvertMydaNum(int num, int gender = 1)
    {
        int result = num;
        if (_player._gender == 0 || gender == 0)
        {
            switch (num)
            {
                case 1:
                    result = 44;
                    break;
                case 2:
                    result = 45;
                    break;
                case 3:
                    result = 46;
                    break;
                case 4:
                    result = 47;
                    break;
                case 5:
                    result = 48;
                    break;
                case 6:
                    result = 49;
                    break;
                case 7:
                    result = 50;
                    break;
                case 8:
                    result = 51;
                    break;
                case 9:
                    result = 52;
                    break;
                case 10:
                    result = 53;
                    break;
                case 11:
                    result = 63;
                    break;
                case 12:
                    result = 55;
                    break;
                case 13:
                    result = 64;
                    break;
                case 14:
                    result = 57;
                    break;
                case 15:
                    result = 56;
                    break;
                case 16:
                    result = 54;
                    break;
                case 17:
                    result = 40;
                    break;
                case 19:
                    result = 61;
                    break;
                case 40:
                    result = 62;
                    break;
                case 41:
                    result = 41;
                    break;
                case 44:
                    result = 1;
                    break;
                case 45:
                    result = 2;
                    break;
                case 46:
                    result = 3;
                    break;
                case 47:
                    result = 4;
                    break;
                case 48:
                    result = 5;
                    break;
                case 49:
                    result = 6;
                    break;
                case 50:
                    result = 7;
                    break;
                case 51:
                    result = 8;
                    break;
                case 52:
                    result = 9;
                    break;
                case 53:
                    result = 10;
                    break;
                case 54:
                    result = 11;
                    break;
                case 55:
                    result = 12;
                    break;
                case 56:
                    result = 13;
                    break;
                case 57:
                    result = 14;
                    break;
                case 58:
                    result = 15;
                    break;
                case 59:
                    result = 16;
                    break;
                case 60:
                    result = 17;
                    break;
                case 61:
                case 62:
                case 63:
                case 64:
                case 65:
                case 66:
                case 67:
                case 68:
                case 69:
                case 70:
                case 71:
                case 72:
                case 73:
                case 74:
                case 75:
                case 76:
                case 77:
                case 78:
                case 79:
                case 80:
                case 81:
                case 82:
                case 83:
                case 84:
                case 85:
                case 86:
                case 87:
                case 88:
                case 89:
                case 90:
                case 91:
                case 92:
                case 93:
                case 94:
                case 95:
                case 96:
                case 97:
                case 98:
                case 99:
                case 100:
                case 101:
                    result = 999;
                    break;
                default:
                    switch (num)
                    {
                        case 101:
                            result = 65;
                            break;
                        case 102:
                            result = 66;
                            break;
                        case 103:
                            result = 999;
                            break;
                        case 104:
                            result = 67;
                            break;
                        case 105:
                            result = 68;
                            break;
                        case 106:
                            result = 69;
                            break;
                        case 107:
                            result = 999;
                            break;
                        case 108:
                            result = 70;
                            break;
                        case 109:
                        case 110:
                            result = 999;
                            break;
                        default:
                            switch (num)
                            {
                                case 111:
                                    result = 71;
                                    break;
                                case 112:
                                    result = 72;
                                    break;
                                case 113:
                                    result = 73;
                                    break;
                                case 114:
                                    result = 74;
                                    break;
                                case 115:
                                    result = 75;
                                    break;
                                case 116:
                                    result = 76;
                                    break;
                                case 117:
                                case 118:
                                case 119:
                                case 120:
                                    result = 999;
                                    break;
                                default:
                                    switch (num)
                                    {
                                        case 121:
                                            result = 77;
                                            break;
                                        case 122:
                                            result = 78;
                                            break;
                                        case 123:
                                            result = 79;
                                            break;
                                        case 124:
                                            result = 80;
                                            break;
                                        case 125:
                                            result = 81;
                                            break;
                                        case 126:
                                            result = 82;
                                            break;
                                        case 127:
                                            result = 83;
                                            break;
                                        case 128:
                                            result = 84;
                                            break;
                                        case 129:
                                            result = 85;
                                            break;
                                        case 130:
                                            result = 999;
                                            break;
                                        case 131:
                                            result = 86;
                                            break;
                                        case 132:
                                            result = 87;
                                            break;
                                        case 133:
                                            result = 88;
                                            break;
                                        case 134:
                                            result = 89;
                                            break;
                                        case 135:
                                            result = 90;
                                            break;
                                        case 136:
                                            result = 91;
                                            break;
                                        case 137:
                                            result = 92;
                                            break;
                                        case 138:
                                            result = 93;
                                            break;
                                        case 139:
                                            result = 999;
                                            break;
                                        case 140:
                                            result = 999;
                                            break;
                                        case 141:
                                            result = 94;
                                            break;
                                        case 142:
                                            result = 95;
                                            break;
                                        case 143:
                                            result = 96;
                                            break;
                                        case 144:
                                            result = 97;
                                            break;
                                        case 145:
                                            result = 98;
                                            break;
                                        case 146:
                                            result = 99;
                                            break;
                                        case 147:
                                            result = 100;
                                            break;
                                        case 148:
                                            result = 101;
                                            break;
                                        case 149:
                                            result = 102;
                                            break;
                                        case 150:
                                            result = 999;
                                            break;
                                        case 151:
                                            result = 103;
                                            break;
                                        case 152:
                                            result = 104;
                                            break;
                                        case 153:
                                            result = 105;
                                            break;
                                        case 154:
                                            result = 106;
                                            break;
                                        case 155:
                                            result = 107;
                                            break;
                                        case 156:
                                            result = 108;
                                            break;
                                        case 157:
                                            result = 109;
                                            break;
                                        case 158:
                                            result = 110;
                                            break;
                                        case 159:
                                            result = 111;
                                            break;
                                        case 160:
                                            result = 112;
                                            break;
                                        case 161:
                                            result = 19;
                                            break;
                                        case 162:
                                            result = 113;
                                            break;
                                        case 163:
                                            result = 114;
                                            break;
                                        case 164:
                                            result = 18;
                                            break;
                                        case 165:
                                            result = 115;
                                            break;
                                        case 166:
                                            result = 116;
                                            break;
                                        case 167:
                                            result = 117;
                                            break;
                                        case 168:
                                            result = 118;
                                            break;
                                        case 169:
                                            result = 119;
                                            break;
                                        case 170:
                                            result = 120;
                                            break;
                                        case 171:
                                            result = 121;
                                            break;
                                        case 172:
                                            result = 122;
                                            break;
                                        case 173:
                                            result = 123;
                                            break;
                                        case 174:
                                            result = 124;
                                            break;
                                        case 175:
                                            result = 125;
                                            break;
                                        case 176:
                                            result = 126;
                                            break;
                                        case 177:
                                            result = 127;
                                            break;
                                        case 205:
                                            result = 128;
                                            break;
                                        case 207:
                                            result = 129;
                                            break;
                                        case 209:
                                            result = 130;
                                            break;
                                        case 211:
                                            result = 131;
                                            break;
                                        case 213:
                                            result = 132;
                                            break;
                                        case 215:
                                            result = 133;
                                            break;
                                        case 219:
                                            result = 134;
                                            break;
                                        case 220:
                                            result = 135;
                                            break;
                                        case 221:
                                            result = 136;
                                            break;
                                        case 223:
                                            result = 137;
                                            break;
                                        case 503:
                                            result = 195;
                                            break;
                                        case 504:
                                            result = 196;
                                            break;
                                        case 505:
                                            result = 197;
                                            break;
                                        case 506:
                                            result = 198;
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
            }
            return result;
        }
        switch (num)
        {
            case 1:
                result = 44;
                break;
            case 2:
                result = 45;
                break;
            case 3:
                result = 46;
                break;
            case 4:
                result = 47;
                break;
            case 5:
                result = 48;
                break;
            case 6:
                result = 49;
                break;
            case 7:
                result = 50;
                break;
            case 8:
                result = 51;
                break;
            case 9:
                result = 52;
                break;
            case 10:
                result = 53;
                break;
            case 11:
                result = 54;
                break;
            case 12:
                result = 55;
                break;
            case 13:
                result = 56;
                break;
            case 14:
                result = 57;
                break;
            case 15:
                result = 41;
                break;
            case 16:
                result = 40;
                break;
            case 17:
                result = 19;
                break;
            case 19:
                result = 73;
                break;
            case 40:
                result = 74;
                break;
            case 41:
                result = 75;
                break;
            case 44:
                result = 1;
                break;
            case 45:
                result = 2;
                break;
            case 46:
                result = 3;
                break;
            case 47:
                result = 4;
                break;
            case 48:
                result = 5;
                break;
            case 49:
                result = 6;
                break;
            case 50:
                result = 7;
                break;
            case 51:
                result = 8;
                break;
            case 52:
                result = 9;
                break;
            case 53:
                result = 10;
                break;
            case 54:
                result = 11;
                break;
            case 55:
                result = 12;
                break;
            case 56:
                result = 13;
                break;
            case 57:
                result = 14;
                break;
            case 58:
                result = 15;
                break;
            case 59:
                result = 16;
                break;
            case 60:
                result = 17;
                break;
            case 61:
            case 62:
            case 63:
            case 64:
            case 65:
            case 66:
            case 67:
            case 68:
            case 69:
            case 70:
            case 71:
            case 72:
            case 73:
            case 74:
            case 75:
            case 76:
            case 77:
            case 78:
            case 79:
            case 80:
            case 81:
            case 82:
            case 83:
            case 84:
            case 85:
            case 86:
            case 87:
            case 88:
            case 89:
            case 90:
            case 91:
            case 92:
            case 93:
            case 94:
            case 95:
            case 96:
            case 97:
            case 98:
            case 99:
            case 100:
            case 101:
                result = 999;
                break;
            default:
                switch (num)
                {
                    case 102:
                        result = 61;
                        break;
                    case 103:
                        result = 62;
                        break;
                    case 104:
                        result = 63;
                        break;
                    case 105:
                        result = 64;
                        break;
                    case 106:
                        result = 65;
                        break;
                    case 107:
                        result = 999;
                        break;
                    case 108:
                        result = 66;
                        break;
                    case 109:
                    case 110:
                        result = 999;
                        break;
                    default:
                        switch (num)
                        {
                            case 111:
                                result = 67;
                                break;
                            case 112:
                                result = 68;
                                break;
                            case 113:
                                result = 69;
                                break;
                            case 114:
                                result = 70;
                                break;
                            case 115:
                                result = 71;
                                break;
                            case 116:
                                result = 72;
                                break;
                            case 117:
                            case 118:
                            case 119:
                            case 120:
                                result = 999;
                                break;
                            default:
                                switch (num)
                                {
                                    case 121:
                                        result = 76;
                                        break;
                                    case 122:
                                        result = 77;
                                        break;
                                    case 123:
                                        result = 78;
                                        break;
                                    case 124:
                                        result = 79;
                                        break;
                                    case 125:
                                        result = 80;
                                        break;
                                    case 126:
                                        result = 81;
                                        break;
                                    case 127:
                                        result = 82;
                                        break;
                                    case 128:
                                        result = 83;
                                        break;
                                    case 129:
                                        result = 84;
                                        break;
                                    case 130:
                                        result = 999;
                                        break;
                                    case 131:
                                        result = 85;
                                        break;
                                    case 132:
                                        result = 86;
                                        break;
                                    case 133:
                                        result = 87;
                                        break;
                                    case 134:
                                        result = 88;
                                        break;
                                    case 135:
                                        result = 89;
                                        break;
                                    case 136:
                                        result = 90;
                                        break;
                                    case 137:
                                        result = 91;
                                        break;
                                    case 138:
                                        result = 92;
                                        break;
                                    case 139:
                                        result = 93;
                                        break;
                                    case 140:
                                        result = 999;
                                        break;
                                    case 141:
                                        result = 94;
                                        break;
                                    case 142:
                                        result = 95;
                                        break;
                                    case 143:
                                        result = 96;
                                        break;
                                    case 144:
                                        result = 97;
                                        break;
                                    case 145:
                                        result = 98;
                                        break;
                                    case 146:
                                        result = 99;
                                        break;
                                    case 147:
                                        result = 100;
                                        break;
                                    case 148:
                                        result = 101;
                                        break;
                                    case 149:
                                        result = 102;
                                        break;
                                    case 150:
                                        result = 999;
                                        break;
                                    case 151:
                                        result = 103;
                                        break;
                                    case 152:
                                        result = 104;
                                        break;
                                    case 158:
                                        result = 105;
                                        break;
                                    case 159:
                                        result = 106;
                                        break;
                                    case 160:
                                        result = 999;
                                        break;
                                    case 161:
                                        result = 107;
                                        break;
                                    case 162:
                                        result = 108;
                                        break;
                                    case 163:
                                        result = 109;
                                        break;
                                    case 164:
                                        result = 110;
                                        break;
                                    case 165:
                                        result = 111;
                                        break;
                                    case 166:
                                        result = 112;
                                        break;
                                    case 167:
                                        result = 113;
                                        break;
                                    case 168:
                                        result = 114;
                                        break;
                                    case 169:
                                        result = 115;
                                        break;
                                    case 170:
                                        result = 116;
                                        break;
                                    case 171:
                                        result = 117;
                                        break;
                                    case 172:
                                        result = 118;
                                        break;
                                    case 176:
                                        result = 119;
                                        break;
                                    case 177:
                                        result = 120;
                                        break;
                                    case 205:
                                        result = 121;
                                        break;
                                    case 207:
                                        result = 122;
                                        break;
                                    case 209:
                                        result = 123;
                                        break;
                                    case 211:
                                        result = 124;
                                        break;
                                    case 213:
                                        result = 125;
                                        break;
                                    case 215:
                                        result = 126;
                                        break;
                                    case 219:
                                        result = 127;
                                        break;
                                    case 220:
                                        result = 128;
                                        break;
                                    case 221:
                                        result = 129;
                                        break;
                                    case 223:
                                        result = 130;
                                        break;
                                    case 254:
                                        result = 999;
                                        break;
                                    case 255:
                                        result = 999;
                                        break;
                                    case 501:
                                        result = 131;
                                        break;
                                    case 502:
                                        result = 132;
                                        break;
                                    case 503:
                                        result = 133;
                                        break;
                                    case 504:
                                        result = 134;
                                        break;
                                    case 505:
                                        result = 135;
                                        break;
                                    case 506:
                                        result = 136;
                                        break;
                                    case 507:
                                        result = 254;
                                        break;
                                    case 508:
                                        result = 255;
                                        break;
                                }
                                break;
                        }
                        break;
                }
                break;
        }
        return result;
    }

    #region Removal

    /// <summary>Removes entity <paramref name="id"/> from its tile and the map's entity table.</summary>
    private void RemoveEntity(uint id, Tile t)
    {
        t._entities.Remove(id);
        _map._entities.Remove(id);
    }

    /// <summary>
    ///     Un-equips the gear in <paramref name="slot"/>: resets the matching body-sprite appendage
    ///     (s/l/w/h/u/c), subtracts its combined stat bonuses from the player, clears the slot, and saves.
    /// </summary>
    private void RemoveEquip(int slot)
    {
        InventoryItem[] equipped = _equipment.ToArray();
        foreach (InventoryItem equip in equipped)
        {
            if (equip._slot != slot)
            {
                continue;
            }
            string appendage = "";
            if (equip._tab == "Shield")
            {
                appendage = "s";
            }
            else if (equip._tab == "Boots")
            {
                appendage = "l";
            }
            else if (equip._tab == "Weapon")
            {
                appendage = "w";
            }
            else if (equip._tab.StartsWith("Helmet"))
            {
                appendage = "h";
            }
            else if (equip._tab.StartsWith("Armor"))
            {
                appendage = "u";
            }
            else if (equip._tab == "Accessory")
            {
                appendage = "c";
            }
            if (appendage == "u")
            {
                _player._body._bodyImgs["a"] = 1;
            }
            if (appendage == "c")
            {
                _player._body._bodyImgs["g"] = 0;
            }
            if (appendage == "h")
            {
                _player._body._helmType = 0;
                _player._body._helmColor = 0;
                _player._body._bodyImgs[appendage] = _player._body._hairType;
                if (equip._bodyImgColor != 0)
                {
                    _player._body._bodyColors[appendage] = _player._body._hairColor;
                }
            }
            else
            {
                _player._body._bodyImgs[appendage] = 0;
                if (equip._bodyImgColor != 0)
                {
                    _player._body._bodyColors[appendage] = 0;
                }
            }
            if (equip._hp != 0)
            {
                _player._combinedHPBonus -= equip._hp;
            }
            if (equip._mp != 0)
            {
                _player._combinedMPBonus -= equip._mp;
            }
            if (equip._str != 0)
            {
                _player._str -= (ushort)equip._str;
            }
            if (equip._int != 0)
            {
                _player._int -= (ushort)equip._int;
            }
            if (equip._wis != 0)
            {
                _player._wis -= (ushort)equip._wis;
            }
            if (equip._con != 0)
            {
                _player._con -= (ushort)equip._con;
            }
            if (equip._dex != 0)
            {
                _player._dex -= (ushort)equip._dex;
            }
            if (equip._ac != 0)
            {
                Player player = _player;
                player._ac -= (byte)equip._ac;
            }
            if (equip._mr != 0)
            {
                _player._mr -= (byte)equip._mr;
            }
            if (equip._hit != 0)
            {
                _player._hit -= (byte)equip._hit;
            }
            if (equip._dmg != 0)
            {
                _player._dmg -= (byte)equip._dmg;
            }
            if (equip._reg != 0)
            {
                _player._reg -= (byte)equip._reg;
            }
            if (!string.IsNullOrEmpty(equip._atk))
            {
                _player._atk = "None";
            }
            if (!string.IsNullOrEmpty(equip._def))
            {
                _player._def = "None";
            }
            if (_player._curHP >= _player._baseHP && _player._baseHP < _player._maxHP)
            {
                _player._curHP = _player._maxHP;
            }
            if (_player._curHP > _player._maxHP)
            {
                _player._curHP = _player._maxHP;
            }
            if (_player._curMP >= _player._baseMP && _player._baseMP < _player._maxMP)
            {
                _player._curMP = _player._maxMP;
            }
            if (_player._curMP > _player._maxMP)
            {
                _player._curMP = _player._maxMP;
            }
            _equipmentSlots[slot - 1]._item = null;
            _equipment.Remove(equip);
            _player._body.setDefault();
            SaveEquip();
            break;
        }
    }

    /// <summary>Removes (or decrements by <paramref name="dropAmount"/>) the inventory item in <paramref name="slot"/>; never removes the Gold slot (72).</summary>
    private void RemoveItem(int slot, int dropAmount = 0)
    {
        foreach (InventoryItem item in _inventory)
        {
            if (item._slot == slot)
            {
                if (dropAmount >= 1 && dropAmount < item._amount)
                {
                    item._amount -= dropAmount;
                }
                else if (slot != 72)
                {
                    _inventorySlots[slot - 1]._item = null;
                    _inventory.Remove(item);
                }
                SaveItems();
                break;
            }
        }
    }

    /// <summary>Removes the skill in the given slot from the skill book.</summary>
    private void RemoveSkill(int slot)
    {
        Skill[] skills = _skills.ToArray();
        foreach (Skill skill in skills)
        {
            if (skill._slot == slot)
            {
                _skills.Remove(skill);
                SaveSkill();
                break;
            }
        }
    }

    /// <summary>Removes the spell in the given slot from the spell book.</summary>
    private void RemoveSpell(int slot)
    {
        Spell[] spells = _spells.ToArray();
        foreach (Spell spell in spells)
        {
            if (spell._slot == slot)
            {
                _spells.Remove(spell);
                SaveSpell();
                break;
            }
        }
    }

    /// <summary>Removes the action in the given slot from the action book.</summary>
    private void RemoveAction(int slot)
    {
        Action[] actions = _actions.ToArray();
        foreach (Action action in actions)
        {
            if (action._slot == slot)
            {
                _actions.Remove(action);
                SaveAction();
                break;
            }
        }
    }

    #endregion

    #region Item / skill / spell / action use

    /// <summary>
    ///     Uses the inventory item in <paramref name="slot"/> (rate-limited by <c>_itemCDConst</c>):
    ///     equips it when it's gear, otherwise runs its consumable script
    ///     (fullHeal/hemloch/confetti/rock/equipdye) and decrements the stack. Unknown items report
    ///     "has no use".
    /// </summary>
    private void UseItem(int slot)
    {
        if (!(DateTime.UtcNow.Subtract(_itemUseKeysDelay).TotalMilliseconds >= _itemCDConst))
        {
            return;
        }
        string itemName = "";
        InventoryItem[] inventory = _inventory.ToArray();
        foreach (InventoryItem inventoryItem in inventory)
        {
            if (inventoryItem._slot != slot)
            {
                continue;
            }
            itemName = inventoryItem._name;
            string scriptName = "";
            switch (itemName)
            {
                case "Hydele Deum":
                    scriptName = "fullHeal";
                    break;
                case "Hemloch Deum":
                    scriptName = "hemloch";
                    break;
                case "Confetti":
                    scriptName = "confetti";
                    break;
                case "Rock":
                    scriptName = "rock";
                    break;
                default:
                    if (itemName.EndsWith("Equipment Dye"))
                    {
                        scriptName = "equipdye";
                        dyeingequipment = true;
                        selectedequipmentdye = inventoryItem._bodyImgColor;
                        selectedequipmentdyeslot = inventoryItem._slot;
                    }
                    break;
            }
            if (inventoryItem._maxDurability > 0)
            {
                NewEquip(itemName, inventoryItem._tab, inventoryItem._durability, inventoryItem._bodyImgColor, inventoryItem);
            }
            else if (scriptName != "")
            {
                if (scriptName != "equipdye")
                {
                    if (inventoryItem._amount > 1)
                    {
                        inventoryItem._amount--;
                    }
                    else
                    {
                        _inventory.Remove(inventoryItem);
                    }
                    SaveItems();
                }
                Scripts(scriptName);
            }
            else
            {
                SystemMsg(itemName + " has no use.", 3);
            }
            break;
        }
        _itemUseKeysDelay = DateTime.UtcNow;
    }

    private void UseSkill(int slot)
    {
        Skill skillSlot = getSkillSlot(slot);
        if (skillSlot != null)
        {
            UseScript(skillSlot);
        }
    }

    private void UseSpell(int slot)
    {
        Spell spellSlot = getSpellSlot(slot);
        if (spellSlot != null)
        {
            UseScript(spellSlot);
        }
    }

    private void UseAction(int slot)
    {
        Action actionSlot = getActionSlot(slot);
        if (actionSlot != null)
        {
            UseScript(actionSlot);
        }
    }

    /// <summary>Bumps a skill/spell/action's use-count and levels it up once it hits its improve rate.</summary>
    private void UseCountAndLevelUp(Skill s)
    {
        if (s._impRate > 0 && s._level < s._maxLevel)
        {
            s._useCount++;
            if (s._useCount == s._impRate)
            {
                s._level++;
                SystemMsg(s._name + " has improved!", 3);
            }
        }
    }

    private void UseCountAndLevelUp(Spell s)
    {
        if (s._impRate > 0 && s._level < s._maxLevel)
        {
            s._useCount++;
            if (s._useCount == s._impRate)
            {
                s._level++;
                SystemMsg(s._name + " has improved!", 3);
            }
        }
    }

    private void UseCountAndLevelUp(Action s)
    {
        if (s._impRate > 0 && s._level < s._maxLevel)
        {
            s._useCount++;
            if (s._useCount == s._impRate)
            {
                s._level++;
                SystemMsg(s._name + " has improved!", 3);
            }
        }
    }

    /// <summary>Plays a spell/skill's cast body-animation, sound effect, and self "from" animation.</summary>
    private void SoundBodyFromAni(Spell s)
    {
        BodyMovement(s._bodyAniType, s._bodyAniSpeed);
        if (s._sound != byte.MaxValue)
        {
            PlaySound("effect" + s._sound);
        }
        if (s._fromani > 0)
        {
            SpellAnimation(_player, s._fromani, s._fromanispeed);
        }
    }

    private void SoundBodyFromAni(Skill s)
    {
        BodyMovement(s._bodyAniType, s._bodyAniSpeed);
        if (s._sound != byte.MaxValue)
        {
            PlaySound("effect" + s._sound);
        }
        if (s._fromani > 0)
        {
            SpellAnimation(_player, s._fromani, s._fromanispeed);
        }
    }

    #endregion

    public void SpellBarCheck(Spell s, Entity e)
    {
        if (s._checkSpellBar || s._allowRefresh)
        {
            NewSpellBar((s._status != "") ? s._status : s._name, s._type, s._frame - 1, s._seconds, s._startMsg, s._reMsg, s._endMsg, e);
        }
    }

    private void SpellBarCheck(Skill s, Entity e)
    {
        if (s._checkSpellBar || s._allowRefresh)
        {
            NewSpellBar((s._status != "") ? s._status : s._name, s._type, s._frame - 1, s._seconds, s._startMsg, s._reMsg, s._endMsg, e);
        }
    }

    /// <summary>
    ///     Casts the currently-loaded spell on tile <paramref name="t"/>: plays its body/cast animations
    ///     and sound, special-cases "blink" (teleport), announces it, sets the cooldown highlight, and
    ///     clears the loaded spell. Rate-limited to 3 casts/second. (Mana is not deducted client-side.)
    /// </summary>
    private void CastLoadedSpellOnTile(Tile t)
    {
        if (DateTime.UtcNow.Subtract(lastSpellUse).TotalMilliseconds < 1000.0)
        {
            if (castCount >= 3)
            {
                return;
            }
            castCount++;
        }
        string spellName = _loadedSpell._name.ToLower();
        BodyMovement(_loadedSpell._bodyAniType, _loadedSpell._bodyAniSpeed);
        if (_loadedSpell._sound != byte.MaxValue)
        {
            PlaySound("effect" + _loadedSpell._sound);
        }
        if (_loadedSpell._fromani > 0)
        {
            SpellAnimation(_player, _loadedSpell._fromani, _loadedSpell._fromanispeed);
        }
        if (_loadedSpell._toani > 0)
        {
            t.SpellAni(_loadedSpell._toani, _loadedSpell._toanispeed);
        }
        if (spellName == "blink")
        {
            Teleport(t.Location.X, t.Location.Y);
        }
        if (_loadedSpell._castMsg)
        {
            SystemMsg("You cast " + _loadedSpell._name, 3);
        }
        if (!_GM && _loadedSpell._cooldown >= 1.0)
        {
            _loadedSpell._highlight = true;
        }
        _loadedSpell._lastSuccess = DateTime.UtcNow;
        _loadedSpell = null;
    }

    /// <summary>
    ///     Casts the currently loaded spell on entity <paramref name="e"/> (rate-limited to ~3 casts/sec).
    ///     Resolves the spell's status against the target: curse and amplify (fas) tiers only land when no
    ///     stronger one is present and they replace the weaker tier, while "remove" spells strip listed
    ///     statuses. On success it plays the body/sound/spell animations, applies damage or heal, shows the
    ///     cast message, and adds the status to the target's spell bar. Finally clears the loaded spell.
    /// </summary>
    private void CastLoadedSpellOnTarget(Entity e)
    {
        bool succeeded = false;
        if (DateTime.UtcNow.Subtract(lastSpellUse).TotalMilliseconds < 1000.0)
        {
            if (castCount >= 3)
            {
                return;
            }
            castCount++;
        }
        string spellName = _loadedSpell._name.ToLower();
        if (!_loadedSpell._checkSpellBar || !e._spellBar.ContainsKey(spellName))
        {
            // Curse stacking: a tier only lands if no stronger curse is present, replacing the weaker one.
            if (_loadedSpell._status.ToLower() == "curse1")
            {
                succeeded = true;
                if (e.hascradh)
                {
                    SystemMsg("Another curse afflicts thee. [cradh]", 3);
                    succeeded = false;
                }
                else if (e.hasmorcradh)
                {
                    SystemMsg("Another curse afflicts thee. [mor cradh]", 3);
                    succeeded = false;
                }
                else if (e.hasardcradh)
                {
                    SystemMsg("Another curse afflicts thee. [ard cradh]", 3);
                    succeeded = false;
                }
                else if (e.hasdiacradh)
                {
                    SystemMsg("Another curse afflicts thee. [dia cradh]", 3);
                    succeeded = false;
                }
            }
            else if (_loadedSpell._status.ToLower() == "curse2")
            {
                succeeded = true;
                if (e.hasbeagcradh)
                {
                    e._spellBar["Curse1"].Remove(replace: true);
                }
                else if (e.hasmorcradh)
                {
                    SystemMsg("Another curse afflicts thee. [mor cradh]", 3);
                    succeeded = false;
                }
                else if (e.hasardcradh)
                {
                    SystemMsg("Another curse afflicts thee. [ard cradh]", 3);
                    succeeded = false;
                }
                else if (e.hasdiacradh)
                {
                    SystemMsg("Another curse afflicts thee. [dia cradh]", 3);
                    succeeded = false;
                }
            }
            else if (_loadedSpell._status.ToLower() == "curse3")
            {
                succeeded = true;
                if (e.hasbeagcradh)
                {
                    e._spellBar["Curse1"].Remove(replace: true);
                }
                else if (e.hascradh)
                {
                    e._spellBar["Curse2"].Remove(replace: true);
                }
                else if (e.hasardcradh)
                {
                    SystemMsg("Another curse afflicts thee. [ard cradh]", 3);
                    succeeded = false;
                }
                else if (e.hasdiacradh)
                {
                    SystemMsg("Another curse afflicts thee. [dia cradh]", 3);
                    succeeded = false;
                }
            }
            else if (_loadedSpell._status.ToLower() == "curse4")
            {
                succeeded = true;
                if (e.hasbeagcradh)
                {
                    e._spellBar["Curse1"].Remove(replace: true);
                }
                else if (e.hascradh)
                {
                    e._spellBar["Curse2"].Remove(replace: true);
                }
                else if (e.hasmorcradh)
                {
                    e._spellBar["Curse3"].Remove(replace: true);
                }
                else if (e.hasdiacradh)
                {
                    SystemMsg("Another curse afflicts thee. [dia cradh]", 3);
                    succeeded = false;
                }
            }
            else if (_loadedSpell._status.ToLower() == "curse5")
            {
                succeeded = true;
                if (e.hasbeagcradh)
                {
                    e._spellBar["Curse1"].Remove(replace: true);
                }
                else if (e.hascradh)
                {
                    e._spellBar["Curse2"].Remove(replace: true);
                }
                else if (e.hasmorcradh)
                {
                    e._spellBar["Curse3"].Remove(replace: true);
                }
                else if (e.hasardcradh)
                {
                    e._spellBar["Curse4"].Remove(replace: true);
                }
            }
            // Amplify (fas) stacking: same tier rule as curses, against the target's fas buffs.
            else if (_loadedSpell._status.ToLower() == "amplify1")
            {
                succeeded = true;
                if (e.hasfas || e.hasmorfas || e.hasardfas || e.hasdiafas)
                {
                    SystemMsg("A better version of that spell is already cast.", 3);
                    succeeded = false;
                }
            }
            else if (_loadedSpell._status.ToLower() == "amplify2")
            {
                succeeded = true;
                if (e.hasbeagfas)
                {
                    e._spellBar["Amplify1"].Remove(replace: true);
                }
                else if (e.hasmorfas || e.hasardfas || e.hasdiafas)
                {
                    SystemMsg("A better version of that spell is already cast.", 3);
                    succeeded = false;
                }
            }
            else if (_loadedSpell._status.ToLower() == "amplify3")
            {
                succeeded = true;
                if (e.hasbeagfas)
                {
                    e._spellBar["Amplify1"].Remove(replace: true);
                }
                else if (e.hasfas)
                {
                    e._spellBar["Amplify2"].Remove(replace: true);
                }
                else if (e.hasardfas || e.hasdiafas)
                {
                    SystemMsg("A better version of that spell is already cast.", 3);
                    succeeded = false;
                }
            }
            else if (_loadedSpell._status.ToLower() == "amplify4")
            {
                succeeded = true;
                if (e.hasbeagfas)
                {
                    e._spellBar["Amplify1"].Remove(replace: true);
                }
                else if (e.hasfas)
                {
                    e._spellBar["Amplify2"].Remove(replace: true);
                }
                else if (e.hasmorfas)
                {
                    e._spellBar["Amplify3"].Remove(replace: true);
                }
                else if (e.hasdiafas)
                {
                    SystemMsg("A better version of that spell is already cast.", 3);
                    succeeded = false;
                }
            }
            else if (_loadedSpell._status.ToLower() == "amplify5")
            {
                succeeded = true;
                if (e.hasbeagfas)
                {
                    e._spellBar["Amplify1"].Remove(replace: true);
                }
                else if (e.hasfas)
                {
                    e._spellBar["Amplify2"].Remove(replace: true);
                }
                else if (e.hasmorfas)
                {
                    e._spellBar["Amplify3"].Remove(replace: true);
                }
                else if (e.hasardfas)
                {
                    e._spellBar["Amplify4"].Remove(replace: true);
                }
            }
            else if (_loadedSpell._removeStatus.Count() > 0)
            {
                foreach (string statusKey in _loadedSpell._removeStatus)
                {
                    if (e._spellBar.ContainsKey(statusKey))
                    {
                        succeeded = true;
                        e._spellBar[statusKey].Remove();
                    }
                }
            }
            else
            {
                succeeded = true;
            }
            // Apply: body/sound/animations, damage or heal, cast message, and the status spell-bar.
            if (!_loadedSpell._successOnly || succeeded)
            {
                BodyMovement(_loadedSpell._bodyAniType, _loadedSpell._bodyAniSpeed);
                if (_loadedSpell._sound != byte.MaxValue)
                {
                    PlaySound("effect" + _loadedSpell._sound);
                }
                if (_loadedSpell._fromani > 0)
                {
                    SpellAnimation(_player, _loadedSpell._fromani, _loadedSpell._fromanispeed);
                }
                if (_loadedSpell._toani > 0)
                {
                    SpellAnimation(e, _loadedSpell._toani, _loadedSpell._toanispeed);
                }
                if (_loadedSpell._dmg > 0)
                {
                    e.DamageHealth(_loadedSpell._dmg, _player);
                }
                if (_loadedSpell._heal > 0)
                {
                    e.HealHealth(_loadedSpell._heal);
                }
                if (_loadedSpell._castMsg)
                {
                    SystemMsg("You cast " + _loadedSpell._name, 3);
                }
                if (_loadedSpell._checkSpellBar || _loadedSpell._allowRefresh)
                {
                    NewSpellBar((_loadedSpell._status != "") ? _loadedSpell._status : spellName, _loadedSpell._type, _loadedSpell._frame - 1, _loadedSpell._seconds, _loadedSpell._startMsg, _loadedSpell._reMsg, _loadedSpell._endMsg, e);
                }
            }
            // Mana cost is server-authoritative; no client-side deduction here.
            if (!_GM && _loadedSpell._cooldown >= 1.0)
            {
                _loadedSpell._highlight = true;
            }
            _loadedSpell._lastSuccess = DateTime.UtcNow;
        }
        _loadedSpell = null;
    }

    /// <summary>
    ///     Casts spell <paramref name="s"/> from the player. After the rate-limit, cooldown, dead/ghost and
    ///     stealth checks, it resolves targets by the spell's target type - return-home (dachaidh), the tile
    ///     in front (facing), a straight line, a radius (lamh / gar), self, a placed trap, or deferred click
    ///     targeting (target/tile/meall) - and applies the animation, status, and damage. On success it shows
    ///     the cast message, counts the use toward leveling, and starts the cooldown.
    /// </summary>
    private void UseScript(Spell s)
    {
        bool succeeded = false;
        if (s._targettype != "target" && s._targettype != "tile" && s._targettype != "meall" && DateTime.UtcNow.Subtract(lastSpellUse).TotalMilliseconds < 1000.0)
        {
            if (castCount >= 3)
            {
                return;
            }
            castCount++;
        }
        if (!_GM && DateTime.UtcNow.Subtract(s._lastSuccess).TotalMilliseconds < s._cooldown * 1000.0)
        {
            if (s._cooldown >= 1.0)
            {
                SystemMsg("You can use that after " + ((int)(s._cooldown - DateTime.UtcNow.Subtract(s._lastSuccess).TotalMilliseconds / 1000.0) + 1) + " seconds.", 3);
            }
            return;
        }
        if (_player._dead && !s._removeStatus.Contains("Death"))
        {
            SystemMsg("You are too injured to move.", 3);
            return;
        }
        if (_player.Ghost && !s._removeStatus.Contains("Ghost"))
        {
            SystemMsg("Spirits can't do that.", 3);
            return;
        }
        // Mana cost is server-authoritative; no client-side deduction here.
        if (s._checkIfHidden && _player.Hidden)
        {
            _player._spellBar["Invisible"].Remove();
            _player.Hidden = false;
            SendDisplayPlayer();
        }
        // --- Resolve targets by the spell's target type, then apply its effects ---
        if (s._name.Equals("dachaidh", StringComparison.CurrentCultureIgnoreCase))
        {
            succeeded = true;
            SoundBodyFromAni(s);
            ReturnHome();
        }
        else if (s._targettype == "facing")
        {
            Tile facingTile = TileImFacing();
            bool soundPlayed = false;
            if (facingTile != null)
            {
                Entity[] targets = facingTile._entities.Values.ToArray();
                foreach (Entity entity in targets)
                {
                    if ((entity is Monster && !(entity as Monster)._companion) || entity is NPC)
                    {
                        succeeded = true;
                        if (!soundPlayed)
                        {
                            soundPlayed = true;
                            SoundBodyFromAni(s);
                        }
                        if (s._toani > 0)
                        {
                            SpellAnimation(entity, s._toani, s._toanispeed);
                        }
                        SpellBarCheck(s, entity);
                        entity.DamageHealth(s._dmg, _player);
                    }
                }
            }
        }
        else if (s._targettype == "line")
        {
            bool soundPlayed = false;
            Entity[] targets = _map._entities.Values.ToArray();
            foreach (Entity entity in targets)
            {
                if (((entity is Monster && !(entity as Monster)._companion) || entity is NPC) && entity._id != _player._id && _player._location.InLine(entity._location, (D)_player._body._direction, s._range))
                {
                    succeeded = true;
                    if (!soundPlayed)
                    {
                        soundPlayed = true;
                        SoundBodyFromAni(s);
                    }
                    if (s._toani > 0)
                    {
                        SpellAnimation(entity, s._toani, s._toanispeed);
                    }
                    SpellBarCheck(s, entity);
                    entity.DamageHealth(s._dmg, _player);
                }
            }
        }
        else if (s._targettype == "lamh")
        {
            bool soundPlayed = false;
            Entity[] targets = _map._entities.Values.ToArray();
            foreach (Entity entity in targets)
            {
                if (((entity is Monster && !(entity as Monster)._companion) || entity is NPC) && entity._id != _player._id && _player._location.DistanceFrom(entity._location) <= s._range)
                {
                    succeeded = true;
                    if (!soundPlayed)
                    {
                        soundPlayed = true;
                        SoundBodyFromAni(s);
                    }
                    if (s._toani > 0)
                    {
                        SpellAnimation(entity, s._toani, s._toanispeed);
                    }
                    SpellBarCheck(s, entity);
                    entity.DamageHealth(s._dmg, _player);
                }
            }
        }
        else if (s._targettype == "gar")
        {
            bool soundPlayed = false;
            Entity[] targets = _map._entities.Values.ToArray();
            foreach (Entity entity in targets)
            {
                if (((entity is Monster && !(entity as Monster)._companion) || entity is NPC) && entity._id != _player._id && _player._location.DistanceFrom(entity._location) <= 13)
                {
                    succeeded = true;
                    if (!soundPlayed)
                    {
                        soundPlayed = true;
                        SoundBodyFromAni(s);
                    }
                    if (s._toani > 0)
                    {
                        SpellAnimation(entity, s._toani, s._toanispeed);
                    }
                    SpellBarCheck(s, entity);
                    entity.DamageHealth(s._dmg, _player);
                }
            }
        }
        else if (s._targettype == "self")
        {
            if (!s._checkSpellBar || !_player._spellBar.ContainsKey(s._name))
            {
                succeeded = true;
                SoundBodyFromAni(s);
                if (s._toani > 0)
                {
                    SpellAnimation(_player, s._toani, s._toanispeed);
                }
                SpellBarCheck(s, _player);
            }
        }
        else if (s._targettype == "trap")
        {
            succeeded = true;
            Tile trapTile = _player._tile;
            Reactor reactor = new Reactor(new Location(_player._location.X, _player._location.Y));
            reactor._trap = s;
            reactor._type = 0;
            reactor._triggerType = 2;
            _map._reactors.Add(reactor);
            trapTile.SpellAni(96, 80, repeat: true);
        }
        // target/tile/meall: defer - load the spell so the next mouse click picks the target/tile
        else if (s._targettype == "target" || s._targettype == "tile" || s._targettype == "meall")
        {
            _loadedSpell = s;
        }
        if (succeeded)
        {
            // (mana cost: server-authoritative, no client-side deduction)
            if (s._castMsg)
            {
                SystemMsg("You cast " + s._name, 3);
            }
            UseCountAndLevelUp(s);
            if (!_GM && s._cooldown >= 1.0)
            {
                s._highlight = true;
            }
            s._lastSuccess = DateTime.UtcNow;
        }
    }

    /// <summary>
    ///     Executes skill <paramref name="s"/>. After the rate-limit, cooldown, dead/ghost, mana and
    ///     stealth checks, it dispatches by skill name - assail (melee in front), ambush (teleport behind a
    ///     lined-up target), transfer blood, charge / flying kick, disengage, throw, wolf fang fist / poison
    ///     punch - or by target type (projectile / facing / line / lamh / gar / self), applying the movement,
    ///     animation, status and damage. On success it shows the cast message, counts the use toward
    ///     leveling, and starts the cooldown. The // === ... === markers below delimit each handler.
    /// </summary>
    private void UseScript(Skill s)
    {
        if (DateTime.UtcNow.Subtract(s._lastUse).TotalMilliseconds < 333.0)
        {
            return;
        }
        s._lastUse = DateTime.UtcNow;
        if (!_GM && DateTime.UtcNow.Subtract(s._lastSuccess).TotalMilliseconds < s._cooldown * 1000.0)
        {
            if (s._cooldown >= 1.0)
            {
                SystemMsg("You can use that after " + ((int)(s._cooldown - DateTime.UtcNow.Subtract(s._lastSuccess).TotalMilliseconds / 1000.0) + 1) + " seconds.", 3);
            }
            return;
        }
        if (_player._dead)
        {
            SystemMsg("You are too injured to move.", 3);
            return;
        }
        if (_player.Ghost)
        {
            SystemMsg("Spirits can't do that.", 3);
            return;
        }
        bool flag = false;
        if (s._name.Equals("assail", StringComparison.CurrentCultureIgnoreCase) && lastArrowKeyDirection != byte.MaxValue && lastArrowKeyPressDT != DateTime.MinValue && DateTime.UtcNow.Subtract(lastArrowKeyPressDT).TotalMilliseconds <= 100.0)
        {
            Tile tile = TileImFacing();
            if (tile != null)
            {
                Entity[] array = tile._entities.Values.ToArray();
                foreach (Entity entity in array)
                {
                    if (entity is NPC && entity._assailPush)
                    {
                        entity.MoveATile(lastArrowKeyDirection, walk: false);
                        entity.CenterEntity();
                        break;
                    }
                }
            }
        }
        if (s._manaCost > 0 && s._manaCost > _player._curMP)
        {
            SystemMsg("Your will is too weak.", 3);
            return;
        }
        if (s._checkIfHidden && _player.Hidden)
        {
            _player._spellBar["Invisible"].Remove();
            _player.Hidden = false;
            SendDisplayPlayer();
        }
        // === Skill: assail (melee the tile in front; STR-based damage) ===
        if (s._name.Equals("assail", StringComparison.CurrentCultureIgnoreCase))
        {
            SoundBodyFromAni(s);
            Tile tile2 = TileImFacing();
            if (tile2 != null)
            {
                Entity[] array2 = tile2._entities.Values.ToArray();
                foreach (Entity entity2 in array2)
                {
                    if ((entity2 is Monster && !(entity2 as Monster)._companion) || entity2 is NPC)
                    {
                        flag = true;
                        if (s._toani > 0)
                        {
                            SpellAnimation(entity2, s._toani, s._toanispeed);
                        }
                        SpellBarCheck(s, entity2);
                        int num = weaponDmg;
                        int dmg = (int)((double)(int)_player._str * 0.9 * 6.0) + _player._dmg * 5 + num;
                        entity2.DamageHealth(dmg, _player);
                    }
                }
            }
            if (s._name.Equals("assail", StringComparison.CurrentCultureIgnoreCase))
            {
                foreach (InventoryItem item in _equipment)
                {
                    if (item._name == "Cursed Saber")
                    {
                        SpawnMonster("Ghost 1", tile2.Location, "old", (byte)_player._body._direction, companion: true);
                    }
                }
            }
        }
        else if (!s._name.Equals("wield staff", StringComparison.CurrentCultureIgnoreCase) && !s._name.Equals("wield two-handed", StringComparison.CurrentCultureIgnoreCase))
        {
            // === Skill: ambush (teleport behind a lined-up target) ===
            if (s._name.Equals("ambush", StringComparison.CurrentCultureIgnoreCase))
            {
                bool flag2 = false;
                foreach (Entity item2 in from z in _map._entities.Values.ToArray()
                                         orderby z._location.DistanceFrom(_player._location)
                                         select z)
                {
                    if (item2._id == _player._id || !_player._location.InLine(item2._location, (D)_player._body._direction, 3) || item2 is Item)
                    {
                        continue;
                    }
                    Tile tile3 = null;
                    Location location = new Location(0, 0);
                    bool flag3 = true;
                    flag = true;
                    if (!flag2)
                    {
                        flag2 = true;
                        SoundBodyFromAni(s);
                    }
                    if (_player._body._direction == 0)
                    {
                        location = new Location(item2._location.X, item2._location.Y - 1);
                        if (location.X < (int)_map._width && location.Y < (int)_map._height && location.X >= 0 && location.Y >= 0)
                        {
                            tile3 = _map._tiles[(item2._location.Y - 1) * (int)_map._width + item2._location.X];
                            if (tile3 != null && tile3.getTopMostNonItem() == null && tile3._walkable)
                            {
                                _player._body._direction = 2;
                                Teleport(tile3.Location.X, tile3.Location.Y);
                                _player._body.setDefault();
                                flag3 = false;
                            }
                        }
                        if (flag3)
                        {
                            location = new Location(item2._location.X - 1, item2._location.Y);
                            if (location.X < (int)_map._width && location.Y < (int)_map._height && location.X >= 0 && location.Y >= 0)
                            {
                                tile3 = _map._tiles[location.Y * (int)_map._width + location.X];
                                if (tile3 != null && tile3.getTopMostNonItem() == null && tile3._walkable)
                                {
                                    _player._body._direction = 1;
                                    Teleport(tile3.Location.X, tile3.Location.Y);
                                    _player._body.setDefault();
                                    flag3 = false;
                                }
                            }
                        }
                        if (flag3)
                        {
                            location = new Location(item2._location.X + 1, item2._location.Y);
                            if (location.X < (int)_map._width && location.Y < (int)_map._height && location.X >= 0 && location.Y >= 0)
                            {
                                tile3 = _map._tiles[location.Y * (int)_map._width + location.X];
                                if (tile3 != null && tile3.getTopMostNonItem() == null && tile3._walkable)
                                {
                                    _player._body._direction = 3;
                                    Teleport(tile3.Location.X, tile3.Location.Y);
                                    _player._body.setDefault();
                                    flag3 = false;
                                }
                            }
                        }
                        if (flag3)
                        {
                            SystemMsg("There is no way to go.", 3);
                        }
                    }
                    else if (_player._body._direction == 2)
                    {
                        location = new Location(item2._location.X, item2._location.Y + 1);
                        if (location.X < (int)_map._width && location.Y < (int)_map._height && location.X >= 0 && location.Y >= 0)
                        {
                            tile3 = _map._tiles[location.Y * (int)_map._width + location.X];
                            if (tile3 != null && tile3.getTopMostNonItem() == null && tile3._walkable)
                            {
                                _player._body._direction = 0;
                                Teleport(tile3.Location.X, tile3.Location.Y);
                                _player._body.setDefault();
                                flag3 = false;
                            }
                        }
                        if (flag3)
                        {
                            location = new Location(item2._location.X + 1, item2._location.Y);
                            if (location.X < (int)_map._width && location.Y < (int)_map._height && location.X >= 0 && location.Y >= 0)
                            {
                                tile3 = _map._tiles[location.Y * (int)_map._width + location.X];
                                if (tile3 != null && tile3.getTopMostNonItem() == null && tile3._walkable)
                                {
                                    _player._body._direction = 3;
                                    Teleport(tile3.Location.X, tile3.Location.Y);
                                    _player._body.setDefault();
                                    flag3 = false;
                                }
                            }
                        }
                        if (flag3)
                        {
                            location = new Location(item2._location.X - 1, item2._location.Y);
                            if (location.X < (int)_map._width && location.Y < (int)_map._height && location.X >= 0 && location.Y >= 0)
                            {
                                tile3 = _map._tiles[location.Y * (int)_map._width + location.X];
                                if (tile3 != null && tile3.getTopMostNonItem() == null && tile3._walkable)
                                {
                                    _player._body._direction = 1;
                                    Teleport(tile3.Location.X, tile3.Location.Y);
                                    _player._body.setDefault();
                                    flag3 = false;
                                }
                            }
                        }
                        if (flag3)
                        {
                            SystemMsg("There is no way to go.", 3);
                        }
                    }
                    else if (_player._body._direction == 3)
                    {
                        location = new Location(item2._location.X - 1, item2._location.Y);
                        if (location.X < (int)_map._width && location.Y < (int)_map._height && location.X >= 0 && location.Y >= 0)
                        {
                            tile3 = _map._tiles[item2._location.Y * (int)_map._width + item2._location.X - 1];
                            if (tile3 != null && tile3.getTopMostNonItem() == null && tile3._walkable)
                            {
                                _player._body._direction = 1;
                                Teleport(tile3.Location.X, tile3.Location.Y);
                                _player._body.setDefault();
                                flag3 = false;
                            }
                        }
                        if (flag3)
                        {
                            location = new Location(item2._location.X, item2._location.Y + 1);
                            if (location.X < (int)_map._width && location.Y < (int)_map._height && location.X >= 0 && location.Y >= 0)
                            {
                                tile3 = _map._tiles[location.Y * (int)_map._width + location.X];
                                if (tile3 != null && tile3.getTopMostNonItem() == null && tile3._walkable)
                                {
                                    _player._body._direction = 0;
                                    Teleport(tile3.Location.X, tile3.Location.Y);
                                    _player._body.setDefault();
                                    flag3 = false;
                                }
                            }
                        }
                        if (flag3)
                        {
                            location = new Location(item2._location.X, item2._location.Y - 1);
                            if (location.X < (int)_map._width && location.Y < (int)_map._height && location.X >= 0 && location.Y >= 0)
                            {
                                tile3 = _map._tiles[location.Y * (int)_map._width + location.X];
                                if (tile3 != null && tile3.getTopMostNonItem() == null && tile3._walkable)
                                {
                                    _player._body._direction = 2;
                                    Teleport(tile3.Location.X, tile3.Location.Y);
                                    _player._body.setDefault();
                                    flag3 = false;
                                }
                            }
                        }
                        if (flag3)
                        {
                            SystemMsg("There is no way to go.", 3);
                        }
                    }
                    else
                    {
                        if (_player._body._direction != 1)
                        {
                            break;
                        }
                        location = new Location(item2._location.X + 1, item2._location.Y);
                        if (location.X < (int)_map._width && location.Y < (int)_map._height && location.X >= 0 && location.Y >= 0)
                        {
                            tile3 = _map._tiles[item2._location.Y * (int)_map._width + item2._location.X + 1];
                            if (tile3 != null && tile3.getTopMostNonItem() == null && tile3._walkable)
                            {
                                _player._body._direction = 3;
                                Teleport(tile3.Location.X, tile3.Location.Y);
                                _player._body.setDefault();
                                flag3 = false;
                            }
                        }
                        if (flag3)
                        {
                            location = new Location(item2._location.X, item2._location.Y - 1);
                            if (location.X < (int)_map._width && location.Y < (int)_map._height && location.X >= 0 && location.Y >= 0)
                            {
                                tile3 = _map._tiles[location.Y * (int)_map._width + location.X];
                                if (tile3 != null && tile3.getTopMostNonItem() == null && tile3._walkable)
                                {
                                    _player._body._direction = 2;
                                    Teleport(tile3.Location.X, tile3.Location.Y);
                                    _player._body.setDefault();
                                    flag3 = false;
                                }
                            }
                        }
                        if (flag3)
                        {
                            location = new Location(item2._location.X, item2._location.Y + 1);
                            if (location.X < (int)_map._width && location.Y < (int)_map._height && location.X >= 0 && location.Y >= 0)
                            {
                                tile3 = _map._tiles[location.Y * (int)_map._width + location.X];
                                if (tile3 != null && tile3.getTopMostNonItem() == null && tile3._walkable)
                                {
                                    _player._body._direction = 0;
                                    Teleport(tile3.Location.X, tile3.Location.Y);
                                    _player._body.setDefault();
                                    flag3 = false;
                                }
                            }
                        }
                        if (flag3)
                        {
                            SystemMsg("There is no way to go.", 3);
                        }
                    }
                    break;
                }
            }
            // === Skill: transfer blood ===
            else if (s._name.Equals("transfer blood", StringComparison.CurrentCultureIgnoreCase))
            {
                Tile tile4 = TileImFacing();
                bool flag4 = false;
                if (tile4 != null)
                {
                    Entity[] array3 = tile4._entities.Values.ToArray();
                    foreach (Entity entity3 in array3)
                    {
                        if (!(entity3 is Item))
                        {
                            flag = true;
                            if (!flag4)
                            {
                                flag4 = true;
                                SoundBodyFromAni(s);
                            }
                            if (s._toani > 0)
                            {
                                SpellAnimation(entity3, s._toani, s._toanispeed);
                            }
                            entity3.HealHealth(_player._maxHP / 10);
                        }
                    }
                }
            }
            // === Skill: charge / flying kick ===
            else if (s._name.Equals("charge", StringComparison.CurrentCultureIgnoreCase) || s._name.Equals("flying kick", StringComparison.CurrentCultureIgnoreCase))
            {
                Tile tile5 = TileImFacing();
                if (tile5 != null && tile5.getTopMostNonItem() == null && tile5._walkable)
                {
                    Teleport(tile5.Location.X, tile5.Location.Y);
                    tile5 = TileImFacing();
                    if (tile5 != null && tile5.getTopMostNonItem() == null && tile5._walkable)
                    {
                        Teleport(tile5.Location.X, tile5.Location.Y);
                        tile5 = TileImFacing();
                        if (tile5 != null && tile5.getTopMostNonItem() == null && tile5._walkable)
                        {
                            Teleport(tile5.Location.X, tile5.Location.Y);
                        }
                    }
                }
                bool flag5 = false;
                if (tile5 != null)
                {
                    Entity[] array4 = tile5._entities.Values.ToArray();
                    foreach (Entity entity4 in array4)
                    {
                        if ((entity4 is Monster && !(entity4 as Monster)._companion) || entity4 is NPC)
                        {
                            flag = true;
                            if (!flag5)
                            {
                                flag5 = true;
                                SoundBodyFromAni(s);
                            }
                            if (s._toani > 0)
                            {
                                SpellAnimation(entity4, s._toani, s._toanispeed);
                            }
                            SpellBarCheck(s, entity4);
                            entity4.DamageHealth(s._dmg, _player);
                        }
                    }
                }
            }
            // === Skill: disengage ===
            else if (s._name.Equals("disengage", StringComparison.CurrentCultureIgnoreCase))
            {
                Tile tile6 = TileImFacing();
                bool flag6 = false;
                if (tile6 != null)
                {
                    Entity[] array5 = tile6._entities.Values.ToArray();
                    foreach (Entity entity5 in array5)
                    {
                        if ((entity5 is Monster && !(entity5 as Monster)._companion) || entity5 is NPC)
                        {
                            flag = true;
                            if (!flag6)
                            {
                                flag6 = true;
                                SoundBodyFromAni(s);
                            }
                            if (s._toani > 0)
                            {
                                SpellAnimation(entity5, s._toani, s._toanispeed);
                            }
                            SpellBarCheck(s, entity5);
                            entity5.DamageHealth(s._dmg, _player);
                        }
                    }
                }
                tile6 = TileBehindMe();
                if (tile6 != null && tile6.getTopMostNonItem() == null && tile6._walkable)
                {
                    Teleport(tile6.Location.X, tile6.Location.Y);
                    tile6 = TileBehindMe();
                    if (tile6 != null && tile6.getTopMostNonItem() == null && tile6._walkable)
                    {
                        Teleport(tile6.Location.X, tile6.Location.Y);
                        tile6 = TileBehindMe();
                        if (tile6 != null && tile6.getTopMostNonItem() == null && tile6._walkable)
                        {
                            Teleport(tile6.Location.X, tile6.Location.Y);
                            tile6 = TileBehindMe();
                            if (tile6 != null && tile6.getTopMostNonItem() == null && tile6._walkable)
                            {
                                Teleport(tile6.Location.X, tile6.Location.Y);
                            }
                        }
                    }
                }
            }
            // === Skill: throw ===
            else if (s._name.Equals("throw", StringComparison.CurrentCultureIgnoreCase))
            {
                bool flag7 = false;
                Tile tile7 = TileImFacing();
                if (tile7 != null)
                {
                    Entity[] array6 = tile7._entities.Values.ToArray();
                    foreach (Entity entity6 in array6)
                    {
                        if (!(entity6 is Item))
                        {
                            flag = true;
                            if (!flag7)
                            {
                                flag7 = true;
                                SoundBodyFromAni(s);
                            }
                            if (s._toani > 0)
                            {
                                SpellAnimation(entity6, s._toani, s._toanispeed);
                            }
                            if (!(entity6 is Player) && !(entity6 is NPC))
                            {
                                entity6.MoveATile(_player._body._direction, walk: false);
                                entity6.CenterEntity();
                            }
                        }
                    }
                }
            }
            // === Target type: projectile ===
            else if (s._targettype == "projectile")
            {
                flag = true;
                SoundBodyFromAni(s);
                SpawnProjectile(_player, s._projType, s._dmg);
            }
            // === Target type: facing (the tile in front) ===
            else if (s._targettype == "facing")
            {
                Tile tile8 = TileImFacing();
                bool flag8 = false;
                if (tile8 != null)
                {
                    Entity[] array7 = tile8._entities.Values.ToArray();
                    foreach (Entity entity7 in array7)
                    {
                        if ((entity7 is Monster && !(entity7 as Monster)._companion) || entity7 is NPC)
                        {
                            flag = true;
                            if (!flag8)
                            {
                                flag8 = true;
                                SoundBodyFromAni(s);
                            }
                            if (s._toani > 0)
                            {
                                SpellAnimation(entity7, s._toani, s._toanispeed);
                            }
                            SpellBarCheck(s, entity7);
                            int num3 = weaponDmg;
                            int dmg2 = s._dmg + _player._dmg * 5 + num3;
                            // === Skill: wolf fang fist / poison punch ===
                            if (s._name.Equals("wolf fang fist", StringComparison.CurrentCultureIgnoreCase) || s._name.Equals("poison punch", StringComparison.CurrentCultureIgnoreCase))
                            {
                                dmg2 = (int)((double)(int)_player._str * 0.9 * 6.0) + _player._dmg * 5 + num3;
                            }
                            entity7.DamageHealth(dmg2, _player);
                        }
                    }
                }
            }
            // === Target type: line ===
            else if (s._targettype == "line")
            {
                bool flag9 = false;
                Entity[] array = _map._entities.Values.ToArray();
                foreach (Entity entity8 in array)
                {
                    if (((entity8 is Monster && !(entity8 as Monster)._companion) || entity8 is NPC) && entity8._id != _player._id && _player._location.InLine(entity8._location, (D)_player._body._direction, s._range))
                    {
                        flag = true;
                        if (!flag9)
                        {
                            flag9 = true;
                            SoundBodyFromAni(s);
                        }
                        if (s._toani > 0)
                        {
                            SpellAnimation(entity8, s._toani, s._toanispeed);
                        }
                        SpellBarCheck(s, entity8);
                        entity8.DamageHealth(s._dmg, _player);
                    }
                }
            }
            // === Target type: lamh (radius) ===
            else if (s._targettype == "lamh")
            {
                bool flag10 = false;
                Entity[] array = _map._entities.Values.ToArray();
                foreach (Entity entity9 in array)
                {
                    if (((entity9 is Monster && !(entity9 as Monster)._companion) || entity9 is NPC) && entity9._id != _player._id && _player._location.DistanceFrom(entity9._location) <= s._range)
                    {
                        flag = true;
                        if (!flag10)
                        {
                            flag10 = true;
                            SoundBodyFromAni(s);
                        }
                        if (s._toani > 0)
                        {
                            SpellAnimation(entity9, s._toani, s._toanispeed);
                        }
                        SpellBarCheck(s, entity9);
                        entity9.DamageHealth(s._dmg, _player);
                    }
                }
            }
            // === Target type: gar (AoE radius) ===
            else if (s._targettype == "gar")
            {
                bool flag11 = false;
                Entity[] array = _map._entities.Values.ToArray();
                foreach (Entity entity10 in array)
                {
                    if (((entity10 is Monster && !(entity10 as Monster)._companion) || entity10 is NPC) && entity10._id != _player._id && _player._location.DistanceFrom(entity10._location) <= 13)
                    {
                        flag = true;
                        if (!flag11)
                        {
                            flag11 = true;
                            SoundBodyFromAni(s);
                        }
                        if (s._toani > 0)
                        {
                            SpellAnimation(entity10, s._toani, s._toanispeed);
                        }
                        SpellBarCheck(s, entity10);
                        entity10.DamageHealth(s._dmg, _player);
                    }
                }
            }
            // === Target type: self ===
            else if (s._targettype == "self" && (!s._checkSpellBar || !_player._spellBar.ContainsKey(s._name)))
            {
                flag = true;
                SoundBodyFromAni(s);
                if (s._toani > 0)
                {
                    SpellAnimation(_player, s._toani, s._toanispeed);
                }
                SpellBarCheck(s, _player);
            }
        }
        if (flag)
        {
            if (s._manaCost > 0 && s._manaCost <= _player._curMP)
            {
                _player._curMP -= s._manaCost;
            }
            if (s._castMsg)
            {
                SystemMsg("You cast " + s._name, 3);
            }
            UseCountAndLevelUp(s);
        }
        if (!_GM && s._cooldown >= 1.0)
        {
            s._highlight = true;
        }
        s._lastSuccess = DateTime.UtcNow;
    }

    /// <summary>
    ///     Executes action <paramref name="s"/> (emotes / utility). After the rate-limit, cooldown,
    ///     dead/ghost and stealth checks it plays the action's body/sound/animation, then handles the few
    ///     special actions: "look" (who is on the tile in front), "study creature" (reveal a monster's
    ///     level/exp), and "unlock" (a level-scaled chance to open a locked monster, which may spring a
    ///     Mimic). Finally starts the cooldown.
    /// </summary>
    private void UseScript(Action s)
    {
        if (DateTime.UtcNow.Subtract(s._lastUse).TotalMilliseconds < 333.0)
        {
            return;
        }
        s._lastUse = DateTime.UtcNow;
        if (!_GM && DateTime.UtcNow.Subtract(s._lastSuccess).TotalMilliseconds < s._cooldown * 1000.0)
        {
            if (s._cooldown >= 1.0)
            {
                SystemMsg("You can use that after " + ((int)(s._cooldown - DateTime.UtcNow.Subtract(s._lastSuccess).TotalMilliseconds / 1000.0) + 1) + " seconds.", 3);
            }
            return;
        }
        if (_player._dead)
        {
            SystemMsg("You are too injured to move.", 3);
            return;
        }
        if (_player.Ghost)
        {
            SystemMsg("Spirits can't do that.", 3);
            return;
        }
        if (s._checkIfHidden && _player.Hidden)
        {
            _player._spellBar["Invisible"].Remove();
            _player.Hidden = false;
            SendDisplayPlayer();
        }
        BodyMovement(s._bodyAniType, s._bodyAniSpeed);
        if (s._sound != byte.MaxValue)
        {
            PlaySound("effect" + s._sound);
        }
        if (s._fromani > 0)
        {
            SpellAnimation(_player, s._fromani, s._fromanispeed);
        }
        // Action "look": list the (non-hidden) entities on the tile in front, in the sense window.
        if (s._name.Equals("look", StringComparison.CurrentCultureIgnoreCase))
        {
            string lookText = _map._number + " - " + _map._name + "\n ";
            Tile facingTile = TileImFacing();
            if (facingTile != null)
            {
                foreach (Entity entity in facingTile._entities.Values.OrderByDescending((Entity z) => z.CreateTime))
                {
                    if (!entity.Hidden)
                    {
                        lookText = lookText + "\n" + entity._name;
                    }
                }
            }
            _senseMenu._labels["senseLabel"].ChangeText(lookText);
            _viewingSense = true;
        }
        // Action "study creature": reveal a monster's level / exp.
        else if (s._name.Equals("study creature", StringComparison.CurrentCultureIgnoreCase))
        {
            Tile facingTile = TileImFacing();
            if (facingTile != null)
            {
                foreach (Entity entity in facingTile._entities.Values.OrderByDescending((Entity z) => z.CreateTime))
                {
                    if (entity is Monster)
                    {
                        entity._showInfo = true;
                        SystemMsg("Creature: " + entity._name + " - LEV: " + entity._lev + " - EXP: " + entity._exp.ToString(), 3);
                    }
                }
            }
        }
        // Action "unlock": level-scaled chance to open a locked monster; rarely springs a Mimic.
        else if (s._name.Equals("unlock", StringComparison.CurrentCultureIgnoreCase))
        {
            Tile facingTile = TileImFacing();
            if (facingTile != null)
            {
                foreach (Entity entity in facingTile._entities.Values.OrderByDescending((Entity z) => z.CreateTime))
                {
                    if (!(entity is Monster) || entity._unlock <= 0)
                    {
                        continue;
                    }
                    bool unlocked = false;
                    int unlockRoll = rand.Next(0, 101);
                    int successThreshold = 90;
                    if (entity._lev == 10)
                    {
                        successThreshold = 90;
                        if (unlockRoll <= successThreshold)
                        {
                            unlocked = true;
                        }
                    }
                    else if (entity._lev == 20)
                    {
                        successThreshold = 80;
                        if (unlockRoll <= successThreshold)
                        {
                            unlocked = true;
                        }
                    }
                    else if (entity._lev == 30)
                    {
                        successThreshold = 70;
                        if (unlockRoll <= successThreshold)
                        {
                            unlocked = true;
                        }
                    }
                    else if (entity._lev == 40)
                    {
                        successThreshold = 60;
                        if (unlockRoll <= successThreshold)
                        {
                            unlocked = true;
                        }
                    }
                    else if (entity._lev == 50)
                    {
                        successThreshold = 50;
                        if (unlockRoll <= successThreshold)
                        {
                            unlocked = true;
                        }
                    }
                    if (unlocked)
                    {
                        UseCountAndLevelUp(s);
                        bool spawnMimic = false;
                        int mimicRoll = rand.Next(0, 101);
                        if (mimicRoll <= 1)
                        {
                            spawnMimic = true;
                        }
                        if (spawnMimic)
                        {
                            (entity as Monster)._deathDelay = 100.0;
                            entity._dead = true;
                            SpawnMonster("Mimic", entity._location, "new", 1);
                            continue;
                        }
                        (entity as Monster)._deathDelay = 100.0;
                        entity._dead = true;
                        if (entity._lev == 10)
                        {
                            GainExp(5000u);
                        }
                    }
                    else
                    {
                        SystemMsg("Failed.", 3);
                        entity._unlock--;
                        if (entity._unlock == 0)
                        {
                            SpellAnimation(entity, 142, 100);
                            (entity as Monster)._deathDelay = 1000.0;
                            entity._dead = true;
                        }
                    }
                }
            }
        }
        if (!_GM && s._cooldown >= 1.0)
        {
            s._highlight = true;
        }
        s._lastSuccess = DateTime.UtcNow;
    }

    private void UseSpeechMacro(byte num)
    {
        _miscMenu._labels["chatLabel"].ChangeText(_player._name + ": " + _macroMenu._textFields["phrase" + num + "Box"].Text);
        _miscMenu._labels["chatLabel"].SetColor(Engine.Color.White);
        _miscMenu._textFields["chatTF"].SetPosition(_miscMenu._labels["chatLabel"]._position.X - 4.0 + (double)(_miscMenu._labels["chatLabel"]._text.Length * 6), _miscMenu._labels["chatLabel"]._position.Y - 2.0);
        _miscMenu._textFields["chatTF"].SetFocus();
        _miscMenu._textFields["chatTF"]._textObj.SetColor(Engine.Color.White);
        _chatMode = true;
    }

    private void ReturnHome()
    {
        int num = 0;
        int num2 = 0;
        int num3 = rand.Next(1, 6);
        string text = "Mileth";
        switch (num3)
        {
            case 2:
                text = "Rucesion";
                break;
            case 3:
                text = "Abel";
                break;
            case 4:
                text = "Piet";
                break;
            case 5:
                text = "Tagor";
                break;
        }
        foreach (JToken item in _mapsDB["inns"].Children())
        {
            if (text == item.Value<string>("nation"))
            {
                num = rand.Next((int)item["dest"]["lowx"], (int)item["dest"]["highx"] + 1);
                num2 = rand.Next((int)item["dest"]["lowy"], (int)item["dest"]["highy"] + 1);
                NewMap(item.Value<int>("mapnum"), num, num2);
                break;
            }
        }
    }

    private void Scripts(string script)
    {
        script = script.ToLower();
        if (script == "armachd" || script.Contains("naomh aite") || script.Contains("beannaich") || script == "fas deireas" || script.Contains("fas nadur"))
        {
            UseScript(script, "cast", checkSpellBar: false, checkHidden: true, castMsg: true, 2, allowRefresh: true);
            return;
        }
        switch (script)
        {
            case "assail":
                if (HasSkill("Two-handed Attack"))
                {
                    UseScript(script, "2h", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                }
                else if (HasSkill("Double Punch"))
                {
                    UseScript(script, "punch", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                }
                else
                {
                    UseScript(script, "assail", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                }
                break;
            case "ambush":
                {
                    foreach (Entity item in from z in _map._entities.Values.ToArray()
                                            orderby z._location.DistanceFrom(_player._location)
                                            select z)
                    {
                        if (item._id == _player._id || !_player._location.InLine(item._location, (D)_player._body._direction, 3) || item is Item)
                        {
                            continue;
                        }
                        if (_player._body._direction == 0)
                        {
                            Tile tile2 = _map._tiles[(item._location.Y - 1) * (int)_map._width + item._location.X];
                            if (tile2 != null)
                            {
                                _player._body._direction = 2;
                                Teleport(tile2.Location.X, tile2.Location.Y);
                                _player._body.setDefault();
                                break;
                            }
                            tile2 = _map._tiles[item._location.Y * (int)_map._width + item._location.X - 1];
                            if (tile2 != null)
                            {
                                _player._body._direction = 1;
                                Teleport(tile2.Location.X, tile2.Location.Y);
                                _player._body.setDefault();
                                break;
                            }
                            tile2 = _map._tiles[item._location.Y * (int)_map._width + item._location.X + 1];
                            if (tile2 != null)
                            {
                                _player._body._direction = 3;
                                Teleport(tile2.Location.X, tile2.Location.Y);
                                _player._body.setDefault();
                            }
                            else
                            {
                                SystemMsg("There is no way to go.", 3);
                            }
                        }
                        else if (_player._body._direction == 2)
                        {
                            Tile tile3 = _map._tiles[(item._location.Y + 1) * (int)_map._width + item._location.X];
                            if (tile3 != null)
                            {
                                _player._body._direction = 0;
                                Teleport(tile3.Location.X, tile3.Location.Y);
                                _player._body.setDefault();
                                break;
                            }
                            tile3 = _map._tiles[item._location.Y * (int)_map._width + item._location.X + 1];
                            if (tile3 != null)
                            {
                                _player._body._direction = 3;
                                Teleport(tile3.Location.X, tile3.Location.Y);
                                _player._body.setDefault();
                                break;
                            }
                            tile3 = _map._tiles[item._location.Y * (int)_map._width + item._location.X - 1];
                            if (tile3 != null)
                            {
                                _player._body._direction = 1;
                                Teleport(tile3.Location.X, tile3.Location.Y);
                                _player._body.setDefault();
                            }
                            else
                            {
                                SystemMsg("There is no way to go.", 3);
                            }
                        }
                        else if (_player._body._direction == 3)
                        {
                            Tile tile4 = _map._tiles[item._location.Y * (int)_map._width + item._location.X - 1];
                            if (tile4 != null)
                            {
                                _player._body._direction = 1;
                                Teleport(tile4.Location.X, tile4.Location.Y);
                                _player._body.setDefault();
                                break;
                            }
                            tile4 = _map._tiles[(item._location.Y + 1) * (int)_map._width + item._location.X];
                            if (tile4 != null)
                            {
                                _player._body._direction = 0;
                                Teleport(tile4.Location.X, tile4.Location.Y);
                                _player._body.setDefault();
                                break;
                            }
                            tile4 = _map._tiles[(item._location.Y - 1) * (int)_map._width + item._location.X];
                            if (tile4 != null)
                            {
                                _player._body._direction = 2;
                                Teleport(tile4.Location.X, tile4.Location.Y);
                                _player._body.setDefault();
                            }
                            else
                            {
                                SystemMsg("There is no way to go.", 3);
                            }
                        }
                        else
                        {
                            if (_player._body._direction != 1)
                            {
                                break;
                            }
                            Tile tile5 = _map._tiles[item._location.Y * (int)_map._width + item._location.X + 1];
                            if (tile5 != null)
                            {
                                _player._body._direction = 3;
                                Teleport(tile5.Location.X, tile5.Location.Y);
                                _player._body.setDefault();
                                break;
                            }
                            tile5 = _map._tiles[(item._location.Y - 1) * (int)_map._width + item._location.X];
                            if (tile5 != null)
                            {
                                _player._body._direction = 2;
                                Teleport(tile5.Location.X, tile5.Location.Y);
                                _player._body.setDefault();
                                break;
                            }
                            tile5 = _map._tiles[(item._location.Y + 1) * (int)_map._width + item._location.X];
                            if (tile5 != null)
                            {
                                _player._body._direction = 0;
                                Teleport(tile5.Location.X, tile5.Location.Y);
                                _player._body.setDefault();
                            }
                            else
                            {
                                SystemMsg("There is no way to go.", 3);
                            }
                        }
                        break;
                    }
                    break;
                }
            case "kick":
                UseScript(script, "kick", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "double punch":
                UseScript(script, "punch", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "mantis kick":
                UseScript(script, "mantis kick", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "flying kick":
                UseScript(script, "flying kick", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "elbow":
                UseScript(script, "elbow", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "high kick":
                UseScript(script, "high kick", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "snake fist":
                UseScript(script, "snake fist", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "uppercut":
                UseScript(script, "uppercut", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "mid kick":
                UseScript(script, "mid kick", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "draco tail kick":
                UseScript(script, "roundhouse", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "highest kick":
                UseScript(script, "highest kick", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "wind blade":
            case "cyclone blade":
            case "crasher":
                UseScript(script, "assail", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "charge":
                UseScript(script, "charge", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "two-handed attack":
                UseScript(script, "2h", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "lunge":
                UseScript(script, "lunge", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "plunge":
                UseScript(script, "plunge", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "jab":
                UseScript(script, "jab", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "upstrike":
                UseScript(script, "upstrike", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "sideswipe":
                UseScript(script, "sideswipe", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "stab and twist":
                UseScript(script, "stab", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "stab twice":
                UseScript(script, "stab twice", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "stab 1":
                UseScript(script, "stab 1", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "stab 2":
                UseScript(script, "stab 2", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "stab 3":
                UseScript(script, "stab 3", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "stab 4":
                UseScript(script, "stab 4", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "distract":
                UseScript(script, "distract", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "throw weapon":
                UseScript(script, "throw weapon", checkSpellBar: false, checkHidden: true, castMsg: false, 1);
                break;
            case "taunt":
                UseScript(script, "taunt", checkSpellBar: false, checkHidden: true, castMsg: false, 3);
                break;
            case "frighten":
                UseScript(script, "frighten", checkSpellBar: false, checkHidden: true, castMsg: false, 3);
                break;
            case "hopak dance":
                UseScript(script, "hopak dance", checkSpellBar: false, checkHidden: true, castMsg: false, 3);
                break;
            case "batusi dance":
                UseScript(script, "batusi dance", checkSpellBar: false, checkHidden: true, castMsg: false, 3);
                break;
            case "sand dance":
                UseScript(script, "sand dance", checkSpellBar: false, checkHidden: true, castMsg: false, 3);
                break;
            case "dance finisher":
                UseScript(script, "dance finisher", checkSpellBar: false, checkHidden: true, castMsg: false, 3);
                break;
            case "stretch":
                UseScript(script, "stretch", checkSpellBar: false, checkHidden: true, castMsg: false, 3);
                break;
            case "grovel":
                UseScript(script, "grovel", checkSpellBar: false, checkHidden: true, castMsg: false, 3);
                break;
            case "throw page":
                UseScript(script, "throw page", checkSpellBar: false, checkHidden: true, castMsg: false, 3);
                break;
            case "throw book":
                UseScript(script, "throw book", checkSpellBar: false, checkHidden: true, castMsg: false, 3);
                break;
            case "rock":
                BodyMovement(1, 200);
                SpawnProjectile(_player, 13, 0);
                break;
            case "throw dagger":
                BodyMovement(1, 200);
                SpawnProjectile(_player, 9, 5000);
                break;
            case "throw surigam":
                BodyMovement(1, 200);
                SpawnProjectile(_player, 12, 5000);
                break;
            case "pramh":
            case "dall":
                UseScript(script, "cast", checkSpellBar: true, checkHidden: true, castMsg: true, 2);
                break;
            case "claw fist":
                UseScript(script, "assail", checkSpellBar: false, checkHidden: true, castMsg: true, 1);
                break;
            case "look":
                {
                    string text2 = _map._number + " - " + _map._name + "\n ";
                    Tile tile = TileImFacing();
                    if (tile != null)
                    {
                        foreach (Entity item2 in tile._entities.Values.OrderByDescending((Entity z) => z.CreateTime))
                        {
                            if (!item2.Hidden)
                            {
                                text2 = text2 + "\n" + item2._name;
                            }
                        }
                    }
                    _senseMenu._labels["senseLabel"].ChangeText(text2);
                    _viewingSense = true;
                    break;
                }
            case "dachaidh":
                {
                    UseScript(script, "cast", checkSpellBar: false, checkHidden: true, castMsg: true, 2);
                    int num = 0;
                    int num2 = 0;
                    int num3 = rand.Next(1, 6);
                    string text = "Mileth";
                    switch (num3)
                    {
                        case 2:
                            text = "Rucesion";
                            break;
                        case 3:
                            text = "Abel";
                            break;
                        case 4:
                            text = "Piet";
                            break;
                        case 5:
                            text = "Tagor";
                            break;
                    }
                    {
                        foreach (JToken item3 in _mapsDB["inns"].Children())
                        {
                            if (text == item3.Value<string>("nation"))
                            {
                                num = rand.Next((int)item3["dest"]["lowx"], (int)item3["dest"]["highx"] + 1);
                                num2 = rand.Next((int)item3["dest"]["lowy"], (int)item3["dest"]["highy"] + 1);
                                NewMap(item3.Value<int>("mapnum"), num, num2);
                                break;
                            }
                        }
                        break;
                    }
                }
            case "fullheal":
                SpellAnimation(_player, 71, 120);
                _player._curHP = _player._maxHP;
                _player._displayHP = true;
                break;
            case "hemloch":
                _player._curHP = 1;
                SystemMsg("Your body weakens, you can barely stand.", 3);
                break;
            case "confetti":
                BodyMovement(2, 300);
                SpellAnimation(_player, 35, 120);
                break;
            case "equipdye":
                _userMsg = "Click the piece of equipment you wish to dye. 'Esc' to quit.";
                break;
            case "hide":
                UseScript(script, "", checkSpellBar: true, checkHidden: false, castMsg: true, 2);
                break;
            case "ao beag cradh":
            case "ao cradh":
            case "ao mor cradh":
            case "ao ard cradh":
            case "ao beag suain":
                if (script == "ao beag suain")
                {
                    UseScript(script, "assail", checkSpellBar: false, checkHidden: true, castMsg: true, 1);
                }
                else
                {
                    UseScript(script, "cast", checkSpellBar: false, checkHidden: true, castMsg: true, 2);
                }
                break;
            case "beag cradh":
            case "cradh":
            case "mor cradh":
            case "ard cradh":
                UseScript(script, "cast", checkSpellBar: true, checkHidden: true, castMsg: true, 2);
                break;
            case "dion":
            case "mor dion":
            case "iron skin":
            case "mor dion comlha":
                UseScript(script, "cast", checkSpellBar: true, checkHidden: true, castMsg: true, 2);
                break;
            case "mor strioch pian gar":
            case "meteor shower":
            case "lightning storm":
                UseScript(script, "float", checkSpellBar: false, checkHidden: true, castMsg: false, 2);
                break;
            case "mana dance":
                UseScript(script, "mana dance", checkSpellBar: false, checkHidden: true, castMsg: false, 2);
                break;
            default:
                UseScript(script, "cast", checkSpellBar: false, checkHidden: true, castMsg: false, 2);
                break;
        }
    }

    /// <summary>
    ///     The shared spell/skill script executor (the overload all the Scripts(...) calls route through).
    ///     After the dead/ghost check it maps <paramref name="scriptType"/> to a body-animation set and
    ///     delay, then - per <paramref name="skillSpellAction"/> - looks <paramref name="script"/> up in
    ///     _spellsDB (2 = spell) or _skillsDB (1 = skill), reads its data, and applies it: the body/sound/cast
    ///     animations, the spell-bar status, and damage, dispatched by the script's target type (self / gar /
    ///     lamh / tile / target). Honors the checkSpellBar / checkHidden / castMsg / allowRefresh flags.
    /// </summary>
    private void UseScript(string script, string scriptType, bool checkSpellBar = false, bool checkHidden = true, bool castMsg = false, byte skillSpellAction = 2, bool allowRefresh = false)
    {
        if (_player._dead)
        {
            SystemMsg("You are too injured to move.", 3);
            return;
        }
        if (_player.Ghost)
        {
            SystemMsg("Spirits can't do that.", 3);
            return;
        }
        string text = "";
        byte aniType = 0;
        int num = 0;
        // Map the requested script type to its body-animation set (aniType) + delay.
        switch (scriptType)
        {
            case "cast":
                aniType = 2;
                text = "03";
                num = 300;
                break;
            case "assail":
                aniType = 1;
                text = "02";
                num = 200;
                break;
            case "hopak dance":
                aniType = 5;
                text = "b";
                num = 200;
                break;
            case "batusi dance":
                aniType = 6;
                text = "b";
                num = 200;
                break;
            case "dance finisher":
                aniType = 7;
                text = "b";
                num = 200;
                break;
            case "grovel":
                aniType = 8;
                text = "b";
                num = 200;
                break;
            case "throw page":
                aniType = 9;
                text = "b";
                num = 200;
                break;
            case "throw book":
                aniType = 10;
                text = "b";
                num = 200;
                break;
            case "2h":
                aniType = 14;
                text = "c";
                num = 200;
                break;
            case "charge":
                aniType = 15;
                text = "c";
                num = 200;
                break;
            case "lunge":
                aniType = 16;
                text = "c";
                num = 200;
                break;
            case "plunge":
                aniType = 17;
                text = "c";
                num = 200;
                break;
            case "jab":
                aniType = 18;
                text = "c";
                num = 200;
                break;
            case "upstrike":
                aniType = 19;
                text = "c";
                num = 200;
                break;
            case "sideswipe":
                aniType = 20;
                text = "c";
                num = 200;
                break;
            case "kick":
                aniType = 24;
                text = "d";
                num = 200;
                break;
            case "punch":
                aniType = 25;
                text = "d";
                num = 200;
                break;
            case "mantis kick":
                aniType = 26;
                text = "d";
                num = 200;
                break;
            case "flying kick":
                aniType = 27;
                text = "d";
                num = 200;
                break;
            case "elbow":
                aniType = 28;
                text = "d";
                num = 200;
                break;
            case "high kick":
                aniType = 29;
                text = "d";
                num = 200;
                break;
            case "snake fist":
                aniType = 30;
                text = "d";
                num = 200;
                break;
            case "uppercut":
                aniType = 31;
                text = "d";
                num = 200;
                break;
            case "mid kick":
                aniType = 32;
                text = "d";
                num = 200;
                break;
            case "roundhouse":
                aniType = 33;
                text = "d";
                num = 200;
                break;
            case "highest kick":
                aniType = 34;
                text = "d";
                num = 200;
                break;
            case "stab":
                aniType = 35;
                text = "e";
                num = 200;
                break;
            case "stab twice":
                aniType = 36;
                text = "e";
                num = 200;
                break;
            case "stab 1":
                aniType = 37;
                text = "e";
                num = 200;
                break;
            case "stab 2":
                aniType = 38;
                text = "e";
                num = 200;
                break;
            case "stab 3":
                aniType = 39;
                text = "e";
                num = 200;
                break;
            case "stab 4":
                aniType = 40;
                text = "e";
                num = 200;
                break;
            case "taunt":
                aniType = 41;
                text = "e";
                num = 200;
                break;
            case "frighten":
                aniType = 42;
                text = "e";
                num = 200;
                break;
            case "distract":
                aniType = 43;
                text = "e";
                num = 200;
                break;
            case "throw weapon":
                aniType = 44;
                text = "e";
                num = 200;
                break;
            case "myda cast":
                aniType = 49;
                text = "f";
                num = 200;
                break;
            case "mana dance":
                aniType = 50;
                text = "f";
                num = 200;
                break;
            case "stretch":
                aniType = 51;
                text = "f";
                num = 200;
                break;
            case "sand dance":
                aniType = 52;
                text = "f";
                num = 200;
                break;
            case "float":
                aniType = 53;
                text = "f";
                num = 200;
                break;
        }
        if (checkHidden && _player.Hidden)
        {
            _player._spellBar["Invisible"].Remove();
            _player.Hidden = false;
            SendDisplayPlayer();
        }
        bool flag = false;
        // Look the named script up in the data DB and apply it (2 = spell -> _spellsDB, 1 = skill -> _skillsDB).
        switch (skillSpellAction)
        {
            // Spell: find <script> in _spellsDB, read its fields, then dispatch by target type (self/gar/lamh/tile/target).
            case 2:
                foreach (JToken item in _spellsDB["spells"].Children())
                {
                    string text11 = item.Value<string>("name");
                    if (!text11.Equals(script, StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }
                    flag = true;
                    string text12 = item.Value<string>("imgpath");
                    if (string.IsNullOrEmpty(text12))
                    {
                        text12 = "spell001";
                    }
                    int num9 = item.Value<int>("image");
                    int num10 = item.Value<int>("toani");
                    int speed4 = 120;
                    int num11 = item.Value<int>("fromani");
                    int speed5 = 120;
                    string text13 = item.Value<string>("targettype");
                    if (string.IsNullOrEmpty(text13))
                    {
                        text13 = "self";
                    }
                    int num12 = item.Value<int>("seconds");
                    string text14 = item.Value<string>("startmsg");
                    if (string.IsNullOrEmpty(text14))
                    {
                        text14 = "";
                    }
                    string text15 = item.Value<string>("endmsg");
                    if (string.IsNullOrEmpty(text15))
                    {
                        text15 = "";
                    }
                    string text16 = item.Value<string>("remsg");
                    if (string.IsNullOrEmpty(text16))
                    {
                        text16 = "";
                    }
                    string text17 = item.Value<string>("sound");
                    if (string.IsNullOrEmpty(text17))
                    {
                        text17 = "8";
                    }
                    int dmg2 = item.Value<int>("dmg");
                    string value5 = item.Value<string>("element");
                    if (string.IsNullOrEmpty(value5))
                    {
                        value5 = "None";
                    }
                    item.Value<int>("heal");
                    int num13 = item.Value<int>("range");
                    if (num13 == 0)
                    {
                        num13 = 1;
                    }
                    if (!string.IsNullOrEmpty(text13))
                    {
                        switch (text13)
                        {
                            case "self":
                                break;
                            case "gar":
                                {
                                    BodyMovement(aniType, (ushort)num);
                                    PlaySound("effect" + text17);
                                    if (num11 > 0)
                                    {
                                        SpellAnimation(_player, num11, speed5);
                                    }
                                    Entity[] array = _map._entities.Values.ToArray();
                                    foreach (Entity entity7 in array)
                                    {
                                        if (entity7._id == _player._id || _player._location.DistanceFrom(entity7._location) > 13 || entity7 is Item)
                                        {
                                            continue;
                                        }
                                        if (num10 > 0)
                                        {
                                            if (entity7._mBody != null)
                                            {
                                                entity7._mBody.SpellAni(num10, speed4);
                                            }
                                            else if (entity7._body != null)
                                            {
                                                SpellAnimation(entity7, num10, speed4);
                                            }
                                        }
                                        if (castMsg)
                                        {
                                            SystemMsg("You cast " + text11, 3);
                                        }
                                        if (checkSpellBar || allowRefresh)
                                        {
                                            NewSpellBar(script, text12, num9 - 1, num12, text14, text16, text15, entity7);
                                        }
                                        entity7.DamageHealth(dmg2, _player);
                                    }
                                    return;
                                }
                            case "lamh":
                                {
                                    BodyMovement(aniType, (ushort)num);
                                    PlaySound("effect" + text17);
                                    if (num11 > 0)
                                    {
                                        SpellAnimation(_player, num11, speed5);
                                    }
                                    Entity[] array = _map._entities.Values.ToArray();
                                    foreach (Entity entity6 in array)
                                    {
                                        if (entity6._id == _player._id || _player._location.DistanceFrom(entity6._location) > num13 || entity6 is Item)
                                        {
                                            continue;
                                        }
                                        if (num10 > 0)
                                        {
                                            if (entity6._mBody != null)
                                            {
                                                entity6._mBody.SpellAni(num10, speed4);
                                            }
                                            else if (entity6._body != null)
                                            {
                                                SpellAnimation(entity6, num10, speed4);
                                            }
                                        }
                                        if (castMsg)
                                        {
                                            SystemMsg("You cast " + text11, 3);
                                        }
                                        if (checkSpellBar || allowRefresh)
                                        {
                                            NewSpellBar(script, text12, num9 - 1, num12, text14, text16, text15, entity6);
                                        }
                                        entity6.DamageHealth(dmg2, _player);
                                    }
                                    return;
                                }
                            default:
                                _ = text13 == "tile";
                                return;
                            case "target":
                                return;
                        }
                    }
                    if (checkSpellBar && _player._spellBar.ContainsKey(script))
                    {
                        return;
                    }
                    if (text != "")
                    {
                        BodyMovement(aniType, (ushort)num);
                    }
                    PlaySound("effect" + text17);
                    if (num11 > 0)
                    {
                        SpellAnimation(_player, num11, speed5);
                    }
                    if (num10 > 0)
                    {
                        if (_player._inMonsterForm && _player._mBody != null)
                        {
                            _player._mBody.SpellAni(num10, speed4);
                        }
                        else if (_player._body != null)
                        {
                            SpellAnimation(_player, num10, speed4);
                        }
                    }
                    if (castMsg)
                    {
                        SystemMsg("You cast " + text11, 3);
                    }
                    if (checkSpellBar || allowRefresh)
                    {
                        NewSpellBar(script, text12, num9 - 1, num12, text14, text16, text15, _player);
                    }
                    return;
                }
                break;
            // Skill: find <script> in _skillsDB and apply (same shape as the spell case).
            case 1:
                foreach (JToken item2 in _skillsDB["skills"].Children())
                {
                    string text4 = item2.Value<string>("name");
                    if (!text4.Equals(script, StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }
                    flag = true;
                    string text5 = item2.Value<string>("imgpath");
                    int num4 = item2.Value<int>("image");
                    int num5 = item2.Value<int>("toani");
                    int speed2 = 120;
                    int num6 = item2.Value<int>("fromani");
                    int speed3 = 120;
                    string text6 = item2.Value<string>("targettype");
                    if (string.IsNullOrEmpty(text6))
                    {
                        text6 = "facing";
                    }
                    int num7 = item2.Value<int>("range");
                    if (num7 == 0)
                    {
                        num7 = 1;
                    }
                    int dmg = item2.Value<int>("dmg");
                    int num8 = item2.Value<int>("seconds");
                    string text7 = item2.Value<string>("startmsg");
                    string text8 = item2.Value<string>("endmsg");
                    string text9 = item2.Value<string>("remsg");
                    if (string.IsNullOrEmpty(text9))
                    {
                        text9 = "";
                    }
                    if (string.IsNullOrEmpty(text5))
                    {
                        text5 = "skill001";
                    }
                    if (string.IsNullOrEmpty(text7))
                    {
                        text7 = "";
                    }
                    if (string.IsNullOrEmpty(text8))
                    {
                        text8 = "";
                    }
                    string text10 = item2.Value<string>("sound");
                    if (string.IsNullOrEmpty(text10))
                    {
                        text10 = "1";
                    }
                    BodyMovement(aniType, (ushort)num);
                    PlaySound("effect" + text10);
                    if (num6 > 0)
                    {
                        SpellAnimation(_player, num6, speed3);
                    }
                    Entity[] array;
                    switch (text6)
                    {
                        case "facing":
                            {
                                Tile tile = TileImFacing();
                                if (tile == null)
                                {
                                    break;
                                }
                                array = tile._entities.Values.ToArray();
                                foreach (Entity entity3 in array)
                                {
                                    if (entity3 is Item)
                                    {
                                        continue;
                                    }
                                    if (num5 > 0)
                                    {
                                        if (entity3._mBody != null)
                                        {
                                            entity3._mBody.SpellAni(num5, speed2);
                                        }
                                        else if (entity3._body != null)
                                        {
                                            SpellAnimation(entity3, num5, speed2);
                                        }
                                    }
                                    if (castMsg)
                                    {
                                        SystemMsg("You cast " + text4, 3);
                                    }
                                    if (checkSpellBar || allowRefresh)
                                    {
                                        NewSpellBar(script, text5, num4 - 1, num8, text7, text9, text8, entity3);
                                    }
                                    entity3.DamageHealth(dmg, _player);
                                }
                                break;
                            }
                        case "line":
                            array = _map._entities.Values.ToArray();
                            foreach (Entity entity in array)
                            {
                                if (entity._id == _player._id || !_player._location.InLine(entity._location, (D)_player._body._direction, num7) || entity is Item)
                                {
                                    continue;
                                }
                                if (num5 > 0)
                                {
                                    if (entity._mBody != null)
                                    {
                                        entity._mBody.SpellAni(num5, speed2);
                                    }
                                    else if (entity._body != null)
                                    {
                                        SpellAnimation(entity, num5, speed2);
                                    }
                                }
                                if (castMsg)
                                {
                                    SystemMsg("You cast " + text4, 3);
                                }
                                if (checkSpellBar || allowRefresh)
                                {
                                    NewSpellBar(script, text5, num4 - 1, num8, text7, text9, text8, entity);
                                }
                                entity.DamageHealth(dmg, _player);
                            }
                            break;
                        case "lamh":
                            array = _map._entities.Values.ToArray();
                            foreach (Entity entity2 in array)
                            {
                                if (entity2._id == _player._id || _player._location.DistanceFrom(entity2._location) > num7 || entity2 is Item)
                                {
                                    continue;
                                }
                                if (num5 > 0)
                                {
                                    if (entity2._mBody != null)
                                    {
                                        entity2._mBody.SpellAni(num5, speed2);
                                    }
                                    else if (entity2._body != null)
                                    {
                                        SpellAnimation(entity2, num5, speed2);
                                    }
                                }
                                if (castMsg)
                                {
                                    SystemMsg("You cast " + text4, 3);
                                }
                                if (checkSpellBar || allowRefresh)
                                {
                                    NewSpellBar(script, text5, num4 - 1, num8, text7, text9, text8, entity2);
                                }
                                entity2.DamageHealth(dmg, _player);
                            }
                            break;
                        case "self":
                            if (num5 > 0)
                            {
                                if (_player._inMonsterForm && _player._mBody != null)
                                {
                                    _player._mBody.SpellAni(num5, speed2);
                                }
                                else if (_player._body != null)
                                {
                                    SpellAnimation(_player, num5, speed2);
                                }
                            }
                            if (castMsg)
                            {
                                SystemMsg("You cast " + text4, 3);
                            }
                            if (checkSpellBar || allowRefresh)
                            {
                                NewSpellBar(script, text5, num4 - 1, num8, text7, text9, text8, _player);
                            }
                            break;
                    }
                    if (script == "throw")
                    {
                        Tile tile2 = TileImFacing();
                        if (tile2 != null)
                        {
                            array = tile2._entities.Values.ToArray();
                            foreach (Entity entity4 in array)
                            {
                                if (!(entity4 is Item))
                                {
                                    entity4.MoveATile(lastArrowKeyDirection, walk: false);
                                    entity4.CenterEntity();
                                    break;
                                }
                            }
                        }
                    }
                    if (lastArrowKeyDirection == byte.MaxValue || !(lastArrowKeyPressDT != DateTime.MinValue) || !(DateTime.UtcNow.Subtract(lastArrowKeyPressDT).TotalMilliseconds <= 200.0))
                    {
                        return;
                    }
                    Tile tile3 = TileImFacing();
                    if (tile3 == null)
                    {
                        return;
                    }
                    array = tile3._entities.Values.ToArray();
                    foreach (Entity entity5 in array)
                    {
                        if (entity5 is NPC)
                        {
                            entity5.MoveATile(lastArrowKeyDirection, walk: false);
                            entity5.CenterEntity();
                            break;
                        }
                    }
                    return;
                }
                break;
            case 3:
                foreach (JToken item3 in _actionsDB["actions"].Children())
                {
                    string text2 = item3.Value<string>("name");
                    if (text2.Equals(script, StringComparison.CurrentCultureIgnoreCase))
                    {
                        flag = true;
                        string value = item3.Value<string>("imgpath");
                        item3.Value<int>("image");
                        item3.Value<int>("toani");
                        int num2 = item3.Value<int>("fromani");
                        int speed = 120;
                        string value2 = item3.Value<string>("targettype");
                        if (string.IsNullOrEmpty(value2))
                        {
                            value2 = "facing";
                        }
                        item3.Value<int>("range");
                        item3.Value<int>("dmg");
                        item3.Value<int>("seconds");
                        string value3 = item3.Value<string>("startmsg");
                        string value4 = item3.Value<string>("endmsg");
                        if (string.IsNullOrEmpty(value))
                        {
                            value = "skill001";
                        }
                        if (string.IsNullOrEmpty(value3))
                        {
                            value3 = "";
                        }
                        if (string.IsNullOrEmpty(value4))
                        {
                            value4 = "";
                        }
                        string text3 = item3.Value<string>("sound");
                        if (string.IsNullOrEmpty(text3))
                        {
                            text3 = "1";
                        }
                        BodyMovement(aniType, (ushort)num);
                        PlaySound("effect" + text3);
                        if (num2 > 0)
                        {
                            SpellAnimation(_player, num2, speed);
                        }
                        return;
                    }
                }
                break;
        }
        if (!flag)
        {
            SystemMsg(script + " - not found in database.", 3);
        }
    }

    private void Emote(byte aniType, ushort aniDelay, bool repeatOnce = false)
    {
        if (_player._dead)
        {
            SystemMsg("You are too injured to move.", 3);
        }
        else if (_player.Ghost)
        {
            SystemMsg("Spirits can't do that.", 3);
        }
        else if (_player._inMonsterForm)
        {
            _player._mBody.Attack();
        }
        else if (!_player._body._emoting && !_player._body._attacking)
        {
            if (GameWindow.ConnectedToServer)
            {
                BodyMovementPacket bodyMovementPacket = new BodyMovementPacket(_player._id, aniType, aniDelay);
                GameWindow.ClientSocket.Send(bodyMovementPacket.Data);
            }
            getAniInfo(aniType, out var bodyAniType, out var aniStartFrame, out var aniEndFrame);
            if (aniType == 3 || aniType == 4)
            {
                _player._body.SetAni(bodyAniType, aniDelay, aniStartFrame, aniEndFrame, repeatOnce);
                _player._body._attacking = true;
            }
            else
            {
                _player._body.Emote(aniDelay, aniStartFrame, aniEndFrame);
                _player._body._emoting = true;
            }
        }
    }

    private void BodyMovement(byte aniType, ushort aniDelay)
    {
        if (aniType == 0)
        {
            return;
        }
        if (_player._dead)
        {
            SystemMsg("You are too injured to move.", 3);
        }
        else if (_player.Ghost)
        {
            SystemMsg("Spirits can't do that.", 3);
        }
        else if (_player._inMonsterForm)
        {
            if (aniType == 2)
            {
                _player._mBody.Attack2();
            }
            else
            {
                _player._mBody.Attack();
            }
        }
        else if (!_player._body._attacking)
        {
            if (GameWindow.ConnectedToServer)
            {
                BodyMovementPacket bodyMovementPacket = new BodyMovementPacket(_player._id, aniType, aniDelay);
                GameWindow.ClientSocket.Send(bodyMovementPacket.Data);
            }
            getAniInfo(aniType, out var bodyAniType, out var aniStartFrame, out var aniEndFrame);
            _player._body.SetAni(bodyAniType, aniDelay, aniStartFrame, aniEndFrame);
            _player._body._attacking = true;
        }
    }

    public void SpellAnimation(Entity en, int ani, int speed)
    {
        if (GameWindow.ConnectedToServer)
        {
            SpellAniS spellAni = default(SpellAniS);
            spellAni.ToID = en._id;
            spellAni.ToAni = (ushort)ani;
            spellAni.Delay = (ushort)speed;
            spellAni.FromID = 0u;
            spellAni.FromAni = 0;
            spellAni.X = 0;
            spellAni.Y = 0;
            SpellAnimationPacket spellAnimationPacket = new SpellAnimationPacket(spellAni);
            GameWindow.ClientSocket.Send(spellAnimationPacket.Data);
        }
        if (_enableSpells)
        {
            if ((en is Player && (en as Player)._inMonsterForm && en._mBody != null) || (!(en is Player) && en._mBody != null))
            {
                en._mBody.SpellAni(ani, speed);
            }
            else if (en._body != null)
            {
                en._body.SpellAni(ani, speed);
            }
        }
    }

    /// <summary>
    ///     Opens / advances the standard NPC dialog popup. Resets the popup, looks the NPC's script up by
    ///     <paramref name="scriptName"/> in the dialog DB, and reads the node chosen by <paramref name="scriptNum"/>:
    ///     it substitutes $variables in the text and applies the node's effects (teleport, spell animation,
    ///     legend mark, give items, menu options, text responses). The remainder dispatches the built-in
    ///     merchant / training dialogs by name: Buy, Sell, Deposit/Withdraw Item, Learn and Forget
    ///     Skill/Spell/Action, Hairstyle, Dye Hair, and Dye Equipment. The // === Handler: ... === markers
    ///     below delimit each. (Data-driven; JSON-soup locals left for a future deeper pass.)
    /// </summary>
    public void DialogPopup(Entity en, string scriptName, byte scriptNum = 0, Texture? img = null)
    {
        _tTT = "";
        _eTT = "";
        _input.Mouse.SetCursorDefault();
        bool flag = false;
        bool flag2 = false;
        bool flag3 = false;
        bool flag4 = false;
        bool flag5 = false;
        bool flag6 = false;
        string text = "";
        if (_standardDialogPopup._disButtons.ContainsKey("chatScrollUpBtn") && _standardDialogPopup._disButtons["chatScrollUpBtn"]._clicked)
        {
            flag2 = true;
        }
        if (_standardDialogPopup._disButtons.ContainsKey("chatScrollDownBtn") && _standardDialogPopup._disButtons["chatScrollDownBtn"]._clicked)
        {
            flag3 = true;
        }
        if (_standardDialogPopup._disButtons.ContainsKey("chatScrollerBtn") && _standardDialogPopup._disButtons["chatScrollerBtn"]._clicked)
        {
            flag = true;
        }
        if (_standardDialogPopup._disButtons.ContainsKey("catScrollerBtn") && _standardDialogPopup._disButtons["catScrollerBtn"]._clicked)
        {
            flag4 = true;
        }
        if (_standardDialogPopup._disButtons.ContainsKey("catScrollUpBtn") && _standardDialogPopup._disButtons["catScrollUpBtn"]._clicked)
        {
            flag5 = true;
        }
        if (_standardDialogPopup._disButtons.ContainsKey("catScrollDownBtn") && _standardDialogPopup._disButtons["catScrollDownBtn"]._clicked)
        {
            flag6 = true;
        }
        string text2 = "default";
        _standardDialogPopup._height = 180.0;
        _standardDialogPopup._catlist.Clear();
        _standardDialogPopup._withdrawcatlist.Clear();
        _standardDialogPopup._dyelist.Clear();
        _standardDialogPopup._buylist.Clear();
        _standardDialogPopup._selllist.Clear();
        _standardDialogPopup._withdrawlist.Clear();
        _standardDialogPopup._depositlist.Clear();
        _standardDialogPopup._learnskills.Clear();
        _standardDialogPopup._learnspells.Clear();
        _standardDialogPopup._learnactions.Clear();
        _standardDialogPopup._backgrounds.Clear();
        _standardDialogPopup._disButtons.Clear();
        _standardDialogPopup._disLabels.Clear();
        _standardDialogPopup._disTextFields.Clear();
        _standardDialogPopup._buttons["sdpNextBtn"]._onPressEvent = delegate
        {
        };
        _standardDialogPopup._buttons["sdpNextBtn"].Hidden = false;
        _standardDialogPopup._buttons["sdpNextBtn"].Enabled = false;
        _standardDialogPopup._buttons["sdpPreviousBtn"]._onPressEvent = delegate
        {
        };
        _standardDialogPopup._buttons["sdpPreviousBtn"].Hidden = false;
        _standardDialogPopup._buttons["sdpPreviousBtn"].Enabled = false;
        _standardDialogPopup._buttons["sdpTopBtn"]._onPressEvent = delegate
        {
        };
        _standardDialogPopup._buttons["sdpTopBtn"].Hidden = true;
        if (img.HasValue)
        {
            _standardDialogPopup._sprite._flip = true;
            _standardDialogPopup._sprite.Texture = img.Value;
        }
        if (scriptName != "Withdraw Item")
        {
            _withdrawItemDialogIndex = 0;
        }
        if (scriptName != "Learn Action")
        {
            _lrnActionDialogIndex = 0;
        }
        if (scriptName != "Learn Spell")
        {
            _lrnSpellDialogIndex = 0;
        }
        if (scriptName != "Learn Skill")
        {
            _lrnSkillDialogIndex = 0;
        }
        if (scriptName != "Buy")
        {
            _buyDialogIndex = 0;
        }
        if (scriptName == "Sell" && scriptNum == 2)
        {
            InventoryItem itemSlot = getItemSlot(_standardDialogPopup._sellitem);
            int num = itemSlot._value / 2;
            if (questvars.ContainsKey("sellvalue"))
            {
                questvars["sellvalue"] = (num * int.Parse(questvars["sellnumber"])).ToString();
            }
            else
            {
                questvars.Add("sellvalue", (num * int.Parse(questvars["sellnumber"])).ToString());
            }
        }
        if (scriptName != "Dye Equipment")
        {
            _dye2DialogIndex = 0;
            _dyeDialogIndex = 0;
        }
        double num2 = 0.0;
        string newtext = "";
        if (en != null)
        {
            newtext = en._name;
        }
        _standardDialogPopup._labels["sdpNameLabel"].ChangeText(newtext);
        bool flag7 = false;
        foreach (JToken item3 in _dialogDB["dialogue"].Children())
        {
            string text3 = item3.Value<string>("name");
            if (!(text3 == scriptName))
            {
                continue;
            }
            string text4 = item3.Value<string>("activevar");
            if (!string.IsNullOrEmpty(text4) && questvars.ContainsKey(text4) && scriptNum <= byte.Parse(questvars[text4]))
            {
                scriptNum = byte.Parse(questvars[text4]);
            }
            JToken jToken = item3[scriptNum.ToString()];
            if (jToken == null)
            {
                break;
            }
            flag7 = true;
            string next = jToken.Value<string>("next");
            string previous = jToken.Value<string>("previous");
            string text5 = jToken.Value<string>("text");
            if (string.IsNullOrEmpty(text5))
            {
                text5 = "";
            }
            if (text5.Contains("$gender"))
            {
                string newValue = "his";
                if (_player._gender == 0)
                {
                    newValue = "her";
                }
                text5 = text5.Replace("$gender", newValue);
            }
            else if (text5.Contains("$bankedgold"))
            {
                text5 = text5.Replace("$bankedgold", _playerbankedgold.ToString("#,0"));
            }
            else if (text5.Contains("$"))
            {
                string[] array = text5.Split();
                string[] array2 = array;
                foreach (string text6 in array2)
                {
                    string text7 = text6.Trim(' ', ',', '.', '!', '\'', '"', '?');
                    if (text6.StartsWith("$") && text5.Contains(text7) && questvars.ContainsKey(text7.Substring(1)))
                    {
                        text5 = text5.Replace(text7, questvars[text7.Substring(1)]);
                    }
                }
            }
            string text8 = jToken.Value<string>("info");
            // --- Node effect: teleport the player ---
            JToken jToken2 = jToken["teleport"];
            bool flag8 = jToken.Value<bool>("arenadeath");
            bool flag9 = jToken.Value<bool>("arenaresurrect");
            if (flag8)
            {
                _player.ClearSpellBar();
                _player._curHP = 0;
                _player.Ghost = true;
                SendDisplayPlayer();
            }
            if (flag9)
            {
                _player._curHP = _player._maxHP;
                _player.Ghost = false;
                SendDisplayPlayer();
            }
            string text9 = jToken.Value<string>("speak");
            if (string.IsNullOrEmpty(text9))
            {
                text9 = "";
            }
            if (text9 != "")
            {
                DisplayChat(0, en._id, en._name + ": " + text9);
            }
            string text10 = jToken.Value<string>("action");
            string text11 = jToken.Value<string>("depositgold");
            if (!string.IsNullOrEmpty(text11))
            {
                int num3 = int.Parse(questvars[text11]);
                InventoryItem itemSlot2 = getItemSlot(72);
                if (itemSlot2._amount >= num3)
                {
                    if (_playerbankedgold + num3 <= 1000000000)
                    {
                        itemSlot2._amount -= num3;
                        _playerbankedgold += num3;
                        DisplayChat(0, en._id, en._name + ": You have deposited " + num3.ToString("#,0") + " coins.");
                    }
                    else
                    {
                        DisplayChat(0, en._id, en._name + ": I cannot hold any more of your gold.");
                    }
                }
                else
                {
                    DisplayChat(0, en._id, en._name + ": Your broke ass doesn't have it.");
                }
            }
            string text12 = jToken.Value<string>("withdrawgold");
            if (!string.IsNullOrEmpty(text12))
            {
                int num4 = int.Parse(questvars[text12]);
                InventoryItem itemSlot3 = getItemSlot(72);
                if (_playerbankedgold >= num4)
                {
                    if (itemSlot3._amount + num4 <= 100000000)
                    {
                        _playerbankedgold -= num4;
                        itemSlot3._amount += num4;
                        DisplayChat(0, en._id, en._name + ": Here is your " + num4.ToString("#,0") + " coins.");
                    }
                    else
                    {
                        DisplayChat(0, en._id, en._name + ": You cannot hold that much gold.");
                    }
                }
                else
                {
                    DisplayChat(0, en._id, en._name + ": You never gave me that much in the first place.");
                }
            }
            // --- Node effect: play a spell animation ---
            JToken jToken3 = jToken["spellani"];
            if (jToken3 != null)
            {
                string text13 = jToken3.Value<string>("target");
                int ani = jToken3.Value<int>("ani");
                int speed = jToken3.Value<int>("speed");
                if (string.IsNullOrEmpty(text13))
                {
                    text13 = "";
                }
                if (text13 == "player")
                {
                    SpellAnimation(_player, ani, speed);
                }
                else
                {
                    SpellAnimation(en, ani, speed);
                }
            }
            // --- Node effect: grant a legend mark ---
            JToken jToken4 = jToken["newlegend"];
            if (jToken4 != null)
            {
                int icon = jToken4.Value<int>("icon");
                string id = jToken4.Value<string>("type");
                string text14 = jToken4.Value<string>("text");
                int color = jToken4.Value<int>("color");
                NewLegendMark(icon, id, text14, color);
            }
            // --- Node effect: give items ---
            JToken jToken5 = jToken["newitems"];
            if (jToken5 != null)
            {
                for (int j = 1; j <= jToken5.Children().Count(); j++)
                {
                    JToken jToken6 = jToken5[j.ToString()];
                    string text15 = jToken6.Value<string>("name");
                    byte b = 1;
                    string text16 = jToken6.Value<string>("stack");
                    if (string.IsNullOrEmpty(text16))
                    {
                        text16 = "1";
                    }
                    if (text16.Contains("-"))
                    {
                        string[] array3 = text16.Split('-');
                        b = (byte)rand.Next(byte.Parse(array3[0]), byte.Parse(array3[1]) + 1);
                    }
                    else
                    {
                        b = byte.Parse(text16);
                    }
                    byte b2 = jToken6.Value<byte>("rate");
                    if (b2 == 0)
                    {
                        b2 = 100;
                    }
                    int num5 = rand.Next(0, 100);
                    if (num5 <= b2)
                    {
                        if (text15.Contains(" e:"))
                        {
                            string text17 = text15.Substring(text15.IndexOf(" e:") + 3);
                            text15 = text15.Remove(text15.IndexOf(" e:"));
                            NewItem(text15, 0, b, 0, 0, text17);
                            SystemMsg(text17 + " " + text15 + " received!", 3);
                        }
                        else
                        {
                            NewItem(text15, 0, b);
                            SystemMsg(text15 + " received!", 3);
                        }
                    }
                }
            }
            text = jToken.Value<string>("img");
            if (string.IsNullOrEmpty(text))
            {
                text = "";
            }
            if (text != "")
            {
                if (text.StartsWith("item"))
                {
                    _standardDialogPopup._sprite.SetPosition(0.0, 0.0);
                    _standardDialogPopup._sprite._flip = true;
                    _standardDialogPopup._sprite.Texture = _textureManager.Get(text + "_C0");
                }
                else
                {
                    MonsterBody monsterBody = new MonsterBody(_textureManager, text, "old");
                    _standardDialogPopup._sprite._flip = true;
                    _standardDialogPopup._sprite.Texture = monsterBody._imgArr[monsterBody._face - 1];
                }
            }
            int num6 = jToken.Value<int>("exp");
            if (num6 != 0)
            {
                GainExp((uint)num6);
            }
            string var = jToken.Value<string>("var");
            string text18 = jToken.Value<string>("activevar");
            string text19 = jToken.Value<string>("savevar");
            if (!string.IsNullOrEmpty(text19))
            {
                if (questvars.ContainsKey(text19))
                {
                    if (!string.IsNullOrEmpty(var))
                    {
                        questvars[text19] = var;
                    }
                    else
                    {
                        questvars[text19] = scriptNum.ToString();
                    }
                }
                else if (!string.IsNullOrEmpty(var))
                {
                    questvars.Add(text19, var);
                }
                else
                {
                    questvars.Add(text19, scriptNum.ToString());
                }
            }
            if (var == "altarcount")
            {
                if (!questvars.ContainsKey(var))
                {
                    questvars.Add(var, "0");
                }
                int num7 = int.Parse(questvars[var]);
                num7++;
                questvars[var] = num7.ToString();
                if (num7 >= 20)
                {
                    questvars[var] = "0";
                }
                SaveQuest(var, questvars[var]);
                if (num7 >= 20)
                {
                    DialogPopup(en, scriptName, 3);
                }
                else if (num7 == 10)
                {
                    DialogPopup(en, scriptName, 2);
                }
                else
                {
                    DialogPopup(en, scriptName, 1);
                }
                return;
            }
            string text20 = jToken.Value<string>("input");
            if (!string.IsNullOrEmpty(text20))
            {
                text2 = "input";
                byte b3 = jToken.Value<byte>("maxchars");
                string text21 = jToken.Value<string>("pretext");
                string text22 = jToken.Value<string>("posttext");
                string text23 = jToken.Value<string>("defaulttext");
                if (string.IsNullOrEmpty(text23))
                {
                    text23 = "";
                }
                _standardDialogPopup._background.Texture = _textureManager.Get("mertop_F0_C0");
                num2 = 0.0;
                for (int k = 0; k < 2; k++)
                {
                    Sprite sprite = new Sprite();
                    sprite.Texture = _textureManager.Get("mermid_F0_C0");
                    sprite._windowOffset = new Vector(0.0, 117.0 + num2, 0.0);
                    sprite.SetPosition(sprite._windowOffset.X + _standardDialogPopup._position.X, sprite._windowOffset.Y + _standardDialogPopup._position.Y);
                    _standardDialogPopup._backgrounds.Add("mid" + k, sprite);
                    num2 += 30.0;
                }
                _standardDialogPopup._height = 180.0 + num2;
                if (!string.IsNullOrEmpty(text21))
                {
                    Vector vector = new Vector(30.0, 117.0, 0.0);
                    Text value = DrawLabel(text21, Engine.Color.White, vector.X, vector.Y, 450, "left", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                    _standardDialogPopup._disLabels.Add("preTT", value);
                }
                Rectangle rect = new Rectangle(130, 142, b3 * 6 + 6, 13);
                TextField textField = new TextField(rect, _font, text23, b3, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                textField._showBack = true;
                textField._windowOffset = new Vector(130.0, 142.0, 0.0);
                switch (text20)
                {
                    case "alpha":
                        textField._alphaonly = true;
                        break;
                    case "numeral":
                        textField._numbersonly = true;
                        break;
                    case "alpha apost":
                        textField._alphaonly = true;
                        textField._allowApostrophe = true;
                        break;
                }
                textField._textObj.SetColor(Engine.Color.Black);
                textField.SetPosition(130.0 + _standardDialogPopup._position.X, 142.0 + _standardDialogPopup._position.Y);
                _standardDialogPopup._disTextFields.Add("inputTF", textField);
                if (!string.IsNullOrEmpty(text22))
                {
                    Vector vector2 = new Vector(30.0, 167.0, 0.0);
                    Text value2 = DrawLabel(text22, Engine.Color.White, vector2.X, vector2.Y, 450, "left", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                    _standardDialogPopup._disLabels.Add("postTT", value2);
                }
                Sprite sprite2 = new Sprite();
                sprite2._windowOffset = new Vector(0.0, 117.0 + num2, 0.0);
                sprite2.Texture = _textureManager.Get("merbot_F0_C0");
                sprite2.SetPosition(sprite2._windowOffset.X + _standardDialogPopup._position.X, sprite2._windowOffset.Y + _standardDialogPopup._position.Y);
                _standardDialogPopup._backgrounds.Add("bot", sprite2);
                _standardDialogPopup._buttons["sdpNextBtn"].Position = new Vector(90.0 + _standardDialogPopup._position.X, 143.0 + num2 + _standardDialogPopup._position.Y, 0.0);
                _standardDialogPopup._buttons["sdpPreviousBtn"].Position = new Vector(20.0 + _standardDialogPopup._position.X, 143.0 + num2 + _standardDialogPopup._position.Y, 0.0);
                _standardDialogPopup._buttons["sdpQuitBtn"].Position = new Vector(364.0 + _standardDialogPopup._position.X, 143.0 + num2 + _standardDialogPopup._position.Y, 0.0);
                _standardDialogPopup._buttons["sdpNextBtn"]._windowOffset = new Vector(90.0, 143.0 + num2, 0.0);
                _standardDialogPopup._buttons["sdpPreviousBtn"]._windowOffset = new Vector(20.0, 143.0 + num2, 0.0);
                _standardDialogPopup._buttons["sdpQuitBtn"]._windowOffset = new Vector(364.0, 143.0 + num2, 0.0);
            }
            if (!string.IsNullOrEmpty(text8))
            {
                if (text8.Contains("$"))
                {
                    string[] array4 = text8.Split();
                    string[] array2 = array4;
                    foreach (string text24 in array2)
                    {
                        string text25 = text24.Trim(' ', ',', '.', '!', '\'', '"', '?');
                        if (text24.StartsWith("$") && text8.Contains(text25) && questvars.ContainsKey(text25.Substring(1)))
                        {
                            text8 = text8.Replace(text25, questvars[text25.Substring(1)]);
                        }
                    }
                }
                SystemMsg(text8, 3);
            }
            if (jToken2 != null)
            {
                if (jToken2.Value<bool>("home"))
                {
                    ReturnHome();
                }
                else
                {
                    NewMap(jToken2.Value<int>("mapnum"), jToken2.Value<int>("x"), jToken2.Value<int>("y"));
                }
            }
            // --- Node: menu options (the player's choices) ---
            JToken jToken7 = jToken["options"];
            if (jToken7 != null)
            {
                text2 = "list";
                _standardDialogPopup._background.Texture = _textureManager.Get("mertop_F0_C0");
                int num8 = 0;
                for (int l = 0; l < jToken7.Children().Count(); l++)
                {
                    string listItemScript = jToken7[(l + 1).ToString()].Value<string>("script");
                    string listItemNext = jToken7[(l + 1).ToString()].Value<string>("next");
                    string listItemText = jToken7[(l + 1).ToString()].Value<string>("text");
                    string listItemAction = jToken7[(l + 1).ToString()].Value<string>("action");
                    string listItemInfo = jToken7[(l + 1).ToString()].Value<string>("info");
                    string listItemVar = jToken7[(l + 1).ToString()].Value<string>("var");
                    byte b4 = jToken7[(l + 1).ToString()].Value<byte>("trigger");
                    if (b4 == 1)
                    {
                        continue;
                    }
                    JToken listnewlegend = jToken7[(l + 1).ToString()]["newlegend"];
                    JToken listteleport = jToken7[(l + 1).ToString()]["teleport"];
                    bool listdeath = jToken7[(l + 1).ToString()].Value<bool>("arenadeath");
                    bool listresurrect = jToken7[(l + 1).ToString()].Value<bool>("arenaresurrect");
                    Sprite sprite3 = new Sprite();
                    sprite3.Texture = _textureManager.Get("mermid_F0_C0");
                    sprite3._windowOffset = new Vector(0.0, 117.0 + num2, 0.0);
                    sprite3.SetPosition(sprite3._windowOffset.X + _standardDialogPopup._position.X, sprite3._windowOffset.Y + _standardDialogPopup._position.Y);
                    _standardDialogPopup._backgrounds.Add("mid" + l, sprite3);
                    Vector vector3 = new Vector(0.0, 128.0 + num2, 0.0);
                    Text text26 = DrawLabel(listItemText, Engine.Color.White, vector3.X, vector3.Y, 450, "center", 0, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                    Engine.Button button = new Engine.Button(_textureManager, 42.0, 124.0 + num2, 369.0, 18.0, "buttonex_F0", "buttonex_F1", "", "", null, text26, 0, multHorzontal: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                    button._onPressEvent = delegate
                    {
                        if (!string.IsNullOrEmpty(listItemVar))
                        {
                            if (questvars.ContainsKey(listItemVar))
                            {
                                questvars[listItemVar] = listItemText;
                            }
                            else
                            {
                                questvars.Add(listItemVar, listItemText);
                            }
                        }
                        if (!string.IsNullOrEmpty(listItemInfo))
                        {
                            SystemMsg(listItemInfo, 3);
                        }
                        if (listnewlegend != null)
                        {
                            int icon2 = listnewlegend.Value<int>("icon");
                            string id2 = listnewlegend.Value<string>("type");
                            string text55 = listnewlegend.Value<string>("text");
                            int color2 = listnewlegend.Value<int>("color");
                            NewLegendMark(icon2, id2, text55, color2);
                        }
                        if (listteleport != null)
                        {
                            if (listteleport.Value<bool>("home"))
                            {
                                ReturnHome();
                            }
                            else
                            {
                                NewMap(listteleport.Value<int>("mapnum"), listteleport.Value<int>("x"), listteleport.Value<int>("y"));
                            }
                        }
                        if (listdeath)
                        {
                            _player.ClearSpellBar();
                            _player._curHP = 0;
                            _player.Ghost = true;
                        }
                        if (listresurrect)
                        {
                            _player._curHP = _player._maxHP;
                            _player.Ghost = false;
                        }
                        if (listItemAction == "close")
                        {
                            _viewingDialog = false;
                        }
                        else if (listItemAction == "default")
                        {
                            DialogPopup(en, en._name, 0, img);
                        }
                        else if (!string.IsNullOrEmpty(listItemNext))
                        {
                            DialogPopup(en, scriptName, byte.Parse(listItemNext), img);
                        }
                        else if (!string.IsNullOrEmpty(listItemScript))
                        {
                            DialogPopup(en, listItemScript, 0, img);
                        }
                        else
                        {
                            DialogPopup(en, listItemText, 0, img);
                        }
                    };
                    _standardDialogPopup._disButtons.Add("btn" + l, button);
                    num2 += 30.0;
                    num8++;
                }
                if (num8 == 1 && scriptName == en._name)
                {
                    DialogPopup(en, _standardDialogPopup._disButtons["btn0"]._text._text, 0, img);
                    return;
                }
                _standardDialogPopup._height = 180.0 + num2;
                Sprite sprite4 = new Sprite();
                sprite4._windowOffset = new Vector(0.0, 117.0 + num2, 0.0);
                sprite4.Texture = _textureManager.Get("merbot_F0_C0");
                sprite4.SetPosition(sprite4._windowOffset.X + _standardDialogPopup._position.X, sprite4._windowOffset.Y + _standardDialogPopup._position.Y);
                _standardDialogPopup._backgrounds.Add("bot", sprite4);
                _standardDialogPopup._buttons["sdpNextBtn"]._windowOffset = new Vector(90.0, 143.0 + num2, 0.0);
                _standardDialogPopup._buttons["sdpNextBtn"].Position = new Vector(90.0 + _standardDialogPopup._position.X, 143.0 + num2 + _standardDialogPopup._position.Y, 0.0);
                _standardDialogPopup._buttons["sdpPreviousBtn"]._windowOffset = new Vector(20.0, 143.0 + num2, 0.0);
                _standardDialogPopup._buttons["sdpPreviousBtn"].Position = new Vector(20.0 + _standardDialogPopup._position.X, 143.0 + num2 + _standardDialogPopup._position.Y, 0.0);
                _standardDialogPopup._buttons["sdpQuitBtn"]._windowOffset = new Vector(364.0, 143.0 + num2, 0.0);
                _standardDialogPopup._buttons["sdpQuitBtn"].Position = new Vector(364.0 + _standardDialogPopup._position.X, 143.0 + num2 + _standardDialogPopup._position.Y, 0.0);
            }
            // --- Node: text-input responses ---
            JToken jToken8 = jToken["responses"];
            if (jToken8 != null)
            {
                int num9 = rand.Next(1, jToken8.Children().Count() + 1);
                if (!string.IsNullOrEmpty(text18) && questvars.ContainsKey(text18))
                {
                    num9 = byte.Parse(questvars[text18]);
                }
                JToken jToken9 = jToken8[num9.ToString()];
                text5 = jToken9.Value<string>("text");
                text8 = jToken9.Value<string>("info");
                string value3 = jToken9.Value<string>("var");
                string text27 = jToken9.Value<string>("savevar");
                if (!string.IsNullOrEmpty(text27))
                {
                    if (questvars.ContainsKey(text27))
                    {
                        if (!string.IsNullOrEmpty(value3))
                        {
                            questvars[text27] = value3;
                        }
                        else
                        {
                            questvars[text27] = num9.ToString();
                        }
                    }
                    else if (!string.IsNullOrEmpty(value3))
                    {
                        questvars.Add(text27, value3);
                    }
                    else
                    {
                        questvars.Add(text27, num9.ToString());
                    }
                }
                string text28 = jToken9.Value<string>("script");
                if (!string.IsNullOrEmpty(text8))
                {
                    SystemMsg(text8, 3);
                }
                if (!string.IsNullOrEmpty(text28))
                {
                    DialogPopup(en, text28, 0);
                    return;
                }
            }
            if ((scriptNum == 0 && (scriptName == "Deposit Item" || scriptName == "Withdraw Item" || scriptName == "Hairstyle" || scriptName == "Dye Hair" || scriptName == "Forget Skill" || scriptName == "Forget Spell" || scriptName == "Forget Action" || scriptName == "Learn Skill" || scriptName == "Learn Spell" || scriptName == "Learn Action" || scriptName == "Buy" || scriptName == "Sell")) || ((scriptNum == 0 || scriptNum == 1) && scriptName == "Dye Equipment"))
            {
                _standardDialogPopup._background.Texture = _textureManager.Get("mertop_F0_C0");
                num2 = 0.0;
                for (int m = 0; m < 6; m++)
                {
                    Sprite sprite5 = new Sprite();
                    sprite5.Texture = _textureManager.Get("mermid_F0_C0");
                    sprite5._windowOffset = new Vector(0.0, 117.0 + num2, 0.0);
                    sprite5.SetPosition(sprite5._windowOffset.X + _standardDialogPopup._position.X, sprite5._windowOffset.Y + _standardDialogPopup._position.Y);
                    _standardDialogPopup._backgrounds.Add("mid" + m, sprite5);
                    num2 += 30.0;
                }
                _standardDialogPopup._height = 180.0 + num2;
                Sprite sprite6 = new Sprite();
                Sprite sprite7 = new Sprite();
                Text goldlab = null;
                if (scriptName == "Deposit Item" || scriptName == "Buy" || scriptName == "Sell")
                {
                    sprite6 = new Sprite();
                    sprite6.Texture = _textureManager.Get("merbot_F2_C0");
                    sprite6._windowOffset = new Vector(0.0, 117.0 + num2, 0.0);
                    sprite6.SetPosition(sprite6._windowOffset.X + _standardDialogPopup._position.X, sprite6._windowOffset.Y + _standardDialogPopup._position.Y);
                    _standardDialogPopup._backgrounds.Add("bot", sprite6);
                    sprite7 = new Sprite();
                    sprite7.Texture = getItemSlot(72)._sprite.Texture;
                    sprite7._windowOffset = new Vector(157.0, 125.0 + num2, 0.0);
                    sprite7.SetPosition(sprite7._windowOffset.X + _standardDialogPopup._position.X, sprite7._windowOffset.Y + _standardDialogPopup._position.Y);
                    _standardDialogPopup._backgrounds.Add("gold", sprite7);
                    goldlab = DrawLabel(getItemSlot(72)._amount.ToString("#,0"), Engine.Color.White, 215.0, 141.0 + num2, 80, "right", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                    _standardDialogPopup._disLabels.Add("goldlab", goldlab);
                }
                else
                {
                    sprite6 = new Sprite();
                    sprite6.Texture = _textureManager.Get("merbot_F0_C0");
                    sprite6._windowOffset = new Vector(0.0, 117.0 + num2, 0.0);
                    sprite6.SetPosition(sprite6._windowOffset.X + _standardDialogPopup._position.X, sprite6._windowOffset.Y + _standardDialogPopup._position.Y);
                    _standardDialogPopup._backgrounds.Add("bot", sprite6);
                }
                if (scriptName == "Dye Hair" || scriptName == "Hairstyle")
                {
                    string text29 = "m";
                    if (_player._gender == 0)
                    {
                        text29 = "w";
                    }
                    Sprite sprite8 = new Sprite();
                    sprite8._windowOffset = new Vector(342.0, 18.0, 0.0);
                    if (_player._body._bodySource["u"] == "new")
                    {
                        sprite8.Texture = _textureManager.Get(text29 + "b" + _player._body._bodyImgs["b"].ToString("000") + "01_F5_new_C0", ".epf", "new", null, khan: true);
                    }
                    else
                    {
                        sprite8.Texture = _textureManager.Get(text29 + "b" + _player._body._bodyImgs["b"].ToString("000") + "01_F5_C0", ".epf", "old", null, khan: true);
                    }
                    sprite8.SetPosition(_standardDialogPopup._position.X + 342.0, _standardDialogPopup._position.Y + 18.0);
                    _standardDialogPopup._backgrounds.Add("pbody", sprite8);
                    Sprite sprite9 = new Sprite();
                    sprite9._windowOffset = new Vector(342.0, 18.0, 0.0);
                    sprite9.Texture = _textureManager.Get(text29 + "h" + _player._body._hairType.ToString("000") + "01_F5_C" + _player._body._hairColor, ".epf", "old", null, khan: true);
                    sprite9.SetPosition(_standardDialogPopup._position.X + 342.0, _standardDialogPopup._position.Y + 18.0);
                    _standardDialogPopup._backgrounds.Add("phair", sprite9);
                    if (_player._body._bodyImgs["u"] == 1 || _player._body._bodyImgs["u"] == 2 || _player._body._bodyImgs["u"] == 5 || _player._body._bodyImgs["u"] == 6 || _player._body._bodyImgs["u"] == 7 || _player._body._bodyImgs["u"] == 8 || _player._body._bodyImgs["u"] == 12)
                    {
                        Sprite sprite10 = new Sprite();
                        sprite10._windowOffset = new Vector(342.0, 18.0, 0.0);
                        sprite10.Texture = _textureManager.Get(text29 + "n" + _player._body._bodyImgs["n"].ToString("000") + "01_F5_C" + _player._body._bodyColors["n"], ".epf", "old", null, khan: true);
                        sprite10.SetPosition(_standardDialogPopup._position.X + 342.0, _standardDialogPopup._position.Y + 18.0);
                        _standardDialogPopup._backgrounds.Add("plegs", sprite10);
                    }
                    Sprite sprite11 = new Sprite();
                    sprite11._windowOffset = new Vector(342.0, 18.0, 0.0);
                    sprite11.Texture = _textureManager.Get(text29 + "l" + _player._body._bodyImgs["l"].ToString("000") + "01_F5_C" + _player._body._bodyColors["l"], ".epf", "old", null, khan: true);
                    sprite11.SetPosition(_standardDialogPopup._position.X + 342.0, _standardDialogPopup._position.Y + 18.0);
                    _standardDialogPopup._backgrounds.Add("pboots", sprite11);
                    Sprite sprite12 = new Sprite();
                    sprite12._windowOffset = new Vector(342.0, 18.0, 0.0);
                    if (_player._body._bodyImgs["u"] > 1000)
                    {
                        sprite12.Texture = _textureManager.Get(text29 + "i" + (_player._body._bodyImgs["u"] - 1000).ToString("000") + "01_F5_new_C" + _player._body._bodyColors["u"], ".epf", "new", null, khan: true);
                    }
                    else
                    {
                        sprite12.Texture = _textureManager.Get(text29 + "u" + _player._body._bodyImgs["u"].ToString("000") + "01_F5_C" + _player._body._bodyColors["u"], ".epf", "old", null, khan: true);
                    }
                    sprite12.SetPosition(_standardDialogPopup._position.X + 342.0, _standardDialogPopup._position.Y + 18.0);
                    _standardDialogPopup._backgrounds.Add("parmor", sprite12);
                    Sprite sprite13 = new Sprite();
                    sprite13._windowOffset = new Vector(342.0, 18.0, 0.0);
                    if (_player._body._bodyImgs["a"] == 320)
                    {
                        sprite13.Texture = _player._body._blankTexture;
                    }
                    else if (_player._body._bodyImgs["a"] > 1000)
                    {
                        sprite13.Texture = _textureManager.Get(text29 + "j" + (_player._body._bodyImgs["a"] - 1000).ToString("000") + "01_F5_new_C" + _player._body._bodyColors["a"], ".epf", "new", null, khan: true);
                    }
                    else
                    {
                        sprite13.Texture = _textureManager.Get(text29 + "a" + _player._body._bodyImgs["a"].ToString("000") + "01_F5_C" + _player._body._bodyColors["a"], ".epf", "old", null, khan: true);
                    }
                    sprite13.SetPosition(_standardDialogPopup._position.X + 342.0, _standardDialogPopup._position.Y + 18.0);
                    _standardDialogPopup._backgrounds.Add("parm", sprite13);
                    Sprite sprite14 = new Sprite();
                    sprite14._windowOffset = new Vector(315.0, 18.0, 0.0);
                    string text30 = "";
                    if (_player._body._bodySource["w"] != "old")
                    {
                        text30 = "_" + _player._body._bodySource["w"];
                    }
                    string text31 = "";
                    if (_player._body._bloody)
                    {
                        text31 = "_B";
                    }
                    if (_player._body._bodyImgs["w"] <= 11 && text29 == "w")
                    {
                        sprite14.Texture = _textureManager.Get("mw" + _player._body._bodyImgs["w"].ToString("000") + "01_F5" + text31 + text30 + "_C0", ".epf", _player._body._bodySource["w"], null, khan: true, _player._body._bloody);
                    }
                    else
                    {
                        sprite14.Texture = _textureManager.Get(text29 + "w" + _player._body._bodyImgs["w"].ToString("000") + "01_F5" + text31 + text30 + "_C0", ".epf", _player._body._bodySource["w"], null, khan: true, _player._body._bloody);
                    }
                    sprite14.SetPosition(_standardDialogPopup._position.X + 315.0, _standardDialogPopup._position.Y + 18.0);
                    _standardDialogPopup._backgrounds.Add("pweapon", sprite14);
                    Sprite sprite15 = new Sprite();
                    sprite15._windowOffset = new Vector(342.0, 18.0, 0.0);
                    sprite15.Texture = _textureManager.Get("ms" + _player._body._bodyImgs["s"].ToString("000") + "01_F5_C0", ".epf", "old", null, khan: true);
                    sprite15.SetPosition(_standardDialogPopup._position.X + 342.0, _standardDialogPopup._position.Y + 18.0);
                    _standardDialogPopup._backgrounds.Add("pshield", sprite15);
                }
                if (scriptName == "Hairstyle")
                {
                    Text value4 = DrawLabel("Current Style: " + _player._body._hairType, Engine.Color.White, 134.0, 60.0, 230, "left", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                    _standardDialogPopup._disLabels.Add("curtext", value4);
                }
                else if (scriptName == "Dye Hair")
                {
                    Text value5 = DrawLabel("Current Color: " + getDye(_player._body._hairColor.ToString()), Engine.Color.White, 134.0, 60.0, 230, "left", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                    _standardDialogPopup._disLabels.Add("curtext", value5);
                }
                else if (scriptName == "Dye Equipment" && scriptNum == 1)
                {
                    string value6 = questvars["dyeequip"];
                    InventoryItem inventoryItem = null;
                    foreach (InventoryItem item4 in _inventory.OrderBy((InventoryItem z) => z._slot))
                    {
                        if (item4._name.Equals(value6, StringComparison.CurrentCultureIgnoreCase))
                        {
                            inventoryItem = item4;
                        }
                    }
                    if (inventoryItem == null)
                    {
                        DialogPopup(en, scriptName, 2, img);
                        return;
                    }
                    if (inventoryItem._dyeable == 0)
                    {
                        DialogPopup(en, scriptName, 3, img);
                        return;
                    }
                    string dye = getDye(inventoryItem._bodyImgColor.ToString());
                    Text value7 = DrawLabel("Current Color: " + dye, Engine.Color.White, 134.0, 60.0, 230, "left", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                    _standardDialogPopup._disLabels.Add("curtext", value7);
                }
                Engine.Button button2 = new Engine.Button(_textureManager, 420.0, 117.0, 13.0, 12.0, "scroll_F0", "scroll_F1", "", "scroll_F1", null, null, 0, multHorzontal: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                button2.Enabled = false;
                button2._windowOffset = new Vector(420.0, 117.0, 0.0);
                if (flag2)
                {
                    button2._clicked = true;
                }
                Engine.Button button3 = new Engine.Button(_textureManager, 420.0, 285.0, 13.0, 12.0, "scroll_F2", "scroll_F3", "", "scroll_F3", null, null, 0, multHorzontal: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                button3.Enabled = false;
                button3._windowOffset = new Vector(420.0, 285.0, 0.0);
                if (flag3)
                {
                    button3._clicked = true;
                }
                Engine.Button button4 = new Engine.Button(_textureManager, 420.0, 129.0, 13.0, 12.0, "scroll_F4", "scroll_F4", "", "", null, null, 0, multHorzontal: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                button4.Hidden = true;
                if (flag)
                {
                    button4._clicked = true;
                }
                button4._windowOffset = new Vector(420.0, 129.0, 0.0);
                Engine.Button button5 = new Engine.Button(_textureManager, 420.0, 129.0, 13.0, 156.0, "scroll_F5", "scroll_F5", "", "", null, null, 13, multHorzontal: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                button5._windowOffset = new Vector(420.0, 129.0, 0.0);
                _standardDialogPopup._disButtons.Add("chatScrollBackBtn", button5);
                _standardDialogPopup._disButtons.Add("chatScrollerBtn", button4);
                _standardDialogPopup._disButtons.Add("chatScrollUpBtn", button2);
                _standardDialogPopup._disButtons.Add("chatScrollDownBtn", button3);
                if (scriptName == "Withdraw Item" || scriptName == "Buy")
                {
                    Engine.Button button6 = new Engine.Button(_textureManager, 135.0, 82.0, 13.0, 12.0, "scroll_F0", "scroll_F1", "", "scroll_F1", null, null, 0, multHorzontal: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                    button6._windowOffset = new Vector(135.0, 82.0, 0.0);
                    button6.Sprite._rotate = true;
                    button6.Enabled = false;
                    if (flag5)
                    {
                        button6._clicked = true;
                    }
                    Engine.Button button7 = new Engine.Button(_textureManager, 421.0, 82.0, 13.0, 12.0, "scroll_F2", "scroll_F3", "", "scroll_F3", null, null, 0, multHorzontal: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                    button7._windowOffset = new Vector(421.0, 82.0, 0.0);
                    button7.Sprite._rotate = true;
                    button7.Enabled = false;
                    if (flag6)
                    {
                        button7._clicked = true;
                    }
                    Engine.Button button8 = new Engine.Button(_textureManager, 148.0, 82.0, 13.0, 12.0, "scroll_F4", "scroll_F4", "", "", null, null, 0, multHorzontal: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                    button8._windowOffset = new Vector(148.0, 82.0, 0.0);
                    button8.Sprite._rotate = true;
                    button8.Hidden = true;
                    if (flag4)
                    {
                        button8._clicked = true;
                    }
                    Engine.Button button9 = new Engine.Button(_textureManager, 148.0, 82.0, 273.0, 12.0, "scroll_F5", "scroll_F5", "", "", null, null, 23, multHorzontal: true, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                    button9._windowOffset = new Vector(148.0, 82.0, 0.0);
                    button9.Sprite._rotate = true;
                    button9.Sprite.Texture = button9._baseImage;
                    _standardDialogPopup._disButtons.Add("catScrollBackBtn", button9);
                    _standardDialogPopup._disButtons.Add("catScrollerBtn", button8);
                    _standardDialogPopup._disButtons.Add("catScrollUpBtn", button6);
                    _standardDialogPopup._disButtons.Add("catScrollDownBtn", button7);
                }
                if (scriptName == "Deposit Item")
                {
                    text2 = "Deposit Item";
                    int fullcount14 = 0;
                    foreach (InventoryItem item5 in _inventory)
                    {
                        if (!item5._bound)
                        {
                            fullcount14++;
                        }
                    }
                    RepositionScroller(_depositItemDialogIndex, fullcount14 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    button2._onHeldEvent = delegate
                    {
                        if (_depositItemDialogIndex > 0)
                        {
                            _depositItemDialogIndex--;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_depositItemDialogIndex, fullcount14 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button2._onPressEvent = delegate
                    {
                        if (_depositItemDialogIndex > 0)
                        {
                            _depositItemDialogIndex--;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_depositItemDialogIndex, fullcount14 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onHeldEvent = delegate
                    {
                        if (_depositItemDialogIndex < _standardDialogPopup._depositlist.Count - 5)
                        {
                            _depositItemDialogIndex++;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_depositItemDialogIndex, fullcount14 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onPressEvent = delegate
                    {
                        if (_depositItemDialogIndex < _standardDialogPopup._depositlist.Count - 5)
                        {
                            _depositItemDialogIndex++;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_depositItemDialogIndex, fullcount14 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button4._onHeldEvent = delegate
                    {
                        double num125 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num126 = (int)Math.Round(num125 * (double)(fullcount14 - 5));
                        _depositItemDialogIndex = ((num126 >= 0) ? ((num126 > fullcount14 - 5) ? (fullcount14 - 5) : num126) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_depositItemDialogIndex, fullcount14 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    button5._onPressEvent = delegate
                    {
                        double num123 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num124 = (int)Math.Round(num123 * (double)(fullcount14 - 5));
                        _depositItemDialogIndex = ((num124 >= 0) ? ((num124 > fullcount14 - 5) ? (fullcount14 - 5) : num124) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_depositItemDialogIndex, fullcount14 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    foreach (InventoryItem item6 in _inventory.OrderBy((InventoryItem e) => e._slot))
                    {
                        if (!item6._bound)
                        {
                            _standardDialogPopup._depositlist.Add(item6._slot, item6._name);
                        }
                    }
                    int num10 = 0;
                    int num11 = 0;
                    foreach (KeyValuePair<int, string> item7 in _standardDialogPopup._depositlist)
                    {
                        InventoryItem itm2 = getItemSlot(item7.Key);
                        num11++;
                        if (num11 <= _depositItemDialogIndex)
                        {
                            continue;
                        }
                        num10++;
                        if (num10 > 5)
                        {
                            break;
                        }
                        Vector vector4 = new Vector(40.0, 86 + num10 * 35, 0.0);
                        string text32 = item7.Value;
                        if (itm2._maxAmount > 1)
                        {
                            text32 = text32 + " (" + itm2._amount + ")";
                        }
                        Text nametext14 = DrawLabel(text32, Engine.Color.White, vector4.X + 40.0, vector4.Y + 12.0, 230, "left", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        Text valuetext4 = DrawLabel(((double)itm2._value / 33.333333333).ToString("#,0"), Engine.Color.White, vector4.X + 280.0, vector4.Y + 12.0, 80, "right", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        _standardDialogPopup._disLabels.Add("nametext" + num11, nametext14);
                        _standardDialogPopup._disLabels.Add("valuetext" + num11, valuetext4);
                        int valuenum3 = int.Parse(valuetext4._text.Replace(",", ""));
                        _ = itm2._slot;
                        _ = itm2._maxAmount;
                        _ = 1;
                        Engine.Button itemBtn14 = new Engine.Button(_textureManager, vector4.X, vector4.Y, 363.0, 33.0, "", "", "", "", null, null, 0, multHorzontal: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        itemBtn14._windowOffset = new Vector(vector4.X, vector4.Y, 0.0);
                        itemBtn14._baseImage = itm2._sprite.Texture;
                        itemBtn14._clickedImage = itm2._sprite.Texture;
                        itemBtn14._focusedImage = itm2._sprite.Texture;
                        itemBtn14.Sprite.Texture = itemBtn14._baseImage;
                        itemBtn14._onHoverEvent = delegate
                        {
                            ItemToolTip(itm2, _input.Mouse.Position);
                        };
                        itemBtn14._onStopHoverEvent = delegate
                        {
                            _tTT = "";
                            _toolTipText.ChangeText("");
                            _input.Mouse.SetCursorDefault();
                        };
                        itemBtn14._onDoublePressEvent = delegate
                        {
                            itm2._amount--;
                            if (itm2._amount <= 0)
                            {
                                _inventory.Remove(itm2);
                            }
                            InventoryItem itemSlot7 = getItemSlot(72);
                            itemSlot7._amount -= valuenum3;
                            if (_playerbank.ContainsKey(itm2._name))
                            {
                                _playerbank[itm2._name]._amount++;
                            }
                            else
                            {
                                BankItem value9 = new BankItem(itm2._name, itm2._tab, 1, itemSlot7._sprite.Texture)
                                {
                                    _maxAmount = itm2._maxAmount,
                                    _description = itm2._description,
                                    _dyeable = itm2._dyeable,
                                    _value = itm2._value,
                                    _maxDurability = itm2._maxDurability,
                                    _level = itm2._level,
                                    _gender = itm2._gender,
                                    _weaponDmg = itm2._weaponDmg,
                                    _atk = itm2._atk,
                                    _def = itm2._def,
                                    _hp = itm2._hp,
                                    _mp = itm2._mp,
                                    _str = itm2._str,
                                    _int = itm2._int,
                                    _wis = itm2._wis,
                                    _con = itm2._con,
                                    _dex = itm2._dex,
                                    _mr = itm2._mr,
                                    _ac = itm2._ac,
                                    _dmg = itm2._dmg,
                                    _hit = itm2._hit,
                                    _reg = itm2._reg
                                };
                                _playerbank.Add(itm2._name, value9);
                            }
                            DialogPopup(en, scriptName, 0, img);
                        };
                        itemBtn14._onPressEvent = delegate
                        {
                            foreach (KeyValuePair<string, Text> disLabel in _standardDialogPopup._disLabels)
                            {
                                if (disLabel.Key.StartsWith("nametext") || disLabel.Key.StartsWith("valuetext"))
                                {
                                    disLabel.Value.SetColor(Engine.Color.White);
                                }
                            }
                            nametext14.SetColor(Engine.Color.LightBlue);
                            valuetext4.SetColor(Engine.Color.LightBlue);
                            itemBtn14.Selected = true;
                        };
                        _standardDialogPopup._disButtons.Add("itemBtn" + num11, itemBtn14);
                    }
                    if (num11 > 5)
                    {
                        _standardDialogPopup._disButtons["chatScrollerBtn"].Hidden = false;
                        _standardDialogPopup._disButtons["chatScrollUpBtn"].Enabled = true;
                        _standardDialogPopup._disButtons["chatScrollDownBtn"].Enabled = true;
                    }
                }
                // === Handler: Withdraw Item (from the bank) ===
                if (scriptName == "Withdraw Item")
                {
                    text2 = "Withdraw Item";
                    int fullcount13 = 0;
                    foreach (KeyValuePair<string, BankItem> item8 in _playerbank)
                    {
                        string tab = item8.Value._tab;
                        if (!(tab != _selectedWithdrawDialogCat))
                        {
                            fullcount13++;
                        }
                    }
                    RepositionScroller(_withdrawItemDialogIndex, fullcount13 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    button2._onHeldEvent = delegate
                    {
                        if (_withdrawItemDialogIndex > 0)
                        {
                            _withdrawItemDialogIndex--;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_withdrawItemDialogIndex, fullcount13 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button2._onPressEvent = delegate
                    {
                        if (_withdrawItemDialogIndex > 0)
                        {
                            _withdrawItemDialogIndex--;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_withdrawItemDialogIndex, fullcount13 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onHeldEvent = delegate
                    {
                        if (_withdrawItemDialogIndex < _standardDialogPopup._withdrawlist.Count - 5)
                        {
                            _withdrawItemDialogIndex++;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_withdrawItemDialogIndex, fullcount13 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onPressEvent = delegate
                    {
                        if (_withdrawItemDialogIndex < _standardDialogPopup._withdrawlist.Count - 5)
                        {
                            _withdrawItemDialogIndex++;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_withdrawItemDialogIndex, fullcount13 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button4._onHeldEvent = delegate
                    {
                        double num121 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num122 = (int)Math.Round(num121 * (double)(fullcount13 - 5));
                        _withdrawItemDialogIndex = ((num122 >= 0) ? ((num122 > fullcount13 - 5) ? (fullcount13 - 5) : num122) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_withdrawItemDialogIndex, fullcount13 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    button5._onPressEvent = delegate
                    {
                        double num119 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num120 = (int)Math.Round(num119 * (double)(fullcount13 - 5));
                        _withdrawItemDialogIndex = ((num120 >= 0) ? ((num120 > fullcount13 - 5) ? (fullcount13 - 5) : num120) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_withdrawItemDialogIndex, fullcount13 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    int fullcatcount2 = 0;
                    foreach (string key in _playerbank.Keys)
                    {
                        foreach (JToken item9 in _itemsDB["items"].Children())
                        {
                            if (item9.Value<string>("name") == key)
                            {
                                string item = item9.Value<string>("tab");
                                if (!_standardDialogPopup._withdrawcatlist.Contains(item))
                                {
                                    _standardDialogPopup._withdrawcatlist.Add(item);
                                }
                                break;
                            }
                        }
                    }
                    fullcatcount2 = _standardDialogPopup._withdrawcatlist.Count;
                    RepositionHorizontalScroller(_withdrawItemDialogCatIndex, fullcatcount2 - 4, 260, _standardDialogPopup, _standardDialogPopup._disButtons["catScrollerBtn"], 148, 82);
                    _standardDialogPopup._disButtons["catScrollUpBtn"]._onHeldEvent = delegate
                    {
                        if (_withdrawItemDialogCatIndex > 0)
                        {
                            _withdrawItemDialogCatIndex--;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionHorizontalScroller(_withdrawItemDialogCatIndex, fullcatcount2 - 4, 260, _standardDialogPopup, _standardDialogPopup._disButtons["catScrollerBtn"], 148, 82);
                        }
                    };
                    _standardDialogPopup._disButtons["catScrollUpBtn"]._onPressEvent = delegate
                    {
                        if (_withdrawItemDialogCatIndex > 0)
                        {
                            _withdrawItemDialogCatIndex--;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionHorizontalScroller(_withdrawItemDialogCatIndex, fullcatcount2 - 4, 260, _standardDialogPopup, _standardDialogPopup._disButtons["catScrollerBtn"], 148, 82);
                        }
                    };
                    _standardDialogPopup._disButtons["catScrollDownBtn"]._onHeldEvent = delegate
                    {
                        if (_withdrawItemDialogCatIndex < _standardDialogPopup._withdrawcatlist.Count - 4)
                        {
                            _withdrawItemDialogCatIndex++;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionHorizontalScroller(_withdrawItemDialogCatIndex, fullcatcount2 - 4, 260, _standardDialogPopup, _standardDialogPopup._disButtons["catScrollerBtn"], 148, 82);
                        }
                    };
                    _standardDialogPopup._disButtons["catScrollDownBtn"]._onPressEvent = delegate
                    {
                        if (_withdrawItemDialogCatIndex < _standardDialogPopup._withdrawcatlist.Count - 4)
                        {
                            _withdrawItemDialogCatIndex++;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionHorizontalScroller(_withdrawItemDialogCatIndex, fullcatcount2 - 4, 260, _standardDialogPopup, _standardDialogPopup._disButtons["catScrollerBtn"], 148, 82);
                        }
                    };
                    _standardDialogPopup._disButtons["catScrollerBtn"]._onHeldEvent = delegate
                    {
                        double num117 = ((double)_input.Mouse.Position.X - _standardDialogPopup._position.X - 148.0 - 7.5) / 265.5;
                        int num118 = (int)Math.Round(num117 * (double)(fullcatcount2 - 4));
                        _withdrawItemDialogCatIndex = ((num118 >= 0) ? ((num118 > fullcatcount2 - 4) ? (fullcatcount2 - 4) : num118) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionHorizontalScroller(_withdrawItemDialogCatIndex, fullcatcount2 - 4, 260, _standardDialogPopup, _standardDialogPopup._disButtons["catScrollerBtn"], 148, 82);
                    };
                    _standardDialogPopup._disButtons["catScrollBackBtn"]._onPressEvent = delegate
                    {
                        double num115 = ((double)_input.Mouse.Position.X - _standardDialogPopup._position.X - 148.0 - 7.5) / 265.5;
                        int num116 = (int)Math.Round(num115 * (double)(fullcatcount2 - 4));
                        _withdrawItemDialogCatIndex = ((num116 >= 0) ? ((num116 > fullcatcount2 - 4) ? (fullcatcount2 - 4) : num116) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionHorizontalScroller(_withdrawItemDialogCatIndex, fullcatcount2 - 4, 260, _standardDialogPopup, _standardDialogPopup._disButtons["catScrollerBtn"], 148, 82);
                    };
                    _standardDialogPopup._withdrawcatlist.Sort();
                    bool flag10 = false;
                    foreach (string item10 in _standardDialogPopup._withdrawcatlist)
                    {
                        if (item10 == _selectedWithdrawDialogCat)
                        {
                            flag10 = true;
                            break;
                        }
                    }
                    int num12 = 0;
                    int num13 = 0;
                    foreach (string item11 in _standardDialogPopup._withdrawcatlist)
                    {
                        num13++;
                        if (num13 <= _withdrawItemDialogCatIndex)
                        {
                            continue;
                        }
                        num12++;
                        if (num12 > 4)
                        {
                            break;
                        }
                        Vector vector5 = new Vector(68 + num12 * 73, 60.0, 0.0);
                        Engine.Button catBtn2 = new Engine.Button(_textureManager, vector5.X, vector5.Y, 66.0, 18.0, "banktab_F1", "banktab_F2", "", "", null, null, 0, multHorzontal: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        catBtn2._windowOffset = new Vector(vector5.X, vector5.Y, 0.0);
                        catBtn2._text = DrawLabel(item11, Engine.Color.White, vector5.X, vector5.Y + 3.0, 66, "center", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        catBtn2._onPressEvent = delegate
                        {
                            foreach (Engine.Button value10 in _standardDialogPopup._disButtons.Values)
                            {
                                value10.Selected = false;
                            }
                            catBtn2.Selected = true;
                            _selectedWithdrawDialogCat = catBtn2._text._text;
                            _withdrawItemDialogIndex = 0;
                            DialogPopup(en, scriptName, scriptNum, img);
                        };
                        if ((!flag10 && num12 == 1) || item11 == _selectedWithdrawDialogCat)
                        {
                            catBtn2.Selected = true;
                            _selectedWithdrawDialogCat = item11;
                        }
                        _standardDialogPopup._disButtons.Add("catBtn" + num13, catBtn2);
                    }
                    if (num13 > 4)
                    {
                        _standardDialogPopup._disButtons["catScrollerBtn"].Hidden = false;
                        _standardDialogPopup._disButtons["catScrollUpBtn"].Enabled = true;
                        _standardDialogPopup._disButtons["catScrollDownBtn"].Enabled = true;
                    }
                    int num14 = 0;
                    int num15 = 0;
                    foreach (KeyValuePair<string, BankItem> item12 in _playerbank)
                    {
                        BankItem z2 = item12.Value;
                        Texture tex = item12.Value._tex;
                        string tab2 = item12.Value._tab;
                        if (tab2 != _selectedWithdrawDialogCat)
                        {
                            continue;
                        }
                        _standardDialogPopup._withdrawlist.Add(item12.Key, item12.Value._amount);
                        num15++;
                        if (num15 <= _withdrawItemDialogIndex)
                        {
                            continue;
                        }
                        num14++;
                        if (num14 > 5)
                        {
                            break;
                        }
                        Vector vector6 = new Vector(40.0, 86 + num14 * 35, 0.0);
                        Text nametext13 = DrawLabel(item12.Key, Engine.Color.White, vector6.X + 40.0, vector6.Y + 12.0, 230, "left", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        Text valuetext3 = DrawLabel(item12.Value._amount.ToString("#,0"), Engine.Color.White, vector6.X + 280.0, vector6.Y + 12.0, 80, "right", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        _standardDialogPopup._disLabels.Add("nametext" + num15, nametext13);
                        _standardDialogPopup._disLabels.Add("valuetext" + num15, valuetext3);
                        int.Parse(valuetext3._text.Replace(",", ""));
                        Engine.Button itemBtn13 = new Engine.Button(_textureManager, vector6.X, vector6.Y, 363.0, 33.0, "", "", "", "", null, null, 0, multHorzontal: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        itemBtn13._windowOffset = new Vector(vector6.X, vector6.Y, 0.0);
                        itemBtn13._baseImage = tex;
                        itemBtn13._clickedImage = tex;
                        itemBtn13._focusedImage = tex;
                        itemBtn13.Sprite.Texture = tex;
                        itemBtn13._onHoverEvent = delegate
                        {
                            ItemToolTip(z2, _input.Mouse.Position);
                        };
                        itemBtn13._onStopHoverEvent = delegate
                        {
                            _tTT = "";
                            _toolTipText.ChangeText("");
                            _input.Mouse.SetCursorDefault();
                        };
                        itemBtn13._onDoublePressEvent = delegate
                        {
                            NewItem(nametext13._text, FirstAvailableInventorySlot());
                            _playerbank[nametext13._text]._amount--;
                            if (_playerbank[nametext13._text]._amount <= 0)
                            {
                                _playerbank.Remove(nametext13._text);
                                if (_withdrawItemDialogIndex > 0)
                                {
                                    _withdrawItemDialogIndex--;
                                }
                            }
                            DialogPopup(en, scriptName, 0, img);
                        };
                        itemBtn13._onPressEvent = delegate
                        {
                            foreach (KeyValuePair<string, Text> disLabel2 in _standardDialogPopup._disLabels)
                            {
                                if (disLabel2.Key.StartsWith("nametext") || disLabel2.Key.StartsWith("valuetext"))
                                {
                                    disLabel2.Value.SetColor(Engine.Color.White);
                                }
                            }
                            nametext13.SetColor(Engine.Color.LightBlue);
                            valuetext3.SetColor(Engine.Color.LightBlue);
                            itemBtn13.Selected = true;
                        };
                        _standardDialogPopup._disButtons.Add("itemBtn" + num15, itemBtn13);
                    }
                    if (num15 > 5)
                    {
                        _standardDialogPopup._disButtons["chatScrollerBtn"].Hidden = false;
                        _standardDialogPopup._disButtons["chatScrollUpBtn"].Enabled = true;
                        _standardDialogPopup._disButtons["chatScrollDownBtn"].Enabled = true;
                    }
                }
                // === Handler: Hairstyle change ===
                if (scriptName == "Hairstyle")
                {
                    text2 = "Hairstyle";
                    int fullcount12 = 60;
                    RepositionScroller(_hairstyleDialogIndex, fullcount12 - 25, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    button2._onHeldEvent = delegate
                    {
                        if (_hairstyleDialogIndex > 0)
                        {
                            _hairstyleDialogIndex -= 5;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_hairstyleDialogIndex, fullcount12 - 25, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button2._onPressEvent = delegate
                    {
                        if (_hairstyleDialogIndex > 0)
                        {
                            _hairstyleDialogIndex -= 5;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_hairstyleDialogIndex, fullcount12 - 25, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onHeldEvent = delegate
                    {
                        if (_hairstyleDialogIndex < 32)
                        {
                            _hairstyleDialogIndex += 5;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_hairstyleDialogIndex, fullcount12 - 25, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onPressEvent = delegate
                    {
                        if (_hairstyleDialogIndex < 32)
                        {
                            _hairstyleDialogIndex += 5;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_hairstyleDialogIndex, fullcount12 - 25, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button4._onHeldEvent = delegate
                    {
                        double num113 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num114 = (int)(5.0 * Math.Round(num113 * (double)(fullcount12 - 25) / 5.0));
                        _hairstyleDialogIndex = ((num114 >= 0) ? ((num114 > fullcount12 - 25) ? (fullcount12 - 25) : num114) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_hairstyleDialogIndex, fullcount12 - 25, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    button5._onPressEvent = delegate
                    {
                        double num111 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num112 = (int)(5.0 * Math.Round(num111 * (double)(fullcount12 - 25) / 5.0));
                        _hairstyleDialogIndex = ((num112 >= 0) ? ((num112 > fullcount12 - 25) ? (fullcount12 - 25) : num112) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_hairstyleDialogIndex, fullcount12 - 25, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    string text33 = "m";
                    if (_player._gender == 0)
                    {
                        text33 = "w";
                    }
                    int num16 = 1;
                    int num17 = 0;
                    int num18 = 0;
                    for (int n = 0; n <= 57; n++)
                    {
                        string text34 = "";
                        Texture texture = _textureManager.Get(text33 + "h" + n.ToString("000") + "01_F5_myda_C0", ".epf", "myda", null, khan: true);
                        num17++;
                        if (num17 <= _hairstyleDialogIndex)
                        {
                            continue;
                        }
                        if (num16 > 5)
                        {
                            break;
                        }
                        Vector vector7 = new Vector(40 + num18 * 75, 86 + num16 * 35, 0.0);
                        num18++;
                        if (num18 == 5)
                        {
                            num18 = 0;
                            num16++;
                        }
                        Text nametext12 = DrawLabel(n.ToString(), Engine.Color.White, vector7.X + 40.0, vector7.Y + 12.0, 230, "left", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        _standardDialogPopup._disLabels.Add("nametext" + num17, nametext12);
                        Engine.Button itemBtn12 = new Engine.Button(_textureManager, vector7.X, vector7.Y, 73.0, 33.0, text34, text34, text34, "", null, null, 0, multHorzontal: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        itemBtn12._windowOffset = new Vector(vector7.X, vector7.Y, 0.0);
                        itemBtn12.Sprite._windowOffset = new Vector(-14.0, -16.0, 0.0);
                        itemBtn12.Sprite.SetPosition(vector7.X + _standardDialogPopup._position.X - 14.0, vector7.Y + _standardDialogPopup._position.Y - 16.0);
                        itemBtn12._baseImage = texture;
                        itemBtn12._clickedImage = texture;
                        itemBtn12._focusedImage = texture;
                        itemBtn12.Sprite.Texture = texture;
                        itemBtn12._onDoublePressEvent = delegate
                        {
                            _player._body._hairType = int.Parse(nametext12._text);
                            _player._body._bodyImgs["h"] = int.Parse(nametext12._text);
                            SaveProfile();
                            DialogPopup(en, scriptName, 0, img);
                        };
                        itemBtn12._onPressEvent = delegate
                        {
                            foreach (KeyValuePair<string, Text> disLabel3 in _standardDialogPopup._disLabels)
                            {
                                if (disLabel3.Key.StartsWith("nametext"))
                                {
                                    disLabel3.Value.SetColor(Engine.Color.White);
                                }
                            }
                            nametext12.SetColor(Engine.Color.LightBlue);
                            itemBtn12.Selected = true;
                        };
                        _standardDialogPopup._disButtons.Add("itemBtn" + num17, itemBtn12);
                    }
                    if (num17 > 25)
                    {
                        _standardDialogPopup._disButtons["chatScrollerBtn"].Hidden = false;
                        _standardDialogPopup._disButtons["chatScrollUpBtn"].Enabled = true;
                        _standardDialogPopup._disButtons["chatScrollDownBtn"].Enabled = true;
                    }
                }
                // === Handler: Dye Hair ===
                if (scriptName == "Dye Hair")
                {
                    text2 = "Dye Hair";
                    int fullcount11 = 36;
                    RepositionScroller(_dyehairDialogIndex, fullcount11 - 15, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    button2._onHeldEvent = delegate
                    {
                        if (_dyehairDialogIndex > 0)
                        {
                            _dyehairDialogIndex -= 3;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_dyehairDialogIndex, fullcount11 - 15, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button2._onPressEvent = delegate
                    {
                        if (_dyehairDialogIndex > 0)
                        {
                            _dyehairDialogIndex -= 3;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_dyehairDialogIndex, fullcount11 - 15, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onHeldEvent = delegate
                    {
                        if (_dyehairDialogIndex < 21)
                        {
                            _dyehairDialogIndex += 3;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_dyehairDialogIndex, fullcount11 - 15, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onPressEvent = delegate
                    {
                        if (_dyehairDialogIndex < 21)
                        {
                            _dyehairDialogIndex += 3;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_dyehairDialogIndex, fullcount11 - 15, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button4._onHeldEvent = delegate
                    {
                        double num109 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num110 = (int)(3.0 * Math.Round(num109 * (double)(fullcount11 - 15) / 3.0));
                        _dyehairDialogIndex = ((num110 >= 0) ? ((num110 > fullcount11 - 15) ? (fullcount11 - 15) : num110) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_dyehairDialogIndex, fullcount11 - 15, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    button5._onPressEvent = delegate
                    {
                        double num107 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num108 = (int)(3.0 * Math.Round(num107 * (double)(fullcount11 - 15) / 3.0));
                        _dyehairDialogIndex = ((num108 >= 0) ? ((num108 > fullcount11 - 15) ? (fullcount11 - 15) : num108) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_dyehairDialogIndex, fullcount11 - 15, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    string text35 = "m";
                    if (_player._gender == 0)
                    {
                        text35 = "w";
                    }
                    int num19 = 1;
                    int num20 = 0;
                    int num21 = 0;
                    for (int num22 = 0; num22 < 36; num22++)
                    {
                        string text36 = "";
                        Texture texture2 = _textureManager.Get(text35 + "h" + _player._body._hairType.ToString("000") + "01_F5_C" + num22, ".epf", "old", null, khan: true);
                        num20++;
                        if (num20 <= _dyehairDialogIndex)
                        {
                            continue;
                        }
                        if (num19 > 5)
                        {
                            break;
                        }
                        Vector vector8 = new Vector(40 + num21 * 125, 86 + num19 * 35, 0.0);
                        num21++;
                        if (num21 == 3)
                        {
                            num21 = 0;
                            num19++;
                        }
                        Text nametext11 = DrawLabel(((Dye)num22).ToString(), Engine.Color.White, vector8.X + 40.0, vector8.Y + 12.0, 230, "left", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        _standardDialogPopup._disLabels.Add("nametext" + num20, nametext11);
                        int test2 = num22;
                        Engine.Button itemBtn11 = new Engine.Button(_textureManager, vector8.X, vector8.Y, 123.0, 33.0, text36, text36, text36, "", null, null, 0, multHorzontal: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        itemBtn11._windowOffset = new Vector(vector8.X, vector8.Y, 0.0);
                        itemBtn11.Sprite._windowOffset = new Vector(-14.0, -16.0, 0.0);
                        itemBtn11.Sprite.SetPosition(vector8.X + _standardDialogPopup._position.X - 14.0, vector8.Y + _standardDialogPopup._position.Y - 16.0);
                        itemBtn11._baseImage = texture2;
                        itemBtn11._clickedImage = texture2;
                        itemBtn11._focusedImage = texture2;
                        itemBtn11.Sprite.Texture = texture2;
                        itemBtn11._onDoublePressEvent = delegate
                        {
                            _player._body._hairColor = test2;
                            _player._body._bodyColors["h"] = test2;
                            SaveProfile();
                            DialogPopup(en, scriptName, 0, img);
                        };
                        itemBtn11._onPressEvent = delegate
                        {
                            foreach (KeyValuePair<string, Text> disLabel4 in _standardDialogPopup._disLabels)
                            {
                                if (disLabel4.Key.StartsWith("nametext"))
                                {
                                    disLabel4.Value.SetColor(Engine.Color.White);
                                }
                            }
                            nametext11.SetColor(Engine.Color.LightBlue);
                            itemBtn11.Selected = true;
                        };
                        _standardDialogPopup._disButtons.Add("itemBtn" + num20, itemBtn11);
                    }
                    if (num20 > 15)
                    {
                        _standardDialogPopup._disButtons["chatScrollerBtn"].Hidden = false;
                        _standardDialogPopup._disButtons["chatScrollUpBtn"].Enabled = true;
                        _standardDialogPopup._disButtons["chatScrollDownBtn"].Enabled = true;
                    }
                }
                // === Handler: Dye Equipment ===
                if (scriptName == "Dye Equipment" && scriptNum == 0)
                {
                    text2 = "Dye Equipment";
                    int fullcount10 = 0;
                    foreach (InventoryItem item13 in _inventory)
                    {
                        if (!item13._bound && item13._dyeable == 1)
                        {
                            fullcount10++;
                        }
                    }
                    RepositionScroller(_dyeDialogIndex, fullcount10 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    button2._onHeldEvent = delegate
                    {
                        if (_dyeDialogIndex > 0)
                        {
                            _dyeDialogIndex--;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_dyeDialogIndex, fullcount10 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button2._onPressEvent = delegate
                    {
                        if (_dyeDialogIndex > 0)
                        {
                            _dyeDialogIndex--;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_dyeDialogIndex, fullcount10 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onHeldEvent = delegate
                    {
                        if (_dyeDialogIndex < _standardDialogPopup._dyelist.Count - 5)
                        {
                            _dyeDialogIndex++;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_dyeDialogIndex, fullcount10 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onPressEvent = delegate
                    {
                        if (_dyeDialogIndex < _standardDialogPopup._dyelist.Count - 5)
                        {
                            _dyeDialogIndex++;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_dyeDialogIndex, fullcount10 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button4._onHeldEvent = delegate
                    {
                        double num105 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num106 = (int)Math.Round(num105 * (double)(fullcount10 - 5));
                        _dyeDialogIndex = ((num106 >= 0) ? ((num106 > fullcount10 - 5) ? (fullcount10 - 5) : num106) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_dyeDialogIndex, fullcount10 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    button5._onPressEvent = delegate
                    {
                        double num103 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num104 = (int)Math.Round(num103 * (double)(fullcount10 - 5));
                        _dyeDialogIndex = ((num104 >= 0) ? ((num104 > fullcount10 - 5) ? (fullcount10 - 5) : num104) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_dyeDialogIndex, fullcount10 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    foreach (InventoryItem item14 in _inventory.OrderBy((InventoryItem e) => e._slot))
                    {
                        if (!item14._bound && item14._dyeable == 1)
                        {
                            _standardDialogPopup._dyelist.Add(item14._slot, item14._name);
                        }
                    }
                    int num23 = 0;
                    int num24 = 0;
                    foreach (KeyValuePair<int, string> item15 in _standardDialogPopup._dyelist)
                    {
                        InventoryItem itm = getItemSlot(item15.Key);
                        num24++;
                        if (num24 <= _dyeDialogIndex)
                        {
                            continue;
                        }
                        num23++;
                        if (num23 > 5)
                        {
                            break;
                        }
                        Vector vector9 = new Vector(40.0, 86 + num23 * 35, 0.0);
                        string text37 = item15.Value;
                        if (itm._maxAmount > 1)
                        {
                            text37 = text37 + " (" + itm._amount + ")";
                        }
                        Text nametext10 = DrawLabel(text37, Engine.Color.White, vector9.X + 40.0, vector9.Y + 12.0, 230, "left", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        _standardDialogPopup._disLabels.Add("nametext" + num24, nametext10);
                        _ = itm._slot;
                        _ = itm._maxAmount;
                        _ = 1;
                        Engine.Button itemBtn10 = new Engine.Button(_textureManager, vector9.X, vector9.Y, 363.0, 33.0, "", "", "", "", null, null, 0, multHorzontal: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        itemBtn10._windowOffset = new Vector(vector9.X, vector9.Y, 0.0);
                        itemBtn10._baseImage = itm._sprite.Texture;
                        itemBtn10._clickedImage = itm._sprite.Texture;
                        itemBtn10._focusedImage = itm._sprite.Texture;
                        itemBtn10.Sprite.Texture = itemBtn10._baseImage;
                        itemBtn10._onHoverEvent = delegate
                        {
                            if (!itemBtn10.Selected)
                            {
                                ItemToolTip(itm, _input.Mouse.Position);
                            }
                        };
                        itemBtn10._onStopHoverEvent = delegate
                        {
                            _tTT = "";
                            _toolTipText.ChangeText("");
                            _input.Mouse.SetCursorDefault();
                        };
                        itemBtn10._onDoublePressEvent = delegate
                        {
                            questvars["dyeequip"] = itm._name;
                            DialogPopup(en, scriptName, 1, img);
                            _tTT = "";
                            _toolTipText.ChangeText("");
                            _input.Mouse.SetCursorDefault();
                        };
                        itemBtn10._onPressEvent = delegate
                        {
                            foreach (KeyValuePair<string, Text> disLabel5 in _standardDialogPopup._disLabels)
                            {
                                if (disLabel5.Key.StartsWith("nametext"))
                                {
                                    disLabel5.Value.SetColor(Engine.Color.White);
                                }
                            }
                            nametext10.SetColor(Engine.Color.LightBlue);
                            itemBtn10.Selected = true;
                        };
                        _standardDialogPopup._disButtons.Add("itemBtn" + num24, itemBtn10);
                    }
                    if (num24 > 5)
                    {
                        _standardDialogPopup._disButtons["chatScrollerBtn"].Hidden = false;
                        _standardDialogPopup._disButtons["chatScrollUpBtn"].Enabled = true;
                        _standardDialogPopup._disButtons["chatScrollDownBtn"].Enabled = true;
                    }
                }
                if (scriptName == "Dye Equipment" && scriptNum == 1)
                {
                    text2 = "Dye Equipment";
                    int fullcount9 = 36;
                    RepositionScroller(_dye2DialogIndex, fullcount9 - 15, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    button2._onHeldEvent = delegate
                    {
                        if (_dye2DialogIndex > 0)
                        {
                            _dye2DialogIndex -= 3;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_dye2DialogIndex, fullcount9 - 15, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button2._onPressEvent = delegate
                    {
                        if (_dye2DialogIndex > 0)
                        {
                            _dye2DialogIndex -= 3;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_dye2DialogIndex, fullcount9 - 15, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onHeldEvent = delegate
                    {
                        if (_dye2DialogIndex < 21)
                        {
                            _dye2DialogIndex += 3;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_dye2DialogIndex, fullcount9 - 15, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onPressEvent = delegate
                    {
                        if (_dye2DialogIndex < 21)
                        {
                            _dye2DialogIndex += 3;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_dye2DialogIndex, fullcount9 - 15, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button4._onHeldEvent = delegate
                    {
                        double num101 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num102 = (int)(3.0 * Math.Round(num101 * (double)(fullcount9 - 15) / 3.0));
                        _dye2DialogIndex = ((num102 >= 0) ? ((num102 > fullcount9 - 15) ? (fullcount9 - 15) : num102) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_dye2DialogIndex, fullcount9 - 15, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    button5._onPressEvent = delegate
                    {
                        double num99 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num100 = (int)(3.0 * Math.Round(num99 * (double)(fullcount9 - 15) / 3.0));
                        _dye2DialogIndex = ((num100 >= 0) ? ((num100 > fullcount9 - 15) ? (fullcount9 - 15) : num100) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_dye2DialogIndex, fullcount9 - 15, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    string value8 = questvars["dyeequip"];
                    InventoryItem theitem = null;
                    foreach (InventoryItem item16 in _inventory.OrderBy((InventoryItem z) => z._slot))
                    {
                        if (item16._name.Equals(value8, StringComparison.CurrentCultureIgnoreCase))
                        {
                            theitem = item16;
                        }
                    }
                    _ = theitem._gender;
                    int num25 = 1;
                    int num26 = 0;
                    int num27 = 0;
                    int num28 = 0;
                    string text38 = "";
                    bool khan = true;
                    if (theitem._tab == "Helmet" || theitem._tab == "Helmet M")
                    {
                        text38 = "mh" + ConvertMydaNum(theitem._bodyImg).ToString("000") + "01_F5_C";
                    }
                    else if (theitem._tab == "Helmet F")
                    {
                        text38 = "wh" + ConvertMydaNum(theitem._bodyImg, 0).ToString("000") + "01_F5_C";
                    }
                    else
                    {
                        khan = false;
                        string text39 = "";
                        if (theitem._source == "new")
                        {
                            text39 = "_new";
                            text38 = "item" + theitem._type.ToString("000") + "_F" + theitem._frame + text39 + "_C";
                        }
                        else if (theitem._source == "myda")
                        {
                            text39 = "_myda";
                            text38 = "item" + theitem._type.ToString("000") + "_F" + theitem._frame + text39 + "_C";
                        }
                        else if (theitem._source == "custom")
                        {
                            text39 = "_custom";
                            text38 = "item" + theitem._type.ToString("000") + "_F" + (theitem._frame + 1) + text39 + "_C";
                        }
                        else
                        {
                            text38 = "item" + theitem._type.ToString("000") + "_F" + theitem._frame + text39 + "_C";
                        }
                        num28 = 20;
                    }
                    string source = theitem._source;
                    Sprite sprite16 = new Sprite();
                    sprite16._windowOffset = new Vector(342.0, 30 + num28, 0.0);
                    sprite16.Texture = _textureManager.Get(text38 + theitem._bodyImgColor, ".epf", source, null, khan);
                    sprite16.SetPosition(_standardDialogPopup._position.X + 342.0, _standardDialogPopup._position.Y + 30.0 + (double)num28);
                    _standardDialogPopup._backgrounds.Add("itmImg", sprite16);
                    for (int num29 = 0; num29 < 36; num29++)
                    {
                        string text40 = "";
                        Texture texture3 = _textureManager.Get(text38 + num29, ".epf", source, null, khan);
                        num26++;
                        if (num26 <= _dye2DialogIndex)
                        {
                            continue;
                        }
                        if (num25 > 5)
                        {
                            break;
                        }
                        Vector vector10 = new Vector(40 + num27 * 125, 86 + num25 * 35, 0.0);
                        num27++;
                        if (num27 == 3)
                        {
                            num27 = 0;
                            num25++;
                        }
                        Text nametext9 = DrawLabel(((Dye)num29).ToString(), Engine.Color.White, vector10.X + 40.0, vector10.Y + 12.0, 230, "left", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        _standardDialogPopup._disLabels.Add("nametext" + num26, nametext9);
                        int test = num29;
                        Engine.Button itemBtn9 = new Engine.Button(_textureManager, vector10.X, vector10.Y, 123.0, 33.0, text40, text40, text40, "", null, null, 0, multHorzontal: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        itemBtn9._windowOffset = new Vector(vector10.X, vector10.Y, 0.0);
                        itemBtn9.Sprite._windowOffset = new Vector(-14.0, -16 + num28, 0.0);
                        itemBtn9.Sprite.SetPosition(vector10.X + _standardDialogPopup._position.X - 14.0, vector10.Y + (double)num28 + _standardDialogPopup._position.Y - 16.0);
                        itemBtn9._baseImage = texture3;
                        itemBtn9._clickedImage = texture3;
                        itemBtn9._focusedImage = texture3;
                        itemBtn9.Sprite.Texture = texture3;
                        itemBtn9._onDoublePressEvent = delegate
                        {
                            theitem._bodyImgColor = test;
                            theitem.RefreshImage();
                            SaveItems();
                            DialogPopup(en, scriptName, 1, img);
                        };
                        itemBtn9._onPressEvent = delegate
                        {
                            foreach (KeyValuePair<string, Text> disLabel6 in _standardDialogPopup._disLabels)
                            {
                                if (disLabel6.Key.StartsWith("nametext"))
                                {
                                    disLabel6.Value.SetColor(Engine.Color.White);
                                }
                            }
                            nametext9.SetColor(Engine.Color.LightBlue);
                            itemBtn9.Selected = true;
                        };
                        _standardDialogPopup._disButtons.Add("itemBtn" + num26, itemBtn9);
                    }
                    if (num26 > 15)
                    {
                        _standardDialogPopup._disButtons["chatScrollerBtn"].Hidden = false;
                        _standardDialogPopup._disButtons["chatScrollUpBtn"].Enabled = true;
                        _standardDialogPopup._disButtons["chatScrollDownBtn"].Enabled = true;
                    }
                }
                // === Handler: Forget Skill ===
                if (scriptName == "Forget Skill")
                {
                    text2 = "Forget Skill";
                    int fullcount8 = 0;
                    foreach (Skill skill in _skills)
                    {
                        _ = skill;
                        fullcount8++;
                    }
                    int num30 = fullcount8 % 2;
                    if (num30 > 0)
                    {
                        fullcount8++;
                    }
                    RepositionScroller(_frgtSkillDialogIndex, fullcount8 - 10, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    button2._onHeldEvent = delegate
                    {
                        if (_frgtSkillDialogIndex > 0)
                        {
                            _frgtSkillDialogIndex -= 2;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_frgtSkillDialogIndex, fullcount8 - 10, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button2._onPressEvent = delegate
                    {
                        if (_frgtSkillDialogIndex > 0)
                        {
                            _frgtSkillDialogIndex -= 2;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_frgtSkillDialogIndex, fullcount8 - 10, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onHeldEvent = delegate
                    {
                        if (_frgtSkillDialogIndex < _skills.Count - 10)
                        {
                            _frgtSkillDialogIndex += 2;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_frgtSkillDialogIndex, fullcount8 - 10, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onPressEvent = delegate
                    {
                        if (_frgtSkillDialogIndex < _skills.Count - 10)
                        {
                            _frgtSkillDialogIndex += 2;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_frgtSkillDialogIndex, fullcount8 - 10, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button4._onHeldEvent = delegate
                    {
                        double num97 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num98 = (int)(2.0 * Math.Round(num97 * (double)(fullcount8 - 10) / 2.0));
                        _frgtSkillDialogIndex = ((num98 >= 0) ? ((num98 > fullcount8 - 10) ? (fullcount8 - 10) : num98) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_frgtSkillDialogIndex, fullcount8 - 10, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    button5._onPressEvent = delegate
                    {
                        double num95 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num96 = (int)(2.0 * Math.Round(num95 * (double)(fullcount8 - 10) / 2.0));
                        _frgtSkillDialogIndex = ((num96 >= 0) ? ((num96 > fullcount8 - 10) ? (fullcount8 - 10) : num96) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_frgtSkillDialogIndex, fullcount8 - 10, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    int num31 = 1;
                    int num32 = 0;
                    int num33 = 0;
                    foreach (Skill item17 in _skills.OrderBy((Skill z) => z._slot))
                    {
                        int slot3 = item17._slot;
                        string text41 = "";
                        foreach (JToken item18 in _skillsDB["skills"].Children())
                        {
                            if (item18.Value<string>("name") == item17._name)
                            {
                                int num34 = item18.Value<int>("image");
                                text41 = "skill001_F" + (num34 - 1);
                                break;
                            }
                        }
                        num32++;
                        if (num32 <= _frgtSkillDialogIndex)
                        {
                            continue;
                        }
                        if (num31 > 5)
                        {
                            break;
                        }
                        Vector vector11 = new Vector(40 + num33 * 185, 86 + num31 * 35, 0.0);
                        num33++;
                        if (num33 == 2)
                        {
                            num33 = 0;
                            num31++;
                        }
                        Text nametext8 = DrawLabel(item17._name, Engine.Color.White, vector11.X + 40.0, vector11.Y + 12.0, 230, "left", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        _standardDialogPopup._disLabels.Add("nametext" + num32, nametext8);
                        Engine.Button itemBtn8 = new Engine.Button(_textureManager, vector11.X, vector11.Y, 168.0, 33.0, text41, text41, text41, "", null, null, 0, multHorzontal: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        itemBtn8._windowOffset = new Vector(vector11.X, vector11.Y, 0.0);
                        itemBtn8._onDoublePressEvent = delegate
                        {
                            if (_frgtSkillDialogIndex > 0)
                            {
                                _frgtSkillDialogIndex -= 2;
                            }
                            RemoveSkill(slot3);
                            DialogPopup(en, scriptName, 0, img);
                        };
                        itemBtn8._onPressEvent = delegate
                        {
                            foreach (KeyValuePair<string, Text> disLabel7 in _standardDialogPopup._disLabels)
                            {
                                if (disLabel7.Key.StartsWith("nametext"))
                                {
                                    disLabel7.Value.SetColor(Engine.Color.White);
                                }
                            }
                            nametext8.SetColor(Engine.Color.LightBlue);
                            itemBtn8.Selected = true;
                        };
                        _standardDialogPopup._disButtons.Add("itemBtn" + num32, itemBtn8);
                    }
                    if (num32 > 10)
                    {
                        _standardDialogPopup._disButtons["chatScrollerBtn"].Hidden = false;
                        _standardDialogPopup._disButtons["chatScrollUpBtn"].Enabled = true;
                        _standardDialogPopup._disButtons["chatScrollDownBtn"].Enabled = true;
                    }
                }
                // === Handler: Forget Spell ===
                if (scriptName == "Forget Spell")
                {
                    text2 = "Forget Spell";
                    int fullcount7 = 0;
                    foreach (Spell spell in _spells)
                    {
                        _ = spell;
                        fullcount7++;
                    }
                    RepositionScroller(_frgtSpellDialogIndex, fullcount7 - 10, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    button2._onHeldEvent = delegate
                    {
                        if (_frgtSpellDialogIndex > 0)
                        {
                            _frgtSpellDialogIndex -= 2;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_frgtSpellDialogIndex, fullcount7 - 10, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button2._onPressEvent = delegate
                    {
                        if (_frgtSpellDialogIndex > 0)
                        {
                            _frgtSpellDialogIndex -= 2;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_frgtSpellDialogIndex, fullcount7 - 10, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onHeldEvent = delegate
                    {
                        if (_frgtSpellDialogIndex < _spells.Count - 10)
                        {
                            _frgtSpellDialogIndex += 2;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_frgtSpellDialogIndex, fullcount7 - 10, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onPressEvent = delegate
                    {
                        if (_frgtSpellDialogIndex < _spells.Count - 10)
                        {
                            _frgtSpellDialogIndex += 2;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_frgtSpellDialogIndex, fullcount7 - 10, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button4._onHeldEvent = delegate
                    {
                        double num93 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num94 = (int)(2.0 * Math.Round(num93 * (double)(fullcount7 - 10) / 2.0));
                        _frgtSpellDialogIndex = ((num94 >= 0) ? ((num94 > fullcount7 - 10) ? (fullcount7 - 10) : num94) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_frgtSpellDialogIndex, fullcount7 - 10, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    button5._onPressEvent = delegate
                    {
                        double num91 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num92 = (int)(2.0 * Math.Round(num91 * (double)(fullcount7 - 10) / 2.0));
                        _frgtSpellDialogIndex = ((num92 >= 0) ? ((num92 > fullcount7 - 10) ? (fullcount7 - 10) : num92) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_frgtSpellDialogIndex, fullcount7 - 10, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    int num35 = 1;
                    int num36 = 0;
                    int num37 = 0;
                    foreach (Spell item19 in _spells.OrderBy((Spell z) => z._slot))
                    {
                        int slot2 = item19._slot;
                        string text42 = "";
                        foreach (JToken item20 in _spellsDB["spells"].Children())
                        {
                            if (item20.Value<string>("name") == item19._name)
                            {
                                int num38 = item20.Value<int>("image");
                                text42 = "spell001_F" + (num38 - 1);
                                break;
                            }
                        }
                        num36++;
                        if (num36 <= _frgtSpellDialogIndex)
                        {
                            continue;
                        }
                        if (num35 > 5)
                        {
                            break;
                        }
                        Vector vector12 = new Vector(40 + num37 * 185, 86 + num35 * 35, 0.0);
                        num37++;
                        if (num37 == 2)
                        {
                            num37 = 0;
                            num35++;
                        }
                        Text nametext7 = DrawLabel(item19._name, Engine.Color.White, vector12.X + 40.0, vector12.Y + 12.0, 230, "left", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        _standardDialogPopup._disLabels.Add("nametext" + num36, nametext7);
                        Engine.Button itemBtn7 = new Engine.Button(_textureManager, vector12.X, vector12.Y, 168.0, 33.0, text42, text42, text42, "", null, null, 0, multHorzontal: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        itemBtn7._windowOffset = new Vector(vector12.X, vector12.Y, 0.0);
                        itemBtn7._onDoublePressEvent = delegate
                        {
                            if (_frgtSpellDialogIndex > 0)
                            {
                                _frgtSpellDialogIndex -= 2;
                            }
                            RemoveSpell(slot2);
                            DialogPopup(en, scriptName, 0, img);
                        };
                        itemBtn7._onPressEvent = delegate
                        {
                            foreach (KeyValuePair<string, Text> disLabel8 in _standardDialogPopup._disLabels)
                            {
                                if (disLabel8.Key.StartsWith("nametext"))
                                {
                                    disLabel8.Value.SetColor(Engine.Color.White);
                                }
                            }
                            nametext7.SetColor(Engine.Color.LightBlue);
                            itemBtn7.Selected = true;
                        };
                        _standardDialogPopup._disButtons.Add("itemBtn" + num36, itemBtn7);
                    }
                    if (num36 > 10)
                    {
                        _standardDialogPopup._disButtons["chatScrollerBtn"].Hidden = false;
                        _standardDialogPopup._disButtons["chatScrollUpBtn"].Enabled = true;
                        _standardDialogPopup._disButtons["chatScrollDownBtn"].Enabled = true;
                    }
                }
                // === Handler: Forget Action ===
                if (scriptName == "Forget Action")
                {
                    text2 = "Forget Action";
                    int fullcount6 = 0;
                    foreach (Action action in _actions)
                    {
                        _ = action;
                        fullcount6++;
                    }
                    RepositionScroller(_frgtActionDialogIndex, fullcount6 - 10, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    button2._onHeldEvent = delegate
                    {
                        if (_frgtActionDialogIndex > 0)
                        {
                            _frgtActionDialogIndex--;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_frgtActionDialogIndex, fullcount6 - 10, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button2._onPressEvent = delegate
                    {
                        if (_frgtActionDialogIndex > 0)
                        {
                            _frgtActionDialogIndex--;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_frgtActionDialogIndex, fullcount6 - 10, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onHeldEvent = delegate
                    {
                        if (_frgtActionDialogIndex < _actions.Count - 10)
                        {
                            _frgtActionDialogIndex++;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_frgtActionDialogIndex, fullcount6 - 10, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onPressEvent = delegate
                    {
                        if (_frgtActionDialogIndex < _actions.Count - 10)
                        {
                            _frgtActionDialogIndex++;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_frgtActionDialogIndex, fullcount6 - 10, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button4._onHeldEvent = delegate
                    {
                        double num89 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num90 = (int)(2.0 * Math.Round(num89 * (double)(fullcount6 - 10) / 2.0));
                        _frgtActionDialogIndex = ((num90 >= 0) ? ((num90 > fullcount6 - 10) ? (fullcount6 - 10) : num90) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_frgtActionDialogIndex, fullcount6 - 10, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    button5._onPressEvent = delegate
                    {
                        double num87 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num88 = (int)(2.0 * Math.Round(num87 * (double)(fullcount6 - 10) / 2.0));
                        _frgtActionDialogIndex = ((num88 >= 0) ? ((num88 > fullcount6 - 10) ? (fullcount6 - 10) : num88) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_frgtActionDialogIndex, fullcount6 - 10, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    int num39 = 1;
                    int num40 = 0;
                    int num41 = 0;
                    foreach (Action item21 in _actions.OrderBy((Action z) => z._slot))
                    {
                        int slot = item21._slot;
                        string text43 = "";
                        foreach (JToken item22 in _actionsDB["actions"].Children())
                        {
                            if (item22.Value<string>("name") == item21._name)
                            {
                                int num42 = item22.Value<int>("image");
                                string text44 = item22.Value<string>("imgpath");
                                if (string.IsNullOrEmpty(text44))
                                {
                                    text44 = "skill001";
                                }
                                text43 = text44 + "_F" + (num42 - 1);
                                break;
                            }
                        }
                        num40++;
                        if (num40 <= _frgtActionDialogIndex)
                        {
                            continue;
                        }
                        if (num39 > 5)
                        {
                            break;
                        }
                        Vector vector13 = new Vector(40 + num41 * 185, 86 + num39 * 35, 0.0);
                        num41++;
                        if (num41 == 2)
                        {
                            num41 = 0;
                            num39++;
                        }
                        Text nametext6 = DrawLabel(item21._name, Engine.Color.White, vector13.X + 40.0, vector13.Y + 12.0, 230, "left", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        _standardDialogPopup._disLabels.Add("nametext" + num40, nametext6);
                        Engine.Button itemBtn6 = new Engine.Button(_textureManager, vector13.X, vector13.Y, 168.0, 33.0, text43, text43, text43, "", null, null, 0, multHorzontal: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        itemBtn6._windowOffset = new Vector(vector13.X, vector13.Y, 0.0);
                        itemBtn6._onDoublePressEvent = delegate
                        {
                            if (_frgtActionDialogIndex > 0)
                            {
                                _frgtActionDialogIndex -= 2;
                            }
                            RemoveAction(slot);
                            DialogPopup(en, scriptName, 0, img);
                        };
                        itemBtn6._onPressEvent = delegate
                        {
                            foreach (KeyValuePair<string, Text> disLabel9 in _standardDialogPopup._disLabels)
                            {
                                if (disLabel9.Key.StartsWith("nametext"))
                                {
                                    disLabel9.Value.SetColor(Engine.Color.White);
                                }
                            }
                            nametext6.SetColor(Engine.Color.LightBlue);
                            itemBtn6.Selected = true;
                        };
                        _standardDialogPopup._disButtons.Add("itemBtn" + num40, itemBtn6);
                    }
                    if (num40 > 10)
                    {
                        _standardDialogPopup._disButtons["chatScrollerBtn"].Hidden = false;
                        _standardDialogPopup._disButtons["chatScrollUpBtn"].Enabled = true;
                        _standardDialogPopup._disButtons["chatScrollDownBtn"].Enabled = true;
                    }
                }
                // === Handler: Learn Skill ===
                if (scriptName == "Learn Skill")
                {
                    text2 = "Learn Skill";
                    int fullcount5 = 0;
                    foreach (string learnskill in en._learnskills)
                    {
                        if (!HasSkill(learnskill))
                        {
                            fullcount5++;
                        }
                    }
                    RepositionScroller(_lrnSkillDialogIndex, fullcount5 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    button2._onHeldEvent = delegate
                    {
                        if (_lrnSkillDialogIndex > 0)
                        {
                            _lrnSkillDialogIndex--;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_lrnSkillDialogIndex, fullcount5 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button2._onPressEvent = delegate
                    {
                        if (_lrnSkillDialogIndex > 0)
                        {
                            _lrnSkillDialogIndex--;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_lrnSkillDialogIndex, fullcount5 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onHeldEvent = delegate
                    {
                        if (_lrnSkillDialogIndex < _standardDialogPopup._learnskills.Count - 5)
                        {
                            _lrnSkillDialogIndex++;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_lrnSkillDialogIndex, fullcount5 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onPressEvent = delegate
                    {
                        if (_lrnSkillDialogIndex < _standardDialogPopup._learnskills.Count - 5)
                        {
                            _lrnSkillDialogIndex++;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_lrnSkillDialogIndex, fullcount5 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button4._onHeldEvent = delegate
                    {
                        double num85 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num86 = (int)Math.Round(num85 * (double)(fullcount5 - 5));
                        _lrnSkillDialogIndex = ((num86 >= 0) ? ((num86 > fullcount5 - 5) ? (fullcount5 - 5) : num86) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_lrnSkillDialogIndex, fullcount5 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    button5._onPressEvent = delegate
                    {
                        double num83 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num84 = (int)Math.Round(num83 * (double)(fullcount5 - 5));
                        _lrnSkillDialogIndex = ((num84 >= 0) ? ((num84 > fullcount5 - 5) ? (fullcount5 - 5) : num84) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_lrnSkillDialogIndex, fullcount5 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    int num43 = 0;
                    int num44 = 0;
                    foreach (string learnskill2 in en._learnskills)
                    {
                        if (HasSkill(learnskill2))
                        {
                            continue;
                        }
                        string text45 = "";
                        foreach (JToken item23 in _skillsDB["skills"].Children())
                        {
                            if (item23.Value<string>("name") == learnskill2)
                            {
                                int num45 = item23.Value<int>("image");
                                text45 = "skill001_F" + (num45 - 1);
                                break;
                            }
                        }
                        _standardDialogPopup._learnskills.Add(learnskill2);
                        num44++;
                        if (num44 <= _lrnSkillDialogIndex)
                        {
                            continue;
                        }
                        num43++;
                        if (num43 > 5)
                        {
                            break;
                        }
                        Vector vector14 = new Vector(40.0, 86 + num43 * 35, 0.0);
                        Text nametext5 = DrawLabel(learnskill2, Engine.Color.White, vector14.X + 40.0, vector14.Y + 12.0, 230, "left", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        _standardDialogPopup._disLabels.Add("nametext" + num44, nametext5);
                        Engine.Button itemBtn5 = new Engine.Button(_textureManager, vector14.X, vector14.Y, 363.0, 33.0, text45, text45, text45, "", null, null, 0, multHorzontal: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        itemBtn5._windowOffset = new Vector(vector14.X, vector14.Y, 0.0);
                        itemBtn5._onDoublePressEvent = delegate
                        {
                            NewSkill(nametext5._text, FirstAvailableSkillSlot(), 0);
                            if (_lrnSkillDialogIndex > 0)
                            {
                                _lrnSkillDialogIndex--;
                            }
                            DialogPopup(en, scriptName, 0, img);
                            RepositionScroller(_lrnSkillDialogIndex, fullcount5 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        };
                        itemBtn5._onPressEvent = delegate
                        {
                            foreach (KeyValuePair<string, Text> disLabel10 in _standardDialogPopup._disLabels)
                            {
                                if (disLabel10.Key.StartsWith("nametext"))
                                {
                                    disLabel10.Value.SetColor(Engine.Color.White);
                                }
                            }
                            nametext5.SetColor(Engine.Color.LightBlue);
                            itemBtn5.Selected = true;
                        };
                        _standardDialogPopup._disButtons.Add("itemBtn" + num44, itemBtn5);
                    }
                    if (num44 > 5)
                    {
                        _standardDialogPopup._disButtons["chatScrollerBtn"].Hidden = false;
                        _standardDialogPopup._disButtons["chatScrollUpBtn"].Enabled = true;
                        _standardDialogPopup._disButtons["chatScrollDownBtn"].Enabled = true;
                    }
                }
                // === Handler: Learn Spell ===
                if (scriptName == "Learn Spell")
                {
                    text2 = "Learn Spell";
                    int fullcount4 = 0;
                    foreach (string learnspell in en._learnspells)
                    {
                        if (!HasSpell(learnspell))
                        {
                            fullcount4++;
                        }
                    }
                    RepositionScroller(_lrnSpellDialogIndex, fullcount4 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    button2._onHeldEvent = delegate
                    {
                        if (_lrnSpellDialogIndex > 0)
                        {
                            _lrnSpellDialogIndex--;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_lrnSpellDialogIndex, fullcount4 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button2._onPressEvent = delegate
                    {
                        if (_lrnSpellDialogIndex > 0)
                        {
                            _lrnSpellDialogIndex--;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_lrnSpellDialogIndex, fullcount4 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onHeldEvent = delegate
                    {
                        if (_lrnSpellDialogIndex < _standardDialogPopup._learnspells.Count - 5)
                        {
                            _lrnSpellDialogIndex++;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_lrnSpellDialogIndex, fullcount4 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onPressEvent = delegate
                    {
                        if (_lrnSpellDialogIndex < _standardDialogPopup._learnspells.Count - 5)
                        {
                            _lrnSpellDialogIndex++;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_lrnSpellDialogIndex, fullcount4 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button4._onHeldEvent = delegate
                    {
                        double num81 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num82 = (int)Math.Round(num81 * (double)(fullcount4 - 5));
                        _lrnSpellDialogIndex = ((num82 >= 0) ? ((num82 > fullcount4 - 5) ? (fullcount4 - 5) : num82) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_lrnSpellDialogIndex, fullcount4 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    button5._onPressEvent = delegate
                    {
                        double num79 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num80 = (int)Math.Round(num79 * (double)(fullcount4 - 5));
                        _lrnSpellDialogIndex = ((num80 >= 0) ? ((num80 > fullcount4 - 5) ? (fullcount4 - 5) : num80) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_lrnSpellDialogIndex, fullcount4 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    int num46 = 0;
                    int num47 = 0;
                    foreach (string learnspell2 in en._learnspells)
                    {
                        if (HasSpell(learnspell2))
                        {
                            continue;
                        }
                        string text46 = "";
                        foreach (JToken item24 in _spellsDB["spells"].Children())
                        {
                            if (item24.Value<string>("name") == learnspell2)
                            {
                                int num48 = item24.Value<int>("image");
                                text46 = "spell001_F" + (num48 - 1);
                                break;
                            }
                        }
                        _standardDialogPopup._learnspells.Add(learnspell2);
                        num47++;
                        if (num47 <= _lrnSpellDialogIndex)
                        {
                            continue;
                        }
                        num46++;
                        if (num46 > 5)
                        {
                            break;
                        }
                        Vector vector15 = new Vector(40.0, 86 + num46 * 35, 0.0);
                        Text nametext4 = DrawLabel(learnspell2, Engine.Color.White, vector15.X + 40.0, vector15.Y + 12.0, 230, "left", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        _standardDialogPopup._disLabels.Add("nametext" + num47, nametext4);
                        Engine.Button itemBtn4 = new Engine.Button(_textureManager, vector15.X, vector15.Y, 363.0, 33.0, text46, text46, text46, "", null, null, 0, multHorzontal: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        itemBtn4._windowOffset = new Vector(vector15.X, vector15.Y, 0.0);
                        itemBtn4._onDoublePressEvent = delegate
                        {
                            NewSpell(nametext4._text, FirstAvailableSpellSlot(), 0);
                            if (_lrnSpellDialogIndex > 0)
                            {
                                _lrnSpellDialogIndex--;
                            }
                            DialogPopup(en, scriptName, 0, img);
                            RepositionScroller(_lrnSpellDialogIndex, fullcount4 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        };
                        itemBtn4._onPressEvent = delegate
                        {
                            foreach (KeyValuePair<string, Text> disLabel11 in _standardDialogPopup._disLabels)
                            {
                                if (disLabel11.Key.StartsWith("nametext"))
                                {
                                    disLabel11.Value.SetColor(Engine.Color.White);
                                }
                            }
                            nametext4.SetColor(Engine.Color.LightBlue);
                            itemBtn4.Selected = true;
                        };
                        _standardDialogPopup._disButtons.Add("itemBtn" + num47, itemBtn4);
                    }
                    if (num47 > 5)
                    {
                        _standardDialogPopup._disButtons["chatScrollerBtn"].Hidden = false;
                        _standardDialogPopup._disButtons["chatScrollUpBtn"].Enabled = true;
                        _standardDialogPopup._disButtons["chatScrollDownBtn"].Enabled = true;
                    }
                }
                // === Handler: Learn Action ===
                if (scriptName == "Learn Action")
                {
                    text2 = "Learn Action";
                    int fullcount3 = 0;
                    foreach (string learnaction in en._learnactions)
                    {
                        if (!HasAction(learnaction))
                        {
                            fullcount3++;
                        }
                    }
                    RepositionScroller(_lrnActionDialogIndex, fullcount3 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    button2._onHeldEvent = delegate
                    {
                        if (_lrnActionDialogIndex > 0)
                        {
                            _lrnActionDialogIndex--;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_lrnActionDialogIndex, fullcount3 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button2._onPressEvent = delegate
                    {
                        if (_lrnActionDialogIndex > 0)
                        {
                            _lrnActionDialogIndex--;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_lrnActionDialogIndex, fullcount3 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onHeldEvent = delegate
                    {
                        if (_lrnActionDialogIndex < _standardDialogPopup._learnactions.Count - 5)
                        {
                            _lrnActionDialogIndex++;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_lrnActionDialogIndex, fullcount3 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onPressEvent = delegate
                    {
                        if (_lrnActionDialogIndex < _standardDialogPopup._learnactions.Count - 5)
                        {
                            _lrnActionDialogIndex++;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_lrnActionDialogIndex, fullcount3 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button4._onHeldEvent = delegate
                    {
                        double num77 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num78 = (int)Math.Round(num77 * (double)(fullcount3 - 5));
                        _lrnActionDialogIndex = ((num78 >= 0) ? ((num78 > fullcount3 - 5) ? (fullcount3 - 5) : num78) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_lrnActionDialogIndex, fullcount3 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    button5._onPressEvent = delegate
                    {
                        double num75 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num76 = (int)Math.Round(num75 * (double)(fullcount3 - 5));
                        _lrnActionDialogIndex = ((num76 >= 0) ? ((num76 > fullcount3 - 5) ? (fullcount3 - 5) : num76) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_lrnActionDialogIndex, fullcount3 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    int num49 = 0;
                    int num50 = 0;
                    foreach (string learnaction2 in en._learnactions)
                    {
                        if (HasAction(learnaction2))
                        {
                            continue;
                        }
                        string text47 = "";
                        string textureId = "";
                        foreach (JToken item25 in _actionsDB["actions"].Children())
                        {
                            if (item25.Value<string>("name") == learnaction2)
                            {
                                string text48 = item25.Value<string>("imgpath");
                                int num51 = item25.Value<int>("image");
                                if (string.IsNullOrEmpty(text48))
                                {
                                    text48 = "skill001";
                                }
                                text47 = item25.Value<string>("source");
                                if (string.IsNullOrEmpty(text47))
                                {
                                    text47 = "old";
                                }
                                string text49 = "";
                                if (text47 == "new")
                                {
                                    text49 = "_new";
                                }
                                if (text47 == "myda")
                                {
                                    text49 = "_myda";
                                }
                                textureId = text48 + "_F" + (num51 - 1) + text49 + "_C0";
                                break;
                            }
                        }
                        _standardDialogPopup._learnactions.Add(learnaction2);
                        num50++;
                        if (num50 <= _lrnActionDialogIndex)
                        {
                            continue;
                        }
                        num49++;
                        if (num49 > 5)
                        {
                            break;
                        }
                        Vector vector16 = new Vector(40.0, 86 + num49 * 35, 0.0);
                        Text nametext3 = DrawLabel(learnaction2, Engine.Color.White, vector16.X + 40.0, vector16.Y + 12.0, 230, "left", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        _standardDialogPopup._disLabels.Add("nametext" + num50, nametext3);
                        Engine.Button itemBtn3 = new Engine.Button(_textureManager, vector16.X, vector16.Y, 363.0, 33.0, "", "", "", "", null, null, 0, multHorzontal: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        itemBtn3._windowOffset = new Vector(vector16.X, vector16.Y, 0.0);
                        itemBtn3._baseImage = _textureManager.Get(textureId, ".epf", text47);
                        itemBtn3._clickedImage = _textureManager.Get(textureId, ".epf", text47);
                        itemBtn3._focusedImage = _textureManager.Get(textureId, ".epf", text47);
                        itemBtn3.Sprite.Texture = itemBtn3._baseImage;
                        itemBtn3._onDoublePressEvent = delegate
                        {
                            NewAction(nametext3._text, FirstAvailableActionSlot(), 0);
                            if (_lrnActionDialogIndex > 0)
                            {
                                _lrnActionDialogIndex--;
                            }
                            DialogPopup(en, scriptName, 0, img);
                            RepositionScroller(_lrnActionDialogIndex, fullcount3 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        };
                        itemBtn3._onPressEvent = delegate
                        {
                            foreach (KeyValuePair<string, Text> disLabel12 in _standardDialogPopup._disLabels)
                            {
                                if (disLabel12.Key.StartsWith("nametext"))
                                {
                                    disLabel12.Value.SetColor(Engine.Color.White);
                                }
                            }
                            nametext3.SetColor(Engine.Color.LightBlue);
                            itemBtn3.Selected = true;
                        };
                        _standardDialogPopup._disButtons.Add("itemBtn" + num50, itemBtn3);
                    }
                    if (num50 > 5)
                    {
                        _standardDialogPopup._disButtons["chatScrollerBtn"].Hidden = false;
                        _standardDialogPopup._disButtons["chatScrollUpBtn"].Enabled = true;
                        _standardDialogPopup._disButtons["chatScrollDownBtn"].Enabled = true;
                    }
                }
                // === Handler: Buy (merchant) ===
                if (scriptName == "Buy")
                {
                    text2 = "Buy";
                    int fullcount2 = 0;
                    foreach (string key2 in en._selllist.Keys)
                    {
                        string text50 = "";
                        foreach (JToken item26 in _itemsDB["items"].Children())
                        {
                            if (item26.Value<string>("name") == key2)
                            {
                                text50 = item26.Value<string>("tab");
                                break;
                            }
                        }
                        if (!(text50 != _selectedBuyDialogCat))
                        {
                            fullcount2++;
                        }
                    }
                    RepositionScroller(_buyDialogIndex, fullcount2 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    button2._onHeldEvent = delegate
                    {
                        if (_buyDialogIndex > 0)
                        {
                            _buyDialogIndex--;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_buyDialogIndex, fullcount2 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button2._onPressEvent = delegate
                    {
                        if (_buyDialogIndex > 0)
                        {
                            _buyDialogIndex--;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_buyDialogIndex, fullcount2 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onHeldEvent = delegate
                    {
                        if (_buyDialogIndex < _standardDialogPopup._selllist.Count - 5)
                        {
                            _buyDialogIndex++;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_buyDialogIndex, fullcount2 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onPressEvent = delegate
                    {
                        if (_buyDialogIndex < _standardDialogPopup._selllist.Count - 5)
                        {
                            _buyDialogIndex++;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_buyDialogIndex, fullcount2 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button4._onHeldEvent = delegate
                    {
                        double num73 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num74 = (int)Math.Round(num73 * (double)(fullcount2 - 5));
                        _buyDialogIndex = ((num74 >= 0) ? ((num74 > fullcount2 - 5) ? (fullcount2 - 5) : num74) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_buyDialogIndex, fullcount2 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    button5._onPressEvent = delegate
                    {
                        double num71 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num72 = (int)Math.Round(num71 * (double)(fullcount2 - 5));
                        _buyDialogIndex = ((num72 >= 0) ? ((num72 > fullcount2 - 5) ? (fullcount2 - 5) : num72) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_buyDialogIndex, fullcount2 - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    int fullcatcount = 0;
                    foreach (string key3 in en._selllist.Keys)
                    {
                        foreach (JToken item27 in _itemsDB["items"].Children())
                        {
                            if (item27.Value<string>("name") == key3)
                            {
                                string item2 = item27.Value<string>("tab");
                                if (!_standardDialogPopup._catlist.Contains(item2))
                                {
                                    _standardDialogPopup._catlist.Add(item2);
                                }
                                break;
                            }
                        }
                    }
                    fullcatcount = _standardDialogPopup._catlist.Count;
                    RepositionHorizontalScroller(_buyDialogCatIndex, fullcatcount - 4, 260, _standardDialogPopup, _standardDialogPopup._disButtons["catScrollerBtn"], 148, 82);
                    _standardDialogPopup._disButtons["catScrollUpBtn"]._onHeldEvent = delegate
                    {
                        if (_buyDialogCatIndex > 0)
                        {
                            _buyDialogCatIndex--;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionHorizontalScroller(_buyDialogCatIndex, fullcatcount - 4, 260, _standardDialogPopup, _standardDialogPopup._disButtons["catScrollerBtn"], 148, 82);
                        }
                    };
                    _standardDialogPopup._disButtons["catScrollUpBtn"]._onPressEvent = delegate
                    {
                        if (_buyDialogCatIndex > 0)
                        {
                            _buyDialogCatIndex--;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionHorizontalScroller(_buyDialogCatIndex, fullcatcount - 4, 260, _standardDialogPopup, _standardDialogPopup._disButtons["catScrollerBtn"], 148, 82);
                        }
                    };
                    _standardDialogPopup._disButtons["catScrollDownBtn"]._onHeldEvent = delegate
                    {
                        if (_buyDialogCatIndex < _standardDialogPopup._catlist.Count - 4)
                        {
                            _buyDialogCatIndex++;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionHorizontalScroller(_buyDialogCatIndex, fullcatcount - 4, 260, _standardDialogPopup, _standardDialogPopup._disButtons["catScrollerBtn"], 148, 82);
                        }
                    };
                    _standardDialogPopup._disButtons["catScrollDownBtn"]._onPressEvent = delegate
                    {
                        if (_buyDialogCatIndex < _standardDialogPopup._catlist.Count - 4)
                        {
                            _buyDialogCatIndex++;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionHorizontalScroller(_buyDialogCatIndex, fullcatcount - 4, 260, _standardDialogPopup, _standardDialogPopup._disButtons["catScrollerBtn"], 148, 82);
                        }
                    };
                    _standardDialogPopup._disButtons["catScrollerBtn"]._onHeldEvent = delegate
                    {
                        double num69 = ((double)_input.Mouse.Position.X - _standardDialogPopup._position.X - 148.0 - 7.5) / 265.5;
                        int num70 = (int)Math.Round(num69 * (double)(fullcatcount - 4));
                        _buyDialogCatIndex = ((num70 >= 0) ? ((num70 > fullcatcount - 4) ? (fullcatcount - 4) : num70) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionHorizontalScroller(_buyDialogCatIndex, fullcatcount - 4, 260, _standardDialogPopup, _standardDialogPopup._disButtons["catScrollerBtn"], 148, 82);
                    };
                    _standardDialogPopup._disButtons["catScrollBackBtn"]._onPressEvent = delegate
                    {
                        double num67 = ((double)_input.Mouse.Position.X - _standardDialogPopup._position.X - 148.0 - 7.5) / 265.5;
                        int num68 = (int)Math.Round(num67 * (double)(fullcatcount - 4));
                        _buyDialogCatIndex = ((num68 >= 0) ? ((num68 > fullcatcount - 4) ? (fullcatcount - 4) : num68) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionHorizontalScroller(_buyDialogCatIndex, fullcatcount - 4, 260, _standardDialogPopup, _standardDialogPopup._disButtons["catScrollerBtn"], 148, 82);
                    };
                    _standardDialogPopup._catlist.Sort();
                    bool flag11 = false;
                    foreach (string item28 in _standardDialogPopup._catlist)
                    {
                        if (item28 == _selectedBuyDialogCat)
                        {
                            flag11 = true;
                            break;
                        }
                    }
                    int num52 = 0;
                    int num53 = 0;
                    foreach (string item29 in _standardDialogPopup._catlist)
                    {
                        num53++;
                        if (num53 <= _buyDialogCatIndex)
                        {
                            continue;
                        }
                        num52++;
                        if (num52 > 4)
                        {
                            break;
                        }
                        Vector vector17 = new Vector(68 + num52 * 73, 60.0, 0.0);
                        Engine.Button catBtn = new Engine.Button(_textureManager, vector17.X, vector17.Y, 66.0, 18.0, "banktab_F1", "banktab_F2", "", "", null, null, 0, multHorzontal: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        catBtn._windowOffset = new Vector(vector17.X, vector17.Y, 0.0);
                        catBtn._text = DrawLabel(item29, Engine.Color.White, vector17.X, vector17.Y + 3.0, 66, "center", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        catBtn._onPressEvent = delegate
                        {
                            foreach (Engine.Button value11 in _standardDialogPopup._disButtons.Values)
                            {
                                value11.Selected = false;
                            }
                            catBtn.Selected = true;
                            _selectedBuyDialogCat = catBtn._text._text;
                            _buyDialogIndex = 0;
                            DialogPopup(en, scriptName, scriptNum, img);
                        };
                        if ((!flag11 && num52 == 1) || item29 == _selectedBuyDialogCat)
                        {
                            catBtn.Selected = true;
                            _selectedBuyDialogCat = item29;
                        }
                        _standardDialogPopup._disButtons.Add("catBtn" + num53, catBtn);
                    }
                    if (num53 > 4)
                    {
                        _standardDialogPopup._disButtons["catScrollerBtn"].Hidden = false;
                        _standardDialogPopup._disButtons["catScrollUpBtn"].Enabled = true;
                        _standardDialogPopup._disButtons["catScrollDownBtn"].Enabled = true;
                    }
                    int num54 = 0;
                    int num55 = 0;
                    foreach (KeyValuePair<string, int> item30 in en._selllist)
                    {
                        BankItem bi = null;
                        Texture texture4 = default(Texture);
                        string text51 = "";
                        foreach (JToken item31 in _itemsDB["items"].Children())
                        {
                            if (item31.Value<string>("name").ToLower() == item30.Key.ToLower())
                            {
                                text51 = item31.Value<string>("tab");
                                int num56 = item31.Value<int>("imagefolder");
                                int num57 = item31.Value<int>("frame");
                                string text52 = item31.Value<string>("imagetype");
                                string text53 = "";
                                if (string.IsNullOrEmpty(text52))
                                {
                                    text52 = "old";
                                }
                                if (text52 != "old")
                                {
                                    text53 = "_" + text52;
                                }
                                texture4 = _textureManager.Get("item" + num56.ToString("000") + "_F" + (num57 - 1) + text53 + "_C0", ".epf", text52);
                                bi = new BankItem(item30.Key, text51, 1, texture4);
                                bi._maxAmount = item31.Value<int>("stack");
                                bi._value = item31.Value<int>("value");
                                bi._description = item31.Value<string>("desc");
                                bi._maxDurability = item31.Value<int>("dura");
                                bi._dyeable = item31.Value<int>("dyeable");
                                JToken jToken10 = item31["stats"];
                                if (jToken10 != null)
                                {
                                    bi._level = jToken10.Value<string>("lev");
                                    bi._gender = jToken10.Value<string>("gender");
                                    bi._weaponDmg = jToken10.Value<string>("w");
                                    bi._atk = jToken10.Value<string>("atk");
                                    bi._def = jToken10.Value<string>("def");
                                    bi._hp = jToken10.Value<int>("hp");
                                    bi._mp = jToken10.Value<int>("mp");
                                    bi._str = jToken10.Value<int>("str");
                                    bi._int = jToken10.Value<int>("int");
                                    bi._wis = jToken10.Value<int>("wis");
                                    bi._con = jToken10.Value<int>("con");
                                    bi._dex = jToken10.Value<int>("dex");
                                    bi._mr = jToken10.Value<int>("mr");
                                    bi._ac = jToken10.Value<int>("ac");
                                    bi._dmg = jToken10.Value<int>("dmg");
                                    bi._hit = jToken10.Value<int>("hit");
                                    bi._reg = jToken10.Value<int>("reg");
                                }
                                break;
                            }
                        }
                        if (text51 != _selectedBuyDialogCat)
                        {
                            continue;
                        }
                        _standardDialogPopup._selllist.Add(item30.Key, item30.Value);
                        num55++;
                        if (num55 <= _buyDialogIndex)
                        {
                            continue;
                        }
                        num54++;
                        if (num54 > 5)
                        {
                            break;
                        }
                        Vector vector18 = new Vector(40.0, 86 + num54 * 35, 0.0);
                        Text nametext2 = DrawLabel(item30.Key, Engine.Color.White, vector18.X + 40.0, vector18.Y + 12.0, 230, "left", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        Text valuetext2 = DrawLabel(item30.Value.ToString("#,0"), Engine.Color.White, vector18.X + 280.0, vector18.Y + 12.0, 80, "right", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        _standardDialogPopup._disLabels.Add("nametext" + num55, nametext2);
                        _standardDialogPopup._disLabels.Add("valuetext" + num55, valuetext2);
                        int valuenum2 = int.Parse(valuetext2._text.Replace(",", ""));
                        Engine.Button itemBtn2 = new Engine.Button(_textureManager, vector18.X, vector18.Y, 363.0, 33.0, "", "", "", "", null, null, 0, multHorzontal: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        itemBtn2._windowOffset = new Vector(vector18.X, vector18.Y, 0.0);
                        itemBtn2._baseImage = texture4;
                        itemBtn2._clickedImage = texture4;
                        itemBtn2._focusedImage = texture4;
                        itemBtn2.Sprite.Texture = texture4;
                        itemBtn2._onHoverEvent = delegate
                        {
                            ItemToolTip(bi, _input.Mouse.Position);
                        };
                        itemBtn2._onStopHoverEvent = delegate
                        {
                            _tTT = "";
                            _toolTipText.ChangeText("");
                            _input.Mouse.SetCursorDefault();
                        };
                        itemBtn2._onDoublePressEvent = delegate
                        {
                            if (valuenum2 <= getItemSlot(72)._amount)
                            {
                                NewItem(nametext2._text, FirstAvailableInventorySlot());
                                getItemSlot(72)._amount -= valuenum2;
                                goldlab.ChangeText(getItemSlot(72)._amount.ToString("#,0"));
                            }
                        };
                        itemBtn2._onPressEvent = delegate
                        {
                            if (valuenum2 > getItemSlot(72)._amount)
                            {
                                goldlab.SetColor(Text.Colors(System.Drawing.Color.Red));
                            }
                            else
                            {
                                goldlab.SetColor(Engine.Color.White);
                            }
                            foreach (KeyValuePair<string, Text> disLabel13 in _standardDialogPopup._disLabels)
                            {
                                if (disLabel13.Key.StartsWith("nametext") || disLabel13.Key.StartsWith("valuetext"))
                                {
                                    disLabel13.Value.SetColor(Engine.Color.White);
                                }
                            }
                            nametext2.SetColor(Engine.Color.LightBlue);
                            valuetext2.SetColor(Engine.Color.LightBlue);
                            itemBtn2.Selected = true;
                        };
                        _standardDialogPopup._disButtons.Add("itemBtn" + num55, itemBtn2);
                    }
                    if (num55 > 5)
                    {
                        _standardDialogPopup._disButtons["chatScrollerBtn"].Hidden = false;
                        _standardDialogPopup._disButtons["chatScrollUpBtn"].Enabled = true;
                        _standardDialogPopup._disButtons["chatScrollDownBtn"].Enabled = true;
                    }
                }
                // === Handler: Sell (merchant) ===
                if (scriptName == "Sell")
                {
                    text2 = "Sell";
                    int fullcount = 0;
                    foreach (InventoryItem item32 in _inventory)
                    {
                        if (!item32._bound)
                        {
                            fullcount++;
                        }
                    }
                    RepositionScroller(_sellDialogIndex, fullcount - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    button2._onHeldEvent = delegate
                    {
                        if (_sellDialogIndex > 0)
                        {
                            _sellDialogIndex--;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_sellDialogIndex, fullcount - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button2._onPressEvent = delegate
                    {
                        if (_sellDialogIndex > 0)
                        {
                            _sellDialogIndex--;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_sellDialogIndex, fullcount - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onHeldEvent = delegate
                    {
                        if (_sellDialogIndex < _standardDialogPopup._buylist.Count - 5)
                        {
                            _sellDialogIndex++;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_sellDialogIndex, fullcount - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button3._onPressEvent = delegate
                    {
                        if (_sellDialogIndex < _standardDialogPopup._buylist.Count - 5)
                        {
                            _sellDialogIndex++;
                            DialogPopup(en, scriptName, scriptNum, img);
                            RepositionScroller(_sellDialogIndex, fullcount - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                        }
                    };
                    button4._onHeldEvent = delegate
                    {
                        double num65 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num66 = (int)Math.Round(num65 * (double)(fullcount - 5));
                        _sellDialogIndex = ((num66 >= 0) ? ((num66 > fullcount - 5) ? (fullcount - 5) : num66) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_sellDialogIndex, fullcount - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    button5._onPressEvent = delegate
                    {
                        double num63 = ((double)_input.Mouse.Position.Y - _standardDialogPopup._position.Y - 129.0 - 6.0) / 150.0;
                        int num64 = (int)Math.Round(num63 * (double)(fullcount - 5));
                        _sellDialogIndex = ((num64 >= 0) ? ((num64 > fullcount - 5) ? (fullcount - 5) : num64) : 0);
                        DialogPopup(en, scriptName, scriptNum, img);
                        RepositionScroller(_sellDialogIndex, fullcount - 5, 144, _standardDialogPopup, _standardDialogPopup._disButtons["chatScrollerBtn"], 420, 129);
                    };
                    foreach (InventoryItem item33 in _inventory.OrderBy((InventoryItem e) => e._slot))
                    {
                        if (!item33._bound)
                        {
                            _standardDialogPopup._buylist.Add(item33._slot, item33._name);
                        }
                    }
                    int num58 = 0;
                    int num59 = 0;
                    foreach (KeyValuePair<int, string> item34 in _standardDialogPopup._buylist)
                    {
                        InventoryItem itemSlot4 = getItemSlot(item34.Key);
                        num59++;
                        if (num59 <= _sellDialogIndex)
                        {
                            continue;
                        }
                        num58++;
                        if (num58 > 5)
                        {
                            break;
                        }
                        Vector vector19 = new Vector(40.0, 86 + num58 * 35, 0.0);
                        string text54 = item34.Value;
                        if (itemSlot4._maxAmount > 1)
                        {
                            text54 = text54 + " (" + itemSlot4._amount + ")";
                        }
                        Text nametext = DrawLabel(text54, Engine.Color.White, vector19.X + 40.0, vector19.Y + 12.0, 230, "left", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        Text valuetext = DrawLabel((itemSlot4._value / 2).ToString("#,0"), Engine.Color.White, vector19.X + 280.0, vector19.Y + 12.0, 80, "right", 1, shade: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        _standardDialogPopup._disLabels.Add("nametext" + num59, nametext);
                        _standardDialogPopup._disLabels.Add("valuetext" + num59, valuetext);
                        int valuenum = int.Parse(valuetext._text.Replace(",", ""));
                        int slotnum = itemSlot4._slot;
                        bool stackable = false;
                        if (itemSlot4._maxAmount > 1)
                        {
                            stackable = true;
                        }
                        Engine.Button itemBtn = new Engine.Button(_textureManager, vector19.X, vector19.Y, 363.0, 33.0, "", "", "", "", null, null, 0, multHorzontal: false, _standardDialogPopup._position.X, _standardDialogPopup._position.Y);
                        itemBtn._windowOffset = new Vector(vector19.X, vector19.Y, 0.0);
                        itemBtn._baseImage = itemSlot4._sprite.Texture;
                        itemBtn._clickedImage = itemSlot4._sprite.Texture;
                        itemBtn._focusedImage = itemSlot4._sprite.Texture;
                        itemBtn.Sprite.Texture = itemBtn._baseImage;
                        itemBtn._onDoublePressEvent = delegate
                        {
                            InventoryItem itemSlot6 = getItemSlot(72);
                            if (valuenum + itemSlot6._amount <= itemSlot6._maxAmount)
                            {
                                _standardDialogPopup._sellitem = slotnum;
                                if (!string.IsNullOrEmpty(var))
                                {
                                    questvars[var] = nametext._text;
                                }
                                if (stackable)
                                {
                                    DialogPopup(en, scriptName, 1, img);
                                }
                                else
                                {
                                    if (questvars.ContainsKey("sellnumber"))
                                    {
                                        questvars["sellnumber"] = "1";
                                    }
                                    else
                                    {
                                        questvars.Add("sellnumber", "1");
                                    }
                                    DialogPopup(en, scriptName, 2, img);
                                }
                            }
                        };
                        itemBtn._onPressEvent = delegate
                        {
                            foreach (KeyValuePair<string, Text> disLabel14 in _standardDialogPopup._disLabels)
                            {
                                if (disLabel14.Key.StartsWith("nametext") || disLabel14.Key.StartsWith("valuetext"))
                                {
                                    disLabel14.Value.SetColor(Engine.Color.White);
                                }
                            }
                            nametext.SetColor(Engine.Color.LightBlue);
                            valuetext.SetColor(Engine.Color.LightBlue);
                            itemBtn.Selected = true;
                        };
                        _standardDialogPopup._disButtons.Add("itemBtn" + num59, itemBtn);
                    }
                    if (num59 > 5)
                    {
                        _standardDialogPopup._disButtons["chatScrollerBtn"].Hidden = false;
                        _standardDialogPopup._disButtons["chatScrollUpBtn"].Enabled = true;
                        _standardDialogPopup._disButtons["chatScrollDownBtn"].Enabled = true;
                    }
                }
                _standardDialogPopup._buttons["sdpNextBtn"]._windowOffset = new Vector(90.0, 143.0 + num2, 0.0);
                _standardDialogPopup._buttons["sdpTopBtn"]._windowOffset = new Vector(20.0, 143.0 + num2, 0.0);
                _standardDialogPopup._buttons["sdpPreviousBtn"]._windowOffset = new Vector(20.0, 143.0 + num2, 0.0);
                _standardDialogPopup._buttons["sdpQuitBtn"]._windowOffset = new Vector(364.0, 143.0 + num2, 0.0);
                _standardDialogPopup._buttons["sdpNextBtn"].Position = new Vector(90.0 + _standardDialogPopup._position.X, 143.0 + num2 + _standardDialogPopup._position.Y, 0.0);
                _standardDialogPopup._buttons["sdpNextBtn"].Hidden = true;
                _standardDialogPopup._buttons["sdpPreviousBtn"].Position = new Vector(20.0 + _standardDialogPopup._position.X, 143.0 + num2 + _standardDialogPopup._position.Y, 0.0);
                if (scriptNum == 0)
                {
                    _standardDialogPopup._buttons["sdpPreviousBtn"].Hidden = true;
                    _standardDialogPopup._buttons["sdpTopBtn"].Position = new Vector(20.0 + _standardDialogPopup._position.X, 143.0 + num2 + _standardDialogPopup._position.Y, 0.0);
                    _standardDialogPopup._buttons["sdpTopBtn"].Hidden = false;
                    _standardDialogPopup._buttons["sdpTopBtn"].Enabled = true;
                    _standardDialogPopup._buttons["sdpTopBtn"]._onPressEvent = delegate
                    {
                        DialogPopup(en, en._name, 0, img);
                    };
                }
                _standardDialogPopup._buttons["sdpQuitBtn"].Position = new Vector(364.0 + _standardDialogPopup._position.X, 143.0 + num2 + _standardDialogPopup._position.Y, 0.0);
            }
            if (!string.IsNullOrEmpty(next))
            {
                _standardDialogPopup._buttons["sdpNextBtn"]._onPressEvent = delegate
                {
                    if (!string.IsNullOrEmpty(var) && _standardDialogPopup._disTextFields["inputTF"] != null)
                    {
                        if (questvars.ContainsKey(var))
                        {
                            questvars[var] = _standardDialogPopup._disTextFields["inputTF"].Text;
                        }
                        else
                        {
                            questvars.Add(var, _standardDialogPopup._disTextFields["inputTF"].Text);
                        }
                    }
                    if (var == "createguild" && questvars.ContainsKey(var))
                    {
                        if (guildExists(questvars[var]))
                        {
                            next = "2";
                        }
                        else
                        {
                            createGuild(questvars[var]);
                        }
                    }
                    if (var == "joinguild" && questvars.ContainsKey(var))
                    {
                        if (!guildExists(questvars[var]))
                        {
                            next = "3";
                        }
                        else if (playerInGuild(questvars[var]))
                        {
                            next = "2";
                        }
                        else
                        {
                            joinGuild(questvars[var]);
                        }
                    }
                    byte result;
                    if (next == "close")
                    {
                        _viewingDialog = false;
                    }
                    else if (byte.TryParse(next, out result))
                    {
                        DialogPopup(en, scriptName, result, img);
                    }
                    else
                    {
                        DialogPopup(en, next, 0);
                    }
                };
                if (text2 == "input")
                {
                    _standardDialogPopup._buttons["sdpNextBtn"].Enabled = false;
                }
                else
                {
                    _standardDialogPopup._buttons["sdpNextBtn"].Enabled = true;
                }
            }
            if (!string.IsNullOrEmpty(previous))
            {
                _standardDialogPopup._buttons["sdpPreviousBtn"]._onPressEvent = delegate
                {
                    DialogPopup(en, scriptName, byte.Parse(previous), img);
                };
                _standardDialogPopup._buttons["sdpPreviousBtn"].Enabled = true;
            }
            _standardDialogPopup._labels["sdpBodyLabel"].ChangeText(text5);
            switch (text10)
            {
                case "sellitem":
                    {
                        int num60 = 1;
                        int num61 = int.Parse(questvars["sellnumber"]);
                        InventoryItem itemSlot5 = getItemSlot(_standardDialogPopup._sellitem);
                        int num62 = itemSlot5._value / 2;
                        if (itemSlot5._maxAmount <= 1)
                        {
                            _inventory.Remove(itemSlot5);
                        }
                        else if (num61 >= itemSlot5._amount)
                        {
                            num60 = itemSlot5._amount;
                            _inventory.Remove(itemSlot5);
                        }
                        else
                        {
                            itemSlot5._amount -= num61;
                            num60 = num61;
                        }
                        getItemSlot(72)._amount += num62 * num60;
                        DialogPopup(en, scriptName, 0, img);
                        return;
                    }
                case "close":
                    _viewingDialog = false;
                    return;
                case "default":
                    DialogPopup(en, en._name, 0, img);
                    return;
            }
            break;
        }
        if (!flag7)
        {
            _viewingDialog = false;
            return;
        }
        if (text2 == "default")
        {
            _standardDialogPopup._background.Texture = _textureManager.Get("msgtop_F0_C0");
            Sprite sprite17 = new Sprite();
            sprite17.Texture = _textureManager.Get("msgbot_F0_C0");
            sprite17._windowOffset = new Vector(0.0, 117.0, 0.0);
            sprite17.SetPosition(sprite17._windowOffset.X + _standardDialogPopup._position.X, sprite17._windowOffset.Y + _standardDialogPopup._position.Y);
            _standardDialogPopup._backgrounds.Add("bot", sprite17);
            _standardDialogPopup._buttons["sdpNextBtn"].Position = new Vector(90.0 + _standardDialogPopup._position.X, 143.0 + _standardDialogPopup._position.Y, 0.0);
            _standardDialogPopup._buttons["sdpPreviousBtn"].Position = new Vector(20.0 + _standardDialogPopup._position.X, 143.0 + _standardDialogPopup._position.Y, 0.0);
            _standardDialogPopup._buttons["sdpQuitBtn"].Position = new Vector(364.0 + _standardDialogPopup._position.X, 143.0 + _standardDialogPopup._position.Y, 0.0);
            _standardDialogPopup._buttons["sdpNextBtn"]._windowOffset = new Vector(90.0, 143.0, 0.0);
            _standardDialogPopup._buttons["sdpPreviousBtn"]._windowOffset = new Vector(20.0, 143.0, 0.0);
            _standardDialogPopup._buttons["sdpQuitBtn"]._windowOffset = new Vector(364.0, 143.0, 0.0);
        }
        if (text.StartsWith("item"))
        {
            _standardDialogPopup._sprite._windowOffset = new Vector(45.0, 33.0, 0.0);
        }
        else
        {
            _standardDialogPopup._sprite._windowOffset = new Vector(70 - _standardDialogPopup._sprite.Texture.Width / 2, 8.0, 0.0);
        }
        _standardDialogPopup._sprite.SetPosition(_standardDialogPopup._sprite._windowOffset.X + _standardDialogPopup._position.X, _standardDialogPopup._sprite._windowOffset.Y + _standardDialogPopup._position.Y);
        _standardDialogPopup.Move(new Engine.Point(dDP.X, (float)((double)dDP.Y - num2)), resetposoff: true);
        _viewingDialog = true;
    }

    private void RepositionScroller(int index, int fullcount, int barHeight, ButtonMenu menu, Engine.Button btn, int btnX, int btnY)
    {
        double num = (double)index / (double)fullcount;
        double num2 = num * (double)barHeight;
        btn.Position = new Vector(menu._position.X + (double)btnX, menu._position.Y + (double)btnY + num2, 0.0);
        btn._windowOffset.Y = (double)btnY + num2;
    }

    private void RepositionHorizontalScroller(int index, int fullcount, int barWidth, ButtonMenu menu, Engine.Button btn, int btnX, int btnY)
    {
        double num = (double)index / (double)fullcount;
        double num2 = num * (double)barWidth;
        btn.Position = new Vector(menu._position.X + (double)btnX + num2, menu._position.Y + (double)btnY, 0.0);
        btn._windowOffset.X = (double)btnX + num2;
    }

    #region Skill / spell / action membership checks

    /// <summary>True if the player knows a skill with this name (case-insensitive).</summary>
    private bool HasSkill(string name)
    {
        foreach (Skill skill in _skills)
        {
            if (skill._name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>True if the player knows a spell with this exact name.</summary>
    private bool HasSpell(string name)
    {
        foreach (Spell spell in _spells)
        {
            if (spell._name == name)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>True if the player has an action with this exact name.</summary>
    private bool HasAction(string name)
    {
        foreach (Action action in _actions)
        {
            if (action._name == name)
            {
                return true;
            }
        }
        return false;
    }

    #endregion

    public void SystemMsg(string msg, byte type = 3)
    {
        string text = "";
        string text2 = "";
        if (type == 0)
        {
            text2 = "{=e";
            if (msg.Contains("\" "))
            {
                text = msg.Substring(0, msg.IndexOf("\" "));
                if (!_whisperList.Contains(text, StringComparer.CurrentCultureIgnoreCase))
                {
                    _whisperList.Add(text);
                    _whisperListIndex = _whisperList.Count();
                }
            }
        }
        if (type == 2)
        {
            text2 = "{=c";
        }
        if (type == 4)
        {
            text2 = "{=y";
        }
        if (type == 12)
        {
            text2 = "{=a";
        }
        _infoBarDelay = _origInfoBarDelay;
        _infoBarText = text2 + msg;
        _infoBarList.Add(text2 + _infoBarText);
        if (_infoBarList.Count == 9)
        {
            _infoMenu._buttons["infoScrollUpBtn"].Enabled = true;
            _infoMenu._buttons["infoScrollDownBtn"].Enabled = true;
            _infoMenu._buttons["infoScrollerBtn"].Hidden = false;
        }
        if (_infoBarList.Count > 8)
        {
            _infoBarIndex = _infoBarList.Count - 8;
        }
        _upperLeftMsgTimer = DateTime.UtcNow;
    }

    /// <summary>
    ///     Parses a chat line the local player typed: if it is a "name: /command" it dispatches the slash
    ///     command (/commands, /debug, /maps, /reset, /gender, /create, /spawn, /gm, /title, /script, /port,
    ///     /where, /m, …); otherwise the line is sent to the server as normal chat.
    /// </summary>
    /// <summary>
    ///     Handles a line the player typed, routing it by the channel prefix the chat box prepended:
    ///     "name: " = say, "name! " = shout, and "!"/"!!"/"!!!" = guild/group/world. A say that begins
    ///     with "/" is instead treated as a slash command and dispatched to the matching (mostly
    ///     debug/developer) handler. Regular chat is sent to the server, or shown locally when offline.
    /// </summary>
    private void ChatMsg(string msg)
    {
        byte b = 0;
        if (msg.StartsWith(_player._name + ": ", StringComparison.CurrentCultureIgnoreCase))
        {
            string message = msg.Remove(0, msg.IndexOf(": ") + 2);
            // === Slash commands (mostly debug / dev tools): /create, /spawn, /port, /reset, /gm, ... ===
            if (message.StartsWith("/"))
            {
                if (message.StartsWith("/commands"))
                {
                    _viewCommands = true;
                }
                else if (message.Equals("/debug"))
                {
                    if (debug)
                    {
                        GameWindow.ShowWindow(GameWindow._consoleHandle, 0);
                        debug = false;
                    }
                    else
                    {
                        GameWindow.ShowWindow(GameWindow._consoleHandle, 5);
                        debug = true;
                    }
                }
                else if (message.StartsWith("/maps"))
                {
                    StreamWriter streamWriter = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\mapnames.txt");
                    foreach (JToken item in from x in _mapsDB["maps"].Children()
                                            orderby x.Value<int>("mapnum")
                                            select x)
                    {
                        int num = item.Value<int>("mapnum");
                        string text2 = item.Value<string>("mapname");
                        streamWriter.WriteLine(num + " - " + text2);
                    }
                    streamWriter.Close();
                }
                else
                {
                    if (message.StartsWith("/map") || message.StartsWith("/wipe"))
                    {
                        return;
                    }
                    if (message.Equals("/reset"))
                    {
                        _player._lev = 1;
                        _player._exp = 1u;
                        _player._tnl = 599u;
                        _player._baseHP = 100;
                        _player._baseMP = 100;
                        _player._availstats = 0;
                        _player._str = 3;
                        _player._int = 3;
                        _player._wis = 3;
                        _player._con = 3;
                        _player._dex = 3;
                        _player._ac = 0;
                        SaveExp();
                        SaveLevel();
                        pdata.basehp = 100;
                        pdata.basemp = 100;
                        pdata.STR = 3;
                        pdata.INT = 3;
                        pdata.WIS = 3;
                        pdata.CON = 3;
                        pdata.DEX = 3;
                        pdata.ac = 0;
                        pdata.availstats = 0;
                        _statMenu._buttons["strBtn"].Enabled = false;
                        _statMenu._buttons["intBtn"].Enabled = false;
                        _statMenu._buttons["wisBtn"].Enabled = false;
                        _statMenu._buttons["conBtn"].Enabled = false;
                        _statMenu._buttons["dexBtn"].Enabled = false;
                        WritePlayerData();
                        return;
                    }
                    if (message.StartsWith("/gender"))
                    {
                        _player.GenderSwap();
                        SaveProfile();
                        SendDisplayPlayer();
                        return;
                    }
                    if (message.StartsWith("/spellani"))
                    {
                        try
                        {
                            _debugSpellAni = int.Parse(message.Remove(0, 9).Trim());
                            return;
                        }
                        catch
                        {
                            SystemMsg("Incorrect format, use: \"/spellani #\"", 3);
                            return;
                        }
                    }
                    if (message.StartsWith("/m"))
                    {
                        try
                        {
                            _debugMonSource = "old";
                            if (message.Contains("new"))
                            {
                                _debugMonSource = "new";
                                message = message.Remove(message.IndexOf("new"));
                            }
                            else if (message.Contains("myda"))
                            {
                                _debugMonSource = "myda";
                                message = message.Remove(message.IndexOf("myda"));
                            }
                            int debugMonImg = 1;
                            if (message.Length > 2)
                            {
                                debugMonImg = int.Parse(message.Remove(0, 2).Trim());
                            }
                            _debugMonImg = debugMonImg;
                            _player.MonsterForm(_debugMonImg, _debugMonSource);
                            return;
                        }
                        catch
                        {
                            SystemMsg("Incorrect format, use: \"/m # new\", new optional, spaces optional.", 3);
                            return;
                        }
                    }
                    if (message.StartsWith("/title"))
                    {
                        if (message.Length > 6)
                        {
                            _player._title = message.Substring(7);
                        }
                        else
                        {
                            _player._title = "";
                        }
                        SaveProfile();
                        return;
                    }
                    if (message.StartsWith("/where"))
                    {
                        string[] source = message.Split(' ');
                        string text3 = source.Last();
                        if (text3.EndsWith("?"))
                        {
                            text3 = text3.Remove(text3.Length);
                        }
                        RequestEntityMapInfoPacket requestEntityMapInfoPacket = new RequestEntityMapInfoPacket(text3);
                        GameWindow.ClientSocket.Send(requestEntityMapInfoPacket.Data);
                        return;
                    }
                    if (message.StartsWith("/port"))
                    {
                        string[] array = message.Split(' ');
                        int num2 = 0;
                        int num3 = 0;
                        if (int.TryParse(array[1], out var result))
                        {
                            try
                            {
                                if (message.Contains(','))
                                {
                                    string[] array2 = array[2].Split(',');
                                    num2 = int.Parse(array2[0]);
                                    num3 = int.Parse(array2[1]);
                                    NewMap(result, num2, num3);
                                    SystemMsg("Teleported to " + result + " - " + _map._name, 3);
                                }
                                else
                                {
                                    NewMap(result, 5, 5);
                                    SystemMsg("Teleported to " + result + " - " + _map._name, 3);
                                }
                                return;
                            }
                            catch
                            {
                                SystemMsg("Incorrect format, use: \"/port 701 5,11\", x,y optional.", 3);
                                return;
                            }
                        }
                        SystemMsg("Porting to player is currently not supported.", 3);
                        return;
                    }
                    if (message.StartsWith("/bloody"))
                    {
                        if (!_player._body._bloody)
                        {
                            _player._body._bloody = true;
                        }
                        else
                        {
                            _player._body._bloody = false;
                        }
                        SendDisplayPlayer();
                        return;
                    }
                    if (message.StartsWith("/create item"))
                    {
                        try
                        {
                            int result2 = 1;
                            string text4 = message.Remove(0, 13);
                            string text5 = "";
                            string text6 = "";
                            string text7 = "";
                            if (text4.Contains(' '))
                            {
                                string[] array3 = text4.Split(' ');
                                string[] array4 = array3;
                                foreach (string text8 in array4)
                                {
                                    if (text8.StartsWith("e:"))
                                    {
                                        text7 = char.ToUpper(text8[2]) + text8.Substring(3);
                                        text4 = text4.Remove(text4.IndexOf(text8) - 1, text8.Length + 1);
                                        break;
                                    }
                                }
                                array3 = text4.Split(' ');
                                text5 = array3.Last();
                                if (int.TryParse(text5, out result2))
                                {
                                    text4 = text4.Remove(text4.Length - text5.Length - 1);
                                    text6 = " (" + text5 + ")";
                                }
                            }
                            if (!NewItem(text4, 0, result2, 0, 0, text7))
                            {
                                SystemMsg("Item " + text4 + " - not found in database.", 3);
                            }
                            else
                            {
                                if (text7 != "")
                                {
                                    text7 += " ";
                                }
                                SystemMsg("Item " + text7 + text4 + text6 + " - was created.", 3);
                            }
                            return;
                        }
                        catch
                        {
                            SystemMsg("Incorrect format, use: \"/create item bat's wing\".", 3);
                            return;
                        }
                    }
                    if (message.StartsWith("/create skill"))
                    {
                        try
                        {
                            string text9 = message.Remove(0, 14);
                            if (!NewSkill(text9, 0, 0))
                            {
                                SystemMsg("Skill " + text9 + " - not found in database.", 3);
                            }
                            else
                            {
                                SystemMsg("Skill: " + text9 + " - was created.", 3);
                            }
                            return;
                        }
                        catch
                        {
                            SystemMsg("Incorrect format, use: \"/create skill assail\".", 3);
                            return;
                        }
                    }
                    if (message.StartsWith("/create spell"))
                    {
                        try
                        {
                            string text10 = message.Remove(0, 14);
                            if (!NewSpell(text10, 0, 0))
                            {
                                SystemMsg("Spell: " + text10 + " - not found in database.", 3);
                            }
                            else
                            {
                                SystemMsg("Spell: " + text10 + " - was created.", 3);
                            }
                            return;
                        }
                        catch
                        {
                            SystemMsg("Incorrect format, use: \"/create spell ioc\".", 3);
                            return;
                        }
                    }
                    if (message.StartsWith("/create action"))
                    {
                        try
                        {
                            string text11 = message.Remove(0, 15);
                            if (!NewAction(text11, 0, 0))
                            {
                                SystemMsg("Action: " + text11 + " - not found in database.", 3);
                            }
                            else
                            {
                                SystemMsg("Action: " + text11 + " - was created.", 3);
                            }
                            return;
                        }
                        catch
                        {
                            SystemMsg("Incorrect format, use: \"/create action dive\".", 3);
                            return;
                        }
                    }
                    if (message.StartsWith("/spawn npc new"))
                    {
                        string[] source2 = message.Split();
                        if (int.TryParse(source2.Last(), out var result3))
                        {
                            SpawnNpc(result3, new Location(_player._location.X, _player._location.Y), useNew: true);
                        }
                    }
                    else if (message.StartsWith("/spawn npc"))
                    {
                        string[] source3 = message.Split();
                        if (int.TryParse(source3.Last(), out var result4))
                        {
                            SpawnNpc(result4, new Location(_player._location.X, _player._location.Y));
                        }
                    }
                    else if (message.StartsWith("/spawn monster new"))
                    {
                        string[] source4 = message.Split();
                        SpawnMonster(source4.Last(), new Location(_player._location.X, _player._location.Y), "new", 1);
                    }
                    else if (message.StartsWith("/spawn monster myda"))
                    {
                        string[] source5 = message.Split();
                        SpawnMonster(source5.Last(), new Location(_player._location.X, _player._location.Y), "myda", 1);
                    }
                    else if (message.StartsWith("/spawn monster"))
                    {
                        string[] source6 = message.Split();
                        SpawnMonster(source6.Last(), new Location(_player._location.X, _player._location.Y), "old", 1);
                    }
                    else if (message.StartsWith("/spawn item"))
                    {
                        SpawnItem(message.Remove(0, 12), new Location(_player._location.X, _player._location.Y));
                    }
                    else if (message.StartsWith("/script"))
                    {
                        Scripts(message.Remove(0, 8));
                    }
                    else
                    {
                        if (!message.StartsWith("/gm"))
                        {
                            return;
                        }
                        if (!_GM)
                        {
                            _GM = true;
                            foreach (Skill skill in _skills)
                            {
                                skill._highlight = false;
                            }
                            foreach (Spell spell in _spells)
                            {
                                spell._highlight = false;
                            }
                            {
                                foreach (Action action in _actions)
                                {
                                    action._highlight = false;
                                }
                                return;
                            }
                        }
                        _GM = false;
                    }
                }
            }
            else if (GameWindow.ConnectedToServer)
            {
                MessagePacket messagePacket = new MessagePacket(b, _player._id, msg);
                GameWindow.ClientSocket.Send(messagePacket.Data);
            }
            else
            {
                DisplayChat(b, _player._id, msg);
            }
        }
        // === Shout channel ("name! message") ===
        else if (msg.StartsWith(_player._name + "! ", StringComparison.CurrentCultureIgnoreCase))
        {
            b = 1;
            if (GameWindow.ConnectedToServer)
            {
                MessagePacket messagePacket2 = new MessagePacket(b, _player._id, msg);
                GameWindow.ClientSocket.Send(messagePacket2.Data);
            }
            else
            {
                DisplayChat(b, _player._id, msg);
            }
        }
        else
        {
            // === Guild (!), group (!!), world (!!!) channels ===
            string channelPrefix = msg.Substring(3, msg.IndexOf(": ") - 3);
            string channelBody = msg.Substring(msg.IndexOf(": ") + 2);
            switch (channelPrefix)
            {
                case "!":
                    b = 12;
                    msg = "<!" + _player._name + "> " + channelBody;
                    break;
                case "!!":
                    b = 4;
                    msg = "[!" + _player._name + "] " + channelBody;
                    break;
                case "!!!":
                    b = 2;
                    msg = "[" + _player._name + "]: " + channelBody;
                    break;
            }
            if (GameWindow.ConnectedToServer)
            {
                SysMessagePacket sysMessagePacket = new SysMessagePacket(b, msg);
                GameWindow.ClientSocket.Send(sysMessagePacket.Data);
            }
            else
            {
                DisplayChat(b, 0u, msg);
            }
        }
    }

    private void PlayMusicFile(string url)
    {
        if (_soundManager.sengine != null)
        {
            string text = url.Remove(url.IndexOf(".ogg"));
            _debugMusic = int.Parse(text.Remove(0, 6));
            if (curTrack != null)
            {
                curTrack.Stop();
            }
            curTrack = _soundManager.sengine.Play2D(url, playLooped: true, startPaused: true);
            MusicVolume();
            curTrack.Paused = false;
        }
    }

    private void PlaySound(string soundId, bool loop = false)
    {
        if (_soundManager.sengine != null)
        {
            float volume = (float)_soundNodeIndex / 100f;
            ISound sound = _soundManager.sengine.Play2D(soundId, loop, startPaused: true);
            sound.Volume = volume;
            sound.Paused = false;
        }
    }

    private void MusicVolume()
    {
        if (_soundManager.sengine != null)
        {
            float volume = (float)_musicNodeIndex / 100f;
            if (curTrack != null)
            {
                curTrack.Volume = volume;
            }
        }
    }

    private void SoundVolume()
    {
        if (_soundManager.sengine == null)
        {
            return;
        }
        float defaultVolume = (float)_soundNodeIndex / 100f;
        foreach (ISoundSource value in _soundManager._soundIdentifier.Values)
        {
            value.DefaultVolume = defaultVolume;
        }
    }

    /// <summary>Handles a group invite from <paramref name="name"/>: shows the in-game prompt, or (when the prompt window is off) auto-accepts while connected.</summary>
    public void GroupRequest(string name, byte type)
    {
        if (_useGroupWindow)
        {
            _groupRequestPrompt._labels["promptText"].ChangeText(name + " invites you to join a group.");
            _showGroupRequestPrompt = true;
        }
        else if (GameWindow.ConnectedToServer)
        {
            GroupRequestPacket groupRequestPacket = new GroupRequestPacket(1, name);
            GameWindow.ClientSocket.Send(groupRequestPacket.Data);
        }
    }

    /// <summary>Spawns a projectile from entity <paramref name="id"/>, with damage chosen by projectile <paramref name="type"/>.</summary>
    public void Projectile(uint id, byte type)
    {
        int dmg = 0;
        switch (type)
        {
            case 13:
                dmg = 0;
                break;
            case 9:
                dmg = 5000;
                break;
            case 12:
                dmg = 5000;
                break;
        }
        if (_map._entities.ContainsKey(id))
        {
            new Projectile(_textureManager, id, type, _map._entities[id]._body._direction, _map._entities[id]._tile, dmg);
        }
    }

    /// <summary>Reports a located player's map and coordinates (the /where command response).</summary>
    public void PortToPlayer(ushort num, byte x, byte y)
    {
        SystemMsg("Player is on map " + num + " at " + x + "," + y + ".", 3);
    }

    /// <summary>Handles a received system message: shows it (except types 8/10) and, for chat types (0/2/4/12), prepends a color code and adds it to the chat log.</summary>
    public void SystemMessageP(byte type, string msg)
    {
        if (type != 8 && type != 10)
        {
            SystemMsg(msg, type);
        }
        if (type == 0 || type == 2 || type == 4 || type == 12)
        {
            string colorCode = "{=e";
            switch (type)
            {
                case 2:
                    colorCode = "{=c";
                    break;
                case 4:
                    colorCode = "{=y";
                    break;
                case 12:
                    colorCode = "{=a";
                    break;
            }
            _chatList.Add(colorCode + msg);
            if (_chatList.Count == 9)
            {
                _chatMenu._buttons["chatScrollUpBtn"].Enabled = true;
                _chatMenu._buttons["chatScrollDownBtn"].Enabled = true;
                _chatMenu._buttons["chatScrollerBtn"].Hidden = false;
            }
            if (_chatList.Count > 8)
            {
                _chatIndex = _chatList.Count - 8;
            }
        }
    }

    /// <summary>Builds and sends the local player's profile packet — name/nation/title/rank/level/guild + equipment + legend marks — tracking the running packet byte length.</summary>
    public void SendProfileData()
    {
        DisplayProfileS profile = default(DisplayProfileS);
        ushort packetLen = 0;
        profile.ID = _player._id;
        packetLen += 4;
        profile.Name = _player._name;
        packetLen++;
        packetLen += (ushort)_player._name.Length;
        profile.Nation = _player._nation;
        packetLen++;
        profile.Title = _player._title;
        packetLen++;
        packetLen += (ushort)_player._title.Length;
        profile.AllowGroup = Convert.ToByte(_allowGrouping);
        packetLen++;
        profile.GroupList = new List<GroupMemberS>();
        packetLen++;
        profile.Rank = _player._rank;
        packetLen++;
        packetLen += (ushort)_player._rank.Length;
        if (_player._master)
        {
            profile.Level = "Master";
            packetLen++;
            packetLen += 6;
        }
        else
        {
            profile.Level = _player._lev.ToString();
            packetLen++;
            packetLen += (ushort)_player._lev.ToString().Length;
        }
        profile.Guild = _player._guild;
        packetLen++;
        packetLen += (ushort)_player._guild.Length;
        profile.Equip = new List<EquippedS>();
        packetLen += 2;
        foreach (InventoryItem equipItem in _equipment)
        {
            EquippedS equippedEntry = default(EquippedS);
            equippedEntry.Slot = (byte)equipItem._slot;
            packetLen++;
            equippedEntry.Color = (byte)equipItem._bodyImgColor;
            packetLen++;
            equippedEntry.Name = equipItem._name;
            packetLen++;
            packetLen += (ushort)equipItem._name.Length;
            equippedEntry.Dura = (uint)equipItem._durability;
            packetLen += 4;
            profile.Equip.Add(equippedEntry);
        }
        profile.Legend = new List<LegendMarkS>();
        packetLen += 2;
        foreach (LegendMark legendMark in _player._legendMarks)
        {
            LegendMarkS legendEntry = default(LegendMarkS);
            legendEntry.Color = (byte)legendMark._color;
            packetLen++;
            legendEntry.Icon = (byte)legendMark._icon;
            packetLen++;
            legendEntry.ID = legendMark._id;
            packetLen++;
            packetLen += (ushort)legendMark._id.Length;
            legendEntry.Text = legendMark._text;
            packetLen++;
            packetLen += (ushort)legendMark._text.Length;
            profile.Legend.Add(legendEntry);
        }
        DisplayProfilePacket displayProfilePacket = new DisplayProfilePacket(profile, packetLen);
        GameWindow.ClientSocket.Send(displayProfilePacket.Data);
    }

    /// <summary>
    ///     Applies a received profile packet: for the local player, updates group state + grouping UI;
    ///     for another player, sets nation/title/rank/level/guild, rebuilds their equipment (looked up
    ///     in the items DB) and legend marks, and selects them as the viewed profile.
    /// </summary>
    public void DisplayProfile(DisplayProfileS profile)
    {
        uint id = profile.ID;
        if (id == _player._id)
        {
            GroupList = profile.GroupList;
            if (GroupList.Count() > 0)
            {
                _profileMenu._buttons["groupProfileMenuBtn"]._baseImage = _textureManager.Get("equip05_F2_C0");
                _profileMenu._buttons["groupProfileMenuBtn"]._clickedImage = _textureManager.Get("equip05_F3_C0");
                _profileMenu._buttons["groupProfileMenuBtn"].Sprite.Texture = _profileMenu._buttons["groupProfileMenuBtn"]._baseImage;
                _allowGrouping = true;
                SaveClientSettings();
                _settingsMenu._labels["settings5Lab"].ChangeText(_settingsMenu._labels["settings5Lab"]._text.Remove(_settingsMenu._labels["settings5Lab"]._text.IndexOf(" : ") + 3) + "ON");
            }
        }
        else
        {
            if (!Player.List.ContainsKey(id))
            {
                return;
            }
            Player.List[id]._nation = profile.Nation;
            Player.List[id]._title = profile.Title;
            Player.List[id]._allowGrouping = Convert.ToBoolean(profile.AllowGroup);
            if (Player.List[id]._allowGrouping)
            {
                _othersProfileMenu._buttons["othersgroupProfileMenuBtn"]._baseImage = _textureManager.Get("equip05_F2_C0");
                _othersProfileMenu._buttons["othersgroupProfileMenuBtn"]._clickedImage = _textureManager.Get("equip05_F3_C0");
                _othersProfileMenu._buttons["othersgroupProfileMenuBtn"].Sprite.Texture = _othersProfileMenu._buttons["othersgroupProfileMenuBtn"]._baseImage;
            }
            else
            {
                _othersProfileMenu._buttons["othersgroupProfileMenuBtn"]._baseImage = _textureManager.Get("equip05_F0_C0");
                _othersProfileMenu._buttons["othersgroupProfileMenuBtn"]._clickedImage = _textureManager.Get("equip05_F1_C0");
                _othersProfileMenu._buttons["othersgroupProfileMenuBtn"].Sprite.Texture = _othersProfileMenu._buttons["othersgroupProfileMenuBtn"]._baseImage;
            }
            Player.List[id]._rank = profile.Rank;
            if (profile.Level == "Master")
            {
                Player.List[id]._lev = 99;
                Player.List[id]._master = true;
            }
            else
            {
                Player.List[id]._lev = byte.Parse(profile.Level);
            }
            Player.List[id]._guild = profile.Guild;
            Player.List[id]._equipment = new List<Equipment>();
            foreach (EquippedS equipped in profile.Equip)
            {
                foreach (JToken itemEntry in _itemsDB["items"].Children())
                {
                    string dbName = itemEntry.Value<string>("name");
                    if (dbName.Equals(equipped.Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        int imageFolder = itemEntry.Value<int>("imagefolder");
                        int frame = itemEntry.Value<int>("frame");
                        string imageType = itemEntry.Value<string>("imagetype");
                        if (string.IsNullOrEmpty(imageType))
                        {
                            imageType = "";
                        }
                        Equipment equipment = new Equipment(_textureManager, equipped.Name, equipped.Slot, equipped.Color, imageType, imageFolder, frame);
                        int boundFlag = itemEntry.Value<int>("bound");
                        if (boundFlag == 1)
                        {
                            equipment._bound = true;
                        }
                        equipment._maxAmount = itemEntry.Value<int>("stack");
                        equipment._tab = itemEntry.Value<string>("tab");
                        equipment._value = itemEntry.Value<int>("value");
                        equipment._description = itemEntry.Value<string>("desc");
                        equipment._maxDurability = itemEntry.Value<int>("dura");
                        equipment._durability = (int)equipped.Dura;
                        equipment._dyeable = itemEntry.Value<int>("dyeable");
                        equipment._bodyImg = itemEntry.Value<int>("bodyimg");
                        JToken statsNode = itemEntry["stats"];
                        if (statsNode != null)
                        {
                            equipment._level = statsNode.Value<string>("lev");
                            equipment._gender = statsNode.Value<string>("gender");
                            equipment._weaponDmg = statsNode.Value<string>("w");
                            equipment._atk = statsNode.Value<string>("atk");
                            equipment._def = statsNode.Value<string>("def");
                            equipment._hp = statsNode.Value<int>("hp");
                            equipment._mp = statsNode.Value<int>("mp");
                            equipment._str = statsNode.Value<short>("str");
                            equipment._int = statsNode.Value<short>("int");
                            equipment._wis = statsNode.Value<short>("wis");
                            equipment._con = statsNode.Value<short>("con");
                            equipment._dex = statsNode.Value<short>("dex");
                            equipment._mr = statsNode.Value<sbyte>("mr");
                            equipment._ac = statsNode.Value<sbyte>("ac");
                            equipment._dmg = statsNode.Value<sbyte>("dmg");
                            equipment._hit = statsNode.Value<sbyte>("hit");
                            equipment._reg = statsNode.Value<sbyte>("reg");
                        }
                        equipment._durability = equipment._maxDurability;
                        Player.List[id]._equipment.Add(equipment);
                        break;
                    }
                }
            }
            Player.List[id]._legendMarks = new List<LegendMark>();
            foreach (LegendMarkS legendData in profile.Legend)
            {
                LegendMark legendMark = new LegendMark(_textureManager, _font, legendData.Icon, legendData.ID, legendData.Text, legendData.Color);
                Player.List[id]._legendMarks.Add(legendMark);
            }
            _profilePlayer = Player.List[id];
        }
    }

    /// <summary>Plays a spell's cast animation on the caster (FromID) and its impact animation on the target (ToID), each only if in view.</summary>
    public void SpellAnimationP(SpellAniS spellAni)
    {
        try
        {
            uint toID = spellAni.ToID;
            if (toID != 0)
            {
                uint fromID = spellAni.FromID;
                if (_map._entities.ContainsKey(fromID))
                {
                    Entity fromEntity = _map._entities[fromID];
                    fromEntity._body.SpellAni(spellAni.FromAni, spellAni.Delay);
                }
                if (_map._entities.ContainsKey(toID))
                {
                    Entity toEntity = _map._entities[toID];
                    toEntity._body.SpellAni(spellAni.ToAni, spellAni.Delay);
                }
            }
        }
        catch
        {
        }
    }

    /// <summary>Plays an emote or attack animation on player <paramref name="id"/> (types 3/4 = attack, else emote); ignored if they are already emoting/attacking.</summary>
    public void EmoteP(uint id, byte aniType, ushort aniDelay)
    {
        if (!Player.List.ContainsKey(id) || Player.List[id]._body._emoting || Player.List[id]._body._attacking)
        {
            return;
        }
        getAniInfo(aniType, out var bodyAniType, out var aniStartFrame, out var aniEndFrame);
        if (aniType == 3 || aniType == 4)
        {
            bool repeatOnce = false;
            if (aniType == 4)
            {
                repeatOnce = true;
            }
            Player.List[id]._body.SetAni(bodyAniType, aniDelay, aniStartFrame, aniEndFrame, repeatOnce);
            Player.List[id]._body._attacking = true;
        }
        else
        {
            Player.List[id]._body.Emote(aniDelay, aniStartFrame, aniEndFrame);
            Player.List[id]._body._emoting = true;
        }
    }

    /// <summary>Plays a body/attack animation on player <paramref name="id"/> (unless already attacking).</summary>
    public void BodyMoveP(uint id, byte aniType, ushort aniDelay)
    {
        if (Player.List.ContainsKey(id) && !Player.List[id]._body._attacking)
        {
            getAniInfo(aniType, out var bodyAniType, out var aniStartFrame, out var aniEndFrame);
            Player.List[id]._body.SetAni(bodyAniType, aniDelay, aniStartFrame, aniEndFrame);
            Player.List[id]._body._attacking = true;
        }
    }

    /// <summary>
    ///     Maps an animation-type code to its body-animation sprite set (<paramref name="bodyAniType"/>) and
    ///     frame range (<paramref name="aniStartFrame"/>..<paramref name="aniEndFrame"/>). Hand-authored lookup
    ///     table; defaults to the idle set ("02", 0..4) for unknown types.
    /// </summary>
    private void getAniInfo(byte aniType, out string bodyAniType, out int aniStartFrame, out int aniEndFrame)
    {
        bodyAniType = "02";
        aniStartFrame = 0;
        aniEndFrame = 4;
        switch (aniType)
        {
            case 2:
                bodyAniType = "03";
                aniEndFrame = 2;
                break;
            case 3:
                bodyAniType = "03";
                aniStartFrame = 2;
                aniEndFrame = 6;
                break;
            case 4:
                bodyAniType = "03";
                aniStartFrame = 6;
                aniEndFrame = 10;
                break;
            case 5:
                bodyAniType = "b";
                aniEndFrame = 10;
                break;
            case 6:
                bodyAniType = "b";
                aniStartFrame = 10;
                aniEndFrame = 24;
                break;
            case 7:
                bodyAniType = "b";
                aniStartFrame = 24;
                aniEndFrame = 32;
                break;
            case 8:
                bodyAniType = "b";
                aniStartFrame = 32;
                aniEndFrame = 42;
                break;
            case 9:
                bodyAniType = "b";
                aniStartFrame = 42;
                aniEndFrame = 50;
                break;
            case 10:
                bodyAniType = "b";
                aniStartFrame = 50;
                aniEndFrame = 54;
                break;
            case 11:
                bodyAniType = "b";
                aniStartFrame = 54;
                aniEndFrame = 60;
                break;
            case 12:
                bodyAniType = "b";
                aniStartFrame = 60;
                aniEndFrame = 66;
                break;
            case 13:
                bodyAniType = "b";
                aniStartFrame = 66;
                aniEndFrame = 68;
                break;
            case 14:
                bodyAniType = "c";
                aniEndFrame = 8;
                break;
            case 15:
                bodyAniType = "c";
                aniStartFrame = 8;
                aniEndFrame = 14;
                break;
            case 16:
                bodyAniType = "c";
                aniStartFrame = 14;
                aniEndFrame = 22;
                break;
            case 17:
                bodyAniType = "c";
                aniStartFrame = 22;
                aniEndFrame = 38;
                break;
            case 18:
                bodyAniType = "c";
                aniStartFrame = 38;
                aniEndFrame = 42;
                break;
            case 19:
                bodyAniType = "c";
                aniStartFrame = 42;
                aniEndFrame = 48;
                break;
            case 20:
                bodyAniType = "c";
                aniStartFrame = 48;
                aniEndFrame = 54;
                break;
            case 21:
                bodyAniType = "c";
                aniStartFrame = 54;
                aniEndFrame = 58;
                break;
            case 22:
                bodyAniType = "c";
                aniStartFrame = 58;
                aniEndFrame = 64;
                break;
            case 23:
                bodyAniType = "c";
                aniStartFrame = 64;
                aniEndFrame = 70;
                break;
            case 24:
                bodyAniType = "d";
                aniStartFrame = 0;
                aniEndFrame = 4;
                break;
            case 25:
                bodyAniType = "d";
                aniStartFrame = 4;
                aniEndFrame = 8;
                break;
            case 26:
                bodyAniType = "d";
                aniStartFrame = 8;
                aniEndFrame = 12;
                break;
            case 27:
                bodyAniType = "d";
                aniStartFrame = 12;
                aniEndFrame = 18;
                break;
            case 28:
                bodyAniType = "d";
                aniStartFrame = 18;
                aniEndFrame = 22;
                break;
            case 29:
                bodyAniType = "d";
                aniStartFrame = 22;
                aniEndFrame = 28;
                break;
            case 30:
                bodyAniType = "d";
                aniStartFrame = 28;
                aniEndFrame = 34;
                break;
            case 31:
                bodyAniType = "d";
                aniStartFrame = 34;
                aniEndFrame = 38;
                break;
            case 32:
                bodyAniType = "d";
                aniStartFrame = 38;
                aniEndFrame = 44;
                break;
            case 33:
                bodyAniType = "d";
                aniStartFrame = 44;
                aniEndFrame = 48;
                break;
            case 34:
                bodyAniType = "d";
                aniStartFrame = 48;
                aniEndFrame = 54;
                break;
            case 35:
                bodyAniType = "e";
                aniStartFrame = 0;
                aniEndFrame = 4;
                break;
            case 36:
                bodyAniType = "e";
                aniStartFrame = 4;
                aniEndFrame = 8;
                break;
            case 37:
                bodyAniType = "e";
                aniStartFrame = 8;
                aniEndFrame = 12;
                break;
            case 38:
                bodyAniType = "e";
                aniStartFrame = 12;
                aniEndFrame = 16;
                break;
            case 39:
                bodyAniType = "e";
                aniStartFrame = 16;
                aniEndFrame = 20;
                break;
            case 40:
                bodyAniType = "e";
                aniStartFrame = 20;
                aniEndFrame = 26;
                break;
            case 41:
                bodyAniType = "e";
                aniStartFrame = 26;
                aniEndFrame = 32;
                break;
            case 42:
                bodyAniType = "e";
                aniStartFrame = 32;
                aniEndFrame = 40;
                break;
            case 43:
                bodyAniType = "e";
                aniStartFrame = 40;
                aniEndFrame = 46;
                break;
            case 44:
                bodyAniType = "e";
                aniStartFrame = 46;
                aniEndFrame = 54;
                break;
            case 45:
                bodyAniType = "e";
                aniStartFrame = 54;
                aniEndFrame = 62;
                break;
            case 46:
                bodyAniType = "e";
                aniStartFrame = 62;
                aniEndFrame = 74;
                break;
            case 47:
                bodyAniType = "e";
                aniStartFrame = 74;
                aniEndFrame = 82;
                break;
            case 48:
                bodyAniType = "f";
                aniStartFrame = 0;
                aniEndFrame = 4;
                break;
            case 49:
                bodyAniType = "f";
                aniStartFrame = 0;
                aniEndFrame = 8;
                break;
            case 50:
                bodyAniType = "f";
                aniStartFrame = 8;
                aniEndFrame = 26;
                break;
            case 51:
                bodyAniType = "f";
                aniStartFrame = 26;
                aniEndFrame = 36;
                break;
            case 52:
                bodyAniType = "f";
                aniStartFrame = 36;
                aniEndFrame = 42;
                break;
            case 53:
                bodyAniType = "f";
                aniStartFrame = 42;
                aniEndFrame = 54;
                break;
            case 54:
                bodyAniType = "f";
                aniStartFrame = 59;
                aniEndFrame = 66;
                break;
            case 55:
                aniStartFrame = 0;
                aniEndFrame = 1;
                break;
            case 56:
                aniStartFrame = 1;
                aniEndFrame = 2;
                break;
            case 57:
                aniStartFrame = 2;
                aniEndFrame = 3;
                break;
            case 58:
                aniStartFrame = 3;
                aniEndFrame = 4;
                break;
            case 59:
                aniStartFrame = 4;
                aniEndFrame = 5;
                break;
            case 60:
                aniStartFrame = 5;
                aniEndFrame = 6;
                break;
            case 61:
                aniStartFrame = 6;
                aniEndFrame = 7;
                break;
            case 62:
                aniStartFrame = 7;
                aniEndFrame = 9;
                break;
            case 63:
                aniStartFrame = 9;
                aniEndFrame = 11;
                break;
            case 64:
                aniStartFrame = 11;
                aniEndFrame = 12;
                break;
            case 65:
                aniStartFrame = 12;
                aniEndFrame = 13;
                break;
            case 66:
                aniStartFrame = 13;
                aniEndFrame = 14;
                break;
            case 67:
                aniStartFrame = 14;
                aniEndFrame = 15;
                break;
            case 68:
                aniStartFrame = 15;
                aniEndFrame = 18;
                break;
            case 69:
                aniStartFrame = 18;
                aniEndFrame = 19;
                break;
            case 70:
                aniStartFrame = 19;
                aniEndFrame = 20;
                break;
            case 71:
                aniStartFrame = 20;
                aniEndFrame = 21;
                break;
            case 72:
                aniStartFrame = 21;
                aniEndFrame = 22;
                break;
            case 73:
                aniStartFrame = 22;
                aniEndFrame = 23;
                break;
            case 74:
                aniStartFrame = 23;
                aniEndFrame = 24;
                break;
            case 75:
                aniStartFrame = 24;
                aniEndFrame = 25;
                break;
            case 76:
                aniStartFrame = 25;
                aniEndFrame = 26;
                break;
            case 77:
                aniStartFrame = 26;
                aniEndFrame = 27;
                break;
            case 78:
                aniStartFrame = 27;
                aniEndFrame = 28;
                break;
            case 79:
                aniStartFrame = 28;
                aniEndFrame = 29;
                break;
            case 80:
                aniStartFrame = 29;
                aniEndFrame = 30;
                break;
            case 81:
                aniStartFrame = 30;
                aniEndFrame = 31;
                break;
            case 82:
                aniStartFrame = 31;
                aniEndFrame = 32;
                break;
            case 83:
                aniStartFrame = 32;
                aniEndFrame = 35;
                break;
            case 84:
                aniStartFrame = 35;
                aniEndFrame = 38;
                break;
            case 85:
                aniStartFrame = 38;
                aniEndFrame = 42;
                break;
        }
    }

    /// <summary>Removes entity/player <paramref name="id"/> from the map, its tile, and the player list (server says they left view).</summary>
    public void RemoveEntityP(uint id)
    {
        if (Player.List.ContainsKey(id))
        {
            if (Player.List[id]._tile._entities.ContainsKey(id))
            {
                Player.List[id]._tile._entities.Remove(id);
            }
            Player.List[id]._map._entities.Remove(id);
            Player.List.Remove(id);
        }
    }

    public void Location(uint id, Location location, byte direction)
    {
        try
        {
            if (Player.List.ContainsKey(id))
            {
                Tile tile = Player.List[id]._tile;
                if (tile._entities.ContainsKey(id))
                {
                    tile._entities.Remove(id);
                }
                Player.List[id]._location = location;
                Player.List[id]._body._direction = direction;
                Player.List[id]._body.setDefault();
                Tile tile2 = _map._tiles[location.Y * (int)_map._width + location.X];
                tile2._entities.Add(id, Player.List[id]);
                Player.List[id]._tileTime = DateTime.UtcNow;
                Player.List[id]._tile = tile2;
                if (Player.List[id]._tile._water)
                {
                    Player.List[id]._body._swimming = true;
                }
                else
                {
                    Player.List[id]._body._swimming = false;
                }
                Vector vector = new Vector(tile2._position.X + tile2._width / 2.0 - 28.0 + 1.0, tile2._position.Y + tile2._height - 85.0 + 1.0, 0.0);
                Player.List[id].SetPosition(vector.X, vector.Y);
            }
        }
        catch
        {
        }
    }

    public void CharacterTurn(uint id, byte direction)
    {
        if (_map._entities.ContainsKey(id))
        {
            _map._entities[id]._body._direction = direction;
            _map._entities[id]._body.setDefault();
        }
    }

    /// <summary>
    ///     Rebuilds another player's display entity from a display-player packet: removes any old
    ///     instance, constructs the Player with all body/equipment sprites + sources, handles
    ///     swimming/ghost body states, and positions it.
    /// </summary>
    public void UpdatePlayerDisplay(DisplayPlayerS player)
    {
        uint id = player.ID;
        byte gender = player.Gender;
        if (Player.List.ContainsKey(id))
        {
            if (Player.List[id]._tile._entities.ContainsKey(id))
            {
                Player.List[id]._tile._entities.Remove(id);
            }
            if (Player.List[id]._map._entities.ContainsKey(id))
            {
                Player.List[id]._map._entities.Remove(id);
            }
            Player.List.Remove(id);
        }
        Location location = new Location(player.X, player.Y);
        Tile tile = _map._tiles[location.Y * (int)_map._width + location.X];
        if (_map._entities.ContainsKey(id))
        {
            _map._entities.Remove(id);
        }
        if (tile._entities.ContainsKey(id))
        {
            tile._entities.Remove(id);
        }
        Player newPlayer = new Player(this, _textureManager, _font, player.Name, location, _map, "", gender, id);
        newPlayer._body._direction = player.Direction;
        newPlayer._body._hairType = player.Head;
        newPlayer._body._hairColor = player.HeadColor;
        newPlayer._body._bodyImgs["b"] = player.Body;
        newPlayer._body._bodyImgs["a"] = player.Arms;
        newPlayer._body._bodyColors["a"] = player.ArmorColor;
        newPlayer._body._bodyImgs["n"] = 1;
        newPlayer._body._bodyColors["u"] = player.ArmorColor;
        newPlayer._body._bodyImgs["u"] = player.Armor;
        newPlayer._body._bodySource["u"] = ((player.ArmorSource == 2) ? "new" : ((player.ArmorSource == 1) ? "myda" : "old"));
        newPlayer._body._bodyImgs["h"] = player.Head;
        newPlayer._body._bodyColors["h"] = player.HeadColor;
        newPlayer._body._bodySource["h"] = ((player.HeadSource == 2) ? "new" : ((player.HeadSource == 1) ? "myda" : "old"));
        newPlayer._body._bodyImgs["l"] = player.Boots;
        newPlayer._body._bodyColors["l"] = player.BootColor;
        newPlayer._body._bodySource["l"] = ((player.BootSource == 2) ? "new" : ((player.BootSource == 1) ? "myda" : "old"));
        newPlayer._body._bodyImgs["s"] = player.Shield;
        newPlayer._body._bodySource["s"] = ((player.ShieldSource == 2) ? "new" : ((player.ShieldSource == 1) ? "myda" : "old"));
        newPlayer._body._bodyImgs["w"] = player.Weapon;
        newPlayer._body._bodySource["w"] = ((player.WeaponSource == 2) ? "new" : ((player.WeaponSource == 1) ? "myda" : "old"));
        newPlayer._body._bodyImgs["c"] = player.Acc;
        newPlayer._body._bodyColors["c"] = player.AccColor;
        newPlayer.Hidden = player.Hidden;
        if (newPlayer._body._bodyImgs["b"] == 5 || newPlayer._body._bodyImgs["b"] == 6)
        {
            newPlayer._body._swimming = true;
        }
        if (newPlayer._body._bodyImgs["b"] == 2)
        {
            newPlayer.Ghost = true;
        }
        tile = newPlayer._tile;
        Vector vector = new Vector(tile._position.X + tile._width / 2.0 - 28.0 + 1.0, tile._position.Y + tile._height - 85.0 + 1.0, 0.0);
        newPlayer.SetPosition(vector.X, vector.Y);
    }

    /// <summary>
    ///     Rebuilds the map's NPCs from a received entity list: clears existing NPCs, then for each
    ///     NPC entity constructs it and loads its dialogue speech-triggers, sell list, and learnable
    ///     skills/spells/actions from the NPC/dialogue DBs.
    /// </summary>
    public void UpdateEntitiesDisplay(List<DisplayEntityS> entities)
    {
        NPC[] existingNpcs = NPC.List.Values.ToArray();
        foreach (NPC staleNpc in existingNpcs)
        {
            if (staleNpc != null)
            {
                staleNpc._tile._entities.Remove(staleNpc._id);
                staleNpc._map._entities.Remove(staleNpc._id);
                NPC.List.Remove(staleNpc._id);
            }
        }
        foreach (DisplayEntityS entity in entities)
        {
            Location location = new Location(entity.X, entity.Y);
            Tile tile = _map._tiles[location.Y * (int)_map._width + location.X];
            if (entity.EntityType == EntityType.Item || entity.EntityType == EntityType.Monster || entity.EntityType != 0 || NPC.List.ContainsKey(entity.ID))
            {
                continue;
            }
            string source = "old";
            if (entity.Source == 1)
            {
                source = "myda";
            }
            else if (entity.Source == 2)
            {
                source = "new";
            }
            TextureManager textureManager = _textureManager;
            Engine.Font font = _font;
            string name = entity.Name;
            Map map = _map;
            ushort image = entity.Image;
            NPC npc = new NPC(this, textureManager, font, name, location, map, image.ToString("000"), (int)entity.Direction, source, entity.ID);
            npc.SetPosition(new Vector(tile._position.X + tile._width / 2.0 - Math.Abs(npc._mBody._sprite.GetWidth()) / 2.0 + 1.0, tile._position.Y + tile._height - npc._mBody._sprite.GetHeight() + 1.0, 0.0));
            foreach (JToken npcEntry in _npcsDB["npc"].Children())
            {
                string npcName = npcEntry.Value<string>("name");
                if (!npcName.Equals(entity.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }
                bool assailPush = npcEntry.Value<bool>("assailpush");
                npc._assailPush = assailPush;
                foreach (JToken dialogueEntry in _dialogDB["dialogue"].Children())
                {
                    string dialogueName = dialogueEntry.Value<string>("name");
                    if (!dialogueName.Equals(npcName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }
                    for (int j = 0; j < dialogueEntry.Children().Count() - 1; j++)
                    {
                        JToken dialoguePage = dialogueEntry[j.ToString()];
                        if (dialoguePage == null)
                        {
                            continue;
                        }
                        JToken options = dialoguePage["options"];
                        if (options == null)
                        {
                            continue;
                        }
                        for (int k = 0; k < options.Children().Count(); k++)
                        {
                            string optionText = options[(k + 1).ToString()].Value<string>("text");
                            JToken speechList = options[(k + 1).ToString()]["speech"];
                            if (speechList != null)
                            {
                                for (int l = 1; l <= speechList.Children().Count(); l++)
                                {
                                    string speechKey = speechList.Value<string>(l.ToString());
                                    npc._dialogSpeechTriggers.Add(speechKey, optionText);
                                }
                            }
                        }
                    }
                }
                JToken sellList = npcEntry["sell"];
                if (sellList != null)
                {
                    foreach (JToken sellEntry in sellList.Children())
                    {
                        npc._selllist.Add(sellEntry.Value<string>("text"), sellEntry.Value<int>("value"));
                    }
                }
                JToken learnSkillList = npcEntry["learnskill"];
                if (learnSkillList != null)
                {
                    foreach (JToken learnSkillEntry in learnSkillList.Children())
                    {
                        npc._learnskills.Add(learnSkillEntry.Value<string>("name"));
                    }
                }
                JToken learnSpellList = npcEntry["learnspell"];
                if (learnSpellList != null)
                {
                    foreach (JToken learnSpellEntry in learnSpellList.Children())
                    {
                        npc._learnspells.Add(learnSpellEntry.Value<string>("name"));
                    }
                }
                JToken learnActionList = npcEntry["learnaction"];
                if (learnActionList == null)
                {
                    continue;
                }
                foreach (JToken learnActionEntry in learnActionList.Children())
                {
                    npc._learnactions.Add(learnActionEntry.Value<string>("name"));
                }
            }
        }
    }

    /// <summary>Builds and sends the local player's appearance packet — position, monster form, every body/equipment sprite id+color+source, name/gender/hidden — tracking the packet byte length.</summary>
    public void SendDisplayPlayer()
    {
        if (GameWindow.ConnectedToServer)
        {
            DisplayPlayerS player = default(DisplayPlayerS);
            ushort packetLen = 0;
            player.X = (byte)_player._location.X;
            packetLen++;
            player.Y = (byte)_player._location.Y;
            packetLen++;
            player.Direction = (byte)_player._body._direction;
            packetLen++;
            player.ID = _player._id;
            packetLen += 4;
            player.Form = (ushort)_player._monsterForm;
            packetLen += 2;
            if (_player.Ghost || _player._body._helmType == 0)
            {
                player.Head = (ushort)_player._body._hairType;
                packetLen += 2;
                player.HeadColor = (byte)_player._body._hairColor;
                packetLen++;
            }
            else
            {
                player.Head = (ushort)_player._body._helmType;
                packetLen += 2;
                player.HeadColor = (byte)_player._body._helmColor;
                packetLen++;
            }
            player.HeadSource = ((_player._body._bodySource["h"] == "new") ? ((byte)2) : ((_player._body._bodySource["h"] == "myda") ? ((byte)1) : ((byte)0)));
            packetLen++;
            player.Body = (ushort)_player._body._bodyImgs["b"];
            packetLen += 2;
            player.Arms = (ushort)_player._body._bodyImgs["a"];
            packetLen += 2;
            player.Boots = (byte)_player._body._bodyImgs["l"];
            packetLen++;
            player.BootColor = (byte)_player._body._bodyColors["l"];
            packetLen++;
            player.BootSource = ((_player._body._bodySource["l"] == "new") ? ((byte)2) : ((_player._body._bodySource["l"] == "myda") ? ((byte)1) : ((byte)0)));
            packetLen++;
            player.Armor = (ushort)_player._body._bodyImgs["u"];
            packetLen += 2;
            player.ArmorColor = (byte)_player._body._bodyColors["u"];
            packetLen++;
            player.ArmorSource = ((_player._body._bodySource["u"] == "new") ? ((byte)2) : ((_player._body._bodySource["u"] == "myda") ? ((byte)1) : ((byte)0)));
            packetLen++;
            player.Shield = (byte)_player._body._bodyImgs["s"];
            packetLen++;
            player.ShieldSource = ((_player._body._bodySource["s"] == "new") ? ((byte)2) : ((_player._body._bodySource["s"] == "myda") ? ((byte)1) : ((byte)0)));
            packetLen++;
            player.Weapon = (ushort)_player._body._bodyImgs["w"];
            packetLen += 2;
            player.WeaponSource = ((_player._body._bodySource["w"] == "new") ? ((byte)2) : ((_player._body._bodySource["w"] == "myda") ? ((byte)1) : ((byte)0)));
            packetLen++;
            player.Acc = (ushort)_player._body._bodyImgs["c"];
            packetLen += 2;
            player.AccColor = (byte)_player._body._bodyColors["c"];
            packetLen++;
            player.Hidden = _player.Hidden;
            packetLen++;
            player.NameTagType = 0;
            packetLen++;
            player.Gender = (byte)_player._gender;
            packetLen++;
            packetLen++;
            player.Name = _player._name;
            packetLen += (ushort)_player._name.Length;
            DisplayPlayerPacket displayPlayerPacket = new DisplayPlayerPacket(player, packetLen);
            GameWindow.ClientSocket.Send(displayPlayerPacket.Data);
        }
    }

    /// <summary>Sends a board request — type 2 = list a board's posts, 3 = view a post (with navigation), 5 = a board action — for <paramref name="boardID"/>.</summary>
    public void RequestBoard(ushort boardID, byte type = 2, ushort postnum = 0, byte navigation = 0)
    {
        ushort lastPostNumber = 0;
        RequestBoardS board = default(RequestBoardS);
        ushort packetLen = 0;
        board.Type = type;
        packetLen++;
        if (board.Type == 2)
        {
            board.BoardID = boardID;
            packetLen += 2;
            board.LastPostNumber = lastPostNumber;
            packetLen += 2;
        }
        else if (board.Type == 3)
        {
            board.BoardID = boardID;
            packetLen += 2;
            board.PostNumber = postnum;
            packetLen += 2;
            board.Navigation = navigation;
            packetLen++;
        }
        else if (board.Type == 5)
        {
            board.BoardID = boardID;
            packetLen += 2;
            board.PostNumber = postnum;
            packetLen += 2;
        }
        if (GameWindow.ConnectedToServer)
        {
            RequestBoardPacket requestBoardPacket = new RequestBoardPacket(board, packetLen);
            GameWindow.ClientSocket.Send(requestBoardPacket.Data);
        }
    }

    /// <summary>Sends a new board post (type 6) to <paramref name="boardID"/> with the given poster name/title/body.</summary>
    public void SendNewPost(ushort boardID, string name, string title, string body)
    {
        RequestBoardS board = default(RequestBoardS);
        ushort packetLen = 0;
        board.Type = 6;
        packetLen++;
        board.BoardID = boardID;
        packetLen += 2;
        board.PosterName = name;
        packetLen++;
        packetLen += (ushort)name.Length;
        board.PostTitle = title;
        packetLen++;
        packetLen += (ushort)title.Length;
        board.PostBody = body;
        packetLen += 2;
        packetLen += (ushort)body.Length;
        if (GameWindow.ConnectedToServer)
        {
            RequestBoardPacket requestBoardPacket = new RequestBoardPacket(board, packetLen);
            GameWindow.ClientSocket.Send(requestBoardPacket.Data);
        }
    }

    /// <summary>
    ///     Renders a received board/mail packet by BoardType: 1 = the list of personal boards, 2 = a
    ///     board's posts (formatted as padded number/poster/date/title rows), 3 = a single post's body,
    ///     4 = clear mail, 6/7 = a prompt dialog.
    /// </summary>
    public void UpdateBoardDisplay(DisplayBoardS board)
    {
        if (board.BoardType == 1)
        {
            personalBoards.Clear();
            int listRow = 1;
            BoardS[] boards = board.BoardList.ToArray();
            for (int i = 0; i < boards.Length; i++)
            {
                BoardS boardEntry = boards[i];
                personalBoards.Add(boardEntry.BoardID, boardEntry);
                if (listRow <= 22)
                {
                    _boardMenu._labels["boardLabel" + listRow].BoardID = boardEntry.BoardID;
                    _boardMenu._labels["boardLabel" + listRow].ChangeText(boardEntry.BoardName);
                }
                listRow++;
            }
            if (listRow < 22)
            {
                for (int j = listRow; j <= 22; j++)
                {
                    _boardMenu._labels["boardLabel" + j].BoardID = 0;
                    _boardMenu._labels["boardLabel" + j].ChangeText("");
                }
            }
        }
        else if (board.BoardType == 2)
        {
            boardPosts.Clear();
            _viewingBoardList = true;
            _boardListMenu._boardID = board.Board.BoardID;
            int postRow = 1;
            foreach (PostS post in from z in board.Board.PostList.ToArray()
                                   orderby z.PostNumber descending
                                   select z)
            {
                boardPosts.Add(post.PostNumber, post);
                if (postRow <= 22)
                {
                    _boardListMenu._labels["boardLabel" + postRow].BoardID = board.Board.BoardID;
                    _boardListMenu._labels["boardLabel" + postRow].PostNumber = post.PostNumber;
                    string line = "";
                    ushort postNumber = post.PostNumber;
                    int padCount = 5 - postNumber.ToString().Length;
                    for (int k = 0; k < padCount; k++)
                    {
                        line += " ";
                    }
                    string line2 = line;
                    ushort postNumber2 = post.PostNumber;
                    line = line2 + postNumber2 + "     " + post.PosterName;
                    padCount = 15 - post.PosterName.Length;
                    for (int l = 0; l < padCount; l++)
                    {
                        line += " ";
                    }
                    byte month = post.Month;
                    padCount = 2 - month.ToString().Length;
                    for (int m = 0; m < padCount; m++)
                    {
                        line += " ";
                    }
                    string line3 = line;
                    byte month2 = post.Month;
                    line = line3 + month2 + "/";
                    byte day = post.Day;
                    padCount = 2 - day.ToString().Length;
                    for (int n = 0; n < padCount; n++)
                    {
                        line += " ";
                    }
                    string line4 = line;
                    byte day2 = post.Day;
                    line = line4 + day2 + "    " + post.PostTitle;
                    _boardListMenu._labels["boardLabel" + postRow].ChangeText(line);
                }
                postRow++;
            }
            if (postRow < 22)
            {
                for (int clearRow = postRow; clearRow <= 22; clearRow++)
                {
                    _boardListMenu._labels["boardLabel" + clearRow].BoardID = 0;
                    _boardListMenu._labels["boardLabel" + clearRow].PostNumber = 0;
                    _boardListMenu._labels["boardLabel" + clearRow].ChangeText("");
                }
            }
        }
        else if (board.BoardType == 3)
        {
            _viewPostMenu._boardID = _boardListMenu._boardID;
            _viewPostMenu._selectedID = board.Post.PostNumber;
            _viewPostMenu._labels["nameViewLabel"].ChangeText(board.Post.PosterName);
            _viewPostMenu._labels["dateViewLabel"].ChangeText(board.Post.Month + "/" + board.Post.Day);
            _viewPostMenu._labels["titleViewLabel"].ChangeText(board.Post.PostTitle);
            _viewPostMenu._textFields["bodyViewTF"].Text = board.Post.PostBody;
            _viewingPost = true;
        }
        else if (board.BoardType == 4)
        {
            mailPosts.Clear();
        }
        else if (board.BoardType != 5)
        {
            if (board.BoardType == 6)
            {
                _prompt._labels["promptText"].ChangeText(board.Prompt.PromptText);
                _prompt._closePost = board.Prompt.CloseBoard;
            }
            else if (board.BoardType == 7)
            {
                _prompt._labels["promptText"].ChangeText(board.Prompt.PromptText);
                _prompt._closePost = board.Prompt.CloseBoard;
            }
        }
    }

    public static void UpdateUserList(List<UserS> userlist)
    {
        _userList = userlist;
    }

    /// <summary>
    ///     Displays an incoming chat message (type 0 = say, 1 = shout, 2 = world): prefixes the color
    ///     code, wraps long lines at 70 chars, makes the speaking entity say it (within 15 tiles), and
    ///     appends it to the chat log (skipping NPC chatter when 'listen to NPCs' is off).
    /// </summary>
    public void DisplayChat(byte msgType, uint id, string msg)
    {
        switch (msgType)
        {
            case 1:
                msg = "{=c" + msg;
                break;
            case 2:
                msg = "{=e" + msg;
                break;
        }
        string line1 = msg;
        string line2 = "";
        string ellipsis = "";
        if (msg.Length > 70)
        {
            line1 = msg.Substring(0, 70);
            line2 = msg.Substring(70);
            ellipsis = "...";
        }
        bool fromNpc = false;
        Entity[] entities = _map._entities.Values.ToArray();
        foreach (Entity entity in entities)
        {
            if (entity is Item)
            {
                continue;
            }
            switch (msgType)
            {
                case 0:
                    if (_player._location.DistanceFrom(entity._location) > 15)
                    {
                        break;
                    }
                    if (id == entity._id)
                    {
                        if (entity is NPC || entity is Monster)
                        {
                            fromNpc = true;
                        }
                        entity.Speak(line1.Substring(line1.IndexOf(": ") + 2) + ellipsis, msgType);
                    }
                    entity._incomingChat.Add(line1);
                    if (line2 != "")
                    {
                        entity._incomingChat.Add(line2);
                    }
                    break;
                case 1:
                    if (_player._location.DistanceFrom(entity._location) <= 15 && id == entity._id)
                    {
                        entity.Speak(line1.Substring(line1.IndexOf("! ") + 2) + ellipsis, msgType);
                    }
                    entity._incomingChat.Add(line1);
                    if (line2 != "")
                    {
                        entity._incomingChat.Add(line2);
                    }
                    break;
            }
        }
        if (!fromNpc || _listenToNPCs)
        {
            _chatList.Add(line1);
            if (line2 != "")
            {
                _chatList.Add(line2);
            }
        }
        if (_chatList.Count == 9)
        {
            _chatMenu._buttons["chatScrollUpBtn"].Enabled = true;
            _chatMenu._buttons["chatScrollDownBtn"].Enabled = true;
            _chatMenu._buttons["chatScrollerBtn"].Hidden = false;
        }
        if (_chatList.Count > 8)
        {
            _chatIndex = _chatList.Count - 8;
        }
    }

    /// <summary>Upserts a quest flag (id → value) into the player's save, then writes it to disk.</summary>
    private void SaveQuest(string id, string value)
    {
        QuestS[] quests = pdata.quest.ToArray();
        for (int i = 0; i < quests.Length; i++)
        {
            QuestS existing = quests[i];
            if (existing.id == id)
            {
                pdata.quest.Remove(existing);
            }
        }
        QuestS quest = default(QuestS);
        quest.id = id;
        quest.value = value;
        pdata.quest.Add(quest);
        WritePlayerData();
    }

    private void SaveExp()
    {
        pdata.exp = _player._exp;
        pdata.next = _player._tnl;
        WritePlayerData();
    }

    private void SaveLevel()
    {
        pdata.lev = _player._lev;
        WritePlayerData();
    }

    private void SaveStat(byte stat, int amount = 0)
    {
        switch (stat)
        {
            case 0:
                pdata.STR++;
                if (pdata.availstats > 0)
                {
                    pdata.availstats--;
                }
                break;
            case 1:
                pdata.INT++;
                if (pdata.availstats > 0)
                {
                    pdata.availstats--;
                }
                break;
            case 2:
                pdata.WIS++;
                if (pdata.availstats > 0)
                {
                    pdata.availstats--;
                }
                break;
            case 3:
                pdata.CON++;
                if (pdata.availstats > 0)
                {
                    pdata.availstats--;
                }
                break;
            case 4:
                pdata.DEX++;
                if (pdata.availstats > 0)
                {
                    pdata.availstats--;
                }
                break;
            case 5:
                pdata.availstats += 2;
                break;
            case 6:
                pdata.basehp += amount;
                pdata.curhp += amount;
                break;
            case 7:
                pdata.basemp += amount;
                pdata.curmp += amount;
                break;
            case 8:
                pdata.ac++;
                break;
        }
        WritePlayerData();
    }

    /// <summary>Sends the current map's info to the server (when connected) and saves the map number.</summary>
    private void SaveMap()
    {
        if (GameWindow.ConnectedToServer)
        {
            MapInfoS map = default(MapInfoS);
            ushort packetSize = 0;
            map.Number = (ushort)_map._number;
            packetSize += 2;
            map.Width = (byte)_map._width;
            packetSize++;
            map.Height = (byte)_map._height;
            packetSize++;
            map.Bitmask = 0;
            packetSize++;
            map.CheckSum = 0;
            packetSize += 2;
            packetSize++;
            map.Name = _map._name;
            packetSize += (ushort)_map._name.Length;
            MapInfoPacket mapInfoPacket = new MapInfoPacket(map, packetSize);
            GameWindow.ClientSocket.Send(mapInfoPacket.Data);
        }
        pdata.mapnum = _map._number;
        WritePlayerData();
    }

    private void SaveLocation(bool ignore = false)
    {
        if (!ignore && GameWindow.ConnectedToServer)
        {
            LocationPacket locationPacket = new LocationPacket(_player._id, new Location(_player._location.X, _player._location.Y), (byte)_player._body._direction);
            GameWindow.ClientSocket.Send(locationPacket.Data);
        }
        pdata.x = (byte)_player._location.X;
        pdata.y = (byte)_player._location.Y;
        pdata.direction = (byte)_player._body._direction;
        WritePlayerData();
    }

    private void SaveItems()
    {
        pdata.inv = new List<InventoryS>();
        foreach (InventoryItem item in _inventory)
        {
            pdata.inv.Add(new InventoryS
            {
                name = item._name,
                stack = item._amount,
                slot = (byte)item._slot,
                color = (byte)item._bodyImgColor,
                dura = item._durability,
                enchantment = item._enchantment
            });
        }
        WritePlayerData();
    }

    private void SaveEquip()
    {
        SendDisplayPlayer();
        pdata.equip = new List<EquipS>();
        foreach (InventoryItem item in _equipment)
        {
            pdata.equip.Add(new EquipS
            {
                slot = (byte)item._slot,
                dura = item._durability,
                name = item._name,
                color = (byte)item._bodyImgColor,
                tab = item._tab,
                enchantment = item._enchantment
            });
        }
        WritePlayerData();
    }

    private void SaveLegend()
    {
        pdata.legend = new List<LegendS>();
        foreach (LegendMark legendMark in _player._legendMarks)
        {
            pdata.legend.Add(new LegendS
            {
                color = (byte)legendMark._color,
                text = legendMark._text,
                type = legendMark._id,
                icon = (byte)legendMark._icon
            });
        }
        WritePlayerData();
    }

    private void SaveSkill()
    {
        pdata.skills = new List<SkillS>();
        foreach (Skill skill in _skills)
        {
            pdata.skills.Add(new SkillS
            {
                level = skill._level,
                name = skill._name,
                slot = (byte)skill._slot
            });
        }
        WritePlayerData();
    }

    private void SaveSpell()
    {
        pdata.spells = new List<SpellS>();
        foreach (Spell spell in _spells)
        {
            pdata.spells.Add(new SpellS
            {
                level = spell._level,
                name = spell._name,
                slot = (byte)spell._slot
            });
        }
        WritePlayerData();
    }

    private void SaveAction()
    {
        pdata.actions = new List<ActionS>();
        foreach (Action action in _actions)
        {
            pdata.actions.Add(new ActionS
            {
                level = action._level,
                name = action._name,
                slot = (byte)action._slot
            });
        }
        WritePlayerData();
    }

    private void SaveProfile()
    {
        SendProfileData();
        pdata.gender = (byte)_player._gender;
        pdata.guild = _player._guild;
        pdata.rank = _player._rank;
        pdata.title = _player._title;
        pdata.national = _player._nation;
        pdata.haircolor = (byte)_player._body._hairColor;
        pdata.hairstyle = (byte)_player._body._hairType;
        WritePlayerData();
    }

    private void WritePlayerData()
    {
        if (!(_player._name != ""))
        {
            return;
        }
        string path = "players\\" + _player._name.ToLower() + "\\" + _player._name.ToLower() + ".txt";
        JsonSerializer jsonSerializer = new JsonSerializer();
        using StreamWriter textWriter = new StreamWriter(path);
        using JsonWriter jsonWriter = new JsonTextWriter(textWriter);
        jsonWriter.Formatting = Formatting.Indented;
        jsonSerializer.Serialize(jsonWriter, pdata);
    }
}
