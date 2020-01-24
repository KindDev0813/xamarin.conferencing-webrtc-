namespace WebRTC.Abstraction
{
    public interface IMediaStreamTrack :INativeObject
    {
        string Kind { get; }
        string Label { get; }
        bool Enable { get; set; }
        
        MediaStreamTrackState State { get; }
    }
}