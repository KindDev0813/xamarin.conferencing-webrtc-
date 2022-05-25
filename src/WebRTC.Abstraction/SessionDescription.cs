using System;

namespace WebRTC.Abstraction
{
    public record SessionDescription(SdpType Type, string Sdp)
    {
        public SdpType Type { get; } = Type;

        public string Sdp { get; } = Sdp;

        public static SdpType GetSdpTypeFromString(string sdp)
        {
            switch (sdp)
            {
                case "offer":
                    return SdpType.Offer;
                case "answer":
                    return SdpType.Answer;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sdp));
            }
        }
    }
}