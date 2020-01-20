namespace WebRTC.Abstraction
{
    public interface IMediaStream
    {
        string StreamId { get; }
        IAudioTrack[] AudioTracks { get; }
        IVideoTrack[] VideoTracks { get; }

        void AddTrack(IAudioTrack audioTrack);
        void AddTrack(IVideoTrack videoTrack);

        void RemoveTrack(IAudioTrack audioTrack);
        void RemoveTrack(IVideoTrack videoTrack);
    }
}