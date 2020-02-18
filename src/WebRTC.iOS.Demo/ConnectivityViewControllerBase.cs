using System;
using AVFoundation;
using Foundation;
using UIKit;
using WebRTC.iOS.Binding;

namespace WebRTC.iOS.Demo
{
    public abstract class ConnectivityViewControllerBase : UIViewController, IRTCAudioSessionDelegate, IARDVideoCallViewControllerDelegate
    {

        protected ConnectivityViewControllerBase()
        {

        }

        protected ConnectivityViewControllerBase(IntPtr handle) : base(handle)
        {

        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var webRTCConfig = new RTCAudioSessionConfiguration();

            webRTCConfig.CategoryOptions |= AVAudioSessionCategoryOptions.DefaultToSpeaker;
            RTCAudioSessionConfiguration.SetWebRTCConfiguration(webRTCConfig);

            var session = RTCAudioSession.SharedInstance;
            session.AddDelegate(this);

            ConfigureAudioSession();
        }



        public void DidFinish(CallViewControllerBase viewController)
        {
            if (!viewController.IsBeingDismissed)
            {
                Console.WriteLine("Dismissing VC");
                viewController.DismissViewController(true, OnDismissVideoController);
            }
            var session = RTCAudioSession.SharedInstance;
            session.IsAudioEnabled = false;
        }

        protected virtual void FinishConfigureAudioSessions()
        {

        }

        protected virtual void OnDismissVideoController()
        {

        }

        protected virtual void AudioSessionDidStartPlayOrRecord(RTCAudioSession session)
        {
            Console.WriteLine("Setting isAudioEnabled to YES.");
            session.IsAudioEnabled = true;
        }

        protected virtual void AudioSessionDidStopPlayOrRecord(RTCAudioSession session)
        {
            Console.WriteLine("audioSessionDidStopPlayOrRecord");
            ConfigureAudioSession();
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
            FinishConfigureAudioSessions();
        }


        [Export("audioSessionDidStartPlayOrRecord:")]
        private void AudioSessionDidStartPlayOrRecordInternal(RTCAudioSession session)
        {
            // Stop playback on main queue and then configure WebRTC.
            RTCDispatcher.DispatchAsyncOnType(RTCDispatcherQueueType.Main, () =>
            {
                Console.WriteLine("Setting isAudioEnabled to YES.");
                session.IsAudioEnabled = true;
            });
        }

        [Export("audioSessionDidStopPlayOrRecord:")]
        private void AudioSessionDidStopPlayOrRecordInternal(RTCAudioSession session)
        {
            // WebRTC is done with the audio session. Restart playback.
            RTCDispatcher.DispatchAsyncOnType(RTCDispatcherQueueType.Main, () =>
            {
                AudioSessionDidStopPlayOrRecord(session);
            });
        }

    }
}
