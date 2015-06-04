using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using LW.NGSSocketServer;
using LW.NSocket.Client.Response;
using LW.NSocket.SocketBase;

namespace LW.NSocket.Client
{
    public class AsyncBinaryXMLSocketGDClient<TResponse> : BaseSocketClient<TResponse> where TResponse : class, Response.IResponse
    {
        #region Private Members
        private readonly EndPoint ServerPoint = null;
        #endregion

        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        /// <param name="protocol"></param>
        public AsyncBinaryXMLSocketGDClient(Protocol.IProtocol<TResponse> protocol)
            : this(protocol, 8192, 9192, 3000, 3000, null)
        {
        }
        /// <summary>
        /// new
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="socketBufferSize"></param>
        /// <param name="messageBufferSize"></param>
        /// <param name="millisecondsSendTimeout"></param>
        /// <param name="millisecondsReceiveTimeout"></param>
        /// <exception cref="ArgumentNullException">protocol is null</exception>
        public AsyncBinaryXMLSocketGDClient(Protocol.IProtocol<TResponse> protocol,
            int socketBufferSize,
            int messageBufferSize,
            int millisecondsSendTimeout,
            int millisecondsReceiveTimeout, EndPoint serverPoint, bool onlySend = false)
            : base(protocol, socketBufferSize, messageBufferSize, millisecondsSendTimeout, millisecondsReceiveTimeout, onlySend)
        {
            ServerPoint = serverPoint;
        }
        #endregion

        #region Protected Methods


        #endregion

        #region Override Methods
        /// <summary>
        /// send request
        /// </summary>
        /// <param name="request"></param>
        protected override void Send(Request<TResponse> request)
        {
            new SocketConnector("SocketConnectorName", ServerPoint, this, (n, c) =>
            {
                OnConnected(n, c);
                c.BeginSend(request);

            },
            OnDisconnected,
            node =>
            {
                node.Stop();
                try
                {
                    request.SetException(
                        new RequestException(RequestException.Errors.GDServerConnectFailed,
                                                request.CmdName));
                }
                catch (Exception ex)
                {
                    SocketBase.Log.Trace.Error(ex.Message, ex);
                }
            }
            ).Start();
        }

        public event Action<IConnection, Request<TResponse>> OnSendSuccess;

        protected override void OnSendSucess(IConnection connection, Request<TResponse> request)
        {
            connection.BeginDisconnect();
            if (OnSendSuccess != null)
                OnSendSuccess(connection, request);
        }
        #endregion


        #region Public Method
        public Task<TResult> Send<TResult>(byte[] consistentKey, string cmdName, XDocument xmlDoc,
            Func<TResponse, TResult> funcResultFactory, object asyncState = null)
        {
            if (string.IsNullOrEmpty(cmdName)) throw new ArgumentNullException("cmdName");
            if (xmlDoc == null) throw new ArgumentNullException("xml");
            if (funcResultFactory == null) throw new ArgumentNullException("funcResultFactory");

            var seqID = base.NextRequestSeqID();
            var uid = DateTime.Now.ToString("yyyyMMdd") +
                            seqID.ToString().PadLeft(int.MaxValue.ToString().Length, '0');

            var head = xmlDoc.Root.Element(GDFields.XHead);
            head.Element(NGSFields.XCmdName).Remove();
            head.Element(NGSFields.XUID).Remove();
            head.Add(new XElement(GDFields.XClinetCode, GDFields.ClinetCode),
                 new XElement(GDFields.XUID, uid));
            var msg = xmlDoc.Declaration + Environment.NewLine + xmlDoc;
            var encoding = Encoding.GetEncoding(GDFields.XEncoding);
            var xmlBytes = encoding.GetBytes(msg);

            var data = new byte[xmlBytes.Length + GDFields.XMsgLengthPlaces];
            Buffer.BlockCopy(encoding.GetBytes(xmlBytes.Length.ToString().PadLeft(GDFields.XMsgLengthPlaces, '0')), 0, data, 0, GDFields.XMsgLengthPlaces);
            Buffer.BlockCopy(xmlBytes, 0, data, GDFields.XMsgLengthPlaces, xmlBytes.Length);


            var source = new TaskCompletionSource<TResult>(asyncState);
            var req = new Request<TResponse>(consistentKey, seqID, uid, cmdName, data,
                                                                   ex => source.TrySetException(ex),
                                                                   response =>
                                                                   {
                                                                       TResult result;
                                                                       try
                                                                       {
                                                                           result = funcResultFactory(response);
                                                                       }
                                                                       catch (Exception ex)
                                                                       {
                                                                           source.TrySetException(ex);
                                                                           return;
                                                                       }

                                                                       source.TrySetResult(result);
                                                                   });
            this.Send(req);
            return source.Task;
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// on connected
        /// </summary>
        /// <param name="node"></param>
        /// <param name="connection"></param>
        private void OnConnected(SocketConnector node, SocketBase.IConnection connection)
        {
            base.RegisterConnection(connection);
        }
        /// <summary>
        /// on disconnected
        /// </summary>
        /// <param name="node"></param>
        /// <param name="connection"></param>
        private void OnDisconnected(SocketConnector node, SocketBase.IConnection connection)
        {
            node.Stop();
        }
        #endregion
    }
}