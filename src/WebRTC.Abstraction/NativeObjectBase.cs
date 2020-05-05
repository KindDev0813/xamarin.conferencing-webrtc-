using System;
using WebRTC.Abstraction;

namespace WebRTC.Abstraction
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
            switch (NativeObject)
            {
                case null:
                    throw new NullReferenceException();
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
        }
    }
}