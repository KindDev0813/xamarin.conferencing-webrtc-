using Newtonsoft.Json;

namespace WebRTC.H113.Signaling.Models
{
    public class RegisterMessage : SignalingMessage
    {
        public RegisterMessage(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
            MessageType = SignalingMessageType.Register;
        }

        [JsonProperty("phoneNumber")] public string PhoneNumber { get; }
    }
}