using Newtonsoft.Json;
using WebRTC.Abstraction;

namespace WebRTC.H113.Signaling.Models
{
    public class SessionDescriptionMessage : SignalingMessage
    {
        public SessionDescriptionMessage()
        {
            MessageType = SignalingMessageType.ReceivedAnswer;
        }

        public SessionDescriptionMessage(SessionDescription description)
        {
            MessageType = SignalingMessageType.Offer;
            Description = description;
        }

        [JsonProperty("offer")]
        public SessionDescription Description { get; set; }
    }
}