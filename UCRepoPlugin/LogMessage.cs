using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPRi
{

    public enum LogMessageType { Message, Info, Error }

    public struct LogMessage
    {
        public LogMessageType type { get; set; }
        public string Message { get; set; }
    }
}
