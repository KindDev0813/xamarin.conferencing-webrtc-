namespace WebRTC.H113.Signaling.Models
{
    public static class MessageTypesConstants
    {
        public const string Register = "new-app-connection";
        public const string Registered = "app-start-video";
        public const string SendOffer = "amk-send-offer";
        public const string ReceivedAnswer = "app-receive-answer";
        public const string ReceiveCandidate = "app-receive-candidate";
        public const string SendCandidate = "amk-send-candidate";
        public const string Reconnecting = "app-connection-id";
        public const string UpdateInfo = "app-update-info";
        public const string DoReconnect = "app-reconnecting-ws";
        public const string StopVideo = "app-amk-stop-video";
        public const string CloseConnection = "app-close-connection";
    }
}