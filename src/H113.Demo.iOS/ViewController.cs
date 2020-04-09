using CoreGraphics;
using Foundation;
using H113.Demo.iOS.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using UIKit;
using WebRTC.Abstraction;
using WebRTC.H113;
using WebRTC.H113.iOS;
using WebRTC.iOS;
using Xamarin.Essentials;

namespace H113.Demo.iOS
{
    public partial class ViewController : UIViewController, IVideoControllerListener
    {

        private const string Token =
          "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiaGplbHAxMTMiLCJ2ZXIiOjEsImlhdCI6MTU4NTMwMDYwNH0.LLXkD0PEOyXVb6wZEGWm2R9V8C0yG8Ihqn-rMsPgNVQ";

        private readonly ConnectionParameters _connectionParameters =
            new ConnectionParameters("wss://video.h113.no/ws", Token, "98056391", 23, 23);

        private VideoViewController _videoViewController;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public void OnConnect()
        {
            StartCallButton.SetTitle("Disconnect", UIControlState.Normal);
        }

        public void OnDisconnect(WebRTC.AppRTC.Abstraction.DisconnectType disconnectType)
        {
            VideoContainer.Hidden = true;
            StartCallButton.SetTitle("Start Call", UIControlState.Normal);
        }

        public void OnError(string message)
        {
            var alertDialog = UIAlertController.Create("Error", message, UIAlertControllerStyle.Alert);
            alertDialog.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Destructive, (s) => _videoViewController.Disconnect()));
            PresentViewController(alertDialog, true, null);
        }

        public void OnFirstFrame()
        {
            VideoContainer.Hidden = false;
        }

        public async Task<bool> RequestCameraPermissionAsync()
        {
            var permission = await Xamarin.Essentials.Permissions.RequestAsync<Permissions.Camera>();
            return permission == PermissionStatus.Granted;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _videoViewController = new VideoViewController();
            _videoViewController.MoveTo(this, VideoControllerContainer);

            if (ObjCRuntime.Runtime.Arch == ObjCRuntime.Arch.SIMULATOR)
            {
                _videoViewController.FileVideoCapturerFactory = videoSource => new FileVideoCapturer(videoSource, "SampleVideo_1280x720_10mb.mp4");
            }

            CameraButton.TouchUpInside += (s, e) =>
            {
                var alertDialog = UIAlertController.Create("Pick scale", _videoViewController.ScalingType.ToString(), UIAlertControllerStyle.ActionSheet);
                foreach (var scaleType in Enum.GetValues(typeof(ScalingType)).Cast<ScalingType>())
                {
                    alertDialog.AddAction(UIAlertAction.Create(scaleType.ToString(), UIAlertActionStyle.Default, (_) =>
                    {
                        _videoViewController.ScalingType = scaleType;
                    }));

                }

                PresentViewController(alertDialog, true, null);
            };

            MicButton.TouchUpInside += (s, e) => _videoViewController.ToggleAudio();

            StartCallButton.TouchUpInside += (s, e) =>
            {
                if (_videoViewController.IsConnected)
                    _videoViewController.Disconnect();
                else
                    _videoViewController.Start(this, _connectionParameters);
            };
        }

    }
}