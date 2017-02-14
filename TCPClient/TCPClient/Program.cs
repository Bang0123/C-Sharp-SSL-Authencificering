using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TCPClient
{
    class Program
    {
        private static string serverCertificateFile = "../../../../ExamServerSSL.cer";
        private static X509Certificate serverCertificate;
        static void Main(string[] args)
        {
            serverCertificate = new X509Certificate(serverCertificateFile, "exam2017");
            TcpClient clientSocket = new TcpClient("127.0.0.1", 9999);
            NetworkStream netStream = clientSocket.GetStream();
            SslStream sslStream = new SslStream(netStream, false, ValidateClientCertificate);
            var x = new X509CertificateCollection {serverCertificate};
            sslStream.AuthenticateAsClient("ExamServerName", x, SslProtocols.Tls, false);
            StreamReader sr = new StreamReader(sslStream);
            StreamWriter sw = new StreamWriter(sslStream);
            sw.AutoFlush = true;
            Console.WriteLine("Example of the protokol: ");
            Console.WriteLine("Get meters from foot: GET GM 5 fod");
            Console.WriteLine("Get foot from meters: GET GF 2 meter");
            Console.WriteLine("Get someones full height: GET GFH 6 2");
            while (true)
            {
                Console.WriteLine("Type a message: ");
                var message = Console.ReadLine();
                sw.WriteLine(message);


                string serverAnswer = sr.ReadLine();
                Console.WriteLine("Server: " + serverAnswer);
            }



        }
        private static bool ValidateClientCertificate(Object sender, X509Certificate remoteCertificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (remoteCertificate.Issuer == serverCertificate.Issuer)
            {
                return true;
            }
            return false;
        }
    }
}
