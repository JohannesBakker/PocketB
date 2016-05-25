using System;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace PocketButler
{
	public class LoginViewModel : ViewModelBase
	{
		public LoginViewModel () : base()
		{
		}

		public const string EmailPropertyName = "Email";
		public string Email { get; set; }

		private Command _guestCommand;
		public const string GuestCommandPropertyName = "GuestCommand";
		public Command GuestCommand
		{
			get
			{
				return _guestCommand ?? (_guestCommand = new Command(() => ExecuteGuestCommand()));
			}
		}

		protected async void ExecuteGuestCommand()
		{
			App.IsWindowAdjustResize = false;
			Globals.Config.IsGuestMode = true;
			App.PageLoaderManager.ShowMainPage ();
        }
	}
}

