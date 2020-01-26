using WebRTC.Abstraction;
using WebRTC.iOS.Extensions;
using WebRTC.iOS.Binding;


namespace WebRTC.iOS
{
    internal class RtpReceiverNative :NativeObjectBase, IRtpReceiver
    {
        private readonly IRTCRtpReceiver _rtpReceiver;
        public RtpReceiverNative(IRTCRtpReceiver rtpReceiver):base(rtpReceiver)
        {
            _rtpReceiver = rtpReceiver;
        }

        public string Id => _rtpReceiver.ReceiverId;
        public IMediaStreamTrack Track => _rtpReceiver.Track.ToNet();
    }
}