using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Windows;
using UnityEngine.UI;
using System.Net.Http;
using System.IO;
using System;
using System.Text;

public class Dialogue : MonoBehaviour
{

    public TextMeshProUGUI textComponent;
    public float textSpeed;
    public TMP_InputField playerInputField;
    private string npcPromptSetting = "";
    public string npcName = "";
    OpenAIAPIClient ai_client = new OpenAIAPIClient(new HttpClient());


    public void readStringInput(string s)
    {
        Debug.Log($"Player wrote: {s}");
        if (!isActiveAndEnabled) //Dialogue disabled with leftover text
        {
            return;
        }
        if (textComponent.text == string.Empty || textComponent.text.Split("\n").Length % 2 == 1) // player's turn
        {
            textComponent.text += "YOU -  " + s + "\n" + npcName.ToUpper() + " - ";
            playerInputField.text = "";
            StopAllCoroutines();
            Debug.Log("typing ai response: ");
            StartCoroutine(typeLine()); //Begin AI response
        }
        
    }
    void OnDisable()
    {
        playerInputField.text = "";
        textComponent.text = string.Empty;
        StopAllCoroutines();
    }

    public void setNpcName(string n)
    {
        npcName = n;
        npcPromptSetting = getNPCPromptSetting("Assets/Character/Prompts/" + npcName.ToLower() + ".txt", Encoding.UTF8);
    }

    string getNPCPromptSetting(string file_path, Encoding encoding)
    {
        if (file_path == null)
        {
            throw new ArgumentNullException("path");
        }
        if (file_path.Length == 0)
        {
            throw new ArgumentException("empty path");
        }
        string result;
        using (StreamReader streamReader = new StreamReader(file_path, encoding))
        {
            result = streamReader.ReadToEnd();
        }
        return result;
    }

    // Start is called before the first frame update
    void Start()
    {
        textComponent.text = string.Empty;
        npcPromptSetting = getNPCPromptSetting("Assets/Character/Prompts/" + npcName.ToLower() + ".txt", Encoding.UTF8);

    }

    private string generatePrompt() {
        string[] lines = textComponent.text.Split("\n");
        string prompt = npcPromptSetting;
        for (int i = 0; i < lines.Length; i++) {
            prompt += lines[i].Replace("\"", "\\\"") + "\\n";
        }
       
        return prompt;
    }

    IEnumerator typeLine()
    {
        string prompt = generatePrompt();
        Debug.Log("sending prompt: \n" + prompt);
        System.Threading.Tasks.Task<string> line_task = ai_client.generate_text_async(prompt, frequency_penalty:2f, presence_penalty:1.0f) ;
        yield return new WaitUntil(() => line_task.IsCompleted);
        string line = line_task.Result;
        string[] words = line.Split();

        for (int i = 0; i < words.Length; i++)
        {
            textComponent.text += (i == words.Length - 1) ? words[i] + "\n" : words[i] + " ";
            yield return new WaitForSeconds(textSpeed);
        }
    }

}
