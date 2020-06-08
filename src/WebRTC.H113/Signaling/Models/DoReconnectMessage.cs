using Newtonsoft.Json;

namespace WebRTC.H113.Signaling.Models
{
    public class DoReconnectMessage : SignalingMessage
    {
        public DoReconnectMessage(string type, string phoneNumber, string id)
        {
            Type = type;
            PhoneNumber = phoneNumber;
            Id = Id;
        }

        [JsonProperty("type")] public string Type { get; }

        [JsonProperty("id")] public string Id { get; }

        [JsonProperty("phoneNumber")] public string PhoneNumber { get; }
    }
}