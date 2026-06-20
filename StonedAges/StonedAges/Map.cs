using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Engine;
using Newtonsoft.Json.Linq;

namespace StonedAges;

public class Map
{
    private delegate void taskWorkerDelegate(List<Tile> tiles);

    private const int xconst = 28;

    private const int yconst = 14;

    public bool _loaded;

    public double _width;

    public double _height;

    public MAPFile _mapFile;

    private TextureManager _textureManager;

    public double xOffset;

    public double yOffset;

    public bool useOffs;

    private Font _font;

    private Input _input;

    public Player _player;

    public int _number;

    public string _name;

    public List<Projectile> _projectiles = new List<Projectile>();

    public List<IWO> _iwos = new List<IWO>();

    public List<Portal> _portals = new List<Portal>();

    public List<Reactor> _reactors = new List<Reactor>();

    public Dictionary<uint, Entity> _entities = new Dictionary<uint, Entity>();

    public Dictionary<string, Spawn> _spawns = new Dictionary<string, Spawn>();

    public List<Tile> _tiles = new List<Tile>();

    public List<Tile> _tilesInView = new List<Tile>();

    private ButtonMenu _menu;

    private JObject _itemsDB;

    private GameState gs;

    private bool _myTaskIsRunning;

    private readonly object _sync = new object();

    public bool IsBusy => _myTaskIsRunning;

    public event AsyncCompletedEventHandler TaskCompleted;

    public int _toTileX(int x, int y)
    {
        return x * 28 + y * 28 * -1;
    }

    public int _toTileY(int x, int y)
    {
        return x * 14 + y * 14;
    }

    public Map()
    {
        _loaded = false;
    }

