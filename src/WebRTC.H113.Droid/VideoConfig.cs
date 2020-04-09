using Android.Graphics;
using WebRTC.Abstraction;

namespace WebRTC.H113.Droid
{
    public class VideoConfig
    {
        public VideoConfig() : this(true,ScalingType.AspectFill,Color.Black)
        {
        }

        public VideoConfig(bool useFrontCamera, ScalingType scaling, Color backgroundColor)
        {
            UseFrontCamera = useFrontCamera;
            Scaling = scaling;
            BackgroundColor = backgroundColor;
        }
        public bool UseFrontCamera { get; } 
        public ScalingType Scaling{ get; }
        public int BackgroundColor { get; }
    }
}