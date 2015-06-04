using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LW.NGSSocketServer
{
    public static class NGSFields
    {
        public static string CmdName = "GDZF";
        public static string XCmdName = "MsgType";
        public static string XUID = "POSSeqNum";
        public static string XErrorCode = "NGSError";

    }
    public static class GDFields
    {
        public static string ServIP = "127.0.0.1";
        public static int ServPort = 8404;

        public static string CmdName = "GDZFPTTS";

        public static string XDeclaration = @"<?xml version=""1.0"" encoding=""ISO-8859-1""?>" + Environment.NewLine;
        public static string XEncoding = "GBK";

        public static string XRootIn = "in";
        public static string XRootOut = "out";
        public static string XBodyIn = "tin";
        public static string XBodyOut = "tout";
        public static string XHead = "head";

        public static string XCmdName = "AnsTranCode";

        public static string XUID = "TrmSeqNum";

        public static string XClinetCode = "InstId";
        public static string ClinetCode = "机构编码XXX";

        public static int XMsgLengthPlaces = 6;
        public static int XMACLengthPlaces = 16;
    }
}
