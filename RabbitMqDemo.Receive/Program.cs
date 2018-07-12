using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Net.Security;
using System.Text;
using System.Threading;

namespace RabbitMqDemo.Receive
{
    class Program
    {
        static void Main(string[] args)
        {
            //1.实例化连接工厂
            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = "admin",
                Password = "admin",
                AutomaticRecoveryEnabled = true,
                Ssl = new SslOption()
                {
                    CertPath = @"E:\git\RabbitMqDemo\RabbitMqDemo.Receive\server.pfx",
                    CertPassphrase = "123123",
                    AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch |
                                                SslPolicyErrors.RemoteCertificateChainErrors,
                    Enabled = true
                },
                //AuthMechanisms = new AuthMechanismFactory[] { new ExternalMechanismFactory() },
                RequestedHeartbeat = 60,
                Port = 5673,
                TopologyRecoveryEnabled = true
            };

            //2. 建立连接
            using (var connection = factory.CreateConnection(new string[1] { "192.168.0.115" }))
            {
                //3. 创建信道
                using (var channel = connection.CreateModel())
                {
                    //4. 申明队列
                    channel.QueueDeclare(queue: "test1", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    //5. 构造消费者实例
                    var consumer = new EventingBasicConsumer(channel);
                    //6. 绑定消息接收后的事件委托
                    consumer.Received += (model, ea) =>
                    {
                        var message = Encoding.UTF8.GetString(ea.Body);
                        Console.WriteLine(" [x] Received {0}", message);
                        Thread.Sleep(500);//模拟耗时
                        Console.WriteLine(" [x] Done");
                    };
                    //7. 启动消费者
                    channel.BasicConsume(queue: "test1", autoAck: true, consumer: consumer);
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
