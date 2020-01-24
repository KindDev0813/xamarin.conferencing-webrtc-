using System;
using WebRTC.Abstraction;

namespace WebRTC.Droid
{
    public abstract class NativeObjectBase : INativeObject
    {
        
        protected NativeObjectBase()
        {
        }
        
        protected NativeObjectBase(object nativeObject)
        {
            NativeObject = nativeObject;
        }

       

        public object NativeObject { get; protected set; }

        public virtual void Dispose()
        {
            if (NativeObject is IDisposable disposable)
                disposable.Dispose();
        }
    }
}