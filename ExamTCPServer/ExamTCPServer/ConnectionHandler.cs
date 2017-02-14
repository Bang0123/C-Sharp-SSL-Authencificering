using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ExamTCPServer
{
    public class ConnectionHandler
    {
        private TcpClient connectionSocket;
        private string serverCertificateFile = "../../../../ExamServerSSL.cer";
        private bool clientCertificateRequired = true;
        private bool checkCertificateRevocation = false;
        private SslProtocols enabledSSLProtocols = SslProtocols.Tls;
        private X509Certificate serverCertificate;

        public ConnectionHandler(TcpClient tcpClient)
        {
            connectionSocket = tcpClient;
            var x = Directory.GetCurrentDirectory();

        }

        public void ServiceConnection()
        {
            serverCertificate = new X509Certificate(serverCertificateFile, "exam2017");
            Stream ns = connectionSocket.GetStream();
            try
            {
                SslStream sslStream = new SslStream(ns, false, ValidateClientCertificate);
                sslStream.AuthenticateAsServer(serverCertificate, clientCertificateRequired, enabledSSLProtocols,
                    checkCertificateRevocation);
                StreamReader sr = new StreamReader(sslStream);
                StreamWriter sw = new StreamWriter(sslStream);
                sw.AutoFlush = true;
                string message = sr.ReadLine();
                string answer = "";

                while (!string.IsNullOrEmpty(message))
                {
                    if (message.StartsWith("GET"))
                    {
                        string[] mArray = message.Split(' ');

                        if (mArray[1].ToLower().Contains("gm") && mArray[3].ToLower().Contains("fod"))
                        {
                            answer = "GetMeter: " + GetMeter(double.Parse(mArray[2]));
                        }
                        if (mArray[1].ToLower().Contains("gf") && mArray[3].ToLower().Contains("meter"))
                        {
                            answer = "GetFeet: " + GetFeet(double.Parse(mArray[2]));
                        }
                        if (mArray[1].ToLower().Contains("gfh"))
                        {
                            answer = "GetFullHeight: " + GetFullHeight(double.Parse(mArray[2]), double.Parse(mArray[3]));
                        }

                        sw.WriteLine(answer);
                    }
                    message = sr.ReadLine();
                    Console.WriteLine("Client: " + message);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Client DC: " + this.GetHashCode());
            }
            ns.Close();
            connectionSocket.Close();
        }

        private bool ValidateClientCertificate(Object sender, X509Certificate remoteCertificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (remoteCertificate.Issuer == serverCertificate.Issuer)
            {
                return true;
            }
            return false;
        }

        public double GetMeter(double lengthF)
        {
            return 0.3048 * lengthF;
        }

        public double GetFeet(double lengthM)
        {
            return lengthM / 0.3048;
        }

        public double GetFullHeight(double lengthF, double lengthT)
        {
            return GetMeter(lengthF) + ConvertTommerToMeter(lengthT);
        }

        private double ConvertTommerToMeter(double lengthT)
        {
            return 0.0254 * lengthT;
        }
    }
}