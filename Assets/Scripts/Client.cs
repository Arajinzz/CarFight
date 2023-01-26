using System;
using System.Collections.Generic;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

public class Client : MonoBehaviour
{
    [SerializeField]
    GameObject playerPrefab;

    [SerializeField]
    GameManager gameManager;

    // to run at fixed tick rate
    private float clientTimer;
    public uint clientTick;
    private float minTimeBetweenTicks;
    private const float CLIENT_TICK_RATE = 120f;

    private Queue<P2Packet?> receivedPackets;

    private Lobby currentLobby;
    private Friend owner;
    private CarController localPlayer;

    // Start is called before the first frame update
    void Start()
    {
        clientTimer = 0.0f;
        clientTick = 0;
        minTimeBetweenTicks = 1 / CLIENT_TICK_RATE;

        receivedPackets = new Queue<P2Packet?>();

        if (SteamLobbyManager.Instance)
        {
            currentLobby = SteamLobbyManager.Instance.CurrentLobby;
            owner = currentLobby.Owner;
            // get server tick
            clientTick = Convert.ToUInt32(currentLobby.GetData("ServerTick"));
            // Instantiate player on server
            var packet = new Packet(Packet.PacketType.InstantiatePlayer);
            SendToServer(packet.buffer.ToArray());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SendToServer(byte[] data)
    {
        SteamNetworking.SendP2PPacket(currentLobby.Owner.Id, data);
    }
}
