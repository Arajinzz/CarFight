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
    private const float CLIENT_TICK_RATE = 60f;

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
        if (SteamLobbyManager.Instance && currentLobby.Owner.Id != owner.Id)
        {
            // Means owner changed, server changed
            owner = currentLobby.Owner;
            clientTick = Convert.ToUInt32(currentLobby.GetData("ServerTick"));
        }

        // Receive packets ASAP, client receives packets only if he is not a server
        if (SteamLobbyManager.Instance && !owner.IsMe)
            ReceivePackets();

        clientTimer += Time.deltaTime;

        while (clientTimer >= minTimeBetweenTicks)
        {
            clientTimer -= minTimeBetweenTicks;

            // Handle tick here
            HandleTick();

            clientTick++;
        }
    }

    private void HandleTick()
    {

    }

    private void ReceivePackets()
    {
        if (!SteamManager.Instance)
            return;

        while (SteamNetworking.IsP2PPacketAvailable())
        {
            var packet = SteamNetworking.ReadP2PPacket();
            if (packet.HasValue)
            {
                receivedPackets.Enqueue(packet);
            }
        }
    }


    private void SendToServer(byte[] data)
    {
        SteamNetworking.SendP2PPacket(currentLobby.Owner.Id, data);
    }
}
