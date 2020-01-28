using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Widget;
using Org.Webrtc;
using WebRTC.Abstraction;
using WebRTC.AppRTC;
using Xamarin.Essentials;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using IVideoCapturer = WebRTC.Abstraction.IVideoCapturer;
using IVideoSink = Org.Webrtc.IVideoSink;


namespace WebRTC.Droid.Demo
{
    [Activity]
    public class CallActivity : AppCompatActivity, IH113ClientEvents
    {
        private SurfaceViewRenderer _fullscreenRenderer;
        private SurfaceViewRenderer _pipRenderer;

        private VideoRendererProxy _localRenderer;
        private VideoRendererProxy _remoteRenderer;

        private H113Client _client;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.call_activity);

            _fullscreenRenderer = FindViewById<SurfaceViewRenderer>(Resource.Id.fullscreen_video_view);
            _pipRenderer = FindViewById<SurfaceViewRenderer>(Resource.Id.pip_video_view);
            
            _localRenderer = new VideoRendererProxy();
            _remoteRenderer = new VideoRendererProxy();

            _localRenderer.Renderer = _pipRenderer;
            _remoteRenderer.Renderer = _fullscreenRenderer;

            _client = new H113Client(this);
            _client.Connect(new ConnectionParameters(H113Constants.WssUrl, H113Constants.Token, "98056391", 54.23,
                12.12));
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            Disconnect();
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnPeerFactoryCreated(IPeerConnectionFactory factory)
        {
            var androidFactory = (IPeerConnectionFactoryAndroid) factory.NativeObject;
            _pipRenderer.Init(androidFactory.EglBaseContext, null);
            _fullscreenRenderer.Init(androidFactory.EglBaseContext, null);
        }

        public void OnDisconnect(DisconnectType disconnectType)
        {
            
        }

        public async void ReadyToCall()
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

        public IVideoCapturer CreateVideoCapturer(IPeerConnectionFactory factory, IVideoSource videoSource) =>
            factory.CreateCameraCapturer(videoSource, true);


        public void OnError(string description)
        {
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
        
    }
}