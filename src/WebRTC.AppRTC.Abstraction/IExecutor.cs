using System;

namespace WebRTC.AppRTC.Abstraction
{
    public interface IExecutor 
    {
        bool IsCurrentExecutor { get; }
        
        void Execute(Action action);
    }

    public interface IExecutorService : IExecutor
    {
        void Release();
    }
}