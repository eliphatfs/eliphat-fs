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
        public async Task RunForwarding(TcpClient src, TcpClient dst)
        {
            try
            {
                src.ReceiveBufferSize = 128 * 1024;
                src.SendBufferSize = 128 * 1024;
                dst.ReceiveBufferSize = 128 * 1024;
                dst.SendBufferSize = 128 * 1024;
                await Task.WhenAll(
                    RunPassThrough(src, dst),
                    RunPassThrough(dst, src)
                );
            }
            finally
            {
                Console.WriteLine($"Ending connector: {src.Client.RemoteEndPoint} <-> {dst.Client.RemoteEndPoint}");
                src.Close();
                dst.Close();
            }
        }
        public async Task RunPassThrough(TcpClient readEnd, TcpClient writeEnd)
        {
            await using var sr = readEnd.GetStream();
            await using var sw = writeEnd.GetStream();
            byte[] buffer = new byte[65536];  // max tcp window size
            byte[] back = new byte[65536];  // double buffer
            ValueTask lastTask = ValueTask.CompletedTask;
            while (!canceller.IsCancellationRequested)
            {
                if (!readEnd.Connected) break;
                var count = await sr.ReadAsync(buffer, canceller.Token);
                if (!writeEnd.Connected) break;
                await lastTask;
                if (!writeEnd.Connected) break;
                lastTask = sw.WriteAsync(buffer.AsMemory(0, count), canceller.Token);
                (buffer, back) = (back, buffer);
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
