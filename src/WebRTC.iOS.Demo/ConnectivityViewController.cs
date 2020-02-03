using System;
using AVFoundation;
using CoreGraphics;
using Foundation;
using UIKit;
using WebRTC.AppRTC;
using WebRTC.iOS.Binding;

namespace WebRTC.iOS.Demo
{
    public class ConnectivityViewController : ConnectivityViewControllerBase, IARDMainViewDelegate
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

            ConfigureAudioSession();
            SetupAudioPlayer();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            _mainView.RoomId = GenerateRoom.GenerateRoomName();
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
            videoCallViewController.ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;

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

        protected override void OnDismissVideoController()
        {
            base.OnDismissVideoController();
            RestartAudioPlayerIfNeeded();
        }

        protected override void AudioSessionDidStartPlayOrRecord(RTCAudioSession session)
        {
            if (_mainView.IsAudioLoopPlaying)
            {
                Console.WriteLine("Stopping audio loop due to WebRTC start.");
                _audioPlayer.Stop();
            }
            base.AudioSessionDidStartPlayOrRecord(session);
        }

        protected override void AudioSessionDidStopPlayOrRecord(RTCAudioSession session)
        {
            base.AudioSessionDidStopPlayOrRecord(session);
            RestartAudioPlayerIfNeeded();
        }


        private void AddSettingsBarButton()
        {
            var settingButton = new UIBarButtonItem(new UIImage(barButtonImageString), UIBarButtonItemStyle.Plain, ShowSettings);
            NavigationItem.RightBarButtonItem = settingButton;
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
