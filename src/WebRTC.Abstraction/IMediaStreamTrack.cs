namespace WebRTC.Abstraction
{
    public interface IMediaStreamTrack
    {
        string Kind { get; }
        string Label { get; }
        bool Enable { get; set; }
        
        SourceState State { get; }
    }

    public interface IAudioTrack : IMediaStreamTrack
    {
        float Volume { get; set; }
    }

    public interface IVideoTrack : IMediaStreamTrack
    {
    }
}