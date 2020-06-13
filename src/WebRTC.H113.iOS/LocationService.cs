using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using CoreLocation;
using Foundation;
using Xamarin.Essentials;

namespace WebRTC.H113.iOS
{
    public class LocationService : ILocationService
    {
        private readonly BehaviorSubject<Location> _onLocationChanged = new BehaviorSubject<Location>(null);

        private readonly CLLocationManager _locationManager;

        private LocationService()
        {
            _locationManager = new CLLocationManager ();
            Init();
        }

        public static LocationService Current { get; } = new LocationService();
        public Task<Location> GetLastLocationAsync() => Geolocation.GetLocationAsync();

        public IObservable<Location> OnLocationChanged => _onLocationChanged.AsObservable();
        

        private void Init()
        {
            _locationManager.RequestAlwaysAuthorization();

            _locationManager.DesiredAccuracy = 1;
            _locationManager.LocationsUpdated += (s, e) =>
            {
                var location = e.Locations.LastOrDefault();
                if (location == null)
                    return;
                _onLocationChanged.OnNext(ToLocation(location));
            };
            
            _locationManager.StartUpdatingLocation();
        }

        private static Location ToLocation(CLLocation location) =>
            new Location
            {
                Latitude = location.Coordinate.Latitude,
                Longitude = location.Coordinate.Longitude,
                Altitude = location.VerticalAccuracy < 0 ? default(double?) : location.Altitude,
                Accuracy = location.HorizontalAccuracy,
                VerticalAccuracy = location.VerticalAccuracy,
                Timestamp = ToDateTime(location.Timestamp),
                Course = location.Course < 0 ? default(double?) : location.Course,
                Speed = location.Speed < 0 ? default(double?) : location.Speed,
                IsFromMockProvider = DeviceInfo.DeviceType == DeviceType.Virtual
            };

        private static DateTimeOffset ToDateTime(NSDate timestamp)
        {
            try
            {
                return new DateTimeOffset((DateTime) timestamp);
            }
            catch
            {
                return DateTimeOffset.UtcNow;
            }
        }
    }
}