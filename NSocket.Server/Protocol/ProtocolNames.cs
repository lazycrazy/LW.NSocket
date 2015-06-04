
namespace LW.NSocket.Server.Protocol
{
    /// <summary>
    /// ProtocolNames
    /// </summary>
    static public class ProtocolNames
    {
        /// <summary>
        /// 异步binary协议
        /// </summary>
        public const string AsyncBinary = "asyncBinary";
        /// <summary>
        /// 异步binaryXML协议
        /// </summary>
        public const string AsyncBinaryXMLInner = "asyncBinaryXMLInner";
        /// <summary>
        /// 异步binaryXML协议
        /// </summary>
        public const string AsyncBinaryXMLOuter = "asyncBinaryXMLOuter";
        /// <summary>
        /// thrift协议
        /// </summary>
        public const string Thrift = "thrift";
        /// <summary>
        /// 命令行协议
        /// </summary>
        public const string CommandLine = "commandLine";
    }
}