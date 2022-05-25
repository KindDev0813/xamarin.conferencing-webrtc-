namespace WebRTC.Abstraction
{
    public interface IAudioTrack : IMediaStreamTrack
    {
        float Volume { get; set; }
    }
}