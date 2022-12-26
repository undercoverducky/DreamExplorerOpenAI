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

    void Start()
    {
        // Replace these with your own API key and model name
        string apiKey = API_KEY;
        string model = "image-alpha-001";

        // Set the prompt for the image generation
        string prompt = "A beautiful landscape with mountains in the background and a lake in the foreground.";

        // Set the image size
        int width = 512;
        int height = 512;

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

            Debug.Log($"Image generated at URL: {imageUrl}");
        }
        else
        {
            Debug.Log($"Error generating image: {response.ReasonPhrase}");
        }
        //StartCoroutine(GetImage());
    }

    IEnumerator GetImage()
    {
        WWWForm form = new WWWForm();
        form.AddField("prompt", "Player");
        form.AddField("n", 1);
        form.AddField("size", "128x128");
        // Create a UnityWebRequest and set the request method to POST
        UnityWebRequest www = UnityWebRequest.Post(API_URL, form);

        // Set the authorization header with the API key
        www.SetRequestHeader("Authorization", "Bearer " + API_KEY);

        // Set the content type header
        www.SetRequestHeader("Content-Type", "application/json");
        Debug.Log("Request: " + www.downloadHandler.text);
        Debug.Log("Response: " + www.uploadHandler.contentType);
        // Send the request
        yield return www.SendWebRequest();

        // Check for errors
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            // do stuff
        }
        
    }
}

