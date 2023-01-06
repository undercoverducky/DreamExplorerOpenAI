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
using System.Threading.Tasks;
using UnityEditor.VersionControl;

public class PlayerScript : MonoBehaviour
{

    OpenAIAPIClient ai_client = new OpenAIAPIClient(new HttpClient());
    public float moveSpeed = 5.0f; // speed of movement
    public string player_img_prompt;
    public float isTalking = 1.0f;

    void Update()
    {
        checkInput();
        
        
    }

    public NPCInteractable getInteractableNPC(float with_dist)
    {
        List<NPCInteractable> interactable_list = new List<NPCInteractable>();
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, with_dist);
        foreach (Collider2D coll in objects)
        {
            if (coll.TryGetComponent(out NPCInteractable n))
            {
                interactable_list.Add(n);
            }
        }
        NPCInteractable closest_npc = null;
        foreach (NPCInteractable n in interactable_list)
        {
            if (closest_npc == null)
            {
                closest_npc = n;
            }
            else
            {
                if ((transform.position - n.transform.position).sqrMagnitude < (transform.position - closest_npc.transform.position).sqrMagnitude)
                {
                    closest_npc = n;
                }
            }
        }
        return closest_npc;

    }



    void Start()
    {
        
        StartCoroutine(setPlayerSprite(player_img_prompt));
    
    }

    void checkInput()
    {
        // movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        transform.Translate(new Vector2(horizontalInput, verticalInput) * moveSpeed * Time.deltaTime * isTalking);

        //...
    }
    
    IEnumerator setPlayerSprite(string prompt = "High quality high definition dark souls boss with no background")
    {

        const int width = 256;
        const int height = 256;

        System.Threading.Tasks.Task<string> url_task = ai_client.generate_image_async(prompt, width, height);
        //wait until url finished generating before revisitng this coroutine
        yield return new WaitUntil(() => url_task.IsCompleted); 
        string url = url_task.Result;

        // non async method. This blocks execution, adds 4 s before game view gets rendered. Not desired
        // string url = ai_client.generate_image(prompt, width, height)

        UnityWebRequest imgReq = UnityWebRequestTexture.GetTexture(url);
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
}

