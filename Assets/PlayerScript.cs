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

    public float speed = 10.0f; // Speed of movement
    void Update()
    {
        // Get input from the 'w', 'a', 's', and 'd' keys
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Use the input to move the game object
        transform.Translate(new Vector2(horizontalInput, verticalInput) * speed * Time.deltaTime);
    }

    async void Start()
    {
        StartCoroutine(recievePlayerSprite());
    }

    IEnumerator recievePlayerSprite()
    {

        // Set the prompt for the image generation
        string prompt = "High quality high definition dark souls boss with no background";

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

            if (imgReq.isNetworkError || imgReq.isHttpError)
            {
                Debug.LogError(imgReq.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(imgReq);
                GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            }
        }
        else
        {
            Debug.Log($"Warning: Error generating image: {response.ReasonPhrase}");
        }
    }
}

