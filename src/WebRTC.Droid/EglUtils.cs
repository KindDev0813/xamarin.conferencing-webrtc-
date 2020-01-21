using System.Linq;
using Android.OS;
using Org.Webrtc;

namespace WebRTC.Droid
{
    public static class EglUtils
    {
        private static IEglBase _eglBase;

        public static IEglBase RootEglBase
        {
            get
            {
                if (_eglBase == null)
                {
                    if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                        _eglBase = EglBase.CreateEgl10(EglBase.ConfigPlain.Cast<int>().ToArray());
                    else
                        _eglBase = EglBase.Create();
                }
                return _eglBase;
            }
        }
    }
}