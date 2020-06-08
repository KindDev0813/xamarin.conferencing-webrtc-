using System;

namespace WebRTC.H113.Schedulers
{
    public interface IExecutor 
    {
        bool IsCurrentExecutor { get; }
        
        void Execute(Action action);
    }
}