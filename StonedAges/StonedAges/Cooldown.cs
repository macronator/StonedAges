using System;

namespace StonedAges;

public class Cooldown
{
    private byte _panel;

    private byte _slot;

    private uint _length;

    private DateTime _timer;

    public Cooldown()
    {
    }

    public Cooldown(byte panel, byte slot, uint length)
    {
        _panel = panel;
        _slot = slot;
        _length = length;
        _timer = DateTime.UtcNow;
    }

    public bool Ready()
    {
        return DateTime.UtcNow.Subtract(_timer).TotalMilliseconds >= (double)(_length * 1000);
    }

    public void Update()
    {
        _ = DateTime.UtcNow.Subtract(_timer).TotalMilliseconds;
        _ = (double)(_length * 1000);
    }
}
