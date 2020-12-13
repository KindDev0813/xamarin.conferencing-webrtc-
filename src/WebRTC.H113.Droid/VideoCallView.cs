using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Org.Webrtc;
using WebRTC.Abstraction;
using WebRTC.Droid.Extensions;

namespace WebRTC.H113.Droid
{
    [Register("webRTC.h113.droid.VideoCallView")]
    public class VideoCallView : SurfaceViewRenderer
    {
        private ScalingType _scalingType = ScalingType.AspectBalanced;

        protected VideoCallView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public VideoCallView(Context context) : base(context)
        {
            Setup();
        }

        public VideoCallView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Setup();
        }

        public ScalingType ScalingType
        {
            get => _scalingType;
            set
            {
                if (_scalingType == value)
                    return;
                _scalingType = value;
                SetScalingType(value.ToNative());
            }
        }

        private void Setup()
        {
            SetScalingType(_scalingType.ToNative());
            SetEnableHardwareScaler(false);
        }
    }
}