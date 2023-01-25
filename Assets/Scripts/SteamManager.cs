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

            try
            {
                SteamClient.RestartAppIfNecessary(AppId);
                SteamClient.Init(AppId, true);

                if (!SteamClient.IsValid)
                {
                    throw new Exception("Steam client not valid");
                }
            } catch (Exception e)
            {
                Debug.Log(e);
            }

        } else if (Instance != this)
        {
            Destroy(gameObject);
        }

    }

}
