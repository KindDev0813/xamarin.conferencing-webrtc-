using WebRTC.Abstraction;
using WebRTC.AppRTC.Extensions;

namespace WebRTC.AppRTC
{
    public partial class H113Client : ISignalingEvents
    {
        void ISignalingEvents.OnChannelConnected(RegisteredMessage registeredMessage)
        {
            _logger.Debug(TAG,"Creating PeerConnectionClient");
            _peerConnectionClient =
                new PeerConnectionClient(new PeerConnectionParameters(registeredMessage.GetIceServers()), this,
                    _logger);
        }

        void ISignalingEvents.OnChannelClose()
        {
            _events.OnDisconnect(DisconnectType.WebSocket);
        }

        void ISignalingEvents.OnChannelError(string description)
        {
            _events.OnError(description);
        }

        void ISignalingEvents.OnRemoteDescription(SessionDescription sdp)
        {
            _executor.Execute(() =>
            {
                if (_peerConnectionClient == null)
                {
                    _logger.Error(TAG, "Received remote SDP for non-initilized peer connection.");
                    return;
                }
                _peerConnectionClient.SetRemoteDescription(sdp);
            });
        }

        void ISignalingEvents.OnRemoteIceCandidate(IceCandidate candidate)
        {
            _executor.Execute(() =>
            {
                if (_peerConnectionClient == null)
                {
                    _logger.Error(TAG, "Received remote SDP for non-initilized peer connection.");
                    return;
                }
                _peerConnectionClient.AddRemoteIceCandidate(candidate);
            });
        }

        void ISignalingEvents.OnRemoteIceCandidatesRemoved(IceCandidate[] candidates)
        {
            if (_peerConnectionClient == null)
            {
                _logger.Error(TAG, "Received remote SDP for non-initilized peer connection.");
                return;
            }
            _peerConnectionClient.RemoveRemoteIceCandidates(candidates);
        }
    }
}