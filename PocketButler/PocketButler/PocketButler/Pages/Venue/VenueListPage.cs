using PocketButler.Controls;
using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PocketButler
{
	public class VenueListPage : BasePage
	{
		class CategoryItem
		{
			public String Id { get; set; }
			public String Text { get; set; }
			public String Icon { get; set; }
			public RestaurantInfo ResInfo { get; set; }

			public CategoryItem(String id, String text, String icon, RestaurantInfo info)
			{
				this.Id = id;
				this.Text = text;
				this.Icon = icon;
				this.ResInfo = info;
			}
		}

		#region PRIVATE MEMBERS
        protected AbsoluteLayout UILayout { get; set; }
        protected AbsoluteLayout MainLayout { get; set; }

		SearchBar searchAddress;
		ListView CategoryListView;
		List<CategoryItem> CategoryList = new List<CategoryItem>();

		ListView FavouriteListView;
		List<CategoryItem> FavouriteList = new List<CategoryItem>();

		ScrollView ItemScrollView { get; set; }

		StackLayout CategoryListLayout { get; set; }
		StackLayout FavouriteListLayout { get; set; }

		DILabel CategoriesLabel;
		DILabel FavouritesLabel;
		#endregion

		public VenueListPage (Action RefreshEvent)
		{
			BackAppearingEvent = RefreshEvent;

			Title = "";
			BackgroundColor = Color.Black;

			HideNavigationBar ();

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
		protected override async void OnAppearing ()
		{
			base.OnAppearing ();

			await RefreshCategoryList ();
		}

        private void BuildUI()
        {
            // Register Custom Navigation Bar
			var MenuImage = new DarkIceImage
			{
				Source = ImageSource.FromFile ("navbar_sidebar.png"),
				Aspect = Aspect.AspectFit,
				IsEnablePadding = true,
			};
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
            MakeCustomNavigationBar(UILayout, MenuImage, PlaceImage, TitleLogoImage, true);

            MenuImage.Tapped += MenuButton_Clicked;
            PlaceImage.Tapped += PlaceButton_Clicked;

			searchAddress = new SearchBar
			{
				Placeholder = "Search Address",
				BackgroundColor = Color.Black
			};

            searchAddress.SearchButtonPressed += async (e, a) =>
            {
				Utils.MixPanel_Track("SearchForVenue", "{SearchText:" + searchAddress.Text + "}");

				var SearchPage = new SearchResultPage("", searchAddress.Text, PageShowingEvent);
				SearchPage.MasterPage = MasterPage;
				await Navigation.PushAsync(SearchPage);
            };

            AbsoluteLayout.SetLayoutFlags(searchAddress, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(searchAddress, new Rectangle(0, 0.03, 1, 0.1));
            MainLayout.Children.Add(searchAddress);

			CategoriesLabel = new DILabel {
				Text = "Categories",
				TextColor = Color.White,
				Font = Font.SystemFontOfSize (NamedSize.Large),
				XAlign = TextAlignment.Center,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 30,
			};

			FavouritesLabel = new DILabel {
				Text = "Favourites",
				TextColor = Color.White,
				Font = Font.SystemFontOfSize (NamedSize.Large),
				XAlign = TextAlignment.Center,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				HeightRequest = 40,
			};

			CategoryListView = new ListView
            {
				HasUnevenRows = true,
            };

			CategoryListView.ItemSelected += categorylistView_ItemSelected;

			CategoryListLayout = new StackLayout {
				Orientation = StackOrientation.Vertical,
				Spacing = 0,
				Padding = 0,
				Children = {CategoryListView}
			};

			FavouriteListView = new ListView
			{
				HasUnevenRows = true,
			};

			FavouriteListView.ItemSelected += favouritelistView_ItemSelected;

			FavouriteListLayout = new StackLayout {
				Orientation = StackOrientation.Vertical,
				Spacing = 0,
				Padding = 0,
				Children = {FavouriteListView}
			};

			StackLayout ItemLayout = new StackLayout {
				Orientation = StackOrientation.Vertical,
				Spacing = 0,
				Padding = 0,
			};

			ItemLayout.Children.Add (CategoriesLabel);
			ItemLayout.Children.Add (CategoryListLayout);
			ItemLayout.Children.Add (FavouritesLabel);
			ItemLayout.Children.Add (FavouriteListLayout);

			ItemScrollView = new ScrollView {
				IsClippedToBounds = true,
				Content = ItemLayout
			};

			AbsoluteLayout.SetLayoutFlags(ItemScrollView, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ItemScrollView, new Rectangle(0, 0.95, 1, 0.85));
			MainLayout.Children.Add(ItemScrollView);

            AbsoluteLayout.SetLayoutFlags(MainLayout, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(MainLayout, new Rectangle(0, 0.7, 1, 0.9));
            UILayout.Children.Add(MainLayout);

            Content = UILayout;
        }

		async void categorylistView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			CategoryItem item = e.SelectedItem as CategoryItem;

			if (item != null)
			{
				CategoryListView.SelectedItem = null;

				Utils.MixPanel_Track("BrowseCategory", "{CategoryName:" + item.Text + "}");

				var CategoryDetailPage = new VenueCategoryDetailPage (PageShowingEvent, item.Id, item.Text);
				CategoryDetailPage.MasterPage = MasterPage;
				await Navigation.PushAsync (CategoryDetailPage);
			}
		}

		async void favouritelistView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			CategoryItem item = e.SelectedItem as CategoryItem;

			if (item != null)
			{
				CategoryListView.SelectedItem = null;

				Utils.MixPanel_Track("BrowseVenueByItem", "{ItemName:" + item.Text + ", VenueName: " + item.ResInfo.name + "}");

				var ItemInfoPage = new VenueItemPage (item.ResInfo, PageShowingEvent, item.Id);
				ItemInfoPage.MasterPage = MasterPage;
				await Navigation.PushAsync (ItemInfoPage);
			}
		}

		async Task RefreshCategoryList()
		{
			ShowLoading ();

			var response = await VenueService.GetRestaurantCategory ("", "", GetUserID(), GetUserToken());

			HideLoading ();

			bool isResultSuccess = VenueService.HasSuccessResult (response.result);
			if (isResultSuccess) {
				CategoryList.Clear ();
				FavouriteList.Clear ();
				List<Category> categoryItems = response.result.categories;
				foreach (Category item in categoryItems) {
					CategoryList.Add (new CategoryItem (item.category_id, item.category_name, item.category_icon, null));
				}

				if (response.result.user_favourites_shortcut != null) {
					List<UserFavouritesItemData> favoritesItems = response.result.user_favourites_shortcut;
					foreach (UserFavouritesItemData favitem in favoritesItems) {
						FavouriteList.Add (new CategoryItem (favitem.item_id, favitem.item_name, favitem.item_image, favitem.restaurant_info));
					}
				}

				// Source of data items.
				CategoryListView.ItemsSource = CategoryList;
				CategoryListView.ItemTemplate = new DataTemplate (() => {
					// Return an assembled ViewCell.
					var viewCell = new ViewCell {
						View = GetCategoryItem ()
					};

					viewCell.Height = 80;
					return viewCell;
				});

				if (CategoryList == null || CategoryList.Count <= 0) {
					CategoryListLayout.HeightRequest = 0;
				} else {
					CategoryListLayout.HeightRequest = CategoryList.Count * 80;
				}

				FavouriteListView.ItemsSource = FavouriteList;
				FavouriteListView.ItemTemplate = new DataTemplate (() => {
					// Return an assembled ViewCell.
					var viewCell = new ViewCell {
						View = GetCategoryItem ()
					};

					viewCell.Height = 80;
					return viewCell;
				});

				if (FavouriteList == null || FavouriteList.Count <= 0) {
					FavouritesLabel.HeightRequest = 0;
					FavouriteListLayout.HeightRequest = 0;
				} else {
					FavouritesLabel.HeightRequest = 40;
					FavouriteListLayout.HeightRequest = FavouriteList.Count * 80;
				}
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
				TextColor = Color.White,
				Font = Font.SystemFontOfSize (NamedSize.Large, FontAttributes.Bold),
				IsDefaultLabel = true,
			};
			ItemTextLabel.SetBinding(DILabel.TextProperty, "Text");

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
		#endregion

        #region EVENTS
        void MenuButton_Clicked()
        {
            MasterPage.IsPresented = true;
        }

        async void PlaceButton_Clicked()
        {
			var MapViewPage = new MapPage ("", String.IsNullOrEmpty(searchAddress.Text) ? "" : searchAddress.Text, PageShowingEvent);
			await Navigation.PushAsync (MapViewPage);
        }
        #endregion
    }
}

