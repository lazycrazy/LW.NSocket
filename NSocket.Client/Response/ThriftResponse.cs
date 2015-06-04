
namespace LW.NSocket.Client.Response
{
    /// <summary>
    /// thrift response.
    /// </summary>
    public class ThriftResponse : IResponse
    {
        /// <summary>
        /// buffer
        /// </summary>
        public readonly byte[] Buffer = null;

        /// <summary>
        /// new
        /// </summary>
        /// <param name="seqID"></param>
        /// <param name="buffer"></param>
        public ThriftResponse(int seqID, byte[] buffer)
        {
            this.SeqID = seqID;
            this.Buffer = buffer;
        }



        public int SeqID { get; private set; }

        public string UID
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}