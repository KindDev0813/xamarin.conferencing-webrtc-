using Org.Webrtc;
using WebRTC.Abstraction;
using WebRTC.Droid.Extensions;

namespace WebRTC.Droid
{
    public class RtpSenderNative :NativeObjectBase, IRtpSender
    {
        private readonly RtpSender _rtpSender;

        public RtpSenderNative(RtpSender nativeRtpSender):base(nativeRtpSender)
        {
             _rtpSender = nativeRtpSender;
        }
        public string SenderId => _rtpSender.Id();

        public IMediaStreamTrack Track
        {
            get => _rtpSender.Track().ToNet();
            set => _rtpSender.SetTrack(value.ToNative(), true);
        }
    }
}