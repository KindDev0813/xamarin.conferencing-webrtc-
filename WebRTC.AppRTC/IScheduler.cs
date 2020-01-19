using System;

namespace WebRTC.AppRTC
{
    public interface IScheduler
    {
        void Schedule(Action action);
    }
}