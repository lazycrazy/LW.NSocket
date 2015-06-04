using System;
using System.Text;
using System.Xml.Linq;
using LW.NGSSocketServer;

namespace LW.NSocket.Client.Protocol
{
    /// <summary>
    /// 异步二进制协议
    /// 协议格式
    /// [Message Length(int32)][SeqID(int32)][Request|Response Flag Length(int16)][Request|Response Flag + Body Buffer]
    /// </summary>
    public sealed class AsyncBinaryXMLProtocol : IProtocol<Response.AsyncBinaryXMLResponse>
    {
        #region IProtocol Members
        /// <summary>
        /// find response
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="buffer"></param>
        /// <param name="readlength"></param>
        /// <returns></returns>
        /// <exception cref="BadProtocolException">bad async binary protocl</exception>
        public Response.AsyncBinaryXMLResponse FindResponse(SocketBase.IConnection connection, ArraySegment<byte> buffer, out int readlength)
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


            readlength = messageLength + GDFields.XMsgLengthPlaces;
            if (buffer.Count < readlength)
            {
                readlength = 0; return null;
            }

            var message = Encoding.GetEncoding(GDFields.XEncoding).GetString(payload, buffer.Offset + GDFields.XMsgLengthPlaces, messageLength);
            var xdoc = XDocument.Parse(message);
            var head = xdoc.Root.Element(GDFields.XHead);
            var flag = head.Element(GDFields.XClinetCode).Value;
            var UID = head.Element(GDFields.XUID).Value;

            byte[] data = new byte[messageLength];
            Buffer.BlockCopy(buffer.Array, buffer.Offset + GDFields.XMsgLengthPlaces, data, 0, messageLength);
            return new Response.AsyncBinaryXMLResponse(flag, UID, xdoc, data);

        }
        #endregion
    }
}