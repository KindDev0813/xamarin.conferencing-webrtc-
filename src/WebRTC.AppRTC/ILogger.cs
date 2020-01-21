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
     
        void Debug(string tag,string message);

       
        void Info(string tag,string message);

      
        void Error(string tag,string message, Exception ex);

      
        void Warning(string tag,string message);
    }
}