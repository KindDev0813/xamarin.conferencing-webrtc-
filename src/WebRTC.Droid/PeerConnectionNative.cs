using System;
using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Util;
using Java.IO;
using Java.Lang;
using Org.Webrtc;
using WebRTC.Abstraction;
using WebRTC.Droid.Extensions;
using Exception = System.Exception;
using IceCandidate = WebRTC.Abstraction.IceCandidate;
using MediaConstraints = WebRTC.Abstraction.MediaConstraints;
using SessionDescription = WebRTC.Abstraction.SessionDescription;

namespace WebRTC.Droid
{
    public class PeerConnectionNative : NativeObjectBase, IPeerConnection
    {
        private readonly PeerConnection _peerConnection;


        public PeerConnectionNative(PeerConnection peerConnection) : base(peerConnection)
        {
            _peerConnection = peerConnection;
        }

        //public IMediaStream[] LocalStreams { get; }
        public SessionDescription LocalDescription => _peerConnection.LocalDescription.ToNet();
        public SessionDescription RemoteDescription => _peerConnection.RemoteDescription.ToNet();
        public SignalingState SignalingState => _peerConnection.InvokeSignalingState().ToNet();
        public IceConnectionState IceConnectionState => _peerConnection.InvokeIceConnectionState().ToNet();
        public PeerConnectionState PeerConnectionState => _peerConnection.ConnectionState().ToNet();
        public IceGatheringState IceGatheringState => _peerConnection.InvokeIceGatheringState().ToNet();

        public IRtpSender[] Senders =>
            _peerConnection.Senders.Select(s => new RtpSenderNative(s)).Cast<IRtpSender>().ToArray();

        public IRtpReceiver[] Receivers => _peerConnection.Receivers.Select(s => new RtpReceiverNative(s))
            .Cast<IRtpReceiver>().ToArray();

        public IRtpTransceiver[] Transceivers => _peerConnection.Transceivers.Select(s => new RtpTransceiverNative(s))
            .Cast<IRtpTransceiver>().ToArray();

        public RTCConfiguration Configuration { get; private set; }

        public bool SetConfiguration(RTCConfiguration configuration)
        {
            Configuration = configuration;
            return _peerConnection.SetConfiguration(configuration.ToNative());
        }

        public void Close()
        {
            _peerConnection.Close();
        }

        public void AddIceCandidate(IceCandidate candidate)
        {
            _peerConnection.AddIceCandidate(candidate.ToNative());
        }

        public void RemoveIceCandidates(IceCandidate[] candidates)
        {
            _peerConnection.RemoveIceCandidates(candidates.Select(c => c.ToNative()).ToArray());
        }

        public void AddStream(IMediaStream stream)
        {
            _peerConnection.AddStream(stream.ToNative<MediaStream>());
        }

        public void RemoveStream(IMediaStream stream)
        {
            _peerConnection.RemoveStream(stream.ToNative<MediaStream>());
        }

        public IRtpSender AddTrack(IMediaStreamTrack track, string[] streamIds)
        {
            var rtpSender = new RtpSenderNative(_peerConnection.AddTrack(track.ToNative(), streamIds));
            return rtpSender;
        }

        public bool RemoveTrack(IRtpSender sender)
        {
            return _peerConnection.RemoveTrack(sender.ToNative<RtpSender>());
        }

        public IRtpTransceiver AddTransceiverWithTrack(IMediaStreamTrack track)
        {
            throw new NotImplementedException();
        }

        public IRtpTransceiver AddTransceiverWithTrack(IMediaStreamTrack track, IRtpTransceiverInit init)
        {
            throw new NotImplementedException();
        }

        public IRtpTransceiver AddTransceiverOfType(RtpMediaType mediaType)
        {
            throw new NotImplementedException();
        }

        public IRtpTransceiver AddTransceiverOfType(RtpMediaType mediaType, IRtpTransceiverInit init)
        {
            throw new NotImplementedException();
        }

        public void CreateOffer(MediaConstraints constraints,
            SdpCompletionHandler completionHandler)
        {
            _peerConnection.CreateOffer(new SdpObserverProxy(completionHandler), constraints.ToNative());
        }

        public void CreateAnswer(MediaConstraints constraints,
            SdpCompletionHandler completionHandler)
        {
            _peerConnection.CreateAnswer(new SdpObserverProxy(completionHandler), constraints.ToNative());
        }

        public void SetLocalDescription(SessionDescription sdp, Action<Exception> completionHandler)
        {
            _peerConnection.SetLocalDescription(new SdpObserverProxy(
                    (_, error) => completionHandler?.Invoke(error)),
                sdp.ToNative());
        }

        public void SetRemoteDescription(SessionDescription sdp, Action<Exception> completionHandler)
        {
            _peerConnection.SetRemoteDescription(new SdpObserverProxy(
                    (_, error) => completionHandler?.Invoke(error)),
                sdp.ToNative());
        }

        public bool SetBitrate(int min, int current, int max)
        {
            return _peerConnection.SetBitrate(new Integer(min), new Integer(current), new Integer(max));
        }

        public bool StartRtcEventLogWithFilePath(string filePath, long maxSizeInBytes)
        {
            try
            {
                var file = new File(filePath);

                var fileDescriptor = ParcelFileDescriptor.Open(file,
                    ParcelFileMode.ReadWrite | ParcelFileMode.Create | ParcelFileMode.Truncate);
                return _peerConnection.StartRtcEventLog(fileDescriptor.DetachFd(), (int) maxSizeInBytes);
            }
            catch (IOException e)
            {
                Log.Error(nameof(PeerConnectionNative), "Failed to create a new file", e);
                return false;
            }
        }

        public void StopRtcEventLog()
        {
            _peerConnection.StopRtcEventLog();
        }
    }
}