using Newtonsoft.Json;

namespace WebRTC.H113.Signaling.Models
{
    public class SignalingMessage
    {
        [JsonProperty("type")] public SignalingMessageType MessageType { get; set; }

        [JsonProperty("amkSocketId")] public string SocketId { get; set; }

        public string ToJson()
        {
            return SignalingMessageFactory.ToJson(this);
        }

        public override string ToString()
        {
            return ToJson();
        }
    }
}