using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using PocketButler;
using Xamarin.Forms.Platform.Android;
using Xamarin;
using PocketButler.Globals;
using PocketButler.Services;
using Android.Locations;
using Android.Content;
using System.Threading;
using System.Threading.Tasks;
using Gcm.Client;

using Thread = Java.Lang.Thread;
using Xamarin.Forms;
using Xamarin.Facebook;
using System.Collections.Generic;
using System.Globalization;

[assembly:Permission (Name = Android.Manifest.Permission.Internet)]
[assembly:Permission (Name = Android.Manifest.Permission.WriteExternalStorage)]
[assembly:MetaData ("com.facebook.sdk.ApplicationId", Value ="@string/app_id")]

namespace PocketButler.Droid
{
	[Activity (Label = "PocketButler", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, WindowSoftInputMode = Android.Views.SoftInput.AdjustPan, LaunchMode = LaunchMode.SingleTask)]
	public class MainActivity : AndroidActivity, ILocationListener, IPageLoader//, Session.IStatusCallback, Request.IGraphUserCallback
	{
		LocationManager locMgr;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			locMgr = GetSystemService (Context.LocationService) as LocationManager;

			// Forms Initialize
			Xamarin.Forms.Forms.Init (this, bundle);
			FormsMaps.Init (this, bundle);

			// Hide Title Bar
			Forms.SetTitleBarVisibility (AndroidTitleBarVisibility.Never);

			try{
				// Insight Initialize
				Insights.Initialize (Globals.Config.InsightKey, Forms.Context);
			}
			catch (Exception ex) {
			}

			// Load App Data
			App.LoadUserDataFromSetting ();

			// Inject classes needed throughout app such as AppText
			var appText = new AppText ();
			LoginServices.AppText = appText;

			App.PageStarted = PageStartedEvent;
			SetPage(App.ShowSplashPage (this));

			ThreadPool.QueueUserWorkItem (o => RegisterGCM ());

			Thread.IUncaughtExceptionHandler defaultHandler = Thread.DefaultUncaughtExceptionHandler;
			Thread.DefaultUncaughtExceptionHandler = new PBUncaughtExceptionHandler ();
			Utils.SaveDataToSettings ("Active", "1");

//			// Open a FB Session and show login if necessary
//			var permissions = new List<string> { "email", "user_birthday" };
//			Session.OpenActiveSession (this, false, permissions, this);

			Console.WriteLine ("Facebook: OpenActiveSession()");
		}
//
//		#region Facebook
//
//		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
//		{
//			base.OnActivityResult (requestCode, resultCode, data);
//
//			Console.WriteLine ("Facebook: OnActivityResult()");
//			// Relay the result to our FB Session
//			Session.ActiveSession.OnActivityResult (this, requestCode, (int)resultCode, data);
//		}
//
//		public void Call (Session session, SessionState state, Java.Lang.Exception exception)
//		{
//			Console.WriteLine ("Facebook: Call()");
//
//			// Make a request for 'Me' information about the current user
//			if (session.IsOpened)
//				Request.ExecuteMeRequestAsync (session, this);
//		}
//
//		public void OnCompleted (Xamarin.Facebook.Model.IGraphUser user, Response response)
//		{
//			Console.WriteLine ("Facebook: OnCompleted()");
//
//			// 'Me' request callback
//			if (user != null)
//				Console.WriteLine ("GOT USER: " + user.Name);
//			else
//				Console.WriteLine ("Failed to get 'me'!");
//		}
//
//		#endregion

		private void RegisterGCM ()
		{
			try {
				GcmClient.CheckDevice (this);
				GcmClient.CheckManifest (this);
				GcmClient.Register (this, GcmBroadcastReceiver.SENDER_IDS);
			} catch (Exception e) {
				Console.WriteLine ("Register GCM Failed : " + e.Message);
			}
		}

		private void PageStartedEvent ()
		{
			if (App.IsWindowAdjustResize == true)
				Window.SetSoftInputMode (SoftInput.AdjustResize);
			else
				Window.SetSoftInputMode (SoftInput.AdjustPan);
		}

		#region IPageLoader implementation
		public void StartApp()
		{
			App._NavigationPage = new NavigationPage (App.GetFirstShowPage (this));
			SetPage (App._NavigationPage);
		}

		public void ShowTypeSliderPage(RestaurantInfo info)
		{
			App._NavigationPage = new NavigationPage (App.GetTypeSliderPage (this, info));
			SetPage (App._NavigationPage);
		}

