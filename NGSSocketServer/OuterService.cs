using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using LW.NSocket.Client;
using LW.NSocket.Server;
using LW.NSocket.Server.Command;
using LW.NSocket.SocketBase;
using NLog;
using NLog.Fluent;

namespace LW.NGSSocketServer
{

    public class OuterService : CommandSocketService<AsyncBinaryXMLOuterCommandInfo>
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 当连接时会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        public override void OnConnected(IConnection connection)
        {
            base.OnConnected(connection);
            Console.WriteLine(connection.RemoteEndPoint.ToString() + " connected");
            _logger.Info().Message("connected").Property("ClientIP", connection.RemoteEndPoint).Write();

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
            _logger.Info().Message("disconnected").Property("ClientIP", connection.RemoteEndPoint).Write();
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
            _logger.Error().Message(ex.ToString()).Property("ClientIP", connection.RemoteEndPoint).Write();
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
            _logger.Info().Message("send " + e.Status.ToString()).Property("ClientIP", connection.RemoteEndPoint).Write();
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
    /// 光大支付平台推送信息
    /// </summary>
    public sealed class GDZFPTTSCommand : ICommand<AsyncBinaryXMLOuterCommandInfo>
    {
        /// <summary>
        /// 返回服务名称
        /// </summary>
        public string Name
        {
            get { return GDFields.CmdName; }
        }
        /// <summary>
        /// 执行命令并返回结果
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandInfo"></param>
        public void ExecuteCommand(IConnection connection, AsyncBinaryXMLOuterCommandInfo commandInfo)
        {
            NGSServ.OuterReceiveMessage(connection, commandInfo);
        }
    }

}
