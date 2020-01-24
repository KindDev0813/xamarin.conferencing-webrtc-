using System;
using System.Threading.Tasks;
using WebRTC.Abstraction;


namespace WebRTC.AppRTC
{
    public enum SignalingChannelState
    {
        Closed,

        // State when connection is established but not ready for use.
        Open,

        // State when connection is established and registered.
        Registered,

        // State when connection encounters a fatal error.
        Error
    }

    public interface ISignalingChannelListener
    {
        void DidChangeState(SignalingChannel channel, SignalingChannelState state);
        void DidReceiveMessage(SignalingChannel channel, SignalingMessage message);
    }
    
    public abstract class SignalingChannel : IDisposable
    {
        protected const string TAG = nameof(SignalingChannel);
        
        private readonly SignalingChannelListenerSchedulerProxy
            _listener = new SignalingChannelListenerSchedulerProxy();

        private SignalingChannelState _state;

        public ISignalingChannelListener Listener
        {
            get => _listener.Listener;
            set => _listener.Listener = value;
        }

        public SignalingChannelState State
        {
            get => _state;
            protected set
            {
                if (_state == value)
                    return;
                _state = value;
                Listener?.DidChangeState(this, value);
            }
        }
        
        public abstract bool IsOpen { get; }

        public virtual void Dispose()
        {
        }

        public abstract Task OpenAsync();

        public abstract Task CloseAsync();
        
        public abstract void SendMessage(SignalingMessage message);

        protected void OnReceivedMessage(string message)
        {
            AppRTC.Logger.Debug(TAG,$"WSS->C:{message}");
            SignalingMessage msg = null;
            try
            {
                msg = SignalingMessage.FromJson(message);
            }
            catch (Exception ex)
            {
                AppRTC.Logger.Error(TAG,"OnReceivedMessage -> Invalid json", ex);
            }

            if (msg == null)
            {
                return;
            }
            
            OnReceivedMessage(msg);

        }

        protected virtual void OnReceivedMessage(SignalingMessage message)
        {
            Listener?.DidReceiveMessage(this, message);
        }

        protected class SignalingChannelListenerSchedulerProxy : ISignalingChannelListener
        {
            private readonly IScheduler _scheduler;

            public SignalingChannelListenerSchedulerProxy(IScheduler scheduler = null)
            {
                _scheduler = scheduler ?? AppRTC.DefaultScheduler;
            }

            public SignalingChannelListenerSchedulerProxy(ISignalingChannelListener listener,
                IScheduler scheduler = null) : this(scheduler)
            {
                Listener = listener;
            }

            public ISignalingChannelListener Listener { get; set; }

            public void DidChangeState(SignalingChannel channel, SignalingChannelState state)
            {
                _scheduler.Schedule(() => Listener.DidChangeState(channel, state));
            }

            public void DidReceiveMessage(SignalingChannel channel, SignalingMessage message)
            {
                _scheduler.Schedule(() => Listener.DidReceiveMessage(channel, message));
            }
        }
    }
}