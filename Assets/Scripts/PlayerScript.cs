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

public class PlayerScript : MonoBehaviour
{
    public class urlObj
    {
        public string url { get; set; }
    }
    public class imgResponse
    {
        public string created { get; set; }
        public urlObj[] data { get; set; }
    }

    private const string API_KEY = "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";
    private const string API_URL = "https://api.openai.com/v1/images/generations";
    private const string img_model = "image-alpha-001";

    public const float speed = 5.0f; // speed of movement


    void Update()
    {
        checkInput();
    }

    void Start()
    {
        
        StartCoroutine(setPlayerSprite());
    
    }

    void checkInput()
    {
        // movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        transform.Translate(new Vector2(horizontalInput, verticalInput) * speed * Time.deltaTime);

        //...
    }
    
    IEnumerator setPlayerSprite(string prompt = "High quality high definition dark souls boss with no background")
    {

        const int width = 256;
        const int height = 256;

        OpenAIAPIClient ai_client = new OpenAIAPIClient();

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
            GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

    }
}
