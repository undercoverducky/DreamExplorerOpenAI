using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class AIDialogueNode : CoreNode {

    [Input] public int entry;
    [Output] public int exit;
    public string speaker_name;
    public Sprite sprite;

    public override string get_string()
    {
        return "AIDialogueNode/" + speaker_name;
    }

    public override Sprite get_sprite()
    {
        return sprite;
    }
}