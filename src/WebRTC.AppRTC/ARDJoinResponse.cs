using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebRTC.AppRTC
{
     public enum ARDJoinResultType
    {
        Unknown,
        Success,
        Full
    }

    public class ARDJoinResponse
    {
        [JsonProperty("params")]
        public ServerParams ServerParams { get; set; }

        [JsonConverter(typeof(ARDJoinResultTypeStringConverter))]
        [JsonProperty("result")]
        public ARDJoinResultType Result { get; set; }
    }

    public class ServerParams
    {
        [JsonProperty("is_initiator")]
        public bool IsInitiator { get; set; }

        [JsonProperty("room_link")]
        public string RoomLink { get; set; }

        [JsonProperty("messages")]
        public string[] Messages { get; set; }

        [JsonProperty("error_messages")]
        public string[] ErrorMessages { get; set; }

        [JsonProperty("offer_options")]
        public string OfferOptions { get; set; }

        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("ice_server_transports")]
        public string IceServerTransports { get; set; }

        [JsonProperty("bypass_join_confirmation")]
        public bool BypassJoinConfirmation { get; set; }

        [JsonProperty("media_constraints")]
        public string MediaConstraints { get; set; }

        [JsonProperty("include_loopback_js")]
        public string IncludeLoopbackJs { get; set; }

        [JsonProperty("is_loopback")]
        public bool IsLoopback { get; set; }

        [JsonProperty("wss_url")]
        public string WssUrl { get; set; }

        [JsonProperty("pc_constraints")]
        public string PcConstraints { get; set; }

        [JsonProperty("pc_config")]
        public string PcConfig { get; set; }

        [JsonProperty("wss_post_url")]
        public string WssPostUrl { get; set; }

        [JsonProperty("ice_server_url")]
        public string IceServerUrl { get; set; }

        [JsonProperty("warning_messages")]
        public string[] WarningMessages { get; set; }

        [JsonProperty("room_id")]
        public string RoomId { get; set; }
    }



    public class PcConfig
    {
        [JsonProperty("rtcpMuxPolicy")]
        public string RtcpMuxPolicy { get; set; }
        
        [JsonProperty("bundlePolicy")]
        public string BundlePolicy { get; set; }
        
        [JsonProperty("iceServers")]
        public IList<string> IceServers { get; set; }
    }
    

    public class ARDJoinResultTypeStringConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(string);


        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return ARDJoinResultType.Unknown;
            var value = serializer.Deserialize<string>(reader);
            if (value == "SUCCESS")
                return ARDJoinResultType.Success;
            if (value == "FULL")
                return ARDJoinResultType.Full;
            return ARDJoinResultType.Unknown;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {

        }
    }
}