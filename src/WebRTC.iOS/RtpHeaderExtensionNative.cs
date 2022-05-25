using WebRTC.Abstraction;
using WebRTC.iOS.Binding;

namespace WebRTC.iOS
{
    internal class RtpHeaderExtensionNative : NativeObjectBase, IRtpHeaderExtension
    {
        private readonly RTCRtpHeaderExtension _header;

        public RtpHeaderExtensionNative(RTCRtpHeaderExtension header) : base(header)
        {
            _header = header;
        }
    }
}