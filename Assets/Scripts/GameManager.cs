using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Steamworks;
using Steamworks.Data;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    TMP_Text LobbyIdTxt;

    [SerializeField]
    TMP_Text HostingTxt;

    [SerializeField]
    TMP_Text TickTxt;

    public Dictionary<SteamId, GameObject> PlayerList;

    [SerializeField]
    GameObject NetManager;

    // Start is called before the first frame update
    void Start()
    {
        PlayerList = new Dictionary<SteamId, GameObject>();
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
                TickTxt.SetText(NetManager.GetComponent<Server>().serverTick.ToString());
            } else
            {
                HostingTxt.SetText("Client");
                TickTxt.SetText(NetManager.GetComponent<Client>().clientTick.ToString());
            }

        } else
        {
            LobbyIdTxt.SetText("Offline");
            TickTxt.SetText(NetManager.GetComponent<Server>().serverTick.ToString());
        }
    }

    public void AddPlayerToList(SteamId id, GameObject player)
    {
        if (!PlayerList.ContainsKey(id))
        {
            PlayerList.Add(id, player);
        }

        Debug.Log("Adding player with id: " + id);
    }

    public void RemovePlayerFromList(SteamId id)
    {
        if (PlayerList.ContainsKey(id))
        {
            Destroy(PlayerList[id]);
            PlayerList.Remove(id);
        }

    }

}
