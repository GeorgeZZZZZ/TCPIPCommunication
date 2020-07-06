using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;

namespace Georgescript
{
    public static class TCPClientIsAlive
    {
        public static bool IsConnected(TcpClient _c)
        {
            // not alive
            if (!_c.Connected) return false;
            try
            {
                return !(_c.Client.Poll(1, SelectMode.SelectRead) && _c.Available == 0);
            }
            catch (SocketException)
            {
                return false;
            }
        }
    } 
}
