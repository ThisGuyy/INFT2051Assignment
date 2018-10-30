using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Locations;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;
using Newtonsoft.Json;
using Square.Picasso;
namespace INFT_Assignment_1
{
    [Activity(Label = "AlarmActivated", ShowForAllUsers = true)]
    public class AlarmActivated : Activity, ISerializable, ILocationListener, Android.Hardware.ISensorEventListener
    {
        Button okalarm;
        TextView txtCity, txtLastUpdate, txtDescription, txtHumidity, txtTime, txtCelsius;
        ImageView imgView;
        LocationManager locationManager;
        private bool isGPSEnabled;
        private bool isNetworkEnabled;
        string provider;
        static double lat, lng;
        OpenWeatherMap openWeatherMap = new OpenWeatherMap();
        private bool canGetLocation;
        bool hasUpdated = false;
        DateTime lastUpdate;
        float last_x = 0.0f;
        float last_y = 0.0f;
        float last_z = 0.0f;
        const int ShakeDetectionTimeLapse = 250;
        const double ShakeThreshold = 800;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_alarm);

            //Weather starts here
            locationManager = (LocationManager)GetSystemService(LocationService);
            provider = locationManager.GetBestProvider(new Criteria(), false);
            locationManager.RequestLocationUpdates(provider, 400, 1, this);
            Location location = getLocation();//locationManager.GetLastKnownLocation(provider);
            if (location == null)
                System.Diagnostics.Debug.WriteLine("No Location");
            if (location != null)
                System.Diagnostics.Debug.WriteLine("Location Found");

            okalarm = FindViewById<Button>(Resource.Id.okalarm);
            okalarm.Click += delegate
            {
                MainActivity.player.Stop();
                this.StartActivity(typeof(MainActivity));
            };

            var sensorManager = GetSystemService(SensorService) as Android.Hardware.SensorManager;
            var sensor = sensorManager.GetDefaultSensor(Android.Hardware.SensorType.Accelerometer);
            sensorManager.RegisterListener(this, sensor, Android.Hardware.SensorDelay.Game);

