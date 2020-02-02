using System;
using AVFoundation;
using CoreGraphics;
using Foundation;
using UIKit;
using WebRTC.AppRTC;
using WebRTC.iOS.Binding;

namespace WebRTC.iOS.Demo
{
    public class ConnectivityViewController : UIViewController, IARDMainViewDelegate, IRTCAudioSessionDelegate, IARDVideoCallViewControllerDelegate
    {
        private const string barButtonImageString = @"ic_settings_black_24dp.png";
        private const string loopbackLaunchProcessArgument = @"loopback";

        private ARDMainView _mainView;
        private AVAudioPlayer _audioPlayer;

        public override void LoadView()
        {
            base.LoadView();
            Title = "AppRTC Mobile";
            _mainView = new ARDMainView(CGRect.Empty);
            _mainView.Delegate = this;
            _mainView.BackgroundColor = UIColor.White;

            View = _mainView;

            AddSettingsBarButton();

            var webRTCConfig = new RTCAudioSessionConfiguration();

            webRTCConfig.CategoryOptions |= AVAudioSessionCategoryOptions.DefaultToSpeaker;
            RTCAudioSessionConfiguration.SetWebRTCConfiguration(webRTCConfig);

            var session = RTCAudioSession.SharedInstance;
            session.AddDelegate(this);

            ConfigureAudioSession();
            SetupAudioPlayer();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            _mainView.RoomId = GenerateRoom.GenerateRoomName();
        }

        [Export("audioSessionDidStartPlayOrRecord:")]
        public void AudioSessionDidStartPlayOrRecord(RTCAudioSession session)
        {
            // Stop playback on main queue and then configure WebRTC.
            RTCDispatcher.DispatchAsyncOnType(RTCDispatcherQueueType.Main, () =>
            {
                if (_mainView.IsAudioLoopPlaying)
                {
                    Console.WriteLine("Stopping audio loop due to WebRTC start.");
                    _audioPlayer.Stop();
                }

                Console.WriteLine("Setting isAudioEnabled to YES.");
                session.IsAudioEnabled = true;
            });
        }

        [Export("audioSessionDidStopPlayOrRecord:")]
        public void AudioSessionDidStopPlayOrRecord(RTCAudioSession session)
        {
            // WebRTC is done with the audio session. Restart playback.
            RTCDispatcher.DispatchAsyncOnType(RTCDispatcherQueueType.Main, () =>
            {
                Console.WriteLine("audioSessionDidStopPlayOrRecord");
                RestartAudioPlayerIfNeeded();
            });
        }

        private void AddSettingsBarButton()
        {
            var settingButton = new UIBarButtonItem(new UIImage(barButtonImageString), UIBarButtonItemStyle.Plain, ShowSettings);
            NavigationItem.RightBarButtonItem = settingButton;
        }

        public void DidFinish(CallViewControllerBase viewController)
        {
            if (!viewController.IsBeingDismissed)
            {
                Console.WriteLine("Dismissing VC");
                viewController.DismissViewController(true, RestartAudioPlayerIfNeeded);
            }
            var session = RTCAudioSession.SharedInstance;
            session.IsAudioEnabled = false;
        }

        public void OnStartCall(ARDMainView mainView, string room, bool isLoopback)
        {

            if (string.IsNullOrWhiteSpace(room))
            {
                if (isLoopback)
                {
                    room = GenerateRoom.GenerateRoomName();
                }
                else
                {
                    ShowAlertWithMessage("Missing room name.");
                    return;
                }
            }

            room = room.Trim();


            // var settingsModel = new ARDSettingsModel();
            var session = RTCAudioSession.SharedInstance;

            // session.UseManualAudio = settingsModel.CurrentUseManualAudioConfigSettingFromStore;
            session.IsAudioEnabled = false;

            var videoCallViewController = new AppRTCCallViewController(room, isLoopback)
            {
                Delegate = this
            };

            // var videoCallViewController = new ARDVideoCallViewController(room, isLoopback, this);
            videoCallViewController.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;

            PresentViewController(videoCallViewController, true, null);
        }

        public void DidToggleAudioLoop(ARDMainView mainView)
        {
            if (mainView.IsAudioLoopPlaying)
            {
                _audioPlayer.Stop();
            }
            else
            {
                _audioPlayer.Play();
            }

            mainView.IsAudioLoopPlaying = _audioPlayer.Playing;
        }



        private void ConfigureAudioSession()
        {
            var configuration = new RTCAudioSessionConfiguration();
            configuration.Category = AVAudioSession.CategoryAmbient;
            configuration.CategoryOptions = AVAudioSessionCategoryOptions.DuckOthers;
            configuration.Mode = AVAudioSession.ModeDefault;

            var session = RTCAudioSession.SharedInstance;
            session.LockForConfiguration();

            bool hasSucceeded;
            NSError error;

            if (session.IsActive)
            {
                hasSucceeded = session.SetConfiguration(configuration, out error);
            }
            else
            {
                hasSucceeded = session.SetConfiguration(configuration, true, out error);
            }

            if (!hasSucceeded)
            {
                Console.WriteLine("Error setting configuration:{0}", error.LocalizedDescription);
            }

            session.UnlockForConfiguration();
        }

        private void SetupAudioPlayer()
        {
            var audioFilePath = NSBundle.MainBundle.PathForResource("mozart", "mp3");
            var audioFileUrl = new NSUrl(audioFilePath);
            _audioPlayer = new AVAudioPlayer(audioFileUrl, "mozart", out _);
            _audioPlayer.NumberOfLoops = -1;
            _audioPlayer.Volume = 1;
            _audioPlayer.PrepareToPlay();
        }

        private void RestartAudioPlayerIfNeeded()
        {
            ConfigureAudioSession();
            if (_mainView.IsAudioLoopPlaying && PresentedViewController != null)
            {
                Console.WriteLine("Starting audio loop due to WebRTC end.");
                _audioPlayer.Play();
            }
        }

        private void ShowSettings(object sender, EventArgs e)
        {
            //var settingsController = new ARDSettingsViewController(UITableViewStyle.Grouped, new ARDSettingsModel());

            //var navigationController = new UINavigationController(settingsController);

            //PresentViewController(navigationController, true, null);
        }

        private void ShowAlertWithMessage(string message)
        {
            var alert = UIAlertController.Create("", message, UIAlertControllerStyle.Alert);
            var defaultAction = UIAlertAction.Create("OK", UIAlertActionStyle.Default, (_) => { });

            alert.AddAction(defaultAction);

            PresentViewController(alert, true, null);
        }

    }
}
