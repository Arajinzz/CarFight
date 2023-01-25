using System.Collections;
using System.Collections.Generic;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TemporarySceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        try
        {
            await SteamLobbyManager.Instance.SearchLobbies("JustTesting");

            List<Lobby> lobbiesFound = SteamLobbyManager.Instance.LobbiesResult;

            if (lobbiesFound != null && lobbiesFound.Count > 0)
            {
                await SteamLobbyManager.Instance.JoinLobby(lobbiesFound[0]);
            } else
            {
                await SteamLobbyManager.Instance.CreateLobby(4, "JustTesting");
            }

        } catch
        {
            SceneManager.LoadScene(1);
        }
    }

}
