using System;
using LW.NSocket.Server;
using LW.NSocket.Server.Command;
using LW.NSocket.SocketBase;
using NLog;
using NLog.Fluent;


namespace LW.NGSSocketServer
{

    public class InnerService : CommandSocketService<AsyncBinaryXMLInnerCommandInfo>
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
            _logger.Info().Message("disconnected" + ex == null ? "" : ex.ToString()).Property("ClientIP", connection.RemoteEndPoint).Write();

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
            connection.BeginSend(new Packet(NGSServ.GetErrorXmlBytes(new AsyncBinaryXMLInnerCommandInfo(NGSFields.XCmdName, "0000000000000000", null, null), "error: " + ex.ToString())));
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
        protected override void HandleUnKnowCommand(IConnection connection, AsyncBinaryXMLInnerCommandInfo commandInfo)
        {
            var msg = "unknow command: " + commandInfo.CmdName;
            Console.WriteLine(msg);
            _logger.Error().Message(msg).Property("ClientIP", connection.RemoteEndPoint).Property("XML", commandInfo.XDoc.ToString()).Write();
            commandInfo.Reply(connection, NGSServ.GetErrorXmlBytes(commandInfo, msg));
        }
    }


    /// <summary>
    /// sum command
    /// 光大支付平台命令处理
    /// </summary>
    public sealed class GDZFCommand : ICommand<AsyncBinaryXMLInnerCommandInfo>
    {
        /// <summary>
        /// 返回服务名称
        /// </summary>
        public string Name
        {
            get { return NGSFields.CmdName; }
        }
        /// <summary>
        /// 执行命令并返回结果
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandInfo"></param>
        public void ExecuteCommand(IConnection connection, AsyncBinaryXMLInnerCommandInfo commandInfo)
        {
            NGSServ.InnerReceiveMessage(connection, commandInfo);
        }
    }

}
