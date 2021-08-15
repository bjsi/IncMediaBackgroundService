using AustinHarris.JsonRpc;
using IncMediaBackgroundService.Helpers;
using IncMediaBackgroundService.Interop;
using IncMediaBackgroundService.MpvMessenger;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace IncMediaBackgroundService
{
    public class BackgroundJsonRpcService : JsonRpcService, IMpvService
    {
        private MpvMessengerBase MpvMessenger { get; set; }

        public BackgroundJsonRpcService()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                MpvMessenger = new UnixMpvMessenger();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                MpvMessenger = new WindowsMpvMessenger();
            else
                throw new PlatformNotSupportedException("Unrecognised OS.");
        }

        [JsonRpcMethod]
        public List<string> GetExistingQueues()
        {
            return MpvMessenger.GetExistingQueues();
        }

        [JsonRpcMethod]
        public MpvResponse ExportToSM(string queue, int unixTime)
        {
            return SendRequest(queue, $"script-message export_to_sm {unixTime}");
        }

        [JsonRpcMethod]
        public MpvResponse SendRequest(string queue, string args)
        {
            var json = new MpvRequest(args.Split(" ")).Serialize();
            return MpvMessenger.Send(queue, json)?.Deserialize<MpvResponse>();
        }

        [JsonRpcMethod]
        public List<string> GetRunning()
        {
            return MpvMessenger.GetRunningQueues();
        }

        [JsonRpcMethod]
        public bool OpenMPV(string queue, string smaServerHost, int smaServerPort, string additionalArgs)
        {
            if (string.IsNullOrEmpty(queue))
                return false;

            var proc = mpv.Start(queue, smaServerHost, smaServerPort, additionalArgs);
            return (proc != null && !proc.HasExited);
        }
    }
}
