using WebRTC.Abstraction;
using WebRTC.iOS.Extensions;
using WebRTC.iOS.Binding;

namespace WebRTC.iOS
{
    internal class MediaSourceNative:NativeObjectBase, IMediaSource
    {
        private readonly RTCMediaSource _mediaSource;

        protected MediaSourceNative(RTCMediaSource mediaSource) : base(mediaSource)
        {
            _mediaSource = mediaSource;
        }

        public SourceState State => _mediaSource.State.ToNet();
    }
}