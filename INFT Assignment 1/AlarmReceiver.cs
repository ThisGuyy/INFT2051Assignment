using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace INFT_Assignment_1
{
    [BroadcastReceiver(Enabled = true)]
    public class AlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Toast.MakeText(context, "THIS IS MY ALARM", ToastLength.Long).Show();
            NotificationCompat.Builder builder = new NotificationCompat.Builder(context, "my_channel_01")
            .SetAutoCancel(true)
            .SetContentTitle("Alarm!")
            .SetSmallIcon(Resource.Drawable.alarm)
            .SetContentText("Your alarm went off!")
            .SetContentInfo("Info");
            //using this will bug the program
            //.SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Ringtone)); // Make phone ring like a call

            NotificationManager manager = (NotificationManager)context.GetSystemService(Context.NotificationService);
            manager.Notify(234, builder.Build());


            //Intent i = new Intent(this, typeof(AlarmActivated));
            intent.SetClass(Android.App.Application.Context, typeof(AlarmActivated));
            intent.SetFlags(ActivityFlags.NewTask);
            context.StartActivity(intent);


        }
    }
}