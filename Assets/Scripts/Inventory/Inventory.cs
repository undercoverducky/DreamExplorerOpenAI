using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemUseEventArgs : EventArgs
{
    public IItem used_item { get; set; }
}

public class Inventory 
{
    public event EventHandler on_item_list_changed;
    public event EventHandler on_item_use;
    public int max_size;
    private List<IItem> item_list;
    private Action<IItem> use_item_action;
    
    public Inventory(Action<IItem> use_item_action, int max_size) {
        this.use_item_action = use_item_action;
        item_list = new List<IItem>();
        this.max_size = max_size;
    }

    public void add_item(IItem item) {
        if (item_list.Count >= max_size)
        {
            Debug.Log("Inventory is full!");
            return;
        }
        if (item.is_stackable()) {
            bool already_in = false;
            foreach (IItem i in item_list) {
                if (i.get_item_type() == item.get_item_type()) {
                    
                    i.set_amount(i.get_amount() + item.get_amount());
                    already_in = true;
                }
            }
            if (!already_in) item_list.Add(item);

        } else {
            item_list.Add(item);
        }
        on_item_list_changed?.Invoke(this, EventArgs.Empty);
    }

    public bool remove_item(IItem item) {
        try {
            if (item.is_stackable())
            {
                IItem item_in_inventory = null;
                foreach (IItem it in item_list)
                {
                    if (it.get_item_type() == item.get_item_type())
                    {
                        it.set_amount(it.get_amount() - item.get_amount());
                        item_in_inventory = it;
                    }
                }
                if (item_in_inventory != null && item_in_inventory.get_amount() <= 0)
                {
                    return item_list.Remove(item_in_inventory);
                }
            }
            else
            {
                IItem item_in_inventory = null;
                foreach (IItem it in item_list)
                {
                    if (it.get_item_type() == item.get_item_type())
                    {
                        item_in_inventory = it;
                    }
                }
                if (item_in_inventory != null)
                {
                    return item_list.Remove(item_in_inventory);
                }
            }
            return false; // some amount of item still remains, or did not find it
        } finally {
            on_item_list_changed?.Invoke(this, EventArgs.Empty);
        }
    }

    public void use_item(IItem item) {
        use_item_action(item);
        on_item_use?.Invoke(this, new ItemUseEventArgs{ used_item = item});
    }

    public List<IItem> get_item_list() {
        return item_list;
    }
}
