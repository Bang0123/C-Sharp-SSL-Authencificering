using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ExamTCPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener serverSocket = new TcpListener(IPAddress.Parse("127.0.0.1"), 9999);
            serverSocket.Start();
            Console.WriteLine("Server started");
            while (true)
            {
                ConnectionHandler conn = new ConnectionHandler(serverSocket.AcceptTcpClient());
                Task.Run(() => conn.ServiceConnection());
                Console.WriteLine("Client Connected: " + conn.GetHashCode());
            }


            Console.ReadLine();
        }
    }
}
