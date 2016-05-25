using System;
using Xamarin.Forms;
using PocketButler.Controls;
using System.Collections.Generic;
using PocketButler.Services;
using System.Linq;

namespace PocketButler
{
	public class FavoritePage : BasePage
	{
		class FavouriteItem
		{
			public String Id { get; set; }
			public String Type { get; set; }
			public String Name { get; set; }
			public String Price { get; set; }
			public String RestaurantName { get; set; }
			public String Address { get; set; }
			public String Distance { get; set; }
			public String Icon { get; set; }
			public bool IsShortcut { get; set; }

			public String ExtraId { get; set; }

			public Color ShortcutBgColor { get; set; }

			public FavouriteItem(String id, String type, String name, String price, String restaurantName, String address, String distance, String icon, String extraId, bool isShortcut)
			{
				this.Id = id;
				this.Type = type;
				this.Name = name;
				this.Price = price;
				this.RestaurantName = restaurantName;
				this.Address = address;
				this.Distance = Utils.GetDistanceString(distance);
				this.Icon = icon;
				this.IsShortcut = isShortcut;
				this.ExtraId = extraId;
				this.ShortcutBgColor = (isShortcut) ? Color.FromRgb(171, 146, 91) : Color.Black;
			}
		}

		#region PRIVATE MEMBERS
		protected AbsoluteLayout UILayout { get; set; }
		protected AbsoluteLayout MainLayout { get; set; }
		protected StackLayout FavouritesItemLayout { get; set; }
		protected ScrollView FavouritesItemScrollView { get; set; }
		protected ListView FavouritesVenueListView { get; set; }

		DILabel EditLabel { get; set; } 

		Button ItemsButton { get; set; }
		Button VenuesButton { get; set; }

		List<FavouriteItem> VenueFavList = new List<FavouriteItem>();
		List<FavouriteItem> ItemFavWithVenueList = new List<FavouriteItem>();
		List<FavouriteItem> ItemFavWithShortcutList = new List<FavouriteItem>();
		List<FavouriteItem> ItemFavWithGeneralList = new List<FavouriteItem>();
		List<FavouriteItem> ItemThisVenueList = new List<FavouriteItem>();
		List<FavouriteItem> ItemThisVenueTypeList = new List<FavouriteItem>();

		StackLayout ShortcutLayout;
		StackLayout GeneralLayout;
		StackLayout ThisVenueLayout;
		StackLayout ThisVenueTypeLayout;
		StackLayout NoVenueLayout;

		StackLayout ShortcutListLayout;
		StackLayout GeneralListLayout;
		StackLayout ThisVenueListLayout;
		StackLayout ThisVenueTypeListLayout;

		Button ShortcutExpandButton;
		Button GeneralExpandButton;
		Button ThisVenueExpandButton;
		Button ThisVenueTypeExpandButton;

		DILabel ShortcutLabel;
		DILabel GeneralLabel;
		DILabel ThisVenueLabel;
		DILabel ThisVenueTypeLabel;

		bool IsShortcutExpanded = false;
		bool IsGeneralExpanded = false;
		bool IsThisVenueExpanded = false;
		bool IsThisVenueTypeExpanded = false;

		bool IsEditing = false;

		ListView ShortCutListView;
		ListView GeneralListView;
		ListView ThisVenueListView;
		ListView ThisVenueTypeListView;

		int curCategory = 0;

		int ExpandedShortcutSize = 0;
		int ExpandedGeneralSize = 0;
		int ExpandedThisVenueSize = 0;
		int ExpandedThisVenueTypeSize = 0;

		List<RestaurantInfo> fav_restaurants;
		List<UserFavouritesItemData> favorite_venue_type;
		List<UserFavouritesItemData> favorite_item;
		List<UserFavouritesItemData> favorite_shortcut;
		List<UserFavouritesItemData> current_favourites;

		#endregion

