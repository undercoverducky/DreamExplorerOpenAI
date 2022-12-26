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
    // Replace API_KEY with your actual API key
    private string API_KEY = "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";
    private string API_URL = "https://api.openai.com/v1/images/generations";

    public float speed = 10.0f; // Speed of movement

    void Start()
    {
        
        StartCoroutine(GetImage());
    }

    void Update()
    {
        // Get input from the 'w', 'a', 's', and 'd' keys
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Use the input to move the game object
        transform.Translate(new Vector2(horizontalInput, verticalInput) * speed * Time.deltaTime);
    }

    IEnumerator GetImage()
    {
        // Replace these with your own API key and model name
        string apiKey = API_KEY;
        string model = "image-alpha-001";

        // Set the prompt for the image generation
        string prompt = "Pixel RPG Top-Down Perspectiv Character";

        // Set the image size
        int width = 256;
        int height = 256;

        // Set the response format to "url" so that the API returns a URL to the generated image
        string responseFormat = "url";

        // Set up the HTTP client
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        // Set up the HTTP POST request
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"https://api.openai.com/v1/images/generations");
        request.Content = new StringContent($"{{\"model\": \"{model}\", \"prompt\": \"{prompt}\", \"size\": \"{width}x{height}\", \"response_format\": \"{responseFormat}\"}}", Encoding.UTF8, "application/json");

        // Send the request and get the response
        HttpResponseMessage response = client.SendAsync(request).Result;

        // Check if the request was successful
        if (response.IsSuccessStatusCode)
        {
            // Get the URL of the generated image from the response
            string imageUrl = response.Content.ReadAsStringAsync().Result;
            string[] splitresp = imageUrl.Split(':');
           
            string url = "https:" + splitresp[4].Split('\"')[0];
            Debug.Log($"Image generated at URL: {url}");
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();

            // Check for errors
            if (www.isHttpError || www.isNetworkError)
            {
                Debug.LogError(www.error);
                yield break;
            }

            // Load the texture data into a new Texture2D
            Texture2D texture = DownloadHandlerTexture.GetContent(www);

            // Create a new sprite from the texture
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

            // Assign the sprite to the SpriteRenderer component
            GetComponent<SpriteRenderer>().sprite = sprite;

        }
        else
        {
            Debug.Log($"Error generating image: {response.ReasonPhrase}");
        }

    }
}

