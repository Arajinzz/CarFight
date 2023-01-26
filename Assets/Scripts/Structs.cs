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

}
