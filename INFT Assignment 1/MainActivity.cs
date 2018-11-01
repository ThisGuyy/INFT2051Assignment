using Android;
using Android.App;
using Android.Content;
using Android.Icu.Util;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Timers;

namespace INFT_Assignment_1
{
    [Activity(Label = "Swiss Alarmy Knife",Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        public static Android.Media.MediaPlayer player { get; set; }
        TextView textMessage;
        TextView txtTimer;
        Button addalarm;
        Button btnStart;
        Button btnPause;
        Button btnLap;
        //Button btnAlarmDate;
        //Button btnAlarmTime;
        Timer timer;
        LinearLayout container;
        int mins = 0, secs = 0, millisecond = 1;
        //Android.Icu.Util.Calendar calendar;
        DatePickerDialog.DateSetEventArgs date;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            textMessage = FindViewById<TextView>(Resource.Id.message);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            txtTimer = FindViewById<TextView>(Resource.Id.txtTimer);
            btnStart = FindViewById<Button>(Resource.Id.btnStart);
            btnPause = FindViewById<Button>(Resource.Id.btnPause);
            btnLap = FindViewById<Button>(Resource.Id.btnLap);
            addalarm = FindViewById<Button>(Resource.Id.addalarm);

            txtTimer.Visibility = ViewStates.Invisible;


            //This code checks that the user has location permissions granted
            if (ContextCompat.CheckSelfPermission(this,
            Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new System.String[] { Manifest.Permission.AccessFineLocation }, 0);
            }

            //This code checks that the user has location permissions granted
            if (ContextCompat.CheckSelfPermission(this,
            Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new System.String[] { Manifest.Permission.AccessCoarseLocation }, 0);
            }


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
                DateTime today = DateTime.Today;
                DatePickerDialog dialog = new DatePickerDialog(this, OnDateSet, today.Year, today.Month - 1, today.Day);
                //the minimum day to pick is today
                dialog.DatePicker.MinDate = today.Millisecond;
                dialog.Show();

  

            };
        }
        void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            date = e;//.ToLongDateString();
            DateTime dt = DateTime.Now;

            var _hour = dt.Hour;
            var _min = dt.Minute;


            DateTime today = DateTime.Today;
            TimePickerDialog dialog = new TimePickerDialog(this, OntimeSet, _hour, _min, true);
            dialog.Show();
        }

        private void OntimeSet(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            DateTime dt = DateTime.Now;
            var minute = 0;
            var hour = 0;
            //Log.Debug("SO", "tTESTESTESTESTESTSETESTESTSETESTESTESTESTESTESTESTESTESTESTES" + date + " and " + e.HourOfDay + "with minute" + e.Minute);
            if (dt.Hour < e.HourOfDay)
            {
                hour = e.HourOfDay - dt.Hour;
                Log.Debug("SO", "the countdown in hours (current is more) is: " + hour);
            }
            else if (dt.Hour > e.HourOfDay) {
                hour = dt.Hour - e.HourOfDay;
                Log.Debug("SO", "the countdown in hours (current is less) is: " + hour);
            }
            if (hour != 0)
            {
                //the last hour is irrelevant as we will use Minute for more precision
                hour--;
            }
            
            if (dt.Minute < e.Minute)
            {
                minute = Math.Abs(dt.Minute - e.Minute);
            }
            else if (dt.Minute > e.Minute)
            {
                minute = (60 - dt.Minute) + e.Minute;
            }
            //minute is difference in time from current to chosen minute
            Log.Debug("SO", "the countdown in minutes is: " + minute);
            using (var manager = (Android.App.AlarmManager)GetSystemService(AlarmService))
                using (var calendar = Calendar.Instance)
                {
                calendar.Add(CalendarField.Minute, minute);
                calendar.Add(CalendarField.Hour, hour);
                //this
                //calendar.Add(CalendarField.DayOfMonth, date.DayOfMonth);
                //calendar.Add(CalendarField.Month, date.Month);

                Log.Debug("SO", "tTESTESTESTESTESTSETESTESTSETESTESTESTESTESTESTESTESTESTESTES" + minute + " and hour " + hour + " and day of month" + date.DayOfMonth + "and month" + date.Month);
                Log.Debug("SO", "Calendar" + calendar.TimeInMillis + "normal time: " + calendar.Time);

                Log.Debug("SO", $"Current date is   : {Calendar.Instance.Time.ToString()}");
                Log.Debug("SO", $"Alarm will fire at {calendar.Time.ToString()}");
                var alarmIntent = new Intent(ApplicationContext, typeof(AlarmReceiver));
                var pendingIntent = PendingIntent.GetBroadcast(this, 0, alarmIntent, PendingIntentFlags.OneShot);
                manager.SetExact(AlarmType.RtcWakeup, calendar.TimeInMillis, pendingIntent);
                }
            DateTime today = DateTime.Today;
            LayoutInflater inflater = (LayoutInflater)BaseContext.GetSystemService(Context.LayoutInflaterService);
            View addView = inflater.Inflate(Resource.Layout.row, null);
            TextView textContent = addView.FindViewById<TextView>(Resource.Id.textView1);
            container = FindViewById<LinearLayout>(Resource.Id.container);
            //textContent.Text = "Alarm in : " + hour + " Hours and " + minute + " Minutes";
            textContent.Text = "Alaram Set for : " + e.HourOfDay + ":" + e.Minute + " - " + date.Date.Day + "/" + date.Date.Month;
            container.AddView(addView);
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

