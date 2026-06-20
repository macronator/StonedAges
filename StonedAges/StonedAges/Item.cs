using System.Collections.Generic;
using System.Drawing;
using Engine;

namespace StonedAges;

public class Item : Entity
{
    public static Dictionary<uint, Item> List = new Dictionary<uint, Item>();

    public string _enchantment = "";

    public string _displayName = "";

    public int _amount = 1;

    public int _durability;

    public int _bodyImgColor;

    public Item(GameState Gs, TextureManager textureManager, Engine.Font font, string name, Location loc, Map map, string imgString, Texture img, int amount, int durability, int bodyImgColor, string enchantment = "", uint id = 0u)
        : base(Gs, textureManager, font, name, loc, map, imgString, img, id)
    {
        _entityType = EntityType.Item;
        _enchantment = enchantment;
        _amount = amount;
        _durability = durability;
        _bodyImgColor = bodyImgColor;
        _targetBox = new Rectangle(0, 0, 27, 27);
        if (id != 0)
        {
            List.Add(id, this);
        }
    }

    public void Update(double elapsedTime)
    {
    }

    public void Render(Renderer renderer)
    {
        renderer.DrawSprite(_sprite, 0);
    }
}
