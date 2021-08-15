using IncMediaBackgroundService.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace IncMediaBackgroundService.MpvMessenger
{
    public static class mpv
    {
        private static string mpvPath = "mpv";

        public static string IncMediaScriptFolder =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "mpv", "scripts", "incremental-media-mpv");

        public static string IncMediaScriptDataFolder =
            Path.Combine(IncMediaScriptFolder, "data");

        public static Process Start(string queue, string smaServerHost, int smaServerPort, string additionalArgs)
        {
            var scriptOpts = new List<string>
            {
                "im-start=yes",
                $"im-queue={queue}",
                $"im-sm_server_host={smaServerHost}",
                $"im-sma_server_port={smaServerPort}",
            };

            if (!string.IsNullOrEmpty(additionalArgs))
                scriptOpts.Add(additionalArgs);

            var args = new string[]
            {
                "--idle=once",
                $"--script-opts={string.Join(",", scriptOpts)}",
            };

            Console.WriteLine("Starting MPV with args: " + string.Join(" ", args));
            var p = ProcessEx.CreateBackgroundProcess(mpvPath, string.Join(" ", args));
            p.Start();
            return p;
        }
    }
}
