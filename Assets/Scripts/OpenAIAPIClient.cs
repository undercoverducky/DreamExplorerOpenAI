using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;


// Reference API: https://beta.openai.com/docs/api-reference/introduction
public class OpenAIAPIClient
{

    public class UrlObj
    {
        public string url { get; set; }
    }
    public class ImgResponse
    {
        public string created { get; set; }
        public UrlObj[] data { get; set; }
    }

    public class TextResponse
    {
        public string created { get; set; }
        public UrlObj[] data { get; set; }
    }

    private const string API_KEY = "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";
    private const string IMG_GEN_API_URL = "https://api.openai.com/v1/images/generations";
    private const string TXT_GEN_API_URL = "https://api.openai.com/v1/completions";
    private const string img_model = "image-alpha-001";
    private const string text_model = "text-davinci-003";

   
    public string generate_image(string prompt, int w=256, int h=256)
    {
        int width = w;
        int height = h;
        string responseFormat = "url";

        //TODO should not create http client every time methos is called. Consider using static singleton httpclient. 
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {API_KEY}");
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, IMG_GEN_API_URL);
        request.Content = new StringContent($"{{\"model\": \"{img_model}\", \"prompt\": \"{prompt}\", \"size\": \"{width}x{height}\", \"response_format\": \"{responseFormat}\"}}", Encoding.UTF8, "application/json");

        HttpResponseMessage response = client.SendAsync(request).Result;

        if (response.IsSuccessStatusCode)
        {
            string imgJSON = response.Content.ReadAsStringAsync().Result;
            ImgResponse imgResp = JsonConvert.DeserializeObject<ImgResponse>(imgJSON);

            return imgResp.data[0].url;
        }
        else
        {
            Debug.Log($"Warning: Error generating image: {response.ReasonPhrase}");
            return $"Warning: Error generating image: {response.ReasonPhrase}";
        }
    }

    //Implement a async version of generate_image. Experimentally, request takes about 4-8 s
    public async Task<string> generate_image_async(string prompt, int w = 256, int h = 256)
    {
        int width = w;
        int height = h;
        string responseFormat = "url";

        //TODO should not create http client every time methos is called. Consider using static singleton httpclient.
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {API_KEY}");
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, IMG_GEN_API_URL);
        request.Content = new StringContent($"{{\"model\": \"{img_model}\", \"prompt\": \"{prompt}\", \"size\": \"{width}x{height}\", \"response_format\": \"{responseFormat}\"}}", Encoding.UTF8, "application/json");

        // Using async resturns control to caller, resumes once request is done 
        HttpResponseMessage response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            string imgJSON = await response.Content.ReadAsStringAsync();
            ImgResponse imgResp = JsonConvert.DeserializeObject<ImgResponse>(imgJSON);

            return imgResp.data[0].url;
        }
        else
        {
            Debug.Log($"Warning: Error generating image: {response.ReasonPhrase}");
            return $"Warning: Error generating image: {response.ReasonPhrase}";
        }
    }

    public async Task<string> generate_text_async(string prompt, int temp=0, int max_tok=7)
    {
        //TODO should not create http client every time methos is called. Consider using static singleton httpclient.
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {API_KEY}");
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, TXT_GEN_API_URL);
        request.Content = new StringContent($"{{\"model\": \"{text_model}\", \"prompt\": \"{prompt}\", \"temperature\": {temp}, \"max_tokens\": {max_tok}}}", Encoding.UTF8, "application/json");

        // Using async resturns control to caller, resumes once request is done 
        HttpResponseMessage response = await client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            string textJSON = await response.Content.ReadAsStringAsync();
            Debug.Log($"Raw response: {textJSON}");
            TextResponse imgResp = JsonConvert.DeserializeObject<TextResponse>(textJSON);
            return imgResp.data[0].url; //change
        }
        else
        {
            Debug.Log($"Warning: Error generating text: {response}");
            return $"Warning: Error generating text: {response.ReasonPhrase}";
        }
        
    }


}
