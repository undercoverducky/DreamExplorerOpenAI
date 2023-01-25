using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using XNode;

using System.Threading.Tasks;

public class UseInventoryItemNode : CoreNode {
    [Input] public int entry;
    [Output] public int valid;
    [Output] public int invalid;
    public ItemType[] valid_items;
    public string verb;
    public string property;

    private OpenAIAPIClient ai_client = new OpenAIAPIClient(new HttpClient());


    public override string get_string()
    {
        return "UseInventoryItemNode/";
    }

    public async Task<bool> use_player_item(PlayerItem item) {
        return await ai_client.ask_yes_no(item.description, verb, property);
    }

    public bool use_game_item(GItem item)
    {
        foreach (ItemType t in valid_items)
        {
            if (item.get_item_type() == t)
            {
                return true;
            }
        }
        return false;
    }

}