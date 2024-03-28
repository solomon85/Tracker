using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GPS_Tracker
{

    public class RSocket
    {
        public RSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
        {
            this._Socket = new Socket(addressFamily, socketType, protocolType);
            this.BufferSize = 4096;
        }
        public RSocket(Socket socket)
        {
            this._Socket = socket;
            new Thread(Receive).Start();
        }
        public RSocket(Socket socket, int bufferSize)
            : this(socket)
        {
            this.BufferSize = bufferSize;
        }

        public int BufferSize { get; set; }
        public Boolean MessageRecieved { get; set; }
        const int TotalDelay = 10;

        Socket _Socket;
        public Socket Socket
        {
            get { return this._Socket; }
            set
            {
                if (this._Socket != value)
                {
                    this._Socket = value;
                    new Thread(Receive).Start();
                }
            }
        }
        public delegate void _ReceiveData(RSocket sender, ReceivedData e);
        public event _ReceiveData ReceiveData;

        public void BeginReceive()
        {
            new Thread(Receive).Start();
        }
        public void Bind(EndPoint localEP)
        {
            this.Socket.Bind(localEP);
        }
        public void Listen(int backlog)
        {
            this.Socket.Listen(backlog);
        }
        public void Connect(IPEndPoint endPoint)
        {
            try
            {
                this.Socket.Connect(endPoint);
                new Thread(Receive).Start();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
        public RSocket Accept()
        {
            try
            {
                Socket s = this.Socket.Accept();
                RSocket res = new RSocket(s, this.BufferSize);
                return res;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
        public int Send(byte[] value)
        {
            byte[] _value = null;
            _value = value;

            int res = _value.Length;
            if (_value.Length < this.BufferSize - 1)
            {
                byte[] data = new byte[_value.Length];
                _value.CopyTo(data, 0);
                this.Socket.Send(data);
            }
            else
            {
                int sendedData = 0;
                while (sendedData < _value.Length)
                {
                    byte[] data;
                    if (sendedData + this.BufferSize - 1 <= _value.Length)
                    {
                        data = new byte[this.BufferSize];
                        data[0] = 1;
                    }
                    else
                    {
                        data = new byte[_value.Length - sendedData + 1];
                        data[0] = 0;
                    }
                    for (int i = 1; i < data.Length; i++, sendedData++)
                        data[i] = _value[sendedData];

                    this.Socket.Send(data);
                }
            }
            return res;
        }
        public int Send(Byte[] value, int index, int count)
        {
            byte[] temp = new byte[count];
            Buffer.BlockCopy(value, index, temp, 0, count);
            return Send(temp);
        }
        public void Close()
        {
            this.Socket.Close();
        }
        public void Close(int timeout)
        {
            this.Socket.Close(timeout);
        }
        public void Shutdown(SocketShutdown how)
        {
            if (this.Socket.Connected)
                this.Socket.Shutdown(how);
        }
        void Receive()
        {
            List<byte> result = new List<byte>();
            while (true)
            {
                if (this.Socket.Connected == false)
                    break;
                byte[] data = new byte[this.BufferSize];
                int recv = 0;
                try
                {
                    recv = this.Socket.Receive(data);
                }
                catch (Exception ex) { Console.WriteLine("Exception : " + ex.Message); }
                //catch (Exception ex) { if (ex.Message != "An existing connection was forcibly closed by the remote host") throw new Exception(ex.Message); }
                if (recv == 0)
                {
                    if (this.ReceiveData != null)
                        this.ReceiveData(this, null);
                    else
                        throw new Exception("Received Data but this event not handled");
                    break;
                }

                if (data[0] == 0)
                {
                    for (int i = 0; i < recv; i++)
                        result.Add(data[i]);
                    byte[] _result = result.ToArray();

                    if (this.ReceiveData != null)
                    {
                        ReceivedData r = new ReceivedData(_result);
                        this.ReceiveData(this, r);
                    }
                    else
                        throw new Exception("Received Data but this event not handled");
                    result = new List<byte>();
                }
                else
                    for (int i = 1; i < recv; i++)
                        result.Add(data[i]);
            }
        }
    }
    public class ReceivedData
    {
        byte[] _data;
        public ReceivedData(byte[] data)
        {
            _data = data;
        }

        public Byte[] ByteData { get { return _data; } }
        public String UniCodeData { get { return System.Text.Encoding.Unicode.GetString(_data); } }
        public String AsciiData { get { return System.Text.Encoding.ASCII.GetString(_data); } }
        public String Utf8Data { get { return System.Text.Encoding.UTF8.GetString(_data); } }
    }
}
