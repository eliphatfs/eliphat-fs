using System.CommandLine;
using EliphatFS.Commands;

namespace EliphatFS
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var rootCommand = new RootCommand();

            var hostCommand = new Command("host");
            hostCommand.SetHandler(Host.Run);
            rootCommand.Add(hostCommand);

            var connectCommand = new Command("connect");
            var ip = new Argument<string>("ip", "IP address of host");
            connectCommand.Add(ip);
            connectCommand.SetHandler(Connect.Run, ip);
            rootCommand.Add(connectCommand);

            var mountCommand = new Command("mount", "Mounts FS from host. Only works on linux.");
            var share = new Argument<string>("share", "FS share name, hostip:/path");
            var fspath = new Argument<string>("path", "Path to mount");
            mountCommand.Add(share);
            mountCommand.Add(fspath);
            mountCommand.SetHandler(Mount.Run, share, fspath);
            rootCommand.Add(mountCommand);

            var shareCommand = new Command("share", "Create local FS share. Only works on linux.");
            var path = new Argument<string>("path", "Share source path.");
            shareCommand.Add(path);
            shareCommand.SetHandler(Share.Run, path);
            rootCommand.Add(shareCommand);

            await rootCommand.InvokeAsync(args);
        }
    }
}
