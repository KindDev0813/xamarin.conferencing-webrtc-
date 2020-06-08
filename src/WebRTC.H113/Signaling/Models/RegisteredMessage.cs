using Newtonsoft.Json;

namespace WebRTC.H113.Signaling.Models
{
    public class RegisteredMessage : SignalingMessage
    {
        public RegisteredMessage()
        {
            MessageType = SignalingMessageType.Registered;
        }

        [JsonProperty("rtcServer")] public RTCServers RTCServer { get; set; }

        [JsonProperty("stun")] public string StunServer { get; set; }

        public class RTCServers
        {
            [JsonProperty("turn1")] public RTCServerEx Turn1 { get; set; }

            [JsonProperty("turn2")] public RTCServerEx Turn2 { get; set; }
        }

        public class RTCServerEx
        {
            [JsonProperty("urls")] public string Url { get; set; }

            [JsonProperty("username")] public string Username { get; set; }

            [JsonProperty("credential")] public string Password { get; set; }

            [JsonProperty("credentialType")] public string CredentialType { get; set; }
        }
    }
}