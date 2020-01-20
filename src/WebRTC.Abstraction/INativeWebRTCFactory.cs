namespace WebRTC.Abstraction
{
    public interface INativeWebRTCFactory 
    {
        IPeerConnectionFactory CreateFactory();
    }
}