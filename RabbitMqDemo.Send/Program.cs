using RabbitMQ.Client;
using System;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading;

namespace RabbitMqDemo.Send
{
    class Program
    {
        static void Main(string[] args)
        {
            //ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = "admin",
                Password = "admin",
                AutomaticRecoveryEnabled = true,
                Ssl = new SslOption()
                {
                    CertPath = @"E:\git\RabbitMqDemo\RabbitMqDemo.Send\server.pfx",
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

            //第一步：创建connection 
            using (var connection = factory.CreateConnection(new string[1] { "192.168.0.115" }))
            {
                //第二步：创建一个channel信道
                using (var channel = connection.CreateModel())
                {
                    //第三步：申明队列 durable:是否持久化
                    var result = channel.QueueDeclare(queue: "test1", durable: true, exclusive: false, autoDelete: false, arguments: null);

                    for (int i = 0; i < int.MaxValue; i++)
                    {
                        //构建byte消息数据包
                        var body = Encoding.UTF8.GetBytes(i.ToString() + "test");
                        channel.BasicPublish(exchange: "", routingKey: "test1", basicProperties: null, body: body);
                        Console.WriteLine("{0} 推送成功", i);
                        Thread.Sleep(500);
                    }
                }
            }
            Console.Read();
        }
    }
}
