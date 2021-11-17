using Microsoft.Extensions.Configuration;
using PlatformService.DTOs;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient: IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        public IModel _channel { get; }

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                Console.WriteLine("--> Connected to MessageBus");

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ Connection Shutdown");
        }

        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message = JsonSerializer.Serialize(platformPublishedDto);
            if (_connection.IsOpen)
            {
                Console.WriteLine("--> Publishing message to RabbitMq");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> No message sent as connection is closed");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(
                body: body,
                basicProperties: null,
                exchange: "trigger",
                routingKey: "");
        }
    }
}
