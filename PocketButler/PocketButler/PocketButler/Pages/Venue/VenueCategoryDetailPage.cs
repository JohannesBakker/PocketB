using System;
using Xamarin.Forms;
using System.Collections.Generic;
using PocketButler.Controls;
using System.Collections.ObjectModel;

namespace PocketButler
{
	public class VenueCategoryDetailPage : BasePage
	{
		class CategoryItemDetail
		{
			public String Id;
			public String Text { get; set; }
			public String TextDetail { get; set; }
			public String Distance { get; set; }
			public String Icon { get; set; }
			public bool IsOffline { get; set; }
			public String OfflineText { get; set; }
			public Color OfflineColor { get; set; }

			public CategoryItemDetail(String id, String text, String textdetail, String icon, String distance, bool isOffline, String offlineText, Color offlineColor)
			{
				this.Id = id;
				this.Text = text;
				this.TextDetail = textdetail;
				this.Icon = icon;
				this.Distance = Utils.GetDistanceString(distance);
				this.IsOffline = isOffline;
				this.OfflineText = offlineText;
				this.OfflineColor = offlineColor;
			}
		}

		#region PRIVATE MEMBERS
		protected AbsoluteLayout UILayout { get; set; }
		protected AbsoluteLayout MainLayout { get; set; }

		SearchBar searchAddress;
		ListView CategoryDetailListView;
		ObservableCollection<CategoryItemDetail> CategoryDetailList = new ObservableCollection<CategoryItemDetail>();
		List<RestaurantInfo> RestaurantItems;

		String Category_Id = "";
		String Category_Name = "";

		#endregion
		public VenueCategoryDetailPage (Action RefreshEvent, String category_id, String category_name)
		{
			BackAppearingEvent = RefreshEvent;

			this.Category_Id = category_id;
			this.Category_Name = category_name;

			Title = "";
			BackgroundColor = Color.Black;

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
		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			//if (Globals.Config.IsPoptoMasterPage == false)
				RefreshCategoryList ();
		}

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
			MakeCustomNavigationBar(UILayout, null, PlaceImage, TitleLogoImage);
			PlaceImage.Tapped += PlaceButton_Clicked;

			searchAddress = new SearchBar
			{
				Placeholder = "Search Address",
				BackgroundColor = Color.Black
			};

			searchAddress.SearchButtonPressed += async (e, a) =>
			{
				Utils.MixPanel_Track("SearchForVenue", "{SearchText:" + searchAddress.Text + "}");

				var SearchPage = new SearchResultPage(Category_Id, searchAddress.Text, PageShowingEvent);
				SearchPage.MasterPage = MasterPage;
				await Navigation.PushAsync(SearchPage);
			};

