using Newtonsoft.Json;

namespace WebRTC.H113.Signaling.Models
{
    public class ReconnectingMessage : SignalingMessage
    {
        public ReconnectingMessage(string id, string phoneNumber)
        {
            Id = id;
            PhoneNumber = phoneNumber;
            MessageType = SignalingMessageType.Reconnecting;
        }

        [JsonProperty("id")]
        public string Id { get; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; }
    }
}