            MainActivity.player = MediaPlayer.Create(Application.Context, RingtoneManager.GetDefaultUri(RingtoneType.Ringtone));
            MainActivity.player.Start();

        }

        public Location getLocation()
        {
            Location location = null;
            double latitude;
            double longitude;

            try
            {
                locationManager = (LocationManager)GetSystemService(LocationService);
                // getting GPS status
                isGPSEnabled = locationManager.IsProviderEnabled(LocationManager.GpsProvider);

                // getting network status
                isNetworkEnabled = locationManager
                        .IsProviderEnabled(LocationManager.NetworkProvider);

                if (!isGPSEnabled && !isNetworkEnabled)
                {
                    // no network provider is enabled
                }
                else
                {
                    this.canGetLocation = true;
                    if (isNetworkEnabled)
                    {
                        locationManager.RequestLocationUpdates(
                                LocationManager.NetworkProvider,
                                200,
                                1, this);
                        System.Diagnostics.Debug.WriteLine("Network", "Network Enabled");
                        if (locationManager != null)
                        {
                            location = locationManager
                                    .GetLastKnownLocation(LocationManager.NetworkProvider);
                            if (location != null)
                            {
                                latitude = location.Latitude;
                                longitude = location.Longitude;
                            }
                        }
                    }
                    // if GPS Enabled get lat/long using GPS Services
                    if (isGPSEnabled)
                    {
                        if (location == null)
                        {
                            locationManager.RequestLocationUpdates(
                                    LocationManager.GpsProvider,
                                    200,
                                    1, this);
                            System.Diagnostics.Debug.WriteLine("GPS", "GPS Enabled");
                            if (locationManager != null)
                            {
                                location = locationManager
                                        .GetLastKnownLocation(LocationManager.GpsProvider);
                                if (location != null)
                                {
                                    latitude = location.Latitude;
                                    longitude = location.Longitude;
                                }
                            }
                        }
                    }
                }

            }
            catch (System.Exception e)
            {
                //e.printStackTrace();
            }

            return location;
        }

        public override void OnAttachedToWindow()
        {

            Window.AddFlags(WindowManagerFlags.ShowWhenLocked |
                            WindowManagerFlags.KeepScreenOn |
                            WindowManagerFlags.DismissKeyguard |
                            WindowManagerFlags.TurnScreenOn);
        }
        protected override void OnResume()
        {
            base.OnResume();
            locationManager.RequestLocationUpdates(provider, 400, 1, this);
        }
        protected override void OnPause()
        {
            base.OnPause();
            locationManager.RemoveUpdates(this);
        }
        public void OnLocationChanged(Location location)
        {
            lat = System.Math.Round(location.Latitude, 4);
            lng = System.Math.Round(location.Longitude, 4);
            new GetWeather(this, openWeatherMap).Execute(Common.APIRequest(lat.ToString(), lng.ToString()));
        }
        public void OnProviderDisabled(string provider) { }
        public void OnProviderEnabled(string provider) { }
        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras) { }

        void ISensorEventListener.OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
        {
        }

        void ISensorEventListener.OnSensorChanged(SensorEvent e)
        {
            if (e.Sensor.Type == Android.Hardware.SensorType.Accelerometer)
            {
                float x = e.Values[0];
                float y = e.Values[1];
                float z = e.Values[2];

                DateTime curTime = System.DateTime.Now;
                if (hasUpdated == false)
                {
                    hasUpdated = true;
                    lastUpdate = curTime;
                    last_x = x;
                    last_y = y;
                    last_z = z;
                }
                else
                {
                    if ((curTime - lastUpdate).TotalMilliseconds > ShakeDetectionTimeLapse)
                    {
                        float diffTime = (float)(curTime - lastUpdate).TotalMilliseconds;
                        lastUpdate = curTime;
                        float total = x + y + z - last_x - last_y - last_z;
                        float speed = System.Math.Abs(total) / diffTime * 10000;

                        if (speed > ShakeThreshold)
                        {
                            //Toast.MakeText(this, "shake detected w/ speed: " + speed, ToastLength.Short).Show();
                            MainActivity.player.Stop();
                            this.StartActivity(typeof(MainActivity));
                        }

                        last_x = x;
                        last_y = y;
                        last_z = z;
                    }
                }
            }
        }

        private class GetWeather : AsyncTask<string, Java.Lang.Void, string>
        {
            private ProgressDialog pd = new ProgressDialog(Application.Context);
            private AlarmActivated activity;
            OpenWeatherMap openWeatherMap;


            public GetWeather(AlarmActivated activity, OpenWeatherMap openWeatherMap)
            {
                this.activity = activity;
                this.openWeatherMap = openWeatherMap;
            }


            protected override string RunInBackground(params string[] @params)
            {
                string stream = null;
                string urlString = @params[0];
                Helper http = new Helper();
                urlString = Common.APIRequest(lat.ToString(), lng.ToString());
                stream = http.GetHTTPData(urlString);
                return stream;
            }
            protected override void OnPostExecute(string result)
            {
                base.OnPostExecute(result);
                if (result.Contains("Error: No City Found"))
                {
                    pd.Dismiss();
                    return;
                }
                Log.Debug("SO", "ALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARMALARM");
                openWeatherMap = JsonConvert.DeserializeObject<OpenWeatherMap>(result);
                pd.Dismiss();

                activity.txtCity = activity.FindViewById<TextView>(Resource.Id.txtCity);
                activity.txtLastUpdate = activity.FindViewById<TextView>(Resource.Id.txtLastUpdate);
                activity.txtDescription = activity.FindViewById<TextView>(Resource.Id.txtDescription);
                activity.txtHumidity = activity.FindViewById<TextView>(Resource.Id.txtHumidity);
                activity.txtTime = activity.FindViewById<TextView>(Resource.Id.txtTime);
                activity.txtCelsius = activity.FindViewById<TextView>(Resource.Id.txtCelsius);
                activity.imgView = activity.FindViewById<ImageView>(Resource.Id.imageView);

                activity.txtCity.Text = $"{openWeatherMap.name},{openWeatherMap.sys.country}";
                activity.txtLastUpdate.Text = $"Last Updated:{DateTime.Now.ToString("dd MMMM yyyy HH: mm ")}";
                activity.txtHumidity.Text = $"Humidity: {openWeatherMap.main.humidity} %";
                activity.txtTime.Text = $"{Common.UnixTimeStampToDateTime(openWeatherMap.sys.sunrise).ToString("HH: mm ")}/" +
                                   $"{Common.UnixTimeStampToDateTime(openWeatherMap.sys.sunset).ToString("HH: mm ")}";
                activity.txtCelsius.Text = $"{openWeatherMap.main.temp} °C";
                if (!string.IsNullOrEmpty(openWeatherMap.weather[0].icon))
                {
                    Picasso.With(activity.ApplicationContext).Load(Common.GetImage(openWeatherMap.weather[0].icon)).Into(activity.imgView);
                }
            }
        }
    }
}