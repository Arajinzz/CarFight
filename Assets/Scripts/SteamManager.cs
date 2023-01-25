using System;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

public class SteamManager : MonoBehaviour
{

    public static SteamManager Instance;

    private static uint AppId = 480u;

    private void Awake()
    {
        
        if (Instance == null)
        {
            DontDestroyOnLoad(this);
            Instance = this;



        }

    }

}
