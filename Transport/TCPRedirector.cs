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
    /// 1-to-1 forward of TCP local port to destination.
    /// Acts as server on local port and client to destination.
    /// </summary>
    public class TCPRedirector : TCPForwarderBase
    {
        public TcpListener listener;
        public IPEndPoint destination;

        public TCPRedirector(int port, IPEndPoint target)
        {
            destination = target;
            listener = new(IPAddress.Any, port);
            listener.Start();
        }

        public async Task Run()
        {
            while (!canceller.IsCancellationRequested)
            {
                var inbound = await listener.AcceptTcpClientAsync(canceller.Token);
                var outbound = new TcpClient();
                outbound.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                await outbound.ConnectAsync(destination, canceller.Token);
                DispatchForwarding(inbound, outbound);
            }
        }

        protected override void InternalDispose()
        {
            base.InternalDispose();
            listener.Stop();
        }
    }
}
