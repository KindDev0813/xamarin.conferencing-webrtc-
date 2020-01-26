using System.Linq;
using WebRTC.Abstraction;
using WebRTC.iOS.Extensions;
using WebRTC.iOS.Binding;


namespace WebRTC.iOS
{
    internal class MediaStreamNative : NativeObjectBase,IMediaStream
    {
        private  readonly RTCMediaStream _mediaStream;

        public MediaStreamNative(RTCMediaStream mediaStream):base(mediaStream)
        {
            _mediaStream = mediaStream;
        }

        public string StreamId => _mediaStream.StreamId;

        public IAudioTrack[] AudioTracks =>
            _mediaStream.AudioTracks.Select(t => t.ToNet()).Cast<IAudioTrack>().ToArray();

        public IVideoTrack[] VideoTracks =>
            _mediaStream.VideoTracks.Select(t => t.ToNet()).Cast<IVideoTrack>().ToArray();
        
        public void AddTrack(IAudioTrack audioTrack)
        {
            _mediaStream.AddAudioTrack(audioTrack.ToNative<RTCAudioTrack>());
        }

        public void AddTrack(IVideoTrack videoTrack)
        {
            _mediaStream.AddVideoTrack(videoTrack.ToNative<RTCVideoTrack>());
        }

        public void RemoveTrack(IAudioTrack audioTrack)
        {
            _mediaStream.RemoveAudioTrack(audioTrack.ToNative<RTCAudioTrack>());
        }

        public void RemoveTrack(IVideoTrack videoTrack)
        {
            _mediaStream.RemoveVideoTrack(videoTrack.ToNative<RTCVideoTrack>());
        }
    }
}