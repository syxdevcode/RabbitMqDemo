using EasyNetQ;
using EasyNetQ.Logging;
using EasyNetQDemo.Message;
using System;
using System.Collections.Generic;
using System.Net.Security;

namespace EasyNetQDemo.Subscribe
{
    class Program
    {
        static void Main(string[] args)
        {
            Test1();

            Console.ReadKey();
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

            bus.Subscribe<TextMessage>("api.notice.zhangsan", msg => Write(msg.MessageBody), x => x.WithTopic("api.notice.zhangsan"));
        }

        static void Write(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
