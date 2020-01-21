using Org.Webrtc;
using WebRTC.Abstraction;
using WebRTC.Droid.Extensions;

namespace WebRTC.Droid
{
    public class MediaStreamTrackNative : NativeObjectBase, IMediaStreamTrack
    {
        private readonly MediaStreamTrack _mediaStreamTrack;
        public MediaStreamTrackNative(MediaStreamTrack mediaStreamTrack) : base(mediaStreamTrack)
        {
            _mediaStreamTrack = mediaStreamTrack;
        }

        public string Kind => _mediaStreamTrack.Kind();

        public string Label => _mediaStreamTrack.Id();

        public bool Enable
        {
            get => _mediaStreamTrack.Enabled();
            set => _mediaStreamTrack.SetEnabled(value);
        }

        public MediaStreamTrackState State => _mediaStreamTrack.InvokeState().ToNet();
    }
}