		public FavoritePage(Action RefreshEvent)
		{
			BackAppearingEvent = RefreshEvent;
			Title = "My Favourites";

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
			var MenuImage = new DarkIceImage
			{
				Source = ImageSource.FromFile ("navbar_sidebar.png"),
				Aspect = Aspect.AspectFit,
				IsEnablePadding = true,
			};

			EditLabel = new DILabel {
				Text = "Edit",
				TextColor = Color.FromRgb(171, 146, 91),
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				YAlign = TextAlignment.Center,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
			};

			EditLabel.Tapped += EditFav_Clicked;

			MakeCustomNavigationBar(UILayout, MenuImage, EditLabel, true);

			MenuImage.Tapped += MenuButton_Clicked;

			ItemsButton = new Button {
				Text = "Items",
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				TextColor = Color.FromRgb (171, 146, 91),
				BorderColor = Color.FromRgb (171, 146, 91),
				BackgroundColor = Color.Transparent,
				BorderRadius = 1,
				BorderWidth = 1,
			};
			ItemsButton.Clicked += (object sender, EventArgs e) => ChangeFavouriteCategory(0);

			VenuesButton = new Button {
				Text = "Venues",
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				TextColor = Color.FromRgb (171, 146, 91),
				BorderColor = Color.FromRgb (171, 146, 91),
				BackgroundColor = Color.Transparent,
				BorderRadius = 1,
				BorderWidth = 1,
			};
			VenuesButton.Clicked += (object sender, EventArgs e) => ChangeFavouriteCategory(1);

			AbsoluteLayout.SetLayoutFlags(ItemsButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ItemsButton, new Rectangle(0.15, 0.05, 0.4, 0.07));
			MainLayout.Children.Add(ItemsButton);

			AbsoluteLayout.SetLayoutFlags(VenuesButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(VenuesButton, new Rectangle(0.85, 0.05, 0.4, 0.07));
			MainLayout.Children.Add(VenuesButton);

			FavouritesItemLayout = new StackLayout {
				Orientation = StackOrientation.Vertical,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Spacing = 0,
				Padding = 0,
			};

			FavouritesItemScrollView = new ScrollView {
				IsClippedToBounds = true,
				Content = FavouritesItemLayout
			};

			ShortCutListView = new ListView {
				HasUnevenRows = true,

				ItemTemplate = new DataTemplate (() => {
					// Return an assembled ViewCell.
					var viewCell = new ViewCell {
						View = GetFavouriteMenuItem (true)
					};

					viewCell.Height = 80;
					return viewCell;
				}),
			};

			ShortCutListView.ItemSelected += (object sender, SelectedItemChangedEventArgs e) => {
				if(e.SelectedItem == null) return;
				((ListView)sender).SelectedItem = null;
				SelectFavMenuItem(e.SelectedItem);
			};

			GeneralListView = new ListView {
				HasUnevenRows = true,

				ItemTemplate = new DataTemplate (() => {
					// Return an assembled ViewCell.
					var viewCell = new ViewCell {
						View = GetFavouriteMenuItem ()
					};

					viewCell.Height = 80;
					return viewCell;
				}),
			};
			GeneralListView.ItemSelected += (object sender, SelectedItemChangedEventArgs e) => {
				if(e.SelectedItem == null) return;
				((ListView)sender).SelectedItem = null;
				SelectFavMenuItem(e.SelectedItem);
			};

			ThisVenueListView = new ListView {
				HasUnevenRows = true,
				ItemTemplate = new DataTemplate(() => {
					var viewCell = new ViewCell{
						View = GetFavouriteMenuItem(),
					};
					viewCell.Height = 80;
					return viewCell;
				}),
			};
			ThisVenueListView.ItemSelected += (object sender, SelectedItemChangedEventArgs e) => {
				if(e.SelectedItem == null) return;
				((ListView)sender).SelectedItem = null;
				SelectFavMenuItem(e.SelectedItem);
			};

			ThisVenueTypeListView = new ListView {
				HasUnevenRows = true,
				ItemTemplate = new DataTemplate(() => {
					var viewCell = new ViewCell{
						View = GetFavouriteMenuItem(),
					};
					viewCell.Height = 80;
					return viewCell;
				}),
			};
			ThisVenueTypeListView.ItemSelected += (object sender, SelectedItemChangedEventArgs e) => {
				if(e.SelectedItem == null) return;
				((ListView)sender).SelectedItem = null;
				SelectFavMenuItem(e.SelectedItem);
			};


			FavouritesVenueListView = new ListView {
				HasUnevenRows = true,

				ItemTemplate = new DataTemplate (() => {
					// Return an assembled ViewCell.
					var viewCell = new ViewCell {
						View = GetFavouriteVenueItem ()
					};

					//viewCell.Height = 80;
					return viewCell;
				}),

				IsVisible = false,
			};

			AbsoluteLayout.SetLayoutFlags(FavouritesItemScrollView, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(FavouritesItemScrollView, new Rectangle(0, 1, 1, 0.85));
			MainLayout.Children.Add(FavouritesItemScrollView);

			AbsoluteLayout.SetLayoutFlags(FavouritesVenueListView, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(FavouritesVenueListView, new Rectangle(0, 1, 1, 0.85));
			MainLayout.Children.Add(FavouritesVenueListView);

			AbsoluteLayout.SetLayoutFlags(MainLayout, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(MainLayout, new Rectangle(0, 0.7, 1, 0.9));
			UILayout.Children.Add(MainLayout);

			#region 

			if(Globals.Config.CurrentVenue != null){
				ThisVenueLabel = new DILabel {
					TextColor = Color.White,
					Text = "   " + Globals.Config.CurrentVenue.name,
					Font = Font.SystemFontOfSize (NamedSize.Medium),
					YAlign = TextAlignment.Center,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					VerticalOptions = LayoutOptions.FillAndExpand,
				};
				ThisVenueExpandButton = new Button {
					Text = IsThisVenueExpanded ? "-" : "+",
					BackgroundColor = Color.Transparent,
					TextColor = Color.White,
					Font = Font.SystemFontOfSize (NamedSize.Medium),
					WidthRequest = 30,
					VerticalOptions = LayoutOptions.FillAndExpand,
					HorizontalOptions = LayoutOptions.End,
				};
				ThisVenueLayout = new StackLayout {
					BackgroundColor = Color.FromRgb (50, 50, 50),
					Orientation = StackOrientation.Horizontal,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					HeightRequest = 50,
					Children = {
						ThisVenueLabel,
						ThisVenueExpandButton
					}
				};

				ThisVenueExpandButton.Clicked += (object sender, EventArgs e) => {
					ThisVenueFav_Clicked();
				};
				ThisVenueLabel.Tapped += ThisVenueFav_Clicked;

				FavouritesItemLayout.Children.Add (ThisVenueLayout);

				ThisVenueListLayout = new StackLayout {
					Orientation = StackOrientation.Vertical,
					Spacing = 0,
					Padding = 0,
					Children = {ThisVenueListView}
				};

				FavouritesItemLayout.Children.Add (ThisVenueListLayout);

				ThisVenueTypeLabel = new DILabel {
					TextColor = Color.White,
					Text = "   Favourites from this Venue Type",
					Font = Font.SystemFontOfSize (NamedSize.Medium),
					YAlign = TextAlignment.Center,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					VerticalOptions = LayoutOptions.FillAndExpand,
				};
				ThisVenueTypeExpandButton = new Button {
					Text = IsThisVenueTypeExpanded ? "-" : "+",
					BackgroundColor = Color.Transparent,
					TextColor = Color.White,
					Font = Font.SystemFontOfSize (NamedSize.Medium),
					WidthRequest = 30,
					VerticalOptions = LayoutOptions.FillAndExpand,
					HorizontalOptions = LayoutOptions.End,
				};
				ThisVenueTypeLayout = new StackLayout {
					BackgroundColor = Color.FromRgb (50, 50, 50),
					Orientation = StackOrientation.Horizontal,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					HeightRequest = 50,
					Children = {
						ThisVenueTypeLabel,
						ThisVenueTypeExpandButton
					}
				};

				ThisVenueTypeExpandButton.Clicked += (object sender, EventArgs e) => {
					ThisVenueTypeFav_Clicked();
				};
				ThisVenueTypeLabel.Tapped += ThisVenueTypeFav_Clicked;

				FavouritesItemLayout.Children.Add (ThisVenueTypeLayout);

				ThisVenueTypeListLayout = new StackLayout {
					Orientation = StackOrientation.Vertical,
					Spacing = 0,
					Padding = 0,
					Children = {ThisVenueTypeListView}
				};

				FavouritesItemLayout.Children.Add (ThisVenueTypeListLayout);

			} else {
				NoVenueLayout = new StackLayout {
					BackgroundColor = Color.FromRgb (171, 146, 91),
					Orientation = StackOrientation.Horizontal,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					HeightRequest = 50,
					Children = {
						new DILabel {
							TextColor = Color.FromRgb (50, 50, 50),
							Text = "You are not currently within a venue",
							Font = Font.SystemFontOfSize (NamedSize.Medium),
							XAlign = TextAlignment.Center,
							YAlign = TextAlignment.Center,
							HorizontalOptions = LayoutOptions.FillAndExpand,
							VerticalOptions = LayoutOptions.FillAndExpand,
						},
					}
				};

				FavouritesItemLayout.Children.Add (NoVenueLayout);
			}


			#endregion

			ShortcutLabel = new DILabel {
				TextColor = Color.White,
				Text = "   Shortcuts",
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				YAlign = TextAlignment.Center,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
			};
			ShortcutExpandButton = new Button {
				Text = IsShortcutExpanded ? "-" : "+",
				BackgroundColor = Color.Transparent,
				TextColor = Color.White,
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				WidthRequest = 30,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.End,
			};
			ShortcutLayout = new StackLayout {
				BackgroundColor = Color.FromRgb (50, 50, 50),
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 50,
				Children = {
					ShortcutLabel,
					ShortcutExpandButton
				}
			};

			ShortcutExpandButton.Clicked += (object sender, EventArgs e) => {
				ShortcutFav_Clicked();
			};
			ShortcutLabel.Tapped += ShortcutFav_Clicked;

			FavouritesItemLayout.Children.Add (ShortcutLayout);

			ShortcutListLayout = new StackLayout {
				Orientation = StackOrientation.Vertical,
				Spacing = 0,
				Padding = 0,
				Children = {ShortCutListView}
			};
			FavouritesItemLayout.Children.Add (ShortcutListLayout);

			GeneralLabel = new DILabel {
				TextColor = Color.White,
				Text = "   General Favourites",
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				YAlign = TextAlignment.Center,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
			};
			GeneralExpandButton = new Button {
				Text = IsGeneralExpanded ? "-" : "+",
				BackgroundColor = Color.Transparent,
				TextColor = Color.White,
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				WidthRequest = 30,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.End,
			};
			GeneralLayout = new StackLayout {
				BackgroundColor = Color.FromRgb (50, 50, 50),
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 50,
				Children = {
					GeneralLabel,
					GeneralExpandButton
				}
			};

			GeneralExpandButton.Clicked += (object sender, EventArgs e) => {
				GeneralFav_Clicked();
			};
			GeneralLabel.Tapped += GeneralFav_Clicked;

			FavouritesItemLayout.Children.Add (GeneralLayout);

			GeneralListLayout = new StackLayout {
				Orientation = StackOrientation.Vertical,
				Spacing = 0,
				Padding = 0,
				Children = {GeneralListView}
			};

			FavouritesItemLayout.Children.Add (GeneralListLayout);

			Content = UILayout;
		}

		public override async void OnPageShowing ()
		{
			base.OnPageShowing ();

			ChangeFavouriteCategory (0);
		}

		private StackLayout GetFavouriteItem()
		{
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
				Children = { ItemTextLabel }
			};

			return StackItem;
		}

		async private void ChangeFavouriteCategory(int category)
		{
			curCategory = category;
			switch (category) {
			case 0:
				ItemsButton.BackgroundColor = Color.FromRgb (171, 146, 91);
				ItemsButton.TextColor = Color.Black;
				VenuesButton.BackgroundColor = Color.Transparent;
				VenuesButton.TextColor = Color.FromRgb (171, 146, 91);
				FavouritesVenueListView.IsVisible = false;
				FavouritesItemScrollView.IsVisible = true;
				break;
			case 1:
				VenuesButton.BackgroundColor = Color.FromRgb (171, 146, 91);
				VenuesButton.TextColor = Color.Black;
				ItemsButton.BackgroundColor = Color.Transparent;
				ItemsButton.TextColor = Color.FromRgb (171, 146, 91);
				FavouritesVenueListView.IsVisible = true;
				FavouritesItemScrollView.IsVisible = false;
				break;
			}

			ShowLoading ();

			if (Globals.Config.CurrentVenue != null) {
				ThisVenueListView.ItemsSource = null;
				ThisVenueTypeListView.ItemsSource = null;
			}

			GeneralListView.ItemsSource = null;

			// Get Favourite Item
			if (category == 0) {
				String curVenueId = "";
				if (Globals.Config.CurrentVenue != null)
					curVenueId = Globals.Config.CurrentVenue.id;

				var responseVenue = await FavouriteService.DisplayUserFavourites (GetUserID(), GetUserToken(), App.UserLatitude.ToString(), App.UserLongitude.ToString(), curVenueId, "");
				bool isSuccessVenue = FavouriteService.HasSuccessResult (responseVenue);
				favorite_venue_type = new List<UserFavouritesItemData> ();
				favorite_item = new List<UserFavouritesItemData> ();
				favorite_shortcut = new List<UserFavouritesItemData>();
				current_favourites = new List<UserFavouritesItemData>();

				if (isSuccessVenue) {
					favorite_venue_type = responseVenue.favourites_from_this_venue_type;
					favorite_item = responseVenue.user_favourites_item;
					favorite_shortcut = responseVenue.user_favourites_shortcut;
					current_favourites = responseVenue.current_restaurant_favourites;
				}
				AddItemsToItemUI (current_favourites, favorite_shortcut, favorite_item, favorite_venue_type);
			} else {
				var responseVenue = await FavouriteService.DisplayFavouritesRestaurant (GetUserID(), GetUserToken(), App.UserLatitude.ToString(), App.UserLongitude.ToString(), "", "");
				bool isSuccessVenue = FavouriteService.HasSuccessResult (responseVenue);
				if (isSuccessVenue) {
					fav_restaurants = responseVenue.user_favourites_restaurants;
					List<FavouriteItem> fav_items = new List<FavouriteItem> ();
					foreach (RestaurantInfo info in fav_restaurants)
						fav_items.Add(new FavouriteItem(info.id, "0", info.name, "", "", info.address, info.distance, info.image, "", false));
					AddItemsToVenueUI (fav_items);
				}
			}

			HideLoading ();
		}

		private void AddItemsToItemUI(List<UserFavouritesItemData> current_favourites, List<UserFavouritesItemData> favorite_shortcut, 
			List<UserFavouritesItemData> favorite_item, List<UserFavouritesItemData> favorite_venue_type)
		{
			Device.BeginInvokeOnMainThread(()=> {
				try{
					ShortcutExpandButton.Text = IsShortcutExpanded ? "-" : "+";
					GeneralExpandButton.Text = IsGeneralExpanded ? "-" : "+";
					if (Globals.Config.CurrentVenue != null) {
						ThisVenueExpandButton.Text = IsThisVenueExpanded ? "-" : "+";
						ThisVenueTypeExpandButton.Text = IsThisVenueTypeExpanded ? "-" : "+";
					}

					List<FavouriteItem> fav_shortcut_item_list = new List<FavouriteItem> ();
					foreach (UserFavouritesItemData fav_shortcut_item in favorite_shortcut)
					{
						fav_shortcut_item_list.Add (new FavouriteItem (fav_shortcut_item.item_id, "1", fav_shortcut_item.item_name, fav_shortcut_item.item_price + " " + fav_shortcut_item.currency_symbol, fav_shortcut_item.restaurant_info.name, fav_shortcut_item.restaurant_info.address, fav_shortcut_item.restaurant_info.distance, fav_shortcut_item.item_image, fav_shortcut_item.restaurant_info.id, true));
					}
					ShortCutListView.ItemsSource = fav_shortcut_item_list;
					ExpandedShortcutSize = 80 * fav_shortcut_item_list.Count;
					ShortcutListLayout.HeightRequest = IsShortcutExpanded ? ExpandedShortcutSize : 0;

					List<FavouriteItem> fav_general_item_list = new List<FavouriteItem> ();
					foreach (UserFavouritesItemData fav_shortcut_item in favorite_item)
					{
						fav_general_item_list.Add (new FavouriteItem (fav_shortcut_item.item_id, "1", fav_shortcut_item.item_name, fav_shortcut_item.item_price + " " + fav_shortcut_item.currency_symbol, fav_shortcut_item.restaurant_info.name, fav_shortcut_item.restaurant_info.address, fav_shortcut_item.restaurant_info.distance, fav_shortcut_item.item_image, fav_shortcut_item.restaurant_info.id, isShortcut(fav_shortcut_item.item_id)));
					}
					GeneralListView.ItemsSource = fav_general_item_list;
					ExpandedGeneralSize = 80 * fav_general_item_list.Count;
					GeneralListLayout.HeightRequest = IsGeneralExpanded ? ExpandedGeneralSize : 0;

					if (Globals.Config.CurrentVenue != null) {
						List<FavouriteItem> fav_cur_item_list = new List<FavouriteItem> ();
						foreach (UserFavouritesItemData fav_cur_favourite in current_favourites) {
							fav_cur_item_list.Add (new FavouriteItem (fav_cur_favourite.item_id, "1", fav_cur_favourite.item_name, fav_cur_favourite.item_price + " " + fav_cur_favourite.currency_symbol, fav_cur_favourite.restaurant_info.name, fav_cur_favourite.restaurant_info.address, fav_cur_favourite.restaurant_info.distance, fav_cur_favourite.item_image, fav_cur_favourite.restaurant_info.id, isShortcut(fav_cur_favourite.item_id)));
						}
						ThisVenueListView.ItemsSource = fav_cur_item_list;
						ExpandedThisVenueSize = 80 * fav_cur_item_list.Count;
						ThisVenueListLayout.HeightRequest = IsThisVenueExpanded ? ExpandedThisVenueSize : 0;

						List<FavouriteItem> fav_venue_item_list = new List<FavouriteItem> ();
						foreach (UserFavouritesItemData fav_venue in favorite_venue_type) {
							fav_venue_item_list.Add(new FavouriteItem (fav_venue.item_id, "1", fav_venue.item_name, fav_venue.item_price + " " + fav_venue.currency_symbol, fav_venue.restaurant_info.name, fav_venue.restaurant_info.address, fav_venue.restaurant_info.distance, fav_venue.item_image, fav_venue.restaurant_info.id, isShortcut(fav_venue.item_id)));
						}
						ThisVenueTypeListView.ItemsSource = fav_venue_item_list;
						ExpandedThisVenueTypeSize = 80 * fav_venue_item_list.Count;
						ThisVenueTypeListLayout.HeightRequest = IsThisVenueTypeExpanded ? ExpandedThisVenueTypeSize : 0;
					}
				}
				catch (Exception ex)
				{
				}
			});
		}	

		private void AddItemsToVenueUI(List<FavouriteItem> favorite_shortcut)
		{
			try{
			FavouritesVenueListView.ItemsSource = favorite_shortcut;

			FavouritesVenueListView.ItemSelected += async (object sender, SelectedItemChangedEventArgs e) => {
				if (e.SelectedItem == null)
					return;

				((ListView)sender).SelectedItem = null;

				FavouriteItem selectedItem = e.SelectedItem as FavouriteItem;
				foreach (RestaurantInfo info in fav_restaurants)
				{
					if (info.id == selectedItem.Id)
					{
						var venueItemPage = new VenueItemPage(info, PageShowingEvent);
						venueItemPage.MasterPage = MasterPage;
						await Navigation.PushAsync(venueItemPage);
						break;
					}
				}
			};
			}
			catch (Exception ex) {
			}
		}

		private StackLayout GetFavouriteMenuItem(bool isShortcutListView = false)
		{
			var DeleteIconImage = new DarkIceImage
			{
				WidthRequest = (IsEditing) ? 40 : 0,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Source = ImageSource.FromFile("red_minus.png"),
				IsShortCutList = isShortcutListView,
			};

			DeleteIconImage.SetBinding (DarkIceImage.RestaurantIdProperty, "ExtraId");
			DeleteIconImage.SetBinding (DarkIceImage.MenuIdProperty, "Id");
			DeleteIconImage.SetBinding (DarkIceImage.IsShortCutProperty, "IsShortcut");

			DeleteIconImage.DeleteDelegate = DeleteFavMenu_Clicked;

			var ItemIconImage = new Image
			{
				WidthRequest = 60,
				VerticalOptions = LayoutOptions.FillAndExpand
			};
			ItemIconImage.SetBinding(Image.SourceProperty, "Icon");

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
			ItemTextLabel.SetBinding(Label.TextProperty, "Name");

			var ItemRestaurantName = new DILabel {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				IsDefaultLabel = true,
				Lines = 1,
				HeightRequest = 20,
				TextColor = Color.Gray,
				Font = Font.SystemFontOfSize (NamedSize.Medium)
			};
			ItemRestaurantName.SetBinding(Label.TextProperty, "RestaurantName");

			var ItemTextPriceLabel = new DILabel {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				IsDefaultLabel = true,
				Lines = 1,
				TextColor = Color.Gray,
				Font = Font.SystemFontOfSize (NamedSize.Medium)
			};
			ItemTextPriceLabel.SetBinding(Label.TextProperty, "Price");

			var ShortcutBtn = new CustomButton {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 25,
				Text = "Shortcut",
				TextColor = Color.White,
				BorderRadius = 2,
				BorderWidth = 2,
				BorderColor = Color.Gray,
				Font = Font.SystemFontOfSize(8),
			};

			ShortcutBtn.SetBinding(CustomButton.BackgroundColorProperty, "ShortcutBgColor");
			ShortcutBtn.SetBinding (CustomButton.DataItemIDProperty, "Id");
			ShortcutBtn.SetBinding (CustomButton.DataItemTypeProperty, "Type");

			ShortcutBtn.Clicked += (object sender, EventArgs e) => {
				var view = sender as CustomButton;
				if (view.ItemID != "" && view.ItemType != "")
				{
					DoShortcut(view.ItemID, view.ItemType);
				}
			};

			var ItemDistanceLabel = new DILabel {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.End,
				HeightRequest = 20,
				YAlign = TextAlignment.Center,
				IsDefaultLabel = true,
				Lines = 1,
				TextColor = Color.Gray,
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				LineBreakMode = LineBreakMode.TailTruncation,
			};
			ItemDistanceLabel.SetBinding(Label.TextProperty, "Distance");

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
					DeleteIconImage,
					ItemIconImage, 
					new StackLayout{
						Orientation = StackOrientation.Vertical,
						Padding = 0,
						Spacing = 0,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						VerticalOptions = LayoutOptions.FillAndExpand,
						Children = {
							ItemTextLabel,
							ItemTextPriceLabel,
							ItemRestaurantName,
						}
					},
					new StackLayout{
						Orientation = StackOrientation.Vertical,
						Padding = 0,
						Spacing = 0,
						HorizontalOptions = LayoutOptions.End,
						VerticalOptions = LayoutOptions.FillAndExpand,
						WidthRequest = (IsEditing) ? 0 : 50,
						Children = {
							ShortcutBtn,
							ItemDistanceLabel
						}
					}
				}
			};

			return StackItem;
		}

		private StackLayout GetFavouriteVenueItem()
		{
			var DeleteIconImage = new DarkIceImage
			{
				WidthRequest = (IsEditing) ? 40 : 0,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Source = ImageSource.FromFile("red_minus.png"),
			};

			DeleteIconImage.SetBinding (DarkIceImage.DataItemIDProperty, "ID");

			DeleteIconImage.SetBinding (DarkIceImage.RestaurantIdProperty, "Id");
			DeleteIconImage.SetBinding (DarkIceImage.MenuIdProperty, "ExtraId");
			DeleteIconImage.SetBinding (DarkIceImage.IsShortCutProperty, "IsShortcut");

			DeleteIconImage.DeleteDelegate = DeleteFavVenue_Clicked;

			var ItemIconImage = new Image
			{
				WidthRequest = 60,
				VerticalOptions = LayoutOptions.FillAndExpand
			};
			ItemIconImage.SetBinding(Image.SourceProperty, "Icon");

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
			ItemTextLabel.SetBinding(Label.TextProperty, "Name");

			var ItemTextDetailLabel = new DILabel {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				IsDefaultLabel = true,
				TextColor = Color.Gray,
				Font = Font.SystemFontOfSize (NamedSize.Medium)
			};
			ItemTextDetailLabel.SetBinding(Label.TextProperty, "Address");

			var ItemDistanceLabel = new DILabel {
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.FillAndExpand,
				WidthRequest = (IsEditing) ? 0 : 50,
				YAlign = TextAlignment.Center,
				IsDefaultLabel = true,
				Lines = 1,
				TextColor = Color.Gray,
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				LineBreakMode = LineBreakMode.TailTruncation,
			};
			ItemDistanceLabel.SetBinding(Label.TextProperty, "Distance");

			var StackItem = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Padding = new Thickness(20, 10, 20, 10),
				Spacing = 10,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				MinimumHeightRequest = 80,
				//HeightRequest = 80,
				Children =
				{
					DeleteIconImage,
					ItemIconImage, 
					new StackLayout{
						Orientation = StackOrientation.Vertical,
						Padding = 0,
						Spacing = 0,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						VerticalOptions = LayoutOptions.FillAndExpand,
						Children = {
							ItemTextLabel,
							new StackLayout{
								Orientation = StackOrientation.Horizontal,
								Padding = 0,
								Spacing = 0,
								HorizontalOptions = LayoutOptions.FillAndExpand,
								VerticalOptions = LayoutOptions.FillAndExpand,
								Children = {
									ItemTextDetailLabel,
									ItemDistanceLabel
								}
							}
						}
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

		void GeneralFav_Clicked()
		{
			IsGeneralExpanded = !IsGeneralExpanded;
			GeneralExpandButton.Text = (IsGeneralExpanded) ? "-" : "+";

			GeneralListLayout.HeightRequest = (IsGeneralExpanded) ? ExpandedGeneralSize : 0;
		}

		void ShortcutFav_Clicked()
		{
			IsShortcutExpanded = !IsShortcutExpanded;
			ShortcutExpandButton.Text = (IsShortcutExpanded) ? "-" : "+";

			ShortcutListLayout.HeightRequest = (IsShortcutExpanded) ? ExpandedShortcutSize : 0;
		}

		void ThisVenueFav_Clicked(){
			IsThisVenueExpanded = !IsThisVenueExpanded;
			ThisVenueExpandButton.Text = (IsThisVenueExpanded) ? "-" : "+";
			ThisVenueListLayout.HeightRequest = (IsThisVenueExpanded) ? ExpandedThisVenueSize : 0;
		}

		void ThisVenueTypeFav_Clicked(){
			IsThisVenueTypeExpanded = !IsThisVenueTypeExpanded;
			ThisVenueTypeExpandButton.Text = (IsThisVenueTypeExpanded) ? "-" : "+";
			ThisVenueTypeListLayout.HeightRequest = (IsThisVenueTypeExpanded) ? ExpandedThisVenueTypeSize : 0;
		}

		async void DoShortcut(String id, String type)
		{
			ShowLoading ();

			RestaurantInfo res_info = GetFavItemData (id);
			if (res_info != null) {
				var responseShortcut = await FavouriteService.SetFavouriteRestaurantShortcutItem (GetUserID(), GetUserToken(),  res_info.id, id);
				bool isSuccessShortcut = FavouriteService.HasSuccessResult (responseShortcut);
				if (isSuccessShortcut) {
					await DisplayAlert ("Success", responseShortcut.message, "OK");
				} else {
					await DisplayAlert ("Failure", responseShortcut.message, "OK");
				}
			}

			HideLoading ();

			ChangeFavouriteCategory (curCategory);
		}

		RestaurantInfo GetFavItemData(String id)
		{
			foreach (UserFavouritesItemData info in favorite_venue_type)
			{
				if (id == info.item_id) {
					return info.restaurant_info;
				}
			}
			foreach (UserFavouritesItemData info in favorite_item)
			{
				if (id == info.item_id) {
					return info.restaurant_info;
				}
			}
			foreach (UserFavouritesItemData info in favorite_shortcut)
			{
				if (id == info.item_id) {
					return info.restaurant_info;
				}
			}
			foreach (UserFavouritesItemData info in current_favourites)
			{
				if (id == info.item_id) {
					return info.restaurant_info;
				}
			}

			return null;
		}

		async void SelectFavMenuItem(object item)
		{
			if (item == null)
				return;

			FavouriteItem data = item as FavouriteItem;

			foreach (UserFavouritesItemData info in favorite_venue_type)
			{
				if (data.Id == info.item_id) {
					var venueItemPage = new VenueItemPage(info.restaurant_info, PageShowingEvent, info.item_id);
					venueItemPage.MasterPage = MasterPage;
					await Navigation.PushAsync(venueItemPage);
					return;
				}
			}
			foreach (UserFavouritesItemData info in favorite_item)
			{
				if (data.Id == info.item_id) {
					var venueItemPage = new VenueItemPage(info.restaurant_info, PageShowingEvent, info.item_id);
					venueItemPage.MasterPage = MasterPage;
					await Navigation.PushAsync(venueItemPage);
					return;
				}
			}
			foreach (UserFavouritesItemData info in favorite_shortcut)
			{
				if (data.Id == info.item_id) {
					var venueItemPage = new VenueItemPage(info.restaurant_info, PageShowingEvent, info.item_id);
					venueItemPage.MasterPage = MasterPage;
					await Navigation.PushAsync(venueItemPage);
					return;
				}
			}
			foreach (UserFavouritesItemData info in current_favourites)
			{
				if (data.Id == info.item_id) {
					var venueItemPage = new VenueItemPage(info.restaurant_info, PageShowingEvent, info.item_id);
					venueItemPage.MasterPage = MasterPage;
					await Navigation.PushAsync(venueItemPage);
					return;
				}
			}
		}
		#endregion

		#region Methods
		bool isShortcut(string item_id){
			if (favorite_shortcut == null || favorite_shortcut.Count == 0) {
				return false;
			}
			return (favorite_shortcut.FirstOrDefault (p => p.item_id == item_id) == null) ? false : true;
		}

		void EditFav_Clicked(){
			if (IsEditing == true) {
				IsEditing = false;
				EditLabel.Text = "Edit";
			} else {
				IsEditing = true;
				EditLabel.Text = "Done";
			}

			if (curCategory == 0) {
				if (current_favourites == null)
					current_favourites = new List<UserFavouritesItemData> ();
				if (favorite_shortcut == null)
					favorite_shortcut = new List<UserFavouritesItemData> ();
				if (favorite_item == null)
					favorite_item = new List<UserFavouritesItemData> ();
				if (favorite_venue_type == null)
					favorite_venue_type = new List<UserFavouritesItemData> ();
				AddItemsToItemUI (current_favourites, favorite_shortcut, favorite_item, favorite_venue_type);
			} else if (curCategory == 1) {
				if (fav_restaurants != null) {
					List<FavouriteItem> fav_items = new List<FavouriteItem> ();
					foreach (RestaurantInfo info in fav_restaurants)
						fav_items.Add(new FavouriteItem(info.id, "0", info.name, "", "", info.address, info.distance, info.image, "", false));
					AddItemsToVenueUI (fav_items);
				}
			}
		}

		private async void DeleteFavMenu_Clicked(String res_id, String menu_id, bool isshortcut, bool isshortcutlistview)
		{
			ShowLoading ();

			BaseResult result;
			if (isshortcutlistview) {
				if (isshortcut)
					result = await FavouriteService.SetFavouriteRestaurantShortcutItem (GetUserID (), GetUserToken (), res_id, menu_id);
			} else {
				if (isshortcut)
					result = await FavouriteService.SetFavouriteRestaurantShortcutItem (GetUserID (), GetUserToken (), res_id, menu_id);
				result = await FavouriteService.SetFavouriteRestaurantItem (GetUserID(), GetUserToken(), res_id, menu_id);
			}

			String curVenueId = "";
			if (Globals.Config.CurrentVenue != null)
				curVenueId = Globals.Config.CurrentVenue.id;
			var responseVenue = await FavouriteService.DisplayUserFavourites (GetUserID(), GetUserToken(), App.UserLatitude.ToString(), App.UserLongitude.ToString(), curVenueId, "");
			bool isSuccessVenue = FavouriteService.HasSuccessResult (responseVenue);
			favorite_venue_type = new List<UserFavouritesItemData> ();
			favorite_item = new List<UserFavouritesItemData> ();
			favorite_shortcut = new List<UserFavouritesItemData>();
			current_favourites = new List<UserFavouritesItemData>();

			if (isSuccessVenue) {
				favorite_venue_type = responseVenue.favourites_from_this_venue_type;
				favorite_item = responseVenue.user_favourites_item;
				favorite_shortcut = responseVenue.user_favourites_shortcut;
				current_favourites = responseVenue.current_restaurant_favourites;
			}

			HideLoading ();

			if (current_favourites == null)
				current_favourites = new List<UserFavouritesItemData> ();
			if (favorite_shortcut == null)
				favorite_shortcut = new List<UserFavouritesItemData> ();
			if (favorite_item == null)
				favorite_item = new List<UserFavouritesItemData> ();
			if (favorite_venue_type == null)
				favorite_venue_type = new List<UserFavouritesItemData> ();
			AddItemsToItemUI (current_favourites, favorite_shortcut, favorite_item, favorite_venue_type);
		}

		private async void DeleteFavVenue_Clicked(String res_id, String menu_id, bool isshortcut, bool isshortcutlistview)
		{
			ShowLoading ();

			BaseResult result;
			result = await FavouriteService.SetFavouriteRestaurant (GetUserID(), GetUserToken(), res_id);

			var responseVenue = await FavouriteService.DisplayFavouritesRestaurant (GetUserID(), GetUserToken(), App.UserLatitude.ToString(), App.UserLongitude.ToString(), "", "");
			bool isSuccessVenue = FavouriteService.HasSuccessResult (responseVenue);
			Console.WriteLine (res_id + " called");
			if (isSuccessVenue) {
				fav_restaurants = responseVenue.user_favourites_restaurants;
				List<FavouriteItem> fav_items = new List<FavouriteItem> ();
				foreach (RestaurantInfo info in fav_restaurants)
					fav_items.Add (new FavouriteItem (info.id, "0", info.name, "", "", info.address, info.distance, info.image, "", false));
				AddItemsToVenueUI (fav_items);
			} else {
				fav_restaurants = new List<RestaurantInfo> ();
				List<FavouriteItem> fav_items = new List<FavouriteItem> ();
				AddItemsToVenueUI (fav_items);
			}

			HideLoading ();
		}
		#endregion
	}
}

