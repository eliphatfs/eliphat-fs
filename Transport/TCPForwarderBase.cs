using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EliphatFS.Transport
{
    public abstract class TCPForwarderBase : IDisposable
    {
        private bool disposedValue;
        public CancellationTokenSource canceller = new();
        protected virtual void InternalDispose()
        {
        }
        public async void DispatchForwarding(TcpClient src, TcpClient dst)
        {
            await RunForwarding(src, dst);
        }
        public async Task RunForwarding(TcpClient src, TcpClient dst, Action? onFirstPacket = null)
        {
            string connectorString = "";
            try
            {
                connectorString = $"{src.Client.RemoteEndPoint} <-> {dst.Client.RemoteEndPoint}";
                await Task.WhenAll(
                    RunPassThrough(src, dst, () => { onFirstPacket?.Invoke(); onFirstPacket = null; }),
                    RunPassThrough(dst, src, () => { onFirstPacket?.Invoke(); onFirstPacket = null; })
                );
            }
            finally
            {
                Console.WriteLine($"Ending connector: {connectorString}");
                src.Close();
                dst.Close();
            }
        }
        public async Task RunPassThrough(TcpClient readEnd, TcpClient writeEnd, Action? onFirstPacket = null)
        {
            try
            {
                await using var sr = readEnd.GetStream();
                await using var sw = writeEnd.GetStream();
                byte[] buffer = new byte[readEnd.ReceiveBufferSize];  // max tcp window size
                byte[] back = new byte[readEnd.ReceiveBufferSize];  // double buffer
                ValueTask lastTask = ValueTask.CompletedTask;
                while (!canceller.IsCancellationRequested)
                {
                    var count = await sr.ReadAsync(buffer, canceller.Token);
                    onFirstPacket?.Invoke();
                    onFirstPacket = null;
                    if (count == 0) break;
                    await lastTask;
                    lastTask = sw.WriteAsync(buffer.AsMemory(0, count), canceller.Token);
                    (buffer, back) = (back, buffer);
                }
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine(ex);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    canceller.Cancel();
                    InternalDispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~TCPForwarder()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
