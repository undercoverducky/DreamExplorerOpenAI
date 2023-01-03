using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Windows;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{

    public TextMeshProUGUI textComponent;
    public float textSpeed;
    public TMP_InputField playerInputField;
    public string npcPromptSetting = "Marv is a helpful AI chat\\n";

    public void readStringInput(string s)
    {
        Debug.Log($"{s}");

        if (textComponent.text == string.Empty || textComponent.text.Split("\n").Length % 2 == 1) // player's turn
        {
            textComponent.text += "You: " + s + "\n" + "Marv: ";
            playerInputField.text = "";
            StopAllCoroutines();
            Debug.Log("typing ai response: ");
            StartCoroutine(typeLine()); //Begin AI response
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        textComponent.text = string.Empty;
    }

    // Update is called once per frame
    void Update()
    {
      
    }


    private string generatePrompt() {
        string[] lines = textComponent.text.Split("\n");
        string prompt = npcPromptSetting;
        for (int i = 0; i < lines.Length; i++) {
            prompt += lines[i] + "\\n";
        }
       
        return prompt;
    }

    IEnumerator typeLine()
    {
        string prompt = generatePrompt();
        Debug.Log("sending prompt: \n" + prompt);
        OpenAIAPIClient ai_client = new OpenAIAPIClient();
        System.Threading.Tasks.Task<string> line_task = ai_client.generate_text_async(prompt,0,30) ;
        yield return new WaitUntil(() => line_task.IsCompleted);
        string line = line_task.Result;
        //string[] words = lines[index].Split();
        string[] words = line.Split();

        for (int i = 0; i < words.Length; i++)
        {
            textComponent.text += (i == words.Length - 1) ? words[i] + "\n" : words[i] + " ";
            yield return new WaitForSeconds(textSpeed);
        }
    }

}
