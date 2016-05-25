using PocketButler.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using PocketButler.Services;

namespace PocketButler
{
	public class ChangePasswordPage : BasePage
	{
		protected AbsoluteLayout UILayout { get; set; }
		protected AbsoluteLayout MainLayout { get; set; }

		Button UserInfoBackButton;
		CustomEntry OldPasswordEntry;
		CustomEntry NewPasswordEntry;
		CustomEntry ConfirmPasswordEntry;

		public ChangePasswordPage(Action RefreshEvent)
		{
			BackAppearingEvent = RefreshEvent;

			Title = "Change Password";

			UILayout = new AbsoluteLayout
			{
				VerticalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.Transparent
			};

			MainLayout = new AbsoluteLayout {
				VerticalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.Transparent
			};

			BuildUI ();
		}

		#region PRIVATE METHODS
		private void BuildUI()
		{
			// Register Custom Navigation Bar
			var SaveLabel = new DILabel {
				Text = "Save",
				TextColor = Color.FromRgb (171, 146, 91),
				BackgroundColor = Color.Transparent,
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				YAlign = TextAlignment.Center
			};
			MakeCustomNavigationBar(UILayout, null, SaveLabel);

			SaveLabel.Tapped += ChangePasswordButton_Clicked;

			UserInfoBackButton = new Button
			{
				Text = "",
				BackgroundColor = Color.Transparent,
				BorderColor = Color.FromRgb(94, 94, 94),
				BorderRadius = 1,
				BorderWidth = 1
			};
			AbsoluteLayout.SetLayoutFlags(UserInfoBackButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(UserInfoBackButton, new Rectangle(0.5, 0.12, 1, 0.28));
			MainLayout.Children.Add(UserInfoBackButton);

			OldPasswordEntry = new CustomEntry
			{
				Placeholder = "Old Password",
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				HasBorder = false,
				PlaceholderTextColor = Color.Gray,
				IsPassword = true,
			};
			AbsoluteLayout.SetLayoutFlags(OldPasswordEntry, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(OldPasswordEntry, new Rectangle(1, 0.12, 0.98, 0.05));
			MainLayout.Children.Add(OldPasswordEntry);

			NewPasswordEntry = new CustomEntry
			{
				Placeholder = "Password",
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				HasBorder = false,
				PlaceholderTextColor = Color.Gray,
				IsPassword = true,
			};
			AbsoluteLayout.SetLayoutFlags(NewPasswordEntry, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(NewPasswordEntry, new Rectangle(1, 0.21, 0.98, 0.05));
			MainLayout.Children.Add(NewPasswordEntry);

			ConfirmPasswordEntry = new CustomEntry
			{
				Placeholder = "Confirm Password",
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				HasBorder = false,
				PlaceholderTextColor = Color.Gray,
				IsPassword = true,
			};
			AbsoluteLayout.SetLayoutFlags(ConfirmPasswordEntry, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ConfirmPasswordEntry, new Rectangle(1, 0.3, 0.98, 0.05));
			MainLayout.Children.Add(ConfirmPasswordEntry);

			var seperatorLineOldPwd = new DarkIceImage
			{
				Source = ImageSource.FromFile ("seperator_hor.png"),
				Aspect = Aspect.Fill
			};
			AbsoluteLayout.SetLayoutFlags(seperatorLineOldPwd, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(seperatorLineOldPwd, new Rectangle(0.6, 0.18, 0.95, 0.003));
			MainLayout.Children.Add(seperatorLineOldPwd);

			var seperatorLinePassword = new Image
			{
				Source = ImageSource.FromFile("seperator_hor.png"),
				Aspect = Aspect.Fill
			};
			AbsoluteLayout.SetLayoutFlags(seperatorLinePassword, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(seperatorLinePassword, new Rectangle(0.6, 0.27, 0.95, 0.003));
			MainLayout.Children.Add(seperatorLinePassword);

			AbsoluteLayout.SetLayoutFlags(MainLayout, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(MainLayout, new Rectangle(0, 0.7, 1, 0.9));
			UILayout.Children.Add(MainLayout);

			Content = UILayout;
		}
		#endregion

		#region BUTTON CLICKS
		async void ChangePasswordButton_Clicked()
		{
			if (NewPasswordEntry.Text != ConfirmPasswordEntry.Text) {
				await DisplayAlert ("Warning", "Password does not match", "OK");
				return;
			}

			if (NewPasswordEntry.Text == "") {
				await DisplayAlert ("Warning", "Please input password", "OK");
				return;
			}

			ShowLoading ();

			// Check if user is registered or not
			var response = await LoginServices.CheckPassword(GetUserID(), GetUserToken(), OldPasswordEntry.Text);
			bool isSuccess = LoginServices.HasSuccessResult (response);
			if (isSuccess) {
				var responsePwd = await LoginServices.ChangePassword (GetUserID(), GetUserToken(), NewPasswordEntry.Text, OldPasswordEntry.Text);
				bool isSuccessPwd = LoginServices.HasSuccessResult (response);
				if (isSuccessPwd) {
					await DisplayAlert ("Success", "Password is changed", "OK");
					Utils.SaveDataToSettings ("user_info_password", NewPasswordEntry.Text);
				} else {
					await DisplayAlert ("Error", response.message, "OK");
				}
				if (BackAppearingEvent != null)
					BackAppearingEvent.Invoke ();
				await Navigation.PopAsync ();
			} else {
				await DisplayAlert ("Warning", "Password is not correct", "OK");
			}

			HideLoading ();
		}
		#endregion
	}
}
