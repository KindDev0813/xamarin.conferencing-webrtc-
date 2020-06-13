using Newtonsoft.Json;

namespace WebRTC.H113.Signaling.Models
{
    public class DoReconnectMessage : SignalingMessage
    {
        public DoReconnectMessage(string phoneNumber, string id)
        {
            PhoneNumber = phoneNumber;
            Id = Id;
            MessageType = SignalingMessageType.DoReconnect;
        }

        [JsonProperty("id")]
        public string Id { get; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; }
    }
}