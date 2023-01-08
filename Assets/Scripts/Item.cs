using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

public enum ItemType
{
    Coin,
    Player_Generated
}

public abstract class IItem
{
    string description { get; set; }
    ItemType item_type;
    int amount { get; set;}

    public abstract Sprite get_sprite();
    public abstract ItemType get_item_type();
    
}

public class GItem : IItem
{
    public ItemType item_type;
    public int amount;

    public GItem(ItemType type, int a)
    {
        item_type = type;
        amount = a;
    }

    public string description
    {
        get { return item_type.ToString(); }
    }

    public override Sprite get_sprite()
    {
        switch (item_type)
        {
            default:
            case ItemType.Coin: return ItemAssets.Instance.coin_sprite;

        }
    }

    public override ItemType get_item_type()
    {
        return item_type;
    }
}

public class PlayerItem : IItem
{
    public ItemType item_type = ItemType.Player_Generated;

    public int amount;

    public string description;

    private Sprite sprite = null;

    public PlayerItem(string prompt = "Sword") {
        description = prompt;
        amount = 1;
    }

    public override ItemType get_item_type()
    {
        return item_type;
    }
    public override Sprite get_sprite()
    {
        return sprite;
    }

    public void set_sprite(Sprite sprite)
    {
        this.sprite = sprite;
    }

}
