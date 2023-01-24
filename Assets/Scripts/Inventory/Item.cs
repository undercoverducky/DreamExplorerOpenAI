using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;

public enum ItemType
{
    Coin,
    Hat,
    Player_Generated,
    Lucidator,
}

public abstract class IItem
{
    public string description { get; set; }

    public abstract Sprite get_sprite();
    public abstract ItemType get_item_type();
    public abstract bool is_stackable();
    public abstract void set_amount(int a);
    public abstract int get_amount();

}

[Serializable]
public class GItem : IItem
{
    public ItemType item_type;
    public int amount;

    public GItem(ItemType type, int a)
    {
        item_type = type;
        amount = a;
    }

    public new string description
    {
        get { return item_type.ToString(); }
    }

    public override Sprite get_sprite()
    {
        switch (item_type)
        {
            default:
            case ItemType.Coin: return ItemAssets.Instance.coin_sprite;
            case ItemType.Hat: return ItemAssets.Instance.hat_sprite;
            case ItemType.Lucidator: return ItemAssets.Instance.lucidator_sprite;
        }
    }

    public override bool is_stackable()
    {
        switch (item_type)
        {
            default:
            case ItemType.Coin:
                return true;
            case ItemType.Hat:
                return false;
            case ItemType.Lucidator:
                return true;
        }
    }

    public override ItemType get_item_type()
    {
        return item_type;
    }

    public override void set_amount(int a)
    {
        amount = a;
    }

    public override int get_amount() {
        return amount;
    }
}

public class PlayerItem : IItem
{
    public event EventHandler on_item_sprite_changed;
    public ItemType item_type = ItemType.Player_Generated;
    public new string description;
    public int amount;

    private Sprite sprite = null;

    public PlayerItem(string prompt = "Sword") {
        description = prompt;
        amount = 1;
        sprite = ItemAssets.Instance.loading_sprite;
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
        on_item_sprite_changed?.Invoke(this, EventArgs.Empty);
    }

    public override bool is_stackable()
    {
        return false;
    }

    public override void set_amount(int a)
    {
        return;
    }

    public override int get_amount()
    {
        return amount;
    }

}
