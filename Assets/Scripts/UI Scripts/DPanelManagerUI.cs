using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DPanelManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject containerGameObject;
    [SerializeField] private PlayerScript player;
    void Start()
    {
       
    }

    private void Update()
    {
        NPCInteractable closest_npc = player.getInteractableNPC(2f);
        if (!containerGameObject.activeInHierarchy && Input.GetKeyDown(KeyCode.E))
        {
            if (player.getInteractableNPC(2f) != null)
            {
                Debug.Log(closest_npc.npcName);
                Dialogue dialogue = containerGameObject.GetComponentInChildren<Dialogue>();
                dialogue.setNpcName(closest_npc.npcName);
                show();
            }

        }
        else if (containerGameObject.activeInHierarchy && Input.GetKeyDown(KeyCode.E) || closest_npc == null) {
            hide();
        }
        
    }

    private void hide()
    {
        containerGameObject.SetActive(false);
    }

    private void show()
    {
        containerGameObject.SetActive(true);
    }
}
