﻿using System;

namespace LW.NSocket.SocketBase
{
    /// <summary>
    /// connection disconnected delegate
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="ex"></param>
    public delegate void DisconnectedHandler(IConnection connection, Exception ex);
}