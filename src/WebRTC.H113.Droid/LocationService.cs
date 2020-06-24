using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Android;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using AndroidX.Core.Content;
using Xamarin.Essentials;

using Location = Xamarin.Essentials.Location;

namespace WebRTC.H113.Droid
{
    public class LocationService : Java.Lang.Object, ILocationListener, ILocationService
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);


        private readonly BehaviorSubject<Location> _onLocationChanged = new BehaviorSubject<Location>(null);

        private LocationService()
        {
            var hasAccessFineLocation = ContextCompat.CheckSelfPermission(Platform.AppContext, Manifest.Permission.AccessFineLocation) == (int)Permission.Granted;
            if (hasAccessFineLocation == false)
                return;

            var locationManager = (LocationManager)Platform.AppContext.GetSystemService(Context.LocationService);
            locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 2000, 1, this);
        }

        public static ILocationService Current { get; } = new LocationService();

        public Task<Location> GetLastLocationAsync() => ContextCompat.CheckSelfPermission(Platform.AppContext, Manifest.Permission.AccessFineLocation) == (int)Permission.Granted ? Geolocation.GetLastKnownLocationAsync() : null;

        public IObservable<Location> OnLocationChanged => _onLocationChanged.AsObservable();

        void ILocationListener.OnLocationChanged(Android.Locations.Location location)
        {
            _onLocationChanged.OnNext(ToLocation(location));
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

        static Location ToLocation(Android.Locations.Location location) =>
            new Location
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Altitude = location.HasAltitude ? location.Altitude : default(double?),
                Timestamp = GetTimestamp(location).ToUniversalTime(),
                Accuracy = location.HasAccuracy ? location.Accuracy : default(float?),
                VerticalAccuracy =
#if __ANDROID_26__
                    H113Platform.HasApiLevelO && location.HasVerticalAccuracy
                        ? location.VerticalAccuracyMeters
                        : default(float?),
#else
                    default(float?),
#endif
                Course = location.HasBearing ? location.Bearing : default(double?),
                Speed = location.HasSpeed ? location.Speed : default(double?),
                IsFromMockProvider = H113Platform.HasApiLevel(global::Android.OS.BuildVersionCodes.JellyBeanMr2) &&
                                     location.IsFromMockProvider
            };

        private static DateTimeOffset GetTimestamp(Android.Locations.Location location)
        {
            try
            {
                return new DateTimeOffset(Epoch.AddMilliseconds(location.Time));
            }
            catch (Exception)
            {
                return new DateTimeOffset(Epoch);
            }
        }
    }
}