using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace WebRTC.Droid.Demo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private EditText _phoneEditText;
        private EditText _codeEditText;
        private Button _loginButton;

        private View _loadingContainer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.main_activity);

            _phoneEditText = FindViewById<EditText>(Resource.Id.et_phone);
            _codeEditText = FindViewById<EditText>(Resource.Id.et_code);
            _loginButton = FindViewById<Button>(Resource.Id.start_call);

            _loadingContainer = FindViewById(Resource.Id.loading_container);

            _loadingContainer.Visibility = ViewStates.Gone;
            
            _loginButton.Click += LoginButtonOnClick;
        }

        private async void LoginButtonOnClick(object sender, EventArgs e)
        {
            _loadingContainer.Visibility = ViewStates.Visible;
            var token = await GetTokenAsync(_phoneEditText.Text, _codeEditText.Text);
            if (string.IsNullOrEmpty(token))
            {
                Toast.MakeText(this, "Failed to get token.", ToastLength.Long).Show();
                return;
            }
            _loadingContainer.Visibility = ViewStates.Gone;


            H113Constants.Token = token;
            StartActivity(typeof(CallActivity));
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        private static async Task<string> GetTokenAsync(string phone, string code)
        {
            try
            {
                using var client = new HttpClient();
                var userId = await GetUserIdAsync(phone, client);
                return await GetTokenAsync(code, userId, client);
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