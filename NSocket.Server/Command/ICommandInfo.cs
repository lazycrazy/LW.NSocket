
namespace LW.NSocket.Server.Command
{
    /// <summary>
    /// command info interface.
    /// </summary>
    public interface ICommandInfo
    {
        /// <summary>
        /// get the command name
        /// </summary>
        string CmdName { get; }
    }
}