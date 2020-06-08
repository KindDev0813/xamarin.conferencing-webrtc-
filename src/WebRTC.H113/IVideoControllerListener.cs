using System.Threading.Tasks;

namespace WebRTC.H113
{
    public interface IVideoControllerListener
    {
        void ShowNotification(int type, string title, string message);

        void OnError(string message);

        void OnDisconnect(DisconnectType disconnectType);

        void OnFirstFrame();

        void OnConnect();

        Task<bool> RequestCameraPermissionAsync();
    }
}