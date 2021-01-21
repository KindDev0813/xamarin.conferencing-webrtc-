using System;
using WebRTC.Abstraction;
using Org.Webrtc;

namespace WebRTC.Droid
{
    public class VideoRendererProxy : Java.Lang.Object, IVideoSink, IVideoRenderer
    {
        private IVideoSink _renderer;
        public object NativeObject => this;

        public IVideoSink Renderer
        {
            get => _renderer;
            set
            {
                if (_renderer == this)
                    throw new InvalidOperationException("You can set renderer to self");
                _renderer = value;
            }
        }

        public virtual void OnFrame(VideoFrame p0)
        {
            Renderer?.OnFrame(p0);
            p0?.Dispose();
        }
    }
}