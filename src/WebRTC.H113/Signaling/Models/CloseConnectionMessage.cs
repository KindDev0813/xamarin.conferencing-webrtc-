namespace WebRTC.H113.Signaling.Models
{
    public class CloseConnectionMessage : SignalingMessage
    {
        public CloseConnectionMessage()
        {
            MessageType = SignalingMessageType.CloseConnection;
        }
    }
}