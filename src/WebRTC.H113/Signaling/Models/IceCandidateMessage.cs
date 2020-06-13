using Newtonsoft.Json;
using WebRTC.Abstraction;

namespace WebRTC.H113.Signaling.Models
{
    public class IceCandidateMessage : SignalingMessage
    {
        public IceCandidateMessage()
        {
            MessageType = SignalingMessageType.ReceiveCandidate;
        }

        public IceCandidateMessage(IceCandidate iceCandidate)
        {
            MessageType = SignalingMessageType.SendCandidate;
            IceCandidate = new IceCandidateEx
            {
                Sdp = iceCandidate.Sdp,
                SdpMid = iceCandidate.SdpMid,
                SdpMLineIndex = iceCandidate.SdpMLineIndex
            };
        }

        [JsonProperty("candidate")]
        public IceCandidateEx IceCandidate { get; set; }

        public class IceCandidateEx
        {
            [JsonProperty("candidate")]
            public string Sdp { get; set; }

            [JsonProperty("sdpMid")]
            public string SdpMid { get; set; }

            [JsonProperty("sdpMlineIndex")]
            public int SdpMLineIndex { get; set; }
        }
    }
}