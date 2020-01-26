using System;

namespace WebRTC.AppRTC
{
    public interface IExecutor : IDisposable
    {
        void Execute(Action action);
    }
}