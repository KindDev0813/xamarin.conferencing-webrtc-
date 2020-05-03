using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WebRTC.Abstraction;
using WebRTC.AppRTC.Abstraction;
using Xamarin.Essentials;

namespace WebRTC.H113
{
    public enum SignalingMessageType
    {
        Unknown,

        [EnumMember(Value = "new-app-connection")]
        Register,

        [EnumMember(Value = "app-start-video")]
        Registered,
        [EnumMember(Value = "amk-send-offer")] Offer,
        [EnumMember(Value = "receive-answer")] ReceivedAnswer,

        [EnumMember(Value = "receive-candidate")]
        ReceiveCandidate,

        [EnumMember(Value = "amk-send-candidate")]
        SendCandidate,

        [EnumMember(Value = "app-connection-id")]
        Reconnecting,

        [EnumMember(Value = "app-update-info")]
        UpdateInfo,

        [EnumMember(Value = "app-reconnecting-ws")]
        DoReconnect,

    }

    public class SignalingMessage
    {
        private static JsonSerializerSettings _settings;

        static SignalingMessage()
        {
            _settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            _settings.Converters.Add(new StringEnumConverter());
        }

        public const string Register = "new-app-connection";
        public const string Registered = "app-start-video";
        public const string SendOffer = "amk-send-offer";
        public const string ReceivedAnswer = "receive-answer";
        public const string ReceiveCandidate = "receive-candidate";
        public const string SendCandidate = "amk-send-candidate";
        public const string Reconnecting = "app-connection-id"; //jls
        public const string UpdateInfo = "app-update-info"; //jls
        public const string DoReconnect = "app-reconnecting-ws"; //jls

        [JsonProperty("type")] public SignalingMessageType MessageType { get; set; }

        [JsonProperty("amkSocketId")] public string SocketId { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, _settings);
        }

        public static SignalingMessage FromJson(string json)
        {
            var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            if (values.ContainsKey("type"))
            {
                switch (values["type"].ToString())
                {
                    case Registered:
                        return JsonConvert.DeserializeObject<RegisteredMessage>(json, _settings);
                    case SendOffer:
                        break;
                    case ReceivedAnswer:
                        return GetAnswerSessionDescription(values["answer"].ToString());
                    case ReceiveCandidate:
                        return JsonConvert.DeserializeObject<IceCandidateMessage>(json, _settings);
                    case Reconnecting:
                        return JsonConvert.DeserializeObject<ReconnectingMessage>(json, _settings);
                }
            }

            return null;
        }

        private static SessionDescriptionMessage GetAnswerSessionDescription(string json)
        {
            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json, _settings);

