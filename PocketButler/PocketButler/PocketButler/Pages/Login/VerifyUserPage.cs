using System;
using Xamarin.Forms;
using PocketButler.Controls;
using PocketButler.Services;

namespace PocketButler
{
	public class VerifyUserPage : BasePage
	{
		protected AbsoluteLayout MainLayout { get; set; }

		DILabel TopLabel;
		CustomEntry CodeEntry;

		Button ResendButton;
		Button VerifyButton;

		new DarkIceImage BackgroundImage;

		public VerifyUserPage (Action RefreshEvent)
		{
			BackAppearingEvent = RefreshEvent;

			MainLayout = new AbsoluteLayout {
				VerticalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.Transparent
			};

			BuildUI ();
		}

		#region PRIVATE METHODS
		private void BuildUI ()
		{
			BackgroundImage = new DarkIceImage {
				VerticalOptions = LayoutOptions.Fill,
				HorizontalOptions = LayoutOptions.Fill,
				Source = ImageSource.FromFile ("welcome_background.png"),
				Aspect = Aspect.Fill,
			};

			AbsoluteLayout.SetLayoutFlags (BackgroundImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (BackgroundImage, new Rectangle (0, 0, 1, 1));
			MainLayout.Children.Add (BackgroundImage);

			TopLabel = new DILabel {
				Text = "A verification code has been sent to your phone. Please provide it here",
				TextColor = Color.White,
				XAlign = TextAlignment.Start
			};
			TopLabel.SetBoldFont (NamedSize.Medium);

			AbsoluteLayout.SetLayoutFlags (TopLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (TopLabel, new Rectangle (0.5, 0.1, 0.9, 0.09));
			MainLayout.Children.Add (TopLabel);

			var CodeBackButton = new Button {
				Text = "",
				BackgroundColor = Color.Transparent,
				BorderColor = Color.FromRgb (145, 145, 145),
				BorderRadius = 1,
				BorderWidth = 1
			};
			AbsoluteLayout.SetLayoutFlags(CodeBackButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(CodeBackButton, new Rectangle(0.5, 0.2, 1, 0.08));
			MainLayout.Children.Add(CodeBackButton);

			CodeEntry = new CustomEntry { 
				Placeholder = "Verification Code",
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				HasBorder = false,
				PlaceholderTextColor = Color.Gray,
				Text = "",
			};
			AbsoluteLayout.SetLayoutFlags (CodeEntry, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (CodeEntry, new Rectangle (0.5, 0.2, 0.9, 0.07));
			MainLayout.Children.Add (CodeEntry);

			ResendButton = new Button {
				Text = "Resend",
				BackgroundColor = Color.FromRgb (171, 146, 91),
				TextColor = Color.FromRgb (54, 54, 54),
				Font = Font.SystemFontOfSize (NamedSize.Medium)
			};
			ResendButton.Clicked += (sender, e) => {
				ResendButton_Clicked();
			};

			AbsoluteLayout.SetLayoutFlags (ResendButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (ResendButton, new Rectangle (0.07, 0.35, 0.45, 0.07));
			MainLayout.Children.Add (ResendButton);

			VerifyButton = new Button {
				Text = "Verify",
				BackgroundColor = Color.FromRgb (171, 146, 91),
				TextColor = Color.FromRgb (54, 54, 54),
				Font = Font.SystemFontOfSize (NamedSize.Medium)
			};
			VerifyButton.Clicked += (sender, e) => {
				VerifyButton_Clicked();
			};

			AbsoluteLayout.SetLayoutFlags (VerifyButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (VerifyButton, new Rectangle (0.93, 0.35, 0.45, 0.07));
			MainLayout.Children.Add (VerifyButton);

			Content = new ScrollView() { Content = MainLayout };
		}
		#endregion

		private void ResendButton_Clicked()
		{
		}

		private async void VerifyButton_Clicked()
		{
			if (String.IsNullOrEmpty (CodeEntry.Text)) {
				await DisplayAlert ("Error", "Invalid Code", "OK");
				return;
			}

			ShowLoading ();

			// Check if user is registered or not
			var response = await LoginServices.VerifyUser (GetUserID(), CodeEntry.Text);

			HideLoading ();

			bool isRegisterSuccess = LoginServices.HasSuccessResult (response);
			if (isRegisterSuccess) {
				BillingAddressPage billingPage = new BillingAddressPage (PageShowingEvent, false);
				await Navigation.PushAsync (billingPage);
			}
			else
				await DisplayAlert ("Error", "Invalid Code", "OK");
		}
	}
}

