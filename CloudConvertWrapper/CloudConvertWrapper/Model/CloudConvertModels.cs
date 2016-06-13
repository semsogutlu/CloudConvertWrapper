using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudConvertWrapper.Model
{
    public class CloudConvertProcessResponse
    {
        public string Url { get; set; }
        public string Id { get; set; }
        public string Host { get; set; }
        public DateTime? Expires { get; set; }
        public int MaxTime { get; set; }
        public int Minutes { get; set; }
    }

    public class CloudConvertConversationResponse
    {
        public string Url { get; set; }
        public string Id { get; set; }
        public string Expire { get; set; }
        public double Percent { get; set; }
        public string Message { get; set; }
        public Upload Upload { get; set; }
        public Output Output { get; set; }
    }

    public class Upload
    {
        public string Url { get; set; }
    }

    public class Output
    {
        public string Url { get; set; }
    }

    public class CloudConvertResponseResult
    {
        public bool IsError { get; set; }
        public string Result { get; set; }
    }
}