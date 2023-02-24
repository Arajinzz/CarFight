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
            HandleTick(minTimeBetweenTicks);

            clientTick++;
        }
    }

    private void HandleTick(float deltaTime)
    {

        // Get input
        Structs.Inputs inputs;
        inputs.up = Input.GetKey(KeyCode.W);
        inputs.down = Input.GetKey(KeyCode.S);
        inputs.left = Input.GetKey(KeyCode.A);
        inputs.right = Input.GetKey(KeyCode.D);
        inputs.rclick = Input.GetMouseButton(0);


        if (localPlayer)
        {
            // if i Am a server let server process
            // to avoid multiple mouvement process
            // this probably should be reworked
            if (owner.Id != SteamManager.Instance.PlayerId)
            {
                // Process mouvement here
                localPlayer.ProcessMouvement(inputs, deltaTime);
                Physics.Simulate(deltaTime);
            }

            Structs.InputMessage inputMsg;
            inputMsg.tick_number = clientTick;
            inputMsg.inputs = inputs;

            var inputPacket = new Packet(Packet.PacketType.InputMessage);
            inputPacket.InsertInputMessage(inputMsg);
            SendToServer(inputPacket.buffer.ToArray());
        }

        // Handle received packets
        while (receivedPackets.Count > 0)
        {
            var recPacket = receivedPackets.Dequeue();

            var packet = new Packet(recPacket.Value.Data);

            if (packet.GetPacketType() == Packet.PacketType.InstantiatePlayer)
            {
                Debug.Log("Will instantiate a player ...");
                GameObject playerObj = Instantiate(playerPrefab, GameObject.Find("SpawnPoint").transform.position, Quaternion.identity);
                SteamId playerId = packet.PopUInt64();
                Debug.Log("Received ID : " + playerId);

                // if me set local player to instantiated player
                if (playerId == SteamManager.Instance.PlayerId)
                {
                    localPlayer = playerObj.GetComponent<CarController>();
                    localPlayer.SetCamera(); // Set the camera
                    gameManager.AddPlayerToList(playerId, localPlayer.gameObject);
                }
                else
                {
                    gameManager.AddPlayerToList(playerId, playerObj.gameObject);
                }
            }
            else if (packet.GetPacketType() == Packet.PacketType.StateMessage)
            {
                SteamId playerId = packet.PopUInt64();
                Structs.StateMessage stateMsg = packet.PopStateMessage();

                // Handle Correction Here
                if (gameManager.PlayerList.ContainsKey(playerId))
                {
                    GameObject _player = gameManager.PlayerList[playerId];
                    Rigidbody _playerRb = _player.GetComponent<Rigidbody>();

                    _player.transform.position = new Vector3(stateMsg.position.x, stateMsg.position.y, stateMsg.position.z);
                    _player.transform.rotation = new Quaternion(stateMsg.rotation.x, stateMsg.rotation.y, stateMsg.rotation.z, stateMsg.rotation.w);

                    _playerRb.velocity = new Vector3(stateMsg.velocity.x, stateMsg.velocity.y, stateMsg.velocity.z);
                    _playerRb.angularVelocity = new Vector3(stateMsg.angular_velocity.x, stateMsg.angular_velocity.y, stateMsg.angular_velocity.z);
                    _playerRb.drag = stateMsg.drag;
                    _playerRb.angularDrag = stateMsg.angular_drag;
                }
            } else if (packet.GetPacketType() == Packet.PacketType.Shoot)
            {

                SteamId playerId = packet.PopUInt64();
                Structs.InputMessage inputMsg = packet.PopInputMessage();

                if (gameManager.PlayerList.ContainsKey(playerId))
                {
                    GameObject _player = gameManager.PlayerList[playerId];
                    _player.GetComponent<CarController>().ProcessShooting(inputMsg.inputs, deltaTime);
                }

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

    public void PacketManualEnqeue(P2Packet packet)
    {
        receivedPackets.Enqueue(packet);
    }

    private void SendToServer(byte[] data)
    {
        SteamNetworking.SendP2PPacket(currentLobby.Owner.Id, data);
    }
}
