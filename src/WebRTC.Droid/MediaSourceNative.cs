using Org.Webrtc;
using WebRTC.Abstraction;
using WebRTC.Droid.Extensions;

namespace WebRTC.Droid
{
    public class MediaStreamTrackNative : IMediaStreamTrack
    {
        private readonly MediaStreamTrack _mediaStreamTrack;
        public MediaStreamTrackNative(MediaStreamTrack mediaStreamTrack)
        {
            NativeObject = _mediaStreamTrack = mediaStreamTrack;
        }
        
        public object NativeObject { get; }
        
        public string Kind => _mediaStreamTrack.Kind();
        
        public string Label => _mediaStreamTrack.Id();
        
        public bool Enable
        {
            get => _mediaStreamTrack.Enabled();
            set => _mediaStreamTrack.SetEnabled(value);
        }

        public SourceState State => _mediaStreamTrack.InvokeState().ToNet();
    }
}