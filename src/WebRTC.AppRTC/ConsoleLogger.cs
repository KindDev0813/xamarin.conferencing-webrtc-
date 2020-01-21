using System;

namespace WebRTC.AppRTC
{
    public class ConsoleLogger : ILogger
    {
        public LogLevel LogLevel { get; set; }

        public void Debug(string tag, string message)
        {
            if (LogLevel > LogLevel.Debug)
                return;
            LogRecord(tag, message, "DEBUG");
        }

        public void Info(string tag, string message)
        {
            if (LogLevel > LogLevel.Info)
                return;
            LogRecord(tag, message, "INFO");
        }

        public void Warning(string tag, string message)
        {
            if (LogLevel > LogLevel.Warning)
                return;
            LogRecord(tag, message, "WARNING");
        }

        public void Error(string tag, string message, Exception ex)
        {
            if (LogLevel > LogLevel.Error)
                return;
            LogRecord(tag, message, "ERROR", ex);
        }


        private void LogRecord(string tag, string message, string logType, Exception exc = null)
        {
            string rec;
            if (exc == null)
                rec = $"{DateTime.UtcNow} {logType} - {message}";
            else
                rec =
                    $"{DateTime.UtcNow} {logType} {message}. EXCEPTION: {exc.Message}. STACK TRACE: {exc.StackTrace ?? ""}.";

            Console.WriteLine(rec);
        }
    }
}