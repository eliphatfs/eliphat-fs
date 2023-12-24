using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliphatFS.Commands
{
    public class Mount
    {
        public static void Run(string share, string mountpoint)
        {
            Process.Start("mount", new[] { "-t", "nfs", "-o", "mountport=2049,port=2049,nfsvers=3,proto=tcp", share, mountpoint }).WaitForExit();
        }
    }
}
