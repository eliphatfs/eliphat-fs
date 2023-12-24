using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EliphatFS.Transport;

namespace EliphatFS.Commands
{
    public class Host
    {
        public static async Task Run()
        {
            using var balancer = new TCPLoadBalancer(2049, 9000);
            await Task.WhenAll(balancer.RunInbound(), balancer.RunOutbound());
        }
    }
}
