namespace WebRTC.H113.Signaling.Models
{
    public class StopVideoMessage : SignalingMessage
    {
        public StopVideoMessage()
        {
            MessageType = SignalingMessageType.StopVideo;
        }
    }
}