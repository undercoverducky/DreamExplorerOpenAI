using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    public string npcName;
    SpriteRenderer npc_SpriteRenderer;
    public float last_interactable = 0f;
    public Color default_color;
    public bool description_only = false;

    private Color interactable_color = Color.red;
   
    void Start()
    {

    }

    void Update()
    {
        if (last_interactable == 0f || Time.time - last_interactable > .25f)
        {
            GetComponent<SpriteRenderer>().color = default_color;
        }

    }

    public void interactable()
    {
        last_interactable = Time.time;
        GetComponent<SpriteRenderer>().color = interactable_color;
    }

}
