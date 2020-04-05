using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Newtonsoft.Json;

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
            RetainInstance = true;
            if (_controller != null)
                return;
            var json = Arguments.GetString(ConnectionParametersExtra);
            var connectionParameters = JsonConvert.DeserializeObject<ConnectionParameters>(json);
            var useFrontCamera = Arguments.GetBoolean(UseFrontCameraExtra);
            _controller = new VideoController(Context, connectionParameters, useFrontCamera);
            _controllerReadyCallback?.OnReadyViewController(_controller);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return _controller.FullScreenRenderer;
        }

        public override void OnDestroyView()
        {
            if (_controller.FullScreenRenderer.Parent is ViewGroup viewGroup)
            {
                viewGroup.RemoveView(_controller.FullScreenRenderer);
            }

            base.OnDestroyView();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _controller.FullScreenRenderer.Release();
            _controller.Disconnect();
        }
    }
}