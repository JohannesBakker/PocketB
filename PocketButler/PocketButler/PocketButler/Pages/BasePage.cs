using System;
using Xamarin.Forms;
using PocketButler.Controls;
using Android.App;
using System.Threading.Tasks;
using Xamarin;

namespace PocketButler
{
	public class BasePage : ContentPage
    {
        #region PUBLIC MEMBERS
        public MasterDetailPage MasterPage { get; set; }
        public Color CustomNavBarBackColor { get; set; }
        public Color CustomNavTextColor { get; set; }

		public bool IsAllowBackAction { get; set; }
		public bool IsDisableBackButton { get; set; }
		public bool IsCustomizeBackAction { get; set; }
		public bool IsRootPageBackAction { get; set; }
		public Action BackAppearingEvent { get; set; }
		public Action PageShowingEvent { get; set; }

		public static bool IsDoingBackAction = false;
        #endregion

		public BasePage (bool IsMenuPage = false)
		{
			IsAllowBackAction = (IsMenuPage == false);
            CustomNavTextColor = Color.White;
            CustomNavBarBackColor = Color.FromRgb(35, 35, 35);

			IsCustomizeBackAction = false;
			PageShowingEvent += OnPageShowing;
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			if (IsAllowBackAction == true)
				OnPageShowing ();
		}

		public virtual void OnPageShowing()
		{
			try{
				if (IsAllowBackAction == true) {
					if (App.PageStarted != null)
						App.PageStarted.Invoke ();
					App.BackButtonPressedEvent = OnBackPressed;
				}
			}
			catch (Exception ex) {
				Insights.Report (ex);
			}
		}

		#region NAVIGATION BAR
        public void HideNavigationBar()
        {
            NavigationPage.SetHasNavigationBar(this, false);
        }

		public void MakeCustomNavigationBar(AbsoluteLayout mainLayout, View leftView, View rightView, bool IsMenuImage = false)
        {
            HideNavigationBar();

            var NavBarTitleStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 0,
                Padding = 0,
                BackgroundColor = CustomNavBarBackColor,
                Children = {
					new DILabel
					{
						Text = Title,
						TextColor = CustomNavTextColor,
						Font = Font.SystemFontOfSize(Globals.Config.TitleHeaderFontSize, FontAttributes.Bold),
						HorizontalOptions = LayoutOptions.FillAndExpand,
						VerticalOptions = LayoutOptions.FillAndExpand,
						XAlign = TextAlignment.Center,
						YAlign = TextAlignment.Center
					}
				}
            };

            AbsoluteLayout.SetLayoutFlags(NavBarTitleStack, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(NavBarTitleStack, new Rectangle(0, 0, 1, 0.08));
            mainLayout.Children.Add(NavBarTitleStack);

            if (leftView != null)
            {
                leftView.HorizontalOptions = LayoutOptions.Start;
                var NavBarLeftStack = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 0,
					Padding = (IsMenuImage ? 0 : new Thickness(10, 3, 10, 3)),
                    BackgroundColor = Color.Transparent,
                    Children = { leftView }
                };

                AbsoluteLayout.SetLayoutFlags(NavBarLeftStack, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(NavBarLeftStack, new Rectangle(0, 0, 1, 0.08));
                mainLayout.Children.Add(NavBarLeftStack);
            }

            if (rightView != null)
            {
                rightView.HorizontalOptions = LayoutOptions.EndAndExpand;
                var NavBarRightStack = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 0,
					Padding = new Thickness(10, 3, 10, 3),
                    BackgroundColor = Color.Transparent,
                    Children = { rightView }
                };

                AbsoluteLayout.SetLayoutFlags(NavBarRightStack, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(NavBarRightStack, new Rectangle(0, 0, 1, 0.08));
                mainLayout.Children.Add(NavBarRightStack);
            }
        }

		public void MakeCustomNavigationBar(AbsoluteLayout mainLayout, View leftView, View rightView, View centerView, bool IsMenuImage = false)
        {
            HideNavigationBar();

            var NavBarTitleStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 0,
				Padding = 10,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = CustomNavBarBackColor,
                Children = { centerView }
            };

            AbsoluteLayout.SetLayoutFlags(NavBarTitleStack, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(NavBarTitleStack, new Rectangle(0, 0, 1, 0.08));
            mainLayout.Children.Add(NavBarTitleStack);

            if (leftView != null)
            {
                leftView.HorizontalOptions = LayoutOptions.Start;
                var NavBarLeftStack = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 0,
					Padding = (IsMenuImage ? 0 : new Thickness(10, 3, 10, 3)),
                    BackgroundColor = Color.Transparent,
                    Children = { leftView }
                };

                AbsoluteLayout.SetLayoutFlags(NavBarLeftStack, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(NavBarLeftStack, new Rectangle(0, 0, 1, 0.08));
                mainLayout.Children.Add(NavBarLeftStack);
            }

            if (rightView != null)
            {
                rightView.HorizontalOptions = LayoutOptions.EndAndExpand;
                var NavBarRightStack = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 0,
					Padding = new Thickness(10, 3, 10, 3),
                    BackgroundColor = Color.Transparent,
                    Children = { rightView }
                };

                AbsoluteLayout.SetLayoutFlags(NavBarRightStack, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(NavBarRightStack, new Rectangle(0, 0, 1, 0.08));
                mainLayout.Children.Add(NavBarRightStack);
            }
        }
		#endregion

