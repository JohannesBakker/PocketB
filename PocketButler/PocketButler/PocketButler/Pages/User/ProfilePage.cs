using PocketButler.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using PocketButler.Services;
using PocketButler.Helpers.Media;
using PocketButler.Helpers;
using Xamarin;

namespace PocketButler
{
    public class ProfilePage : BasePage
    {
        protected AbsoluteLayout UILayout { get; set; }
        protected AbsoluteLayout MainLayout { get; set; }
		DarkIceImage UserImage;
        DILabel TopLabel;
        CustomLabel UpdateLabel;

        Button UserInfoBackButton;
        CustomEntry UserFirstNameEntry;
        CustomEntry UserLastNameEntry;
        //CustomEntry UserBirthdayEntry;
		CustomDatePicker UserBirthdayPicker;

		String profileImgFileName = "";

		new DarkIceImage BackgroundImage;

		public ProfilePage(Action RefreshEvent)
        {
			BackAppearingEvent = RefreshEvent;

            Title = "Profile";

            UILayout = new AbsoluteLayout
            {
                VerticalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.Transparent
            };

            MainLayout = new AbsoluteLayout
            {
                VerticalOptions = LayoutOptions.Fill
            };

            BuildUI();
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
            //BackgroundColor = Color.FromRgb(25, 24, 24);
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

			var backgroundColorImage = new DarkIceImage {
				VerticalOptions = LayoutOptions.Fill,
				HorizontalOptions = LayoutOptions.Fill,
				Source = "",
				Aspect = Aspect.Fill,
				BackgroundColor = Color.FromRgb(25, 24, 24)
			};

			AbsoluteLayout.SetLayoutFlags(backgroundColorImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(backgroundColorImage, new Rectangle(0, 0, 1, 1));
			UILayout.Children.Add(backgroundColorImage);

            // Register Custom Navigation Bar
			var SaveProfileLabel = new DILabel {
				Text = "Save",
				TextColor = Color.FromRgb (171, 146, 91),
				BackgroundColor = Color.Transparent,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				Font = Font.SystemFontOfSize (NamedSize.Medium),
			};
            MakeCustomNavigationBar(UILayout, null, SaveProfileLabel);
			SaveProfileLabel.Tapped += SaveProfileButton_Clicked;

			UserImage = new DarkIceImage
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
				Source = GetUserImage(),
                Aspect = Aspect.AspectFit
            };

            AbsoluteLayout.SetLayoutFlags(UserImage, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(UserImage, new Rectangle(0.5, 0.1, 0.5, 0.15));
            MainLayout.Children.Add(UserImage);

			UserImage.Tapped += UserImage_Select;

			TopLabel = new DILabel
			{
				Text = GetUserEmail(),
				TextColor = Color.White
			};
            TopLabel.SetBoldFont(NamedSize.Large);

            AbsoluteLayout.SetLayoutFlags(TopLabel, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(TopLabel, new Rectangle(0.2, 0.32, 0.8, 0.09));
            MainLayout.Children.Add(TopLabel);

			var imgBalloon = new DarkIceImage
			{
				Source = ImageSource.FromFile("sidebar_venue.png")
			};
            AbsoluteLayout.SetLayoutFlags(imgBalloon, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(imgBalloon, new Rectangle(0.05, 0.7, 0.05, 0.04));
            MainLayout.Children.Add(imgBalloon);

			UpdateLabel = new CustomLabel {
				Text = "Update Billing Address",
				TextColor = Color.White,
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				IsUnderline = true,
				YAlign = TextAlignment.Center,
			};
			UpdateLabel.Tapped += UpdateLabel_Clicked;
            AbsoluteLayout.SetLayoutFlags(UpdateLabel, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(UpdateLabel, new Rectangle(0.4, 0.71, 0.7, 0.08));
            MainLayout.Children.Add(UpdateLabel);

			UserInfoBackButton = new Button {
				Text = "",
				BackgroundColor = Color.FromRgb (35, 35, 35),
				BorderColor = Color.FromRgb (94, 94, 94),
				BorderRadius = 1,
				BorderWidth = 1
			};
            AbsoluteLayout.SetLayoutFlags(UserInfoBackButton, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(UserInfoBackButton, new Rectangle(0.5, 0.48, 1, 0.25));
            MainLayout.Children.Add(UserInfoBackButton);

            UserFirstNameEntry = new CustomEntry
            {
                Placeholder = "FirstName",
                TextColor = Color.White,
                BackgroundColor = Color.Transparent,
                HasBorder = false,
                PlaceholderTextColor = Color.Gray,
				Text = GetUserFirstName()
            };
            AbsoluteLayout.SetLayoutFlags(UserFirstNameEntry, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(UserFirstNameEntry, new Rectangle(1, 0.4, 0.98, 0.05));
            MainLayout.Children.Add(UserFirstNameEntry);

            UserLastNameEntry = new CustomEntry
            {
                Placeholder = "LastName",
                TextColor = Color.White,
                BackgroundColor = Color.Transparent,
                HasBorder = false,
                PlaceholderTextColor = Color.Gray,
				Text = GetUserLastName()
            };
            AbsoluteLayout.SetLayoutFlags(UserLastNameEntry, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(UserLastNameEntry, new Rectangle(1, 0.48, 0.98, 0.05));
            MainLayout.Children.Add(UserLastNameEntry);

			DateTime dateTime = DateTime.Now;
			String userdob = GetUserDob().Replace ("-", "/");
			String[] dobArr = userdob.Split ('/');
			//if (DateTime.TryParse (userdob, out dateTime) == false)
			//	dateTime = DateTime.Now;
			try{
				dateTime = new DateTime (int.Parse(dobArr[2]), int.Parse(dobArr[1]), int.Parse(dobArr[0]));
			}
			catch (Exception ex) {
				dateTime = DateTime.Now;
			}

			UserBirthdayPicker = new CustomDatePicker
            {
                BackgroundColor = Color.Transparent,
				Date = dateTime,
				TextColor = Color.White,
				YAlign = TextAlignment.Center,
				Hint = "Date of Birth",
            };
			AbsoluteLayout.SetLayoutFlags(UserBirthdayPicker, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(UserBirthdayPicker, new Rectangle(1, 0.58, 0.98, 0.08));
			MainLayout.Children.Add(UserBirthdayPicker);

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
            AbsoluteLayout.SetLayoutBounds(seperatorLineLN, new Rectangle(0.6, 0.525, 0.95, 0.003));
            MainLayout.Children.Add(seperatorLineLN);

            AbsoluteLayout.SetLayoutFlags(MainLayout, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(MainLayout, new Rectangle(0, 0.7, 1, 0.9));
            UILayout.Children.Add(MainLayout);

			Content = new ScrollView { Content = UILayout };
        }
        #endregion

        #region EVENTS
        async void SaveProfileButton_Clicked()
        {
			ShowLoading ();

			// Check if user is registered or not
			byte[] imgData = null;
			//if (String.IsNullOrEmpty (profileImgFileName) == false)
			//	imgData = ImageHelper.ReadImageFile (profileImgFileName);

			var response = await UserServices.UpdateUserProfile (GetUserID(), GetUserToken(), UserFirstNameEntry.Text, UserLastNameEntry.Text, UserBirthdayPicker.Date.ToString("yyyy-MM-dd"), profileImgFileName);

			HideLoading ();

			bool isUpdateSuccess = UserServices.HasSuccessResult (response);
			if (isUpdateSuccess) {
				await DisplayAlert ("Success", response.message, "OK");

				if (String.IsNullOrEmpty (response.userinfo.user_id))
					response.userinfo.user_id = GetUserID ();
				if (String.IsNullOrEmpty (response.userinfo.user_token))
					response.userinfo.user_token = GetUserToken ();

				Globals.Config.USER_INFO = response.userinfo;
				SetUserFirstName(UserFirstNameEntry.Text);
				SetUserLastName(UserLastNameEntry.Text);
				SetUserDob(UserBirthdayPicker.Date.ToString("g"));
				SetUserImage (response.userinfo.user_image);
			}
			else
				await DisplayAlert ("Update Failure", response.message, "OK");
			if (BackAppearingEvent != null)
				BackAppearingEvent.Invoke ();
			await Navigation.PopAsync();
        }

		void UpdateLabel_Clicked()
		{
			var BillingUpdatePage = new BillingAddressPage (PageShowingEvent, true);
			Navigation.PushAsync (BillingUpdatePage);
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
