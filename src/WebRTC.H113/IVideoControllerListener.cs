using System.Threading.Tasks;
using WebRTC.AppRTC.Abstraction;

namespace WebRTC.H113
{
    public interface IVideoControllerListener
    {
        void OnError(string message);
        
        void OnDisconnect(DisconnectType disconnectType);

        void OnFirstFrame();

        void OnConnect();
        
        Task<bool> RequestCameraPermissionAsync();
    }
}