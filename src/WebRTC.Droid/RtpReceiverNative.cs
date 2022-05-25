using WebRTC.Abstraction;
using Org.Webrtc;
using WebRTC.Droid.Extensions;

namespace WebRTC.Droid
{
    internal class RtpReceiverNative :NativeObjectBase, IRtpReceiver
    {
        private readonly RtpReceiver _receiver;

        public RtpReceiverNative(RtpReceiver receiver):base(receiver)
        {
             _receiver = receiver;
        }

        public string Id => _receiver.Id();
        public IMediaStreamTrack Track => _receiver.Track().ToNet();
    }
}