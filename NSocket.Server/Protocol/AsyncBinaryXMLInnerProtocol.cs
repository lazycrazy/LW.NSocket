using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using LW.NGSSocketServer;

namespace LW.NSocket.Server.Protocol
{

    public class AsyncBinaryXMLInnerProtocol : IProtocol<Command.AsyncBinaryXMLInnerCommandInfo>
    {

        public Command.AsyncBinaryXMLInnerCommandInfo FindCommandInfo(SocketBase.IConnection connection, ArraySegment<byte> buffer, int maxMessageSize, out int readlength)
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
            var xdoc = XDocument.Parse(message);
            var head = xdoc.Root.Element(GDFields.XHead);

            var data = new byte[messageLength];
            Buffer.BlockCopy(payload, buffer.Offset + GDFields.XMsgLengthPlaces, data, 0, messageLength);
            return new Command.AsyncBinaryXMLInnerCommandInfo(head.Element(NGSFields.XCmdName).Value, head.Element(NGSFields.XUID).Value, xdoc, data);
        }
    }
}
