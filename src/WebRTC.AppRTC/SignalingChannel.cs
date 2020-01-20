using System;


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
        private SignalingChannelState _state;

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

        public virtual void Dispose()
        {
        }

        public abstract void Open();

        public abstract void Close();
        public abstract void SendMessage(SignalingMessage message);

        protected void OnReceivedMessage(string message)
        {
            AppRTC.Logger.Debug($"WSS->C:{message}");
            try
            {
                var msg = SignalingMessage.FromJson(message);
                OnReceivedMessage(msg);
            }
            catch (Exception ex)
            {
                AppRTC.Logger.Error("OnReceivedMessage -> Invalid json", ex);
            }
        }

        protected virtual void OnReceivedMessage(SignalingMessage message)
        {
            Listener?.DidReceiveMessage(this, message);
        }
    }
}