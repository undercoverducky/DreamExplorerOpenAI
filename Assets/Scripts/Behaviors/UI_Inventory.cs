using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using TMPro;

public class UI_Inventory : MonoBehaviour
{
    public static UI_Inventory Instance { get; set; }
    private Inventory inventory;
    private Transform itemslot_container;
    private Transform itemslot_template;

    private float itemslot_cell_size = 150f;
    private int max_cols;

    public bool on = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    public void show() {
        gameObject.SetActive(true);
        on = true;
    }

    public void hide()
    {
        gameObject.SetActive(false);
        on = false;
    }


    public Inventory get_inventory() {
        return inventory;
    }
    public void set_inventory(Inventory inventory)
    {
        this.inventory = inventory;
        if (itemslot_container == null || itemslot_template == null)
        {
            itemslot_container = transform.Find("itemSlotContainer");
            itemslot_template = itemslot_container.Find("itemSlotTemplate");
        }
        inventory.on_item_list_changed += inventory_on_item_list_changed;
        max_cols = inventory.max_size;
        refresh_inventory_items();
    }

    private void inventory_on_item_list_changed(object sender, System.EventArgs e)
    {
        refresh_inventory_items();
    }

    
    public void refresh_inventory_items()
    {
        foreach (Transform child in itemslot_container)
        {
            if (child == itemslot_template) continue;
            Destroy(child.gameObject);
        }
        int x = 0;
        int y = 0;
        foreach (IItem item in inventory.get_item_list())
        {
            RectTransform itemslot_rect_transform = Instantiate(itemslot_template, itemslot_container).GetComponent<RectTransform>();
            itemslot_rect_transform.gameObject.SetActive(true);

            itemslot_rect_transform.GetComponent<CodeMonkey.Utils.Button_UI>().ClickFunc = () =>
            {
                //use_item.
                inventory.use_item(item);
            };
            itemslot_rect_transform.anchoredPosition = new Vector2(x * itemslot_cell_size, -y * itemslot_cell_size);
            Image image = itemslot_rect_transform.Find("Image").GetComponent<Image>();
            image.sprite = item.get_sprite();
            TextMeshProUGUI amount_text = itemslot_rect_transform.Find("amount").GetComponent<TextMeshProUGUI>();
            if (item.get_amount() > 1)
            {
                amount_text.SetText(item.get_amount().ToString());
            }
            else
            {
                amount_text.SetText(String.Empty);
            }
            x++;
            if (x > max_cols-1)
            {
                x = 0;
                y++;
            }
        }
    }

    

    

}
