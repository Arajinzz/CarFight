using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structs
{

    public struct Inputs
    {
        public bool up;
        public bool down;
        public bool left;
        public bool right;

    }

    public struct InputMessage
    {
        public float delivery_time;
        public uint tick_number;
        public Inputs inputs;
    }

    public struct PlayerState
    {
        public Inputs inputs;
        public Vector3 position;
        public Quaternion rotation;
    }

    public struct StateMessage
    {
        public float delivery_time;
        public uint tick_number;
        public Vector3 position; // 12 bytes
        public Quaternion rotation; // 16 bytes
        public Vector3 velocity; // 12 bytes
        public Vector3 angular_velocity; // 12 bytes
    }

}
