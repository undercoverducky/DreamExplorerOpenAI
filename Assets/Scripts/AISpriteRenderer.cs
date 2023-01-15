using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static UnityEditor.Progress;
using System;
using UnityEngine.Rendering.VirtualTexturing;

public class AISpriteRenderer : MonoBehaviour
{
    public static AISpriteRenderer Instance { get; set; }
    private OpenAIAPIClient ai_client = new OpenAIAPIClient(new HttpClient());

    public void Awake()
    {
        Instance = this;
    }
   
    public IEnumerator set_sprite_renderer(SpriteRenderer sr, string prompt)
    {
        System.Threading.Tasks.Task<string> url_task = ai_client.generate_image_async(prompt, 256, 256);
        //wait until url finished generating before revisitng this coroutine
        yield return new WaitUntil(() => url_task.IsCompleted);
        string url = url_task.Result;

        UnityWebRequest imgReq = UnityWebRequestTexture.GetTexture(url);
        yield return imgReq.SendWebRequest();

        if ((imgReq.result == UnityWebRequest.Result.ConnectionError) || (imgReq.result == UnityWebRequest.Result.ProtocolError))
        {
            Debug.LogError("Error setting item sprite: " + imgReq.error);
            
            sr.sprite = ItemAssets.Instance.error_sprite;
        }
        else
        {
            while (!imgReq.downloadHandler.isDone)
                Debug.Log("waiting");
            yield return null;
            //Texture2D texture = RemoveWhite(DownloadHandlerTexture.GetContent(imgReq));
            Texture2D texture = DownloadHandlerTexture.GetContent(imgReq);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 256f);
            sr.sprite = sprite;
        }
    }

    public IEnumerator get_sprite(string prompt, System.Action<Sprite> callback, int w = 256, int h = 256)
    {
        System.Threading.Tasks.Task<string> url_task = ai_client.generate_image_async(prompt, 256, 256);
        //wait until url finished generating before revisitng this coroutine
        yield return new WaitUntil(() => url_task.IsCompleted);
        string url = url_task.Result;

        UnityWebRequest imgReq = UnityWebRequestTexture.GetTexture(url);
        yield return imgReq.SendWebRequest();

        if ((imgReq.result == UnityWebRequest.Result.ConnectionError) || (imgReq.result == UnityWebRequest.Result.ProtocolError))
        {
            Debug.LogError("Error setting item sprite: " + imgReq.error);
            callback(ItemAssets.Instance.error_sprite);
        }
        else
        {
            while (!imgReq.downloadHandler.isDone)
                Debug.Log("waiting");
            yield return null;
            //Texture2D texture = RemoveWhite(DownloadHandlerTexture.GetContent(imgReq));
            Texture2D texture = DownloadHandlerTexture.GetContent(imgReq);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 256f);
            callback(sprite);
        }
    }

    Texture2D RemoveWhite(Texture2D imgs)
    {

        Color[] pixels = imgs.GetPixels(0, 0, imgs.width, imgs.height, 0);

        for (int p = 0; p < pixels.Length; p++)
        {
            if (Math.Min(pixels[p].r, Math.Min(pixels[p].g, pixels[p].b)) * 255f > 230f)
                pixels[p] = Color.clear;
        }

        Texture2D result_texture = new Texture2D(imgs.width, imgs.height, TextureFormat.ARGB32, false);
        result_texture.SetPixels(pixels);
        result_texture.Apply();
        return result_texture;
    }

    public IEnumerator set_item_sprite(PlayerItem item, ItemCreation ui_item_creation = null)
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
        }
        else
        {
            while (!imgReq.downloadHandler.isDone)
                Debug.Log("waiting");
            yield return null;
            //Texture2D texture = RemoveWhite(DownloadHandlerTexture.GetContent(imgReq));
            Texture2D texture = DownloadHandlerTexture.GetContent(imgReq);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 256f);
            item.set_sprite(sprite);
            //ui_item_creation.set_item_image();
        }
    }

    
}
