using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class EndNode : CoreNode {

    [Input] public int entry;
    public override string get_string()
    {
        return "End";
    }
}