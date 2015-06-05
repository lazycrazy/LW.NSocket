using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using LW.NSocket.Client;
using LW.NSocket.Server;
using LW.NSocket.Server.Command;
using LW.NSocket.SocketBase;

namespace LW.NGSSocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            LW.NSocket.SocketBase.Log.Trace.EnableDiagnostic();
            NGSServ.Start();

            Console.ReadLine();
        }
    }

}
