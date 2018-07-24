using System;
using System.Collections.Generic;
using System.Text;

namespace EasyNetQDemo.Message
{
    public class ResponseMessage
    {
        public string MessageID { get; set; }

        public string MessageTitle { get; set; }

        public string MessageBody { get; set; }

        public string MessageRouter { get; set; }
    }
}
