using System.Collections;
using System.Linq;
using Org.Webrtc;
using WebRTC.Abstraction;

namespace WebRTC.Droid
{
    public class MediaStreamNative : IMediaStream
    {
        private readonly MediaStream _mediaStream;

        public MediaStreamNative(MediaStream mediaStream)
        {
            _mediaStream = mediaStream;
        }

        public string StreamId => _mediaStream.Id;

        public IAudioTrack[] AudioTracks => GetAudioTracks();

        public IVideoTrack[] VideoTracks => GetVideoTracks();

        public void AddTrack(IAudioTrack audioTrack)
        {
            var nativeAudioTrack = (AudioTrackNative) audioTrack;
            _mediaStream.AddTrack((AudioTrack)nativeAudioTrack.NativeTrack);
        }

        public void AddTrack(IVideoTrack videoTrack)
        {
            var nativeVideoTrack = (VideoTrackNative) videoTrack;
            _mediaStream.AddTrack((VideoTrack)nativeVideoTrack.NativeTrack);
        }

        public void RemoveTrack(IAudioTrack audioTrack)
        {
            var nativeAudioTrack = (AudioTrackNative) audioTrack;
            _mediaStream.RemoveTrack((AudioTrack)nativeAudioTrack.NativeTrack);
        }

        public void RemoveTrack(IVideoTrack videoTrack)
        {
            var nativeVideoTrack = (VideoTrackNative) videoTrack;
            _mediaStream.RemoveTrack((VideoTrack)nativeVideoTrack.NativeTrack);
        }
        private IAudioTrack[] GetAudioTracks()
        {
            var items = _mediaStream.AudioTracks;
            var arr = new IAudioTrack[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                arr[i] = new AudioTrackNative((AudioTrack) items[i]);
            }
            return arr;
        }
        
        private IVideoTrack[] GetVideoTracks()
        {
            var items = _mediaStream.VideoTracks;
            var arr = new IVideoTrack[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                arr[i] = new VideoTrackNative((VideoTrack) items[i]);
            }
            return arr;
        }
    }
}