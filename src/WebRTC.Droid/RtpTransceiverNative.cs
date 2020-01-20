using Org.Webrtc;
using WebRTC.Abstraction;

namespace WebRTC.Droid
{
    public class RtpTransceiverNative:IRtpTransceiver
    {
        public RtpTransceiverNative(RtpTransceiver rtpTransceiverNative)
        {
            NativeObject = rtpTransceiverNative;
        }

        public object NativeObject { get; }
    }
}