		#region PAGE LOADING
#if __ANDROID__
		ProgressDialog p = null;
		public void ShowLoading()
		{
			p = new ProgressDialog(Forms.Context);
			p.SetMessage("Loading...");
			p.SetCancelable (false);
			p.Show();
		}

		public void HideLoading()
		{
			if (p != null)
			{
				p.Dismiss();
				p = null;
			}
		}
#endif
		#endregion

		#region USER INFO
		public String GetUserID()
		{
			if (Globals.Config.USER_INFO != null)
				return Globals.Config.USER_INFO.user_id;

			return "";
		}

		public String GetUserToken()
		{
			if (Globals.Config.USER_INFO != null)
				return Globals.Config.USER_INFO.user_token;

			return "";
		}

		public String GetUserImage()
		{
			if (Globals.Config.USER_INFO != null)
				return Globals.Config.USER_INFO.user_image;

			return "";
		}

		public String GetUserEmail()
		{
			if (Globals.Config.USER_INFO != null)
				return Globals.Config.USER_INFO.user_email;

			return "";
		}

		public String GetUserFirstName()
		{
			if (Globals.Config.USER_INFO != null)
				return Globals.Config.USER_INFO.user_first_name;

			return "";
		}

		public String GetUserLastName()
		{
			if (Globals.Config.USER_INFO != null)
				return Globals.Config.USER_INFO.user_last_name;

			return "";
		}

		public String GetUserDob()
		{
			if (Globals.Config.USER_INFO != null)
				return Globals.Config.USER_INFO.user_dob;

			return "";
		}

		public void SetUserFirstName(String firstName)
		{
			if (Globals.Config.USER_INFO != null) {
				Globals.Config.USER_INFO.user_first_name = firstName;
				Utils.SaveDataToSettings ("user_info_first_name", Globals.Config.USER_INFO.user_first_name);
			}
		}

		public void SetUserLastName(String lastName)
		{
			if (Globals.Config.USER_INFO != null) {
				Globals.Config.USER_INFO.user_last_name = lastName;
				Utils.SaveDataToSettings ("user_info_last_name", Globals.Config.USER_INFO.user_last_name);
			}
		}

		public void SetUserDob(String dob)
		{
			if (Globals.Config.USER_INFO != null) {
				Globals.Config.USER_INFO.user_dob = dob;
				Utils.SaveDataToSettings ("user_info_dob", Globals.Config.USER_INFO.user_dob);
			}
		}

		public void SetUserImage(String imageurl)
		{
			if (Globals.Config.USER_INFO != null) {
				Globals.Config.USER_INFO.user_image = imageurl;
				Utils.SaveDataToSettings ("user_info_image", Globals.Config.USER_INFO.user_image);
			}
		}

		public ContactAddress GetUserContactAddr()
		{
			if (Globals.Config.USER_INFO != null && Globals.Config.USER_INFO.contact_address != null)
				return Globals.Config.USER_INFO.contact_address;

			return new ContactAddress();
		}

		public void SetUserContactAddr(ContactAddress contactAddr)
		{
			if (Globals.Config.USER_INFO != null) {
				Globals.Config.USER_INFO.contact_address = contactAddr;
				Utils.SaveDataToSettings ("user_contact_city_name", Globals.Config.USER_INFO.contact_address.city_name);
				Utils.SaveDataToSettings ("user_contact_country_name", Globals.Config.USER_INFO.contact_address.country_name);
				Utils.SaveDataToSettings ("user_contact_postal_code", Globals.Config.USER_INFO.contact_address.postal_code);
				Utils.SaveDataToSettings ("user_contact_state_name", Globals.Config.USER_INFO.contact_address.state_name);
				Utils.SaveDataToSettings ("user_contact_street_name", Globals.Config.USER_INFO.contact_address.street_name);
			}
		}
		#endregion

		public void Show_LearnAboutPage()
		{
			Device.OpenUri(new Uri(Globals.Config.kWebsite_Url));
		}

		public async Task<bool> OnBackPressed()
		{
			if (IsDoingBackAction == true)
				return false;

			IsDoingBackAction = true;
			try{
				if (IsRootPageBackAction)
				{
					IsDoingBackAction = false;
					return true;
				}

				if (IsDisableBackButton == true)
				{
					IsDoingBackAction = false;
					return false;
				}

				if (IsCustomizeBackAction == true) {
					//if (App.RootPagePageShowingEvent != null)
					//	App.RootPagePageShowingEvent.Invoke ();
					//await Navigation.PopToRootAsync ();
					App.PageLoaderManager.ShowTypeSliderPage(Globals.Config.CurrentVenue);
				} else {
					if (BackAppearingEvent != null)
						BackAppearingEvent.Invoke ();
					await Navigation.PopAsync ();
				}
			}
			catch (Exception ex) {
				Insights.Report (ex);
			}

			IsDoingBackAction = false;
			return false;
		}
	}
}

