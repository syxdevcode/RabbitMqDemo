using System;

namespace EasyNetQDemo.Message
{
    public class TextMessage
    {
        public string MessageID { get; set; }

        public string MessageTitle { get; set; }

        public string MessageBody { get; set; }

        public string MessageRouter { get; set; }
    }
}
