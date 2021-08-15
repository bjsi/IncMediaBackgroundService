using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace IncMediaBackgroundService.MpvMessenger
{
    public abstract class MpvMessengerBase
    {
        protected abstract string WriteToMpvIPC(int pid, string message);

        public string Send(string queue, string message)
        {
            if (!IncMediaScriptIsRunning(queue))
            {
                Console.WriteLine("incremental media mpv script is not running");
                return null;
            }

            int pid = GetPid(queue);
            if (pid == -1)
            {
                Console.WriteLine("Failed to get pid of incremental media mpv script");
                return null;
            }

            return WriteToMpvIPC(pid, message);
        }

        public List<string> GetExistingQueues()
        {
            var ret = new List<string>();
            try
            {
                ret = Directory.GetDirectories(mpv.IncMediaScriptDataFolder)
                    ?.Select(x => Path.GetFileName(x))
                    ?.ToList();
            }
            catch
            {
            }
            return ret;
        }

        public List<string> GetRunningQueues()
        {
            var existing = GetExistingQueues();
            var running = new List<string>();
            foreach (var q in existing)
            {
                if (IncMediaScriptIsRunning(q))
                    running.Add(q);
            }
            return running;
        }

        public bool IncMediaScriptIsRunning(string queue) 
        {
            try 
            { 
                var pid = GetPid(queue);
                if (pid != -1)
                {
                    var proc = Process.GetProcessById(pid);
                    return proc != null && !proc.HasExited;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private static string GetPidFile(string queue) => Path.Combine(mpv.IncMediaScriptDataFolder, queue, "pid_file");
        private static int GetPid(string queue)
        {
            var pidFile = GetPidFile(queue);
            int pid = -1;
            if (File.Exists(pidFile))
            {
                try
                {
                    var pidText = File.ReadAllText(pidFile);
                    int.TryParse(pidText, out pid);
                }
                catch (IOException)
                {
                    return -1;
                }
            }
            return pid;
        }
    }
}