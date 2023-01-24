using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class StartNode : CoreNode {

    [Output] public int exit;
    public override string get_string()
    {
        return "Start";
    }
}