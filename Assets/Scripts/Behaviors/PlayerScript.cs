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
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript Instance { get; set; }
    [SerializeField] private UI_Inventory ui_inventory;
    [SerializeField] private ItemCreation ui_item_creation;
    [SerializeField] private NodeParser dialogue_panel;

    public string player_img_prompt;
    public float is_talking = 1.0f;
    public float is_in_menu = 1.0f;
    public bool actions_enabled = true;

    private Inventory inventory;
    private SpriteRenderer sprite_renderer;

    private void Awake()
    {
        if (Instance != null) {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        inventory = new Inventory(use_item, 10);
        GameObject.DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        ui_inventory.set_inventory(inventory);
        ui_item_creation.set_inventory(inventory);
        dialogue_panel.set_inventory(inventory);
        inventory.add_item(new GItem(ItemType.Lucidator, 3));
    }

    // movement
    void Update()
    {
        check_input();
    }


    private void check_input()
    {

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!ui_inventory.on)
            {
                ui_inventory.show();
            }
            else
            {
                ui_inventory.hide();
            }
        }
        else if (Input.GetMouseButtonDown(0)) {
            SceneManager.LoadScene("Scene2");
        }
    }

    private void use_item(IItem item)
    {
        switch (item.get_item_type())
        {
            case ItemType.Lucidator:
                Debug.Log("clicked Lucidator");
                if (!ui_item_creation.on)
                {
                    ui_item_creation.Show();
                }
                break;
            default: break;
        }
    }

    public void disable_player_action()
    {
        actions_enabled = false;
    }
    public void enable_player_action()
    {
        actions_enabled = true;
    }
}

