using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public bool focusPlayer;
    public float cameraSpeed;

    private GameObject Player;

    private Transform playerTransform;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = Player.transform;
        focusPlayer = true;
        cameraSpeed = 3f;
    }

    void Update()
    {
        if(Player.activeSelf)
        {
            if (focusPlayer) followPlayer();
        }
    }

    void followPlayer()
    {
        Vector3 destPos = playerTransform.position;
        Vector3 smoothedCamPos = Vector3.Lerp(transform.position, destPos + new Vector3(0, 0, -10), cameraSpeed * Time.deltaTime);
        transform.position = smoothedCamPos;
    }
}
