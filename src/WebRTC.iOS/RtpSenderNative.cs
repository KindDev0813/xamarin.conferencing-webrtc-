using WebRTC.Abstraction;
using WebRTC.iOS.Extensions;
using WebRTC.iOS.Binding;

namespace WebRTC.iOS
{
    internal class RtpSenderNative : NativeObjectBase,IRtpSender
    {
        private readonly IRTCRtpSender _rtpSender;

        public RtpSenderNative(IRTCRtpSender rtpSender) : base(rtpSender)
        {
            _rtpSender = rtpSender;
        }

        public string SenderId => _rtpSender.SenderId;

        public IMediaStreamTrack Track
        {
            get => _rtpSender.Track.ToNet();
            set => _rtpSender.Track = value.ToNative();
        }
    }
}