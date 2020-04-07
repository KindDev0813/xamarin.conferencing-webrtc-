using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using WebRTC.AppRTC.Abstraction;
using WebRTC.H113;
using WebRTC.H113.Droid;
using Xamarin.Essentials;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace H113.Demo.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true,
        ScreenOrientation = ScreenOrientation.Sensor)]
    public class MainActivity : AppCompatActivity, IVideoControllerReadyCallback, IVideoControllerListener
    {
        private const string Token =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiaGplbHAxMTMiLCJ2ZXIiOjEsImlhdCI6MTU4NTMwMDYwNH0.LLXkD0PEOyXVb6wZEGWm2R9V8C0yG8Ihqn-rMsPgNVQ";

        private readonly ConnectionParameters _connectionParameters =
            new ConnectionParameters("wss://video.h113.no/ws", Token, "98056391", 23, 23);

        private VideoController _videoController;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource

            Xamarin.Essentials.Platform.Init(this,
                savedInstanceState); // add this line to your code, it may also be called: bundle

            SetContentView(Resource.Layout.activity_main);

            var fragment = VideoFragment.NewInstance(_connectionParameters, true);
            fragment.GetVideoControllerAsync(this);

            HideVideoContainer(false);
            
            SupportFragmentManager.BeginTransaction()
                .Add(fragment, "video_fragment")
                .CommitNow();
            

            var switchCamera = FindViewById<ImageButton>(Resource.Id.btn_switch_camera);
            switchCamera.Click += (sender, args) => _videoController?.SwitchCamera();

            var mute = FindViewById<ImageButton>(Resource.Id.btn_mute);
            mute.Click += (sender, args) => _videoController.ToggleAudio();

            var startCall = FindViewById<ImageButton>(Resource.Id.btn_start_call);
            startCall.Click += (sender, args) =>
            {
                if (_videoController.IsConnected)
                {
                    _videoController.Disconnect();
                    
                }
                else
                {
                    _videoController.Start(this);
                }
            };
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnReadyViewController(VideoController videoController)
        {
            _videoController = videoController;
        }

        public void OnError(string message)
        {
            if (_videoController == null)
                return;
            new AlertDialog.Builder(this)
                .SetTitle("Error")
                .SetMessage(message)
                .SetCancelable(false)
                .SetNeutralButton("OK", ((sender, args) =>
                {
                    var dialog = (AlertDialog) sender;
                    dialog.Cancel();
                    _videoController.Disconnect();
                }))
                .Create()
                .Show();
        }

        public void OnFirstFrame()
        {
            HideVideoContainer(false);
        }

        public void OnConnect()
        {
            
        }

        public async Task<bool> RequestCameraPermissionAsync()
        {
            var permission = await Xamarin.Essentials.Permissions.RequestAsync<Permissions.Camera>();
            return permission == PermissionStatus.Granted;
        }

        public void OnDisconnect(DisconnectType disconnectType)
        {
            Console.WriteLine(disconnectType);
            _videoController?.Disconnect();
        }

        private void HideVideoContainer(bool hide)
        {
            var container = FindViewById(Resource.Id.fl_video_container);
            container.Visibility = hide ? ViewStates.Gone : ViewStates.Visible;
        }
    }
}