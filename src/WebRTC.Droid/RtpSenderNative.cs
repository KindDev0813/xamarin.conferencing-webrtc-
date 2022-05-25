using WebRTC.Abstraction;
using Org.Webrtc;
using WebRTC.Abstraction.Extensions;
using WebRTC.Droid.Extensions;

namespace WebRTC.Droid
{
    internal class RtpSenderNative :NativeObjectBase, IRtpSender
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

        public IRtpParameters Parameters => new RtpParametersNative(_rtpSender.Parameters);
        public bool SetParameters(IRtpParameters parameters)
        {
            return _rtpSender.SetParameters(parameters.ToNative<RtpParameters>());
        }
    }
}