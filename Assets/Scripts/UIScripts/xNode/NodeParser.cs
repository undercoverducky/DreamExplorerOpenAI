using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using XNode;
using static UnityEditor.Progress;
using System.Net.Http;
using System.Text;
using System;
using System.IO;

public class NodeParser : MonoBehaviour
{
    /*
     Move all this to the dialogue script (move type line ai into the parse node dialogue case). Add public dialogue graph to every interactable npc to be passed in
     Create a player choice node, which displays possible choices and whether or not that icludes an input field. Processing this node
        means pushing the text so far up and resizing the options evently in the dialogue window.
        These choices should be templates which can be created and have mouse click functions on them like the inventory
     When dialogue ui is activated, set the dialogu graph, check if theres a character prompt to use and begin the parse node coroutine.
     USe "ai_generated" in dialogueline where ai should be used.
     */
    
    Coroutine _parser;
    public Transform text_container;
    public Transform text_template;
    public Image speaker_image;
    public float textSpeed;
    public UI_Inventory ui_inventory;

    private Transform choices_container;
    private Transform text_choice_template;
    private Transform input_choice_template;
    private string transcript;
    private OpenAIAPIClient ai_client = new OpenAIAPIClient(new HttpClient());
    private string npcPromptSetting = "";
    private NPCInteractable npc;
    private DialogueGraph graph;
    private IItem item_used = null;
    private Inventory inventory;
    // Start is called before the first frame update
    void Start()
    {
        
        choices_container = transform.Find("ChoicesContainer");
        text_choice_template = choices_container.Find("TextChoiceTemplate");
        input_choice_template = choices_container.Find("InputChoiceTemplate");

    }

    public void set_inventory(Inventory inventory) {
        this.inventory = inventory;
        this.inventory.on_item_use += inventory_on_item_use;
    }
    private void set_graph(DialogueGraph graph) {
        this.graph = graph;
        foreach (CoreNode node in graph.nodes)
        {
            if (node.get_string().Equals("Start"))
            {
                graph.current = node;
                break;
            }
        }
    }

    public void begin_parsing_graph() {
        _parser = StartCoroutine(parse_node());
    }

    IEnumerator parse_node() {
        CoreNode node = graph.current;
        string data = node.get_string();
        string[] data_parts = data.Split('/');

        if (data_parts[0].Equals("Start"))
        {
            next_node("exit");
        }
        else if (data_parts[0].Equals("End"))
        {
            yield return new WaitForSeconds(1.5f);
            Player.Instance.enable_player_action();
            Player.Instance.is_talking = 1.0f;
            gameObject.SetActive(false);
        }
        else if (data_parts[0].Equals("DialogueNode"))
        {
            // run dialogue processing
            //text_component.text += data_parts[1].ToUpper() + " - " + data_parts[2] + "\n";
            create_text(data_parts[1].ToUpper() + " - " + data_parts[2]);
            transcript += data_parts[1].ToUpper() + " - " + data_parts[2] + "\n";
            if (node.get_sprite() != null)
            {
                speaker_image.sprite = node.get_sprite();
                speaker_image.gameObject.SetActive(true);
            }

            //yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            // return new WaitUntil(() => Input.GetMouseButtonUp(0));
            yield return new WaitForSeconds(.5f);
            next_node("exit");
        }
        else if (data_parts[0].Length > 16 && data_parts[0].Substring(0, 16).Equals("PlayerChoiceNode"))
        {
            int num_choices = data_parts[0].ToCharArray()[16] - '0';
            // "start/text/blah/input/writehere/text/blah"
            int cell_height = 194 / num_choices;
            for (int i = 0; i < num_choices; i++)
            {
                string content = data_parts[i * 2 + 2];

                if (data_parts[i * 2 + 1].Equals("text"))
                {
                    RectTransform text_choice_transform = Instantiate(text_choice_template, choices_container).GetComponent<RectTransform>();
                    text_choice_transform.gameObject.SetActive(true);
                    text_choice_transform.GetComponent<CodeMonkey.Utils.Button_UI>().ClickFunc = () =>
                    {
                        //select this choice, figure out how to set multppile exits
                        transcript += "YOU - " + content + "\n";
                        create_text("YOU - " + text_choice_transform.Find("TextChoice").GetComponent<TextMeshProUGUI>().text.Substring(3));
                        destroy_choices();
                        next_node("choice" + text_choice_transform.Find("TextChoice").GetComponent<TextMeshProUGUI>().text.ToCharArray()[0]);
                    };
                    text_choice_transform.anchoredPosition = new Vector2(0, 82 - cell_height * i); //tune this position
                    TextMeshProUGUI text = text_choice_transform.Find("TextChoice").GetComponent<TextMeshProUGUI>();
                    text.text = (i + 1).ToString() + ". " + content;
                }
                else if (data_parts[i * 2 + 1].Equals("input"))
                { //GUARANTEED TO BE FIRST CHOICE
                    RectTransform input_choice_transform = Instantiate(input_choice_template, choices_container).GetComponent<RectTransform>();
                    input_choice_transform.gameObject.SetActive(true);
                    input_choice_transform.anchoredPosition = new Vector2(0, 82 - cell_height * i); //tune this position
                }

            }

        }
        else if (data_parts[0].Equals("AIDialogueNode")) // ONLY ALLOWED IN NPC WITH CHARACTER PROMPTS
        {
            set_npc_prompt(npc.npcName);
            //text_component.text += data_parts[1].ToUpper() + " - ";
            TextMeshProUGUI temp_text = create_text(data_parts[1].ToUpper() + " - ");
            transcript += data_parts[1].ToUpper() + " - ";
            if (node.get_sprite() != null)
            {
                speaker_image.sprite = node.get_sprite();
                speaker_image.gameObject.SetActive(true);
            }
            string prompt = generatePrompt();
            // run dialogue processing
            Debug.Log("sending prompt: \n" + prompt);
            System.Threading.Tasks.Task<string> line_task = ai_client.generate_text_async(prompt, frequency_penalty: 2f, presence_penalty: 1.0f);
            yield return new WaitUntil(() => line_task.IsCompleted);
            string ai_response = line_task.Result;
            string[] words = ai_response.Split();
            for (int i = 0; i < words.Length; i++)
            {
                temp_text.text += (i == words.Length - 1) ? words[i] + "\n" : words[i] + " ";
                transcript += (i == words.Length - 1) ? words[i] + "\n" : words[i] + " ";
                yield return new WaitForSeconds(textSpeed);
            }
            next_node("exit");
        }
        else if (data_parts[0].Equals("UseInventoryItemNode")) {
            ui_inventory.show();

            item_used = null;
            while (item_used == null || (item_used != null && item_used.get_item_type() == ItemType.Lucidator)) {

                yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
            }
            ui_inventory.hide();
            if (item_used.get_item_type() == ItemType.Player_Generated) {
                System.Threading.Tasks.Task<bool> ask_task = ((UseInventoryItemNode)node).use_player_item((PlayerItem)item_used);
                yield return new WaitUntil(() => ask_task.IsCompleted);
                bool response = ask_task.Result;
                if (response)
                {
                    Debug.Log("AI says it would work");
                    next_node("valid");
                }
                else {
                    Debug.Log("AI says it wouldn't work");
                    next_node("invalid");
                }
            }
            else {
                Debug.Log("Player used item " + ((GItem)item_used).description);
                if (((UseInventoryItemNode)node).use_game_item((GItem)item_used))
                {
                    next_node("valid");
                }
                else {
                    next_node("invalid");
                }
            }
            
        }
        
    }

