using System;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using LW.NSocket.Client.Response;
using LW.NSocket.SocketBase.Utils;

namespace LW.NSocket.Client
{
    /// <summary>
    /// 异步socket客户端
    /// </summary>
    public class AsyncBinaryXMLSocketClient : PooledSocketClient<AsyncBinaryXMLResponse>
    {
        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        public AsyncBinaryXMLSocketClient()
            : base(new Protocol.AsyncBinaryXMLProtocol())
        {
        }
        /// <summary>
        /// new
        /// </summary>
        /// <param name="socketBufferSize"></param>
        /// <param name="messageBufferSize"></param>
        public AsyncBinaryXMLSocketClient(int socketBufferSize, int messageBufferSize)
            : base(new Protocol.AsyncBinaryXMLProtocol(), socketBufferSize, messageBufferSize, 3000, 3000)
        {
        }
        /// <summary>
        /// new
        /// </summary>
        /// <param name="socketBufferSize"></param>
        /// <param name="messageBufferSize"></param>
        /// <param name="millisecondsSendTimeout"></param>
        /// <param name="millisecondsReceiveTimeout"></param>
        public AsyncBinaryXMLSocketClient(int socketBufferSize,
            int messageBufferSize,
            int millisecondsSendTimeout,
            int millisecondsReceiveTimeout, bool onlySend = false)
            : base(new Protocol.AsyncBinaryXMLProtocol(),
            socketBufferSize,
            messageBufferSize,
            millisecondsSendTimeout,
            millisecondsReceiveTimeout, onlySend)
        {

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// send
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="cmdName"></param>
        /// <param name="payload"></param>
        /// <param name="funcResultFactory"></param>
        /// <param name="asyncState"></param>
        /// <returns></returns>
        public Task<TResult> Send<TResult>(string cmdName, XDocument xmlDoc,
            Func<AsyncBinaryXMLResponse, TResult> funcResultFactory, object asyncState = null)
        {
            return this.Send(null, cmdName, xmlDoc, funcResultFactory, asyncState);
        }
        /// <summary>
        /// new
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="consistentKey"></param>
        /// <param name="cmdName"></param>
        /// <param name="payload"></param>
        /// <param name="funcResultFactory"></param>
        /// <param name="asyncState"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">cmdName is null or empty.</exception>
        /// <exception cref="ArgumentNullException">funcResultFactory is null.</exception>
        public Task<TResult> Send<TResult>(byte[] consistentKey, string cmdName, XDocument xmlDoc,
            Func<AsyncBinaryXMLResponse, TResult> funcResultFactory, object asyncState = null)
        {
            if (string.IsNullOrEmpty(cmdName)) throw new ArgumentNullException("cmdName");
            if (xmlDoc == null) throw new ArgumentNullException("xml");
            if (funcResultFactory == null) throw new ArgumentNullException("funcResultFactory");

            var seqID = base.NextRequestSeqID();
            var uid = DateTime.Now.ToString("yyyyMMdd") +
                            seqID.ToString().PadLeft(int.MaxValue.ToString().Length, '0');

            var head = xmlDoc.Root.Element("head");
            head.Element("MsgType").Remove();
            head.Element("POSSeqNum").Remove();
            head.Add(new XElement("InstId", "商户机构号test"),
                 new XElement("TrmSeqNum", uid));
            var msg = xmlDoc.Declaration + Environment.NewLine + xmlDoc;
            var encoding = Encoding.GetEncoding("GBK");
            var xmlBytes = encoding.GetBytes(msg);

            var data = new byte[xmlBytes.Length + 6];
            Buffer.BlockCopy(encoding.GetBytes(xmlBytes.Length.ToString().PadLeft(6, '0')), 0, data, 0, 6);
            Buffer.BlockCopy(xmlBytes, 0, data, 6, xmlBytes.Length);


            var source = new TaskCompletionSource<TResult>(asyncState);
            base.Send(new Request<Response.AsyncBinaryXMLResponse>(consistentKey, seqID, uid, cmdName, data,
                ex => source.TrySetException(ex),
                response =>
                {
                    TResult result;
                    try { result = funcResultFactory(response); }
                    catch (Exception ex) { source.TrySetException(ex); return; }

                    source.TrySetResult(result);
                }));
            return source.Task;
        }


        #endregion
    }
}