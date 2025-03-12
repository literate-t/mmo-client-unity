using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace ServerCore
{
    public class Connector
    {
        Func<Session> _sessionFactory;

        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory, int count = 1)
        {      
            _sessionFactory = sessionFactory;

            for (int i = 0; i < count; i++)
            {
                // 커넥터를 통해 여러 클라이언트의 접속을 도와야 하므로
                // 멤버 변수로 가지고 있을 필요가 없다
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                SocketAsyncEventArgs eventArgs = new SocketAsyncEventArgs();
                eventArgs.Completed += OnConnectedCompleted;
                eventArgs.RemoteEndPoint = endPoint;
                eventArgs.UserToken = socket;

                RegisterConnect(eventArgs);
            }
        }

        void RegisterConnect(SocketAsyncEventArgs connectEvent)
        {
            // 캐스팅에 실패하면 null 반환
            Socket socket = connectEvent.UserToken as Socket;
            if (null == socket)
            {
                return;
            }

            bool pending = socket.ConnectAsync(connectEvent);
            if (false == pending)
            {
                OnConnectedCompleted(null, connectEvent);
            }
        }

        void OnConnectedCompleted(object sender, SocketAsyncEventArgs eventArgs)
        {
            if (SocketError.Success == eventArgs.SocketError)
            {
                Session session = _sessionFactory();
                session.OnConnected(eventArgs.RemoteEndPoint);
                session.Start(eventArgs.ConnectSocket);
            }
            else
            {
                Debug.Log($"OnConnectedCompleted Failed : {eventArgs.SocketError}");
            }
        }
    }
}
