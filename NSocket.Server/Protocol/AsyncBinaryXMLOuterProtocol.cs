using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using LW.NGSSocketServer;

namespace LW.NSocket.Server.Protocol
{

    public class AsyncBinaryXMLOuterProtocol : IProtocol<Command.AsyncBinaryXMLOuterCommandInfo>
    {

        public Command.AsyncBinaryXMLOuterCommandInfo FindCommandInfo(SocketBase.IConnection connection, ArraySegment<byte> buffer, int maxMessageSize, out int readlength)
        {
            if (buffer.Count < GDFields.XMsgLengthPlaces) { readlength = 0; return null; }

            var payload = buffer.Array;

            //获取message length
            int messageLength;
            if (!int.TryParse(Encoding.ASCII.GetString(payload, buffer.Offset, GDFields.XMsgLengthPlaces).TrimStart('0'), out messageLength))
            {
                throw new BadProtocolException("bad async binary xml protocl:message length parse error ");
            }

            if (messageLength < 7) throw new BadProtocolException("bad async binary protocl");
            if (messageLength > maxMessageSize) throw new BadProtocolException("message is too long");

            readlength = messageLength + GDFields.XMsgLengthPlaces;
            if (buffer.Count < readlength)
            {
                readlength = 0; return null;
            }

            var message = Encoding.GetEncoding(GDFields.XEncoding).GetString(payload, buffer.Offset + GDFields.XMsgLengthPlaces, messageLength);
            var xdoc = XDocument.Parse(message); var head = xdoc.Root.Element(GDFields.XHead);
            var cmdName = GDFields.CmdName;
            var uid = head.Element(GDFields.XUID).Value;

            var data = new byte[messageLength];
            Buffer.BlockCopy(payload, buffer.Offset + GDFields.XMsgLengthPlaces, data, 0, messageLength);
            return new Command.AsyncBinaryXMLOuterCommandInfo(cmdName, uid, xdoc, data);
        }
    }
}
