using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private PlayerScript player;
    void Start()
    {
       
    }

    private void Update()
    {
        NPCInteractable closest_npc = player.getInteractableNPC(2f);
        if (closest_npc != null)
        {
            // change color when in range
            SpriteRenderer npc_renderer = closest_npc.GetComponent<SpriteRenderer>();
            npc_renderer.color = Color.blue;
            closest_npc.last_interactable = Time.time;
        }
        if (!dialoguePanel.activeInHierarchy && Input.GetKeyDown(KeyCode.E))
        {
            if (player.getInteractableNPC(2f) != null)
            {
                Debug.Log(closest_npc.npcName);
                Dialogue dialogue = dialoguePanel.GetComponentInChildren<Dialogue>();
                dialogue.setNpcName(closest_npc.npcName);
                show();
            }

        }
        else if (dialoguePanel.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape) || closest_npc == null) {
            hide();
        }
        
    }

    private void hide()
    {
        dialoguePanel.SetActive(false);
        player.isTalking = 1.0f;

    }

    private void show()
    {
        dialoguePanel.SetActive(true);
        player.isTalking = 0;
    }
}