            return new SessionDescriptionMessage
            {
                MessageType = SignalingMessageType.ReceivedAnswer,
                Description = new SessionDescription(SdpType.Answer, values["sdp"])
            };
        }
    }

    public class RegisterMessage : SignalingMessage
    {
        public RegisterMessage(string phoneNumber, double longitude, double latitude)
        {
            PhoneNumber = phoneNumber;
            Longitude = longitude;
            Latitude = latitude;
            MessageType = SignalingMessageType.Register;
        }

        [JsonProperty("phoneNumber")] public string PhoneNumber { get; }

        [JsonProperty("longitude")] public double Longitude { get; }

        [JsonProperty("latitude")] public double Latitude { get; }
    }

    public class DoReconnectMessage : SignalingMessage
    {
        public DoReconnectMessage(string type, string phoneNumber, string id)
        {
            Type = type;
            PhoneNumber = phoneNumber;
            Id = Id;
        }

        [JsonProperty("type")] public string Type { get; }

        [JsonProperty("id")] public string Id { get; }

        [JsonProperty("phoneNumber")] public string PhoneNumber { get; }
    }

    public class UpdateInfoMessage : SignalingMessage
    {
        public UpdateInfoMessage(string type, string id, Location location)
        {
            Type = type;
            Id = id;

            Latitude = location.Latitude;
            Longitude = location.Longitude;
            if (location.Accuracy != null)
                Accuracy = (double)location.Accuracy;
            Seconds = (int)Math.Round(DateTime.Now.Subtract(location.Timestamp.LocalDateTime).TotalSeconds, 0);
            if (location.Course != null)
                Heading = (double)location.Course;
            if (location.Altitude != null)
                Altitude = (double)location.Altitude;
            if (location.VerticalAccuracy != null)
                AltitudeAccuracy = (double)location.VerticalAccuracy;

            MessageType = SignalingMessageType.UpdateInfo;
        }

        [JsonProperty("type")] public string Type { get; }
        [JsonProperty("id")] public string Id { get; }
        [JsonProperty("latitude")] public double Latitude { get; }
        [JsonProperty("longitude")] public double Longitude { get; }
        [JsonProperty("accuracy")] public double Accuracy { get; }
        [JsonProperty("seconds")] public int Seconds { get; }
        [JsonProperty("heading")] public double Heading { get; }
        [JsonProperty("altitude")] public double Altitude { get; }
        [JsonProperty("altitudeAccuracy")] public double AltitudeAccuracy { get; }
    }


    public class ReconnectingMessage : SignalingMessage
    {
        public ReconnectingMessage(string type, string id, string phoneNumber)
        {
            Type = type;
            Id = id;
            PhoneNumber = phoneNumber;
            MessageType = SignalingMessageType.Reconnecting;
        }

        [JsonProperty("type")] public string Type { get; }

        [JsonProperty("id")] public string Id { get; }

        [JsonProperty("phoneNumber")] public string PhoneNumber { get; }
    }

    public class RegisteredMessage : SignalingMessage, ISignalingParameters
    {
        public RegisteredMessage()
        {
            MessageType = SignalingMessageType.Registered;
        }

        [JsonProperty("rtcServer")] public RTCServers RTCServer { get; set; }

        [JsonProperty("stun")] public string StunServer { get; set; }

        public class RTCServers
        {
            [JsonProperty("turn1")] public RTCServerEx Turn1 { get; set; }

            [JsonProperty("turn2")] public RTCServerEx Turn2 { get; set; }
        }

        public class RTCServerEx
        {
            [JsonProperty("urls")] public string Url { get; set; }

            [JsonProperty("username")] public string Username { get; set; }

            [JsonProperty("credential")] public string Password { get; set; }

            [JsonProperty("credentialType")] public string CredentialType { get; set; }
        }
    }

    public class IceCandidateMessage : SignalingMessage
    {
        public IceCandidateMessage()
        {
            MessageType = SignalingMessageType.ReceiveCandidate;
        }

        public IceCandidateMessage(IceCandidate iceCandidate)
        {
            MessageType = SignalingMessageType.SendCandidate;
            IceCandidate = new IceCandidateEx
            {
                Sdp = iceCandidate.Sdp,
                SdpMid = iceCandidate.SdpMid,
                SdpMLineIndex = iceCandidate.SdpMLineIndex
            };
        }

        [JsonProperty("candidate")] public IceCandidateEx IceCandidate { get; set; }

        public class IceCandidateEx
        {
            [JsonProperty("candidate")]
            public string Sdp { get; set; }

            [JsonProperty("sdpMid")]
            public string SdpMid { get; set; }

            [JsonProperty("sdpMlineIndex")]
            public int SdpMLineIndex { get; set; }
        }
    }

    public class SessionDescriptionMessage : SignalingMessage
    {
        public SessionDescriptionMessage()
        {
            MessageType = SignalingMessageType.ReceivedAnswer;
        }

        public SessionDescriptionMessage(SessionDescription description)
        {
            MessageType = SignalingMessageType.Offer;
            Description = description;
        }

        [JsonProperty("offer")]
        public SessionDescription Description { get; set; }
    }
}