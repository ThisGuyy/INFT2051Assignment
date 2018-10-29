using Android.App;
using Android.Content;
using Android.Icu.Util;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Timers;

namespace INFT_Assignment_1
{
    [Activity(Label = "@string/app_name", ShowForAllUsers = true, Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        public static Android.Media.MediaPlayer player { get; set; }
        TextView textMessage;
        TextView txtTimer;
        Button addalarm;
        Button btnStart;
        Button btnPause;
        Button btnLap;
        Timer timer;
        LinearLayout container;
        int mins = 0, secs = 0, millisecond = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            textMessage = FindViewById<TextView>(Resource.Id.message);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            addalarm = FindViewById<Button>(Resource.Id.addalarm);
            txtTimer = FindViewById<TextView>(Resource.Id.txtTimer);
            btnStart = FindViewById<Button>(Resource.Id.btnStart);
            btnPause = FindViewById<Button>(Resource.Id.btnPause);
            btnLap = FindViewById<Button>(Resource.Id.btnLap);

            btnStart.Click += delegate {
                if (timer == null)
                {
                    timer = new Timer();
                    timer.Interval = 1; // 1 Milliseconds  
                    timer.Elapsed += Timer_Elapsed;
                    timer.Start();
                }
            };
            btnPause.Click += delegate {
                if (timer != null)
                {
                    timer.Stop();
                    timer = null;
                }

            };
            btnLap.Click += delegate {
                LayoutInflater inflater = (LayoutInflater)BaseContext.GetSystemService(Context.LayoutInflaterService);
                View addView = inflater.Inflate(Resource.Layout.row, null);
                TextView textContent = addView.FindViewById<TextView>(Resource.Id.textView1);
                container = FindViewById<LinearLayout>(Resource.Id.container);
                textContent.Text = txtTimer.Text;
                container.AddView(addView);
            };

            navigation.SetOnNavigationItemSelectedListener(this);

            addalarm.Click += delegate
            {
                //var alarmIntent = new Intent(this, typeof(AlarmReceiver));
                //alarmIntent.PutExtra("title", "Hello");
                //alarmIntent.PutExtra("message", "World!");
                // testing

                //var pending = PendingIntent.GetBroadcast(this, 0, alarmIntent, PendingIntentFlags.UpdateCurrent);

                //var alarmManager = GetSystemService(AlarmService).JavaCast<AlarmManager>();
               // alarmManager.Set(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime() + 6 * 1000, pending);

                using (var manager = (Android.App.AlarmManager)GetSystemService(AlarmService))
                using (var calendar = Calendar.Instance)
                {
                    calendar.Add(CalendarField.Second, 20);
                    Log.Debug("SO", $"Current date is   : {Calendar.Instance.Time.ToString()}");
                    Log.Debug("SO", $"Alarm will fire at {calendar.Time.ToString()}");
                    var alarmIntent = new Intent(ApplicationContext, typeof(AlarmReceiver));
                    var pendingIntent = PendingIntent.GetBroadcast(this, 0, alarmIntent, PendingIntentFlags.OneShot);
                    manager.SetExact(AlarmType.RtcWakeup, calendar.TimeInMillis, pendingIntent);
                }
            };
        }
        //taken from: https://www.c-sharpcorner.com/article/xamarin-android-stop-watch-milliseconds/
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            millisecond++;
            if (millisecond > 1000)
            {
                secs++;
                millisecond = 0;
            }
            if (secs == 59)
            {
                mins++;
                secs = 0;
            }
            RunOnUiThread(() => {
                txtTimer.Text = String.Format("{0}:{1:00}:{2:000}", mins, secs, millisecond);
            });
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    //textMessage.SetText(Resource.String.title_home);
                    addalarm.Visibility = ViewStates.Visible;
                    txtTimer.Visibility = ViewStates.Invisible;
                    btnStart.Visibility = ViewStates.Invisible;
                    btnPause.Visibility = ViewStates.Invisible;
                    btnLap.Visibility = ViewStates.Invisible;


                    return true;
                case Resource.Id.navigation_stopwatch:
                    //textMessage.SetText(Resource.String.title_stopwatch);
                    addalarm.Visibility = ViewStates.Invisible;
                    txtTimer.Visibility = ViewStates.Visible;
                    btnStart.Visibility = ViewStates.Visible;
                    btnPause.Visibility = ViewStates.Visible;
                    btnLap.Visibility = ViewStates.Visible;

                    return true;
                case Resource.Id.navigation_profile:
                    //textMessage.SetText(Resource.String.title_profile);
                    txtTimer.Visibility = ViewStates.Invisible;
                    btnStart.Visibility = ViewStates.Invisible;
                    btnPause.Visibility = ViewStates.Invisible;
                    btnLap.Visibility = ViewStates.Invisible;

                    return true;
            }
            return false;
        }

        public override void OnAttachedToWindow()
        {
            Window.AddFlags(WindowManagerFlags.ShowWhenLocked |
                            WindowManagerFlags.KeepScreenOn |
                            WindowManagerFlags.DismissKeyguard |
                            WindowManagerFlags.TurnScreenOn);
        }
    }
}

