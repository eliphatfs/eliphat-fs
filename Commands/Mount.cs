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
            Process.Start("mount", new[] { "-t", "nfs", share, mountpoint }).WaitForExit();
        }
    }
}
