using AustinHarris.JsonRpc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IncMediaBackgroundService
{
    public class JsonRpcServer
    {
        public bool HasExited { get; set; }
        private TcpListener Server { get; set; }
        private API Service { get; set; } = new API();
        private const int Port = 9890;

        public async Task StartAsync()
        {
            Server = new TcpListener(IPAddress.Parse("127.0.0.1"), Port);
            Server.Start();
            while (!HasExited)
            {
                try
                {
                    using (var client = await Server.AcceptTcpClientAsync())
                    using (var stream = client.GetStream())
                    {
                        Console.WriteLine("Client connected.");

                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
                        {

                            var line = await reader.ReadLineAsync();
                            Console.WriteLine("Received request from client: " + line);

                            var response = await JsonRpcProcessor.Process(line);
                            Console.WriteLine("Sent response to client: " + response);

                            await writer.WriteLineAsync(response);
                            await writer.FlushAsync();

                            client.Close();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Caught exception: " + e.Message);
                }
            }

            Console.WriteLine("Stopping JsonRpcServer");
            Server.Stop();
        }
    }
}
