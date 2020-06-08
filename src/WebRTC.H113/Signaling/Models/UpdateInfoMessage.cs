using System;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace WebRTC.H113.Signaling.Models
{
    public class UpdateInfoMessage : SignalingMessage
    {
        public UpdateInfoMessage(string id, Location location)
        { 
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
        
        [JsonProperty("id")] public string Id { get; }
        [JsonProperty("latitude")] public double Latitude { get; }
        [JsonProperty("longitude")] public double Longitude { get; }
        [JsonProperty("accuracy")] public double Accuracy { get; }
        [JsonProperty("seconds")] public int Seconds { get; }
        [JsonProperty("heading")] public double Heading { get; }
        [JsonProperty("altitude")] public double Altitude { get; }
        [JsonProperty("altitudeAccuracy")] public double AltitudeAccuracy { get; }
    }
}