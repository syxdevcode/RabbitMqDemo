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
            Test4();
            Console.ReadLine();
        }

        static void Test1()
        {
            // 实例化连接工厂
            // 加证书
            //ConnectionFactory factory = new ConnectionFactory()
            //{
            //    UserName = "admin",
            //    Password = "admin",
            //    Ssl = new SslOption()
            //    {
            //        CertPath = @"E:\git\RabbitMqDemo\server.pfx",
            //        CertPassphrase = "123123",

            //        AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch |
            //                                    SslPolicyErrors.RemoteCertificateChainErrors,
            //        Enabled = true
            //    },
            //    //AuthMechanisms = new AuthMechanismFactory[] { new ExternalMechanismFactory() },
            //    RequestedHeartbeat = 60,
            //    Port = 5673
            //};

            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = "admin",
                Password = "admin",
                AutomaticRecoveryEnabled = true,
                Port = 5672,
                TopologyRecoveryEnabled = true
            };

            //  建立连接
            using (var connection = factory.CreateConnection(new string[1] { "192.168.0.115" }))
            {
                //  创建信道
                using (var channel = connection.CreateModel())
                {
                    //申明direct类型exchange
                    channel.ExchangeDeclare(exchange: "directEC", type: "direct");

                    //绑定队列到direct类型exchange，需指定路由键routingKey
                    channel.QueueBind(queue: "green-queue", exchange: "directEC", routingKey: "green");

                    // 申明队列
                    channel.QueueDeclare(queue: "green-queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

                    // 设置prefetchCount : 1来告知RabbitMQ，在未收到消费端的消息确认时，不再分发消息，也就确保了当消费端处于忙碌状态时
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    // 构造消费者实例
                    var consumer = new EventingBasicConsumer(channel);
                    // 绑定消息接收后的事件委托
                    consumer.Received += (model, ea) =>
                    {
                        var message = Encoding.UTF8.GetString(ea.Body);
                        Console.WriteLine(" [x] Received {0}", message);
                        // TODO 注：耗时过长（测试使用5000）,，使用证书模式手动确认会报错，原因不明
                        Thread.Sleep(2000);//模拟耗时 
                        Console.WriteLine(" [x] Done {0}", message);

                        // 发送消息确认信号（手动消息确认）
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };
                    //  启动消费者
                    /*
                     autoAck:true；自动进行消息确认
                     autoAck:false；关闭自动消息确认，通过调用BasicAck方法手动进行消息确认
                     */
                    //channel.BasicConsume(queue: "test1", autoAck: true, consumer: consumer);
                    channel.BasicConsume(queue: "green-queue", autoAck: false, consumer: consumer);
                    Console.WriteLine(" Press [enter] to exit.");
                }
            }
        }

        static void Test2()
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = "admin",
                Password = "admin",
                AutomaticRecoveryEnabled = true,
                Port = 5672,
                TopologyRecoveryEnabled = true
            };

            //  建立连接
            using (var connection = factory.CreateConnection(new string[1] { "192.168.0.115" }))
            {
                //  创建信道
                using (var channel = connection.CreateModel())
                {
                    //申明direct类型exchange
                    channel.ExchangeDeclare(exchange: "directEC", type: "direct");

                    var queueName = channel.QueueDeclare().QueueName;
                    channel.QueueBind(queue: queueName, exchange: "directEC", routingKey: "direct-key");

                    var consumer = new EventingBasicConsumer(channel);

                    //  绑定消息接收后的事件委托
                    consumer.Received += (model, ea) =>
                    {
                        // var body = ea.Body;涉及到闭包，必须赋予变量
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);
                        Console.WriteLine(" [x] Done1 {0}", message);
                    };

                    channel.BasicConsume(queue: queueName,
                                         autoAck: true,
                                         consumer: consumer);
                    //channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
                    Console.WriteLine(" Press [enter] to exit.");

                    Console.ReadLine(); // 必须存在
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
                Port = 5672,
                TopologyRecoveryEnabled = true
            };

            //  建立连接
            using (var connection = factory.CreateConnection(new string[1] { "192.168.0.115" }))
            {
                //  创建信道
                using (var channel = connection.CreateModel())
                {
                    //申明direct类型exchange
                    channel.ExchangeDeclare(exchange: "fanoutEC", type: "fanout");

                    var queueName = channel.QueueDeclare().QueueName;

                    // 绑定队列到指定fanout类型exchange，无需指定路由键
                    channel.QueueBind(queue: queueName, exchange: "fanoutEC", routingKey: "");

                    var consumer = new EventingBasicConsumer(channel);

                    //  绑定消息接收后的事件委托
                    consumer.Received += (model, ea) =>
                    {
                        // var body = ea.Body;涉及到闭包，必须赋予变量
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);
                        Console.WriteLine(" [x] Done1 {0}", message);
                    };

                    channel.BasicConsume(queue: queueName,
                                         autoAck: true,
                                         consumer: consumer);
                    //channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
                    Console.WriteLine(" Press [enter] to exit.");

                    Console.ReadLine(); // 必须存在
                }
            }
        }

        static void Test4()
        {
            // 实例化连接工厂
            // 加证书
            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = "admin",
                Password = "admin",
                Ssl = new SslOption()
                {
                    CertPath = @"E:\git\RabbitMqDemo\client.pfx",
                    CertPassphrase = "123456",

                    AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch |
                                                SslPolicyErrors.RemoteCertificateChainErrors,
                    Enabled = true
                },
                //AuthMechanisms = new AuthMechanismFactory[] { new ExternalMechanismFactory() },
                RequestedHeartbeat = 60,
                Port = 5673
            };

            //  建立连接
            using (var connection = factory.CreateConnection(new string[1] { "192.168.0.115" }))
            {
                //  创建信道
                using (var channel = connection.CreateModel())
                {
                    //申明direct类型exchange
                    channel.ExchangeDeclare(exchange: "topicEC", type: "topic");

                    var queueName = channel.QueueDeclare().QueueName;

                    //绑定队列到topic类型exchange，需指定路由键routingKey
                    channel.QueueBind(queue: queueName, exchange: "topicEC", routingKey: "#.*.fast");

                    var consumer = new EventingBasicConsumer(channel);

                    //  绑定消息接收后的事件委托
                    consumer.Received += (model, ea) =>
                    {
                        // var body = ea.Body;涉及到闭包，必须赋予变量
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);
                        Console.WriteLine(" [x] Done1 {0}", message);
                    };

                    channel.BasicConsume(queue: queueName,
                                         autoAck: true,
                                         consumer: consumer);
                    //channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
                    Console.WriteLine(" Press [enter] to exit.");

                    Console.ReadLine(); // 必须存在
                }
            }
        } 
    }
}
