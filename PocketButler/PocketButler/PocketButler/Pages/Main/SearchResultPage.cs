using System;
using Xamarin.Forms;
using PocketButler.Controls;
using System.Collections.Generic;

namespace PocketButler
{
	public class SearchResultPage : BasePage
	{
		class SearchVenueData
		{
			public String Id;
			public String Icon { get; set; }
			public String Name { get; set; }
			public String Address { get; set; }
			public String Distance { get; set; }
			public SearchVenueData(String id, String icon, String name, String address, String distance)
			{
				this.Id = id;
				this.Icon = icon;
				this.Name = name;
				this.Address = address;
				this.Distance = Utils.GetDistanceString(distance);
			}
		}

		class SearchItemData
		{
			public String Id;
			public String Icon { get; set; }
			public String Name { get; set; }
			public String Price { get; set; }
			public String RestaurantName { get; set; }
			public String Distance { get; set; }
			public SearchItemData(String id, String icon, String name, String price, String restaurantname, String distance)
			{
				this.Id = id;
				this.Icon = icon;
				this.Name = name;
				this.Price = price;
				this.RestaurantName = restaurantname;
				this.Distance = Utils.GetDistanceString(distance);
			}
		}

		#region PRIVATE MEMBERS
		protected AbsoluteLayout UILayout { get; set; }
		protected AbsoluteLayout MainLayout { get; set; }

		DILabel EditLabel { get; set; } 

		Button ItemsButton { get; set; }
		Button VenuesButton { get; set; }

		DarkIceImage NoResultLogoImage;
		DILabel NoResultLabel;

		List<RestaurantInfo> VenueList;
		List<MenuItem> ItemList;
		List<SearchVenueData> SearchVenueList = new List<SearchVenueData>();
		List<SearchItemData> SearchItemList = new List<SearchItemData>();

		ListView SearchVenueListView;
		ListView SearchItemListView;

		String searchKey = "";
		String categoryId = "";
		int currentCategory = 1;
		#endregion

