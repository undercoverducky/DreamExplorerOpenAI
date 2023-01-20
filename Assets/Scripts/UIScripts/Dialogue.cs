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
    public string npcName = "";
    OpenAIAPIClient ai_client = new OpenAIAPIClient(new HttpClient());

    private string npcPromptSetting = "";
    private NPCInteractable npc;

    public void readStringInput(string s)
    {
        Debug.Log($"Player wrote: {s}");
        if (!isActiveAndEnabled) //Dialogue disabled with leftover text
        {
            return;
        }
        if (!npc.description_only) // not scripted dialogue
        {
            if (textComponent.text == string.Empty || textComponent.text.Split("\n").Length % 2 == 1) // player's turn
            {
                textComponent.text += "YOU -  " + s + "\n" + npcName.ToUpper() + " - ";
                playerInputField.text = "";
                StopAllCoroutines();
                Debug.Log("typing ai response: ");
                StartCoroutine(typeLineAI()); //Begin AI response
            }
        }
        else {
            textComponent.text = npc.npcName.ToUpper() + " - " + npcPromptSetting;
        }
        
        
    }
    void OnDisable()
    {
        playerInputField.text = "";
        textComponent.text = string.Empty;
        StopAllCoroutines();
    }

    private void set_npc_name(string n)
    {
        this.npcName = n;
    }



    private void set_npc_prompt(string npcName)
    {
        if (!npc.description_only)
        {
            npcPromptSetting = getNPCPromptSetting("Assets/InteractableText/Characters/" + npcName.ToLower() + ".txt", Encoding.UTF8);
        }
        else
        {
            npcPromptSetting = getNPCPromptSetting("Assets/InteractableText/Objects/" + npcName.ToLower() + ".txt", Encoding.UTF8);
        }
       
    }

    public void set_npc(NPCInteractable npc)
    {
        this.npc = npc;
        set_npc_name(npc.npcName);
        set_npc_prompt(npc.npcName);
        
    }

    public void begin_dialogue() {
        StartCoroutine(typeLine());
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
            if (!npc.description_only)
            {
                result = streamReader.ReadToEnd();
            }
            else {
                result = streamReader.ReadLine();
                result = streamReader.ReadLine();
            }
            
        }
        return result;
    }

    // Start is called before the first frame update
    void Start()
    {
        textComponent.text = string.Empty;
        //npcPromptSetting = getNPCPromptSetting("Assets/Character/Prompts/" + npcName.ToLower() + ".txt", Encoding.UTF8);
        
    }

    private string generatePrompt() {
        string[] lines = textComponent.text.Split("\n");
        string prompt = npcPromptSetting;
        for (int i = 0; i < lines.Length; i++) {
            prompt += lines[i].Replace("\"", "\\\"") + "\\n";
        }
       
        return prompt;
    }

    IEnumerator typeLineAI()
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

    IEnumerator typeLine()
    {
        string line = npc.npcName.ToUpper() + " - " + npcPromptSetting;
       
        string[] words = line.Split();
        Debug.Log(words[0]);
        textComponent.text = line;

        /*for (int i = 0; i < words.Length; i++)
        {
            textComponent.text += (i == words.Length - 1) ? words[i] + "\n" : words[i] + " ";
            yield return new WaitForSeconds(textSpeed);
        }*/
        yield return new WaitForSeconds(textSpeed);
    }

}
