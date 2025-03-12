using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace ServerCore
{
    public abstract class PacketSession : Session
    {
        public const ushort HeaderSize = 2;
        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            int processLength = 0;
            while (true)
            {
                int dataSize = buffer.Count;

                if (dataSize < HeaderSize)
                    break;

                ushort packetSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                if (dataSize < packetSize)
                    break;

                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, packetSize));
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + packetSize, dataSize - packetSize);

                processLength += packetSize;
            }

            return processLength;
        }

        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }

    public abstract class Session
    {
        object _lock = new object();

        Socket _socket;
        int _disconnected = 0;
        RecvBuffer _recvBuffer = new RecvBuffer(65535);

        Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
        SocketAsyncEventArgs _sendEvent = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvEvent = new SocketAsyncEventArgs();

        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numberOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);


        public void Start(Socket socket)
        {
            _socket = socket;

            _recvEvent.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            _sendEvent.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            //_recvEvent.SetBuffer(new byte[1024], 0, 1024);

            RegisterRecv();
        }

        void RegisterRecv()
        {
            if (1 == _disconnected)
                return;

            _recvBuffer.Clean();
            ArraySegment<byte> writeSegment = _recvBuffer.WriteSegment;
            _recvEvent.SetBuffer(writeSegment.Array, writeSegment.Offset, writeSegment.Count);

            try
            {
                bool pending = _socket.ReceiveAsync(_recvEvent);
                if (false == pending)
                {
                    OnRecvCompleted(null, _recvEvent);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"RegisterRecv falied {ex}");
            }
        }

        public void Send(ArraySegment<byte> sendBuffer)
        {
            bool sendRegistered = false;
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuffer);
                if (0 == _pendingList.Count)
                    sendRegistered = true;
            }

            if (sendRegistered)
                RegisterSend();
        }

        public void Send(List<ArraySegment<byte>> sendBuffers)
        {
            if (0 == sendBuffers.Count)
                return;

            bool sendRegistered = false;
            lock (_lock)
            {
                foreach (ArraySegment<byte> buffer in sendBuffers)
                    _sendQueue.Enqueue(buffer);
                if (0 == _pendingList.Count)
                    sendRegistered = true;
            }

            if (sendRegistered)
                RegisterSend();
        }

        #region network

        void RegisterSend()
        {
            if (1 == _disconnected)
                return;

            lock (_lock)
            {
                while (0 < _sendQueue.Count)
                {
                    ArraySegment<byte> buffer = _sendQueue.Dequeue();
                    _pendingList.Add(buffer);
                }
                _sendEvent.BufferList = _pendingList;
            }

            try
            {
                bool pending = _socket.SendAsync(_sendEvent);
                if (false == pending)
                {
                    OnSendCompleted(null, _sendEvent);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"RegisterSend falied {ex}");
            }
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs sendEvent)
        {
            if (0 < sendEvent.BytesTransferred && SocketError.Success == sendEvent.SocketError)
            {
                try
                {
                    lock (_lock)
                    {
                        _sendEvent.BufferList = null;
                        _pendingList.Clear();

                        OnSend(sendEvent.BytesTransferred);

                        if (0 < _sendQueue.Count)
                            RegisterSend();
                    }
                }
                catch (Exception e)
                {
                    Debug.Log($"OnSendCompleted Error {e.Message}");
                }
            }
            else
            {
                Disconnect();
            }
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs eventArgs)
        {
            // success
            if (0 < eventArgs.BytesTransferred && SocketError.Success == eventArgs.SocketError)
            {
                try
                {
                    // 사용한 만큼 버퍼의 write position 이동
                    if (false == _recvBuffer.OnWrite(eventArgs.BytesTransferred))
                    {
                        Disconnect();
                        return;
                    }

                    // 컨텐츠 단으로 데이터를 넘기고 처리 바이트 수를 받는다
                    int processLength = OnRecv(_recvBuffer.ReadSegment);
                    if (processLength < 0 || _recvBuffer.DataSize < processLength)
                    {
                        Disconnect();
                        return;
                    }

                    // 데이터를 읽은 만큼 read position 이동
                    if (false == _recvBuffer.OnRead(processLength))
                    {
                        Disconnect();
                        return;
                    }

                    RegisterRecv();
                }
                catch (Exception e)
                {
                    Debug.Log($"OnRecvCompleted Error {e.Message}");
                }
            }
            else
            {
                Disconnect();
            }
        }

        void Clear()
        {
            lock (_lock)
            {
                _pendingList.Clear();
                _sendQueue.Clear();
            }
        }

        public void Disconnect()
        {
            // 종료를 한 번만 하는 걸 보장한다
            if (1 == Interlocked.Exchange(ref _disconnected, 1))
                return;

            OnDisconnected(_socket.RemoteEndPoint);

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();

            Clear();
        }

        #endregion
    }
}
