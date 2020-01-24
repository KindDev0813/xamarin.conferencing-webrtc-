using WebRTC.Abstraction;
using WebRTC.iOS.Extensions;

namespace WebRTC.iOS
{
    internal class MediaSourceNative:NativeObjectBase, IMediaSource
    {
        private readonly RTCMediaSource _mediaSource;

        public MediaSourceNative(RTCMediaSource mediaSource) : base(mediaSource)
        {
            _mediaSource = mediaSource;
        }

        public SourceState State => _mediaSource.State.ToNet();
    }
}