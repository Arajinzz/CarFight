using System;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

public class SteamLobbyManager : MonoBehaviour
{

    public static SteamLobbyManager Instance;

    private void Awake()
    {
        
        if (Instance == null)
        {
            DontDestroyOnLoad(this);
            Instance = this;
        } else if (Instance != this)
        {
            Destroy(gameObject);
        }

    }

}