		public void ShowMainPage ()
		{
			if (Globals.Config.IsGuestMode == false) {
				var userInfo = new Dictionary<string, string> {
					{ Insights.Traits.Email, Globals.Config.USER_EMAIL },
					{ "Last Login", Globals.Config.LOGIN_TIME.ToString () },
				};

				Insights.Identify ((Globals.Config.USER_INFO != null) ? Globals.Config.USER_INFO.user_id : "", userInfo);
			}

			if (Globals.Config.IsGuestMode == false &&
				Globals.Config.IsSliderPageOut == true &&
				Globals.Config.SliderPageRestaurant != null)
				App._NavigationPage = new NavigationPage (App.GetTypeSliderPage(this, Globals.Config.SliderPageRestaurant));
			else
				App._NavigationPage = new NavigationPage (App.GetMainPage (this));

			Globals.Config.IsSliderPageOut = false;
			SetPage (App._NavigationPage);
		}

		public void Logout ()
		{
			App._NavigationPage = new NavigationPage (App.GetLoginPage (this));
			SetPage (App._NavigationPage); 
		}

		public void FacebookLogin()
		{
			var fbAuth = new Intent (this, typeof (FBAuthActivity));
			StartActivityForResult (fbAuth, 0);
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			if (data != null) {
				var email = data.GetStringExtra ("Email");
				var firstName = data.GetStringExtra ("FirstName");
				var lastName = data.GetStringExtra ("LastName");
				var facebookId = data.GetStringExtra ("FacebookId");

				var dob = data.GetStringExtra ("DateOfBirth");
				DateTime? dateOfBirth = null;

				if (!String.IsNullOrEmpty (dob)) {
					DateTime parsedDate;
					if (DateTime.TryParseExact (dob, "MM/dd/yyyy", null, 
						DateTimeStyles.None, out parsedDate))
						dateOfBirth = parsedDate;
				}

				Globals.Config.IsGuestMode = false;
				Globals.Config.USER_EMAIL = email;
				//if (App._FacebookPage != null)
				//	App._FacebookPage.CompletedEvent ();

				App._NavigationPage = new NavigationPage (App.GetLoginPage (null, email, firstName, lastName, dateOfBirth, facebookId));
				SetPage (App._NavigationPage);
			}
		}

		public void StartIntent(String newUrl)
		{
			if (newUrl.StartsWith("openurl::"))
				newUrl = newUrl.Replace("openurl::", "");

			var uri = Android.Net.Uri.Parse (newUrl);
			var intent = new Intent (Intent.ActionView, uri); 
			StartActivity (intent); 
		}

		#endregion

		protected override void OnResume ()
		{
			base.OnResume ();

			Criteria locationCriteria = new Criteria ();

			locationCriteria.Accuracy = Accuracy.Coarse;
			locationCriteria.PowerRequirement = Power.Low;
			locationCriteria.Accuracy = Accuracy.NoRequirement;

			string locationProvider = locMgr.GetBestProvider (locationCriteria, true);
			/*string gpsProvider = LocationManager.GpsProvider;
			string networkProvider = LocationManager.NetworkProvider;

			if (locMgr.IsProviderEnabled (networkProvider)) {
				locMgr.RequestLocationUpdates (networkProvider, 1, 1, this);
			} else if (locMgr.IsProviderEnabled (gpsProvider)) {
				locMgr.RequestLocationUpdates (gpsProvider, 1, 1, this);
			}*/
			if (String.IsNullOrEmpty (locationProvider)) {
				AlertDialog.Builder builder = new AlertDialog.Builder (this);
				builder.SetTitle ("Location Services Not Active");
				builder.SetMessage ("Please enable Location Services and GPS");
				builder.SetPositiveButton ("OK", delegate(object sender, DialogClickEventArgs e) {
					// Show location settings when the user acknowledges the alert dialog
					Intent intent = new Intent (Android.Provider.Settings.ActionLocationSourceSettings);
					StartActivity (intent);
				});
				builder.SetNegativeButton ("Cancel", delegate(object sender, DialogClickEventArgs e) {
				});

				Dialog alertDialog = builder.Create ();
				alertDialog.SetCanceledOnTouchOutside (false);
				alertDialog.Show ();
				//Log.Info(tag, "No location providers available");
			} else {
				locMgr.RequestLocationUpdates (locationProvider, 1000, 1, this);
			}
		}

		public void OnProviderEnabled (string provider)
		{
		}

		public void OnProviderDisabled (string provider)
		{
		}

		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{
		}

		public void OnLocationChanged (Android.Locations.Location location)
		{
			App.UserLatitude = location.Latitude;
			App.UserLongitude = location.Longitude;
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			locMgr.RemoveUpdates (this);
		}

		public override async void OnBackPressed ()
		{
			bool isAllow = true;
			if (App.BackButtonPressedEvent != null)
				isAllow = await App.BackButtonPressedEvent.Invoke ();

			if (isAllow)
				base.OnBackPressed ();
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			Utils.SaveDataToSettings ("Active", "0");
		}

		private class PBUncaughtExceptionHandler : Java.Lang.Object, Java.Lang.Thread.IUncaughtExceptionHandler
		{
			public void UncaughtException (Thread thread, Java.Lang.Throwable throwable)
			{
				Utils.SaveDataToSettings ("Active", "0");
			}
		}
	}
}

