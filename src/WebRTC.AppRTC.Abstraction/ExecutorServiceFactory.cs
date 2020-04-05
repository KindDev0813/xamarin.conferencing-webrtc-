using System;

namespace WebRTC.AppRTC.Abstraction
{
    public static class ExecutorServiceFactory
    {
        public static Func<string,IExecutorService> Factory { get; set; }
        public static IExecutor MainExecutor { get; set; }
        public static IExecutorService CreateExecutorService(string tag) => Factory(tag);
    }
}