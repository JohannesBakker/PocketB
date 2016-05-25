
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
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using System.Globalization;

namespace PocketButler.Droid
{
	[Activity (Label = "OpenCreateAccountActivity")]			
	public class OpenCreateAccountActivity : FormsApplicationActivity
	{

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public DateTime? DateOfBirth { get; set; }

		public string EmailAddress { get; set; }


		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Load up register screen and pass in information
			var email = Intent.GetStringExtra ("Email");
			var firstName = Intent.GetStringExtra ("FirstName");
			var lastName = Intent.GetStringExtra ("LastName");
			var facebookId = Intent.GetStringExtra ("FacebookId");

			var dob = Intent.GetStringExtra ("DateOfBirth");
			DateTime? dateOfBirth = null;

			if (!String.IsNullOrEmpty (dob)) {
				DateTime parsedDate;
				if (DateTime.TryParseExact (dob, "MM/dd/yyyy", null, 
					    DateTimeStyles.None, out parsedDate))
					dateOfBirth = parsedDate;
			}

			Globals.Config.IsGuestMode = false;
			Globals.Config.USER_EMAIL = email;
			/*
			var createAccountPage = new CreateAccountPage (null, email, firstName, lastName, dateOfBirth, facebookId);

			App._NavigationPage = new NavigationPage (createAccountPage);

			SetPage (App._NavigationPage);
			*/

			App._NavigationPage = new NavigationPage (App.GetLoginPage (null, email, firstName, lastName, dateOfBirth, facebookId));
			SetPage (App._NavigationPage);

			Finish ();
		}
	}
}

