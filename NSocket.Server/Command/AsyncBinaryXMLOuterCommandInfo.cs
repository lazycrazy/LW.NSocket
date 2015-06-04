using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LW.NSocket.Server.Command
{
    public class AsyncBinaryXMLOuterCommandInfo : ICommandInfo
    {
        public AsyncBinaryXMLOuterCommandInfo(string cmdName, string uid, XDocument xdoc, byte[] buffer)
        {
            XDoc = xdoc;
            AnsTranCode = CmdName = cmdName;
            Buffer = buffer;
            UID = uid;
        }
        public string AnsTranCode
        {
            get;
            private set;
        }

        public string UID
        {
            get;
            private set;
        }

        public byte[] Buffer
        {
            get;
            private set;
        }

        public XDocument XDoc { get; private set; }


        public int SeqID
        {
            get;
            private set;
        }
        public string CmdName
        {
            get;
            private set;
        }
    }
}
