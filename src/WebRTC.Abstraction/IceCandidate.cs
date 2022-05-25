namespace WebRTC.Abstraction
{
    public record IceCandidate(string Sdp, string SdpMid, int SdpMLineIndex)
    {
        public string Sdp { get; } = Sdp;

        public string SdpMid { get; } = SdpMid;

        public int SdpMLineIndex { get; } = SdpMLineIndex;
    }
}