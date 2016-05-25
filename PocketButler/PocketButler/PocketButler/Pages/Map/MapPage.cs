using System;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using PocketButler.Controls;
using System.Collections.Generic;
using Xamarin;

namespace PocketButler
{
	public class MapPage : BasePage
	{
		protected AbsoluteLayout UILayout { get; set; }
		protected AbsoluteLayout MainLayout { get; set; }

		Map map;
		String categoryId;
		String searchKey;
		String restaurantId;

		String latitude = "";
		String longitude = "";

		double dLatitude = 0;
		double dLongitude = 0;

		bool IsFromOrigin = true;
		List<RestaurantInfo> categoryItems;

		public MapPage (String category_id, String search_key, Action RefreshEvent, String venueLatitude = "", String venueLongitude = "", bool isFromOrigin = true)
		{
			BackAppearingEvent = RefreshEvent;

			this.restaurantId = "";
			Title = "Map";
			categoryId = category_id;
			searchKey = search_key;
			latitude = venueLatitude;
			longitude = venueLongitude;
			IsFromOrigin = isFromOrigin;

			dLatitude = App.UserLatitude;
			dLongitude = App.UserLongitude;

			if (isFromOrigin == true) {
				latitude = App.UserLatitude.ToString ();
				longitude = App.UserLongitude.ToString ();
			} else {
				double.TryParse (venueLatitude, out dLatitude);
				double.TryParse (venueLongitude, out dLongitude);
			}

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

		public MapPage (String restaurant_id, String venueLatitude, String venueLongitude, Action RefreshEvent)
		{
			BackAppearingEvent = RefreshEvent;

			this.restaurantId = restaurant_id;
			Title = "Map";
			categoryId = "";
			searchKey = "";
			latitude = venueLatitude;
			longitude = venueLongitude;
			IsFromOrigin = false;

			double.TryParse (venueLatitude, out dLatitude);
			double.TryParse (venueLongitude, out dLongitude);

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
			var TitleLogoImage = new DarkIceImage
			{
				Source = ImageSource.FromFile("welcome_logo.png"),
				Aspect = Aspect.AspectFit,
				HorizontalOptions = LayoutOptions.FillAndExpand
			};

			MakeCustomNavigationBar(UILayout, null, null, TitleLogoImage);

			map = new Map(MapSpan.FromCenterAndRadius(new Position(dLatitude, dLongitude), Distance.FromMiles(0.5)))
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				IsShowingUser = true,
				MapType = MapType.Street
			};

			AbsoluteLayout.SetLayoutFlags(map, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(map, new Rectangle(0, 0, 1, 1));
			MainLayout.Children.Add(map);

			AbsoluteLayout.SetLayoutFlags(MainLayout, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(MainLayout, new Rectangle(0, 0.7, 1, 0.9));
			UILayout.Children.Add(MainLayout);

			Content = UILayout;

			//map.SelectedPinChanged += PinSelected;

			LoadVenueFromCurrentLocation ();
		}
		#endregion

		#region EVENTS
		async void MenuButton_Clicked()
		{
			if (BackAppearingEvent != null)
				BackAppearingEvent.Invoke ();
			await Navigation.PopAsync ();
		}

		async void PinSelected(String label)
		{
			if (categoryItems != null) {
				try{
					foreach (RestaurantInfo item in categoryItems)
					{
						if (label.Equals(item.name))
						{
							var VenueSplashPage = new VenueItemPage(item, PageShowingEvent);
							VenueSplashPage.MasterPage = this.MasterPage;
							await Navigation.PushAsync(VenueSplashPage);
							break;
						}
					}
				}
				catch (Exception ex) {
					Insights.Report (ex);
				}
			}
		}

		async void LoadVenueFromCurrentLocation()
		{
			ShowLoading ();

			var response = await VenueService.GetRestaurantNearMe (latitude, longitude, categoryId, searchKey);

			HideLoading ();
			bool isResultSuccess = VenueService.HasSuccessResult (response.result);
			if (isResultSuccess) {
				categoryItems = response.result.restaurants_list;
				int idx = 0;
				map.Pins.Clear ();
				//map.CustomPins.Clear ();

				foreach (RestaurantInfo item in categoryItems) {
					if (restaurantId.Equals ("")) {
						var newPin = new Pin {
							//PinIcon = "mappin_marker_golden",
							Type = PinType.Place,
							Position = new Position (double.Parse (item.lat), double.Parse (item.lng)),
							Label = item.name,
							Address = item.address,
						};
						newPin.Clicked += async (object sender, EventArgs e) => {
							try{
								Pin objSender = sender as Pin;
								if (objSender == null || String.IsNullOrEmpty(objSender.Label))
									return;
								PinSelected(objSender.Label);
							}
							catch (Exception ex)
							{
							}
						};

						map.Pins.Add(newPin);
						idx++;
					} else {
						if (item.id.Equals (this.restaurantId)) {
							var newPin = new Pin {
								//PinIcon = "mappin_marker_golden",
								Type = PinType.Place,
								Position = new Position (double.Parse (item.lat), double.Parse (item.lng)),
								Label = item.name,
								Address = item.address,
							};
							newPin.Clicked += async (object sender, EventArgs e) => {
								try{
									Pin objSender = sender as Pin;
									if (objSender == null || String.IsNullOrEmpty(objSender.Label))
										return;
									PinSelected(objSender.Label);
								}
								catch (Exception ex)
								{
								}
							};

							map.Pins.Add(newPin);
							idx++;
						}
					}
				}
				//map.ForceRedraw = true;
			}
			else
				await DisplayAlert ("Failure", response.result.message, "OK");
		}
		#endregion
	}
}

