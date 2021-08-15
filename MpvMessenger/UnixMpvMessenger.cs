using IncMediaBackgroundService.Helpers;
using System;
using System.Diagnostics;
using System.IO;

namespace IncMediaBackgroundService.MpvMessenger
{
    public class UnixMpvMessenger : MpvMessengerBase
    {

        private string SystemTempFile { get; } = @"/tmp/send_to_mpv.json";
        protected bool WriteToTempFile(string data)
        {
            try
            {
                File.WriteAllText(SystemTempFile, data + Environment.NewLine);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        protected override string WriteToMpvIPC(int pid, string message)
        {
            if (!WriteToTempFile(message))
            {
                Console.WriteLine("Failed to write json data to temp file");
                return null;
            }

            var (status, stdout, _) = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/sh",
                    Arguments = $"-c \"cat {SystemTempFile} | socat - /tmp/mpv-socket-{pid}\"",
                    UseShellExecute = false,
                }
            }.ExecuteBlockingWithOutputs();

            if (status != 0)
            {
                Console.WriteLine("Attempt to send json data to mpv socket exited with status", status);
                return null;
            }

            return stdout;
        }
    }
}
