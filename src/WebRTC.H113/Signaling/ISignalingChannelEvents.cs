using WebRTC.H113.Signaling.Models;

namespace WebRTC.H113.Signaling
{
    public interface ISignalingChannelEvents
    {
        void ChannelDidChangeState(SignalingChannel channel, SignalingChannelState state);
        void ChannelDidReceiveMessage(SignalingChannel channel, SignalingMessage message);
        void ChannelOnError(SignalingChannel channel, string description);
        void ChannelDidReconnect(SignalingChannel channel);
        void ChannelDidClose(SignalingChannel channel, int code, string reason);
    }
}