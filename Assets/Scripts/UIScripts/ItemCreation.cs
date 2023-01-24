using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;
using UnityEngine.UI;
using UnityEngine.Windows;

public class ItemCreation : MonoBehaviour
{
    public TMP_InputField playerInputField;
    public bool on = false;
    public Player player;

    public TextMeshProUGUI awaiting_text;
    private Transform item_image;
    private Inventory inventory;
    private bool one_time_use = true;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Show() {
        this.gameObject.SetActive(true);
        awaiting_text.text = "Awaiting Lucidation Spark...";
        one_time_use = true;
        on = true;
        player.disable_player_action();
        StartCoroutine(loading_dots());
    }

    public void Hide() {
        item_image.GetComponent<Image>().sprite = null;
        on = false;
        player.enable_player_action();
        StopAllCoroutines();
        playerInputField.text = string.Empty;
        this.gameObject.SetActive(false);
    }

    public void set_inventory(Inventory inventory) {
        this.inventory = inventory;
        if (awaiting_text == null || item_image == null)
        {
            
            item_image = transform.Find("itemImage"); // defaults to loading sprite
        }
        
    }

    public void readStringInput(string s) {
        
        if (one_time_use) {
            Debug.Log($"Player wrote: {s}");
            one_time_use = false;
            string prompt = s;
            PlayerItem new_item = new PlayerItem(prompt);
            Image image = item_image.GetComponent<Image>();
            image.sprite = ItemAssets.Instance.loading_sprite;
            new_item.on_item_sprite_changed += set_item_image;
            StartCoroutine(AISpriteRenderer.Instance.set_item_sprite(new_item));
        }
    }
    

    private void set_item_image(object sender, System.EventArgs e) {
        PlayerItem new_item = (PlayerItem)sender;
        if (new_item != null)
        {
            
            Image image = item_image.GetComponent<Image>();
            image.sprite = new_item.get_sprite();
            awaiting_text.text = "Successful Lucidation!";
            StartCoroutine(add_item_to_inventory(new_item));
            
        }
    }

    public IEnumerator loading_dots() {
        string text = awaiting_text.text;
        while (text.ToCharArray()[0] == 'A')
        {
            yield return new WaitForSecondsRealtime(1.5f);
            string last_three = text.Substring(text.Length - 3);
            if (last_three.Equals("...")) {
                awaiting_text.text = "Awaiting Lucidation Spark";
            } else {
                awaiting_text.text += ".";
            }
            text = awaiting_text.text;
        }
        awaiting_text.text = "Successful Lucidation!";

    }

    public IEnumerator add_item_to_inventory(PlayerItem item)
    {
        yield return new WaitForSecondsRealtime(5f);
        // item creation confetti and flashing
        inventory.add_item(item);
        inventory.remove_item(new GItem(ItemType.Lucidator, 1));
        Hide();
        
    }
}
