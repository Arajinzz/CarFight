using System;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

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


    private void Start()
    {

        SteamMatchmaking.OnLobbyCreated += OnLobbyCreatedCallback;
        SteamMatchmaking.OnLobbyMemberDisconnected += OnLobbyMemberDisconnectedCallback;
        SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeaveCallback;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoinedCallback;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEnteredCallback;
        SteamMatchmaking.OnLobbyInvite += OnLobbyInviteCallback;
        SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreatedCallback;
        SteamMatchmaking.OnChatMessage += OnChatMessageCallback;
        SteamNetworking.OnP2PSessionRequest += OnP2PSessionRequestCallback;
        SceneManager.sceneLoaded += OnSceneLoadedCallback;

    }




    #region Help functions

    private void CleanUp()
    {
        SteamMatchmaking.OnLobbyCreated -= OnLobbyCreatedCallback;
        SteamMatchmaking.OnLobbyMemberDisconnected -= OnLobbyMemberDisconnectedCallback;
        SteamMatchmaking.OnLobbyMemberLeave -= OnLobbyMemberLeaveCallback;
        SteamMatchmaking.OnLobbyMemberJoined -= OnLobbyMemberJoinedCallback;
        SteamMatchmaking.OnLobbyEntered -= OnLobbyEnteredCallback;
        SteamMatchmaking.OnLobbyInvite -= OnLobbyInviteCallback;
        SteamMatchmaking.OnLobbyGameCreated -= OnLobbyGameCreatedCallback;
        SteamMatchmaking.OnChatMessage -= OnChatMessageCallback;
        SteamNetworking.OnP2PSessionRequest -= OnP2PSessionRequestCallback;
        SceneManager.sceneLoaded -= OnSceneLoadedCallback;
    }

    #endregion






}
