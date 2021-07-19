using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IncMediaBackgroundService
{
    public static class mpv
    {
        private static string mpvPath = "mpv";

        public static Process Start(string queue, string ipcSocket)
        {
            var args = new string[]
            {
                "--idle=once",
                $"--input-ipc-server={ipcSocket}",
                $"--script-opts=im-start=yes,im-queue={queue}",
            };
            var p = ProcessEx.CreateBackgroundProcess(mpvPath, string.Join(" ", args));
            p.Start();
            return p;
        }
    }
}
