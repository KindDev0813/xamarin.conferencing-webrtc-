namespace WebRTC.Abstraction
{
    public interface IVideoFrame
    {
        
    }
    
    public interface IVideoSink
    {
        void OnFrame(IVideoFrame frame);
    }
}