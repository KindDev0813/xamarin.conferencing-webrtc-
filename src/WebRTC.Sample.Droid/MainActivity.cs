using System;
using System.Linq;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media.Projection;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Org.Webrtc;

namespace WebRTC.Sample.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private const int AllPermissionRequest = 0xff;
        private const int CAPTURE_PERMISSION_REQUEST_CODE = 1;

        private readonly string[] MANDATORY_PERMISSIONS =
        {
            "android.permission.MODIFY_AUDIO_SETTINGS",
            "android.permission.RECORD_AUDIO", "android.permission.INTERNET",
            Manifest.Permission.Camera
        };

        private bool _haveCapturePermission;
        private Intent _mediaProjectionPermissionResultData;
        private SurfaceViewRenderer _surfaceViewRenderer;

        private PeerConnectionClient _peerConnectionClient;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar =
                FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            _surfaceViewRenderer = FindViewById<SurfaceViewRenderer>(Resource.Id.fullscreen_video_view);

            CreatePeerConnectionClient();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            StartCall();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum]
            Android.Content.PM.Permission[] grantResults)
        {
            if (requestCode == AllPermissionRequest && grantResults.All(i => i == Permission.Granted))
            {
                CreatePeerConnectionClient();
                return;
            }

            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
        {
            if (requestCode == CAPTURE_PERMISSION_REQUEST_CODE)
            {
                _haveCapturePermission = resultCode == Result.Ok;
                _mediaProjectionPermissionResultData = data;
                if (_haveCapturePermission)
                    StartCall();
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }

        private void StartCall()
        {
            if (!_haveCapturePermission)
            {
                var mediaProjectionManager = (MediaProjectionManager) GetSystemService(MediaProjectionService);
                StartActivityForResult(mediaProjectionManager.CreateScreenCaptureIntent(),
                    CAPTURE_PERMISSION_REQUEST_CODE);
                return;
            }

            var videoCapturer = new ScreenCapturerAndroid(_mediaProjectionPermissionResultData, new MediaCallback());
            _peerConnectionClient.CreatePeerConnection(_surfaceViewRenderer, null, videoCapturer);
        }


        private bool HavePermission()
        {
            return MANDATORY_PERMISSIONS.All(permission =>
                CheckCallingOrSelfPermission(permission) == Permission.Granted);
        }

        private void CreatePeerConnectionClient()
        {
            if (!HavePermission())
            {
                RequestPermissions(MANDATORY_PERMISSIONS, AllPermissionRequest);
                return;
            }

            var eglBase = EglBase.Create();

            _surfaceViewRenderer.Init(eglBase.EglBaseContext, null);
            _surfaceViewRenderer.SetScalingType(RendererCommon.ScalingType.ScaleAspectFill);

            _surfaceViewRenderer.SetEnableHardwareScaler(false);

            _peerConnectionClient = new PeerConnectionClient(ApplicationContext, eglBase);
        }

        private class MediaCallback : MediaProjection.Callback
        {
            public override void OnStop()
            {
                base.OnStop();
            }
        }
    }
}