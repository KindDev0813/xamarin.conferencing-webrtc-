using System;
using AVFoundation;
using CoreGraphics;
using Foundation;
using UIKit;
using WebRTC.iOS.Binding;

namespace WebRTC.iOS.Demo
{
    public interface IARDVideoCallViewDelegate
    {
        void DidSwitchCamera(ARDVideoCallView view);
        void DidChangeRoute(ARDVideoCallView view);
        void DidHangup(ARDVideoCallView view);
        void DidEnableStats(ARDVideoCallView view);
    }

    public class ARDVideoCallView : UIView, IRTCVideoViewDelegate
    {
        const float kButtonPadding = 16;
        const float kButtonSize = 48;
        const float kLocalVideoViewSize = 120;
        const float kLocalVideoViewPadding = 8;
        const float kStatusBarHeight = 20;



        private readonly UIButton _routeChangeButton;
        private readonly UIButton _cameraSwitchButton;
        private readonly UIButton _hangupButton;
        private CGSize _remoteVideoSize;

        public ARDVideoCallView(CGRect frame, bool useCameraPreview) : base(frame)
        {
            RemoteView = new RTCEAGLVideoView
            {
                Delegate = this
            };

            AddSubview(RemoteView);

            if (useCameraPreview)
            {
                LocalVideoView = new RTCCameraPreviewView();
            }
            else
            {
                LocalVideoView = new RTCEAGLVideoView
                {
                    Delegate = this
                };
            }
            AddSubview(LocalVideoView);

            StatsView = new ARDStatsView(CGRect.Empty);
            StatsView.Hidden = true;
            AddSubview(StatsView);

            _routeChangeButton = new UIButton(UIButtonType.Custom);
            _routeChangeButton.BackgroundColor = UIColor.White;
            _routeChangeButton.Layer.CornerRadius = kButtonSize / 2;
            _routeChangeButton.Layer.MasksToBounds = true;
            _routeChangeButton.SetImage(UIImage.FromBundle("ic_surround_sound_black_24dp.png"), UIControlState.Normal);
            _routeChangeButton.TouchUpInside += OnRouteChange;
            AddSubview(_routeChangeButton);


            _cameraSwitchButton = new UIButton(UIButtonType.Custom);
            _cameraSwitchButton.BackgroundColor = UIColor.White;
            _cameraSwitchButton.Layer.CornerRadius = kButtonSize / 2;
            _cameraSwitchButton.Layer.MasksToBounds = true;
            _cameraSwitchButton.SetImage(UIImage.FromBundle("ic_switch_video_black_24dp.png"), UIControlState.Normal);
            _cameraSwitchButton.TouchUpInside += OnCameraSwitch;
            AddSubview(_cameraSwitchButton);

            _hangupButton = new UIButton(UIButtonType.Custom);
            _hangupButton.BackgroundColor = UIColor.White;
            _hangupButton.Layer.CornerRadius = kButtonSize / 2;
            _hangupButton.Layer.MasksToBounds = true;
            _hangupButton.SetImage(UIImage.FromBundle("ic_call_end_black_24dp.png").ChangeColor(UIColor.Red), UIControlState.Normal);
            _hangupButton.TouchUpInside += OnHangup;
            AddSubview(_hangupButton);

            StatusLabel = new UILabel();
            StatusLabel.Font = UIFont.SystemFontOfSize(16);
            StatusLabel.TextColor = UIColor.White;
            AddSubview(StatusLabel);

            var tapRecognizer = new UITapGestureRecognizer(DidTripleTap);
            tapRecognizer.NumberOfTapsRequired = 3;

            AddGestureRecognizer(tapRecognizer);
        }

        public IARDVideoCallViewDelegate Delegate { get; set; }
        public UILabel StatusLabel { get; }
        public UIView LocalVideoView { get; }
        public IRTCVideoRenderer RemoteVideoRender => RemoteView as IRTCVideoRenderer;
        public IRTCVideoRenderer LocalVideoRender => LocalVideoView as IRTCVideoRenderer;
        public UIView RemoteView { get; }
        public ARDStatsView StatsView { get; }


        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            var bounds = Bounds;

