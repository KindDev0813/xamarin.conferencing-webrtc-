using System;
using System.Threading.Tasks;

namespace WebRTC.Abstraction.Extensions
{
    public static class PeerConnectionExtensions
    {
        public static Task<SessionDescription> CreateOfferAsync(this IPeerConnection self,
            MediaConstraints mediaConstraints)
        {
            var observer = new SdpObserver();
            self.CreateOffer(mediaConstraints, observer);
            return observer.OnCreateAsync();
        }

        public static async Task<bool> SetLocalDescriptionAsync(this IPeerConnection self, SessionDescription sdp,
            bool throwIfFails = false)
        {
            try
            {
                var observer = new SdpObserver();
                self.SetLocalDescription(sdp, observer);
                return await observer.OnSetAsync();
            }
            catch
            {
                if (throwIfFails)
                    throw;
                return false;
            }
        }

        private class SdpObserver : ISdpObserver
        {
            private readonly TaskCompletionSource<SessionDescription> _onCreate =
                new TaskCompletionSource<SessionDescription>();

            private readonly TaskCompletionSource<bool> _onSet = new TaskCompletionSource<bool>();


            public Task<SessionDescription> OnCreateAsync() => _onCreate.Task;

            public Task<bool> OnSetAsync() => _onSet.Task;

            public void OnCreateSuccess(SessionDescription sdp)
            {
                _onCreate.SetResult(sdp);
            }

            public void OnSetSuccess()
            {
                _onSet.SetResult(true);
            }

            public void OnCreateFailure(string error)
            {
                _onCreate.SetException(new Exception(error));
            }

            public void OnSetFailure(string error)
            {
                _onSet.SetException(new Exception(error));
            }
        }
    }
}