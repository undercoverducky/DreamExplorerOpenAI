using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory 
{
    public event EventHandler on_item_list_changed;
    private List<IItem> item_list;

    public Inventory() {
        item_list = new List<IItem>();
    }

    public void add_item(IItem item)
    {
        if (item.is_stackable())
        {
            bool already_in = false;
            foreach (IItem i in item_list)
            {
                if (i.get_item_type() == item.get_item_type())
                {
                    
                    i.set_amount(i.get_amount() + item.get_amount());
                    already_in = true;
                }
            }
            if (!already_in) item_list.Add(item);

        }
        else {
            item_list.Add(item);
        }
        on_item_list_changed?.Invoke(this, EventArgs.Empty);
    }

    public List<IItem> get_item_list()
    {
        return item_list;
    }
}
