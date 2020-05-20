using System.Threading.Tasks;
using WebRTC.AppRTC.Abstraction;

namespace WebRTC.H113
{
    public interface IVideoControllerListener
    {
        void ShowNotification(int type, string title, string message);

        void OnError(string message);

        void OnDisconnect(DisconnectType disconnectType);

        void OnFirstFrame();

        void OnConnect(DisconnectType disconnectType);

        Task<bool> RequestCameraPermissionAsync();
    }
}