using System;
using Java.Lang;
using Java.Util.Concurrent;
using IExecutor = WebRTC.AppRTC.IExecutor;

namespace WebRTC.Droid.Demo
{
    public class ExecutorServiceImpl : IExecutor
    {
        private readonly IExecutorService _executorService = Executors.NewSingleThreadExecutor();

        public void Execute(Action action)
        {
            _executorService.Execute(new Runnable(action));
        }
        

        public void Dispose()
        {
            _executorService.Shutdown();
            _executorService?.Dispose();
        }
    }
}