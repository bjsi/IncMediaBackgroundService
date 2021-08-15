using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;

namespace IncMediaBackgroundService.MpvMessenger
{
    public class WindowsMpvMessenger : MpvMessengerBase
    {
        protected override string WriteToMpvIPC(int pid, string message)
        {
            string data = null;
            try
            {
                using (var client = new NamedPipeClientStream(".", $@"\\pipe\.\mpv-pipe-{pid}", PipeDirection.InOut, PipeOptions.None, System.Security.Principal.TokenImpersonationLevel.Impersonation))
                using (var pReader = new StreamReader(client))
                using (var pWriter = new StreamWriter(client))
                {
                    client.Connect();
                    pWriter.AutoFlush = true;
                    pWriter.WriteLine(message);
                    client.WaitForPipeDrain();
                    if (pReader.Peek() > 0)
                    {
                        data = pReader.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }

            return data;
        }
    }
}
