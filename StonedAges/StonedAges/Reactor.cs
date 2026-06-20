namespace StonedAges;

public class Reactor
{
    public Location _location;

    public byte _type;

    public byte _triggerType;

    public string _monType = "";

    public Spell _trap;

    public Reactor(Location loc)
    {
        _location = loc;
    }

    public void Update(double elapsedTime)
    {
    }
}
