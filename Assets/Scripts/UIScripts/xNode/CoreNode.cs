using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class CoreNode : Node {

	// Use this for initialization
	protected override void Init() {
		base.Init();

	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		return null; // Replace this
	}

	public virtual string get_string() {
		return null;
	}

    public virtual Sprite get_sprite()
    {
        return null;
    }

	public virtual void set_sprite(Sprite s) {
		return;
	}
}