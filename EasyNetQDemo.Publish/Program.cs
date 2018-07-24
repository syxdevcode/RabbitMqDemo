using EasyNetQ;
using EasyNetQ.Logging;
using EasyNetQDemo.Message;
using System;
using System.Collections.Generic;
using System.Net.Security;

namespace EasyNetQDemo.Publish
{
    class Program
    {
        static void Main(string[] args)
        {
            TestRPC();
            Console.ReadKey();
        }

        static void TestRPC()
        {
            var rpcClient = new RpcClient();

            Console.WriteLine(" [x] Requesting fib(30)");
            var response = rpcClient.Call("30");

            Console.WriteLine(" [.] Got '{0}'", response);
        }

        static void TestConnection()
        {
            var bus = RabbitHutch.CreateBus("host=192.168.0.169:5672;username=admin;password=admin");
            var message = new TextMessage { MessageBody = "Hello Rabbit" };
            bus.Publish<TextMessage>(message);
        }

        static void TestSSL()
        {
            var connection = new ConnectionConfiguration();

            connection.Port = 5673;
            connection.UserName = "admin";
            connection.Password = "admin";
            connection.Product = "SSLTest";

            var host1 = new HostConfiguration();
            host1.Host = "192.168.0.169";
            host1.Port = 5673;
            host1.Ssl.Enabled = true;
            host1.Ssl.ServerName = "www.shiyx.top";
            host1.Ssl.CertPath = @"E:\git\RabbitMqDemo\server.pfx";
            host1.Ssl.CertPassphrase = "123123";
            host1.Ssl.AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch |
                                                SslPolicyErrors.RemoteCertificateChainErrors;

            //var host2 = new HostConfiguration();
            //host2.Host = "192.168.0.115";
            //host2.Port = 5673;
            //host2.Ssl.Enabled = true;
            //host2.Ssl.ServerName = "www.shiyx.top";
            //host2.Ssl.CertPath = @"E:\git\RabbitMqDemo\server.pfx";
            //host2.Ssl.CertPassphrase = "123123";

            connection.Hosts = new List<HostConfiguration> { host1 };

            connection.Validate();

            var bus = RabbitHutch.CreateBus(connection, services => services.Register<ILogProvider>(ConsoleLogProvider.Instance));

            var message = new TextMessage { MessageRouter = "api.notice.zhangsan", MessageBody = "Hello Rabbit" };

            bus.Publish(message, x => x.WithTopic(message.MessageRouter));

            Console.WriteLine("发送成功");
        }

        static void TestSend()
        {
            var connection = new ConnectionConfiguration();

            connection.Port = 5673;
            connection.UserName = "admin";
            connection.Password = "admin";
            connection.Product = "SSLTest";

            var host1 = new HostConfiguration();
            host1.Host = "192.168.0.177";
            host1.Port = 5673;
            host1.Ssl.Enabled = true;
            host1.Ssl.ServerName = "www.shiyx.top";
            host1.Ssl.CertPath = @"E:\git\RabbitMqDemo\server.pfx";
            host1.Ssl.CertPassphrase = "123123";
            host1.Ssl.AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch |
                                                SslPolicyErrors.RemoteCertificateChainErrors;
            
            connection.Hosts = new List<HostConfiguration> { host1 };

            connection.Validate();

            var bus = RabbitHutch.CreateBus(connection, services => services.Register<ILogProvider>(ConsoleLogProvider.Instance));

            bus.Send("my.queue", new TextMessage { MessageID="2", MessageTitle="test", MessageBody = "Hello Widgets!" });
        }
    }
}
