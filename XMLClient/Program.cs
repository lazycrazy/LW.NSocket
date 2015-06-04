using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLClient
{
    class Program
    {
        static void Main(string[] args)
        {
            LW.NSocket.SocketBase.Log.Trace.EnableConsole();
            LW.NSocket.SocketBase.Log.Trace.EnableDiagnostic();

            var client = new LW.NSocket.Client.AsyncBinaryXMLSocketClient(8192, 8192, 3000, 3000,false);
            var gdserName = "127.0.0.1:8402";
            //注册服务器节点，这里可注册多个(name不能重复）
            client.RegisterServerNode(gdserName, new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 8402));
            //client.RegisterServerNode("127.0.0.1:8402", new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.2"), 8401));

            var msg = @"<?xml version=""1.0"" encoding=""ISO-8859-1"" ?>
<in>
	<head>
		<Version>1.0.1</Version>
		<MsgType>消息类别</MsgType>
		<AnsTranCode>交易码</AnsTranCode>
		<POSSeqNum>POS跟踪码</POSSeqNum>
	</head>
<tin>
<TAG1>VALUE1</TAG1>
<TAG2>VALUE2</TAG2>
</tin>
</in>
";

            //client.Send("GDZF", msg, res => BitConverter.ToInt32(res.Buffer, 0)).ContinueWith(c =>
            //    {
            //        client.UnRegisterServerNode(gdserName);
            //        if (c.IsFaulted)
            //        {
            //            Console.WriteLine(c.Exception.ToString());
            //            return;
            //        }
            //        Console.WriteLine(c.Result);
            //    });


            Console.ReadLine();
        }
    }
}
