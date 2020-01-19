using System;
using WebRTC.Common;
using SessionDescription = Org.Webrtc.SessionDescription;

namespace WebRTC.Droid.Extensions
{
    public static class SessionDescriptionExtension
    {
        public static SessionDescription ToNative(this Common.SessionDescription self)
        {
            return new SessionDescription(ToNative(self.Type), self.Sdp);
        }

        public static Common.SessionDescription ToNet(this SessionDescription self)
        {
            return new Common.SessionDescription(ToNet(self.SdpType), self.Description);
        }

        private static SessionDescription.Type ToNative(Common.SdpType sdpType)
        {
            switch (sdpType)
            {
                case SdpType.Answer:
                    return SessionDescription.Type.Answer;
                case SdpType.Offer:
                    return SessionDescription.Type.Offer;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sdpType), sdpType, null);
            }
        }


        private static SdpType ToNet(SessionDescription.Type type)
        {
            if (type == SessionDescription.Type.Answer)
                return SdpType.Answer;
            if (type == SessionDescription.Type.Offer)
                return SdpType.Offer;
            throw new NotSupportedException();
        }
    }
}