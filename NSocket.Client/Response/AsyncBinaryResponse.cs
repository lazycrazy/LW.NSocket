
namespace LW.NSocket.Client.Response
{
    /// <summary>
    /// async binary response
    /// </summary>
    public class AsyncBinaryResponse : IResponse
    {
        /// <summary>
        /// flag
        /// </summary>
        public readonly string Flag = null;
        /// <summary>
        /// buffer
        /// </summary>
        public readonly byte[] Buffer = null;

        /// <summary>
        /// new
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="seqID"></param>
        /// <param name="buffer"></param>
        public AsyncBinaryResponse(string flag, int seqID, byte[] buffer)
        {
            this.SeqID = seqID;
            this.Flag = flag;
            this.Buffer = buffer;
        }

        public string UID { get; private set; }

        public int SeqID { get; private set; }
    }
}