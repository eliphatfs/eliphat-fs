using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EliphatFS.Transport;

namespace EliphatFS.Commands
{
    public class Connect
    {
        public static async Task Run(string ip)
        {
            using var forwarder = new TCPPipe(
                new IPEndPoint(IPAddress.Loopback, 2049),
                new IPEndPoint(IPAddress.Parse(ip), 9000)
            );
            int backoff = 1;
            while (true)
                try
                {
                    await forwarder.Run(async () => { Console.WriteLine("First packet received. Starting new connector."); await Run(ip); });
                    Console.WriteLine($"Ended connection, back off in {500 * backoff} ms");
                    backoff = 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine($"Exception caught, back off in {500 * backoff} ms");
                    await Task.Delay(500 * backoff);
                    backoff *= 2;
                }
        }
    }
}
