using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance { get; set; }
    public void Awake()
    {
        Instance = this;
    }

    public Transform pfItemWorld;

    public Sprite coin_sprite;
    public Sprite error_sprite;
    public Sprite loading_sprite;
    public Sprite hat_sprite;
    public Sprite lucidator_sprite;
}
