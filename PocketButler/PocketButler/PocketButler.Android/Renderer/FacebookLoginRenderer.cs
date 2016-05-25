using System;
using Xamarin.Forms;
using PocketButler;
using PocketButler.Droid;
using Xamarin.Forms.Platform.Android;
using Android.App;
using Xamarin.Auth;
using AndroidHUD;
using System.Threading.Tasks;
using Android.Content;

[assembly: ExportRenderer (typeof(FacebookLoginPage), typeof(FacebookLoginRenderer))]
namespace PocketButler.Droid
{
	public class FacebookLoginRenderer : PageRenderer//, Session.IStatusCallback, Xamarin.Facebook.Request.IGraphUserCallback
	{
		 protected override void OnAttachedToWindow ()
		{
			base.OnAttachedToWindow ();

		}

		protected override void OnElementChanged (ElementChangedEventArgs<Page> e)
		{
			base.OnElementChanged (e);

			Console.WriteLine ("FacebookLoginRenderer OnElementChanged");

			var fbAuth = new Intent (this.Context, typeof (FBAuthActivity));
			Forms.Context.StartActivity (fbAuth);
		}

	}
}

