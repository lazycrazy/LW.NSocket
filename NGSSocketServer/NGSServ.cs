using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using LW.NSocket.Client;
using LW.NSocket.Client.Protocol;
using LW.NSocket.Client.Response;
using LW.NSocket.Server;
using LW.NSocket.Server.Command;
using LW.NSocket.SocketBase;
using NLog.Fluent;


namespace LW.NGSSocketServer
{
    public class NGSServ
    {
        private static AsyncBinaryXMLSocketGDClient<AsyncBinaryXMLResponse> GDClient;
        private static IPEndPoint GDSerPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(GDFields.ServIP), GDFields.ServPort);
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        public static void Start()
        {
            LW.NSocket.SocketBase.Log.Trace.Info("服务开始启动...");
            GDClient = new LW.NSocket.Client.AsyncBinaryXMLSocketGDClient<AsyncBinaryXMLResponse>(new AsyncBinaryXMLProtocol(), 8192, 8192, 3000, 3000 * 10, GDSerPoint, true);
            GDClient.OnSendSuccess += GDClient_OnSendSuccess;
            SocketServerManager.Init();
            SocketServerManager.Start();
            LW.NSocket.SocketBase.Log.Trace.Info("服务已启动！");
        }

        static void GDClient_OnSendSuccess(IConnection connection, Request<AsyncBinaryXMLResponse> request)
        {
            _logger.Info().Message(" sended to GD outer message ").Property("ClientIP", connection.RemoteEndPoint).Property("XML", Encoding.GetEncoding(GDFields.XEncoding).GetString(request.Payload)).Write();

        }

        public static void Stop()
        {
            LW.NSocket.SocketBase.Log.Trace.Info("服务开始关闭...");
            SocketServerManager.Stop();
            GDClient.Stop();
            LW.NSocket.SocketBase.Log.Trace.Info("服务已关闭！");
        }

        public static void InnerReceiveMessage(IConnection connection, AsyncBinaryXMLInnerCommandInfo cmdInfo)
        {
            _logger.Info().Message(" received POS inner message ").Property("ClientIP", connection.RemoteEndPoint).Property("XML", cmdInfo.XDoc.ToString()).Write();

            var outEle = new XElement(cmdInfo.XDoc.Root);
            //outEle.Name = "out";
            //outEle.Element("tin").Name = "tout";


            var outXDoc = XDocument.Parse(GDFields.XDeclaration + outEle.ToString());
            var ansTranCode = outEle.Element(GDFields.XHead).Element(GDFields.XCmdName).Value;
            //var sendMsg = Encoding.GetEncoding("GBK").GetBytes(@"<?xml version=""1.0"" encoding=""ISO-8859-1""?>" + Environment.NewLine + outEle.ToString());
            //var length = sendMsg.Length.ToString().PadLeft(6, '0');
            //var data = new byte[sendMsg.Length + 6];
            //Buffer.BlockCopy(Encoding.ASCII.GetBytes(length), 0, data, 0, 6);
            //Buffer.BlockCopy(sendMsg, 0, data, 6, sendMsg.Length);

            GDClient.Send(null, ansTranCode, outXDoc, res =>
                {
                    var head = res.XDoc.Root.Element(GDFields.XHead);
                    head.Element(GDFields.XClinetCode).Remove();
                    head.Element(GDFields.XUID).Remove();
                    head.Add(new XElement(NGSFields.XCmdName, cmdInfo.CmdName));
                    head.Add(new XElement(NGSFields.XUID, cmdInfo.UID));
                    return res;
                }).ContinueWith((Task<AsyncBinaryXMLResponse> t) =>
                    {
                        if (t.IsFaulted)
                        {
                            LW.NSocket.SocketBase.Log.Trace.Error(t.Exception.Message, t.Exception);
                            var errorXML = GetErrorXmlBytes(cmdInfo, t.Exception.ToString());
                            cmdInfo.Reply(connection, errorXML);
                            _logger.Error().Message(" send to POS inner message " + errorXML).Property("ClientIP", connection.RemoteEndPoint).Property("XML", cmdInfo.XDoc.ToString()).Write();
                            return;
                        }
                        cmdInfo.Reply(connection, GetXmlBytes(t.Result.XDoc));
                        _logger.Info().Message(" send to POS inner message ").Property("ClientIP", connection.RemoteEndPoint).Property("XML", t.Result.XDoc.ToString()).Write();
                    });

        }
        public static void OuterReceiveMessage(IConnection connection, AsyncBinaryXMLOuterCommandInfo cmdInfo)
        {
            _logger.Info().Message(" received GD outer message ").Property("ClientIP", connection.RemoteEndPoint).Property("XML", cmdInfo.XDoc.ToString()).Write();

            AsyncBinaryXMLResponse response = null;

            response = ConvertCmdToRep(cmdInfo);
            if (response == null)
            {
                _logger.Info().Message(" ConvertCmdToRep error ").Property("ClientIP", connection.RemoteEndPoint).Property("XML", cmdInfo.XDoc.ToString()).Write();
            }
            GDClient.MessageReceived(response);

        }
        private static AsyncBinaryXMLResponse ConvertCmdToRep(AsyncBinaryXMLOuterCommandInfo cmdInfo)
        {
            var head = cmdInfo.XDoc.Root.Element(GDFields.XHead);
            var flag = head.Element(GDFields.XCmdName).Value;
            var uid = head.Element(GDFields.XUID).Value;

            return new AsyncBinaryXMLResponse(flag, uid, cmdInfo.XDoc, cmdInfo.Buffer);
        }

        public static byte[] GetXmlBytes(XDocument xdoc)
        {
            return GetXmlBytes(xdoc.ToString());
        }

        public static byte[] GetXmlBytes(string message)
        {
            var bytes = Encoding.GetEncoding(GDFields.XEncoding).GetBytes(message);
            var msgLength = bytes.Length;
            var data = new byte[msgLength + GDFields.XMsgLengthPlaces];
            Buffer.BlockCopy(Encoding.GetEncoding(GDFields.XEncoding).GetBytes(msgLength.ToString().PadLeft(GDFields.XMsgLengthPlaces, '0')), 0, data, 0, GDFields.XMsgLengthPlaces);
            Buffer.BlockCopy(bytes, 0, data, GDFields.XMsgLengthPlaces, msgLength);
            return data;
        }

        public static byte[] GetErrorXmlBytes(AsyncBinaryXMLInnerCommandInfo cmdInfo, string errorMsg)
        {
            var msg = @"<?xml version=""1.0"" encoding=""ISO-8859-1""?>
<out><head><Version>1.0.1</Version><{0}>{1}</{0}><{2}>Error</{2}><{3}>{4}</{3}></head>
<tout><errorCode>{5}</errorCode><errorMessage>{6}</errorMessage><errorDetail></errorDetail></tout></out>";
            var message = string.Format(msg, NGSFields.XCmdName, NGSFields.CmdName, GDFields.XCmdName, NGSFields.XUID, cmdInfo.UID, NGSFields.XErrorCode,
                                        System.Security.SecurityElement.Escape(errorMsg));
            return GetXmlBytes(message);
        }
    }
}
