using System.Collections.Generic;
using WebRTC.Abstraction;
using WebRTC.AppRTC.Extensions;

namespace WebRTC.AppRTC
{
    public partial class H113Client : ISignalingEvents
    {
        private IList<IceCandidate> _wsQueue = new List<IceCandidate>();
        private bool _receivedSdp;
        void ISignalingEvents.OnChannelConnected(RegisteredMessage registeredMessage)
        {
            _logger.Debug(TAG, "Creating PeerConnectionClient");
            _executor.Execute(() =>
            {
                _wsQueue.Clear();
                var peerConnectionClientParams = new PeerConnectionParameters(registeredMessage.GetIceServers())
                {
                    VideoCallEnabled = true
                };
                _peerConnectionClient =
                    new PeerConnectionClient(peerConnectionClientParams, this,
                        _logger);
                _peerConnectionClient.CreatePeerConnectionFactory();
                _events.ReadyToCall();
                _logger.Debug(TAG, "Created PeerConnectionClient");
            });
        }

        void ISignalingEvents.OnChannelClose()
        {
            _executor.Execute(() =>
            {
                _events.OnDisconnect(DisconnectType.WebSocket);
            });
        }

        void ISignalingEvents.OnChannelError(string description)
        {
           _executor.Execute(() =>
           {
               _events.OnError(description);
           });
        }

        void ISignalingEvents.OnRemoteDescription(SessionDescription sdp)
        {
            _executor.Execute(() =>
            {
                if (_peerConnectionClient == null)
                {
                    _logger.Error(TAG, "Received remote SDP for non-initilized peer connection.");
                    return;
                }
                _peerConnectionClient.SetRemoteDescription(sdp);
                _receivedSdp = true;
                DrainQueue();
            });
        }

        void ISignalingEvents.OnRemoteIceCandidate(IceCandidate candidate)
        {
            _executor.Execute(() =>
            {
                if (_peerConnectionClient == null)
                {
                    _logger.Error(TAG, "Received remote SDP for non-initilized peer connection.");
                    return;
                }
                if (!_receivedSdp)
                {
                    _wsQueue.Add(candidate);
                }
                else
                {
                    _peerConnectionClient.AddRemoteIceCandidate(candidate);
                }
            });
        }

        void ISignalingEvents.OnRemoteIceCandidatesRemoved(IceCandidate[] candidates)
        {
            _executor.Execute(() =>
            {
                if (_peerConnectionClient == null)
                {
                    _logger.Error(TAG, "Received remote SDP for non-initilized peer connection.");
                    return;
                }
                _peerConnectionClient.RemoveRemoteIceCandidates(candidates);
            });
        }

        private void DrainQueue()
        {
            foreach (var candidate in _wsQueue)
            {
                _rtcClient.SendLocalIceCandidate(candidate);
            }
        }
    }
}