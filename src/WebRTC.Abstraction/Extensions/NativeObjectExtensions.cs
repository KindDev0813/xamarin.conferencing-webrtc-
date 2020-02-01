namespace WebRTC.Abstraction.Extensions
{
    public static class NativeObjectExtensions
    {
        public static T ToNative<T>(this INativeObject self)
        {
            return (T) self.NativeObject;
        }
    }
}