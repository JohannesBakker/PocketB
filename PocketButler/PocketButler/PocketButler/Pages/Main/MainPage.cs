using PocketButler.Controls;
using PocketButler.Model;
using System;
using Xamarin.Forms;
using PocketButler.Services;
using DSoft.Messaging;
using System.Threading.Tasks;

namespace PocketButler
{
    public class MainPage : CustomMasterDetailPage
	{
		#region PRIVATE VARIABLES
		private MenuPage pageMasterMenu;

		private VenueListPage pageVenueList;
		MessageBusEventHandler newEvHandler;
		bool IsNewNavigation = true;
		#endregion

		public MainPage (BasePage page = null)
		{
			if (page != null)
				IsNewNavigation = false;

			// Set Current Total Price
			double dTotalPrice = App._DbManager.GetPaymentTotalPrice ();
			Globals.Config.PaymentInfo.Price = dTotalPrice.ToString("0.00");

			pageMasterMenu = new MenuPage ();
			pageVenueList = new VenueListPage (null);
            pageVenueList.MasterPage = this;
			pageVenueList.IsDisableBackButton = true;

			Title = "";

			BackgroundColor = Color.Black;

            Master = pageMasterMenu;
			if (page == null)
				Detail = pageVenueList;
			else
				Detail = page;

            pageMasterMenu.Menu.ItemSelected += Menu_ItemSelected;
			newEvHandler = null;
		}

		protected override async void OnAppearing ()
		{
			App.IsWindowAdjustResize = false;
			base.OnAppearing ();
			//Globals.Config.IsPoptoMasterPage = false;

			if (newEvHandler == null) {
				newEvHandler = new MessageBusEventHandler () {
					EventId = Globals.Config.EVENT_MAIN,
					EventAction = (sender, data) => {
						if(!(Detail is MyOrderPage)){
							Device.BeginInvokeOnMainThread(async () => {
								BasePage displayPage = await GetShowPage(1);
								if (displayPage != null)
								{
									displayPage.MasterPage = this;
									Detail = displayPage;
								}
							});
						} else {
							Device.BeginInvokeOnMainThread(async () => {
								((MyOrderPage)Detail).RefreshOrder();
							});
						}
					},
				};
			}

			MessageBus.Default.Register (newEvHandler);
		}

		protected override void OnDisappearing(){
			base.OnDisappearing ();
			MessageBus.Default.Clear(Globals.Config.EVENT_MAIN);
		}

		static long lastEventTickTime = 0;
        async void Menu_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
			long curTickTime = DateTime.Now.Ticks / 1000 / 1000;
			if (Math.Abs(curTickTime - lastEventTickTime) < 3)
				return;
			lastEventTickTime = curTickTime;

            OptionItem selectedItem = e.SelectedItem as OptionItem;
			if (selectedItem == null)
				return;

			IsPresented = false;

            var displayPage = await GetShowPage(selectedItem.Id);
            if (displayPage != null)
            {
                displayPage.MasterPage = this;
				if (IsNewNavigation)
					Detail = displayPage;//new NavigationPage (displayPage);
				else
					Detail = displayPage;
            }

			((ListView)sender).SelectedItem = null;
        }

        public void ShowMenuPage()
        {
            this.IsPresented = true;
        }

		public void SetDetailPage(ContentPage childPage)
		{
			if (IsNewNavigation)
				Detail = childPage;//new NavigationPage (childPage);
			else
				Detail = childPage;
		}

		public void ChangeCurrentVenue(bool isSelected)
		{
			pageMasterMenu.ChangeCurrentVenueLabel (isSelected);
		}

        #region PRIVATE METHODS
        async private Task<BasePage> GetShowPage(int id)
        {
            BasePage newPage = null;

			if (Globals.Config.IsGuestMode == false) {
				switch (id) {
				case 0: // Browse Venues
					newPage = new VenueListPage (null);
					newPage.MasterPage = this;
					newPage.IsDisableBackButton = true;
					break;
				case 1:
					newPage = new MyOrderPage (null);
					newPage.MasterPage = this;
					newPage.IsDisableBackButton = true;
					break;
				case 2:
					newPage = null;
					if (Globals.Config.CurrentVenue != null) {
						var venueItemPage = new VenueItemPage (Globals.Config.CurrentVenue, null);
						venueItemPage.MasterPage = this;
						venueItemPage.IsDisableBackButton = true;
						Navigation.PushAsync (venueItemPage);
					}
					break;
				case 3:
					return null;
				case 4:
					newPage = new FavoritePage (null);
					newPage.MasterPage = this;
					newPage.IsDisableBackButton = true;
					break;
				case 5: // Settings Page
					newPage = new SettingsPage (null);
					newPage.MasterPage = this;
					newPage.IsDisableBackButton = true;
					break;
				case 6: // About Us Page
					newPage = new AboutUsPage (null);
					newPage.MasterPage = this;
					newPage.IsDisableBackButton = true;
					break;
				case 7: // Logout
					DoLogout ();
					break;
				default:
					newPage = new BasePage ();
					newPage.IsDisableBackButton = true;
					break;
				}
			} else {
				switch (id) {
				case 0: // Sign In
					App.PageLoaderManager.Logout ();
					Utils.SaveDataToSettings ("user_last_logged", "0");
					break;
				case 1: // Browse Venues
					newPage = new VenueListPage (null);
					newPage.MasterPage = this;
					newPage.IsDisableBackButton = true;
					break;
				case 2:
					newPage = null;
					if (Globals.Config.CurrentVenue != null) {
						var venueItemPage = new VenueItemPage (Globals.Config.CurrentVenue, null);
						venueItemPage.MasterPage = this;
						venueItemPage.IsDisableBackButton = true;
						Navigation.PushAsync (venueItemPage);
					}
					break;
				case 3:
					return null;
				case 4: // About Us Page
					newPage = new AboutUsPage (null);
					newPage.MasterPage = this;
					newPage.IsDisableBackButton = true;
					break;
				default:
					newPage = new BasePage ();
					newPage.IsDisableBackButton = true;
					break;
				}
			}

            return newPage;
        }

		async void DoLogout()
		{
			Utils.SaveDataToSettings ("user_last_logged", "0");
			var action = await DisplayAlert ("PocketButler", "Are you sure you want to log out?", "OK", "Cancel");

			if (action == true)
			{
				ShowLoading ();

				var device_token = Utils.LoadDataFromSettings ("device_token");
				// Check if user is registered or not
				var response = await LoginServices.Logout (GetUserID(), GetUserToken(), device_token);

				HideLoading ();

				//Navigation.PopToRootAsync ();
				//Navigation.PopAsync ();
				//App._NavigationPage.PopToRootAsync ();
				App.PageLoaderManager.Logout ();
			}
		}
        #endregion
    }
}

