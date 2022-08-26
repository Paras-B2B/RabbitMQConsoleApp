using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Consumer;
using RabbitMQ.Consumer.Api;

namespace ReadRabbitMQConsole
{
    public class ReadQueue
    {
        private Action<string> OnLoginSuccess;
        private Action<string> OnRewardSuccess;
        public void StartRabbitMQConnection(string? uri, string? queueName1, string? queueName2)
        {
            if(string.IsNullOrEmpty(uri)) return;
            var factory = new ConnectionFactory
            {
                Uri = new Uri(uri)
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queueName1,
                true,
                false,
                false,
                null);
            channel.QueueDeclare(queueName2,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                ReadQueue rd = new();
                var messageType = e.BasicProperties.MessageId;
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                switch (messageType)
                {
                    case "Login":
                        rd.OnLoginSuccess = (msg) =>
                        {
                            var m = new {Name = "For Atlas", Message = msg};
                            var b = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(m));
                            channel.BasicPublish("", queueName2, null, b);
                        };
                        rd.OnLoginAction(message);
                        break;
                    case "Rewards":
                        rd.OnRewardSuccess = (msg) =>
                        {
                            var m = new {Name = "For Atlas", Message = msg};
                            var b = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(m));
                            channel.BasicPublish("", queueName2, null, b);
                        };
                        rd.OnRewardAction(message);
                        break;
                }
            };

            channel.BasicConsume(queueName1, true, consumer);
            Console.ReadLine();
        }

        void OnLoginAction(string message)
        {
            try
            {
                LoginJson json = JsonConvert.DeserializeObject<LoginJson>(message);
                Console.WriteLine("Login successful.");
                Console.WriteLine($"Name is {json.name}");
                Console.WriteLine($"Age is {json.age}");
                IApiController weatherApiController = new WeatherApiController();
                weatherApiController.GetData();
            }
            catch (Exception e)
            {
                OnLoginSuccess?.Invoke(e.Message);
                OnRewardSuccess  = (str) => { };
            }
            Console.WriteLine("successful .");
            OnLoginSuccess?.Invoke("Login successful");
        }

        void OnRewardAction(string message)
        {
            try
            {
                RewardsJson json = JsonConvert.DeserializeObject<RewardsJson>(message);
                Console.WriteLine("Get Rewards successful.");
                Console.WriteLine($"Coins is {json.coins}");
                Console.WriteLine($"Levels is {json.levels}");
            }
            catch (Exception e)
            {
                OnRewardSuccess?.Invoke(e.Message);
                OnRewardSuccess  = (str) => { };
            }
            OnRewardSuccess?.Invoke("Reward successful");
        }
    }
}