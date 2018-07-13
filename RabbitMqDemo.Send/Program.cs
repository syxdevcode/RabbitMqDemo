using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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
            Test5();
            Console.Read();
        }

        /// <summary>
        /// 链接，工作队列，发送/接收消息，轮训分发，消息确认，消息持久化，公平分发
        /// </summary>
        static void Test1()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
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

            // 创建connection 
            using (var connection = factory.CreateConnection(new string[1] { "192.168.0.115" }))
            {
                // 创建一个channel信道
                using (var channel = connection.CreateModel())
                {
                    //将消息标记为持久性 - 将IBasicProperties.SetPersistent设置为true
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    // 申明队列 durable:是否持久化
                    var result = channel.QueueDeclare(queue: "test1", durable: true, exclusive: false, autoDelete: false, arguments: null);

                    for (int i = 0; i < 21; i++)
                    {
                        //构建byte消息数据包
                        var body = Encoding.UTF8.GetBytes(i.ToString() + "test");
                        channel.BasicPublish(exchange: "", routingKey: "test1", basicProperties: null, body: body);
                        Console.WriteLine("{0} 推送成功", i);
                        Thread.Sleep(2000);
                    }
                }
            }
        }

        /// <summary>
        /// direct
        /// </summary>
        static void Test2()
        {
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

            // 创建connection 
            using (var connection = factory.CreateConnection(new string[1] { "192.168.0.115" }))
            {
                // 创建一个channel信道
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "directEC", type: "direct");

                    //将消息标记为持久性 - 将IBasicProperties.SetPersistent设置为true
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    for (int i = 0; i < 21; i++)
                    {
                        //构建byte消息数据包
                        var body = Encoding.UTF8.GetBytes(i.ToString() + "test");
                        channel.BasicPublish(exchange: "directEC", routingKey: "direct-key", basicProperties: null, body: body);
                        Console.WriteLine("{0} 推送成功", i);
                        //Thread.Sleep(2000);
                    }
                }
            }
        }

        static void Test3()
        {
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

            // 创建connection 
            using (var connection = factory.CreateConnection(new string[1] { "192.168.0.115" }))
            {
                // 创建一个channel信道
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "fanoutEC", type: "fanout");

                    //将消息标记为持久性 - 将IBasicProperties.SetPersistent设置为true
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    for (int i = 0; i < 21; i++)
                    {
                        //构建byte消息数据包
                        var body = Encoding.UTF8.GetBytes(i.ToString() + "test");

                        //发布到指定exchange，fanout类型无需指定routingKey
                        channel.BasicPublish(exchange: "fanoutEC", routingKey: "", basicProperties: properties, body: body);
                        Console.WriteLine("{0} 推送成功", i);
                        //Thread.Sleep(2000);
                    }
                }
            }
        }

        static void Test4()
        {
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

            // 创建connection 
            using (var connection = factory.CreateConnection(new string[1] { "192.168.0.115" }))
            {
                // 创建一个channel信道
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "topicEC", type: "topic");

                    //将消息标记为持久性 - 将IBasicProperties.SetPersistent设置为true
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    for (int i = 0; i < 21; i++)
                    {
                        //构建byte消息数据包
                        var body = Encoding.UTF8.GetBytes(i.ToString() + "test");

                        ///发布到topic类型exchange，必须指定routingKey
                        channel.BasicPublish(exchange: "topicEC", routingKey: "first.green.fast", basicProperties: properties, body: body);
                        Console.WriteLine("{0} 推送成功", i);
                        //Thread.Sleep(2000);
                    }
                }
            }
        }

        static void Test5()
        {
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

            // 创建connection 
            using (var connection = factory.CreateConnection(new string[1] { "192.168.0.115" }))
            {
                 
            }
        }
    }
}


