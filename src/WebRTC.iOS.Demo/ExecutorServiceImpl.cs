using System;
using CoreFoundation;
using WebRTC.AppRTC;

namespace WebRTC.iOS.Demo
{
    public class ExecutorServiceImpl : IExecutor
    {
        private readonly DispatchQueue _dispatchQueue = new DispatchQueue("ExecutorServiceImpl",false);
        
        
        public void Execute(Action action)
        {
            _dispatchQueue.DispatchAsync(action);
        }
        

        public void Dispose()
        {
            _dispatchQueue?.Dispose();
        }
    }
}