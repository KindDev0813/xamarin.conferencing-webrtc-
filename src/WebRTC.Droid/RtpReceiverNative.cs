using Org.Webrtc;
using WebRTC.Abstraction;

namespace WebRTC.Droid
{
    public class RtpReceiverNative : IRtpReceiver
    {
        private readonly RtpReceiver _receiver;

        public RtpReceiverNative(RtpReceiver receiver)
        {
            _receiver = receiver;
        }
    }
}