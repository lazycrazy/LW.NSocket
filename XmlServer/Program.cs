using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using LW.NSocket.Server;
using LW.NSocket.Server.Command;
using LW.NSocket.SocketBase;

namespace XmlServer
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketServerManager.Init();
            SocketServerManager.Start();

            Console.ReadLine();
        }
    }



    /// <summary>
    /// 实现自定义服务
    /// </summary>
    public class MyService : CommandSocketService<AsyncBinaryXMLOuterCommandInfo>
    {
        /// <summary>
        /// 当连接时会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        public override void OnConnected(IConnection connection)
        {
            base.OnConnected(connection);
            Console.WriteLine(connection.RemoteEndPoint.ToString() + " connected");
        }
        /// <summary>
        /// 当连接断开时会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        public override void OnDisconnected(IConnection connection, Exception ex)
        {
            base.OnDisconnected(connection, ex);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(connection.RemoteEndPoint.ToString() + " disconnected");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        /// <summary>
        /// 当发生错误时会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        public override void OnException(IConnection connection, Exception ex)
        {
            base.OnException(connection, ex);
            Console.WriteLine("error: " + ex.ToString());
        }
        /// <summary>
        /// 当服务端发送Packet完毕会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="e"></param>
        public override void OnSendCallback(IConnection connection, SendCallbackEventArgs e)
        {
            base.OnSendCallback(connection, e);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("send " + e.Status.ToString());
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        /// <summary>
        /// 处理未知的命令
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandInfo"></param>
        protected override void HandleUnKnowCommand(IConnection connection, AsyncBinaryXMLOuterCommandInfo commandInfo)
        {
            Console.WriteLine("unknow command: " + commandInfo.CmdName);
        }
    }

    /// <summary>
    /// sum command
    /// 光大支付平台命令处理
    /// </summary>
    public sealed class GDZFPTTSCommand : ICommand<AsyncBinaryXMLOuterCommandInfo>
    {
        /// <summary>
        /// 返回服务名称
        /// </summary>
        public string Name
        {
            get { return "GDZFPTTS"; }
        }
        /// <summary>
        /// 执行命令并返回结果
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandInfo"></param>
        public void ExecuteCommand(IConnection connection, AsyncBinaryXMLOuterCommandInfo commandInfo)
        {
            var msg = Encoding.GetEncoding("GBK").GetString(commandInfo.Buffer);
            var inXDoc = XDocument.Parse(msg);
            //var outXdoc = new XDocument(new XElement("out", new XElement("head"), new XElement("tout")));
            //var sendMsg = Encoding.GetEncoding("GBK").GetBytes(@"<?xml version=""1.0"" encoding=""ISO-8859-1""?>" + Environment.NewLine + outXdoc.ToString());
            //var length = sendMsg.Length.ToString().PadLeft(6, '0');
            //var data = new byte[sendMsg.Length + 6];
            //Buffer.BlockCopy(Encoding.ASCII.GetBytes(length), 0, data, 0, 6);
            //Buffer.BlockCopy(sendMsg, 0, data, 6, sendMsg.Length);

            //commandInfo.Reply(connection, data);
        }
    }
}
