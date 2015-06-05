using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            string host;
            int port = 8403;

            if (args.Length == 0)
                // If no server name is passed as argument to this program, 
                // use the current host name as the default.
                host = Dns.GetHostName();
            else
                host = args[0];

            string result = SocketSendReceive(host, port);
            Console.WriteLine(result);
            Console.ReadLine();
        }


        private static Socket ConnectSocket(string server, int port)
        {
            Socket s = null;

            IPEndPoint ipe = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), port);
            //IPEndPoint ipe = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("10.10.10.203"), port);
            Socket tempSocket =
                new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            tempSocket.Connect(ipe);

            if (tempSocket.Connected)
            {
                s = tempSocket;

            }
            return s;
        }

        private static string SocketSendReceive(string server, int port)
        {
            string request = string.Format(@"<?xml version=""1.0"" encoding=""ISO-8859-1"" ?>
<out>
	<head>
		<Version>1.0.1</Version>		
		<AnsTranCode>BJCEBRWKRes</AnsTranCode>
		<TrmSeqNum>{0}</TrmSeqNum>
        <InstId>商户编号test</InstId>
	</head>
<tout>
<TAG1>VALUE1</TAG1>
<TAG2>VALUE2</TAG2>
</tout>
</out>
", DateTime.Now.ToString("yyyyMMdd") + "0000000001");
            var length = Encoding.GetEncoding("GBK").GetByteCount(request).ToString().PadLeft(6, '0');
            var bytesSent = Encoding.GetEncoding("GBK").GetBytes((length + request));
            Byte[] bytesReceived = new Byte[256];
            Socket s = ConnectSocket(server, port);
            if (s == null)
                return ("Connection failed");
            s.Send(bytesSent, bytesSent.Length, 0);

            int bytes = 0;
            string response = "";

            do
            {
                bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
                response = response + Encoding.GetEncoding("GBK").GetString(bytesReceived, 0, bytes);
            }
            while (bytes == bytesReceived.Length);
            return response;
        }
    }
}
