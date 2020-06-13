using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace WebRTC.H113.iOS
{
    public class LocationService : ILocationService
    {
        private LocationService()
        {
            
        }
        
        public static LocationService Current { get; } = new LocationService();
        public Task<Location> GetLastLocationAsync() => Geolocation.GetLocationAsync();
    
        public IObservable<Location> OnLocationChanged { get; }
    }
}