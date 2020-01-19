using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using WebRTC.Common;

namespace WebRTC.AppRTC
{

    public enum SignalingMessageType
    {
        Unknown,
        [EnumMember(Value = "new-app-connection")]
        Register,
        [EnumMember(Value = "app-start-video")]
        Registered,
        [EnumMember(Value = "amk-send-offer")]
        Offer,
        [EnumMember(Value = "receive-answer")]
        ReceivedAnswer,
        [EnumMember(Value = "receive-candidate")]
        ReceiveCandidate,
        [EnumMember(Value = "amk-send-candidate")]
        SendCandidate,
    }
    
    public class SignalingMessage
    {
        [JsonProperty("type")]
        public SignalingMessageType MessageType { get; set; }
        
        [JsonProperty("amkSocketId")]
        public string SocketId { get; set; }

        public static SignalingMessage FromJson(string json)
        {
            var values = JsonConvert.DeserializeObject<Dictionary<string,string>>(json);
            if (values.ContainsKey("type"))
            {
                var type = JsonConvert.DeserializeObject<SignalingMessageType>(values["type"]);
                switch (type)
                {
                    case SignalingMessageType.Registered:
                        return JsonConvert.DeserializeObject<RegisteredMessage>(json);
                    case SignalingMessageType.Offer:
                        break;
                    case SignalingMessageType.ReceivedAnswer:
                        return JsonConvert.DeserializeObject<SessionDescriptionMessage>(json);
                    case SignalingMessageType.ReceiveCandidate:
                        return JsonConvert.DeserializeObject<IceCandidateMessage>(json);
                }
            }

            AppRTC.Logger.Debug($"SignalingMessage.FromJson invalid json:{json}");
            return new SignalingMessage();
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
        
        
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; }
        
        [JsonProperty("longitude")]
        public double Longitude { get; }
        
        [JsonProperty("latitude")]
        public double Latitude { get; }
    }

    public class RegisteredMessage : SignalingMessage
    {
        public RegisteredMessage()
        {
            MessageType =  SignalingMessageType.Registered;
        }
        
        [JsonProperty("rtcServer")]
        public RTCServers RTCServer { get; set; }
        
        [JsonProperty("stun")]
        public string StunServer { get; set; }

        public class RTCServers
        {
            [JsonProperty("turn1")]
            public RTCServerEx Turn1 { get; set; }
            
            [JsonProperty("turn2")]
            public RTCServerEx Turn2 { get; set; }
        }

        public class RTCServerEx
        {
            [JsonProperty("urls")]
            public string Url { get; set; }
            
            [JsonProperty("username")]
            public string Username { get; set; }
            
            [JsonProperty("credential")]
            public string Password { get; set; }
            
            [JsonProperty("credentialType")]
            public string CredentialType { get; set; }
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
            IceCandidate = iceCandidate;
        }

        [JsonProperty("candidate")]
        public IceCandidate IceCandidate { get; set; }
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