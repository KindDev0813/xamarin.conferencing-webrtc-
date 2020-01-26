using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebRTC.AppRTC
{
    public class LoginService 
    {
        private readonly HttpClient _httpClient = new HttpClient();
        
        public async Task<string> LoginAsync(string phone, string code)
        {
            try
            {
               
                var userId = await GetUserIdAsync(phone, _httpClient);
                return await GetTokenAsync(code, userId, _httpClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "";
            }
        }
        
        private static async Task<int> GetUserIdAsync(string phoneNumber, HttpClient client)
        {
            var loginRequest = GetJsonHttpContent(new {phoneNumber});
            var loginResponse = await GetJson(await client.PostAsync(H113Constants.LoginUrl, loginRequest));
            return int.Parse(loginResponse["userId"]);
        }

        private static async Task<string> GetTokenAsync(string code, int userId, HttpClient client)
        {
            var codeRequest = GetJsonHttpContent(new {id = userId, code});
            var codeResponse = await GetJson(await client.PostAsync(H113Constants.CodeUrl, codeRequest));
            return codeResponse["token"];
        }

        private static async Task<Dictionary<string, string>> GetJson(HttpResponseMessage responseMessage)
        {
            var json = await responseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        private static HttpContent GetJsonHttpContent(object data)
        {
            return new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        }
    }
}