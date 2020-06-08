using System.Runtime.Serialization;

namespace WebRTC.H113.Signaling.Models
{
    public enum SignalingMessageType
    {
        Unknown,

        [EnumMember(Value = "new-app-connection")]
        Register,

        [EnumMember(Value = "app-start-video")]
        Registered,
        [EnumMember(Value = "amk-send-offer")] Offer,
        [EnumMember(Value = "receive-answer")] ReceivedAnswer,

        [EnumMember(Value = "receive-candidate")]
        ReceiveCandidate,

        [EnumMember(Value = "amk-send-candidate")]
        SendCandidate,

        [EnumMember(Value = "app-connection-id")]
        Reconnecting,

        [EnumMember(Value = "app-update-info")]
        UpdateInfo,

        [EnumMember(Value = "app-reconnecting-ws")]
        DoReconnect,

        [EnumMember(Value = "app-close-connection")]
        CloseConnection,
        
        [EnumMember(Value = "app-amk-stop-video")]
        StopVideo,
    }
}