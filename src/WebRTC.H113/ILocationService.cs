using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace WebRTC.H113
{
    public interface ILocationService
    {
        Task<Location> GetLastLocationAsync();
        
        IObservable<Location> OnLocationChanged { get; }
    }
}