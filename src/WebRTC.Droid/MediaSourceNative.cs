using Org.Webrtc;
using WebRTC.Abstraction;
using WebRTC.Droid.Extensions;

namespace WebRTC.Droid
{
    internal class MediaSourceNative : NativeObjectBase, IMediaSource
    {
        private readonly MediaSource _mediaSource;

        public MediaSourceNative(MediaSource mediaSource) : base(mediaSource)
        {
            _mediaSource = mediaSource;
        }

        public SourceState State => _mediaSource.InvokeState().ToNet();
    }
}