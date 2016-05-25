using PocketButler.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using PocketButler.Services;

namespace PocketButler
{
    public class SettingsPage : BasePage
    {
        protected AbsoluteLayout UILayout { get; set; }

        class SettingsItem
        {
            public int Id;
            public String Text { get; set; }
            public ImageSource Icon { get; set; }
            
            public Utils.ItemClickedDelegate Tapped;

			public SettingsItem(int id, String text, String icon, Utils.ItemClickedDelegate tappedEvent)
            {
                this.Id = id;
                this.Text = text;
                this.Icon = ImageSource.FromFile(icon);
                this.Tapped = tappedEvent;
            }
        }

        List<SettingsItem> SettingsItemList = new List<SettingsItem>();

        ListView SettingsListView;

		public SettingsPage(Action RefreshEvent)
        {
			BackAppearingEvent = RefreshEvent;

            Title = "Settings";

			// Make Settings
			SettingsItemList.Add(new SettingsItem(0, "Profile", "sidebar_profile.png", ProfileItemSelected));
			SettingsItemList.Add(new SettingsItem(1, "Change Pin", "sidebar_changepwd.png", ChangePinItemSelected));
			SettingsItemList.Add(new SettingsItem(2, "Change Password", "sidebar_changepwd.png", ChangePasswordItemSelected));
			SettingsItemList.Add(new SettingsItem(3, "Terms and Conditions", "sidebar_tnc.png", TermsItemSelected));

            UILayout = new AbsoluteLayout
            {
                VerticalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.Transparent
            };

            BuildUI();
        }

        void listView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            SettingsItem item = e.SelectedItem as SettingsItem;

            if (item != null)
            {
                if (item.Tapped != null)
                    item.Tapped.Invoke();

				SettingsListView.SelectedItem = null;
            }
        }

        #region PRIVATE METHODS
        private void BuildUI()
        {
			var MenuImage = new DarkIceImage
			{
				Source = ImageSource.FromFile ("navbar_sidebar.png"),
				Aspect = Aspect.AspectFit,
				IsEnablePadding = true,
			};
            MenuImage.Tapped += MenuButton_Clicked;

			SettingsListView = new ListView
            {
                HasUnevenRows = true,

                // Source of data items.
                ItemsSource = SettingsItemList,

                ItemTemplate = new DataTemplate(() =>
                {
                    // Return an assembled ViewCell.
                    var viewCell = new ViewCell
                    {
                        View = GetSettingItem()
                    };

                    viewCell.Height = 50;
                    return viewCell;
                })
            };

			SettingsListView.ItemSelected += listView_ItemSelected;

			var MainLayout = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Padding = 10
			};
			MainLayout.Children.Add(SettingsListView);

			String versionName = Forms.Context.PackageManager.GetPackageInfo(Forms.Context.PackageName, 0).VersionName;

			MainLayout.Children.Add (
				new Label {
				Text = "Version : " + versionName,
				TextColor = Color.Gray,
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				VerticalOptions = LayoutOptions.End,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				XAlign = TextAlignment.Center
			});

            var scrollView = new ScrollView { Content = MainLayout };
            AbsoluteLayout.SetLayoutFlags(scrollView, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(scrollView, new Rectangle(0, 0.7, 1, 0.9));
            UILayout.Children.Add(scrollView);

            MakeCustomNavigationBar(UILayout, MenuImage, null, true);

            Content = UILayout;
        }

        private StackLayout GetSettingItem()
        {
			var ItemIconImage = new Image
			{
				WidthRequest = 40,
				VerticalOptions = LayoutOptions.FillAndExpand
			};
			ItemIconImage.SetBinding(DarkIceImage.SourceProperty, "Icon");

			var ItemTextLabel = new Label {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				TextColor = Color.White,
				Font = Font.SystemFontOfSize (NamedSize.Medium)
			};
			ItemTextLabel.SetBinding(DILabel.TextProperty, "Text");

            var StackItem = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Padding = 10,
                Spacing = 10,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 50,
                Children =
                {
					ItemIconImage, 
					ItemTextLabel,
					new DarkIceImage {
						Source = ImageSource.FromFile("directionicon.png"),
						HorizontalOptions = LayoutOptions.End,
						VerticalOptions = LayoutOptions.FillAndExpand, WidthRequest = 20
					}
                }
            };

            return StackItem;
        }

        async private void ProfileItemSelected()
        {
			await Navigation.PushAsync(new ProfilePage(PageShowingEvent));
        }

        private void ChangePinItemSelected()
        {
			InputDialogHelper.InputDialogResultEvent = InputDialogResultEventForChangePin;
			InputDialogHelper.ShowInputDialog (Forms.Context, "Please enter PocketButler password to change pin");
        }

        async private void ChangePasswordItemSelected()
        {
			//InputDialogHelper.InputDialogResultEvent = InputDialogResultEventForChangePwd;
			//InputDialogHelper.ShowInputDialog (Forms.Context, "Please enter your new password");
			await Navigation.PushAsync (new ChangePasswordPage (PageShowingEvent));
        }

        async private void TermsItemSelected()
        {
			await Navigation.PushAsync(new TermsConditionsPage(PageShowingEvent));
        }
        #endregion

        #region EVENTS
        void MenuButton_Clicked()
        {
            MasterPage.IsPresented = true;
        }

		async void InputDialogResultEventForChangePin(String resultMsg)
		{
			if (Globals.Config.USER_INFO == null)
				return;

			ShowLoading ();
			var response = await LoginServices.CheckPassword(GetUserID(), GetUserToken(), resultMsg);
			bool isSuccess = LoginServices.HasSuccessResult (response);
			HideLoading ();

			if (isSuccess) {
				Utils.SaveDataToSettings ("FailedCount", "");
				await Navigation.PushAsync (new CreatePassCodePage (PageShowingEvent, true, true));
			} else {
				await DisplayAlert ("Error", "Invalid Password", "OK");
				return;
			}
		}

		void InputDialogResultEventForChangePwd(String resultMsg)
		{
			//InputDialogHelper.InputDialogResultEvent = InputDialogResultEventForConfirmPwd;
			//InputDialogHelper.ShowInputDialog (Forms.Context, "Please re-enter your new password");
		}

		void InputDialogResultEventForConfirmPwd(String resultMsg)
		{
		}
        #endregion
    }
}
