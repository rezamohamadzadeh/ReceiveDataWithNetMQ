using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using NetMQ.Sockets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetMQ.Web
{
    public class NetMqBackgroundService : BackgroundService
    {
        private readonly IHubContext<NotifHub> _hubContext;

        public NetMqBackgroundService(IHubContext<NotifHub> hubContext)
        {
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var server = new ResponseSocket("tcp://*:8050");


            while (!stoppingToken.IsCancellationRequested)
            {
                string serverMessage = "";
                await Task.Run(() =>
                {
                    try
                    {
                        serverMessage = server.ReceiveFrameString();
                        Console.WriteLine($"Client Sent: {serverMessage} DateTime: {DateTime.Now}");
                        Console.WriteLine("Sending reply to the client ...");

                        const string msg1 = "Hi", msg2 = "How are you ?";

                        var response = new NetMQMessage();
                        response.Append(msg1);
                        response.Append(msg2);

                        server.SendMultipartMessage(response);
                        Console.WriteLine($"Response has been sent => 1: {msg1}, 2: {msg2}");

                        _hubContext.Clients.All.SendAsync("ReceiveNotification",serverMessage);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message} DateTime: {DateTime.Now}");
                    }


                });
            }
        }
    }
}
