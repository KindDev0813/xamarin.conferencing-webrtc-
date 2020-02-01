using System;
using CoreGraphics;
using UIKit;
using WebRTC.Abstraction;
using WebRTC.AppRTC.Abstraction;
using Xamarin.Essentials;

namespace WebRTC.iOS.Demo
{
    public abstract class CallViewControllerBase<T> : UIViewController, IARDVideoCallViewDelegate, IAppRTCEngineEvents
        where T : AppRTCEngineBase
    {

        private ARDVideoCallView _videoCallView;

        private VideoRendererProxy _localRenderer;
        private VideoRendererProxy _remoteRenderer;

        private ARDFileCaptureController _fileCaptureController;

        private T _rtcEngine;


        protected abstract T CreateEngine();

        protected abstract void Connect(T rtcEngine);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _localRenderer = new VideoRendererProxy();
            _remoteRenderer = new VideoRendererProxy();

            _localRenderer.Renderer = _videoCallView.RemoteVideoRender;
            _remoteRenderer.Renderer = _videoCallView.RemoteVideoRender;

            _rtcEngine = CreateEngine();

            Connect(_rtcEngine);

          
        }

        public override void LoadView()
        {
            base.LoadView();

            _videoCallView = new ARDVideoCallView(CGRect.Empty);
            _videoCallView.Delegate = this;

            View = _videoCallView;
        }



        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            return UIInterfaceOrientationMask.All;
        }


        public void OnPeerFactoryCreated(IPeerConnectionFactory factory)
        {
        }

        public void OnDisconnect(DisconnectType disconnectType)
        {

        }

        public IVideoCapturer CreateVideoCapturer(IPeerConnectionFactory factory, IVideoSource videoSource)
        {
            if (ObjCRuntime.Runtime.Arch != ObjCRuntime.Arch.SIMULATOR)
            {
                return factory.CreateCameraCapturer(videoSource, true);
            }

            var fileCapturer = factory.CreateFileCapturer(videoSource, "");

            _fileCaptureController = new ARDFileCaptureController((IFileVideoCaptureriOS)fileCapturer);
            return fileCapturer;

        }


        public async void ReadyToStart()
        {
            var permission = await Xamarin.Essentials.Permissions.RequestAsync<Permissions.Camera>();
            if (permission == PermissionStatus.Granted)
            {
                _rtcEngine.StartVideoCall(_localRenderer, _remoteRenderer);
            }
            else
            {

            }
        }

        public void OnError(string description)
        {
            var alertDialog = UIAlertController.Create("Error", description, UIAlertControllerStyle.Alert);
            alertDialog.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Destructive, (s) => Disconnect()));
        }


        public void DidSwitchCamera(ARDVideoCallView view)
        {
            if(_fileCaptureController != null)
            {
                _fileCaptureController.Toggle();
                return;
            }
            _rtcEngine.SwitchCamera();
        }

        public void DidChangeRoute(ARDVideoCallView view)
        {

        }

        public void DidHangup(ARDVideoCallView view)
        {
            Disconnect();
        }

        public void DidEnableStats(ARDVideoCallView view)
        {

        }


        private void Disconnect()
        {           
            _rtcEngine.Disconnect();
        }
    }
}
