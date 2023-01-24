using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Player player;
    void Start()
    {
       
    }

    private void Update()
    {
        NPCInteractable closest_npc = player.getInteractableNPC(2f);
        if (closest_npc != null) // interactable!
        {
            closest_npc.interactable();
        }
        // player chooses to interact

        if (!dialoguePanel.activeInHierarchy && Input.GetKeyDown(KeyCode.E) && player.actions_enabled)
        {
            if (closest_npc != null)
            {
                player.disable_player_action();
                /*Debug.Log(closest_npc.npcName);
                Dialogue dialogue = dialoguePanel.GetComponentInChildren<Dialogue>();
                dialogue.set_npc(closest_npc);
                show();
                dialogue.begin_dialogue();
                */
                NodeParser node_parser = dialoguePanel.GetComponentInChildren<NodeParser>();
                node_parser.set_npc(closest_npc);
                show();
                node_parser.begin_parsing_graph();

            }

        }
        else if ((dialoguePanel.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape)) || closest_npc == null) {
            player.enable_player_action();
            hide();
        }
        
    }

    private void hide()
    {
        dialoguePanel.SetActive(false);
        player.is_talking = 1.0f;
    }

    private void show()
    {
        dialoguePanel.SetActive(true);
        player.is_talking = 0.0f;
    }
}
