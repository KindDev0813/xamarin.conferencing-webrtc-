using System;
using CoreGraphics;
using Foundation;
using WebRTC.Abstraction;
using WebRTC.iOS.Binding;

namespace WebRTC.iOS
{
    public class VideoRendererProxy : NSObject, IRTCVideoRenderer, IVideoRenderer
    {
        private IRTCVideoRenderer _renderer;

        public object NativeObject => this;

        public IRTCVideoRenderer Renderer
        {
            get => _renderer;
            set
            {
                if (Equals(_renderer, this))
                    throw new InvalidOperationException("You can set renderer to self");
                _renderer = value;
            }
        }

        public Action OnFirstFrame { get; set; }
       
        public void RenderFrame(RTCVideoFrame frame)
        {
            Renderer?.RenderFrame(frame);
            OnFirstFrame?.Invoke();
            OnFirstFrame = null;
            frame.Dispose();
        }

        public void SetSize(CGSize size)
        {
            Renderer?.SetSize(size);
        }
    }
}
