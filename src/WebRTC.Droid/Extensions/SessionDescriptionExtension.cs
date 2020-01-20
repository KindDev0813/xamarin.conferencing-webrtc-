using System;
using WebRTC.Abstraction;
using SessionDescription = Org.Webrtc.SessionDescription;

namespace WebRTC.Droid.Extensions
{
    public static class SessionDescriptionExtension
    {
        public static SessionDescription ToNative(this Abstraction.SessionDescription self)
        {
            return new SessionDescription(self.Type.ToNative(), self.Sdp);
        }

        public static Abstraction.SessionDescription ToNet(this SessionDescription self)
        {
            return new Abstraction.SessionDescription(self.SdpType.ToNet(), self.Description);
        }
    }
}