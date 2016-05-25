using System;
using Xamarin.Forms;
using PocketButler.Controls;
using PocketButler.Helpers.Media;
using PocketButler.Helpers;
using PocketButler.Services;
using Xamarin;
using System.Collections.Generic;

namespace PocketButler
{
	public class CreateAccountPage : BasePage
	{
        protected AbsoluteLayout UILayout { get; set; }
        protected AbsoluteLayout MainLayout { get; set; }
        DarkIceImage UserImage;

        Button UserInfoBackButton;
        CustomEntry UserNameEntry;
        CustomEntry UserEmailEntry;
		CustomSlidePicker UserBirthdayEntry;
        CustomEntry PasswordEntry;
        CustomEntry MobileEntry;

        CustomLabel BottomHintLabel;
        CustomLabel TermsConditionsLabel;

        DarkIceImage CheckOptImage;
        DILabel AcceptTermsLabel;

        bool isRemember;

        new DarkIceImage BackgroundImage;

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public DateTime? DateOfBirth { get; set; }

		public string EmailAddress { get; set; }

		public string FacebookId { get; set; }
		        
		public string profileImgFileName = "";

		public CreateAccountPage(Action RefreshEvent, string email = "", string firstName = "", string lastName = "", DateTime? dob = null, string facebookId = "")
		{
			BackAppearingEvent = RefreshEvent;

			EmailAddress = email;
			FirstName = firstName;
			LastName = lastName;
			DateOfBirth = dob;
			FacebookId = facebookId;

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

		protected override async void OnAppearing ()
		{
			base.OnAppearing ();

			if (String.IsNullOrEmpty (FacebookId) == false) {
				profileImgFileName = await ImageHelper.DownloadUrlImagetoLocal(new Uri ("http://graph.facebook.com/" + FacebookId + "/picture?type=square"));
			}
		}

		public override void OnPageShowing ()
		{
			App.IsWindowAdjustResize = true;
			base.OnPageShowing ();
		}

		protected override void OnDisappearing ()
		{
			App.IsWindowAdjustResize = false;
			base.OnDisappearing ();
		}

        #region PRIVATE METHODS
        private void BuildUI()
        {
            // Set Background Image
            BackgroundImage = new DarkIceImage
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                Source = ImageSource.FromFile("welcome_background.png"),
                Aspect = Aspect.Fill,
            };

            AbsoluteLayout.SetLayoutFlags(BackgroundImage, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(BackgroundImage, new Rectangle(0, 0, 1, 1));
            UILayout.Children.Add(BackgroundImage);

            // Register Custom Navigation Bar
			var CreateAccountLabel = new DILabel {
				Text = "Create Account",
				TextColor = Color.FromRgb (171, 146, 91),
				BackgroundColor = Color.Transparent,
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				YAlign = TextAlignment.Center
			};
			MakeCustomNavigationBar(UILayout, null, CreateAccountLabel);

			CreateAccountLabel.Tapped += CreateAccountButton_Clicked;

            // Make Main UI
            UserImage = new DarkIceImage
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                Source = ImageSource.FromFile("createaccount_profile_pic.png"),
                Aspect = Aspect.AspectFit
            };

			// If we have an image set it
			if (!String.IsNullOrEmpty (FacebookId)) {
				UserImage.Source = ImageSource.FromUri (new Uri ("http://graph.facebook.com/" + FacebookId + "/picture?type=square"));
			}

            UserImage.Tapped += UserImage_Select;

            AbsoluteLayout.SetLayoutFlags(UserImage, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(UserImage, new Rectangle(0.5, 0.1, 0.5, 0.15));
            MainLayout.Children.Add(UserImage);

			var ProfilePictureLabel = new DILabel {
				Text = "Profile Picture",
				TextColor = Color.White,
				XAlign = TextAlignment.Center,
			};
			AbsoluteLayout.SetLayoutFlags (ProfilePictureLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (ProfilePictureLabel, new Rectangle (0.5, 0.3, 0.9, 0.09));
			MainLayout.Children.Add (ProfilePictureLabel);

            UserInfoBackButton = new Button
			{
				Text = "",
				BackgroundColor = Color.Transparent,
				BorderColor = Color.FromRgb(94, 94, 94),
				BorderRadius = 1,
				BorderWidth = 1
			};
            AbsoluteLayout.SetLayoutFlags(UserInfoBackButton, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(UserInfoBackButton, new Rectangle(0.5, 0.6, 1, 0.4));
            MainLayout.Children.Add(UserInfoBackButton);

			String name = "";
			if (String.IsNullOrEmpty (FirstName) == false ||
			    String.IsNullOrEmpty (LastName) == false)
				name = FirstName + " " + LastName;
			
            UserNameEntry = new CustomEntry
            {
                Placeholder = "*Name",
                TextColor = Color.White,
                BackgroundColor = Color.Transparent,
                HasBorder = false,
                PlaceholderTextColor = Color.Gray,
				Text = name,
            };
			AbsoluteLayout.SetLayoutFlags(UserNameEntry, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(UserNameEntry, new Rectangle(1, 0.4, 0.98, 0.05));
			MainLayout.Children.Add(UserNameEntry);

            UserEmailEntry = new CustomEntry
            {
                Placeholder = "*Email Address",
                TextColor = Color.White,
                BackgroundColor = Color.Transparent,
                HasBorder = false,
				PlaceholderTextColor = Color.Gray,
				Text = EmailAddress,
            };
			AbsoluteLayout.SetLayoutFlags(UserEmailEntry, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(UserEmailEntry, new Rectangle(1, 0.482, 0.98, 0.05));
			MainLayout.Children.Add(UserEmailEntry);

			if (DateOfBirth == null) {
				DateOfBirth = new DateTime (DateTime.Now.Year - 20, DateTime.Now.Month, DateTime.Now.Day);//DateTime.Now;
			}			

			UserBirthdayEntry = new CustomSlidePicker
            {
                TextColor = Color.White,
                BackgroundColor = Color.Transparent,
				XAlign = TextAlignment.Start,
            };
            
			List<String> valueArrList = new List<string>();
			for (int i = 18; i <= 99; i++)
				valueArrList.Add(i.ToString());
			UserBirthdayEntry.PickerList = valueArrList;
			UserBirthdayEntry.SelectedIndex = 0;

            AbsoluteLayout.SetLayoutFlags(UserBirthdayEntry, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(UserBirthdayEntry, new Rectangle(1, 0.568, 0.98, 0.05));
            MainLayout.Children.Add(UserBirthdayEntry);
            
            PasswordEntry = new CustomEntry
            {
                Placeholder = "*Password",
                TextColor = Color.White,
                BackgroundColor = Color.Transparent,
                HasBorder = false,
                PlaceholderTextColor = Color.Gray,
				IsPassword = true,
            };
            AbsoluteLayout.SetLayoutFlags(PasswordEntry, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(PasswordEntry, new Rectangle(1, 0.646, 0.98, 0.05));
            MainLayout.Children.Add(PasswordEntry);

            MobileEntry = new CustomEntry
            {
                Placeholder = "*Mobile Number",
                TextColor = Color.White,
                BackgroundColor = Color.Transparent,
                HasBorder = false,
                PlaceholderTextColor = Color.Gray,
            };
			AbsoluteLayout.SetLayoutFlags(MobileEntry, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(MobileEntry, new Rectangle(1, 0.728, 0.98, 0.05));
			MainLayout.Children.Add(MobileEntry);

			var seperatorLineFN = new DarkIceImage
			{
				Source = ImageSource.FromFile ("seperator_hor.png"),
				Aspect = Aspect.Fill
			};
            AbsoluteLayout.SetLayoutFlags(seperatorLineFN, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(seperatorLineFN, new Rectangle(0.6, 0.445, 0.95, 0.003));
            MainLayout.Children.Add(seperatorLineFN);

			var seperatorLineLN = new DarkIceImage
			{
				Source = ImageSource.FromFile("seperator_hor.png"),
				Aspect = Aspect.Fill
			};
            AbsoluteLayout.SetLayoutFlags(seperatorLineLN, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(seperatorLineLN, new Rectangle(0.6, 0.522, 0.95, 0.003));
            MainLayout.Children.Add(seperatorLineLN);

            var seperatorLineBirthday = new Image
			{
				Source = ImageSource.FromFile("seperator_hor.png"),
				Aspect = Aspect.Fill
			};
            AbsoluteLayout.SetLayoutFlags(seperatorLineBirthday, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(seperatorLineBirthday, new Rectangle(0.6, 0.599, 0.95, 0.003));
            MainLayout.Children.Add(seperatorLineBirthday);

            var seperatorLinePassword = new Image
			{
				Source = ImageSource.FromFile("seperator_hor.png"),
				Aspect = Aspect.Fill
			};
            AbsoluteLayout.SetLayoutFlags(seperatorLinePassword, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(seperatorLinePassword, new Rectangle(0.6, 0.676, 0.95, 0.003));
            MainLayout.Children.Add(seperatorLinePassword);

			BottomHintLabel = new CustomLabel {
				Text = "Learn about Pocket Butler",
				TextColor = Color.White,
				XAlign = TextAlignment.Center,
				Font = Font.SystemFontOfSize (NamedSize.Small),
				IsUnderline = true
			};

			BottomHintLabel.Tapped += Show_LearnAboutPage;

            AbsoluteLayout.SetLayoutFlags(BottomHintLabel, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(BottomHintLabel, new Rectangle(0.5, 0.93, 0.8, 0.03));
            MainLayout.Children.Add(BottomHintLabel);

			TermsConditionsLabel = new CustomLabel {
				Text = "Terms & Conditions",
				TextColor = Color.FromRgb (171, 146, 91),
				XAlign = TextAlignment.Center,
				Font = Font.SystemFontOfSize (NamedSize.Small),
				IsUnderline = true
			};

			TermsConditionsLabel.Tapped += TermsConditions_Clicked;

            AbsoluteLayout.SetLayoutFlags(TermsConditionsLabel, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(TermsConditionsLabel, new Rectangle(0.7, 0.9, 0.8, 0.03));
            MainLayout.Children.Add(TermsConditionsLabel);

            CheckOptImage = new DarkIceImage
            {
                Source = ImageSource.FromFile("welcome_remember_email_empty.png"),
                Aspect = Aspect.AspectFit
            };
            CheckOptImage.Tapped += OnRememberClicked;
            AbsoluteLayout.SetLayoutFlags(CheckOptImage, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(CheckOptImage, new Rectangle(0.04, 0.85, 0.07, 0.07));
            MainLayout.Children.Add(CheckOptImage);

            AcceptTermsLabel = new DILabel
            {
                Text = "By Creating an account you agree to abide by the PocketButler",
                Font = Font.SystemFontOfSize(NamedSize.Medium),
                TextColor = Color.FromRgb(170, 170, 170),
                YAlign = TextAlignment.Center
            };
            AcceptTermsLabel.Tapped += OnRememberClicked;
            AbsoluteLayout.SetLayoutFlags(AcceptTermsLabel, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(AcceptTermsLabel, new Rectangle(0.8, 0.86, 0.8, 0.09));
            MainLayout.Children.Add(AcceptTermsLabel);

            AbsoluteLayout.SetLayoutFlags(MainLayout, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(MainLayout, new Rectangle(0, 0.7, 1, 0.9));
            UILayout.Children.Add(MainLayout);

			Content = new ScrollView { Content = UILayout };
        }
        private String GetOptImageWithStatus(bool isSelected)
        {
            if (isSelected == true)
                return "welcome_remember_email_checked.png";
            else
                return "welcome_remember_email_empty.png";
        }

        #endregion

        #region BUTTON CLICKS
		async private void TermsConditions_Clicked()
		{
			var TermsConditionsPage = new TermsConditionsPage (PageShowingEvent);
			await Navigation.PushAsync(TermsConditionsPage);
		}

        private async void UserImage_Select()
        {
            var action = await DisplayActionSheet("Select Source", "Cancel", null, "Camera", "Photo Library");
            //nPhotoType = 1;
            if (action == "Camera")
            {
                doCameraPhoto();
            }
            else if (action == "Photo Library")
            {
                doPhotoLibrary();
            }
        }
        private void OnRememberClicked()
        {
            isRemember = !isRemember;

            CheckOptImage.Source = ImageSource.FromFile(GetOptImageWithStatus(isRemember));
        }
        
		async void CreateAccountButton_Clicked()
        {
			if (String.IsNullOrEmpty (UserNameEntry.Text)) {
				await DisplayAlert ("Warning", "Please enter your name", "OK");
				return;
			}

			if (String.IsNullOrEmpty (UserEmailEntry.Text)) {
				await DisplayAlert ("Warning", "Please enter your email", "OK");
				return;
			}

			if (String.IsNullOrEmpty(PasswordEntry.Text) || PasswordEntry.Text.Length < 6) {
				await DisplayAlert ("Warning", "Password must be at least 6 characters long", "OK");
				return;
			}
			/*
			if (PasswordEntry.Text != ConfirmPasswordEntry.Text) {
				await DisplayAlert ("Warning", "Password does not match", "OK");
				return;
			}*/

			if (PasswordEntry.Text == "") {
				await DisplayAlert ("Warning", "Please input password", "OK");
				return;
			}

			if (MobileEntry.Text == "") {
				await DisplayAlert ("Warning", "Please input mobile number", "OK");
				return;
			}

			if (isRemember == false) {
				await DisplayAlert ("Warning", "Please agree to the PocketButler Terms & Conditions", "OK");
				return;
			}

			ShowLoading ();

			var device_token = Utils.LoadDataFromSettings ("device_token");

			// Check if user is registered or not
			var response = await LoginServices.RegisterUser (UserEmailEntry.Text, PasswordEntry.Text, UserNameEntry.Text, "", (UserBirthdayEntry.SelectedIndex + 18).ToString(), MobileEntry.Text, device_token, "Android", profileImgFileName);

			HideLoading ();

			bool isRegisterSuccess = LoginServices.HasSuccessResult (response.result);
			if (isRegisterSuccess) {
				Globals.Config.LOGIN_TIME = DateTime.Now;
				Globals.Config.USER_INFO = response.result.userinfo;

				if (BackAppearingEvent == null) { // comes from facebook login
					//Utils.SaveDataToSettings ("remember_password", (isRemember) ? "1" : "0");
					Utils.SaveDataToSettings ("user_email", Globals.Config.USER_EMAIL);
				}

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

				if (String.IsNullOrEmpty (Globals.Config.USER_INFO.status) == false && Globals.Config.USER_INFO.status.Equals ("1")) {
					BillingAddressPage billingPage = new BillingAddressPage (PageShowingEvent, false);
					await Navigation.PushAsync (billingPage);
				} else {
					VerifyUserPage verifyUserPage = new VerifyUserPage (PageShowingEvent);
					await Navigation.PushAsync (verifyUserPage);
				}
			}
			else
				await DisplayAlert ("Register Failure", response.result.message, "OK");
            
        }
        #endregion

        #region PHOTO_MANAGE
        async void doCameraPhoto()
        {
#if __ANDROID__
            MediaPicker picker = new MediaPicker(Forms.Context);
#else
            MediaPicker picker = new MediaPicker();
#endif

            if (picker.IsCameraAvailable == false)
            {
                var page = new ContentPage();
                var result = page.DisplayAlert("Warning", "Camera is not available", "OK");

                return;
            }
            else
            {
                try
                {
                    var resultfile = await picker.TakePhoto(null);
                    String filename = resultfile.Path;
#if __ANDROID__
                    Android.Graphics.Bitmap _bitmap = ImageHelper.DecodeSampledBitmapFromResource(filename, 300, 300);
                    Byte[] imgBytes = ImageHelper.BitmapToBytes(_bitmap);
					string newfilename = filename.Substring(0, filename.LastIndexOf("."));
					Java.IO.File f = new Java.IO.File(newfilename + "_scaled.png");

                    f.CreateNewFile();

                    //write the bytes in file
                    Java.IO.FileOutputStream fo = new Java.IO.FileOutputStream(f);
                    fo.Write(imgBytes);

                    // remember close de FileOutput
                    fo.Close();

                    UserImage.Source = ImageSource.FromFile(f.AbsolutePath);
					profileImgFileName = f.AbsolutePath;
#else
                    
#endif
                }
                catch (Exception ex)
                {
					Insights.Report (ex);
                }
            }
        }

        async void doPhotoLibrary()
        {
#if __ANDROID__
            MediaPicker picker = new MediaPicker(Forms.Context);
#else
            MediaPicker picker = new MediaPicker();
#endif

            if (picker.IsPhotoGalleryAvailable == false)
            {
                var page = new ContentPage();
                var result = page.DisplayAlert("Warning", "Photo is not available", "OK");

                return;
            }
            else
            {
                try
                {
                    var resultfile = await picker.PickPhoto();
                    String filename = resultfile.Path;
#if __ANDROID__
                    Android.Graphics.Bitmap _bitmap = ImageHelper.DecodeSampledBitmapFromResource(filename, 300, 300);
                    Byte[] imgBytes = ImageHelper.BitmapToBytes(_bitmap);
					string newfilename = filename.Substring(0, filename.LastIndexOf("."));
					Java.IO.File f = new Java.IO.File(newfilename + "_scaled.png");

                    f.CreateNewFile();

                    //write the bytes in file
                    Java.IO.FileOutputStream fo = new Java.IO.FileOutputStream(f);
                    fo.Write(imgBytes);

                    // remember close de FileOutput
                    fo.Close();

                    UserImage.Source = ImageSource.FromFile(f.AbsolutePath);
					profileImgFileName = f.AbsolutePath;
#else
                    
#endif
                }
                catch (Exception ex)
                {
					Insights.Report (ex);
                }
            }
        }
        #endregion
    }
}

