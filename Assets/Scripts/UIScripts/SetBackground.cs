using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using UnityEngine.Networking;

public class SetBackground : MonoBehaviour
{
    // Start is called before the first frame update
    public string background_img_prompt;
    OpenAIAPIClient ai_client = new OpenAIAPIClient(new HttpClient());
    void Start()
    {
        StartCoroutine(setBackgroundSprite(background_img_prompt));
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator setBackgroundSprite(string prompt)
    {

        const int width = 256;
        const int height = 256;

        System.Threading.Tasks.Task<string> url_task = ai_client.generate_image_async(prompt, width, height);
        //wait until url finished generating before revisitng this coroutine
        yield return new WaitUntil(() => url_task.IsCompleted);
        string url = url_task.Result;

        // non async method. This blocks execution, adds 4 s before game view gets rendered. Not desired
        // string url = ai_client.generate_image(prompt, width, height)

        UnityWebRequest imgReq = UnityWebRequestTexture.GetTexture(url);
        yield return imgReq.SendWebRequest();

        if ((imgReq.result == UnityWebRequest.Result.ConnectionError) || (imgReq.result == UnityWebRequest.Result.ProtocolError))
        {
            Debug.LogError(imgReq.error);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(imgReq);
            GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Tiled;
            GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 64.0f, 0, SpriteMeshType.FullRect);
            GetComponent<SpriteRenderer>().size = new Vector2(100f, 100f);
        }

    }

}
