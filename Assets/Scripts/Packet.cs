using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Packet
{

    public enum PacketType : ushort
    {
        InstantiatePlayer,
        InputMessage,
        StateMessage,
    }

    public UInt16 packetType;
    public List<byte> buffer;
    public int offset;

    public Packet(PacketType type)
    {
        packetType = Convert.ToUInt16(type);
        buffer = new List<byte>();
        buffer.AddRange(BitConverter.GetBytes(packetType));
    }

    public Packet(byte[] data)
    {
        buffer = new List<byte>(data);
        packetType = BitConverter.ToUInt16(buffer.GetRange(offset, sizeof(ushort)).ToArray());
        offset += sizeof(ushort);
    }



}
