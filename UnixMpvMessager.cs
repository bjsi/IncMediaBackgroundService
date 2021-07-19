using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IncMediaBackgroundService
{
    public class UnixMpvMessager : IMpvMessenger
    {
        private Process MpvProc { get; }
        private string Socket { get; }

        public UnixMpvMessager(Process mpvProc, string socket)
        {
            MpvProc = mpvProc;
            Socket = socket;
        }

        public Result Send(string message)
        {
            if (MpvProc == null || MpvProc.HasExited)
            {
                return new Result(false, null, "No mpv process running.");
            }

            var tmp = "/tmp/send_to_mpv.json";
            File.WriteAllText(tmp, message + "\n");

            var (status, stdout, _) = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/sh",
                    Arguments = $"-c \"cat {tmp} | socat - {Socket}\"",
                    UseShellExecute = false,
                }
            }.ExecuteBlockingWithOutputs();

            if (status != 0)
            {
                return new Result(false, null, "Unsuccessful call to socat: " + stdout);
            }

            return new Result(true, stdout, "success");
        }
    }
}