    public Map(JObject itemsdb, Font font, Input input, GameState Gs, TextureManager textureManager, Player player, int number, ButtonMenu menu, JObject mapsDB, List<string> wallaniarr, MAPFile mapFile)
    {
        gs = Gs;
        _font = font;
        _itemsDB = itemsdb;
        _loaded = false;
        JToken jToken = null;
        foreach (JToken item3 in mapsDB["maps"].Children())
        {
            int num = item3.Value<int>("mapnum");
            if (num != number)
            {
                continue;
            }
            _name = item3.Value<string>("mapname");
            _width = item3.Value<double>("width");
            _height = item3.Value<double>("height");
            JToken jToken2 = item3["reactors"];
            if (jToken2 != null)
            {
                foreach (JToken item4 in item3["reactors"].Children())
                {
                    string[] array = item4.Value<string>("xy").Split(',');
                    byte x = byte.Parse(array[0]);
                    byte y = byte.Parse(array[1]);
                    Reactor reactor = new Reactor(new Location(x, y))
                    {
                        _type = item4.Value<byte>("type"),
                        _triggerType = item4.Value<byte>("trigger")
                    };
                    if (reactor._triggerType == 0)
                    {
                        reactor._monType = item4.Value<string>("dialog");
                    }
                    else if (reactor._triggerType == 1)
                    {
                        reactor._monType = item4.Value<string>("montype");
                    }
                    _reactors.Add(reactor);
                }
            }
            if (number != 1001)
            {
                foreach (JToken item5 in item3["to"].Children())
                {
                    int toMap = item5.Value<int>("map");
                    string[] array2 = item5.Value<string>("ports").Split('>');
                    int fromX = int.Parse(array2[0].Split(',')[0]);
                    int fromY = int.Parse(array2[0].Split(',')[1]);
                    int toX = int.Parse(array2[1].Split(',')[0]);
                    int toY = int.Parse(array2[1].Split(',')[1]);
                    Portal item = new Portal(number, toMap, fromX, fromY, toX, toY);
                    _portals.Add(item);
                }
            }
            jToken = item3["iwos"];
            break;
        }
        _mapFile = mapFile;
        _menu = menu;
        _textureManager = textureManager;
        _input = input;
        _player = player;
        _number = number;
        double num2 = 283 - _toTileX(_player._location.X, _player._location.Y);
        double num3 = 181 - _toTileY(_player._location.X, _player._location.Y);
        for (int i = 0; (double)i < _height; i++)
        {
            for (int j = 0; (double)j < _width; j++)
            {
                Location location = new Location(j, i);
                location.InView(_player._location);
                Texture texture = default(Texture);
                int floorTile = _mapFile[j, i].FloorTile;
                if (floorTile != 0)
                {
                    texture = _textureManager.Get("stc" + floorTile.ToString("00000") + "_F0_C0", ".tile");
                }
                bool flag = false;
                bool flag2 = false;
                Texture lwTexture = default(Texture);
                Texture rwTexture = default(Texture);
                int leftWall = _mapFile[j, i].LeftWall;
                int rightWall = _mapFile[j, i].RightWall;
                if (leftWall % 10000 > 11)
                {
                    flag = true;
                    lwTexture = _textureManager.Get("stc" + leftWall.ToString("00000") + ".hpf_F0_C0", ".hpf");
                }
                if (rightWall % 10000 > 11)
                {
                    flag2 = true;
                    rwTexture = _textureManager.Get("stc" + rightWall.ToString("00000") + ".hpf_F0_C0", ".hpf");
                }
                bool walkable = true;
                if ((leftWall % 10000 > 11 && leftWall < _textureManager.SOTP().Count() && _textureManager.SOTP()[leftWall - 1] != 0) || (rightWall % 10000 > 11 && rightWall < _textureManager.SOTP().Count() && _textureManager.SOTP()[rightWall - 1] != 0))
                {
                    walkable = false;
                }
                Wall wall = null;
                if (flag || flag2)
                {
                    wall = new Wall(_textureManager, leftWall, rightWall, lwTexture, rwTexture, j, i, num2 + (double)(j * 56 / 2), num3 + (double)(j * 28 / 2));
                    if (gs.wallaninums.Contains(leftWall) || gs.wallaninums.Contains(rightWall))
                    {
                        string[] array3 = wallaniarr.ToArray();
                        foreach (string text in array3)
                        {
                            if (!text.Contains(leftWall.ToString()) && !text.Contains(rightWall.ToString()))
                            {
                                continue;
                            }
                            string[] source = text.Split();
                            string text2 = source.Last();
                            string[] array4;
                            if (source.Contains(leftWall.ToString()) && leftWall.ToString() != text2)
                            {
                                wall._idleSpeed = int.Parse(text2);
                                array4 = source.ToArray();
                                foreach (string text3 in array4)
                                {
                                    if (text3 != text2)
                                    {
                                        int item2 = int.Parse(text3);
                                        _textureManager.Get("stc" + item2.ToString("00000") + ".hpf_F0_C0", ".hpf");
                                        wall._lIdleArr.Add(item2);
                                    }
                                }
                            }
                            if (!source.Contains(rightWall.ToString()) || !(rightWall.ToString() != text2))
                            {
                                continue;
                            }
                            wall._idleSpeed = int.Parse(text2);
                            array4 = source.ToArray();
                            foreach (string text4 in array4)
                            {
                                if (text4 != text2)
                                {
                                    int num4 = int.Parse(text4);
                                    _textureManager.Get("stc" + num4.ToString("00000") + ".hpf_F0_C0", ".hpf");
                                    wall._rIdleArr.Add(int.Parse(text4));
                                }
                            }
                        }
                    }
                }
                Tile tile = new Tile(this, _textureManager, floorTile, j, i, num2 + (double)(j * 56 / 2), num3 + (double)(j * 28 / 2), texture, walkable, wall);
                if (waterTiles((ushort)floorTile))
                {
                    tile._water = true;
                }
                tile._rendered = true;
                _tiles.Add(tile);
                IWO iWO = null;
                if (flag || flag2)
                {
                    if (leftWall == 913)
                    {
                        iWO = new IWO(0, "Chest", 0, new Location(j, i), _number, "", tile);
                    }
                    else if (rightWall == 921)
                    {
                        iWO = new IWO(0, "Chest", 1, new Location(j, i), _number, "", tile);
                    }
                    else
                    {
                        switch (leftWall)
                        {
                            case 2164:
                            case 2228:
                            case 2292:
                                iWO = new IWO(0, "DoorC", 0, new Location(j, i), _number, "", tile);
                                break;
                            case 2168:
                            case 2232:
                            case 2296:
                                iWO = new IWO(0, "DoorC", 0, new Location(j, i), _number, "", tile)
                                {
                                    _enabled = true,
                                    _tile =
                                {
                                    _walkable = true
                                }
                                };
                                break;
                            default:
                                switch (rightWall)
                                {
                                    case 2197:
                                    case 2261:
                                    case 2329:
                                        iWO = new IWO(0, "DoorC", 1, new Location(j, i), _number, "", tile);
                                        break;
                                    case 2193:
                                    case 2265:
                                    case 2325:
                                        iWO = new IWO(0, "DoorC", 1, new Location(j, i), _number, "", tile)
                                        {
                                            _enabled = true,
                                            _tile =
                                    {
                                        _walkable = true
                                    }
                                        };
                                        break;
                                    default:
                                        switch (leftWall)
                                        {
                                            case 1994:
                                            case 2436:
                                                iWO = new IWO(0, "DoorD", 0, new Location(j, i), _number, "", tile);
                                                break;
                                            case 1997:
                                            case 2432:
                                                iWO = new IWO(0, "DoorD", 0, new Location(j, i), _number, "", tile)
                                                {
                                                    _enabled = true,
                                                    _tile =
                                        {
                                            _walkable = true
                                        }
                                                };
                                                break;
                                            default:
                                                switch (rightWall)
                                                {
                                                    case 2000:
                                                    case 2461:
                                                        iWO = new IWO(0, "DoorD", 1, new Location(j, i), _number, "", tile);
                                                        break;
                                                    case 2003:
                                                    case 2465:
                                                        iWO = new IWO(0, "DoorD", 1, new Location(j, i), _number, "", tile)
                                                        {
                                                            _enabled = true,
                                                            _tile =
                                            {
                                                _walkable = true
                                            }
                                                        };
                                                        break;
                                                    default:
                                                        switch (leftWall)
                                                        {
                                                            case 2688:
                                                            case 2898:
                                                            case 2946:
                                                            case 2994:
                                                            case 3059:
                                                            case 3119:
                                                            case 3179:
                                                                iWO = new IWO(0, "Door", 0, new Location(j, i), _number, "", tile);
                                                                break;
                                                            case 2695:
                                                            case 2904:
                                                            case 2952:
                                                            case 3000:
                                                            case 3067:
                                                            case 3127:
                                                            case 3187:
                                                                iWO = new IWO(0, "Door", 0, new Location(j, i), _number, "", tile)
                                                                {
                                                                    _enabled = true,
                                                                    _tile =
                                                {
                                                    _walkable = true
                                                }
                                                                };
                                                                break;
                                                            default:
                                                                switch (rightWall)
                                                                {
                                                                    case 2674:
                                                                    case 2929:
                                                                    case 2971:
                                                                    case 3019:
                                                                    case 3090:
                                                                    case 3150:
                                                                    case 3210:
                                                                        iWO = new IWO(0, "Door", 1, new Location(j, i), _number, "", tile);
                                                                        break;
                                                                    case 2681:
                                                                    case 2923:
                                                                    case 2977:
                                                                    case 3025:
                                                                    case 3098:
                                                                    case 3158:
                                                                    case 3218:
                                                                        iWO = new IWO(0, "Door", 1, new Location(j, i), _number, "", tile)
                                                                        {
                                                                            _enabled = true,
                                                                            _tile =
                                                    {
                                                        _walkable = true
                                                    }
                                                                        };
                                                                        break;
                                                                    default:
                                                                        switch (leftWall)
                                                                        {
                                                                            case 2688:
                                                                                iWO = new IWO(0, "DoorA", 0, new Location(j, i), _number, "", tile);
                                                                                break;
                                                                            case 2695:
                                                                                iWO = new IWO(0, "DoorA", 0, new Location(j, i), _number, "", tile)
                                                                                {
                                                                                    _enabled = true,
                                                                                    _tile =
                                                        {
                                                            _walkable = true
                                                        }
                                                                                };
                                                                                break;
                                                                            default:
                                                                                switch (rightWall)
                                                                                {
                                                                                    case 2674:
                                                                                        iWO = new IWO(0, "DoorA", 1, new Location(j, i), _number, "", tile);
                                                                                        break;
                                                                                    case 2681:
                                                                                        iWO = new IWO(0, "DoorA", 1, new Location(j, i), _number, "", tile)
                                                                                        {
                                                                                            _enabled = true,
                                                                                            _tile =
                                                            {
                                                                _walkable = true
                                                            }
                                                                                        };
                                                                                        break;
                                                                                    default:
                                                                                        switch (leftWall)
                                                                                        {
                                                                                            case 2727:
                                                                                            case 2761:
                                                                                                iWO = new IWO(0, "DoorB", 0, new Location(j, i), _number, "", tile);
                                                                                                break;
                                                                                            case 2734:
                                                                                            case 2768:
                                                                                                iWO = new IWO(0, "DoorB", 0, new Location(j, i), _number, "", tile)
                                                                                                {
                                                                                                    _enabled = true,
                                                                                                    _tile =
                                                                {
                                                                    _walkable = true
                                                                }
                                                                                                };
                                                                                                break;
                                                                                            default:
                                                                                                switch (rightWall)
                                                                                                {
                                                                                                    case 2715:
                                                                                                    case 2777:
                                                                                                        iWO = new IWO(0, "DoorB", 1, new Location(j, i), _number, "", tile);
                                                                                                        break;
                                                                                                    case 2722:
                                                                                                    case 2784:
                                                                                                        iWO = new IWO(0, "DoorB", 1, new Location(j, i), _number, "", tile)
                                                                                                        {
                                                                                                            _enabled = true,
                                                                                                            _tile =
                                                                    {
                                                                        _walkable = true
                                                                    }
                                                                                                        };
                                                                                                        break;
                                                                                                    default:
                                                                                                        switch (leftWall)
                                                                                                        {
                                                                                                            case 2875:
                                                                                                                iWO = new IWO(0, "DoorE", 0, new Location(j, i), _number, "", tile);
                                                                                                                break;
                                                                                                            case 2882:
                                                                                                                iWO = new IWO(0, "DoorE", 0, new Location(j, i), _number, "", tile)
                                                                                                                {
                                                                                                                    _enabled = true,
                                                                                                                    _tile =
                                                                        {
                                                                            _walkable = true
                                                                        }
                                                                                                                };
                                                                                                                break;
                                                                                                            default:
                                                                                                                switch (rightWall)
                                                                                                                {
                                                                                                                    case 2851:
                                                                                                                        iWO = new IWO(0, "DoorE", 1, new Location(j, i), _number, "", tile);
                                                                                                                        break;
                                                                                                                    case 2858:
                                                                                                                        iWO = new IWO(0, "DoorE", 1, new Location(j, i), _number, "", tile)
                                                                                                                        {
                                                                                                                            _enabled = true,
                                                                                                                            _tile =
                                                                            {
                                                                                _walkable = true
                                                                            }
                                                                                                                        };
                                                                                                                        break;
                                                                                                                }
                                                                                                                break;
                                                                                                        }
                                                                                                        break;
                                                                                                }
                                                                                                break;
                                                                                        }
                                                                                        break;
                                                                                }
                                                                                break;
                                                                        }
                                                                        break;
                                                                }
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                        }
                                        break;
                                }
                                break;
                        }
                    }
                    if (jToken != null)
                    {
                        foreach (JToken item6 in jToken.Children())
                        {
                            int num5 = item6.Value<int>("x");
                            int num6 = item6.Value<int>("y");
                            if (num5 == j && num6 == i)
                            {
                                ushort id = item6.Value<ushort>("id");
                                string text5 = item6.Value<string>("text");
                                iWO = new IWO(id, item6.Value<string>("type"), item6.Value<int>("direction"), new Location(j, i), _number, text5, tile);
                                if (_textureManager.openDoors(leftWall) || _textureManager.openDoors(rightWall))
                                {
                                    iWO._enabled = true;
                                    iWO._tile._walkable = true;
                                }
                            }
                        }
                    }
                }
                Portal portal = tileHasPortal(j, i);
                if (portal != null)
                {
                    if (portal._toMap == 1001)
                    {
                        tile.SpellAni(214, 320, repeat: true);
                    }
                    else
                    {
                        tile.SpellAni(96, 80, repeat: true);
                    }
                }
            }
            num2 -= 28.0;
            num3 += 14.0;
        }
        _player._mapName = _name;
        _player._mapNum = _number;
        _entities.Add(_player._id, _player);
        _player._tileTime = DateTime.UtcNow;
        _loaded = true;
    }

    private void thrd()
    {
        foreach (Tile item in from z in _tiles.ToArray()
                              orderby z.Location.DistanceFrom(_player._location)
                              select z)
        {
            if (item._floor > 0)
            {
                item._sprite.Texture = _textureManager.Get("stc" + item._floor.ToString("00000") + "_F0_C0", ".tile");
            }
            if (item._wall != null)
            {
                if (item._wall._lwall % 10000 > 11)
                {
                    item._wall._leftWall.Texture = _textureManager.Get("stc" + item._wall._lwall.ToString("00000") + ".hpf_F0_C0", ".hpf");
                }
                if (item._wall._rwall % 10000 > 11)
                {
                    item._wall._rightWall.Texture = _textureManager.Get("stc" + item._wall._rwall.ToString("00000") + ".hpf_F0_C0", ".hpf");
                }
            }
            item._rendered = true;
        }
    }

    private void taskWorker(List<Tile> tiles)
    {
        foreach (Tile item in from z in tiles.ToArray()
                              orderby z.Location.DistanceFrom(_player._location)
                              select z)
        {
            if (item._floor > 0)
            {
                item._sprite.Texture = _textureManager.Get("stc" + item._floor.ToString("00000") + "_F0_C0", ".tile");
            }
            if (item._wall != null)
            {
                if (item._wall._lwall % 10000 > 11)
                {
                    item._wall._leftWall.Texture = _textureManager.Get("stc" + item._wall._lwall.ToString("00000") + ".hpf_F0_C0", ".hpf");
                }
                if (item._wall._rwall % 10000 > 11)
                {
                    item._wall._rightWall.Texture = _textureManager.Get("stc" + item._wall._rwall.ToString("00000") + ".hpf_F0_C0", ".hpf");
                }
            }
            item._rendered = true;
        }
    }

    public void taskAsync(List<Tile> tiles)
    {
        taskWorkerDelegate taskWorkerDelegate = taskWorker;
        AsyncCallback callback = taskCompletedCallback;
        lock (_sync)
        {
            if (_myTaskIsRunning)
            {
                throw new InvalidOperationException("The control is currently busy.");
            }
            AsyncOperation @object = AsyncOperationManager.CreateOperation(null);
            taskWorkerDelegate.BeginInvoke(tiles, callback, @object);
            _myTaskIsRunning = true;
        }
    }

    private void taskCompletedCallback(IAsyncResult ar)
    {
        taskWorkerDelegate taskWorkerDelegate = (taskWorkerDelegate)((AsyncResult)ar).AsyncDelegate;
        AsyncOperation asyncOperation = (AsyncOperation)ar.AsyncState;
        taskWorkerDelegate.EndInvoke(ar);
        lock (_sync)
        {
            _myTaskIsRunning = false;
        }
        AsyncCompletedEventArgs arg = new AsyncCompletedEventArgs(null, cancelled: false, null);
        asyncOperation.PostOperationCompleted(delegate (object e)
        {
            OnTaskCompleted((AsyncCompletedEventArgs)e);
        }, arg);
    }

    protected virtual void OnTaskCompleted(AsyncCompletedEventArgs e)
    {
        if (this.TaskCompleted != null)
        {
            this.TaskCompleted(this, e);
        }
    }

    public void Update(double elapsedTime)
    {
    }

    public void Render(Renderer renderer, double elapsedTime)
    {
        foreach (Tile tile in _tiles)
        {
            if (tile.Location.InView(_player._location))
            {
                tile.RenderAndUpdate(renderer, elapsedTime);
            }
        }
        foreach (Tile tile2 in _tiles)
        {
            if (!tile2.Location.InView(_player._location) && (tile2._wall == null || (tile2._wall._lIdleArr.Count() <= 0 && tile2._wall._rIdleArr.Count() <= 0)))
            {
                continue;
            }
            renderer.DrawSprite(tile2._spellAni, 0);
            if (tile2._highlight)
            {
                renderer.DrawSprite(tile2._highlightSprite, 0);
            }
            foreach (Entity item in tile2._entities.Values.OrderBy((Entity e) => e.CreateTime.Millisecond))
            {
                if (item is Item)
                {
                    (item as Item).Render(renderer);
                }
            }
            foreach (Entity item2 in tile2._entities.Values.OrderBy((Entity e) => e.CreateTime.Millisecond))
            {
                if (item2._id == _player._id)
                {
                    (item2 as Player).Update(elapsedTime);
                    (item2 as Player).Render(renderer);
                }
                else if (item2 is NPC && !item2.Hidden)
                {
                    (item2 as NPC).Update(elapsedTime);
                    (item2 as NPC).Render(renderer);
                }
                else if (item2 is Monster)
                {
                    (item2 as Monster).Update(elapsedTime);
                    (item2 as Monster).Render(renderer);
                }
                else if (item2 is Player && !item2.Hidden)
                {
                    (item2 as Player).Update(elapsedTime);
                    (item2 as Player).Render(renderer);
                }
            }
            if (tile2._wall != null)
            {
                renderer.DrawSprite(tile2._wall._leftWall, 0);
                renderer.DrawSprite(tile2._wall._rightWall, 0);
            }
        }
        Projectile[] array = _projectiles.ToArray();
        foreach (Projectile projectile in array)
        {
            projectile.Update(elapsedTime);
            projectile.Render(renderer);
        }
    }

    private bool waterTiles(ushort tile)
    {
        int[] source = new int[29]
        {
            1291, 1292, 1293, 1294, 1308, 1309, 1310, 1311, 1337, 1352,
            1364, 1366, 1369, 1371, 1374, 1375, 1379, 1380, 1536, 1543,
            1545, 1546, 1548, 1549, 1550, 1553, 1558, 1559, 1561
        };
        if (tile >= 327 && tile <= 425)
        {
            return true;
        }
        if (tile >= 756 && tile <= 768)
        {
            return true;
        }
        if (tile >= 983 && tile <= 998)
        {
            return true;
        }
        if (tile >= 1313 && tile <= 1328)
        {
            return true;
        }
        if (source.Contains(tile))
        {
            return true;
        }
        return false;
    }

    public Portal tileHasPortal(int x, int y)
    {
        Portal[] array = _portals.ToArray();
        foreach (Portal portal in array)
        {
            if (portal._fromX == x && portal._fromY == y)
            {
                return portal;
            }
        }
        return null;
    }

    public Reactor tileHasReactor(byte x, byte y)
    {
        Reactor[] array = _reactors.ToArray();
        foreach (Reactor reactor in array)
        {
            if (reactor._location.X == x && reactor._location.Y == y)
            {
                return reactor;
            }
        }
        return null;
    }

    public void SpawnItem(string name, Location loc, int amount = 1, int durability = 0, int bodyImgColor = 0, string enchantment = "")
    {
        if (name.Contains(" e:"))
        {
            enchantment = name.Substring(name.IndexOf(" e:") + 3);
            name = name.Remove(name.IndexOf(" e:"));
        }
        foreach (JToken item2 in _itemsDB["items"].Children())
        {
            string text = item2.Value<string>("name");
            if (!text.Equals(name, StringComparison.CurrentCultureIgnoreCase))
            {
                continue;
            }
            int num = item2.Value<int>("imagefolder");
            int num2 = item2.Value<int>("frame");
            if (text == "Gold")
            {
                if (amount <= 0)
                {
                    break;
                }
                if (amount == 1)
                {
                    text = "Silver Coin";
                    num2 = 138;
                }
                else if (amount < 10)
                {
                    text = "Gold Coin";
                    num2 = 137;
                }
                else if (amount < 100)
                {
                    text = "Silver Pile";
                    num2 = 141;
                }
                else
                {
                    text = "Gold Pile";
                    num2 = 140;
                }
            }
            string imgString = "item" + num.ToString("000") + "_F" + (num2 - 1) + "_C0";
            Texture img = _textureManager.Get("item" + num.ToString("000") + "_F" + (num2 - 1) + "_C0");
            Item item = new Item(gs, _textureManager, _font, text, loc, this, imgString, img, amount, durability, bodyImgColor, enchantment);
            Tile tile = _tiles[loc.Y * (int)_width + loc.X];
            item.SetPosition(new Vector(tile._position.X + tile._width / 2.0 - item._sprite.GetWidth() / 2.0 + 1.0, tile._position.Y + tile._height - item._sprite.GetHeight() + 1.0, 0.0));
            break;
        }
    }
}
