using System;
using Xamarin.Forms;
using PocketButler.Controls;
using PocketButler.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ServiceStack.Text;
using System.Threading.Tasks;
using Xamarin;

namespace PocketButler
{
	public class VenueCustomizePage : BasePage
	{
		class RelatedItemData
		{
			public String Id;
			public String Icon { get; set; }
			public String Name { get; set; }
			public String Price { get; set; }
			public String RestaurantName { get; set; }
			public String Distance { get; set; }
			public RelatedItemData(String id, String icon, String name, String price, String restaurantname, String distance)
			{
				this.Id = id;
				this.Icon = icon;
				this.Name = name;
				this.Price = price;
				this.RestaurantName = restaurantname;
				this.Distance = Utils.GetDistanceString(distance);
			}
		}

		class ExtraItemData : INotifyPropertyChanged
		{
			public String Id;
			public String Name { get; set; }
			public String Price { get; set; }
			public ImageSource Icon { get; set; }
			public bool IsSelected { get; set; }

			public ExtraItemData(String id, String name, String price, bool isSelected)
			{
				this.Id = id;
				this.Name = name;
				this.Price = price;
				this.IsSelected = isSelected;
				ChangeIcon();
			}

			public void ChangeIcon()
			{
				Icon = (IsSelected) ? ImageSource.FromFile("welcome_remember_email_checked.png") : ImageSource.FromFile("welcome_remember_email_empty.png");
				OnPropertyChanged("Icon");
			}

