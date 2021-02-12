using Android.Content;
using Org.Webrtc;

namespace WebRTC.Droid.Extensions
{
    public static class ContextExtensions
    {
        public static ICameraEnumerator GetAllCameras(this Context self, bool captureToTexture = false)
        {
            return UseCamera2(self)
                ? new Camera2Enumerator(self)
                : (ICameraEnumerator) new Camera1Enumerator(captureToTexture);
        }

        private static bool UseCamera2(Context context) => Camera2Enumerator.IsSupported(context);
    }
}