            if (_remoteVideoSize.Width > 0 && _remoteVideoSize.Height > 0)
            {
                // Aspect fill remote video into bounds.

                var remoteVideoFrame = bounds.WithAspectRatio(_remoteVideoSize);
                nfloat scale = 1f;
                if (remoteVideoFrame.Size.Width > remoteVideoFrame.Size.Height)
                {
                    // Scale by height.
                    scale = bounds.Size.Height / remoteVideoFrame.Size.Height;
                }
                else
                {
                    // Scale by width.
                    scale = bounds.Size.Width / remoteVideoFrame.Size.Width;
                }

                remoteVideoFrame.Size = new CGSize(remoteVideoFrame.Size.Width * scale, remoteVideoFrame.Size.Height * scale);
                RemoteView.Frame = remoteVideoFrame;
                RemoteView.Center = new CGPoint(bounds.GetMidX(), bounds.GetMidY());
            }
            else
            {
                RemoteView.Frame = bounds;
            }
            // Aspect fit local video view into a square box.
            var localVideoFrame = new CGRect(0, 0, kLocalVideoViewSize, kLocalVideoViewSize);
            // Place the view in the bottom right.
            localVideoFrame.Location = new CGPoint(
                bounds.GetMaxX() - localVideoFrame.Size.Width - kLocalVideoViewPadding, bounds.GetMaxY() - localVideoFrame.Size.Height - kLocalVideoViewPadding - AppDelegate.SafeAreaInsets.Top);

            LocalVideoView.Frame = localVideoFrame;

            // Place stats at the top.
            var statsSize = StatsView.SizeThatFits(bounds.Size);
            StatsView.Frame = new CGRect(bounds.GetMinX(), bounds.GetMinY() + kStatusBarHeight + AppDelegate.SafeAreaInsets.Top, statsSize.Width, statsSize.Height);

            // Place hangup button in the bottom left.
            _hangupButton.Frame = new CGRect(bounds.GetMinX() + kButtonPadding, bounds.GetMaxY() - kButtonPadding - kButtonSize - AppDelegate.SafeAreaInsets.Bottom, kButtonSize, kButtonSize);

            // Place button to the right of hangup button.
            var cameraSwitchFrame = _hangupButton.Frame;
            cameraSwitchFrame.Location = new CGPoint(cameraSwitchFrame.GetMaxX() + kButtonPadding, cameraSwitchFrame.Location.Y);
            _cameraSwitchButton.Frame = cameraSwitchFrame;

            // Place route button to the right of camera button.
            var routeChangeFrame = _cameraSwitchButton.Frame;
            routeChangeFrame.Location = new CGPoint(routeChangeFrame.GetMaxX() + kButtonPadding, routeChangeFrame.Location.Y);
            _routeChangeButton.Frame = routeChangeFrame;

            StatusLabel.SizeToFit();
            StatusLabel.Center = new CGPoint(bounds.GetMidX(), bounds.GetMidY());
        }

        [Export("videoView:didChangeVideoSize:")]
        public void DidChangeVideoSize(IRTCVideoRenderer videoView, CGSize size)
        {
            if (videoView == RemoteVideoRender)
            {
                _remoteVideoSize = size;
            }
            SetNeedsLayout();
        }


        private void OnCameraSwitch(object sender, EventArgs e)
        {
            Delegate?.DidSwitchCamera(this);
        }

        private void OnRouteChange(object sender, EventArgs e)
        {
            Delegate?.DidChangeRoute(this);

        }

        private void OnHangup(object sender, EventArgs e)
        {
            Delegate?.DidHangup(this);

        }

        private void DidTripleTap()
        {
            Delegate?.DidEnableStats(this);
        }
    }
}
