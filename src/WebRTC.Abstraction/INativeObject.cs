using System;

namespace WebRTC.Abstraction
{
    public interface INativeObject : IDisposable
    {
        object NativeObject { get; }
    }
}