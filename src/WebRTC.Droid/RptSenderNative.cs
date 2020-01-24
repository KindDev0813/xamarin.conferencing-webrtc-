using Org.Webrtc;
using WebRTC.Abstraction;
using WebRTC.Droid.Extensions;

namespace WebRTC.Droid
{
    internal class RptSenderNative : NativeObjectBase, IRtpSender
    {
        private readonly RtpSender _rtpSender;

        public RptSenderNative(RtpSender rtpSender) : base(rtpSender)
        {
            _rtpSender = rtpSender;
        }

        public string SenderId => _rtpSender.Id();
        
        public IMediaStreamTrack Track => _rtpSender.Track()?.ToNet();
    }
}