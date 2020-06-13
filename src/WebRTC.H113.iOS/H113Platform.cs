using System;
using System.Runtime.InteropServices;
using CoreFoundation;
using ObjCRuntime;
using WebRTC.H113.Schedulers;
using WebRTC.iOS.Binding;

namespace WebRTC.H113.iOS
{
    static class H113Platform
    {
        public static void Init()
        {
            WebRTC.iOS.Platform.Init();
            
            ExecutorServiceFactory.MainExecutor = new MainExecutor();

            RTCLog.SetMinDebugLogLevel(RTCLoggingSeverity.Error);

            ExecutorServiceFactory.Factory = (tag) => new ExecutorService(tag);
        }

        public static void Cleanup()
        {
            WebRTC.iOS.Platform.Cleanup();
        }

        private class MainExecutor : IExecutor
        {
            public bool IsCurrentExecutor => DispatchQueue.MainQueue == DispatchQueue.CurrentQueue;

            public void Execute(Action action) => DispatchQueue.MainQueue.DispatchAsync(action);
        }

        private class ExecutorService : IExecutorService
        {
            [DllImport(Constants.libcLibrary)]
            private static extern IntPtr dispatch_release(IntPtr o);

            private DispatchQueue _dispatchQueue;

            public ExecutorService(string tag)
            {
                _dispatchQueue = new DispatchQueue(tag, new DispatchQueue.Attributes
                {
                    QualityOfService = DispatchQualityOfService.Background,
                    Concurrent = false
                });
            }

            public bool IsCurrentExecutor => _dispatchQueue == DispatchQueue.CurrentQueue;

            public void Execute(Action action) => _dispatchQueue.DispatchAsync(action);


            public void Release()
            {
                dispatch_release(_dispatchQueue.Handle);
                _dispatchQueue = null;
            }
        }
    }
}