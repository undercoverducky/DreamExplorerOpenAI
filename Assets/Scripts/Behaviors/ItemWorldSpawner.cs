using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorldSpawner : MonoBehaviour
{
    public GItem item;

    public void Awake()
    {
        ItemWorld.spawn_item_world(transform.position, item);
        Destroy(gameObject);
    }
}
