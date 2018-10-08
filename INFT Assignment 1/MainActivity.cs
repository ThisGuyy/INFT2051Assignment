using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace INFT_Assignment_1
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        TextView textMessage;
        Button addalarm;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            textMessage = FindViewById<TextView>(Resource.Id.message);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            addalarm = FindViewById<Button>(Resource.Id.addalarm);
            

            navigation.SetOnNavigationItemSelectedListener(this);

            addalarm.Click += delegate
            {
                var alarmIntent = new Intent(this, typeof(AlarmReceiver));
                alarmIntent.PutExtra("title", "Hello");
                alarmIntent.PutExtra("message", "World!");

                var pending = PendingIntent.GetBroadcast(this, 0, alarmIntent, PendingIntentFlags.UpdateCurrent);

                var alarmManager = GetSystemService(AlarmService).JavaCast<AlarmManager>();
                alarmManager.Set(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime() + 1 * 1000, pending);
            };
        }
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    textMessage.SetText(Resource.String.title_home);
                    addalarm.Visibility = ViewStates.Visible;
                    return true;
                case Resource.Id.navigation_stopwatch:
                    textMessage.SetText(Resource.String.title_stopwatch);
                    addalarm.Visibility = ViewStates.Invisible;
                    return true;
                case Resource.Id.navigation_profile:
                    textMessage.SetText(Resource.String.title_profile);
                    addalarm.Visibility = ViewStates.Invisible;
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

