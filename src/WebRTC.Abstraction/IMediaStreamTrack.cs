namespace WebRTC.Abstraction
{
    public interface IMediaStreamTrack :INativeObject
    {
        string Kind { get; }
        string Label { get; }
        bool IsEnabled { get; set; }
        
        MediaStreamTrackState State { get; }
    }
}