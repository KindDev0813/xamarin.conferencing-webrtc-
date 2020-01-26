using System.Linq;
using CoreFoundation;
using Foundation;
using WebRTC.Abstraction;
using WebRTC.iOS.Extensions;
using WebRTC.iOS.Binding;


namespace WebRTC.iOS
{
    internal class PeerConnectionListenerProxy : NSObject, IRTCPeerConnectionDelegate
    {
        private readonly IPeerConnectionListener _listener;

        public PeerConnectionListenerProxy(IPeerConnectionListener listener)
        {
            _listener = listener;
        }

        public void DidChangeSignalingState(RTCPeerConnection peerConnection, RTCSignalingState stateChanged)
        {
            DispatchQueue.MainQueue.DispatchAsync(() => _listener?.OnSignalingChange(stateChanged.ToNet()));
        }

        public void DidAddStream(RTCPeerConnection peerConnection, RTCMediaStream stream)
        {
            DispatchQueue.MainQueue.DispatchAsync(() => _listener?.OnAddStream(new MediaStreamNative(stream)));
        }

        public void DidRemoveStream(RTCPeerConnection peerConnection, RTCMediaStream stream)
        {
            DispatchQueue.MainQueue.DispatchAsync(() => _listener?.OnRemoveStream(new MediaStreamNative(stream)));
        }

        public void PeerConnectionShouldNegotiate(RTCPeerConnection peerConnection)
        {
            DispatchQueue.MainQueue.DispatchAsync(() => _listener?.OnRenegotiationNeeded());
        }

        public void DidChangeIceConnectionState(RTCPeerConnection peerConnection, RTCIceConnectionState newState)
        {
            DispatchQueue.MainQueue.DispatchAsync(() => _listener?.OnIceConnectionChange(newState.ToNet()));
        }

        public void DidChangeIceGatheringState(RTCPeerConnection peerConnection, RTCIceGatheringState newState)
        {
            DispatchQueue.MainQueue.DispatchAsync(() => _listener?.OnIceGatheringChange(newState.ToNet()));
        }

        public void DidGenerateIceCandidate(RTCPeerConnection peerConnection, RTCIceCandidate candidate)
        {
            DispatchQueue.MainQueue.DispatchAsync(() => _listener?.OnIceCandidate(candidate.ToNet()));
        }

        public void DidRemoveIceCandidates(RTCPeerConnection peerConnection, RTCIceCandidate[] candidates)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
                _listener?.OnIceCandidatesRemoved(candidates.ToNet().ToArray()));
        }

        public void DidOpenDataChannel(RTCPeerConnection peerConnection, RTCDataChannel dataChannel)
        {
            DispatchQueue.MainQueue.DispatchAsync(() => _listener?.OnDataChannel(new DataChannelNative(dataChannel)));
        }
    }
}