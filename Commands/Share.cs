using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliphatFS.Commands
{
    public class Share
    {
        public static void Run(string path)
        {
            File.WriteAllText("exports", $"{path} 127.0.0.1(rw,insecure,all_squash,anonuid=0,anongid=0)");
            Process.Start("unfsd", new[] { "-t", "-p", "-e", Path.GetFullPath("exports") }).WaitForExit();
        }
    }
}
