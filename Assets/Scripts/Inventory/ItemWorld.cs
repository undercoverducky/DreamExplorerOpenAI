using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ItemWorld : MonoBehaviour
{
    public static ItemWorld spawn_item_world(Vector2 position, GItem item)
    {
        Transform temp = ItemAssets.Instance.pfItemWorld;
        Transform transform = Instantiate(temp, position, Quaternion.identity);
        ItemWorld item_world = transform.GetComponent<ItemWorld>();
        item_world.set_item(item);
        return item_world;
    }

    private GItem item;
    private SpriteRenderer sprite_renderer;
    private TextMeshPro text;

    private void Awake()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
        text = transform.Find("amount").GetComponent<TextMeshPro>();
    }

    public void set_item(GItem item)
    {
        this.item = item;
        sprite_renderer.sprite = item.get_sprite();
        if (item.get_amount() > 1)
        {
            text.SetText(item.get_amount().ToString());
        }
        
    }

    public GItem get_item()
    {
        return item;
    }

    public void destroy_item()
    {
        Destroy(gameObject);
    }

}