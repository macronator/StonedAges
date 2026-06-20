using System;
using System.Collections.Generic;
using System.Linq;

namespace StonedAges;

public class Pathfind
{
    public List<Tile> _thePath = new List<Tile>();

    private Map _map;

    private int _startx;

    private int _starty;

    public int _endx;

    public int _endy;

    private List<Tile> _openList = new List<Tile>();

    private List<Tile> _closedList = new List<Tile>();

    private Tile _startTile;

    private Tile _endTile;

    public Pathfind(int startx, int starty, int endx, int endy, Map map)
    {
        _map = map;
        _startx = startx;
        _starty = starty;
        _endx = endx;
        _endy = endy;
        foreach (Tile tile2 in _map._tiles)
        {
            tile2._F = 0;
            tile2._G = 0;
            tile2._H = 0;
            tile2._parent = null;
        }
        _startTile = _map._tiles[_starty * (int)_map._width + _startx];
        _endTile = _map._tiles[_endy * (int)_map._width + _endx];
        _openList.Add(_startTile);
        List<Tile> surrounding = getSurrounding(new Location(_startx, _starty));
        foreach (Tile item in surrounding)
        {
            item._parent = _startTile;
            _openList.Add(item);
        }
        _openList.Remove(_startTile);
        _closedList.Add(_startTile);
        H();
        foreach (Tile item2 in surrounding)
        {
            G(item2);
            F(item2);
        }
        while (true)
        {
            Tile lowF = getLowF();
            _openList.Remove(lowF);
            _closedList.Add(lowF);
            surrounding = getSurrounding(new Location(lowF.Location.X, lowF.Location.Y));
            foreach (Tile item3 in surrounding)
            {
                if (!_openList.Contains(item3))
                {
                    _openList.Add(item3);
                    item3._parent = lowF;
                    continue;
                }
                int num = lowF._G + 10;
                if (num < item3._G)
                {
                    item3._parent = lowF;
                    G(item3);
                    F(item3);
                }
            }
            if (_openList.Count() == 0 || _closedList.Contains(_endTile))
            {
                break;
            }
            foreach (Tile open in _openList)
            {
                G(open);
                F(open);
            }
        }
        Tile tile = _endTile;
        do
        {
            _thePath.Add(tile);
            tile = tile._parent;
        }
        while (tile != null && (tile.Location.X != _startx || tile.Location.Y != _starty));
    }

    private List<Tile> getSurrounding(Location snode)
    {
        Location[] array = new Location[4]
        {
            new Location(snode.X + 1, snode.Y),
            new Location(snode.X - 1, snode.Y),
            new Location(snode.X, snode.Y + 1),
            new Location(snode.X, snode.Y - 1)
        };
        List<Tile> list = new List<Tile>();
        Location[] array2 = array;
        foreach (Location location in array2)
        {
            if (!((double)location.X < _map._width) || !((double)location.Y < _map._height) || location.X < 0 || location.Y < 0)
            {
                continue;
            }
            try
            {
                Tile tile = _map._tiles[location.Y * (int)_map._width + location.X];
                if (tile != null && tile._walkable && !_closedList.Contains(tile))
                {
                    list.Add(tile);
                }
            }
            catch
            {
            }
        }
        return list;
    }

    private void G(Tile tile)
    {
        if (tile._parent != null)
        {
            tile._G = tile._parent._G + 10;
        }
        else
        {
            tile._G = 0;
        }
    }

    private void H()
    {
        foreach (Tile tile in _map._tiles)
        {
            tile._H = 10 * (Math.Abs(tile.Location.X - _endx) + Math.Abs(tile.Location.Y - _endy));
        }
    }

    private void F(Tile tile)
    {
        tile._F = tile._G + tile._H;
    }

    private Tile getLowF()
    {
        List<int> list = new List<int>();
        foreach (Tile open in _openList)
        {
            list.Add(open._F);
        }
        foreach (Tile open2 in _openList)
        {
            if (open2._F == list.Min())
            {
                return open2;
            }
        }
        return null;
    }
}
