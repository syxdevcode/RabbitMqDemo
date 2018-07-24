using EasyNetQ;
using EasyNetQ.Events;
using EasyNetQ.Logging;
using EasyNetQDemo.Message;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace EasyNetQDemo.Subscribe
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
            var connection = new ConnectionConfiguration();
            connection.Port = 5673;
            connection.UserName = "admin";
            connection.Password = "admin";
            connection.Product = "SSLTest";

            var host1 = new HostConfiguration();
            host1.Host = "192.168.0.183";
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

            Console.WriteLine(" [x] Awaiting RPC requests");

            bus.Respond<TextMessage, ResponseMessage>(request => new ResponseMessage { MessageBody = "Responding to " + request.MessageBody });
        }

        /// Assumes only valid positive integer input.
        /// Don't expect this one to work for big numbers, and it's
        /// probably the slowest recursive implementation possible.
        /// 
        private static int fib(int n)
        {
            if (n == 0 || n == 1)
            {
                return n;
            }

            return fib(n - 1) + fib(n - 2);
        }

        static void Test1()
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
            host1.Ssl.CertPath = @"E:\git\RabbitMqDemo\client.pfx";
            host1.Ssl.CertPassphrase = "123456";
            host1.Ssl.AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch |
                                                SslPolicyErrors.RemoteCertificateChainErrors;

            connection.Hosts = new List<HostConfiguration> { host1 };

            connection.Validate();

            var bus = RabbitHutch.CreateBus(connection, services => services.Register<ILogProvider>(ConsoleLogProvider.Instance));

            bus.Subscribe<TextMessage>("api.notice.zhangsan", msg => Console.WriteLine(msg.MessageBody), x => x.WithTopic("api.notice.zhangsan"));
        }

        static void TestReceive()
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
            host1.Ssl.CertPath = @"E:\git\RabbitMqDemo\client.pfx";
            host1.Ssl.CertPassphrase = "123456";
            host1.Ssl.AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch |
                                                SslPolicyErrors.RemoteCertificateChainErrors;

            connection.Hosts = new List<HostConfiguration> { host1 };

            connection.Validate();

            var bus = RabbitHutch.CreateBus(connection, services => services.Register<ILogProvider>(ConsoleLogProvider.Instance));

            bus.Receive<TextMessage>("my.queue", message => Console.WriteLine("MyMessage: {0}", message.MessageBody));

        }
    }
}
