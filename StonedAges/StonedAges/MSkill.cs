namespace StonedAges;

public class MSkill
{
    public string _name;

    public string _trigger;

    public int _cdperc;

    public byte _delaymin;

    public byte _delaymax;

    public MSkill(string name, string trigger, int cdperc, byte delaymin, byte delaymax)
    {
        _name = name;
        _trigger = trigger;
        _cdperc = cdperc;
        _delaymin = delaymin;
        _delaymax = delaymax;
        if (_delaymin == 0)
        {
            _delaymin = 1;
        }
        if (_delaymax == 0)
        {
            _delaymax = 2;
        }
    }
}
