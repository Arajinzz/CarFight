using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    TMP_Text LobbyIdTxt;

    [SerializeField]
    TMP_Text HostingTxt;

    [SerializeField]
    TMP_Text TickTxt;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SteamLobbyManager.Instance)
        {
            LobbyIdTxt.SetText(SteamLobbyManager.Instance.CurrentLobby.Id.ToString());

            if (SteamLobbyManager.Instance.CurrentLobby.Owner.Id.Value == SteamManager.Instance.PlayerId.Value)
            {
                HostingTxt.SetText("Host");
            } else
            {
                HostingTxt.SetText("Client");
            }

        } else
        {
            LobbyIdTxt.SetText("Offline");
        }
    }
}
