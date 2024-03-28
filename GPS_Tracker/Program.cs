using GPS_Tracker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Sockets;
using System.Reflection;




Redis.Configure();
RSocket ServerSocket = new RSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
IPEndPoint ServerIP = new IPEndPoint(IPAddress.Any, 9090);

    ServerSocket.Bind(ServerIP);
    ServerSocket.Listen(100);
    Thread StartT = new Thread(Start);
    StartT.Start();


void Start()
{
    Console.WriteLine("Started Service");
    int connectionCount = 1;
    while (true)
    {
        try
        {
            
            ConnectionThread connectionThread = new ConnectionThread();
            connectionThread.SetNewId();
            connectionThread.client = ServerSocket.Accept();
            ConnectionThread.allSocket.Add(connectionThread.id, connectionThread.client);
            connectionThread.client.ReceiveData += new RSocket._ReceiveData(connectionThread.client_ReceiveData);
            connectionCount++;
        }
        catch { break; }
    }
}