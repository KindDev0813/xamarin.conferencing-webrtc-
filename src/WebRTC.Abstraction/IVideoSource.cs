namespace WebRTC.Abstraction
{
    public interface IVideoSource : IMediaSource
    {
        void AdaptOutputFormat(int width, int height, int fps);
    }
}