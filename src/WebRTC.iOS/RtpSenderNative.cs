using WebRTC.Abstraction;
using WebRTC.Abstraction.Extensions;
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

        public IRtpParameters Parameters =>  new RtpParametersNative(_rtpSender.Parameters);
        public bool SetParameters(IRtpParameters parameters)
        {
            try
            {
                _rtpSender.Parameters = parameters.ToNative<RTCRtpParameters>();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}