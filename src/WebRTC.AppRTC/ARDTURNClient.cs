using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebRTC.Abstraction;

namespace WebRTC.AppRTC
{
    public class ARDTURNClient
    {
        private const string TURNRefererURLString = @"https://appr.tc";

        private readonly HttpClient _httpClient = new HttpClient();

        private readonly string _url;

        public ARDTURNClient(string url)
        {
            _url = url;
        }

        public async Task<IceServer[]> RequestServersAsync()
        {
            var response = await _httpClient.GetAsync(_url).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var serverParams = JsonConvert.DeserializeObject<ServerParams>(json);

            var turnResponse = await MakeTurnServerRequestToUrlAsync(serverParams.IceServerUrl).ConfigureAwait(false);

            var array = turnResponse.IceServers
                .Select(iceServer => new IceServer(iceServer.Urls, iceServer.Username, iceServer.Credential)).ToArray();

            return array;
        }

        private async Task<ARDTurnResponse> MakeTurnServerRequestToUrlAsync(string iceServer)
        {
            _httpClient.DefaultRequestHeaders.Referrer = new Uri(TURNRefererURLString);
            var response = await _httpClient.PostAsync(iceServer, null).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<ARDTurnResponse>(json);
        }
    }
}