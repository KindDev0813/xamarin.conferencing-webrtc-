using Org.Webrtc;
using WebRTC.Abstraction;
using WebRTC.Droid.Extensions;

namespace WebRTC.Droid
{
    public class RtpReceiverNative :NativeObjectBase, IRtpReceiver
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