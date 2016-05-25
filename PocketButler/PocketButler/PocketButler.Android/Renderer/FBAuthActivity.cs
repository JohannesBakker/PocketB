
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
using Xamarin.Facebook;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ServiceStack.Text;

namespace PocketButler.Droid
{
	[Activity (Label = "Facebook Login")]			
	public class FBAuthActivity : Activity, Session.IStatusCallback, Request.IGraphUserCallback
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			var permissions = new List<string> { "email", "user_birthday" };
			Session.OpenActiveSession (this, true, permissions, this);

		}

		#region Facebook

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			Console.WriteLine ("Facebook: OnActivityResult()");
			// Relay the result to our FB Session
			Session.ActiveSession.OnActivityResult (this, requestCode, (int)resultCode, data);
		}

		public void Call (Session session, SessionState state, Java.Lang.Exception exception)
		{
			Console.WriteLine ("Facebook: Call()");

			// Make a request for 'Me' information about the current user
			if (session.IsOpened)
				Request.ExecuteMeRequestAsync (session, this);
		}

		public void OnCompleted (Xamarin.Facebook.Model.IGraphUser user, Response response)
		{
			Console.WriteLine ("Facebook: OnCompleted()");

			// 'Me' request callback
			if (user != null) {
				Console.WriteLine ("GOT USER: " + user.Name);

				//Intent i = new Intent(this, typeof(OpenCreateAccountActivity));
				Intent i = new Intent ();

				// Parse JSON and get actual email address
				var facebookData = user.InnerJSONObject.ToString().FromJson<FacebookProfileData> ();

				i.PutExtra ("Email", facebookData.email);
				i.PutExtra ("FirstName", user.FirstName);
				i.PutExtra ("LastName", user.LastName);
				i.PutExtra ("DateOfBirth", user.Birthday);
				i.PutExtra ("FacebookId", user.Id);

				SetResult (Result.Ok, i);
				Finish ();
				//this.StartActivity(i);

			} else {
				Console.WriteLine ("Failed to get 'me'!");

				// TODO: Error and go back

			}
		}

		#endregion
	}

}

