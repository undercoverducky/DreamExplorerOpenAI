using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class PlayerChoiceNode : CoreNode {
    // figure out if entries and exits can have mutliple outs and ins
    [Input] public int entry;
    
    public string[] player_choices;

    public override string get_string()
    {
        string result = "PlayerChoiceNode/";
        int count = 0;
        foreach (string choice in player_choices)
        {
            result += choice + "/";
            count++;
            if (count >= get_num_choices())
            {
                break;
            }
        }
        return result;
    }

    public virtual int get_num_choices() {
        return 0; 
    }
}