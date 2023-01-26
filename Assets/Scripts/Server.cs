using System;
using System.Collections.Generic;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

public class Server : MonoBehaviour
{
    // to run at fixed tick rate
    private float serverTimer;
    public uint serverTick;
    private float minTimeBetweenTicks;
    private const float SERVER_TICK_RATE = 60f;

    private Queue<P2Packet?> receivedPackets;

    private Lobby currentLobby;
    private Friend owner;

    // Start is called before the first frame update
    void Start()
    {
        serverTimer = 0.0f;
        serverTick = 0;
        minTimeBetweenTicks = 1 / SERVER_TICK_RATE;

        receivedPackets = new Queue<P2Packet?>();

        if (SteamLobbyManager.Instance)
        {
            currentLobby = SteamLobbyManager.Instance.CurrentLobby;
            owner = currentLobby.Owner;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (SteamLobbyManager.Instance && SteamLobbyManager.Instance.CurrentLobby.Owner.Id != owner.Id)
        {
            // Means owner changed, server changed
            owner = currentLobby.Owner;
            serverTick = Convert.ToUInt32(currentLobby.GetData("ServerTick"));
        }

        if (SteamLobbyManager.Instance && !owner.IsMe)
        {
            return;
        }

        // Receive packets ASAP
        ReceivePackets();

        serverTimer += Time.deltaTime;

        while (serverTimer >= minTimeBetweenTicks)
        {
            serverTimer -= minTimeBetweenTicks;

            // Handle tick here
            if (SteamManager.Instance)
                currentLobby.SetData("ServerTick", serverTick.ToString());

            HandleTick();

            serverTick++;
        }

    }


    private void HandleTick()
    {

        // handle received packets
        while (receivedPackets.Count > 0)
        {
            var recPacket = receivedPackets.Dequeue();
            var packet = new Packet(recPacket.Value.Data);

            if (packet.GetPacketType() == Packet.PacketType.InstantiatePlayer)
            {
                packet.InsertUInt64(recPacket.Value.SteamId);

                P2Packet packetToSend;
                packetToSend.SteamId = recPacket.Value.SteamId;
                packetToSend.Data = packet.buffer.ToArray();

                // Send to all players
                SendToAllLobby(packetToSend);

                // Send all players to target
            }
            else if (packet.GetPacketType() == Packet.PacketType.InputMessage)
            {
                Structs.InputMessage inputMsg = packet.PopInputMessage();
                // Mouvement replication goes here
            }

        }


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

    private void SendToTarget(SteamId target, byte[] data)
    {
        SteamNetworking.SendP2PPacket(target, data);
    }

    public void SendToAllLobby(P2Packet packet)
    {
        foreach (Friend member in currentLobby.Members)
        {
            // This is me
            if (member.Id == owner.Id)
            {
                // Some handling needs to be done here
                continue;
            }
            SendToTarget(member.Id, packet.Data);
        }
    }

}