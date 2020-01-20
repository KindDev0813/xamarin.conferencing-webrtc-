namespace WebRTC.Abstraction
{
    public interface IWebRTCFactory
    {
        IPeerConnectionFactory CreateFactory();
    }
}