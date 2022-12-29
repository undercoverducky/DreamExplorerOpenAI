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

        // Set the image size
        const int width = 256;
        const int height = 256;

        // Set the response format to "url" so that the API returns a URL to the generated image
        string responseFormat = "url";

        // Set up the HTTP client
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {API_KEY}");

        // Set up the HTTP POST request
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, API_URL);
        request.Content = new StringContent($"{{\"model\": \"{img_model}\", \"prompt\": \"{prompt}\", \"size\": \"{width}x{height}\", \"response_format\": \"{responseFormat}\"}}", Encoding.UTF8, "application/json");

        // Send the request and get the response
        HttpResponseMessage response = client.SendAsync(request).Result;

        // Check if the request was successful
        if (response.IsSuccessStatusCode)
        {
            // Get the URL of the generated image from the response
            string imgJSON = response.Content.ReadAsStringAsync().Result;
            imgResponse imgResp = JsonConvert.DeserializeObject<imgResponse>(imgJSON);

            UnityWebRequest imgReq = UnityWebRequestTexture.GetTexture(imgResp.data[0].url);
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
        else
        {
            Debug.Log($"Warning: Error generating image: {response.ReasonPhrase}");
        }
    }
}

