using System.Collections;
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
