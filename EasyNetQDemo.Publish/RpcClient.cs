using EasyNetQ;
using EasyNetQ.Events;
using EasyNetQ.Logging;
using EasyNetQDemo.Message;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace EasyNetQDemo.Publish
{
    public class RpcClient
    {
        private readonly IBus bus;
        private readonly string replyQueueName;

        private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
        private readonly MessageProperties propes;

        public RpcClient()
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

            bus = RabbitHutch.CreateBus(connection, services => services.Register<ILogProvider>(ConsoleLogProvider.Instance));

        }
        public string Call(string message)
        {
            var request = new TextMessage() { MessageTitle = "requestTitle", MessageID = "1", MessageBody = "bodys" };

            string result = string.Empty;
            var task = bus.RequestAsync<TextMessage, ResponseMessage>(request);
            task.ContinueWith(response =>
            {
                Console.WriteLine("Got response: '{0}'", response.Result.MessageBody);
            });

            return result;
        }
    }

    public class TestRequestMessage
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public bool CausesExceptionInServer { get; set; }
        public string ExceptionInServerMessage { get; set; }
        public bool CausesServerToTakeALongTimeToRespond { get; set; }
    }

    public class TestResponseMessage
    {
        public long Id { get; set; }
        public string Text { get; set; }
    }

    public class TestAsyncRequestMessage
    {
        public string Text { get; set; }
    }

    public class TestAsyncResponseMessage
    {
        public string Text { get; set; }
    }
}
