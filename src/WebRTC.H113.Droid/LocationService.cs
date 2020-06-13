using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Android.Content;
using Android.Locations;
using Android.OS;
using Xamarin.Essentials;
using Location = Xamarin.Essentials.Location;

namespace WebRTC.H113.Droid
{
    public class LocationService :Java.Lang.Object,ILocationListener,  ILocationService
    {
        private readonly BehaviorSubject<Location> _onLocationChanged = new BehaviorSubject<Location>(null);

        private LocationService()
        {
            // var locationManager = (LocationManager) Platform.AppContext.GetSystemService(Context.LocationService);
            // locationManager.RequestLocationUpdates(LocationManager.GpsProvider,2000,1,this);
        }
        
        public static ILocationService Current { get; } = new LocationService();
        
        public Task<Location> GetLastLocationAsync() => Geolocation.GetLastKnownLocationAsync();

        public IObservable<Location> OnLocationChanged => _onLocationChanged.AsObservable();
        
        void ILocationListener.OnLocationChanged(Android.Locations.Location location)
        {
            _onLocationChanged.OnNext(new Location
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Accuracy = location.Accuracy,
                Altitude = location.Altitude,
                Speed = location.Speed,
                VerticalAccuracy = location.VerticalAccuracyMeters,
                IsFromMockProvider = false,
            });
        }

        void ILocationListener.OnProviderDisabled(string provider)
        {
        }

        void ILocationListener.OnProviderEnabled(string provider)
        {
        }

        void ILocationListener.OnStatusChanged(string provider, Availability status, Bundle extras)
        {
         
        }
    }
}