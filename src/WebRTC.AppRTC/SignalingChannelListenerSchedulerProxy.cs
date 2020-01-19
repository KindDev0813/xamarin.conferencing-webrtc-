namespace WebRTC.AppRTC
{
    public class SignalingChannelListenerSchedulerProxy : ISignalingChannelListener
    {
        private readonly ISignalingChannelListener _listener;
        private readonly IScheduler _scheduler;

        public SignalingChannelListenerSchedulerProxy(ISignalingChannelListener listener, IScheduler scheduler)
        {
            _listener = listener;
            _scheduler = scheduler;
        }

        public void DidChangeState(SignalingChannel channel, SignalingChannelState state)
        {
            _scheduler.Schedule(() => _listener.DidChangeState(channel, state));
        }

        public void DidReceiveMessage(SignalingChannel channel, SignalingMessage message)
        {
            _scheduler.Schedule(() => _listener.DidReceiveMessage(channel, message));
        }
    }
}