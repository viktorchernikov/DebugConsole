using System;
using System.Text;

namespace VCUE
{
    /// <summary>
    /// Entry that captures relevant information about an event, such as the severity level, a timestamp, the source or location of the event.
    /// </summary>
    public struct LogMessage
    {
        public LogMessage(string message, string source, DateTime timestamp, LogSeverity severity = LogSeverity.Info)
        {
            Message = message;
            Source = source;
            Timestamp = timestamp;
            Severity = severity;
        }


        public readonly DateTime Timestamp;
        public readonly string Message;
        public readonly string Source;
        public readonly LogSeverity Severity;


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(Timestamp.Hour);
            sb.Append(":");
            sb.Append(Timestamp.Minute);
            sb.Append(":");
            sb.Append(Timestamp.Second);
            sb.Append("] ");
            if (Source != null)
            {
                sb.Append("{");
                sb.Append(Source);
                sb.Append("} ");
            }
            sb.Append(Message);
            return sb.ToString();
        }
    }
}
