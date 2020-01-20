using Newtonsoft.Json;

namespace WebRTC.Abstraction
{
    public class IceCandidate
    {
        public IceCandidate(string sdp, string sdpMid, int sdpMLineIndex)
        {
            Sdp = sdp;
            SdpMid = sdpMid;
            SdpMLineIndex = sdpMLineIndex;
        }

        [JsonProperty("sdp")]
        public string Sdp { get; }
        
        [JsonProperty("sdpMid")]
        public string SdpMid { get; }
        
        [JsonProperty("sdpMlineIndex")]
        public int SdpMLineIndex { get; }
    }
}