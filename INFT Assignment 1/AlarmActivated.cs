using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace INFT_Assignment_1
{
    [Activity(Label = "AlarmActivated")]
    public class AlarmActivated : Activity
    {
        private Button okalarm;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_alarm);
            // Create your application here

            okalarm = FindViewById<Button>(Resource.Id.okalarm);
            okalarm.Click += delegate
            {
                this.StartActivity(typeof(MainActivity));
            };
        }
    }
}