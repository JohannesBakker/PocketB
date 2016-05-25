
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
using PocketButler;
using Xamarin.Forms;
using Xamarin.Facebook.Widget;
using Xamarin.Facebook;
using Xamarin.Facebook.Model;
using PocketButler.Droid;

[assembly: ExportRenderer (typeof(FacebookLoginButton), typeof(FacebookLoginButtonRenderer))]
namespace PocketButler.Droid
{
	public class FacebookLoginButtonRenderer : ViewRenderer
	{

		LoginButton FbLoginButton;

		protected override void OnElementChanged (ElementChangedEventArgs<Xamarin.Forms.View> e)
		{
			base.OnElementChanged (e);

			var activity = this.Context as Activity;
			Console.WriteLine ("FacebookLoginButtonRenderer OnElementChanged");

			FbLoginButton = new LoginButton (Context);
			var permissions = new List<string> { "email", "user_birthday" };
			FbLoginButton.SetReadPermissions (permissions);

			FbLoginButton.UserInfoChangedCallback = new MyUserInfoChangedCallback (activity);

			SetNativeControl (FbLoginButton);
	}


		private Session.IStatusCallback callback;

		class MyUserInfoChangedCallback : Java.Lang.Object, LoginButton.IUserInfoChangedCallback
		{
			Activity owner;

			public MyUserInfoChangedCallback (Activity owner)
			{
				this.owner = owner;
			}

			public void OnUserInfoFetched (IGraphUser user)
			{
				Console.WriteLine("User: {0}", user.FirstName);
			}
		}
	}

}

