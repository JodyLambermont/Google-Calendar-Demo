using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Xml;

namespace CalendarQuickstart
{
    class Sender
    {
        public static void Main(string[] args) {

            var factory = new ConnectionFactory() { HostName = "10.3.56.27", UserName = "manager", Password = "ehb", Port = 5672 };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "logs", type: "fanout");
                string doc = "createevent";
                var body = Encoding.UTF8.GetBytes(doc);
                Console.WriteLine(body);
                channel.BasicPublish(exchange: "logs",
                    routingKey: "Planning",
                    basicProperties: null,
                    body: body);
                Console.WriteLine("succes m8");
                Console.ReadKey(true);
            }
        }
    }
}