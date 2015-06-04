
namespace LW.NSocket.Client.Response
{
    /// <summary>
    /// response interface.
    /// </summary>
    public interface IResponse
    {
        int SeqID { get; }
        string UID { get; }
    }

}