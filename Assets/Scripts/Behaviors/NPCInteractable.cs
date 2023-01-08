using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    public string npcName;
    SpriteRenderer npc_SpriteRenderer;
    public float last_interactable = 0f; 

   
    void Start()
    {

    }

    void Update()
    {
        if (last_interactable == 0f || Time.time - last_interactable > .25f)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
        }

    }

}
