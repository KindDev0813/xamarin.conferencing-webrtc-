using System.Linq;
using WebRTC.Abstraction;
using WebRTC.iOS.Extensions;
using WebRTC.iOS.Binding;


namespace WebRTC.iOS
{
    internal class PeerConnectionListenerProxy : RTCPeerConnectionDelegate
    {
        private readonly IPeerConnectionListener _listener;

        public PeerConnectionListenerProxy(IPeerConnectionListener listener)
        {
            _listener = listener;
        }

        public override void DidChangeSignalingState(RTCPeerConnection peerConnection, RTCSignalingState stateChanged)
        {
            _listener?.OnSignalingChange(stateChanged.ToNet());
        }

        public override void DidAddStream(RTCPeerConnection peerConnection, RTCMediaStream stream)
        {
            _listener?.OnAddStream(new MediaStreamNative(stream));
        }

        public override void DidRemoveStream(RTCPeerConnection peerConnection, RTCMediaStream stream)
        {
            _listener?.OnRemoveStream(new MediaStreamNative(stream));
        }

        public override void PeerConnectionShouldNegotiate(RTCPeerConnection peerConnection)
        {
            _listener?.OnRenegotiationNeeded();
        }

        public override void DidChangeIceConnectionState(RTCPeerConnection peerConnection, RTCIceConnectionState newState)
        {
            _listener?.OnIceConnectionChange(newState.ToNet());
        }

        public override void DidChangeIceGatheringState(RTCPeerConnection peerConnection, RTCIceGatheringState newState)
        {
            _listener?.OnIceGatheringChange(newState.ToNet());
        }

        public override void DidGenerateIceCandidate(RTCPeerConnection peerConnection, RTCIceCandidate candidate)
        {
            _listener?.OnIceCandidate(candidate.ToNet());
        }

        public override void DidRemoveIceCandidates(RTCPeerConnection peerConnection, RTCIceCandidate[] candidates)
        {
            _listener?.OnIceCandidatesRemoved(candidates.ToNet().ToArray());
        }

        public override void DidOpenDataChannel(RTCPeerConnection peerConnection, RTCDataChannel dataChannel)
        {
            _listener?.OnDataChannel(new DataChannelNative(dataChannel));
        }

        public override void DidChangeConnectionState(RTCPeerConnection peerConnection, RTCPeerConnectionState newState)
        {
            _listener?.OnConnectionChange(newState.ToNet());
        }

    }
}