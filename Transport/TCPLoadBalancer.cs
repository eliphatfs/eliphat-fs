using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EliphatFS.Transport
{
    /// <summary>
    /// A TCP forwarder that load-balances connections into client connections.
    /// Acts as TCP server on both sides (passive).
    /// </summary>
    public class TCPLoadBalancer : TCPForwarderBase
    {
        public TcpListener inListener;
        public TcpListener outListener;
        public ConcurrentBag<TcpClient> balanced = new();
        public TCPLoadBalancer(int inport, int outport)
        {
            inListener = new(IPAddress.Any, inport);
            outListener = new(IPAddress.Any, outport);
            inListener.Start();
            outListener.Start();
        }

        public async Task RunOutbound()
        {
            while (!canceller.IsCancellationRequested)
            {
                var client = await outListener.AcceptTcpClientAsync(canceller.Token);
                Console.WriteLine($"Got service connector: {client.Client.RemoteEndPoint}");
                balanced.Add(client);
            }
        }
        public async Task RunInbound()
        {
            while (!canceller.IsCancellationRequested)
            {
                var client = await inListener.AcceptTcpClientAsync(canceller.Token);
                Console.WriteLine($"Got client connector: {client.Client.RemoteEndPoint}");
                while (!canceller.IsCancellationRequested)
                {
                    if (balanced.TryTake(out var balancedLoad))
                    {
                        Console.WriteLine($"Dispatching connector: {client.Client.RemoteEndPoint} <-> {balancedLoad.Client.RemoteEndPoint}");
                        DispatchForwarding(client, balancedLoad);
                        break;
                    }
                    await Task.Delay(100);
                }
            }
        }

        protected override void InternalDispose()
        {
            base.InternalDispose();
            inListener.Stop();
            outListener.Stop();
        }
    }
}
