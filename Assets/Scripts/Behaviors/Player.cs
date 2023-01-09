using System.Collections;
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
    public float isTalking = 1.0f;

    private Inventory inventory;
    private SpriteRenderer sprite_renderer;

    private void Awake()
    {
        inventory = new Inventory();
        
    }

    private void Start()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
        sprite_renderer.sprite = ItemAssets.Instance.loading_sprite;
        
        StartCoroutine(AISpriteRenderer.Instance.set_sprite_renderer(sprite_renderer, player_img_prompt));
        ui_inventory.set_inventory(inventory);

        
    }

    void Update() 
    {
        checkInput();

    }

    public NPCInteractable getInteractableNPC(float with_dist)
    {
        List<NPCInteractable> interactable_list = new List<NPCInteractable>();
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, with_dist);
        foreach (Collider2D coll in objects)
        {
            if (coll.TryGetComponent(out NPCInteractable n))
            {
                interactable_list.Add(n);
            }
        }
        NPCInteractable closest_npc = null;
        foreach (NPCInteractable n in interactable_list)
        {
            if (closest_npc == null)
            {
                closest_npc = n;
            }
            else
            {
                if ((transform.position - n.transform.position).sqrMagnitude < (transform.position - closest_npc.transform.position).sqrMagnitude)
                {
                    closest_npc = n;
                }
            }
        }
        return closest_npc;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ItemWorld itemWorld = other.GetComponent<ItemWorld>();
        if (itemWorld != null) {
            inventory.add_item(itemWorld.get_item());
            itemWorld.destroy_item();
        }
        
    }

    void checkInput()
    {
        // movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        GetComponent<Rigidbody2D>().velocity = new Vector2(horizontalInput, verticalInput) * moveSpeed * isTalking;
        //transform.Translate(new Vector2(horizontalInput, verticalInput) * moveSpeed * Time.deltaTime * isTalking);
        if (Input.GetKeyDown(KeyCode.Q))
        {
            inventory.add_item(new PlayerItem(prompt: "cartoon sword clipart"));
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            inventory.add_item(new GItem(ItemType.Coin, 1));
        }
        //...
    }
}

