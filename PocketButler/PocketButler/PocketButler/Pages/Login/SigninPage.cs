using System;
using Xamarin.Forms;
using PocketButler.Controls;
using PocketButler.Services;

namespace PocketButler
{
	public class SigninPage : BasePage
	{
		protected AbsoluteLayout UILayout { get; set; }
		protected AbsoluteLayout MainLayout { get; set; }

		DILabel TopLabel;
        DILabel UserEmailLabel;
        
        CustomLabel BottomHintLabel;

		CustomLabel RecoverPwdLabel;
		Button SigninButton;

        CustomEntry PwdEntry;
        DarkIceImage PwdImage;

		new DarkIceImage BackgroundImage;

		public SigninPage(Action RefreshEvent)
		{
			Title = "Sign In";
			BackAppearingEvent = RefreshEvent;

			BindingContext = new LoginViewModel ();

			UILayout = new AbsoluteLayout
			{
				VerticalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.Transparent
			};

			MainLayout = new AbsoluteLayout {
				VerticalOptions = LayoutOptions.Fill
			};

			BuildUI ();
		}

		#region PRIVATE METHODS
		private void BuildUI()
		{
			BackgroundImage = new DarkIceImage {
				VerticalOptions = LayoutOptions.Fill,
				HorizontalOptions = LayoutOptions.Fill,
				Source = ImageSource.FromFile ("welcome_background.png"),
				Aspect = Aspect.Fill,
			};

			AbsoluteLayout.SetLayoutFlags(BackgroundImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(BackgroundImage, new Rectangle(0, 0, 1, 1));
			UILayout.Children.Add(BackgroundImage);

			// Register Custom Navigation Bar
			MakeCustomNavigationBar(UILayout, null, null);

			//AbsoluteLayout.SetLayoutFlags (BackgroundImage, AbsoluteLayoutFlags.All);
			//AbsoluteLayout.SetLayoutBounds (BackgroundImage, new Rectangle (0, 0, 1, 1));
			//MainLayout.Children.Add (BackgroundImage);

			TopLabel = new DILabel
			{
				Text = "Sign in as:",
				TextColor = Color.White
			};
			TopLabel.SetBoldFont (NamedSize.Medium);

			AbsoluteLayout.SetLayoutFlags (TopLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (TopLabel, new Rectangle (0.2, 0.05, 0.8, 0.09));
			MainLayout.Children.Add (TopLabel);

			UserEmailLabel = new DILabel
			{
				Text = Globals.Config.USER_EMAIL,
				TextColor = Color.White
			};
            UserEmailLabel.SetBoldFont(NamedSize.Medium);

            AbsoluteLayout.SetLayoutFlags(UserEmailLabel, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(UserEmailLabel, new Rectangle(0.2, 0.1, 0.8, 0.09));
            MainLayout.Children.Add(UserEmailLabel);

			BottomHintLabel = new CustomLabel {
				Text = "Learn about Pocket Butler",
				TextColor = Color.White,
				XAlign = TextAlignment.Center,
				Font = Font.SystemFontOfSize (NamedSize.Small),
				IsUnderline = true
			};

			BottomHintLabel.Tapped += Show_LearnAboutPage;

			AbsoluteLayout.SetLayoutFlags (BottomHintLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (BottomHintLabel, new Rectangle (0.5, 0.93, 0.8, 0.03));
			MainLayout.Children.Add (BottomHintLabel);

			var PwdBackButton = new Button {
				Text = "",
				BackgroundColor = Color.Transparent,
				BorderColor = Color.FromRgb (145, 145, 145),
				BorderRadius = 1,
				BorderWidth = 1
			};
            AbsoluteLayout.SetLayoutFlags(PwdBackButton, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(PwdBackButton, new Rectangle(0.5, 0.2, 1, 0.08));
            MainLayout.Children.Add(PwdBackButton);

			PwdImage = new DarkIceImage{
				Source = ImageSource.FromFile("signin_password.png"),
				Aspect = Aspect.AspectFit
			};
            AbsoluteLayout.SetLayoutFlags(PwdImage, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(PwdImage, new Rectangle(0.04, 0.2, 0.08, 0.07));
            MainLayout.Children.Add(PwdImage);

			PwdEntry = new CustomEntry{ 
				Placeholder = "Password",
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				HasBorder = false,
                PlaceholderTextColor = Color.Gray,
				IsPassword = true,
			};
            AbsoluteLayout.SetLayoutFlags(PwdEntry, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(PwdEntry, new Rectangle(1, 0.21, 0.86, 0.05));
            MainLayout.Children.Add(PwdEntry);

            RecoverPwdLabel = new CustomLabel
            {
				Text = "Forgot Password?",
				Font = Font.SystemFontOfSize(NamedSize.Medium),
				TextColor = Color.FromRgb(170, 170, 170),
				YAlign = TextAlignment.Center,
                IsUnderline = true
			};

            RecoverPwdLabel.Tapped += OnRecoverPasswordClicked;
            AbsoluteLayout.SetLayoutFlags(RecoverPwdLabel, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(RecoverPwdLabel, new Rectangle(0.4, 0.3, 0.7, 0.07));
            MainLayout.Children.Add(RecoverPwdLabel);

			SigninButton = new Button {
				Text = "Sign In",
				TextColor = Color.FromRgb(171, 146, 91),
                Font = Font.SystemFontOfSize(NamedSize.Medium),
				BackgroundColor = Color.Transparent
			};
			SigninButton.Clicked += delegate {
				SigninButton_Clicked();
			};
            AbsoluteLayout.SetLayoutFlags(SigninButton, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(SigninButton, new Rectangle(0.93, 0.3, 0.2, 0.07));
            MainLayout.Children.Add(SigninButton);

			AbsoluteLayout.SetLayoutFlags(MainLayout, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(MainLayout, new Rectangle(0, 0.7, 1, 0.9));
			UILayout.Children.Add(MainLayout);

			Content = UILayout;
        }

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			PwdEntry.Focus ();
		}
        #endregion

        #region EVENT METHODS
        async void OnRecoverPasswordClicked()
        {
			ShowLoading ();

			// Check if user is registered or not
			var response = await UserServices.ForgetPassword (Globals.Config.USER_EMAIL);

			HideLoading ();

			DisplayAlert ("Result", response.message, "OK");
        }

		async void SigninButton_Clicked()
		{
			ShowLoading ();

			var device_token = Utils.LoadDataFromSettings ("device_token");

			// Check if user is registered or not
			var response = await LoginServices.AuthenticateUser (Globals.Config.USER_EMAIL, PwdEntry.Text, device_token, "Android");

			HideLoading ();

			bool isLoginSuccess = LoginServices.HasSuccessResult (response.result);
			if (isLoginSuccess) {
				Globals.Config.LOGIN_TIME = DateTime.Now;
				Globals.Config.USER_INFO = response.result.userinfo;

				if (Globals.Config.USER_INFO.contact_address != null) {
					Utils.SaveDataToSettings ("user_contact_city_name", Globals.Config.USER_INFO.contact_address.city_name);
					Utils.SaveDataToSettings ("user_contact_country_name", Globals.Config.USER_INFO.contact_address.country_name);
					Utils.SaveDataToSettings ("user_contact_postal_code", Globals.Config.USER_INFO.contact_address.postal_code);
					Utils.SaveDataToSettings ("user_contact_state_name", Globals.Config.USER_INFO.contact_address.state_name);
					Utils.SaveDataToSettings ("user_contact_street_name", Globals.Config.USER_INFO.contact_address.street_name);
				}

				Utils.SaveDataToSettings ("user_info_dob", Globals.Config.USER_INFO.user_dob);
				Utils.SaveDataToSettings ("user_info_email", Globals.Config.USER_INFO.user_email);
				Utils.SaveDataToSettings ("user_info_first_name", Globals.Config.USER_INFO.user_first_name);
				Utils.SaveDataToSettings ("user_info_last_name", Globals.Config.USER_INFO.user_last_name);
				Utils.SaveDataToSettings ("user_info_id", Globals.Config.USER_INFO.user_id);
				Utils.SaveDataToSettings ("user_info_image", Globals.Config.USER_INFO.user_image);
				Utils.SaveDataToSettings ("user_info_password", Globals.Config.USER_INFO.user_password);
				Utils.SaveDataToSettings ("user_info_token", Globals.Config.USER_INFO.user_token);

				App.PageLoaderManager.ShowMainPage ();
			}
			else
				await DisplayAlert ("Login Failure", response.result.message, "OK");
		}
        #endregion
    }
}

