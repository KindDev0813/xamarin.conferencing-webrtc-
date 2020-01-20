using Newtonsoft.Json;

namespace WebRTC.Abstraction
{
    public class SessionDescription
    {
        public SessionDescription(SdpType type, string sdp)
        {
            Sdp = sdp;
            Type = type;
        }


        [JsonProperty("type")] public SdpType Type { get; }

        [JsonProperty("sdp")] public string Sdp { get; }
    }
}