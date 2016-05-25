using PocketButler.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using PocketButler.Services;

namespace PocketButler
{
    public class BillingAddressPage : BasePage
    {
        protected AbsoluteLayout UILayout { get; set; }
        protected AbsoluteLayout MainLayout { get; set; }

		DILabel TopUsernameLabel;
		DILabel TopEmailLabel;

        Button UserInfoBackButton;
        CustomEntry StreetEntry;
        CustomEntry CityEntry;
        CustomEntry StateEntry;
		CustomEntry PostalCodeEntry;
		CustomEntry CountryEntry;

		bool IsFromProfile = false;

		new DarkIceImage BackgroundImage;

		public BillingAddressPage(Action RefreshEvent, bool isFromProfile = false)
        {
			App.IsWindowAdjustResize = true;
			BackAppearingEvent = RefreshEvent;

			IsFromProfile = isFromProfile;
            Title = "Billing Address";

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
			if (IsFromProfile == false) {
				var SkipAddressLabel = new DILabel {
					Text = "Skip",
					TextColor = Color.FromRgb (171, 146, 91),
					BackgroundColor = Color.Transparent,
					VerticalOptions = LayoutOptions.FillAndExpand,
					YAlign = TextAlignment.Center,
					Font = Font.SystemFontOfSize (NamedSize.Medium),
				};

				var SaveAddressLabel = new DILabel {
					Text = "Save",
					TextColor = Color.FromRgb (171, 146, 91),
					BackgroundColor = Color.Transparent,
					VerticalOptions = LayoutOptions.FillAndExpand,
					YAlign = TextAlignment.Center,
					Font = Font.SystemFontOfSize (NamedSize.Medium),
				};
				MakeCustomNavigationBar (UILayout, SkipAddressLabel, SaveAddressLabel);
				SkipAddressLabel.Tapped += SkipButton_Clicked;
				SaveAddressLabel.Tapped += SaveProfileButton_Clicked;
			} else {
				var SaveAddressLabel = new DILabel {
					Text = "Save",
					TextColor = Color.FromRgb (171, 146, 91),
					BackgroundColor = Color.Transparent,
					VerticalOptions = LayoutOptions.FillAndExpand,
					YAlign = TextAlignment.Center,
					Font = Font.SystemFontOfSize (NamedSize.Medium),
				};
				MakeCustomNavigationBar (UILayout, null, SaveAddressLabel);
				SaveAddressLabel.Tapped += SaveProfileButton_Clicked;
			}

			var TopBillingAddressLabel = new DILabel
			{
				Text = "Please enter your billing address:",
				TextColor = Color.White,
				Font = Font.SystemFontOfSize(NamedSize.Medium, FontAttributes.Bold),
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};

			//AbsoluteLayout.SetLayoutFlags(TopBillingAddressLabel, AbsoluteLayoutFlags.All);
			//AbsoluteLayout.SetLayoutBounds(TopBillingAddressLabel, new Rectangle(0.5, 0.05, 0.95, 0.09));
			//MainLayout.Children.Add(TopBillingAddressLabel);

			String hintText = "(This is required for payments." + ((IsFromProfile == false) ? " You can also set it up later from Settings>Profile" : "") + ")";
			var TopHintLabel = new DILabel
			{
				Text = hintText,
				TextColor = Color.White,
				Font = Font.SystemFontOfSize(NamedSize.Medium, FontAttributes.Italic),
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};

			//AbsoluteLayout.SetLayoutFlags(TopHintLabel, AbsoluteLayoutFlags.All);
			//AbsoluteLayout.SetLayoutBounds(TopHintLabel, new Rectangle(0.5, 0.13, 0.95, 0.18));
			//MainLayout.Children.Add(TopHintLabel);
			var TopLabelLayout = new StackLayout {
				Orientation = StackOrientation.Vertical,
				Spacing = 0,
				Padding = 0,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = {
					TopBillingAddressLabel,
					TopHintLabel
				}
			};
			AbsoluteLayout.SetLayoutFlags(TopLabelLayout, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(TopLabelLayout, new Rectangle(0.5, 0.05, 0.95, 0.3));
			MainLayout.Children.Add(TopLabelLayout);

			TopUsernameLabel = new DILabel
			{
				Text = GetUserFirstName() + " " + GetUserLastName(),
				TextColor = Color.White,
				Font = Font.SystemFontOfSize(NamedSize.Medium)
			};
			TopUsernameLabel.SetBoldFont(NamedSize.Large);

			AbsoluteLayout.SetLayoutFlags(TopUsernameLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(TopUsernameLabel, new Rectangle(0.5, 0.27, 0.95, 0.09));
			MainLayout.Children.Add(TopUsernameLabel);

			TopEmailLabel = new DILabel
			{
				Text = GetUserEmail(),
				TextColor = Color.White,
				Font = Font.SystemFontOfSize(NamedSize.Medium)
			};
			TopEmailLabel.SetBoldFont(NamedSize.Large);

			AbsoluteLayout.SetLayoutFlags(TopEmailLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(TopEmailLabel, new Rectangle(0.5, 0.32, 0.95, 0.09));
			MainLayout.Children.Add(TopEmailLabel);

			if (IsFromProfile == true) {
				UserInfoBackButton = new Button {
					Text = "",
					BackgroundColor = Color.FromRgb (35, 35, 35),
					BorderColor = Color.FromRgb (94, 94, 94),
					BorderRadius = 1,
					BorderWidth = 1
				};
				AbsoluteLayout.SetLayoutFlags (UserInfoBackButton, AbsoluteLayoutFlags.All);
				AbsoluteLayout.SetLayoutBounds (UserInfoBackButton, new Rectangle (0.5, 0.6, 1, 0.4));
				MainLayout.Children.Add (UserInfoBackButton);

				StreetEntry = new CustomEntry {
					Placeholder = "Street",
					TextColor = Color.White,
					BackgroundColor = Color.Transparent,
					HasBorder = false,
					PlaceholderTextColor = Color.Gray,
					Text = GetUserContactAddr ().street_name
				};
				AbsoluteLayout.SetLayoutFlags (StreetEntry, AbsoluteLayoutFlags.All);
				AbsoluteLayout.SetLayoutBounds (StreetEntry, new Rectangle (1, 0.4, 0.98, 0.05));
				MainLayout.Children.Add (StreetEntry);

				CityEntry = new CustomEntry {
					Placeholder = "City",
					TextColor = Color.White,
					BackgroundColor = Color.Transparent,
					HasBorder = false,
					PlaceholderTextColor = Color.Gray,
					Text = GetUserContactAddr ().city_name
				};
				AbsoluteLayout.SetLayoutFlags (CityEntry, AbsoluteLayoutFlags.All);
				AbsoluteLayout.SetLayoutBounds (CityEntry, new Rectangle (1, 0.48, 0.98, 0.05));
				MainLayout.Children.Add (CityEntry);

				StateEntry = new CustomEntry {
					Placeholder = "State",
					TextColor = Color.White,
					BackgroundColor = Color.Transparent,
					HasBorder = false,
					PlaceholderTextColor = Color.Gray,
					Text = GetUserContactAddr ().state_name
				};
				AbsoluteLayout.SetLayoutFlags (StateEntry, AbsoluteLayoutFlags.All);
				AbsoluteLayout.SetLayoutBounds (StateEntry, new Rectangle (1, 0.56, 0.98, 0.05));
				MainLayout.Children.Add (StateEntry);

				PostalCodeEntry = new CustomEntry {
					Placeholder = "Postal Code",
					TextColor = Color.White,
					BackgroundColor = Color.Transparent,
					HasBorder = false,
					PlaceholderTextColor = Color.Gray,
					Text = GetUserContactAddr ().postal_code
				};
				AbsoluteLayout.SetLayoutFlags (PostalCodeEntry, AbsoluteLayoutFlags.All);
				AbsoluteLayout.SetLayoutBounds (PostalCodeEntry, new Rectangle (1, 0.65, 0.98, 0.05));
				MainLayout.Children.Add (PostalCodeEntry);

				CountryEntry = new CustomEntry {
					Placeholder = "Country",
					TextColor = Color.White,
					BackgroundColor = Color.Transparent,
					HasBorder = false,
					PlaceholderTextColor = Color.Gray,
					Text = GetUserContactAddr ().country_name
				};
				AbsoluteLayout.SetLayoutFlags (CountryEntry, AbsoluteLayoutFlags.All);
				AbsoluteLayout.SetLayoutBounds (CountryEntry, new Rectangle (1, 0.73, 0.98, 0.05));
				MainLayout.Children.Add (CountryEntry);

				var seperatorLineSt = new DarkIceImage {
					Source = ImageSource.FromFile ("seperator_hor.png"),
					Aspect = Aspect.Fill
				};
				AbsoluteLayout.SetLayoutFlags (seperatorLineSt, AbsoluteLayoutFlags.All);
				AbsoluteLayout.SetLayoutBounds (seperatorLineSt, new Rectangle (0.6, 0.445, 0.95, 0.003));
				MainLayout.Children.Add (seperatorLineSt);

				var seperatorLineCt = new DarkIceImage {
					Source = ImageSource.FromFile ("seperator_hor.png"),
					Aspect = Aspect.Fill
				};
				AbsoluteLayout.SetLayoutFlags (seperatorLineCt, AbsoluteLayoutFlags.All);
				AbsoluteLayout.SetLayoutBounds (seperatorLineCt, new Rectangle (0.6, 0.525, 0.95, 0.003));
				MainLayout.Children.Add (seperatorLineCt);

				var seperatorLineTa = new DarkIceImage {
					Source = ImageSource.FromFile ("seperator_hor.png"),
					Aspect = Aspect.Fill
				};
				AbsoluteLayout.SetLayoutFlags (seperatorLineTa, AbsoluteLayoutFlags.All);
				AbsoluteLayout.SetLayoutBounds (seperatorLineTa, new Rectangle (0.6, 0.605, 0.95, 0.003));
				MainLayout.Children.Add (seperatorLineTa);

				var seperatorLinePc = new DarkIceImage {
					Source = ImageSource.FromFile ("seperator_hor.png"),
					Aspect = Aspect.Fill
				};
				AbsoluteLayout.SetLayoutFlags (seperatorLinePc, AbsoluteLayoutFlags.All);
				AbsoluteLayout.SetLayoutBounds (seperatorLinePc, new Rectangle (0.6, 0.685, 0.95, 0.003));
				MainLayout.Children.Add (seperatorLinePc);
			} else {
				PostalCodeEntry = new CustomEntry {
					Placeholder = "Postal Code",
					TextColor = Color.White,
					BackgroundColor = Color.Transparent,
					HasBorder = false,
					PlaceholderTextColor = Color.Gray,
					Text = "",
				};
				AbsoluteLayout.SetLayoutFlags (PostalCodeEntry, AbsoluteLayoutFlags.All);
				AbsoluteLayout.SetLayoutBounds (PostalCodeEntry, new Rectangle (1, 0.4, 0.98, 0.05));
				MainLayout.Children.Add (PostalCodeEntry);
			}

            AbsoluteLayout.SetLayoutFlags(MainLayout, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(MainLayout, new Rectangle(0, 0.7, 1, 0.9));
            UILayout.Children.Add(MainLayout);

			Content = new ScrollView{ Content = UILayout};
        }
        #endregion

        #region EVENTS
        async void SkipButton_Clicked()
        {
			/*if (BackAppearingEvent != null)
				BackAppearingEvent.Invoke ();
            await Navigation.PopAsync();*/
			ContactAddress contactAddr = new ContactAddress ();
			contactAddr.city_name = "";
			contactAddr.country_name = "";
			contactAddr.postal_code = "";
			contactAddr.state_name = "";
			contactAddr.street_name = "";

			SetUserContactAddr (contactAddr);

			CreatePassCodePage passCodePage = new CreatePassCodePage(PageShowingEvent);
			await Navigation.PushAsync(passCodePage);
        }

        async void SaveProfileButton_Clicked()
        {
			ShowLoading ();

			if (IsFromProfile) {
				// Check if user is registered or not
				var response = await UserServices.UpdateUserBillingAddress (GetUserID (), GetUserToken (), StreetEntry.Text, CityEntry.Text, StateEntry.Text, PostalCodeEntry.Text, CountryEntry.Text);

				HideLoading ();

				bool isUpdateSuccess = UserServices.HasSuccessResult (response);
				if (isUpdateSuccess) {
					await DisplayAlert ("Success", response.message, "OK");

					ContactAddress contactAddr = new ContactAddress ();
					contactAddr.city_name = CityEntry.Text;
					contactAddr.country_name = CountryEntry.Text;
					contactAddr.postal_code = PostalCodeEntry.Text;
					contactAddr.state_name = StateEntry.Text;
					contactAddr.street_name = StreetEntry.Text;

					SetUserContactAddr (contactAddr);
				} else
					await DisplayAlert ("Update Failure", response.message, "OK");
				if (BackAppearingEvent != null)
					BackAppearingEvent.Invoke ();
				await Navigation.PopAsync ();
			} else {
				var response = await UserServices.UpdateUserBillingAddress (GetUserID (), GetUserToken (), "", "", "", PostalCodeEntry.Text, "");

				HideLoading ();
				bool isUpdateSuccess = UserServices.HasSuccessResult (response);
				if (isUpdateSuccess) {
					await DisplayAlert ("Success", response.message, "OK");

					ContactAddress contactAddr = new ContactAddress ();
					contactAddr.city_name = "";
					contactAddr.country_name = "";
					contactAddr.postal_code = PostalCodeEntry.Text;
					contactAddr.state_name = "";
					contactAddr.street_name = "";

					SetUserContactAddr (contactAddr);
				} else
					await DisplayAlert ("Update Failure. Please set it later", response.message, "OK");

				CreatePassCodePage passCodePage = new CreatePassCodePage(PageShowingEvent);
				await Navigation.PushAsync(passCodePage);
			}
        }
        #endregion
    }
}
