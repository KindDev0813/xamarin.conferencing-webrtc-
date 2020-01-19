using System;

namespace WebRTC.AppRTC
{
    public enum LogLevel
    {
        All = 0, // logs all records
        Debug = 1, // logs all records
        Info = 2, // logs info, warning and error records
        Warning = 3, // logs warning and error records
        Error = 4 // logs only error records
    }
    
    public interface ILogger
    {
        /// <summary>
        /// Logs debug message to current log file.
        /// </summary>
        /// <param name="message">The message.</param>
        void Debug(string message);

        /// <summary>
        /// Logs information message to current log file
        /// </summary>
        /// <param name="message">The message.</param>
        void Info(string message);

        /// <summary>
        /// Logs error message including the exception and stack trace to current log file.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        void Error(string message, Exception ex);

        /// <summary>
        /// Logs a warning message to current log file
        /// </summary>
        /// <param name="message">The message.</param>
        void Warning(string message);
    }
}