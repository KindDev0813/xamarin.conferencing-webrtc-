using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace WebRTC.Common
{
    public enum SdpType
    {
        [EnumMember(Value = "answer")]
        Answer,
        [EnumMember(Value = "offer")]
        Offer
    }

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