using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueNode : CoreNode {

	[Input] public int entry;
	[Output] public int exit;
	public string speaker_name;
	public string dialogue_line;
	public Sprite sprite;

    public override string get_string()
    {
        return "DialogueNode/" + speaker_name + "/" + dialogue_line;
    }

	public override Sprite get_sprite() {
		return sprite;
	}

    public override void set_sprite(Sprite s)
    {
        sprite = s;
    }
}