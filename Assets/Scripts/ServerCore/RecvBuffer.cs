using System;

namespace ServerCore
{
    internal class RecvBuffer
    {
        ArraySegment<byte> _buffer;

        int _readPosition = 0;
        int _writePosition = 0;

        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        public int DataSize {  get { return _writePosition - _readPosition; } }
        public int FreeSize {  get { return _buffer.Count - _writePosition; } }

        public ArraySegment<byte> ReadSegment
        {
            // _buffer.Offset을 넣는 건 0이라서 의미 없지만 확장성을 고려해 넣어주도록 하자
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPosition, DataSize); }
        }

        public ArraySegment<byte> WriteSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePosition, FreeSize); }
        }

        public void Clean()
        {
            if (0 == DataSize)
            {
                _readPosition = _writePosition = 0;
            }
            else
            {
                // r,w 위치가 다를 경우 그대로 복사해 배열 맨앞으로 이동한다
                Array.Copy(_buffer.Array, _buffer.Offset + _readPosition, _buffer.Array, _buffer.Offset, DataSize);
                _readPosition = 0;
                _writePosition = DataSize;
            }
        }

        public bool OnRead(int numberOfBytes)
        {
            if (DataSize < numberOfBytes)
                return false;

            _readPosition += numberOfBytes;
            return true;
        }

        public bool OnWrite(int numberOfBytes)
        {
            if (FreeSize < numberOfBytes)
                return false;

            _writePosition += numberOfBytes;
            return true;
        }
    }
}
