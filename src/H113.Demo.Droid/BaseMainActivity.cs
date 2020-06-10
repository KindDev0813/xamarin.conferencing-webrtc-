using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using DemoConfig;
using WebRTC.H113;
using WebRTC.H113.Droid;
using Xamarin.Essentials;

namespace H113.Demo.Droid
{
    public class BaseMainActivity : AppCompatActivity, IVideoControllerReadyCallback, IVideoControllerListener
    {
        private readonly ConnectionParameters _connectionParameters =
            new ConnectionParameters(Config.WssUrl, Config.Token, Config.Phone);

        private VideoController _videoController;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource

            Xamarin.Essentials.Platform.Init(this,
                savedInstanceState); // add this line to your code, it may also be called: bundle

            SetContentView(Resource.Layout.activity_main);

            if (SupportFragmentManager.FindFragmentByTag("video_fragment") == null)
            {
                var fragment = VideoFragment.NewInstance(_connectionParameters);
                fragment.GetVideoControllerAsync(this);

                SupportFragmentManager.BeginTransaction()
                    .Add(Resource.Id.video_controller_container, fragment, "video_fragment")
                    .CommitAllowingStateLoss();
            }

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


        public virtual void OnFirstFrame()
        {
            HideVideoContainer(false);
        }

        public virtual void OnConnect()
        {
            Toast.MakeText(this, "Connected", ToastLength.Short).Show();
        }

        public async Task<bool> RequestCameraPermissionAsync()
        {
            var permission = await Xamarin.Essentials.Permissions.RequestAsync<Permissions.Camera>();
            return permission == PermissionStatus.Granted;
        }

        public virtual void OnDisconnect(DisconnectType disconnectType)
        {
            Toast.MakeText(this, $"Disconnect {disconnectType.ToString()}", ToastLength.Short).Show();
            _videoController?.Disconnect();
            HideVideoContainer(true);
        }

        protected void HideVideoContainer(bool hide)
        {
            var container = FindViewById(Resource.Id.fl_video_container);
            container.Visibility = hide ? ViewStates.Gone : ViewStates.Visible;
        }

        public void ShowNotification(int type, string title, string message)
        {
        }
    }
}