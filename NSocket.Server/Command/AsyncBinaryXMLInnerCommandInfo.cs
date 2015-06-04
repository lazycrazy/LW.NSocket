using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using LW.NSocket.SocketBase;

namespace LW.NSocket.Server.Command
{
    public class AsyncBinaryXMLInnerCommandInfo : ICommandInfo
    {
        public AsyncBinaryXMLInnerCommandInfo(string cmdName, string uid, XDocument xdoc, byte[] buffer)
        {
            MsgType = CmdName = cmdName;
            XDoc = xdoc;
            Buffer = buffer;
            UID = uid;
        }



        public string MsgType
        {
            get;
            private set;
        }

        public string UID
        {
            get;
            private set;
        }

        public XDocument XDoc { get; private set; }

        public byte[] Buffer
        {
            get;
            private set;
        }

        public string CmdName
        {
            get;
            private set;
        }
        public void Reply(SocketBase.IConnection connection, byte[] payload)
        {
            connection.BeginSend(new SocketBase.Packet(payload));
        }
    }
}
