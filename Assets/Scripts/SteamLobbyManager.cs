using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SteamLobbyManager : MonoBehaviour
{

    public static SteamLobbyManager Instance;

    // Lobby currently joined
    public Lobby CurrentLobby { get; set; }

    // Here resides the lobby search result
    public List<Lobby> LobbiesResult;

    private void Awake()
    {
        
        if (Instance == null)
        {
            DontDestroyOnLoad(this);
            Instance = this;
            LobbiesResult = new List<Lobby>();
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


    private void OnApplicationQuit()
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

    #region Help functions

    public async Task<bool> CreateLobby(int maxPlayers, string gameName)
    {

        try
        {
            var lobbyOut = await SteamMatchmaking.CreateLobbyAsync(maxPlayers);

            if (!lobbyOut.HasValue)
            {
                throw new System.Exception("Lobby created but not instantiated correctly");
            }

            lobbyOut.Value.SetPublic();
            lobbyOut.Value.SetJoinable(true);
            lobbyOut.Value.SetData("GameName", gameName);

            return true;

        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }

        return false;

    }

    public async Task<bool> JoinLobby(Lobby lobby)
    {

        RoomEnter joinedLobbySuccess = await lobby.Join();
        if (joinedLobbySuccess != RoomEnter.Success)
        {
            return false;
        }
        return true;
    }

    public void LeaveLobby()
    {

        try
        {
            CurrentLobby.Leave();
        }
        catch
        {
            return;
        }

        // Left Successfully
        // Maybe we want to load another scene
        // SceneManager.LoadScene(N);

    }

    public async Task<bool> SearchLobbies(string gameName)
    {

        try
        {
            LobbiesResult.Clear();
            Lobby[] lobbies = await SteamMatchmaking.LobbyList.WithMaxResults(20)
                                                              .WithKeyValue("GameName", gameName)
                                                              .RequestAsync();
            if (lobbies != null)
            {
                foreach (Lobby lobby in lobbies.ToList())
                {
                    LobbiesResult.Add(lobby);
                }
            }
            return true;
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }

        return false;

    }


    #endregion


    #region Callbacks

    // Executed when a lobby is created
    private void OnLobbyCreatedCallback(Result result, Lobby lobby)
    {

        if (result == Result.OK)
        {
            CurrentLobby = lobby;
            CurrentLobby.SetData("GameState", "Waiting");

            // TEMPORARY: START GAME IMMEDIATLY
            CurrentLobby.SetGameServer(SteamManager.Instance.PlayerId);
        }

    }

    // Executed only when a member Disconnected
    // Which means when i get Disconnected it will not get executed 
    private void OnLobbyMemberDisconnectedCallback(Lobby lobby, Friend member)
    {

        SteamManager.Instance.CloseP2P(member.Id);

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
