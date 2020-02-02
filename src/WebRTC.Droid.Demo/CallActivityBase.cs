using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Org.Webrtc;
using WebRTC.Abstraction;
using WebRTC.AppRTC.Abstraction;
using WebRTC.Droid.Extensions;
using Xamarin.Essentials;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using IVideoCapturer = WebRTC.Abstraction.IVideoCapturer;


namespace WebRTC.Droid.Demo
{
    public abstract class CallActivityBase<TConnectionParam, TSignalParam,TController> : BaseActivity, IAppRTCEngineEvents, CallFragment.IOnCallEvents
        where TSignalParam : ISignalingParameters
        where TConnectionParam : IConnectionParameters
        where TController : AppRTCControllerBase<TConnectionParam,TSignalParam>
    {
        private CallFragment _callFragment;

        private SurfaceViewRenderer _fullscreenRenderer;
        private SurfaceViewRenderer _pipRenderer;

        private VideoRendererProxy _localRenderer;
        private VideoRendererProxy _remoteRenderer;

        private TController _client;
        private bool _isSwappedFeed;
        private bool _callControlFragmentVisible = true;
        private bool _micEnabled = true;

        protected abstract TController CreateEngine();

        protected abstract void Connect(TController rtcEngine, Intent intent);

        protected abstract CallFragment CreateCallFragment(Intent intent);

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestWindowFeature(WindowFeatures.NoTitle);

            Window.AddFlags(WindowManagerFlags.Fullscreen | WindowManagerFlags.KeepScreenOn |
                            WindowManagerFlags.ShowWhenLocked | WindowManagerFlags.TurnScreenOn);

            Window.DecorView.SystemUiVisibility = GetSystemUiVisibility();

            SetContentView(Resource.Layout.activity_call);

            _fullscreenRenderer = FindViewById<SurfaceViewRenderer>(Resource.Id.fullscreen_video_view);
            _pipRenderer = FindViewById<SurfaceViewRenderer>(Resource.Id.pip_video_view);

            _callFragment = CreateCallFragment(Intent);


            _localRenderer = new VideoRendererProxy();
            _remoteRenderer = new VideoRendererProxy();

            _pipRenderer.Click += (sender, args) => SetSwappedFeeds(!_isSwappedFeed);
            _pipRenderer.SetScalingType(RendererCommon.ScalingType.ScaleAspectFit);
            _pipRenderer.SetZOrderMediaOverlay(true);
            _pipRenderer.SetEnableHardwareScaler(true);

            _fullscreenRenderer.Click += (sender, args) => ToggleCallControlFragmentVisibility();
            _fullscreenRenderer.SetScalingType(RendererCommon.ScalingType.ScaleAspectFill);
            _fullscreenRenderer.SetEnableHardwareScaler(false);


            SetSwappedFeeds(true);

            var ft = SupportFragmentManager.BeginTransaction();
            ft.Add(Resource.Id.call_fragment_container, _callFragment);
            ft.Commit();


            _client = CreateEngine();
            Connect(_client, Intent);
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            Disconnect();
        }


        public void OnPeerFactoryCreated(IPeerConnectionFactory factory)
        {
            var androidFactory = (IPeerConnectionFactoryAndroid) factory.NativeObject;
            _pipRenderer.Init(androidFactory.EglBaseContext, null);
            _fullscreenRenderer.Init(androidFactory.EglBaseContext, null);
        }

        public void OnDisconnect(DisconnectType disconnectType)
        {
            Disconnect();
        }

        public IVideoCapturer CreateVideoCapturer(IPeerConnectionFactory factory, IVideoSource videoSource) =>
            factory.CreateCameraCapturer(videoSource, true);

        public async void ReadyToStart()
        {
            var permission = await Xamarin.Essentials.Permissions.RequestAsync<Permissions.Camera>();
            if (permission == PermissionStatus.Granted)
            {
                _client.StartVideoCall(_localRenderer, _remoteRenderer);
            }
            else
            {
                Toast.MakeText(this, "No video permission.", ToastLength.Long).Show();
            }
        }


        public void OnError(string description)
        {
            if (_client == null)
                return;
            new AlertDialog.Builder(this)
                .SetTitle("Error")
                .SetMessage(description)
                .SetCancelable(false)
                .SetNeutralButton("OK", ((sender, args) =>
                {
                    var dialog = (AlertDialog) sender;
                    dialog.Cancel();
                    Disconnect();
                }))
                .Create()
                .Show();
        }

        private void Disconnect()
        {
            _client?.Disconnect();
            _client = null;
            _pipRenderer?.Release();
            _pipRenderer = null;
            _fullscreenRenderer?.Release();
            _fullscreenRenderer = null;

            Finish();
        }

        public void OnCallHangUp()
        {
            Disconnect();
        }

        public void OnCameraSwitch()
        {
            _client?.SwitchCamera();
        }

        public void OnVideoScalingSwitch(ScalingType scalingType)
        {
            _fullscreenRenderer.SetScalingType(scalingType.ToNative());
        }

        public void OnCaptureFormatChange(int width, int height, int framerate)
        {
            _client?.ChangeCaptureFormat(width, height, framerate);
        }

        public bool OnToggleMic()
        {
            if (_client == null)
                return _micEnabled;
            _micEnabled = !_micEnabled;
            _client.SetAudioEnabled(_micEnabled);
            return _micEnabled;
        }


        private void SetSwappedFeeds(bool isSwappedFeed)
        {
            _isSwappedFeed = isSwappedFeed;
            _localRenderer.Renderer = _isSwappedFeed ? _fullscreenRenderer : _pipRenderer;
            _remoteRenderer.Renderer = _isSwappedFeed ? _pipRenderer : _fullscreenRenderer;
            _fullscreenRenderer.SetMirror(isSwappedFeed);
            _pipRenderer.SetMirror(!isSwappedFeed);
        }

        private void ToggleCallControlFragmentVisibility()
        {
            if (!_client.Connected || !_callFragment.IsAdded)
            {
                return;
            }

            _callControlFragmentVisible = !_callControlFragmentVisible;

            var ft = SupportFragmentManager.BeginTransaction();
            if (_callControlFragmentVisible)
            {
                ft.Show(_callFragment);
            }
            else
            {
                ft.Hide(_callFragment);
            }

            ft.SetTransition(FragmentTransaction.TransitFragmentFade);
            ft.Commit();
        }

        private StatusBarVisibility GetSystemUiVisibility()
        {
            var flags = SystemUiFlags.HideNavigation | SystemUiFlags.Fullscreen;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
                flags = SystemUiFlags.ImmersiveSticky;
            return (StatusBarVisibility) flags;
        }
    }
}