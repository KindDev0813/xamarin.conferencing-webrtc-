using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using WebRTC.H113;

namespace WebRTC.Droid.Demo
{
    [Activity]
    public class LoginActivity : BaseActivity
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

            SetContentView(Resource.Layout.activity_login);
            
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
            StartActivity(typeof(H113CallActivity));
        }

       
    }
}