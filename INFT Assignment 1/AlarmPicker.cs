using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;

namespace INFT_Assignment_1
{
    public class AlarmPicker : MainActivity
    {
        private Button okalarm;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_alarm);
            okalarm = FindViewById<Button>(Resource.Id.okalarm);
            // Create your application here

            okalarm.Click += delegate
            {
                MainActivity.player.Stop();
                this.StartActivity(typeof(MainActivity));
            };

            MainActivity.player = MediaPlayer.Create(Application.Context, RingtoneManager.GetDefaultUri(RingtoneType.Ringtone));
            MainActivity.player.Start();
        }
    }
}