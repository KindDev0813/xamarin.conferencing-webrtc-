namespace WebRTC.AppRTC
{
    public static class AppRTC
    {
        public static IAppRTCFactory AppRTCFactory { get; private set; }

        public static IScheduler DefaultScheduler { get; private set; }

        public static ILogger Logger { get; private set; }
        
        public static void Init(IAppRTCFactory rtcFactory)
        {
            AppRTCFactory = rtcFactory;
            
            DefaultScheduler = rtcFactory.CreateDefaultScheduler();
            Logger = rtcFactory.CreateLogger();
        }
        
    }
}