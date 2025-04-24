using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataHolder : MonoBehaviour
{
    public static MapDataHolder Instance;

    public Location CurrentMap { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
