using Org.Webrtc;

namespace WebRTC.Sample.Droid
{
    public partial class PeerConnectionClient :PeerConnection.IObserver
    {
        public void OnAddStream(MediaStream p0)
        {
            
        }

        public void OnAddTrack(RtpReceiver p0, MediaStream[] p1)
        {
        }

        public void OnConnectionChange(PeerConnection.PeerConnectionState newState)
        {
        }

        public void OnDataChannel(DataChannel p0)
        {
        }

        public void OnIceCandidate(IceCandidate p0)
        {
        }

        public void OnIceCandidatesRemoved(IceCandidate[] p0)
        {
        }

        public void OnIceConnectionChange(PeerConnection.IceConnectionState p0)
        {
        }

        public void OnIceConnectionReceivingChange(bool p0)
        {
        }

        public void OnIceGatheringChange(PeerConnection.IceGatheringState p0)
        {
        }

        public void OnRemoveStream(MediaStream p0)
        {
        }

        public void OnRenegotiationNeeded()
        {
        }

        public void OnSignalingChange(PeerConnection.SignalingState p0)
        {
        }
    }
}