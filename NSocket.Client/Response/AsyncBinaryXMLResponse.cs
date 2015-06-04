
using System.Xml.Linq;

namespace LW.NSocket.Client.Response
{
    /// <summary>
    /// async binary xml response
    /// </summary>
    public class AsyncBinaryXMLResponse : IResponse
    {

        /// <summary>
        /// buffer
        /// </summary>
        public readonly XDocument XDoc = null;
        public readonly byte[] Buffer = null;
        public readonly string Flag = null;
        /// <summary>
        /// new
        /// </summary>
        /// <param name="seqID"></param>
        /// <param name="buffer"></param>
        public AsyncBinaryXMLResponse(string flag, string uid, XDocument xdoc,byte[] buffer)
        {
            this.UID = uid;
            this.Flag = flag;
            this.XDoc = xdoc;
            this.Buffer = buffer;
        }
        public string UID { get; private set; }

        public int SeqID
        {
            get;
            private set;
        }
    }
}