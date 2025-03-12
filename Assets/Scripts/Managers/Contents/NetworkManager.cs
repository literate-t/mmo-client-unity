using System;
using System.Collections.Generic;
using System.Net;
using Google.Protobuf;
using ServerCore;

public class NetworkManager
{
    ServerSession _session = new();

    public void Send(ArraySegment<byte> sendBuffer)
    {
        _session.Send(sendBuffer);
    }

    public void Send(IMessage packet)
    {
        _session.Send(packet);
    }


    public void Init()
    {
        string host = Dns.GetHostName();

        IPAddress ipAddress;
        IPAddress.TryParse("127.0.0.1", out ipAddress);
        IPEndPoint endPoint = new IPEndPoint(ipAddress, 9999);

        Connector connector = new Connector();
        connector.Connect(endPoint, () => _session, 1);
    }

    public void Update()
    {
        List<PacketMessage> packets = PacketQueue.Instance.PopAll();
        foreach (PacketMessage packet in packets)
        {
            Action<PacketSession, IMessage> handler = ServerPacketManager.Instance.GetPacketHandler(packet.Id);
            if (handler != null)
                handler(_session, packet.Message);
        }
    }
}
