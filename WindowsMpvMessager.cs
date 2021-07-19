using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;

namespace IncMediaBackgroundService
{
    public class WindowsMpvMessager : IMpvMessenger
    {
        private string PipePath { get; }
        private Process MpvProc { get; }

        public WindowsMpvMessager(Process mpvProc, string pipePath)
        {
            PipePath = pipePath;
            MpvProc = mpvProc;
        }

        private string GetPipeName(string pipePath)
        {
            return pipePath.Replace(@"\\.\pipe\", "");
        }

        public Result Send(string json)
        {
            if (MpvProc == null || MpvProc.HasExited)
            {
                return new Result(false, "Failed to send a pipe request because there is no mpv process", null);
            }

            string data = null;

            try
            {
                var pipeName = GetPipeName(PipePath);
                using (var client = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.None, System.Security.Principal.TokenImpersonationLevel.Impersonation))
                using (var pReader = new StreamReader(client))
                using (var pWriter = new StreamWriter(client))
                {
                    client.Connect();
                    pWriter.AutoFlush = true;
                    pWriter.WriteLine(json);
                    client.WaitForPipeDrain();
                    if (pReader.Peek() > 0)
                    {
                        data = pReader.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {
                return new Result(false, data, $"Exception {e} while communicating with mpv through the pipe.");
            }

            return new Result(true, data, "success");
        }
    }
}
