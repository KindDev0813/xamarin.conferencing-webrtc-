using System.Runtime.CompilerServices;
using WebRTC.Abstraction;

namespace WebRTC
{
    public static class NativeObjectExtensions
    {
        public static T ToNative<T>(this INativeObject self)
        {
            return (T) self.NativeObject;
        }
    }
}