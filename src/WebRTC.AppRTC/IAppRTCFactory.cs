namespace WebRTC.AppRTC
{
    public interface IAppRTCFactory
    {
        IWebSocketConnection CreateWebSocketConnection();
        IScheduler CreateDefaultScheduler();
        ILogger CreateLogger();
    }
}