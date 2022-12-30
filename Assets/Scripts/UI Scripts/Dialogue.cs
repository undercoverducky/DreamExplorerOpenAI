using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI textComponent;
    public string[]  lines;
    public float textSpeed;
    private int index;

    public string npcPromptSetting;
    private string[] dialogues;


    void Start()
    {
        textComponent.text = string.Empty;
        startDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index])
            {
                nextLine();
            }
            else {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    void startDialogue() {
        index = 0;
        StartCoroutine(typeLine());
    }

    IEnumerator typeLine()
    {
        string[] words = lines[index].Split();
        for (int i = 0; i < words.Length; i++)
        {
            textComponent.text += (i == words.Length - 1) ? words[i] : words[i] + " ";
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void nextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(typeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