			public event PropertyChangedEventHandler PropertyChanged;
			protected virtual void OnPropertyChanged(string propertyName)
			{
				PropertyChangedEventHandler handler = PropertyChanged;
				if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
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

		DILabel ItemCountLabel { get; set; }

		String Item_id;
		MenuItem Itemdata;
		DILabel BasketTotalPriceLabel;
		ListView RelatedListView;
		ListView ExtraListView;
		ListView TypeListView;

		String Customization;
		int Uid;
		bool IsFromCart;
		bool IsOnline;
		String OnlineString = "";
		bool IsExtraChanged = false;
		bool IsTypeChanged = false;

		int currentItemCount = 1;

		DarkIceImage FavouriteImage;

		List<RelatedItemData> RelatedItemList = new List<RelatedItemData>();
		List<ExtraItemData> ExtraItemList = new List<ExtraItemData>();
		List<TypeItemData> TypeItemList = new List<TypeItemData>();
		Dictionary<int, TypeItem> SelectedTypes = new Dictionary<int, TypeItem> ();

		List<CustomSlidePicker> TypePickerList = new List<CustomSlidePicker>();
		#endregion

		#region
		//When Editing
		List<PaymentExtraItem> CurrentExtraItem;
		List<PaymentTypeItem> CurrentTypeItem;
		#endregion

		public VenueCustomizePage (String item_id, bool isOnline, String offlineText, String customization, int uid, bool isFromCart, Action refresh)
		{
			BackAppearingEvent = refresh;
			this.Item_id = item_id;
			this.IsOnline = isOnline;
			OnlineString = offlineText;
			this.Customization = customization;
			this.IsFromCart = isFromCart;
			this.Uid = uid;
			CurrentExtraItem = new List<PaymentExtraItem> ();
			CurrentTypeItem = new List<PaymentTypeItem> ();
			IsExtraChanged = false;
			IsTypeChanged = false;
			SelectedTypes.Clear ();
			Title = "Customise";

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

			LoadCustomizeDetailData ();
		}

		//Edit
		public VenueCustomizePage (String item_id, bool isOnline, String offlineText, String customization, int uid, bool isFromCart, List<PaymentExtraItem> extras, List<PaymentTypeItem> types, Action refresh)
		{
			this.Item_id = item_id;
			this.IsOnline = isOnline;
			OnlineString = offlineText;
			this.Customization = customization;
			this.IsFromCart = isFromCart;
			this.Uid = uid;
			this.CurrentExtraItem = extras;
			this.CurrentTypeItem = types;
			BackAppearingEvent = refresh;
			SelectedTypes.Clear ();

			Title = "Customise";

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

			LoadCustomizeDetailData ();
		}

		#region PRIVATE METHODS
		async void LoadCustomizeDetailData()
		{
			ShowLoading ();

			var response = await VenueService.GetRestaurantMenuItemDetail (GetUserID(), GetUserToken(), Item_id);

			HideLoading ();

			bool isResultSuccess = VenueService.HasSuccessResult (response.result);
			if (isResultSuccess) {
				Itemdata = response.result.item_info;
				TypePickerList.Clear ();
				BuildUI();
			}
			else
				await DisplayAlert ("Failure", response.result.message, "OK");
		}

		private void BuildUI()
		{
			BasketTotalPriceLabel = new DILabel {
				Text = "",
				TextColor = Color.FromRgb (171, 146, 91),
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				YAlign = TextAlignment.Center
			};
			BasketTotalPriceLabel.BindingContext = Globals.Config.PaymentInfo;
			BasketTotalPriceLabel.SetBinding (Label.TextProperty, "Price");

			var BucketImage = new DarkIceImage { Source = "navbar_cart.png", Aspect = Aspect.AspectFit };
			BucketImage.Tapped += AddToBucket_Clicked;
			BasketTotalPriceLabel.Tapped += AddToBucket_Clicked;

			if (IsOnline) {
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

				// Register Custom Navigation Bar
				if (IsFromCart == false)
					MakeCustomNavigationBar (UILayout, null, BucketStack);
				else
					MakeCustomNavigationBar (UILayout, null, null);
			} else {
				if (String.IsNullOrEmpty (OnlineString)) {
					// Register Custom Navigation Bar
					MakeCustomNavigationBar (UILayout, null, null);
				} else {
					var StatusLabel = new DILabel
					{
						Text = OnlineString,
						TextColor = RestaurantInfo.GetStatusColorFromStr(OnlineString),
						YAlign = TextAlignment.Center,
						Font = Font.SystemFontOfSize(NamedSize.Medium)
					};

					// Register Custom Navigation Bar
					MakeCustomNavigationBar (UILayout, null, StatusLabel);
				}
			}

			FavouriteImage = new DarkIceImage {
				Source = Itemdata.is_favourite.Equals("1") ? "favorite_on.png" : "favorite_off.png",
				HorizontalOptions = LayoutOptions.EndAndExpand
			};

			FavouriteImage.Tapped += Favourite_Clicked;

			var ItemMainInfoLayout = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				Padding = new Thickness (10, 20, 10, 10),
				Children = {
					new DILabel {
						Text = Itemdata.item_name,
						TextColor = Color.White,
						YAlign = TextAlignment.Center,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						Font = Font.SystemFontOfSize (20),
						HeightRequest = 40,
						WidthRequest = 300,
						Lines = 1,
					},
					FavouriteImage
				}
			};

			AbsoluteLayout.SetLayoutFlags(ItemMainInfoLayout, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ItemMainInfoLayout, new Rectangle(0, 0.02, 1, 0.2));
			MainLayout.Children.Add(ItemMainInfoLayout);

			var VenuePrice = new DILabel {
				Text = Itemdata.item_price + " " + Itemdata.currency_symbol,
				TextColor = Color.Gray,
				Font = Font.SystemFontOfSize(NamedSize.Medium),
				YAlign = TextAlignment.Center,
			};

			AbsoluteLayout.SetLayoutFlags(VenuePrice, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(VenuePrice, new Rectangle(0.2, 0.15, 0.7, 0.1));
			MainLayout.Children.Add(VenuePrice);

			var CustomVenueImage = new DarkIceImage {
				Source = String.IsNullOrEmpty(Itemdata.item_image_large) ? Itemdata.item_image : Itemdata.item_image_large,
				Aspect = Aspect.AspectFit,
				HeightRequest = 150,
			};

			var VenueLabel = new DILabel {
				Text = Itemdata.item_description,
				TextColor = Color.Gray,
				Font = Font.SystemFontOfSize(NamedSize.Medium),
				YAlign = TextAlignment.Center,
				XAlign = TextAlignment.Center,
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};

			var VenueAdditionRequirements = new DILabel {
				Text = "Additional Requirements:\n(Special requests cannot be guaranteed)",
				TextColor = Color.White,
				Font = Font.SystemFontOfSize(NamedSize.Medium),
				YAlign = TextAlignment.Center,
			};

			var VenueAdditionEntry = new Editor {
				BackgroundColor = Color.Gray,
				//Font = Font.SystemFontOfSize(NamedSize.Medium),
				HeightRequest = 100,
				Text = Customization,
				//XAlign = TextAlignment.Start,
			};

			var SubstractImage = new DarkIceImage{
				Source = ImageSource.FromFile("subtract_pressed.png"),
			};
			SubstractImage.Tapped += Substract_Clicked;

			/*var QuantityImage = new DarkIceImage{
				Source = ImageSource.FromFile("quantity.png"),
			};*/

			ItemCountLabel = new DILabel {
				Text = currentItemCount.ToString(),
				TextColor = Color.Black,
				Font = Font.SystemFontOfSize(NamedSize.Medium),
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
				HasBackground = true,
			};

			var AddImage = new DarkIceImage{
				Source = ImageSource.FromFile("add_pressed.png"),
			};
			AddImage.Tapped += Add_Clicked;

			var AddOrderButton = new Button {
				Text = (IsFromCart) ? "Save Changes" : "Add to Order",
				TextColor = Color.Black,
				BackgroundColor = Color.FromRgb(171, 146, 91),
			};

			AddOrderButton.Clicked += async (object sender, EventArgs e) => {
				if (IsFromCart == true)
				{
					App._DbManager.UpdateCustomization(Uid, VenueAdditionEntry.Text);

					if(IsExtraChanged){
						List<MenuExtraItem> extraItems = new List<MenuExtraItem>();

						if(ExtraItemList != null && ExtraItemList.Count > 0){
							foreach(ExtraItemData extraData in ExtraItemList){
								if(extraData.IsSelected){
									MenuExtraItem extraItem = Itemdata.extras.Find(o => o.id == extraData.Id);
									extraItems.Add(extraItem);
								}
							}
						}
						App._DbManager.UpdateExtra(Uid, extraItems);
					}

					if(IsTypeChanged){
						List<MenuTypeItem> typeItems = new List<MenuTypeItem>();

						if(SelectedTypes != null && SelectedTypes.Count > 0){
							foreach(TypeItem type in SelectedTypes.Values){
								MenuTypeItem menuItem = new MenuTypeItem();
								menuItem.currency_symbol = type.Currency;
								menuItem.currency_symbol_position_is_right = "";
								menuItem.id = type.Id;
								menuItem.is_default = "";
								menuItem.name = type.Name;
								menuItem.unit_price = type.Price;
								typeItems.Add(menuItem);
							}
						}
						App._DbManager.UpdateTypes(Uid, typeItems);
					}

					//App._DbManager.UpdatePrice(Uid, 0);
					Globals.Config.CustomizeEventHandle.IsCustomizeUpdated = !Globals.Config.CustomizeEventHandle.IsCustomizeUpdated;

					double dTotalPrice = App._DbManager.GetPaymentTotalPrice (Globals.Config.RestaurantId);
					Globals.Config.PaymentInfo.Price = dTotalPrice.ToString("0.00");

					if(this.BackAppearingEvent != null)
						this.BackAppearingEvent.Invoke();
					await Navigation.PopAsync();
				}
				else
				{
					List<MenuExtraItem> extraItems = new List<MenuExtraItem>();

					if(ExtraItemList != null && ExtraItemList.Count > 0){
						foreach(ExtraItemData extraData in ExtraItemList){
							if(extraData.IsSelected){
								MenuExtraItem extraItem = Itemdata.extras.Find(o => o.id == extraData.Id);
								extraItems.Add(extraItem);
							}
						}
					}

					List<MenuTypeItem> typeItems = new List<MenuTypeItem>();

					if(SelectedTypes != null && SelectedTypes.Count > 0){
						foreach(TypeItem type in SelectedTypes.Values){
							MenuTypeItem menuItem = new MenuTypeItem();
							menuItem.currency_symbol = type.Currency;
							menuItem.currency_symbol_position_is_right = "";
							menuItem.id = type.Id;
							menuItem.is_default = "";
							menuItem.name = type.Name;
							menuItem.unit_price = type.Price;
							typeItems.Add(menuItem);
						}
					}

					App._DbManager.AddPaymentItem(Globals.Config.RestaurantId, Globals.Config.RestaurantName, Itemdata.item_id, Itemdata.item_name, Itemdata.item_price, extraItems, typeItems, VenueAdditionEntry.Text, currentItemCount);

					double dItemPrice = 0;
					double.TryParse(Itemdata.item_price, out dItemPrice);

					double dTotalPrice = App._DbManager.GetPaymentTotalPrice (Globals.Config.RestaurantId);
					Globals.Config.PaymentInfo.Price = dTotalPrice.ToString("0.00");
					if (BackAppearingEvent != null)
						BackAppearingEvent.Invoke ();
					await Navigation.PopAsync();
				}
			};

			var CustomVenueInfoStack = new StackLayout {
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Spacing = 10,
				Padding = new Thickness(0, 20, 0, 20),
			};

			CustomVenueInfoStack.Children.Add (CustomVenueImage);
			CustomVenueInfoStack.Children.Add (VenueLabel);

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

									bool isDefault = item_type_detail.Get<bool> ("is_default");
									if (IsOnline == true || (isDefault != null && isDefault == true))
										itemData.Items.Add (item);
								}
							}
							TypeItemList.Add (itemData);
						}
					}
				}

				if (TypeItemList != null && TypeItemList.Count > 0) {
					var stack = GetTypeItemView ();

					var typeStack = new StackLayout {
						Orientation = StackOrientation.Vertical,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						Spacing = 10,
						Padding = new Thickness(0, 20, 0, 0),
						Children = { 
							new DILabel{
								IsDefaultLabel = true,
								Text = "   Customisations",
								BackgroundColor = Color.Gray,
								TextColor = Color.White,
								HeightRequest = 40,
								YAlign = TextAlignment.Center,
								Font = Font.SystemFontOfSize(NamedSize.Medium),
							},
							stack
						}
					};

					for (int j = 0; j < TypePickerList.Count; j++)
					{
						TypePickerList [j].SelectionChanged += () => {
							for (int i = 0; i < TypePickerList.Count; i++)
							{
								var TypePicker = TypePickerList[i];
								int idx = TypePicker.SelectedIndex;

								if (idx >= 0 && idx < TypePicker.PickerList.Count)
								{
									try{
										if(idx == 0){
											TypePicker.SelectedIndex = -1;
											SelectedTypes.Remove(TypePicker.Idx);
										} else {
											TypeItem item = TypeItemList[TypePicker.Idx].Items[idx - 1];
											TypeItemList[TypePicker.Idx].SelectedPrice = item.Price;

											if(item != null){
												SelectedTypes.Remove(TypePicker.Idx);
												SelectedTypes.Add(TypePicker.Idx, item);
											}
										}
										IsTypeChanged = true;

									}catch(Exception ex){
										Insights.Report (ex);
									}
								}
							}
						};
					}

					CustomVenueInfoStack.Children.Add (typeStack);
				}
			}

			if (Itemdata.extras != null && Itemdata.extras.Count > 0) {
				foreach (MenuExtraItem item in Itemdata.extras) {
					PaymentExtraItem extraItem = CurrentExtraItem.FirstOrDefault (o => o.ExtraId == item.id);
					ExtraItemList.Add (new ExtraItemData (item.id, item.name, item.unit_price, extraItem == null ? false : true));
				}
				ExtraListView = new ListView {
					HasUnevenRows = true,

					// Source of data items.
					ItemsSource = ExtraItemList,
					ItemTemplate = new DataTemplate (() => {
						// Return an assembled ViewCell.
						var viewCell = new ViewCell {
							View = GetExtraItemView ()
						};

						viewCell.Height = 40;
						return viewCell;
					}),
				};

				ExtraListView.ItemSelected += (object sender, SelectedItemChangedEventArgs e) => {
					if (e.SelectedItem == null)
						return;

					IsExtraChanged = true;
					var selectedItemData = e.SelectedItem as ExtraItemData;
					ExtraItemData itemInfo = ExtraItemList.Find(st => st.Id == selectedItemData.Id);
					itemInfo.IsSelected = !itemInfo.IsSelected;
					itemInfo.ChangeIcon();
					ExtraListView.SelectedItem = null;
					ExtraListView.ItemsSource = ExtraItemList;
				};

				var extraListViewStack = new StackLayout {
					Orientation = StackOrientation.Vertical,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					Spacing = 0,
					Padding = 0,
				};

				extraListViewStack.Children.Add (ExtraListView);
				extraListViewStack.HeightRequest = 60 * Itemdata.extras.Count;

				var extraStack = new StackLayout {
					Orientation = StackOrientation.Vertical,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					Spacing = 10,
					Padding = new Thickness(0, 20, 0, 0),
					Children = { 
						new DILabel{
							IsDefaultLabel = true,
							Text = "   Extras",
							BackgroundColor = Color.Gray,
							TextColor = Color.White,
							HeightRequest = 40,
							YAlign = TextAlignment.Center,
							Font = Font.SystemFontOfSize(NamedSize.Medium),
						},
						extraListViewStack
					}
				};

				CustomVenueInfoStack.Children.Add (extraStack);
			}

			CustomVenueInfoStack.Children.Add (new StackLayout {
				Orientation = StackOrientation.Vertical,
				Padding = new Thickness(20, 20, 20, 5),
				Spacing = 5,
				Children = {
					VenueAdditionRequirements,
					VenueAdditionEntry
				}
			});

			if (IsOnline == true && IsFromCart == false && Itemdata.related_items != null && Itemdata.related_items.Count > 0) {
				foreach (MenuItem item in Itemdata.related_items) {
					RelatedItemList.Add (new RelatedItemData (item.item_id, item.item_image, item.item_name, item.item_price + " " + item.currency_symbol, "", ""));
				}
				RelatedListView = new ListView {
					HasUnevenRows = true,

					// Source of data items.
					ItemsSource = RelatedItemList,
					ItemTemplate = new DataTemplate (() => {
						// Return an assembled ViewCell.
						var viewCell = new ViewCell {
							View = GetRelatedItemView ()
						};

						viewCell.Height = 60;
						return viewCell;
					}),
				};

				RelatedListView.ItemSelected += async (object sender, SelectedItemChangedEventArgs e) => {
					if (e.SelectedItem == null)
						return;

					var selectedItemData = e.SelectedItem as RelatedItemData;
					MenuItem itemInfo = Itemdata.related_items.Find(st => st.item_id == selectedItemData.Id);
					RelatedListView.SelectedItem = null;
					if (itemInfo != null)
					{
						var venueCustomizePage = new VenueCustomizePage(itemInfo.item_id, IsOnline, OnlineString, "", -1, false, PageShowingEvent);
						venueCustomizePage.MasterPage = MasterPage;
						await Navigation.PushAsync(venueCustomizePage);
					}
				};

				var relatedListViewStack = new StackLayout {
					Orientation = StackOrientation.Vertical,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					Spacing = 0,
					Padding = 0,
				};

				relatedListViewStack.Children.Add (RelatedListView);
				relatedListViewStack.HeightRequest = 60 * Itemdata.related_items.Count;

				var relatedStack = new StackLayout {
					Orientation = StackOrientation.Vertical,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					Spacing = 10,
					Padding = new Thickness(0, 20, 0, 20),
					Children = { 
						new DILabel{
							IsDefaultLabel = true,
							Text = "   Other related items:",
							TextColor = Color.White,
							Font = Font.SystemFontOfSize(NamedSize.Medium),
						},
						relatedListViewStack
					}
				};

				CustomVenueInfoStack.Children.Add (relatedStack);
			}

			if (IsFromCart == false && IsOnline == true) {
				CustomVenueInfoStack.Children.Add (new StackLayout{
					Orientation = StackOrientation.Horizontal,
					HorizontalOptions = LayoutOptions.End,
					Padding = 10,
					Spacing = 5,
					Children = {
						SubstractImage,
						ItemCountLabel,
						AddImage
					}
				});
			}

			if (IsFromCart == true || IsOnline == true)
			{
				CustomVenueInfoStack.Children.Add (new StackLayout {
					Orientation = StackOrientation.Vertical,
					Padding = new Thickness(20, 0, 20, 0),
					Spacing = 5,
					Children = {
						AddOrderButton
					}
				});
			}

			var CustomVenueInfoScrollView = new ScrollView {
				Content = CustomVenueInfoStack,
				HorizontalOptions = LayoutOptions.Fill,
				Orientation = ScrollOrientation.Vertical,
				IsClippedToBounds = true,
			};

			AbsoluteLayout.SetLayoutFlags(CustomVenueInfoScrollView, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(CustomVenueInfoScrollView, new Rectangle(0, 0.98, 1, 0.8));
			MainLayout.Children.Add(CustomVenueInfoScrollView);

			AbsoluteLayout.SetLayoutFlags(MainLayout, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(MainLayout, new Rectangle(0, 0, 1, 1));
			UILayout.Children.Add(MainLayout);

			Content = UILayout;
		}
		#endregion

		#region EVENTS
		private void Substract_Clicked()
		{
			if (currentItemCount > 1)
				currentItemCount--;

			ItemCountLabel.Text = currentItemCount.ToString ();
		}

		private void Add_Clicked()
		{
			currentItemCount++;

			ItemCountLabel.Text = currentItemCount.ToString ();
		}

		async private void Favourite_Clicked()
		{
			ShowLoading ();

			// Set Favourite Item
			var response = await FavouriteService.SetFavouriteRestaurantItem (GetUserID(), GetUserToken(), Globals.Config.RestaurantId, Itemdata.item_id);

			HideLoading ();

			bool isRegisterSuccess = FavouriteService.HasSuccessResult (response);
			if (isRegisterSuccess) {
				Itemdata.is_favourite = Itemdata.is_favourite.Equals ("0") ? "1" : "0";

				Device.BeginInvokeOnMainThread (() => {
					FavouriteImage.Source = Itemdata.is_favourite.Equals("1") ? "favorite_on.png" : "favorite_off.png";
				});
			}
			await DisplayAlert ("Result", response.message, "OK");
		}

		private async void AddToBucket_Clicked()
		{
			try{
				double dTotalPrice = App._DbManager.GetPaymentTotalPrice (Globals.Config.RestaurantId);
				if (dTotalPrice == 0)
					return;

				if (Globals.Config.IsGuestMode == true) {
					bool isLogin = await DisplayAlert ("PocketButler", "You need to login to perform this action.", "Login", "Later");
					if (isLogin)
						await Navigation.PushAsync (new LoginPage ());
				} else {
					var orderPage = new OrderPage (PageShowingEvent){
						MasterPage = MasterPage,
					};

					if (MasterPage != null)
						MasterPage.Detail = orderPage;
					else{
						var NewMainPage = new MainPage (orderPage);
						orderPage.MasterPage = NewMainPage;

						//NewMainPage.Detail = orderPage;
						await Navigation.PushAsync(NewMainPage);
					}
				}
			}
			catch (Exception ex) {
			}
		}

		private StackLayout GetExtraItemView()
		{
			var ItemIconImage = new Image
			{
				WidthRequest = 20,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.End,
			};
			ItemIconImage.SetBinding(DarkIceImage.SourceProperty, "Icon");

			var ItemTextLabel = new DILabel {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				TextColor = Color.White,
				HeightRequest = 40,
				Font = Font.SystemFontOfSize (NamedSize.Medium, FontAttributes.Bold),
				IsDefaultLabel = true,
				Lines = 2,
			};
			ItemTextLabel.SetBinding(DILabel.TextProperty, "Name");

			var ItemPriceLabel = new DILabel {
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				XAlign = TextAlignment.End,
				TextColor = Color.Gray,
				HeightRequest = 40,
				WidthRequest = 100,
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				IsDefaultLabel = true,
				Lines = 1,
			};
			ItemPriceLabel.SetBinding(DILabel.TextProperty, "Price");

			var StackItem = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Padding = new Thickness(20, 5, 20, 5),
				Spacing = 20,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 40,
				Children =
				{
					ItemTextLabel,
					ItemPriceLabel,
					ItemIconImage,
				}
			};

			return StackItem;
		}

		private StackLayout GetRelatedItemView()
		{
			var ItemIconImage = new Image
			{
				WidthRequest = 50,
				VerticalOptions = LayoutOptions.FillAndExpand
			};
			ItemIconImage.SetBinding(DarkIceImage.SourceProperty, "Icon");

			var ItemTextLabel = new DILabel {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				TextColor = Color.White,
				HeightRequest = 25,
				Font = Font.SystemFontOfSize (NamedSize.Medium, FontAttributes.Bold),
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
				HeightRequest = 20,
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
				HeightRequest = 60,
				Children =
				{
					ItemIconImage, 
					StackMainDataItem,
					ItemDistanceLabel
				}
				};

			return StackItem;
		}

		private StackLayout GetTypeItemView()
		{
			var StackItem = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Padding = new Thickness(20, 5, 20, 5),
				Spacing = 5,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
			};

			int index = 0;

			foreach (TypeItemData menuType in TypeItemList) {

				var ItemTextLabel = new DILabel {
					HorizontalOptions = LayoutOptions.Start,
					VerticalOptions = LayoutOptions.FillAndExpand,
					YAlign = TextAlignment.Center,
					TextColor = Color.White,
					HeightRequest = 40,
					Font = Font.SystemFontOfSize(NamedSize.Medium, FontAttributes.Bold),
					IsDefaultLabel = true,
					Lines = 1,
					Text = menuType.TypeName,
				};

				List<TypeItem> menuTypeItems = menuType.Items;
				int curIndex = -1;
				menuType.SelectedPrice = "";

				string[] options = new string[menuTypeItems.Count + 1];
				options [0] = "No Type";
				for (int i = 0; i < menuTypeItems.Count; i++) {
					options [i + 1] = menuTypeItems [i].Name + " - " + menuTypeItems [i].Price+ menuTypeItems [i].Currency;
					if (this.CurrentTypeItem != null && this.CurrentTypeItem.Count > 0) {
						PaymentTypeItem typeItem = CurrentTypeItem.FirstOrDefault (o => o.TypeID == menuTypeItems[i].Id);
						if (typeItem != null) {
							curIndex = i + 1;
						}
						menuType.SelectedPrice = typeItem == null ? "" : typeItem.Price;
					}
				}

				var TypePicker = new CustomSlidePicker
				{
					VerticalOptions = LayoutOptions.FillAndExpand,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					BackgroundColor = Color.Transparent,
					Idx = index,
					SelectedIndex = (curIndex <= 0 && IsFromCart == false) ? 1 : curIndex,
				};

				TypePickerList.Add (TypePicker);

				List<String> itemArrList = new List<string> ();
				foreach (string option in options) {
					itemArrList.Add (option);
				}

				TypePicker.PickerList = itemArrList;
				//TypePicker.SelectedIndex = curIndex;

				var ItemPriceLabel = new DILabel {

					HorizontalOptions = LayoutOptions.End,
					VerticalOptions = LayoutOptions.FillAndExpand,
					YAlign = TextAlignment.Center,
					XAlign = TextAlignment.End,
					TextColor = Color.Gray,
					HeightRequest = 40,
					Font = Font.SystemFontOfSize (NamedSize.Medium),
					IsDefaultLabel = true,
					Lines = 1,
					Text = "0.00$",
				};

				ItemPriceLabel.SetBinding (DILabel.TextProperty, "SelectedPrice");

				var StackSubItem = new StackLayout {
					Orientation = StackOrientation.Horizontal,
					Padding = 0,
					Spacing = 20,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					VerticalOptions = LayoutOptions.FillAndExpand,
					HeightRequest = 40,
					Children = {
						ItemTextLabel, 
						new StackLayout {
							Orientation = StackOrientation.Vertical,
							Padding = 0,
							Spacing = 0,
							HorizontalOptions = LayoutOptions.FillAndExpand,
							VerticalOptions = LayoutOptions.FillAndExpand,
							Children = {
								TypePicker,
								new StackLayout {
									Orientation = StackOrientation.Vertical,
									Padding = 0,
									Spacing = 0,
									HorizontalOptions = LayoutOptions.FillAndExpand,
									VerticalOptions = LayoutOptions.End,
									HeightRequest = 1,
									BackgroundColor = Color.Gray,
								}
							}
						} 
						//ItemPriceLabel,
					},
				};

				StackItem.Children.Add (StackSubItem);
				index++;
			}

			return StackItem;
		}
		#endregion
	}
}

