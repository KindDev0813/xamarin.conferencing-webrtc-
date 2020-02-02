using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace WebRTC.iOS.Demo
{
    public interface IARDMainViewDelegate
    {
        void OnStartCall(ARDMainView mainView, string room, bool isLoopback);
        void DidToggleAudioLoop(ARDMainView mainView);
    }

    public class ARDMainView : UIView, IUITextFieldDelegate
    {

        const float kRoomTextFieldHeight = 40;
        const float kRoomTextFieldMargin = 8;
        const float kCallControlMargin = 8;


        private readonly ARDRoomTextField _roomText;
        private readonly UIButton _startRegularCallButton;
        private readonly UIButton _startLoopbackCallButton;
        private readonly UIButton _audioLoopButton;
        private bool _isAudioLoopPlaying;

        public ARDMainView(CGRect frame) : base(frame)
        {
            _roomText = new ARDRoomTextField(CGRect.Empty);
            AddSubview(_roomText);

            var controlFont = UIFont.BoldSystemFontOfSize(18);
            var controlFontColor = UIColor.White;

            _startRegularCallButton = new UIButton(UIButtonType.System);
            _startRegularCallButton.TitleLabel.Font = controlFont;
            _startRegularCallButton.SetTitleColor(controlFontColor, UIControlState.Normal);
            _startRegularCallButton.BackgroundColor = UIColor.FromRGB((nfloat)(66.0 / 255.0), (nfloat)(200.0 / 255.0), (nfloat)(90.0 / 255.0));
            _startRegularCallButton.SetTitle("Call room", UIControlState.Normal);
            _startRegularCallButton.TouchUpInside += OnStartRegularCall;
            AddSubview(_startRegularCallButton);

            _startLoopbackCallButton = new UIButton(UIButtonType.System);
            _startLoopbackCallButton.TitleLabel.Font = controlFont;
            _startLoopbackCallButton.SetTitleColor(controlFontColor, UIControlState.Normal);
            _startLoopbackCallButton.BackgroundColor = UIColor.FromRGB(0, (nfloat)(122.0 / 255.0), 1);
            _startLoopbackCallButton.SetTitle("Loopback call", UIControlState.Normal);
            _startLoopbackCallButton.TouchUpInside += OnStartLoopbackCall;
            AddSubview(_startLoopbackCallButton);

            _audioLoopButton = new UIButton(UIButtonType.System);
            _audioLoopButton.TitleLabel.Font = controlFont;
            _audioLoopButton.SetTitleColor(controlFontColor, UIControlState.Normal);
            _audioLoopButton.BackgroundColor = UIColor.FromRGB(1, (nfloat)(149.0 / 255.0), 0);
            UpdateAudioLoopButton();
            _audioLoopButton.TouchUpInside += OnToggleAudioLoop;
            AddSubview(_audioLoopButton);
        }



        public IARDMainViewDelegate Delegate { get; set; }

        public string RoomId
        {
            get => _roomText.RoomText;
            set => _roomText.RoomText = value;
        }

        public bool IsAudioLoopPlaying
        {
            get => _isAudioLoopPlaying;
            set
            {
                if (_isAudioLoopPlaying == value)
                    return;
                _isAudioLoopPlaying = value;
                UpdateAudioLoopButton();
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var bounds = Bounds;
            var roomTextWidth = bounds.Size.Width - 2 * kRoomTextFieldMargin;
            var roomTextHeight = _roomText.SizeThatFits(bounds.Size).Height;

            _roomText.Frame = new CGRect(kRoomTextFieldMargin, kRoomTextFieldMargin + 100, roomTextWidth,
                 roomTextHeight);

            var buttonHeight = (bounds.GetMaxY() - _roomText.Frame.GetMaxY() - kCallControlMargin * 4) / 3;

            var regularCallFrameTop = _roomText.Frame.GetMaxY() + kCallControlMargin;
            var regularCallFrame = new CGRect(kCallControlMargin,
                                                 regularCallFrameTop,
                                                 bounds.Size.Width - 2 * kCallControlMargin,
                                                 buttonHeight);

            var loopbackCallFrameTop = regularCallFrame.GetMaxY() + kCallControlMargin;
            CGRect loopbackCallFrame = new CGRect(kCallControlMargin,
                                                  loopbackCallFrameTop,
                                                  bounds.Size.Width - 2 * kCallControlMargin,
                                                  buttonHeight);

            var audioLoopTop = loopbackCallFrame.GetMaxY() + kCallControlMargin;
            var audioLoopFrame = new CGRect(kCallControlMargin,
                                               audioLoopTop,
                                               bounds.Size.Width - 2 * kCallControlMargin,
                                               buttonHeight);

            _startRegularCallButton.Frame = regularCallFrame;
            _startLoopbackCallButton.Frame = loopbackCallFrame;
            _audioLoopButton.Frame = audioLoopFrame;
        }

        private void UpdateAudioLoopButton()
        {
            _audioLoopButton.SetTitle(IsAudioLoopPlaying ? "Stop sound" : "Play sound", UIControlState.Normal);
        }

        private void OnStartRegularCall(object sender, EventArgs e)
        {
            Delegate?.OnStartCall(this, _roomText.RoomText, false);
        }

        private void OnStartLoopbackCall(object sender, EventArgs e)
        {
            Delegate?.OnStartCall(this, _roomText.RoomText, true);
        }

        private void OnToggleAudioLoop(object sender, EventArgs e)
        {
            Delegate?.DidToggleAudioLoop(this);
        }


        public class ARDRoomTextField : UIView, IUITextFieldDelegate
        {
            private readonly UITextField _roomText;

            public ARDRoomTextField(CGRect frame) : base(frame)
            {
                _roomText = new UITextField(CGRect.Empty);
                _roomText.BorderStyle = UITextBorderStyle.None;
                _roomText.Font = UIFont.SystemFontOfSize(12);
                _roomText.Placeholder = "Room name";
                _roomText.AutocorrectionType = UITextAutocorrectionType.No;
                _roomText.AutocapitalizationType = UITextAutocapitalizationType.None;
                _roomText.ClearButtonMode = UITextFieldViewMode.Always;
                _roomText.Delegate = this;

                AddSubview(_roomText);

                Layer.BorderWidth = 1;
                Layer.BorderColor = UIColor.LightGray.CGColor;
                Layer.CornerRadius = 1;
            }

            public string RoomText
            {
                get => _roomText.Text;
                set => _roomText.Text = value;
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();

                _roomText.Frame = new CGRect(kRoomTextFieldMargin, 0, Bounds.Width - kRoomTextFieldMargin, kRoomTextFieldHeight);
            }

            public override CGSize SizeThatFits(CGSize size)
            {
                size.Height = kRoomTextFieldHeight;
                return size;
            }

            [Export("textFieldShouldReturn:")]
            public virtual bool ShouldReturn(UITextField textField)
            {
                _roomText.ResignFirstResponder();
                return true;
            }

        }
    }
}
