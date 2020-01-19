using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Org.Webrtc;
using WebRTC.AppRTC;

namespace WebRTC.Droid.Demo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            
            AppRTC.AppRTC.Init(new AppRTCFactory());

            var fabBtn = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fabBtn.Click += FabBtnOnClick;
        }

        private void FabBtnOnClick(object sender, EventArgs e)
        {         
            var ws = new WebSocketClient("wss://video.h113.no/ws","eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6OTk5OSwidXNlck5hbWUiOiJUZXN0IiwiaWF0IjoxNTc5NDc0NzA3LCJleHAiOjE1ODAwNzk1MDd9.3uwcwOpFZu88kz0P4Hsgb22agbBXMNz8eVDqQJQrBvk");
            ws.Open();
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

