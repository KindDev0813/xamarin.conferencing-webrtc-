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
        
        private SignalingChannelState _state;

        protected SignalingChannel(ILogger logger = null)
        {
            Logger = logger ?? new ConsoleLogger();
        }
        
        protected ILogger Logger { get;  }

        public ISignalingChannelListener Listener { get; set; }

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
            SignalingMessage msg = null;
            try
            {
                msg = SignalingMessage.FromJson(message);
            }
            catch (Exception ex)
            {
                Logger.Error(TAG,"OnReceivedMessage -> Invalid json", ex);
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
    }
}