using Newtonsoft.Json;

namespace WebRTC.H113.Signaling.Models
{
    public class RegisterMessage : SignalingMessage
    {
        public RegisterMessage(string phoneNumber, double longitude, double latitude)
        {
            PhoneNumber = phoneNumber;
            Longitude = longitude;
            Latitude = latitude;
            MessageType = SignalingMessageType.Register;
        }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; }

        [JsonProperty("longitude")]
        public double Longitude { get; }

        [JsonProperty("latitude")]
        public double Latitude { get; }
    }
}