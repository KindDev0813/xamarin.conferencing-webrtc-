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
using Newtonsoft.Json.Converters;
using WebRTC.AppRTC;

namespace WebRTC.Droid.Demo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private readonly LoginService _loginService = new LoginService();
        private EditText _phoneEditText;
        private EditText _codeEditText;
        private Button _loginButton;

        private View _loadingContainer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.main_activity);
            
            H113Platform.Init(this);

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
            var token = await _loginService.LoginAsync(_phoneEditText.Text, _codeEditText.Text);
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
    }
}