using System;
using System.Collections.Generic;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TemporarySceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        bool state = false;

        try
        {
            await SteamLobbyManager.Instance.SearchLobbies("JustTesting");

            List<Lobby> lobbiesFound = SteamLobbyManager.Instance.LobbiesResult;

            if (lobbiesFound != null && lobbiesFound.Count > 0)
            {
                state = await SteamLobbyManager.Instance.JoinLobby(lobbiesFound[0]);
            } else
            {
                state = await SteamLobbyManager.Instance.CreateLobby(4, "JustTesting");
            }

        } catch (Exception e)
        {
            Debug.Log(e);
            state = false;
        }

        if (!state)
        {
            SceneManager.LoadScene(1);
        }
    }

}
