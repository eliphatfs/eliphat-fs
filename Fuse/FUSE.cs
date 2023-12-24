using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EliphatFS.Fuse
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FuseOperations
    {
        public IntPtr getattr;
        public IntPtr readlink;
        public IntPtr mknod;
        public IntPtr mkdir;
        public IntPtr unlink;
        public IntPtr rmdir;
        public IntPtr symlink;
        public IntPtr rename;
        public IntPtr link;
        public IntPtr chmod;
        public IntPtr chown;
        public IntPtr truncate;
        public IntPtr open;
        public IntPtr read;
        public IntPtr write;
        public IntPtr statfs;
        public IntPtr flush;
        public IntPtr release;
        public IntPtr fsync;
        public IntPtr setxattr;
        public IntPtr getxattr;
        public IntPtr listxattr;
        public IntPtr removexattr;
        public IntPtr opendir;
        public IntPtr readdir;
        public IntPtr releasedir;
        public IntPtr fsyncdir;
        public IntPtr init;
        public IntPtr destroy;
        public IntPtr access;
        public IntPtr create;
        public IntPtr @lock;
        public IntPtr utimens;
        public IntPtr bmap;
        public IntPtr ioctl;
        public IntPtr poll;
        public IntPtr write_buf;
        public IntPtr read_buf;
        public IntPtr flock;
        public IntPtr fallocate;
        public IntPtr copy_file_range;
        public IntPtr lseek;
    }
    /// <summary>
    /// Unused by now. For future FUSE mounting instead of going through NFS protocols.
    /// </summary>
    public class FUSE
    {
    }
}
