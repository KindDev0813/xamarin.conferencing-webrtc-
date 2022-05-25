namespace WebRTC.Abstraction
{
    public interface IMediaSource : INativeObject
    {
        SourceState State { get; }
    }
}