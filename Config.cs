using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IncMediaBackgroundService
{
    public static class Config
    {
        public static string MpvPath { get; set; } = "mpv";
        public static string Host { get; set; } = "127.0.0.1";
        public static int Port { get; set; } = 9890;
    }
}
