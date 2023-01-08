using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory 
{
    private List<IItem> item_list;

    public Inventory() {
        item_list = new List<IItem>();
    }

    public void add_item(IItem item)
    {
        item_list.Add(item);
    }

    public List<IItem> get_item_list()
    {
        return item_list;
    }
}
