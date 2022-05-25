using Org.Webrtc;
using WebRTC.Abstraction;

namespace WebRTC.Droid
{
    internal class RtpHeaderExtensionNative : NativeObjectBase, IRtpHeaderExtension
    {
        private readonly RtpParameters.HeaderExtension _header;

        public RtpHeaderExtensionNative(RtpParameters.HeaderExtension header) : base(header)
        {
            _header = header;
        }
    }
}