			AbsoluteLayout.SetLayoutFlags(searchAddress, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(searchAddress, new Rectangle(0, 0.03, 1, 0.1));
			MainLayout.Children.Add(searchAddress);

			DILabel BrowseAllLabel = new DILabel {
				Text = Category_Name,
				TextColor = Color.White,
				Font = Font.SystemFontOfSize (NamedSize.Large),
				XAlign = TextAlignment.Center
			};
			AbsoluteLayout.SetLayoutFlags(BrowseAllLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(BrowseAllLabel, new Rectangle(0, 0.15, 1, 0.1));
			MainLayout.Children.Add(BrowseAllLabel);

			CategoryDetailListView = new ListView
			{
				HasUnevenRows = true,
			};

			CategoryDetailListView.ItemSelected += listView_ItemSelected;

			AbsoluteLayout.SetLayoutFlags(CategoryDetailListView, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(CategoryDetailListView, new Rectangle(0, 0.95, 1, 0.75));
			MainLayout.Children.Add(CategoryDetailListView);

			AbsoluteLayout.SetLayoutFlags(MainLayout, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(MainLayout, new Rectangle(0, 0.7, 1, 0.9));
			UILayout.Children.Add(MainLayout);

			Content = UILayout;
		}

		async void listView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			CategoryItemDetail item = e.SelectedItem as CategoryItemDetail;

			if (item != null)
			{
				CategoryDetailListView.SelectedItem = null;

				RestaurantInfo info = RestaurantItems.Find (s => s.id == item.Id);
				//CategoryItemDetail itemSelected = CategoryDetailList.Find (s => s.Id == item.Id);
				//if (itemSelected != null)
				if (info != null) {
					var VenueItemPage = new VenueItemPage (info, PageShowingEvent);
					VenueItemPage.MasterPage = MasterPage;
					await Navigation.PushAsync (VenueItemPage);
				}
			}
		}

		async void RefreshCategoryList()
		{
			ShowLoading ();

			var response = await VenueService.GetRestaurantSearchByCoordinate (App.UserLatitude.ToString(), App.UserLongitude.ToString(), Category_Id, "");

			HideLoading ();

			bool isResultSuccess = VenueService.HasSuccessResult (response.result);
			if (isResultSuccess) {
				CategoryDetailList.Clear ();
				RestaurantItems = response.result.restaurants_list;

				foreach (RestaurantInfo item in RestaurantItems) {
					item.CheckOrderStatus ();
					CategoryDetailList.Add (new CategoryItemDetail (item.id, item.name, item.address, item.image, item.distance, !item.CanPlaceOrder(), item.VenueStatusText(), item.GetStatusColor()));
				}

				// Source of data items.
				CategoryDetailListView.ItemsSource = CategoryDetailList;

				CategoryDetailListView.ItemTemplate = new DataTemplate (() => {
					// Return an assembled ViewCell.
					var viewCell = new ViewCell {
						View = GetCategoryItem ()
					};

					viewCell.Height = 80;
					return viewCell;
				});
			}
			else
				await DisplayAlert ("Failure", response.result.message, "OK");
		}
		#endregion

		#region PRIVATE METHODS
		private StackLayout GetCategoryItem()
		{
			var ItemIconImage = new DarkIceImage
			{
				WidthRequest = 60,
				VerticalOptions = LayoutOptions.FillAndExpand,
				IsDefaultImage = true,
			};
			ItemIconImage.SetBinding(DarkIceImage.SourceProperty, "Icon");

			var ItemTextLabel = new DILabel {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				HeightRequest = 30,
				IsDefaultLabel = true,
				Lines = 1,
				TextColor = Color.White,
				Font = Font.SystemFontOfSize (NamedSize.Large, FontAttributes.Bold)
			};
			ItemTextLabel.SetBinding(DILabel.TextProperty, "Text");

			var ItemTextDetailLabel = new DILabel {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				IsDefaultLabel = true,
				Lines = 2,
				TextColor = Color.Gray,
				Font = Font.SystemFontOfSize (NamedSize.Medium),
			};
			ItemTextDetailLabel.SetBinding(DILabel.TextProperty, "TextDetail");

			var ItemTextDistanceLabel = new DILabel {
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				IsDefaultLabel = true,
				Lines = 1,
				WidthRequest = 50,
				TextColor = Color.Gray,
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				LineBreakMode = LineBreakMode.TailTruncation,
			};
			ItemTextDistanceLabel.SetBinding(DILabel.TextProperty, "Distance");
			/*
			var StackMainDataItem = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				Padding = 0,
				Spacing = 5,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 50,
				Children =
				{
					ItemTextDetailLabel, 
					ItemTextDistanceLabel
				}
			};

			var ImageLayout = new AbsoluteLayout {
				WidthRequest = 60,
				HorizontalOptions = LayoutOptions.StartAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.Transparent
			};*/

			//AbsoluteLayout.SetLayoutFlags(ItemIconImage, AbsoluteLayoutFlags.All);
			//AbsoluteLayout.SetLayoutBounds(ItemIconImage, new Rectangle(0, 0, 1, 1));
			//ImageLayout.Children.Add(ItemIconImage);

			var OfflineLabel = new DILabel {
				WidthRequest = 60,
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
				//TextColor = Color.FromRgb(150, 0, 0),
				Text = "",
				IsDefaultLabel = true,
				BackgroundColor = Color.FromRgba(0, 0, 0, 200),
				FontSize = 15,
			};
			OfflineLabel.SetBinding (DILabel.IsVisibleProperty, "IsOffline");
			OfflineLabel.SetBinding (DILabel.TextProperty, "OfflineText");
			OfflineLabel.SetBinding (DILabel.TextColorProperty, "OfflineColor");

			//AbsoluteLayout.SetLayoutFlags(OfflineLabel, AbsoluteLayoutFlags.All);
			//AbsoluteLayout.SetLayoutBounds(OfflineLabel, new Rectangle(0, 0, 1, 1));
			//ImageLayout.Children.Add(OfflineLabel);

			var StackItem = new AbsoluteLayout {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.Transparent
			};

			AbsoluteLayout.SetLayoutFlags(ItemIconImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ItemIconImage, new Rectangle(0, 0, 0.2, 1));
			StackItem.Children.Add(ItemIconImage);

			AbsoluteLayout.SetLayoutFlags(OfflineLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(OfflineLabel, new Rectangle(0, 0, 0.2, 1));
			StackItem.Children.Add(OfflineLabel);

			AbsoluteLayout.SetLayoutFlags(ItemTextLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ItemTextLabel, new Rectangle(1, 0, 0.77, 0.45));
			StackItem.Children.Add(ItemTextLabel);

			AbsoluteLayout.SetLayoutFlags(ItemTextDetailLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ItemTextDetailLabel, new Rectangle(0.6, 1, 0.6, 0.55));
			StackItem.Children.Add(ItemTextDetailLabel);

			AbsoluteLayout.SetLayoutFlags(ItemTextDistanceLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ItemTextDistanceLabel, new Rectangle(1, 1, 0.15, 0.55));
			StackItem.Children.Add(ItemTextDistanceLabel);

			var StackItemLayout = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				Padding = new Thickness(20, 10, 20, 10),
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 80,
				Children = {
					StackItem,
				}
			};

			/*
			var StackItem = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Padding = new Thickness(20, 10, 20, 10),
				Spacing = 10,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 80,
				Children =
				{
					ImageLayout,					 
					new StackLayout{
						Orientation = StackOrientation.Vertical,
						Padding = 0,
						Spacing = 0,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						VerticalOptions = LayoutOptions.FillAndExpand,
						Children = {
							ItemTextLabel, StackMainDataItem
						}
					}
				}
			};*/

			return StackItemLayout;
		}
		#endregion

		#region EVENTS
		async void MenuButton_Clicked()
		{
			if (BackAppearingEvent != null)
				BackAppearingEvent.Invoke ();
			await Navigation.PopAsync ();
		}

		async void PlaceButton_Clicked()
		{
			await Navigation.PushAsync (new MapPage (Category_Id, String.IsNullOrEmpty(searchAddress.Text) ? "" : searchAddress.Text, PageShowingEvent));
		}
		#endregion
	}
}