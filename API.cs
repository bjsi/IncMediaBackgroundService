using AustinHarris.JsonRpc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IncMediaBackgroundService
{
    [Serializable]
    public class MpvRequest
    {
        public string[] command { get; }
        public MpvRequest(string[] args)
        {
            this.command = args;
        }
    }

    [Serializable]
    public class Result
    {
        public bool Success { get; set; }
        public string Data { get; set; }
        public string Message { get; set; }
        public Result(bool success, string data, string msg)
        {
            this.Success = success;
            this.Data = data;
            this.Message = msg;
        }
    }

    public class API : JsonRpcService
    {
        private IMpvMessenger MpvMessenger { get; set; }

        [JsonRpcMethod]
        public Result SendRequest(string args)
        {
            if (MpvMessenger == null)
                return new Result(false, null, "No mpv process started.");

            var json = new MpvRequest(args.Split(" ")).Serialize();
            return MpvMessenger.Send(json);
        }

        [JsonRpcMethod]
        public Result OpenMPV(string queue, string pipeOrSocketPath)
        {
            if (string.IsNullOrEmpty(queue))
                return new Result(false, null, "Invalid queue name.");

            var proc = mpv.Start(queue, pipeOrSocketPath);

            if (proc != null)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    MpvMessenger = new UnixMpvMessager(proc, pipeOrSocketPath);
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    MpvMessenger = new WindowsMpvMessager(proc, pipeOrSocketPath);

                return new Result(true, null, $"Successfully started mpv process");
            }
            else
            {
                return new Result(false, null, $"Failed to start mpv process.");
            }
        }
    }
}
