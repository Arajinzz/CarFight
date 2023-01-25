using System;
using System.Threading.Tasks;
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

    public async Task<bool> CreateLobby(int maxPlayers, string gameName)
    {

        
        return true;

    }

    public async Task<bool> JoinLobby(Lobby lobby)
    {
        
        return true;
    }

    public void LeaveLobby()
    {
        


    }

    public async Task<bool> SearchLobbies(string gameName)
    {

        return true;

    }


    #endregion


    #region Callbacks

    // Executed when a lobby is created
    private void OnLobbyCreatedCallback(Result result, Lobby lobby)
    {
        


    }

    // Executed only when a member Disconnected
    // Which means when i get Disconnected it will not get executed 
    private void OnLobbyMemberDisconnectedCallback(Lobby lobby, Friend member)
    {



    }

    // Executed only when a member leaves
    // Which means when i leave it will not get executed 
    private void OnLobbyMemberLeaveCallback(Lobby lobby, Friend member)
    {
        


    }

    // Executed when a member joins the lobby
    private void OnLobbyMemberJoinedCallback(Lobby lobby, Friend member)
    {
        
    }

    // Executed when we enter the lobby
    private void OnLobbyEnteredCallback(Lobby lobby)
    {
        


    }

    private void OnLobbyInviteCallback(Friend member, Lobby lobby)
    {

    }

    // When Game is started
    private void OnLobbyGameCreatedCallback(Lobby lobby, uint ip, ushort port, SteamId gameServerId)
    {
        // Load game scene

    }

    // Executed when we receive a chat message
    private void OnChatMessageCallback(Lobby lobby, Friend friend, string message)
    {
        // Received chat message
        
    }

    private void OnP2PSessionRequestCallback(SteamId user)
    {

    }

    // Executed when a scene is loaded
    private void OnSceneLoadedCallback(Scene scene, LoadSceneMode loadSceneMode)
    {
        
    }

    #endregion


}
