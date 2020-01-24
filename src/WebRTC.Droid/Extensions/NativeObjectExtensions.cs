using WebRTC.Abstraction;

namespace WebRTC.Droid.Extensions
{
    internal static class NativeObjectExtensions
    {
        public static T ToNative<T>(this INativeObject self)
        {
            return (T) self.NativeObject;
        }
    }
}