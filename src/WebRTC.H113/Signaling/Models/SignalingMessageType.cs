using System.Runtime.Serialization;

namespace WebRTC.H113.Signaling.Models
{
    public enum SignalingMessageType
    {
        Unknown,

        [EnumMember(Value = MessageTypesConstants.Register)]
        Register,

        [EnumMember(Value = MessageTypesConstants.Registered)]
        Registered,

        [EnumMember(Value = MessageTypesConstants.SendOffer)]
        Offer,

        [EnumMember(Value = MessageTypesConstants.ReceivedAnswer)]
        ReceivedAnswer,

        [EnumMember(Value = MessageTypesConstants.ReceiveCandidate)]
        ReceiveCandidate,

        [EnumMember(Value = MessageTypesConstants.SendCandidate)]
        SendCandidate,

        [EnumMember(Value = MessageTypesConstants.Reconnecting)]
        Reconnecting,

        [EnumMember(Value = MessageTypesConstants.UpdateInfo)]
        UpdateInfo,

        [EnumMember(Value = MessageTypesConstants.DoReconnect)]
        DoReconnect,

        [EnumMember(Value = MessageTypesConstants.CloseConnection)]
        CloseConnection,

        [EnumMember(Value = MessageTypesConstants.StopVideo)]
        StopVideo,
    }
}