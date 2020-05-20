using System;
using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using UIKit;
using WebRTC.Abstraction;
using WebRTC.AppRTC.Abstraction;
using WebRTC.iOS;
using WebRTC.iOS.Binding;
using Xamarin.Essentials;

namespace WebRTC.H113.iOS
{
    public partial class VideoViewController : UIViewController, IAppRTCEngineEvents
    {
        private readonly VideoRendererProxy _localRenderer = new VideoRendererProxy();

        private readonly H113Controller _controller;

        private IVideoControllerListener _videoControllerListener;

        private VideoView _videoView;


        public VideoViewController() : base()
        {
            H113Platform.Init();
            _controller = new H113Controller(this);
        }

        public Func<IVideoSource, FileVideoCapturer> FileVideoCapturerFactory { get; set; }

        public bool UseFrontCamera { get; set; } = true;

        public bool IsConnected => _controller.IsWebRTCConnected;

        public ScalingType ScalingType
        {
            get => _videoView.ScalingType;
            set => _videoView.ScalingType = value;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                H113Platform.Cleanup();
            }
            base.Dispose(disposing);
        }


        public void Start(IVideoControllerListener videoControllerListener, ConnectionParameters connectionParameters)
        {
            _videoControllerListener = null;
            _controller.Disconnect();
            _videoControllerListener = videoControllerListener;
            _controller.Connect(connectionParameters);
            _localRenderer.OnFirstFrame = () =>
            {
                DispatchQueue.MainQueue.DispatchAsync(_videoControllerListener.OnFirstFrame);
            };
        }

        public void SwitchCamera()
        {
            _controller.SwitchCamera();
        }

        public void ToggleAudio()
        {
            _controller.SetAudioEnabled(!_controller.IsAudioEnable);
        }

        public void ToggleVideo()
        {
            _controller.SetVideoEnabled(!_controller.IsVideoEnable);
        }

        public void Disconnect()
        {
            _controller.Disconnect();
        }


        public override void LoadView()
        {
            base.LoadView();
            _videoView = new VideoView();
            _localRenderer.Renderer = _videoView.VideoRenderer;
            View = _videoView;
        }


        void IAppRTCEngineEvents.OnPeerFactoryCreated(IPeerConnectionFactory factory)
        {

        }

        void IAppRTCEngineEvents.OnDisconnect(DisconnectType disconnectType)
        {
            _videoControllerListener.OnDisconnect(disconnectType);
        }

        IVideoCapturer IAppRTCEngineEvents.CreateVideoCapturer(IPeerConnectionFactory factory, IVideoSource videoSource)
        {
            if (FileVideoCapturerFactory != null)
                return FileVideoCapturerFactory(videoSource);
            return factory.CreateCameraCapturer(videoSource, UseFrontCamera);
        }

        async void IAppRTCEngineEvents.ReadyToStart()
        {
            var isAllowed = await _videoControllerListener.RequestCameraPermissionAsync();
            if (isAllowed)
                _controller.StartVideoCall(_localRenderer, null);
        }

        void IAppRTCEngineEvents.OnError(string description)
        {
            _videoControllerListener.OnError(description);
        }

        void IAppRTCEngineEvents.OnConnect(DisconnectType disconnectType)
        {
            _videoControllerListener.OnConnect(disconnectType);
        }

        public void ShowNotification(int type, string title, string message)
        {
            _videoControllerListener.ShowNotification(type, title, message);
        }

        private class VideoView : UIView, IRTCVideoViewDelegate
        {
            private static float BalancedVisibleFraction = 0.56f;


            private readonly RTCEAGLVideoView _localVideoView;

            private CGSize _localVideoSize;

            private ScalingType _scalingType = ScalingType.AspectFit;

            public VideoView()
            {
                _localVideoView = new RTCEAGLVideoView
                {
                    Delegate = this
                };
                BackgroundColor = UIColor.Black;
                AddSubview(_localVideoView);
            }

            public IRTCVideoRenderer VideoRenderer => _localVideoView;

            public ScalingType ScalingType
            {
                get => _scalingType;
                set
                {
                    _scalingType = value;
                    SetNeedsLayout();
                }
            }


            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                var bounds = Bounds;

                if (_localVideoSize.Width > 0 && _localVideoSize.Height > 0)
                {
                    var videoAspectRatio = _localVideoSize.Width / _localVideoSize.Height;

                    var size = GetDisplaySize(ConvertScalingTypeToVisibleFraction(ScalingType), videoAspectRatio, bounds.Width, bounds.Height);
                    _localVideoView.Frame = new CGRect(CGPoint.Empty, size);
                    _localVideoView.Center = new CGPoint(bounds.GetMidX(), bounds.GetMidY());
                }
                else
                {
                    _localVideoView.Frame = bounds;
                }
            }

            [Export("videoView:didChangeVideoSize:")]
            private void DidChangeVideoSize(IRTCVideoRenderer videoView, CGSize size)
            {
                _localVideoSize = size;
                SetNeedsLayout();
            }

            private static nfloat ConvertScalingTypeToVisibleFraction(ScalingType scalingType)
            {
                switch (scalingType)
                {
                    case ScalingType.AspectFill:
                        return 0f;
                    case ScalingType.AspectFit:
                        return 1f;
                    case ScalingType.AspectBalanced:
                        return BalancedVisibleFraction;
                    default:
                        throw new NotImplementedException();
                }
            }

            private static CGSize GetDisplaySize(nfloat minVisibleFraction, nfloat videoAspectRatio, nfloat maxDisplayWidth, nfloat maxDisplayHeight)
            {
                // If there is no constraint on the amount of cropping, fill the allowed display area.
                if (minVisibleFraction == 0 || videoAspectRatio == 0)
                    return new CGSize(maxDisplayWidth, maxDisplayHeight);
                // Each dimension is constrained on max display size and how much we are allowed to crop.
                var width = Math.Min(maxDisplayWidth,
                    (int)(maxDisplayHeight / minVisibleFraction * videoAspectRatio));
                var height = Math.Min(maxDisplayHeight,
                    (int)(maxDisplayWidth / minVisibleFraction / videoAspectRatio));
                return new CGSize(width, height);
            }

        }
    }
}

