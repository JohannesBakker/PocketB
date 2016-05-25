using System;
using Xamarin.Forms;
using PocketButler.Controls;
using System.Collections.Generic;
using PocketButler.Services;
using ServiceStack.Text;
using Xamarin.Forms.Maps;
using System.Threading.Tasks;

namespace PocketButler
{
	public class VenueTypeSliderPage : BasePage
	{
		class CustomVenueItem
		{
			public int Id;
			public String Text { get; set; }
			public String TextDetail { get; set; }
			public String Icon { get; set; }

			public CustomVenueItem(int id, String text, String textdetail, String icon)
			{
				this.Id = id;
				this.Text = text;
				this.TextDetail = textdetail;
				this.Icon = icon;
			}
		}

		class TypeItem
		{
			public String Id;
			public String Currency { get; set; }
			public String Name { get; set; }
			public String Price { get; set; }

			public TypeItem(String id, String currency, String name, String price)
			{
				this.Id = id;
				this.Currency = currency;
				this.Name = name;
				this.Price = price;
			}
		}

		class TypeItemData{
			public String TypeName { get; set;}
			public List<TypeItem> Items { get; set;}
			public String SelectedPrice { get; set;}
		}

		#region PRIVATE MEMBERS
		protected AbsoluteLayout UILayout { get; set; }
		protected AbsoluteLayout MainLayout { get; set; }
		protected StackLayout InfoLayout { get; set; }
		DarkIceImage InfoImage { get; set; }
		Label InfoTextLabel { get; set; }
		SliderControl ItemSliderControl {get; set; }
		ScrollView InfoScrollView;
		StackLayout InfoContentStack;
		DILabel BasketTotalPriceLabel;

		RestaurantInfo RestaurantData = null;

		bool isPopularExist = false;
		bool isShowInfo = false;
		Rectangle InfoImageBounds;

		DarkIceImage favouriteImage;
		#endregion

