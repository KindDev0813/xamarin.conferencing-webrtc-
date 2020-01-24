namespace WebRTC.Abstraction
{
    public interface IVideoTrack : IMediaStreamTrack
    {
        void AddRenderer(IVideoRenderer videoRenderer);
        void RemoveRenderer(IVideoRenderer videoRenderer);
    }
}