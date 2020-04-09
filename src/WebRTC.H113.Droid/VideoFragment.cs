using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Org.Webrtc;
using WebRTC.Droid;
using WebRTC.Droid.Extensions;

namespace WebRTC.H113.Droid
{
    public class VideoFragment : Fragment
    {
        private const string ConnectionParametersExtra = "connection_parameters_extra";
        private const string VideoConfigExtra = "video_config_extra";

        private VideoController _controller;
        private VideoConfig _videoConfig;
        private IVideoControllerReadyCallback _controllerReadyCallback;

        public VideoFragment()
        {
        }
        
        public static VideoFragment NewInstance(ConnectionParameters connectionParameters,
            VideoConfig videoConfig = null)
        {
            var videoFragment = new VideoFragment();
            var args = new Bundle();
            args.PutString(ConnectionParametersExtra, JsonConvert.SerializeObject(connectionParameters));
            args.PutString(VideoConfigExtra, JsonConvert.SerializeObject(videoConfig ?? new VideoConfig()));
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
            if (context is IVideoControllerReadyCallback videoControllerReadyCallback && _controllerReadyCallback == null)
            {
                GetVideoControllerAsync(videoControllerReadyCallback);
            }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            if (_controller != null)
                return;
            var connectionParameters = GetJson<ConnectionParameters>(Arguments, ConnectionParametersExtra);
            _videoConfig = GetJson<VideoConfig>(Arguments, VideoConfigExtra);
            _controller = new VideoController(connectionParameters, _videoConfig.UseFrontCamera);
            _controllerReadyCallback?.OnReadyViewController(_controller);
            Platform.EglFactory = _ => _controller.EglBase;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var frameLayout = new FrameLayout(Context);
            var surfaceViewRenderer = new SurfaceViewRenderer(Context);
            frameLayout.AddView(surfaceViewRenderer,
                new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent,
                    GravityFlags.Center));
            frameLayout.SetBackgroundColor(new Color(_videoConfig.BackgroundColor));
            surfaceViewRenderer.SetScalingType(_videoConfig.Scaling.ToNative());
            surfaceViewRenderer.SetEnableHardwareScaler(false);
            _controller.OnViewCreated(surfaceViewRenderer);
            return frameLayout;
        }

        public override void OnDestroyView()
        {
            _controller.OnViewDestroyed();
            base.OnDestroyView();
        }

        public override void OnDestroy()
        {
            _controller.Disconnect();
            Platform.EglFactory = null;
            _controller.EglBase.Release();
            base.OnDestroy();
        }

        private static T GetJson<T>(Bundle bundle, string key)
        {
            var json = bundle.GetString(key);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}