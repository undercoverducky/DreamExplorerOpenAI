using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    private Inventory inventory;
    private Transform itemslot_container;
    private Transform itemslot_template;
    private OpenAIAPIClient ai_client = new OpenAIAPIClient(new HttpClient());

    private float itemslot_cell_size = 128f;
    private int max_cols = 4; 

    public void set_inventory(Inventory inventory)
    {
        this.inventory = inventory;
        if (itemslot_container == null || itemslot_template == null)
        {
            itemslot_container = transform.Find("itemSlotContainer");
            itemslot_template = itemslot_container.Find("itemSlotTemplate");
        }
        refresh_inventory_items();
    }

    public void add_item(IItem item) 
    {
        inventory.add_item(item);
        int x = (inventory.get_item_list().Count-1) % max_cols;
        int y = (inventory.get_item_list().Count-1) / max_cols;

        RectTransform itemslot_rect_transform = Instantiate(itemslot_template, itemslot_container).GetComponent<RectTransform>();
        itemslot_rect_transform.gameObject.SetActive(true);
        itemslot_rect_transform.anchoredPosition = new Vector2(x * itemslot_cell_size + 10, -y * itemslot_cell_size);

        Image image = itemslot_rect_transform.Find("Image").GetComponent<Image>();
        if (item.get_item_type() == ItemType.Player_Generated && item.get_sprite() == null)
        {
            Debug.Log("started item coroutine");
            StartCoroutine(set_item_sprite((PlayerItem)item, image));
        }
        else
        {
            image.sprite = item.get_sprite();
        }
    }

    private void refresh_inventory_items()
    {
        int x = 0;
        int y = 0;
        float itemslot_cell_size = 150f;
        foreach (IItem item in inventory.get_item_list())
        {
            RectTransform itemslot_rect_transform = Instantiate(itemslot_template, itemslot_container).GetComponent<RectTransform>();
            itemslot_rect_transform.gameObject.SetActive(true);
            itemslot_rect_transform.anchoredPosition = new Vector2(x * itemslot_cell_size+50, y * itemslot_cell_size);
            Image image = itemslot_rect_transform.Find("Image").GetComponent<Image>();
            if (item.get_item_type() == ItemType.Player_Generated && item.get_sprite() == null)
            {
                Debug.Log("started item coroutine");
                StartCoroutine(set_item_sprite((PlayerItem)item, image));
            }
            else {
                image.sprite = item.get_sprite();
            }
            x++;
            if (x > max_cols)
            {
                x = 0;
                y++;
            }
        }
    }

    IEnumerator set_item_sprite(PlayerItem item, Image image)
    {
        System.Threading.Tasks.Task<string> url_task = ai_client.generate_image_async(item.description, 256, 256);
        //wait until url finished generating before revisitng this coroutine
        yield return new WaitUntil(() => url_task.IsCompleted);
        string url = url_task.Result;

        UnityWebRequest imgReq = UnityWebRequestTexture.GetTexture(url);
        yield return imgReq.SendWebRequest();

        if ((imgReq.result == UnityWebRequest.Result.ConnectionError) || (imgReq.result == UnityWebRequest.Result.ProtocolError))
        {
            Debug.LogError("Error setting item sprite: " + imgReq.error);
            item.set_sprite(ItemAssets.Instance.error_sprite);
            image.sprite = ItemAssets.Instance.error_sprite;
        }
        else
        {
            while (!imgReq.downloadHandler.isDone)
                Debug.Log("waiting");
                yield return null;
            Texture2D texture = DownloadHandlerTexture.GetContent(imgReq);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 256f);
            item.set_sprite(sprite);
            image.sprite = sprite;
        }
    }

}