    private void inventory_on_item_use(object sender, System.EventArgs e)
    {
        item_used = ((ItemUseEventArgs)e).used_item;
        
    }

    private TextMeshProUGUI create_text(string content) {
        RectTransform text_transform = Instantiate(text_template, text_container).GetComponent<RectTransform>();
        text_transform.gameObject.SetActive(true);
        text_transform.GetComponent<TextMeshProUGUI>().text = content;
        return text_transform.GetComponent<TextMeshProUGUI>();
    }

    private string generatePrompt() {
        string[] lines = transcript.Split("\n");
        string prompt = npcPromptSetting;
        for (int i = 0; i < lines.Length; i++) {
            prompt += lines[i].Replace("\"", "\\\"") + "\\n";
        }
        return prompt;
    }

    public void readStringInput(string s)
    {
        Debug.Log($"Player wrote: {s}");
        if (!isActiveAndEnabled || s.Equals(string.Empty)) //Dialogue disabled with leftover text
        {
            return;
        }
        transcript += "YOU - " + s + "\n";
        create_text("YOU - " + s);
        destroy_choices();
        next_node("choice1"); //GUARANTEED TO BE FIRST CHOICE
    }

    public void next_node(string field_name) {
        if (_parser != null) {
            StopCoroutine(_parser);
            _parser = null;
        }
        foreach (NodePort np in graph.current.Ports) {
            if (np.fieldName.Equals(field_name)) {
                graph.current = np.Connection.node as CoreNode;
                break;
            }
        }
        _parser = StartCoroutine(parse_node());
    }

    private void destroy_choices() {
        foreach (Transform child in choices_container)
        {
            if (child == text_choice_template || child == input_choice_template) continue;
            Destroy(child.gameObject);
        }
    }

    private void destroy_texts() {
        foreach (Transform child in text_container)
        {
            if (child == text_template) continue;
            Destroy(child.gameObject);
        }
    }

    void OnDisable() {
        destroy_texts();
        transcript = string.Empty;
        speaker_image.sprite = null;
        speaker_image.gameObject.SetActive(false);
        graph = null;
        destroy_choices();
        StopAllCoroutines();
    }

   
    private void set_npc_prompt(string npcName) {
      npcPromptSetting = getNPCPromptSetting("Assets/InteractableText/Characters/" + npc.npcName.ToLower() + ".txt", Encoding.UTF8);
    }

    public void set_npc(NPCInteractable npc) {
        this.npc = npc;
        
        set_graph(npc.dialogue_graph);

    }

    string getNPCPromptSetting(string file_path, Encoding encoding) {
        if (file_path == null) {
            throw new ArgumentNullException("path");
        }
        if (file_path.Length == 0) {
            throw new ArgumentException("empty path");
        }
        string result;
        using (StreamReader streamReader = new StreamReader(file_path, encoding)) {
            result = streamReader.ReadToEnd();
        }
        return result;
    }
}
