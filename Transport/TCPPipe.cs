using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EliphatFS.Transport
{
    /// <summary>
    /// 1-to-1 between two TCP endpoints.
    /// Acts as clients on both source and destination ends.
    /// </summary>
    public class TCPPipe : TCPForwarderBase
    {
        public IPEndPoint dst;
        public IPEndPoint src;

        public TCPPipe(IPEndPoint source, IPEndPoint destination)
        {
            src = source;
            dst = destination;
        }

        public async Task Run(Action? onFirstPacket = null)
        {
            using var csrc = new TcpClient();
            using var cdst = new TcpClient();
            csrc.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            cdst.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            await csrc.ConnectAsync(src, canceller.Token);
            await cdst.ConnectAsync(dst, canceller.Token);
            await RunForwarding(csrc, cdst, onFirstPacket);
        }
    }
}
