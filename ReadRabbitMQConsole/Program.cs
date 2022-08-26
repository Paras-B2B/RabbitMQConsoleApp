using System;
using Microsoft.Extensions.Configuration;
using ReadRabbitMQConsole;

namespace ReadRabbitMQConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").Build();

            var uri = builder["URI"];
            var queueName1 = builder.GetSection("QueueName1").Value;
            var queueName2 = builder.GetSection("QueueName2").Value;
            Console.WriteLine("Console App Started.");

            ReadQueue rb = new ReadQueue();
            rb.StartRabbitMQConnection(uri, queueName1, queueName2);
        }
    }
}