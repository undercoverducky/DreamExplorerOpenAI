﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.JsonUtility;
using System.Web;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.VersionControl;
using System.Net;
using static UnityEditor.Progress;

public class Player : MonoBehaviour
{
    [SerializeField] private UI_Inventory ui_inventory;

    public float moveSpeed = 5.0f; // speed of movement
    public string player_img_prompt;
    public float is_talking = 1.0f;
    public float is_in_menu = 1.0f;

    private Inventory inventory;
    private SpriteRenderer sprite_renderer;

    private void Awake() {
        inventory = new Inventory(use_item, 10);
        
    }

    private void Start() {
        sprite_renderer = GetComponent<SpriteRenderer>();
        sprite_renderer.sprite = ItemAssets.Instance.loading_sprite;
        
        StartCoroutine(AISpriteRenderer.Instance.set_sprite_renderer(sprite_renderer, player_img_prompt));
        ui_inventory.set_inventory(inventory);
        inventory.add_item(new GItem(ItemType.Lucidator, 3));
    }

    // movement
    void Update()  {
        check_input();
    }

    // returns the closest interactable npc
    public NPCInteractable getInteractableNPC(float with_dist) {
        List<NPCInteractable> interactable_list = new List<NPCInteractable>();
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, with_dist);
        foreach (Collider2D coll in objects) {
            if (coll.TryGetComponent(out NPCInteractable n)) {
                interactable_list.Add(n);
            }
        }
        NPCInteractable closest_npc = null;
        foreach (NPCInteractable n in interactable_list) {
            if (closest_npc == null) {
                closest_npc = n;
            }
            else {
                if ((transform.position - n.transform.position).sqrMagnitude < (transform.position - closest_npc.transform.position).sqrMagnitude) {
                    closest_npc = n;
                }
            }
        }
        return closest_npc;

    }

    // destroys in game items we collide with
    private void OnTriggerEnter2D(Collider2D other) {
        ItemWorld itemWorld = other.GetComponent<ItemWorld>();
        if (itemWorld != null) {
            inventory.add_item(itemWorld.get_item());
            itemWorld.destroy_item();
        }
        
    }

    private void check_input() {
        // movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        GetComponent<Rigidbody2D>().velocity = new Vector2(horizontalInput, verticalInput) * moveSpeed * is_talking * is_in_menu;
        //transform.Translate(new Vector2(horizontalInput, verticalInput) * moveSpeed * Time.deltaTime * isTalking);
        if (Input.GetKeyDown(KeyCode.I)) {
            if (is_in_menu == 1.0f) {
                ui_inventory.gameObject.SetActive(true);
                is_in_menu = 0;
            } else {
                ui_inventory.gameObject.SetActive(false);
                is_in_menu = 1.0f;
            }
            
        }
        
    }

    private void use_item(IItem item) {
        switch (item.get_item_type()) {
            case ItemType.Lucidator:
                Debug.Log("clicked Lucidator");
                
                inventory.remove_item(new GItem(ItemType.Lucidator, 1));

                break;
            default: break;
        }
    }
}

