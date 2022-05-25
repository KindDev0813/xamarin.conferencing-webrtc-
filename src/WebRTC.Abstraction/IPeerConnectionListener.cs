namespace WebRTC.Abstraction
{
    public interface IPeerConnectionListener
    {
        void OnSignalingChange(SignalingState signalingState);
        void OnIceConnectionChange(IceConnectionState iceConnectionState);
        void OnConnectionChange(PeerConnectionState newState);
        void OnIceGatheringChange(IceGatheringState iceGatheringState);
        void OnIceCandidate(IceCandidate iceCandidate);
        void OnIceCandidatesRemoved(IceCandidate[] iceCandidates);
        void OnAddStream(IMediaStream mediaStream);
        void OnRemoveStream(IMediaStream mediaStream);
        void OnDataChannel(IDataChannel dataChannel);
        void OnRenegotiationNeeded();
        void OnAddTrack(IRtpReceiver rtpReceiver, IMediaStream[] mediaStreams);
        void OnTrack(IRtpTransceiver transceiver);
    }
}