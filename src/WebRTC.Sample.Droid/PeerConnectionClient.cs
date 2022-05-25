using System;
using System.Collections.Generic;
using System.Diagnostics;
using Android.Content;
using Java.Lang;
using Java.Util.Concurrent;
using Org.Webrtc;
using Exception = System.Exception;

namespace WebRTC.Sample.Droid
{
    public partial class PeerConnectionClient : Java.Lang.Object
    {
        private const int VideoWidth = 1280;
        private const int VideoHeight = 720;

        private readonly Context _context;
        private readonly IEglBase _eglBase;

        private PeerConnectionFactory _factory;
        private PeerConnection _peerConnection;

        private MediaConstraints _audioConstraints;
        private MediaConstraints _sdpMediaConstraints;

        private SurfaceTextureHelper _surfaceTextureHelper;

        private IVideoSink _localVideoSink;
        private VideoTrack _localVideoTrack;

        private IVideoSink _removeVideoSink;

        private IVideoCapturer _videoCapturer;
        private VideoSource _videoSource;


        private AudioSource _audioSource;
        private AudioTrack _localAudioTrack;
        private static IExecutorService Executor { get; } = Executors.NewSingleThreadExecutor();


        public PeerConnectionClient(Context context, IEglBase eglBase)
        {
            _context = context.ApplicationContext;
            _eglBase = eglBase;
            Executor.Execute(new Runnable(() =>
            {
                PeerConnectionFactory.Initialize(PeerConnectionFactory.InitializationOptions.InvokeBuilder(context)
                    .CreateInitializationOptions());

                CreatePeerConnectionFactoryInternal();
            }));
        }

        public void CreatePeerConnection(IVideoSink localVideoSink, IVideoSink remoteVideoSink,
            IVideoCapturer videoCapturer)
        {
            _localVideoSink = localVideoSink;
            _removeVideoSink = remoteVideoSink;

            _videoCapturer = videoCapturer;

            Executor.Execute(new Runnable(() =>
            {
                try
                {
                    CreateMediaConstraintsInternal();
                    CreatePeerConnectionInternal();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to create peer connection:{ex.Message}");
                }
            }));
        }


        private void CreateMediaConstraintsInternal()
        {
            _audioConstraints = new MediaConstraints();

            _sdpMediaConstraints = new MediaConstraints();

            _sdpMediaConstraints.Mandatory.Add(new MediaConstraints.KeyValuePair("OfferToReceiveAudio", "true"));
            _sdpMediaConstraints.Mandatory.Add(new MediaConstraints.KeyValuePair("OfferToReceiveVideo", "true"));
        }

        private void CreatePeerConnectionInternal()
        {
            var rtcConfig = new PeerConnection.RTCConfiguration(new List<PeerConnection.IceServer>())
            {
                TcpCandidatePolicy = PeerConnection.TcpCandidatePolicy.Disabled,
                BundlePolicy = PeerConnection.BundlePolicy.Maxbundle,
                RtcpMuxPolicy = PeerConnection.RtcpMuxPolicy.Require,
                ContinualGatheringPolicy = PeerConnection.ContinualGatheringPolicy.GatherContinually,
                KeyType = PeerConnection.KeyType.Ecdsa,
                EnableDtlsSrtp = new Java.Lang.Boolean(true),
                SdpSemantics = PeerConnection.SdpSemantics.UnifiedPlan
            };

            _peerConnection = _factory.CreatePeerConnection(rtcConfig, this);

            var mediaStreamLabels = new List<string> {"ARDAMS"};
            _peerConnection.AddTrack(CreateVideoTrack(_videoCapturer), mediaStreamLabels);
            _peerConnection.AddTrack(CreateAudioTrack(), mediaStreamLabels);
        }

        private void CreatePeerConnectionFactoryInternal()
        {
            var encoderFactory = new DefaultVideoEncoderFactory(_eglBase.EglBaseContext, true, true);
            var decoderFactory = new DefaultVideoDecoderFactory(_eglBase.EglBaseContext);

            _factory = PeerConnectionFactory.InvokeBuilder()
                .SetVideoEncoderFactory(encoderFactory)
                .SetVideoDecoderFactory(decoderFactory)
                .CreatePeerConnectionFactory();
        }

        private AudioTrack CreateAudioTrack()
        {
            _audioSource = _factory.CreateAudioSource(_audioConstraints);
            _localAudioTrack = _factory.CreateAudioTrack("ARDAMSa0", _audioSource);
            _localAudioTrack.SetEnabled(true);
            return _localAudioTrack;
        }

        private VideoTrack CreateVideoTrack(IVideoCapturer videoCapturer)
        {
            _surfaceTextureHelper = SurfaceTextureHelper.Create("CaptureThread", _eglBase.EglBaseContext);

            _videoSource = _factory.CreateVideoSource(true);
            videoCapturer.Initialize(_surfaceTextureHelper, _context, _videoSource.CapturerObserver);
            videoCapturer.StartCapture(VideoWidth, VideoHeight, 30);

            _localVideoTrack = _factory.CreateVideoTrack("ARDAMSv0", _videoSource);
            _localVideoTrack.SetEnabled(true);
            _localVideoTrack.AddSink(_localVideoSink);
            return _localVideoTrack;
        }
    }
}