		public VenueTypeSliderPage (RestaurantInfo restaurantInfo, Action RefreshEvent)
		{
			restaurantInfo.CheckOrderStatus ();

			Globals.Config.SliderPageRestaurant = restaurantInfo;
			BackAppearingEvent = RefreshEvent;
			PageShowingEvent += OnPageShowing;
			RestaurantData = restaurantInfo;

			Globals.Config.RestaurantId = RestaurantData.id;
			Globals.Config.RestaurantName = RestaurantData.name;
			Globals.Config.RestaurantImage = RestaurantData.image;

			Title = "";

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

			BuildUI ();

			if (String.IsNullOrEmpty(Globals.Config.FavoriteItemid) == false)
				Navigation.PushAsync (new VenueCustomizePage (Globals.Config.FavoriteItemid, RestaurantData.CanPlaceOrder(), RestaurantData.VenueStatusText(), "", -1, false, PageShowingEvent));
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
			var TitleLogoImage = new DarkIceImage
			{
				Source = RestaurantData.image,
				Aspect = Aspect.AspectFit,
				HorizontalOptions = LayoutOptions.FillAndExpand
			};

			var BucketImage = new DarkIceImage { Source = "navbar_cart.png", Aspect = Aspect.AspectFit };
			BasketTotalPriceLabel = new DILabel {
				Text = "",
				TextColor = Color.FromRgb (171, 146, 91),
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				YAlign = TextAlignment.Center
			};

			BasketTotalPriceLabel.BindingContext = Globals.Config.PaymentInfo;
			BasketTotalPriceLabel.SetBinding (Label.TextProperty, "Price");

			BucketImage.Tapped += AddToBucket_Clicked;
			BasketTotalPriceLabel.Tapped += AddToBucket_Clicked;

			if (RestaurantData.CanPlaceOrder ()) {
				var CheckoutLabel = new DILabel {
					Text = "Checkout",
					TextColor = Color.FromRgb (171, 146, 91),
					Font = Font.SystemFontOfSize (NamedSize.Small),
					YAlign = TextAlignment.Center,
					XAlign = TextAlignment.Center,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					HeightRequest = 20,
				};

				CheckoutLabel.Tapped += AddToBucket_Clicked;

				var BucketStack = new StackLayout {
					Orientation = StackOrientation.Vertical,
					Padding = 0,
					Spacing = 0,
					Children = {
						new StackLayout{
							Orientation = StackOrientation.Horizontal,
							Padding = 0,
							Spacing = 0,
							Children = {
								BucketImage,
								BasketTotalPriceLabel
							}
						},
						CheckoutLabel
					}
				};
				MakeCustomNavigationBar (UILayout, MenuImage, BucketStack, TitleLogoImage, true);
			} else {
				var StatusLabel = new DILabel
				{
					Text = RestaurantData.VenueStatusText(),
					TextColor = RestaurantData.GetStatusColor(),
					YAlign = TextAlignment.Center,
					Font = Font.SystemFontOfSize(NamedSize.Medium)
				};
				MakeCustomNavigationBar (UILayout, MenuImage, StatusLabel, TitleLogoImage, true);
			}

			MenuImage.Tapped += MenuButton_Clicked;

			var ItemLabel = new DILabel
			{
				Text = RestaurantData.name,
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
				TextColor = Color.White,
				Lines = 2,
				Font = Font.SystemFontOfSize(NamedSize.Large, FontAttributes.Bold)
			};
			AbsoluteLayout.SetLayoutFlags(ItemLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ItemLabel, new Rectangle(0, 0.1, 1, 0.1));
			MainLayout.Children.Add(ItemLabel);

			ItemSliderControl = new SliderControl ();
			ItemSliderControl.is_online = RestaurantData.CanPlaceOrder();

			ItemSliderControl.CategoryClicked = SliderListView_Category_Clicked;
			ItemSliderControl.CategorySubItemClicked = SliderListView_Category_SubItem_Clicked;
			ItemSliderControl.CategoryFavItemClicked = SliderListView_Category_FavItem_Clicked;
			ItemSliderControl.CategoryAddItemClicked = SliderListView_Category_AddItem_Clicked;

			List<MenuCategory> sliderMenuCategories = new List<MenuCategory> ();
			foreach (MenuCategory categorItem in RestaurantData.menu_categories)
				sliderMenuCategories.Add (categorItem);
			MenuCategory _popularCategory = new MenuCategory ();
			List<SubCategory> _subCategoryList = new List<SubCategory> ();

			// Make Popular Category
			foreach (MenuCategory _category in RestaurantData.menu_categories) {
				List<MenuItem> popularMenus = new List<MenuItem> ();
				if (_category.is_menuitem_exist.Equals ("1")) {
					foreach (MenuItem item in _category.menuitems) {
						if (item.is_popular.Equals ("1")) {
							item.IsPopularCategory = true;
							item.Category_Id = _category.category_id;
							popularMenus.Add (item);
						}
					}
				}

				if (_category.is_subcat_exists.Equals("1")){
					foreach (SubCategory _subcategory in _category.subcategories) {
						if (_subcategory.menuitems != null) {
							foreach (MenuItem item in _subcategory.menuitems) {
								if (item.is_popular.Equals ("1")) {
									item.IsPopularCategory = true;
									item.Category_Id = _category.category_id;
									popularMenus.Add (item);
								}
							}
						}
					}
				}

				if (popularMenus.Count > 0) {
					SubCategory _subCategory = new SubCategory ();
					_subCategory.menuitems = popularMenus;
					_subCategory.sub_category_id = "";
					_subCategory.sub_category_name = _category.category_name;

					_subCategoryList.Add (_subCategory);
				}
			}

			if (_subCategoryList.Count > 0) {
				_popularCategory.category_name = "Popular";
				_popularCategory.category_id = "0";
				_popularCategory.is_menuitem_exist = "0";
				_popularCategory.is_subcat_exists = "1";
				_popularCategory.subcategories = _subCategoryList;

				isPopularExist = true;
				sliderMenuCategories.Insert (0, _popularCategory);
			}

			ItemSliderControl.ListItems = sliderMenuCategories;

			AbsoluteLayout.SetLayoutFlags(ItemSliderControl, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ItemSliderControl, new Rectangle(0, 0.7, 1, 0.73));
			MainLayout.Children.Add(ItemSliderControl);

			InfoImage = new DarkIceImage
			{
				Source = ImageSource.FromFile ("btn_info.png"),
				Aspect = Aspect.AspectFit,
				IsSwipeAction = true,
			};
			AbsoluteLayout.SetLayoutFlags(InfoImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(InfoImage, new Rectangle(0.95, 1, 0.3, 0.07));
			MainLayout.Children.Add(InfoImage);

			InfoTextLabel = new Label
			{
				Text = "Info",
				TextColor = Color.FromRgb(100, 100, 100),
				FontSize = 20,
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
			};
			AbsoluteLayout.SetLayoutFlags(InfoTextLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(InfoTextLabel, new Rectangle(0.95, 1, 0.3, 0.07));
			MainLayout.Children.Add(InfoTextLabel);

			InfoImage.Tapped += InfoImage_Clicked;

			favouriteImage = new DarkIceImage {
				Source = RestaurantData.is_favourite.Equals("1") ? "favorite_on.png" : "favorite_off.png",
				HorizontalOptions = LayoutOptions.EndAndExpand
			};

			favouriteImage.Tapped += Favourite_Clicked;

			StackLayout categoryLayout = new StackLayout {
				Orientation = StackOrientation.Vertical,
				Spacing = 0,
				Padding = 0,
			};

			if (RestaurantData.menu_categories != null && RestaurantData.menu_categories.Count > 0) {
				ListView CategoryListView = new ListView {
					HasUnevenRows = true,

					// Source of data items.
					ItemsSource = RestaurantData.menu_categories,

					ItemTemplate = new DataTemplate (() => {
						// Return an assembled ViewCell.
						var viewCell = new ViewCell {
							View = GetCategoryItem ()
						};

						viewCell.Height = 40;
						return viewCell;
					})
				};

				CategoryListView.ItemSelected += (object sender, SelectedItemChangedEventArgs e) => {
					if (e.SelectedItem == null)
						return;

					MenuCategory selectedData = e.SelectedItem as MenuCategory;
					int index = RestaurantData.menu_categories.IndexOf(selectedData);
					if (RestaurantData.menu_categories.Count > 1)
					{
						index++;
						if (isPopularExist == true)
							index++;
					}
					ItemSliderControl.CategoryIndex = index;
					ItemSliderControl.CategoryIndexChanged = !ItemSliderControl.CategoryIndexChanged;
					CategoryListView.SelectedItem = null;
					InfoImage_Clicked();
				};

				categoryLayout.Children.Add (CategoryListView);
				categoryLayout.HeightRequest = 40 * RestaurantData.menu_categories.Count;
			}

			double dLat = 0, dLon = 0;
			double.TryParse (RestaurantData.lat, out dLat);
			double.TryParse (RestaurantData.lng, out dLon);

			AbsoluteLayout MapVenueLayout = new AbsoluteLayout {
				WidthRequest = 100,
				HeightRequest = 100,
				VerticalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.Transparent
			};

			var MapVenue = new Map(MapSpan.FromCenterAndRadius(new Position(dLat, dLon), Distance.FromMiles(0.1))) {
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				IsShowingUser = false,
				IsEnabled = false,
				MapType = MapType.Street,
				HasZoomEnabled = false,
			};

			MapVenue.Pins.Add (new Pin {
				Position = new Position(dLat, dLon),
				Label = RestaurantData.name,
			});

			AbsoluteLayout.SetLayoutFlags(MapVenue, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(MapVenue, new Rectangle(0, 0, 1, 1));
			MapVenueLayout.Children.Add(MapVenue);

			var MapVenueButton = new Button {
				Text = "",
				BackgroundColor = Color.Transparent,
				BorderColor = Color.Transparent,
				BorderRadius = 0,
				BorderWidth = 0,
			};

			MapVenueButton.Clicked += (object sender, EventArgs e) => {
				MapVenueImage_Clicked();
			};

			AbsoluteLayout.SetLayoutFlags(MapVenueButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(MapVenueButton, new Rectangle(0, 0, 1, 1));
			MapVenueLayout.Children.Add(MapVenueButton);

			String[] openingHoursArr = new String[7];
			for (int i = 0; i < 7; i++)
				openingHoursArr [i] = "-";

			if (RestaurantData.opening_hours != null) {
				for (int i = 0; i < RestaurantData.opening_hours.Count; i++) {
					if (RestaurantData.opening_hours [i].day == "Monday") {
						if (RestaurantData.opening_hours [i].is_closed.Equals ("1"))
							openingHoursArr [0] = "Closed";
						else
							openingHoursArr [0] = RestaurantData.opening_hours[i].timing;
					}
					else if (RestaurantData.opening_hours [i].day == "Tuesday") {
						if (RestaurantData.opening_hours [i].is_closed.Equals ("1"))
							openingHoursArr [1] = "Closed";
						else
							openingHoursArr [1] = RestaurantData.opening_hours[i].timing;
					}
					else if (RestaurantData.opening_hours [i].day == "Wednesday") {
						if (RestaurantData.opening_hours [i].is_closed.Equals ("1"))
							openingHoursArr [2] = "Closed";
						else
							openingHoursArr [2] = RestaurantData.opening_hours[i].timing;
					}
					else if (RestaurantData.opening_hours [i].day == "Thursday") {
						if (RestaurantData.opening_hours [i].is_closed.Equals ("1"))
							openingHoursArr [3] = "Closed";
						else
							openingHoursArr [3] = RestaurantData.opening_hours[i].timing;
					}
					else if (RestaurantData.opening_hours [i].day == "Friday") {
						if (RestaurantData.opening_hours [i].is_closed.Equals ("1"))
							openingHoursArr [4] = "Closed";
						else
							openingHoursArr [4] = RestaurantData.opening_hours[i].timing;
					}
					else if (RestaurantData.opening_hours [i].day == "Saturday") {
						if (RestaurantData.opening_hours [i].is_closed.Equals ("1"))
							openingHoursArr [5] = "Closed";
						else
							openingHoursArr [5] = RestaurantData.opening_hours[i].timing;
					}
					else if (RestaurantData.opening_hours [i].day == "Sunday") {
						if (RestaurantData.opening_hours [i].is_closed.Equals ("1"))
							openingHoursArr [6] = "Closed";
						else
							openingHoursArr [6] = RestaurantData.opening_hours[i].timing;
					}

				}
			}

			InfoContentStack = new StackLayout {
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Spacing = 10,
				Padding = 0,
				Children = {
					new StackLayout{
						Orientation = StackOrientation.Horizontal,
						Padding = new Thickness(10, 20, 10, 10),
						Children = {
							new DILabel{
								Text = RestaurantData.name,
								TextColor = Color.White,
								YAlign = TextAlignment.Center,
								HorizontalOptions = LayoutOptions.FillAndExpand,
								Font = Font.SystemFontOfSize(20),
								WidthRequest = 300
							},
							favouriteImage
						}
					},
					new StackLayout{
						BackgroundColor = Color.FromRgb(63, 54, 32),	
						Orientation = StackOrientation.Horizontal,
						Padding = new Thickness(10, 5, 10, 5),
						Spacing = 20,
						Children = {
							new DILabel{
								Text = "Categories",
								TextColor = Color.White,
								YAlign = TextAlignment.Center,
								HorizontalOptions = LayoutOptions.FillAndExpand
							},
						}
					},
					categoryLayout,
					new StackLayout{
						BackgroundColor = Color.FromRgb(63, 54, 32),	
						Orientation = StackOrientation.Horizontal,
						Padding = new Thickness(10, 5, 10, 5),
						Children = {
							new DILabel{
								Text = RestaurantData.name,
								TextColor = Color.White,
								YAlign = TextAlignment.Center,
								HorizontalOptions = LayoutOptions.FillAndExpand,
								WidthRequest = 250,
							},
							new DILabel{
								Text = RestaurantData.distance,
								TextColor = Color.White,
								YAlign = TextAlignment.Center,
								HorizontalOptions = LayoutOptions.EndAndExpand
							},
						}
					},
					new StackLayout{
						Orientation = StackOrientation.Horizontal,
						Padding = new Thickness(10, 0, 10, 0),
						Children = {
							new DILabel{
								Text = "Restaurant",
								TextColor = Color.White, 
								YAlign = TextAlignment.Center, 
								HorizontalOptions = LayoutOptions.FillAndExpand 
							},
						}
					},
					new StackLayout{
						Orientation = StackOrientation.Horizontal,
						Padding = new Thickness(0, 5, 0, 5),
						Children = {
							new DarkIceImage { 
								Source = RestaurantData.image_large, 
								HorizontalOptions = LayoutOptions.FillAndExpand,
								HeightRequest = 150,
							}
						}
					},
					new StackLayout{
						Orientation = StackOrientation.Horizontal,
						Padding = new Thickness(10, 0, 10, 0),
						Children = {
							new StackLayout{
								Orientation = StackOrientation.Vertical,
								HorizontalOptions = LayoutOptions.FillAndExpand,
								Children = {
									new DILabel{ 
										Text = RestaurantData.address, 
										TextColor = Color.White, 
										YAlign = TextAlignment.Center, 
										IsHtmlLabel = true,
										HorizontalOptions = LayoutOptions.FillAndExpand 
									},
									new DILabel{ 
										Text = RestaurantData.phone_no, 
										TextColor = Color.White, 
										IsHtmlLabel = true,
										YAlign = TextAlignment.Center, 
										HorizontalOptions = LayoutOptions.FillAndExpand 
									},
									new DILabel{ 
										Text = RestaurantData.website, 
										TextColor = Color.White, 
										IsHtmlLabel = true,
										YAlign = TextAlignment.Center, 
										HorizontalOptions = LayoutOptions.FillAndExpand 
									},
								}
							},
							new StackLayout{
								Orientation = StackOrientation.Vertical,
								WidthRequest = 100,
								HeightRequest = 100,
								VerticalOptions = LayoutOptions.FillAndExpand,
								Children = {
									MapVenueLayout
								}
							},
						}
					},
					new StackLayout{
						Padding = new Thickness(10, 20, 10, 5),
						Orientation = StackOrientation.Horizontal,
						Children = {
							new DILabel{ 
								Text = "Opening Hours:", 
								TextColor = Color.White, 
								YAlign = TextAlignment.Center, 
								HorizontalOptions = LayoutOptions.FillAndExpand 
							},
						}
					},
				}
			};

			if (RestaurantData.opening_hours != null && RestaurantData.opening_hours.Count > 0) {
				InfoContentStack.Children.Add (
					new StackLayout{
						Orientation = StackOrientation.Horizontal,
						Padding = new Thickness(10, 0, 20, 10),
						Spacing = 5,
						Children = {
							new StackLayout{
								Orientation = StackOrientation.Vertical,
								HorizontalOptions = LayoutOptions.FillAndExpand,
								Padding = 0,
								Children = {
									new DILabel{ 
										Text = "Monday:", 
										TextColor = Color.White, 
										YAlign = TextAlignment.Center, 
										HorizontalOptions = LayoutOptions.FillAndExpand 
									},
									new DILabel{ 
										Text = "Tuesday:", 
										TextColor = Color.White, 
										YAlign = TextAlignment.Center, 
										HorizontalOptions = LayoutOptions.FillAndExpand 
									},
									new DILabel{ 
										Text = "Wednesday:", 
										TextColor = Color.White, 
										YAlign = TextAlignment.Center, 
										HorizontalOptions = LayoutOptions.FillAndExpand 
									},
									new DILabel{ 
										Text = "Thursday:", 
										TextColor = Color.White, 
										YAlign = TextAlignment.Center, 
										HorizontalOptions = LayoutOptions.FillAndExpand 
									},
									new DILabel{ 
										Text = "Friday:", 
										TextColor = Color.White, 
										YAlign = TextAlignment.Center, 
										HorizontalOptions = LayoutOptions.FillAndExpand 
									},
									new DILabel{ 
										Text = "Saturday:", 
										TextColor = Color.White, 
										YAlign = TextAlignment.Center, 
										HorizontalOptions = LayoutOptions.FillAndExpand 
									},
									new DILabel{ 
										Text = "Sunday:", 
										TextColor = Color.White, 
										YAlign = TextAlignment.Center, 
										HorizontalOptions = LayoutOptions.FillAndExpand 
									}
								}
							},
							new StackLayout{
								Orientation = StackOrientation.Vertical,
								HorizontalOptions = LayoutOptions.End,
								WidthRequest = 150,
								Padding = 0,
								Children = {
									new DILabel{ 
										Text = openingHoursArr[0], 
										TextColor = Color.White, 
										YAlign = TextAlignment.Center, 
										XAlign = TextAlignment.End,
										HorizontalOptions = LayoutOptions.FillAndExpand 
									},
									new DILabel{ 
										Text = openingHoursArr[1],
										TextColor = Color.White, 
										YAlign = TextAlignment.Center, 
										XAlign = TextAlignment.End,
										HorizontalOptions = LayoutOptions.FillAndExpand 
									},
									new DILabel{ 
										Text = openingHoursArr[2],
										TextColor = Color.White, 
										YAlign = TextAlignment.Center, 
										XAlign = TextAlignment.End,
										HorizontalOptions = LayoutOptions.FillAndExpand 
									},
									new DILabel{ 
										Text = openingHoursArr[3], 
										TextColor = Color.White, 
										YAlign = TextAlignment.Center, 
										XAlign = TextAlignment.End,
										HorizontalOptions = LayoutOptions.FillAndExpand 
									},
									new DILabel{ 
										Text = openingHoursArr[4], 
										TextColor = Color.White, 
										YAlign = TextAlignment.Center, 
										XAlign = TextAlignment.End,
										HorizontalOptions = LayoutOptions.FillAndExpand 
									},
									new DILabel{ 
										Text = openingHoursArr[5], 
										TextColor = Color.White, 
										YAlign = TextAlignment.Center, 
										XAlign = TextAlignment.End,
										HorizontalOptions = LayoutOptions.FillAndExpand 
									},
									new DILabel{ 
										Text = openingHoursArr[6], 
										TextColor = Color.White, 
										YAlign = TextAlignment.Center, 
										XAlign = TextAlignment.End,
										HorizontalOptions = LayoutOptions.FillAndExpand 
									}
								}
							}
						}
					}
				);
			} else {
				InfoContentStack.Children.Add(
					new StackLayout{
						Orientation = StackOrientation.Horizontal,
						Padding = new Thickness(10, 0, 20, 10),
						Spacing = 5,
						Children = {
							new DILabel{
								TextColor = Color.White,
								HorizontalOptions = LayoutOptions.FillAndExpand,
								Text = "Opening Hours not available.",
							},
						}
					}
				);
			}

			InfoScrollView = new ScrollView{
				Content = InfoContentStack,
				HorizontalOptions = LayoutOptions.Fill,
				Orientation = ScrollOrientation.Vertical,
				IsClippedToBounds = true,
			};

			InfoLayout = new StackLayout {
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Start,
				Spacing = 0,
				Padding = 0,
				BackgroundColor = Color.FromRgb(52, 44, 27),
				Children = { InfoScrollView }
			};

			AbsoluteLayout.SetLayoutFlags(InfoLayout, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(InfoLayout, new Rectangle(0, 1, 1, 0));
			MainLayout.Children.Add(InfoLayout);

			AbsoluteLayout.SetLayoutFlags(MainLayout, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(MainLayout, new Rectangle(0, 0, 1, 1));
			UILayout.Children.Add(MainLayout);

			Content = UILayout;
		}

		public override void OnPageShowing()
		{
			base.OnPageShowing();

			// Set Current Total Price
			double dTotalPrice = App._DbManager.GetPaymentTotalPrice (RestaurantData.id);
			Globals.Config.PaymentInfo.Price = dTotalPrice.ToString("0.00");

			List<MenuCategory> sliderMenuCategories = new List<MenuCategory> ();
			foreach (MenuCategory categorItem in RestaurantData.menu_categories)
				sliderMenuCategories.Add (categorItem);
			MenuCategory _popularCategory = new MenuCategory ();
			List<SubCategory> _subCategoryList = new List<SubCategory> ();

			// Make Popular Category
			foreach (MenuCategory _category in RestaurantData.menu_categories) {
				List<MenuItem> popularMenus = new List<MenuItem> ();
				if (_category.is_menuitem_exist.Equals ("1")) {
					foreach (MenuItem item in _category.menuitems) {
						if (item.is_popular.Equals ("1")) {
							item.IsPopularCategory = true;
							item.Category_Id = _category.category_id;
							popularMenus.Add (item);
						}
					}
				}

				if (_category.is_subcat_exists.Equals("1")){
					foreach (SubCategory _subcategory in _category.subcategories) {
						if (_subcategory.menuitems != null) {
							foreach (MenuItem item in _subcategory.menuitems) {
								if (item.is_popular.Equals ("1")) {
									item.IsPopularCategory = true;
									item.Category_Id = _category.category_id;
									popularMenus.Add (item);
								}
							}
						}
					}
				}

				if (popularMenus.Count > 0) {
					SubCategory _subCategory = new SubCategory ();
					_subCategory.menuitems = popularMenus;
					_subCategory.sub_category_id = "";
					_subCategory.sub_category_name = _category.category_name;

					_subCategoryList.Add (_subCategory);
				}
			}

			if (_subCategoryList.Count > 0) {
				_popularCategory.category_name = "Popular";
				_popularCategory.category_id = "0";
				_popularCategory.is_menuitem_exist = "0";
				_popularCategory.is_subcat_exists = "1";
				_popularCategory.subcategories = _subCategoryList;

				isPopularExist = true;
				sliderMenuCategories.Insert (0, _popularCategory);
			}

			ItemSliderControl.ListItems = sliderMenuCategories;
			ItemSliderControl.RefreshFlag = !ItemSliderControl.RefreshFlag;
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			((MainPage)MasterPage).ChangeCurrentVenue (true);
			//App.RootPagePageShowingEvent += PageShowingEvent;
			ReAlignInfoLayout ();
		}

		private void ReAlignInfoLayout()
		{
			//Rectangle rcBounds = InfoImage.Bounds;
			//Rectangle rcInfoBounds = InfoLayout.Bounds;
			//rcInfoBounds.Top = rcBounds.Top + rcBounds.Height;
			//InfoLayout.LayoutTo (rcInfoBounds, 100, Easing.Linear);
			//InfoLayout.IsVisible = false;
		}

		async void MapVenueImage_Clicked()
		{
			var MapPage = new MapPage (RestaurantData.id, RestaurantData.lat, RestaurantData.lng, PageShowingEvent);
			MapPage.MasterPage = this.MasterPage;
			await Navigation.PushAsync (MapPage);
		}

		void SliderListView_Category_Clicked(String category_id, String category_name)
		{
		}

		double dlastTimeCalled = 0; //remove double calling
		double dEqualLimit = 100; // 100miliseconds
		async void SliderListView_Category_SubItem_Clicked(String category_id, String item_id, MenuItem item_data)
		{
			int curTime = DateTime.Now.Millisecond;
			if (Math.Abs (curTime - dlastTimeCalled) < dEqualLimit)
				return;

			dlastTimeCalled = curTime;

			await Navigation.PushAsync (new VenueCustomizePage (item_id, RestaurantData.CanPlaceOrder(), RestaurantData.VenueStatusText(), "", -1, false, PageShowingEvent));
		}

		async void SliderListView_Category_FavItem_Clicked(String category_id, String item_id, MenuItem item_data)
		{
			int curTime = DateTime.Now.Millisecond;
			if (Math.Abs (curTime - dlastTimeCalled) < dEqualLimit)
				return;

			dlastTimeCalled = curTime;
			// Set Favourite Item
			var response = await FavouriteService.SetFavouriteRestaurantItem (GetUserID(), GetUserToken(), Globals.Config.RestaurantId, item_id);
		}

		async Task SliderListView_Category_AddItem_Clicked(String category_id, String item_id, MenuItem item_data)
		{
			int curTime = DateTime.Now.Millisecond;
			if (Math.Abs (curTime - dlastTimeCalled) < dEqualLimit)
				return;

			List<MenuTypeItem> typeItems = new List<MenuTypeItem>();

			ShowLoading ();
			var response = await VenueService.GetRestaurantMenuItemDetail (GetUserID(), GetUserToken(), item_id);
			bool isResultSuccess = VenueService.HasSuccessResult (response.result);
			if (isResultSuccess) {
				try{
					var Itemdata = response.result.item_info;
					List<TypeItemData> TypeItemList = new List<TypeItemData>();
					if (Itemdata.types != null && Itemdata.types.Count > 0) {					
						foreach (JsonObject item_type in Itemdata.types) {
							String[] keyArr = item_type.Keys.ToArray<string> ();
							if (keyArr != null && keyArr.Length > 0) {
								for (int k = 0; k < keyArr.Length; k++) {
									TypeItemData itemData = new TypeItemData ();
									itemData.TypeName = keyArr [k];
									itemData.Items = new List<TypeItem> ();
									JsonArrayObjects jsonArr = item_type.Get<JsonArrayObjects> (keyArr [k]);
									if (jsonArr != null && jsonArr.Count > 0) {
										foreach (JsonObject item_type_detail in jsonArr) {
											String symbol = item_type_detail.Get ("currency_symbol");
											//String symbol_is_right = item_type_detail.Get ("currency_symbol_position_is_right");
											String id = item_type_detail.Get ("id");
											String name = item_type_detail.Get ("name");
											String price = item_type_detail.Get ("unit_price");
											TypeItem item = new TypeItem (id, symbol, name, price);
											itemData.Items.Add (item);
										}
									}
									TypeItemList.Add (itemData);
								}
							}
						}
					}

					if (TypeItemList.Count > 0) {
						foreach (TypeItemData itemData in TypeItemList) {
							if (itemData.Items != null && itemData.Items.Count > 0) {
								MenuTypeItem menuItem = new MenuTypeItem();
								menuItem.currency_symbol = itemData.Items[0].Currency;
								menuItem.currency_symbol_position_is_right = "";
								menuItem.id = itemData.Items[0].Id;
								menuItem.is_default = "";
								menuItem.name = itemData.Items[0].Name;
								menuItem.unit_price = itemData.Items[0].Price;
								typeItems.Add(menuItem);
							}
						}
					}
				}
				catch (Exception ex) {
				}
			}

			HideLoading ();

			dlastTimeCalled = curTime;

			if (typeItems != null && typeItems.Count > 0)
				Navigation.PushAsync (new VenueCustomizePage (item_id, RestaurantData.CanPlaceOrder(), RestaurantData.VenueStatusText(), "", -1, false, PageShowingEvent));
			else {
				App._DbManager.AddPaymentItem(Globals.Config.RestaurantId, Globals.Config.RestaurantName, item_id, item_data.item_name, item_data.item_price, new List<MenuExtraItem>(), typeItems, "", 1);

				double dItemPrice = 0;
				double.TryParse(item_data.item_price, out dItemPrice);

				double dTotalPrice = App._DbManager.GetPaymentTotalPrice (Globals.Config.RestaurantId);
				Globals.Config.PaymentInfo.Price = dTotalPrice.ToString("0.00");
			}
		}
		#endregion

		#region PRIVATE METHODS
		private StackLayout GetCustomVenueItem()
		{
			var ItemIconImage = new Image
			{
				WidthRequest = 80,
				VerticalOptions = LayoutOptions.FillAndExpand
			};
			ItemIconImage.SetBinding(DarkIceImage.SourceProperty, "Icon");

			var ItemTextLabel = new Label {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				TextColor = Color.White,
				Font = Font.SystemFontOfSize (NamedSize.Large, FontAttributes.Bold)
			};
			ItemTextLabel.SetBinding(DILabel.TextProperty, "Text");

			var ItemTextDetailLabel = new Label {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				TextColor = Color.White,
				Font = Font.SystemFontOfSize (NamedSize.Large, FontAttributes.Bold)
			};
			ItemTextDetailLabel.SetBinding(DILabel.TextProperty, "TextDetail");

			var StackItem = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Padding = new Thickness(20, 10, 20, 10),
				Spacing = 20,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 100,
				Children =
				{
					ItemIconImage,
					new StackLayout{
						Orientation = StackOrientation.Vertical,
						Padding = 0,
						Spacing = 0,
						Children = { ItemTextLabel, ItemTextDetailLabel }
					},
					new DarkIceImage {
						Source = ImageSource.FromFile("directionicon.png"),
						HorizontalOptions = LayoutOptions.End,
						VerticalOptions = LayoutOptions.FillAndExpand, WidthRequest = 20
					}
				}
				};

			return StackItem;
		}

		private StackLayout GetCategoryItem()
		{
			var ItemTextLabel = new Label {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				TextColor = Color.White,
				Font = Font.SystemFontOfSize (NamedSize.Medium)
			};
			ItemTextLabel.SetBinding(DILabel.TextProperty, "category_name");

			var StackItem = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Padding = new Thickness(20, 0, 20, 0),
				Spacing = 0,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 40,
				Children =
				{
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
		private void MenuButton_Clicked()
		{
			((MainPage)MasterPage).ShowMenuPage ();
		}

		private void InfoImage_Clicked()
		{
			if (isShowInfo == false)
				InfoImageBounds = InfoImage.Bounds;

			Rectangle rcBounds = InfoImageBounds;
			Rectangle rcInfoBounds = InfoLayout.Bounds;
			if (isShowInfo == false) {
				InfoLayout.IsVisible = true;
				rcBounds.Top = rcBounds.Bottom / 4;
				//rcInfoBounds.Top = rcBounds.Top + rcBounds.Height;
				rcInfoBounds.Top = rcBounds.Bottom;
				rcInfoBounds.Bottom = InfoImageBounds.Bottom;
			}
			else {
				rcBounds.Top = InfoImageBounds.Top;
				//rcInfoBounds.Top = rcBounds.Top + rcBounds.Height;
				rcInfoBounds.Top = rcInfoBounds.Bottom = InfoImageBounds.Bottom;
			}

			InfoTextLabel.LayoutTo (rcBounds, 100, Easing.Linear);
			InfoImage.LayoutTo (rcBounds, 100, Easing.Linear);
			InfoLayout.LayoutTo (rcInfoBounds, 100, Easing.Linear);
			isShowInfo = !isShowInfo;
		}

		async private void Favourite_Clicked()
		{
			ShowLoading ();

			// Set Favourite Item
			var response = await FavouriteService.SetFavouriteRestaurant (GetUserID(), GetUserToken(), RestaurantData.id);

			HideLoading ();

			bool isRegisterSuccess = FavouriteService.HasSuccessResult (response);
			if (isRegisterSuccess) {
				RestaurantData.is_favourite = RestaurantData.is_favourite.Equals ("0") ? "1" : "0";

				Device.BeginInvokeOnMainThread (() => {
					favouriteImage.Source = RestaurantData.is_favourite.Equals("1") ? "favorite_on.png" : "favorite_off.png";
				});
			}
			await DisplayAlert ("Result", response.message, "OK");
		}

		async private void AddToBucket_Clicked()
		{
			double dTotalPrice = App._DbManager.GetPaymentTotalPrice (RestaurantData.id);
			if (dTotalPrice == 0)
				return;

			if (Globals.Config.IsGuestMode == true) {
				bool isLogin = await DisplayAlert ("PocketButler", "You need to login to perform this action.", "Login", "Later");
				if (isLogin) {
					Globals.Config.IsSliderPageOut = true;
					await Navigation.PushAsync (new LoginPage ());
				}
			} else {
				var orderPage = new OrderPage (PageShowingEvent){
				};
				var NewMainPage = new MainPage (orderPage);
				orderPage.MasterPage = NewMainPage;

				//NewMainPage.Detail = orderPage;
				await Navigation.PushAsync(NewMainPage);

				/*orderPage.MasterPage = MasterPage;
				await Navigation.PushAsync(orderPage);*/
			}
		}
		#endregion
	}
}

