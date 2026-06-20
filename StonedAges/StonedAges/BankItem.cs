using Engine;

namespace StonedAges;

public class BankItem
{
    public Texture _tex;

    public string _name;

    public string _tab;

    public int _amount;

    public int _value;

    public int _maxAmount = 1;

    public int _maxDurability;

    public string _displayName = "";

    public string _enchantment = "";

    public string _description = "";

    public int _dyeable;

    public string _level = "";

    public string _gender = "";

    public string _weaponDmg = "";

    public string _atk = "";

    public string _def = "";

    public int _hp;

    public int _mp;

    public int _str;

    public int _int;

    public int _wis;

    public int _con;

    public int _dex;

    public int _mr;

    public int _ac;

    public int _dmg;

    public int _hit;

    public int _reg;

    public BankItem(string name, string tab, int amount, Texture tex)
    {
        _name = name;
        _displayName = name;
        _tab = tab;
        _amount = amount;
        _tex = tex;
    }
}