		public SearchResultPage(String categoryId, String searchKey, Action RefreshEvent)
		{
			Title = "";
			BackAppearingEvent = RefreshEvent;

			this.searchKey = searchKey;
			this.categoryId = categoryId;

			UILayout = new AbsoluteLayout
			{
				VerticalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.Transparent
			};

			MainLayout = new AbsoluteLayout
			{
				VerticalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.Transparent
			};

			BuildUI();
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
			var PlaceImage = new DarkIceImage
			{
				Source = ImageSource.FromFile("navbar_places.png"),
				Aspect = Aspect.AspectFit
			};

			PlaceImage.Tapped += PlaceButton_Clicked;

			MakeCustomNavigationBar(UILayout, null, PlaceImage, TitleLogoImage);

			var searchAddress = new SearchBar
			{
				Placeholder = "Search Address",
				BackgroundColor = Color.Black,
				Text = searchKey
			};

			searchAddress.SearchButtonPressed += (e, a) =>
			{
				this.searchKey = searchAddress.Text;
				ChangeSearchCategory(currentCategory);
			};

			AbsoluteLayout.SetLayoutFlags(searchAddress, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(searchAddress, new Rectangle(0, 0.03, 1, 0.1));
			MainLayout.Children.Add(searchAddress);

			ItemsButton = new Button {
				Text = "Items",
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				TextColor = Color.FromRgb (171, 146, 91),
				BorderColor = Color.FromRgb (171, 146, 91),
				BackgroundColor = Color.Transparent,
				BorderRadius = 1,
				BorderWidth = 1,
			};
			ItemsButton.Clicked += (object sender, EventArgs e) => ChangeSearchCategory(0);

			VenuesButton = new Button {
				Text = "Venues",
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				TextColor = Color.FromRgb (171, 146, 91),
				BorderColor = Color.FromRgb (171, 146, 91),
				BackgroundColor = Color.Transparent,
				BorderRadius = 1,
				BorderWidth = 1,
			};
			VenuesButton.Clicked += (object sender, EventArgs e) => ChangeSearchCategory(1);

			AbsoluteLayout.SetLayoutFlags(ItemsButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ItemsButton, new Rectangle(0.85, 0.15, 0.4, 0.07));
			MainLayout.Children.Add(ItemsButton);

			AbsoluteLayout.SetLayoutFlags(VenuesButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(VenuesButton, new Rectangle(0.15, 0.15, 0.4, 0.07));
			MainLayout.Children.Add(VenuesButton);

			var SearchResultLabel = new DILabel {
				Text = "Search Results",
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
				TextColor = Color.White,
				Font = Font.SystemFontOfSize(NamedSize.Large)
			};

			AbsoluteLayout.SetLayoutFlags(SearchResultLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(SearchResultLabel, new Rectangle(0.5, 0.25, 0.5, 0.07));
			MainLayout.Children.Add(SearchResultLabel);

			SearchVenueListView = new ListView
			{
				HasUnevenRows = true,
			};
			SearchVenueListView.ItemSelected += async (object sender, SelectedItemChangedEventArgs e) => {
				SearchVenueData selectedItemData = e.SelectedItem as SearchVenueData;
				SearchVenueListView.SelectedItem = null;
				if (selectedItemData != null && VenueList != null)
				{
					RestaurantInfo venueInfo = VenueList.Find(st => st.id == selectedItemData.Id);
					if (venueInfo != null)
					{
						var venueItemPage = new VenueItemPage(venueInfo, PageShowingEvent);
						venueItemPage.MasterPage = MasterPage;
						await Navigation.PushAsync(venueItemPage);
					}
				}
			};

			AbsoluteLayout.SetLayoutFlags(SearchVenueListView, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(SearchVenueListView, new Rectangle(0, 0.95, 1, 0.65));
			MainLayout.Children.Add(SearchVenueListView);

			SearchItemListView = new ListView
			{
				HasUnevenRows = true,
			};
			SearchItemListView.ItemSelected += async (object sender, SelectedItemChangedEventArgs e) => {
				SearchItemData selectedItemData = e.SelectedItem as SearchItemData;

				SearchItemListView.SelectedItem = null;

				if (selectedItemData != null && ItemList != null)
				{
					MenuItem itemInfo = ItemList.Find(st => st.item_id == selectedItemData.Id);
					if (itemInfo != null)
					{
						var venueItemPage = new VenueItemPage(itemInfo.restaurant_info, PageShowingEvent);
						venueItemPage.MasterPage = MasterPage;
						await Navigation.PushAsync(venueItemPage);
					}
				}
			};

			AbsoluteLayout.SetLayoutFlags(SearchItemListView, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(SearchItemListView, new Rectangle(0, 0.95, 1, 0.65));
			MainLayout.Children.Add(SearchItemListView);

			NoResultLogoImage = new DarkIceImage
			{
				Source = ImageSource.FromFile("welcome_logo.png"),
				Aspect = Aspect.AspectFit,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				IsVisible = false
			};
			NoResultLabel = new DILabel {
				Text = "\"No results found.\"",
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
				TextColor = Color.White,
				Font = Font.SystemFontOfSize(NamedSize.Large),
				IsVisible = false
			};

			AbsoluteLayout.SetLayoutFlags(NoResultLogoImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(NoResultLogoImage, new Rectangle(0.5, 0.55, 0.8, 0.15));
			MainLayout.Children.Add(NoResultLogoImage);

			AbsoluteLayout.SetLayoutFlags(NoResultLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(NoResultLabel, new Rectangle(0, 0.7, 1, 0.1));
			MainLayout.Children.Add(NoResultLabel);

			AbsoluteLayout.SetLayoutFlags(MainLayout, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(MainLayout, new Rectangle(0, 0.7, 1, 0.9));
			UILayout.Children.Add(MainLayout);

			Content = UILayout;

			ChangeSearchCategory (1);
		}

		private StackLayout GetSearchVenueView()
		{
			var ItemIconImage = new Image
			{
				WidthRequest = 80,
				VerticalOptions = LayoutOptions.FillAndExpand
			};
			ItemIconImage.SetBinding(DarkIceImage.SourceProperty, "Icon");

			var ItemTextLabel = new DILabel {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				TextColor = Color.White,
				HeightRequest = 40,
				Font = Font.SystemFontOfSize (NamedSize.Large, FontAttributes.Bold),
				IsDefaultLabel = true,
				Lines = 2,
			};
			ItemTextLabel.SetBinding(DILabel.TextProperty, "Name");

			var ItemAddressLabel = new DILabel {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				TextColor = Color.Gray,
				HeightRequest = 45,
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				IsDefaultLabel = true,
				Lines = 2,
			};
			ItemAddressLabel.SetBinding(DILabel.TextProperty, "Address");

			var ItemDistanceLabel = new DILabel {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.End,
				TextColor = Color.Gray,
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				IsDefaultLabel = true,
				Lines = 1,
				LineBreakMode = LineBreakMode.TailTruncation,
			};
			ItemDistanceLabel.SetBinding(DILabel.TextProperty, "Distance");

			var StackMainDataItem = new StackLayout {
				Orientation = StackOrientation.Vertical,
				Padding = 0,
				Spacing = 5,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				WidthRequest = 150,
				Children =
				{
					ItemTextLabel, 
					ItemAddressLabel
				}
			};

			var StackItem = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Padding = new Thickness(20, 5, 20, 5),
				Spacing = 20,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 100,
				Children =
				{
					ItemIconImage, 
					StackMainDataItem,
					ItemDistanceLabel
				}
			};

			return StackItem;
		}

		private StackLayout GetSearchItemView()
		{
			var ItemIconImage = new Image
			{
				WidthRequest = 80,
				VerticalOptions = LayoutOptions.FillAndExpand
			};
			ItemIconImage.SetBinding(DarkIceImage.SourceProperty, "Icon");

			var ItemTextLabel = new DILabel {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				TextColor = Color.White,
				HeightRequest = 45,
				Font = Font.SystemFontOfSize (NamedSize.Large, FontAttributes.Bold),
				IsDefaultLabel = true,
				Lines = 2,
			};
			ItemTextLabel.SetBinding(DILabel.TextProperty, "Name");

			var ItemPriceLabel = new DILabel {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				TextColor = Color.Gray,
				HeightRequest = 25,
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				IsDefaultLabel = true,
				Lines = 1,
			};
			ItemPriceLabel.SetBinding(DILabel.TextProperty, "Price");

			var ItemRestaurantLabel = new DILabel {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				TextColor = Color.Gray,
				HeightRequest = 25,
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				IsDefaultLabel = true,
				Lines = 1,
			};
			ItemRestaurantLabel.SetBinding(DILabel.TextProperty, "RestaurantName");

			var ItemDistanceLabel = new DILabel {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.End,
				TextColor = Color.Gray,
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				IsDefaultLabel = true,
				Lines = 1,
				LineBreakMode = LineBreakMode.TailTruncation,
			};
			ItemDistanceLabel.SetBinding(DILabel.TextProperty, "Distance");

			var StackMainDataItem = new StackLayout {
				Orientation = StackOrientation.Vertical,
				Padding = 0,
				Spacing = 5,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				WidthRequest = 150,
				Children =
				{
					ItemTextLabel, 
					ItemPriceLabel,
					ItemRestaurantLabel
				}
			};

			var StackItem = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Padding = new Thickness(20, 5, 20, 5),
				Spacing = 20,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 100,
				Children =
				{
					ItemIconImage, 
					StackMainDataItem,
					ItemDistanceLabel
				}
				};

			return StackItem;
		}

		async private void ChangeSearchCategory(int category)
		{
			currentCategory = category;
			switch (category) {
			case 0:
				ItemsButton.BackgroundColor = Color.FromRgb(171, 146, 91);
				ItemsButton.TextColor = Color.Black;
				VenuesButton.BackgroundColor = Color.Transparent;
				VenuesButton.TextColor = Color.FromRgb (171, 146, 91);
				break;
			case 1:
				VenuesButton.BackgroundColor = Color.FromRgb (171, 146, 91);
				VenuesButton.TextColor = Color.Black;
				ItemsButton.BackgroundColor = Color.Transparent;
				ItemsButton.TextColor = Color.FromRgb (171, 146, 91);
				break;
			}

			ShowLoading ();

			SearchVenueListView.IsVisible = false;
			SearchItemListView.IsVisible = false;
			NoResultLabel.IsVisible = false;
			NoResultLogoImage.IsVisible = false;

			if (category == 0) {
				var response = await VenueService.GetRestaurantMenuItem (App.UserLatitude.ToString(), App.UserLongitude.ToString(), GetUserID(), GetUserToken(), searchKey);
				bool isResultSuccess = VenueService.HasSuccessResult (response.result);
				if (isResultSuccess) {
					SearchItemListView.IsVisible = true;
					SearchItemList.Clear ();

					ItemList = response.result.restaurants_menu_items;
					foreach (MenuItem item in ItemList) {
						SearchItemList.Add (new SearchItemData (item.item_id, item.item_image, item.item_name, item.item_price + " " + item.currency_symbol, item.restaurant_info.name, item.restaurant_info.distance));
					}

					// Source of data items.
					SearchItemListView.ItemsSource = SearchItemList;
					SearchItemListView.ItemTemplate = new DataTemplate (() => {
						// Return an assembled ViewCell.
						var viewCell = new ViewCell {
							View = GetSearchItemView ()
						};

						viewCell.Height = 100;
						return viewCell;
					});
				} else {
					NoResultLabel.IsVisible = true;
					NoResultLogoImage.IsVisible = true;
				}
			} else {
				var response = await VenueService.GetRestaurantSearchByCoordinate (App.UserLatitude.ToString(), App.UserLongitude.ToString(), categoryId, searchKey);
				bool isResultSuccess = VenueService.HasSuccessResult (response.result);
				if (isResultSuccess) {
					SearchVenueListView.IsVisible = true;
					SearchVenueList.Clear ();

					VenueList = response.result.restaurants_list;
					foreach (RestaurantInfo item in VenueList) {
						SearchVenueList.Add (new SearchVenueData (item.id, item.image, item.name, item.address, item.distance));
					}

					// Source of data items.
					SearchVenueListView.ItemsSource = SearchVenueList;
					SearchVenueListView.ItemTemplate = new DataTemplate (() => {
						// Return an assembled ViewCell.
						var viewCell = new ViewCell {
							View = GetSearchVenueView ()
						};

						viewCell.Height = 100;
						return viewCell;
					});
				} else {
					NoResultLabel.IsVisible = true;
					NoResultLogoImage.IsVisible = true;
				}
			}

			HideLoading ();
		}
		#endregion

		#region EVENTS
		async void MenuButton_Clicked()
		{
			//MasterPage.IsPresented = true;
			if (BackAppearingEvent != null)
				BackAppearingEvent.Invoke ();
			await Navigation.PopAsync ();
		}

		async void PlaceButton_Clicked()
		{
			var MapViewPage = new MapPage (categoryId, searchKey, PageShowingEvent);
			await Navigation.PushAsync (MapViewPage);
		}
		#endregion
	}
}

