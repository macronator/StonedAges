using System;

namespace StonedAges;

public class Location
{
    public int X { get; set; }

    public int Y { get; set; }

    public Location(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool InLine(Location loc, D direction, int range)
    {
        if (DistanceFrom(loc) <= range)
        {
            if (direction == D.Up && X == loc.X && Y >= loc.Y)
            {
                return true;
            }
            if (direction == D.Down && X == loc.X && Y <= loc.Y)
            {
                return true;
            }
            if (direction == D.Right && X <= loc.X && Y == loc.Y)
            {
                return true;
            }
            if (direction == D.Left && X >= loc.X && Y == loc.Y)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsInFront(Location loc, D direction, int range = 1)
    {
        if (DistanceFrom(loc) <= range)
        {
            if (loc.Y == Y - 1 && direction == D.Up)
            {
                return true;
            }
            if (loc.Y == Y + 1 && direction == D.Down)
            {
                return true;
            }
            if (loc.X == X + 1 && direction == D.Right)
            {
                return true;
            }
            if (loc.X == X - 1 && direction == D.Left)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsBehind(Location loc, D direction, int range = 1)
    {
        if (DistanceFrom(loc) <= range)
        {
            if (loc.Y == Y - 1 && direction == D.Down)
            {
                return true;
            }
            if (loc.Y == Y + 1 && direction == D.Up)
            {
                return true;
            }
            if (loc.X == X + 1 && direction == D.Left)
            {
                return true;
            }
            if (loc.X == X - 1 && direction == D.Right)
            {
                return true;
            }
        }
        return false;
    }

    public bool WithinRange(Location loc, int range)
    {
        if (Math.Abs(X - loc.X) <= range)
        {
            return Math.Abs(Y - loc.Y) <= range;
        }
        return false;
    }

    public int DistanceFrom(Location loc)
    {
        return Math.Abs(X - loc.X) + Math.Abs(Y - loc.Y);
    }

    public bool InView(Location loc)
    {
        return DistanceFrom(loc) <= 21;
    }
}
