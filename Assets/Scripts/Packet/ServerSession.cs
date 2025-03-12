using System;
using System.Net;
using Google.Protobuf;
using Google.Protobuf.MyProtocol;
using ServerCore;
using UnityEngine;

class ServerSession : PacketSession
{
    public void Send(IMessage packet)
    {
        string messageName = packet.Descriptor.Name.Replace("_", string.Empty);
        MessageId messageId = (MessageId)Enum.Parse(typeof(MessageId), messageName);

        ushort size = (ushort)packet.CalculateSize();
        byte[] sendBuffer = new byte[size + 4];
        Array.Copy(BitConverter.GetBytes(size + 4), 0, sendBuffer, 0, sizeof(ushort));
        ushort protocolId = (ushort)messageId;
        Array.Copy(BitConverter.GetBytes(protocolId), 0, sendBuffer, sizeof(ushort), sizeof(ushort));
        Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);

        Send(new ArraySegment<byte>(sendBuffer));
    }

    public override void OnConnected(EndPoint endPoint)
    {
        ServerPacketManager.Instance.PacketCallback = (id, packet) =>
        {
            PacketQueue.Instance.Push(id, packet);
        };

        Debug.Log($"OnConnected {endPoint}");
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        Console.WriteLine($"OnDisconnected {endPoint}");
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        ServerPacketManager.Instance.OnRecvPacket(this, buffer);
    }

    public override void OnSend(int numberOfBytes)
    {
    }
}

