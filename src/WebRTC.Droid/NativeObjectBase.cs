using System;
using WebRTC.Abstraction;

namespace WebRTC.Droid
{
    public abstract class NativeObjectBase : INativeObject
    {
        public NativeObjectBase(object nativeObject)
        {
            NativeObject = nativeObject;
        }

        public object NativeObject { get; }

        public void Dispose()
        {
            if (NativeObject is IDisposable disposable)
                disposable.Dispose();
        }
    }
}