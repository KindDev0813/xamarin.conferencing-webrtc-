using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Org.Webrtc;

namespace WebRTC.H113.Droid
{
    public class VideoFragment : Fragment
    {
        private const string ConnectionParametersExtra = "connection_parameters_extra";
        private const string UseFrontCameraExtra = "use_front_camera_extra";

        private VideoController _controller;
        private IVideoControllerReadyCallback _controllerReadyCallback;

        private VideoFragment()
        {
        }

        public static VideoFragment NewInstance(ConnectionParameters connectionParameters, bool useFrontCamera = true)
        {
            var videoFragment = new VideoFragment();
            var args = new Bundle();
            args.PutString(ConnectionParametersExtra, JsonConvert.SerializeObject(connectionParameters));
            args.PutBoolean(UseFrontCameraExtra, useFrontCamera);
            videoFragment.Arguments = args;
            return videoFragment;
        }

        public void GetVideoControllerAsync(IVideoControllerReadyCallback videoControllerReadyCallback)
        {
            _controllerReadyCallback = videoControllerReadyCallback;
            if (_controller != null)
                _controllerReadyCallback?.OnReadyViewController(_controller);
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            H113Platform.Init(Activity);
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (_controller != null)
                return;
            var json = Arguments.GetString(ConnectionParametersExtra);
            var connectionParameters = JsonConvert.DeserializeObject<ConnectionParameters>(json);
            var useFrontCamera = Arguments.GetBoolean(UseFrontCameraExtra);
            _controller = new VideoController(connectionParameters, useFrontCamera);
            _controllerReadyCallback?.OnReadyViewController(_controller);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return _controller.OnCreateView(Context);
        }

        public override void OnDestroyView()
        {
            _controller.OnViewDestroyed();
            base.OnDestroyView();
        }
    }
}