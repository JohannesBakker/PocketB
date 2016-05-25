using System.Text;
using System;
using Android.App;
using Android.Content;
using Android.Util;
using Gcm.Client;
using PocketButler;
using System.Threading;
using System.Threading.Tasks;
using Android.Media;
using System.Collections.Generic;
using ServiceStack.Text;
using DSoft.Messaging;
using Android.Support.V4.App;

namespace PocketButler.Droid
{
	[BroadcastReceiver(Permission=Constants.PERMISSION_GCM_INTENTS)]
	[IntentFilter(new string[]{Constants.INTENT_FROM_GCM_MESSAGE}, Categories=new string[]{"com.pocketbutler.pocketbutler"})]
	[IntentFilter(new string[]{Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK}, Categories=new string[]{"com.pocketbutler.pocketbutler"})]
	[IntentFilter(new string[]{Constants.INTENT_FROM_GCM_LIBRARY_RETRY}, Categories=new string[]{"com.pocketbutler.pocketbutler"})]

	public class GcmBroadcastReceiver: GcmBroadcastReceiverBase<PushHandlerService>
	{
		public static string[] SENDER_IDS = new string[]{Globals.Config.GCM_SENDER_ID};

		public const string TAG = "PushSharp-GCM";
	}

	[Service]
	public class PushHandlerService : GcmServiceBase
	{
		public PushHandlerService() : base(GcmBroadcastReceiver.SENDER_IDS){
		}

		const string TAG = "PocketButler";

		protected override void OnRegistered(Context context, string registrationId){
			Log.Verbose (TAG, "GCM Registered : " + registrationId);
			Utils.SaveDataToSettings ("device_token", registrationId);

			UpdateToken (registrationId);
		}

		async void UpdateToken(string registrationId){
			if (Globals.Config.USER_INFO != null && !Globals.Config.USER_INFO.user_id.Equals ("")) {
				var response = await Services.UserServices.UpdateDeviceToken(Globals.Config.USER_INFO.user_id, "Android", registrationId);

				bool isRegisterSuccess = Services.UserServices.HasSuccessResult (response);
				if (isRegisterSuccess) {
					Log.Verbose (TAG, "Update Token Success!");
				} else {
					Log.Verbose (TAG, "Update Token Failed!");
				}
			}
		}

		protected override void OnUnRegistered(Context context, string registrationId){
			Log.Verbose (TAG, "GCM Unregistered : " + registrationId);
		}

		protected override void OnMessage(Context context, Intent intent){
			Log.Info (TAG, "GCM Message Received!");
			var msg = new StringBuilder ();

			try{
				Dictionary<string, string> parts = new Dictionary<string, string>();
				if (intent != null && intent.Extras != null) {
					foreach(var key in intent.Extras.KeySet()){
						parts[key.ToString()] = intent.Extras.Get(key).ToString();
					}
				}

				string message = parts["message"];
				string title = parts["title"];
				string subtitle = parts["subtitle"];
				string tickerText = parts["tickerText"];
				string vibrate = parts["vibrate"];
				string sound = parts["sound"];
				string largeIcon = parts["largeIcon"];
				string smallIcon = parts["smallIcon"];

				/*

				'message' => 'Test Message',
				'title' => 'Pocketbutler',
				'subtitle' => 'Pocketbutler',
				'tickerText' => 'Pocketbutler',
				'vibrate' => 1,
				'sound' => 1,
				'largeIcon' => 'large_icon',
				'smallIcon' => 'small_icon'
*/

				createNotification(title, subtitle, message, tickerText, vibrate, sound, largeIcon, smallIcon);

			}catch(Exception e){
				Console.WriteLine (e.Message);
			}
		}

		protected override bool OnRecoverableError(Context context, string errorId){
			Log.Warn (TAG, "Recoverable Error:" + errorId);
			return base.OnRecoverableError (context, errorId);
		}

		protected override void OnError(Context context, string errorId){
			Log.Error (TAG, "GCM Error:" + errorId);
		}

		void createNotification (string title, string subtitle, string message, string tickertext, string vibrate, string sound, string largeIcon, string smallIcon)
		{


			Intent uiIntent = new Intent (this, typeof(EmptyActivity));

			/*Android.Support.V4.App.TaskStackBuilder stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create (this);
			stackBuilder.AddParentStack (Java.Lang.Class.FromType(typeof(EmptyActivity)));
			stackBuilder.AddNextIntent (uiIntent);


			PendingIntent resultPendingIntent = stackBuilder.GetPendingIntent (0, (int)PendingIntentFlags.UpdateCurrent);*/

			const int pendingIntentId = 0;

			PendingIntent resultPendingIntent = PendingIntent.GetActivity(this, pendingIntentId, uiIntent, PendingIntentFlags.OneShot);

			NotificationCompat.BigTextStyle textStyle = new NotificationCompat.BigTextStyle ();
			textStyle.BigText (message);

			NotificationCompat.Builder builder = new NotificationCompat.Builder (this)
				.SetAutoCancel (true)
				.SetContentIntent (resultPendingIntent)
				.SetContentTitle (title)
				.SetTicker (tickertext)
				//.SetSubText (subtitle)
				.SetContentText (message)
				.SetDefaults (NotificationCompat.DefaultSound | NotificationCompat.DefaultVibrate)
				.SetSmallIcon(Resource.Drawable.ic_stat_icon);

			builder.SetStyle (textStyle);

			if (!String.IsNullOrEmpty(vibrate) && vibrate.Equals ("1")) {
				builder.SetVibrate(new long[] { 500, 500, 500, 500, 500, 500, 500, 500, 500 });
			}

			if (!String.IsNullOrEmpty (sound) && sound.Equals ("1")) {
				builder.SetSound (RingtoneManager.GetDefaultUri (RingtoneType.Notification));
			}

			//Create notification
			NotificationManager notificationManager = GetSystemService (Context.NotificationService) as NotificationManager;

			notificationManager.Notify (1, builder.Build());
		}

	}
}

