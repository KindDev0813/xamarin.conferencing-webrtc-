using System;

namespace WebRTC.AppRTC
{
    public class ConsoleLogger : ILogger
    {
        
        public LogLevel LogLevel { get; set; }
        
        public void Debug(string message)
        {
            if (LogLevel > LogLevel.Debug)
                return;
            LogRecord(message,"DEBUG");
        }

        public void Info(string message)
        {
            if (LogLevel > LogLevel.Info)
                return;
            LogRecord(message,"INFO");
        }

        public void Warning(string message)
        {
            if (LogLevel > LogLevel.Warning)
                return;
            LogRecord(message,"WARNING");
        }

        public void Error(string message, Exception ex)
        {
            if (LogLevel > LogLevel.Error)
                return;
            LogRecord(message,"ERROR",ex);
        }
        
        /// <summary>
        /// Saves a message, possibly exception message and stack trace to current log files
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exc">The exc.</param>
        private void LogRecord(string message,string logType, Exception exc = null)
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