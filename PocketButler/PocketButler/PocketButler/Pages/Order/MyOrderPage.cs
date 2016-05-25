using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using PocketButler.Controls;
using DSoft.Messaging;
using ServiceStack.Text;

namespace PocketButler
{
    public class MyOrderPage : BasePage
    {

		public static String OrderStatusPending = "0";
		public static String OrderStatusStarted = "1";
		public static String OrderStatusCompleted = "2";
		public static String OrderStatusDelivered = "3";
		public static String OrderStatusCancelled = "4";
		public static String OrderstatusPartialRefund = "5";
		static long lastEventTickTime = 0;

		class OrderHistoryItemDetail
		{
			public String RestaurantName { get; set; }
			public String RestaurantId { get; set; }
			public String RefId{ get; set;}
			public String Id { get; set;}
			public String OrderDate { get; set;}
			public String TotalAmount { get; set; }
			public String OrderStatus { get; set; }

			public String OrderStatusText { get; set;}

			public Color OrderStatusColor{ get; set; }
			public Utils.ItemClickedDelegate Tapped;

			public double IconWidth { get; set;}

			public String Icon { get; set; }

			public bool IsVisible { get; set; }

			public bool IsStatusButtonVisible { get; set;}

			public OrderHistoryItemDetail(String restaurantname, String restaurantid, String ref_id, String id, String order_date, String total_amount, String order_status, Utils.ItemClickedDelegate tappedEvent){
				this.RestaurantName = restaurantname;
				this.RestaurantId = restaurantid;
				this.RefId = ref_id;
				this.Id = id;
				this.OrderDate = order_date;
				this.TotalAmount = total_amount;

				double ta;
				bool ret = Double.TryParse(this.TotalAmount, out ta);
				if(ret){
					this.TotalAmount = String.Format("{0:0.00}", ta);
				}

				this.TotalAmount = "$" + this.TotalAmount;

				this.OrderStatus = order_status;

				OrderStatusColor = Color.FromRgb(171, 146, 91);

				IsStatusButtonVisible = true;

				if(this.OrderStatus.Equals(OrderStatusPending) || this.OrderStatus.Equals(OrderStatusStarted) || this.OrderStatus.Equals(OrderstatusPartialRefund)){
					OrderStatusText = "Pending";
				} else if (this.OrderStatus.Equals(OrderStatusCompleted)){
					OrderStatusText = "Ready";
				} else if (this.OrderStatus.Equals(OrderStatusCancelled)){
					OrderStatusText = "Cancelled";
					OrderStatusColor = Color.FromRgb(250, 63, 37);
				} else {
					OrderStatusText = "";
					IsStatusButtonVisible = false;
				}
				this.Tapped = tappedEvent;
				IconWidth = 0;
				Icon = "red_minus.png";
				IsVisible = false;
			}
		}

		#region PRIVATE MEMBERS
		protected AbsoluteLayout UILayout { get; set; }
		protected AbsoluteLayout MainLayout { get; set; }

		bool IsEditing = false;
		DILabel EditLabel { get; set; } 
		ListView OrderHistoryListView;
		List<OrderHistoryItemDetail> OrderHistoryDetailList = new List<OrderHistoryItemDetail> ();
		List<OrderHistory> OrderHistoryItems;

		#endregion

		MessageBusEventHandler newEvHandler;

		public MyOrderPage(Action RefreshEvent)
        {
			BackAppearingEvent = RefreshEvent;

			Title = "My Orders";

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
				TextColor = Color.FromRgb (171, 146, 91),
				BackgroundColor = Color.Transparent,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				Font = Font.SystemFontOfSize (NamedSize.Medium),
			};
			MakeCustomNavigationBar(UILayout, MenuImage, EditLabel, true);

			MenuImage.Tapped += MenuButton_Clicked;
			EditLabel.Tapped += EditButton_Clicked;

			OrderHistoryListView = new ListView {
				HasUnevenRows = true,
			};
			OrderHistoryListView.ItemSelected += listView_ItemSelected;

			AbsoluteLayout.SetLayoutFlags (OrderHistoryListView, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(OrderHistoryListView, new Rectangle(0, 0.98, 1, 0.99));
			MainLayout.Children.Add (OrderHistoryListView);

			AbsoluteLayout.SetLayoutFlags(MainLayout, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(MainLayout, new Rectangle(0, 1, 1, 0.9));
			UILayout.Children.Add(MainLayout);

			Content = UILayout;
		}

		public void RefreshOrder(){
			RefreshOrderHistory ();
		}

		async void RefreshOrderHistory(){
			int totalOrderPageCount = 1;
			bool IsSucess = true;
			String failureMsg = "";
			OrderHistoryListView.ItemsSource = null;

			ShowLoading ();

			OrderHistoryDetailList.Clear ();
			OrderHistoryItems = new List<OrderHistory>();
			for (int i = 1; i <= totalOrderPageCount; i++) {
				var response = await PaymentService.GetOrderHistory (GetUserID (), GetUserToken (), i);
				bool isResultSuccess = PaymentService.HasSuccessResult (response.result);
				if (isResultSuccess) {
					totalOrderPageCount = response.result.total_pages;
					List<OrderHistory> resultList = response.result.order_list;
					foreach (OrderHistory item in resultList) {
						//if (item.order_id.Equals ("326")) {
						//	item.orderstatus = OrderStatusCancelled;
						//}

						OrderHistoryItemDetail ItemDetail = new OrderHistoryItemDetail (item.restaurant_name, item.restaurant_id, item.order_ref_id, item.order_id, item.orderdate, item.totalamount, item.orderstatus, null);

						if (ItemDetail.OrderStatus.Equals (OrderStatusCancelled) || ItemDetail.OrderStatus.Equals (OrderStatusDelivered)) {
							ItemDetail.Icon = "red_minus.png";
						} else {
							ItemDetail.Icon = "";
						}
						OrderHistoryDetailList.Add (ItemDetail);
						OrderHistoryItems.Add (item);
					}
				} else {
					IsSucess = false;
					failureMsg = response.result.message;
				}
			}

			//if (IsSucess == false)
			//	await DisplayAlert ("Failure", failureMsg, "OK");

			OrderHistoryListView.ItemsSource = OrderHistoryDetailList;

			OrderHistoryListView.ItemTemplate = new DataTemplate (() => {
				var viewCell = new ViewCell {
					View = GetOrderHistoryItem(),
				};

				viewCell.Height = 80;
				return viewCell;
			});

			HideLoading ();
		}

		private StackLayout GetOrderHistoryItem(){

			var ItemIconImage = new DarkIceImage
			{
				WidthRequest = (IsEditing == true) ? 25 : 0,
				VerticalOptions = LayoutOptions.FillAndExpand,
			};

			//ItemIconImage.SetBinding (Image.IsVisibleProperty, "IsVisible");
			ItemIconImage.SetBinding(DarkIceImage.SourceProperty, "Icon");
			//ItemIconImage.SetBinding (DarkIceImage.WidthRequestImageProperty, "IconWidth", BindingMode.TwoWay);
			//ItemIconImage.SetBinding (Image.WidthProperty, "IconWidth");
			ItemIconImage.SetBinding (DarkIceImage.DataItemIDProperty, "Id");
			ItemIconImage.SetBinding (DarkIceImage.DataItemTypeProperty, "OrderStatus");

			ItemIconImage.Tapped = () => {
				long curTickTime = DateTime.Now.Ticks / 1000 / 1000;
				if (Math.Abs(curTickTime - lastEventTickTime) < 3)
					return;
				lastEventTickTime = curTickTime;

				String order_id = ItemIconImage.ItemID;
				String order_status = ItemIconImage.ItemType;
				if (order_status.Equals (OrderStatusCancelled) || order_status.Equals (OrderStatusDelivered)) {
					Console.WriteLine("Remove Order History : Id = " + order_id);
					RemoveOrderHistory(order_id);
				}
			};

			var RestNameLabel = new DILabel {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				HeightRequest = 30,
				IsDefaultLabel = true,
				Lines = 1,
				TextColor = Color.White,
				Font = Font.SystemFontOfSize(NamedSize.Large, FontAttributes.Bold),
			};
			RestNameLabel.SetBinding (Label.TextProperty, "RestaurantName");

			var RefIdLabel = new DILabel {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				IsDefaultLabel = true,
				Lines = 1,
				HeightRequest = 20,
				TextColor = Color.Gray,
				Font = Font.SystemFontOfSize(NamedSize.Medium),
			};
			RefIdLabel.SetBinding (Label.TextProperty, "RefId");

			var OrderDateLabel = new DILabel {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				IsDefaultLabel = true,
				Lines = 1,
				TextColor = Color.Gray,
				Font = Font.SystemFontOfSize(NamedSize.Medium),
			};
			OrderDateLabel.SetBinding (Label.TextProperty, "OrderDate");

			var OrderAmountLabel = new DILabel {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
				XAlign = TextAlignment.Center,
				IsDefaultLabel = true,
				Lines = 1,
				HeightRequest = 20,
				TextColor = Color.Gray,
				Font = Font.SystemFontOfSize(NamedSize.Medium),
			};
			OrderAmountLabel.SetBinding (Label.TextProperty, "TotalAmount");

			var OrderStatusLayout = new AbsoluteLayout {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 27,
			};

			var OrderStatusBtn = new CustomButton {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 27,
				TextColor = Color.White,
				BorderRadius = 2,
				BorderWidth = 2,
				BorderColor = Color.Gray,
				Font = Font.SystemFontOfSize(10),
			};

			OrderStatusBtn.SetBinding (CustomButton.BackgroundColorProperty, "OrderStatusColor");
			OrderStatusBtn.SetBinding (CustomButton.DataItemIDProperty, "Id");
			//OrderStatusBtn.SetBinding (CustomButton.TextProperty, "OrderStatusText");
			OrderStatusBtn.SetBinding (CustomButton.IsVisibleProperty, "IsStatusButtonVisible");

			var OrderStatusText = new DILabel {
				IsDefaultLabel = true,
				TextColor = Color.White,
				Font = Font.SystemFontOfSize(10),
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
			};
			OrderStatusText.SetBinding (DILabel.TextProperty, "OrderStatusText");
			//OrderStatusText.SetBinding (DILabel.BackgroundColorProperty, "OrderStatusColor");
			OrderStatusText.SetBinding (DILabel.IsVisibleProperty, "IsStatusButtonVisible");

			OrderStatusLayout.Children.Add (OrderStatusBtn, new Rectangle (0, 0, 1, 1), AbsoluteLayoutFlags.All);
			OrderStatusLayout.Children.Add (OrderStatusText, new Rectangle (0, 0, 1, 1), AbsoluteLayoutFlags.All);

			var SameAgainLabel = new DILabel {
				Text = "Same Again", 
				TextColor = Color.FromRgb(171, 146, 91),
				Font = Font.SystemFontOfSize(10),
				XAlign = TextAlignment.Center,
			};
			SameAgainLabel.SetBinding (DILabel.DataItemIDProperty, "Id");

			SameAgainLabel.TappedWithId += async (string id) => {
				AddSameAgain(id);
			};

			var StackItem = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				Padding = new Thickness(20, 10, 20, 10),
				Spacing = 10,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 80,
				Children = {
					ItemIconImage,
					new StackLayout{
						Orientation = StackOrientation.Vertical,
						Padding = 0, 
						Spacing = 0, 
						HorizontalOptions = LayoutOptions.FillAndExpand,
						VerticalOptions = LayoutOptions.FillAndExpand,
						Children={
							RestNameLabel,
							RefIdLabel,
							OrderDateLabel,
						},
					},
					new StackLayout{
						Orientation = StackOrientation.Vertical,
						Padding = 0, 
						Spacing = 0,
						HorizontalOptions = LayoutOptions.End,
						VerticalOptions = LayoutOptions.FillAndExpand,
						WidthRequest = 75,
						Children = {
							OrderStatusLayout,
							OrderAmountLabel,
							SameAgainLabel
						},
					},
				},
			};

			return StackItem;
		}

		async void RemoveOrderHistory(string order_id){
			bool isRemove = await DisplayAlert ("PocketButler", "Do you want to remove this order?", "OK", "Cancel");
			if (!isRemove)
				return;

			ShowLoading ();

			var response = await PaymentService.RemoveUserOrder (GetUserID (), GetUserToken (), order_id);

			HideLoading ();

			bool isResultSuccess = PaymentService.HasSuccessResult (response.result);
			await DisplayAlert ("PocketButler", response.result.message, "OK"); 
			RefreshOrderHistory ();
		}

		#endregion

		#region OVERRIDE
		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			/*if (newEvHandler == null) {
				newEvHandler = new MessageBusEventHandler () {
					EventId = Globals.Config.EVENT_REFRESH_ORDER,
					EventAction = (sender, data) => {
						//Todo Refresh Order
						Console.WriteLine("Refresh Order");
						RefreshOrderHistory();
					},
				};
			}

			MessageBus.Default.Register (newEvHandler);*/

			//if (Globals.Config.IsPoptoMasterPage == false)
				RefreshOrderHistory ();

		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();

			//MessageBus.Default.Clear (Globals.Config.EVENT_REFRESH_ORDER);

		}

		#endregion

		#region EVENTS

		async void listView_ItemSelected(object sender, SelectedItemChangedEventArgs e){
			OrderHistoryItemDetail item = e.SelectedItem as OrderHistoryItemDetail;

			if (item != null) {
				if (item.Tapped != null)
					item.Tapped.Invoke ();

				OrderHistoryListView.SelectedItem = null;

				OrderHistory info = OrderHistoryItems.Find (s => s.order_id == item.Id);
				if (info != null) {
					var OrderDetailPage = new OrderDetailPage (info, PageShowingEvent);
					OrderDetailPage.MasterPage = MasterPage;
					await Navigation.PushAsync (OrderDetailPage);
				}
			}
		}

		void MenuButton_Clicked()
		{
			if (MasterPage != null)
				MasterPage.IsPresented = true;
		}

		void EditButton_Clicked()
		{
			if (EditLabel.Text == "Edit") {
				IsEditing = true;
				EditLabel.Text = "Done";
				OrderHistoryListView.ItemsSource = null;
				OrderHistoryListView.ItemsSource = OrderHistoryDetailList;
				/*foreach(OrderHistoryItemDetail item in OrderHistoryDetailList){
					if (item.OrderStatus.Equals (OrderStatusCancelled) || item.OrderStatus.Equals (OrderStatusDelivered)) {
						item.Icon = "red_minus.png";
					} else {
						item.Icon = "";
					}
					item.IconWidth = 50;
				}*/
			} else {
				EditLabel.Text = "Edit";
				IsEditing = false;
				OrderHistoryListView.ItemsSource = null;
				OrderHistoryListView.ItemsSource = OrderHistoryDetailList;
				/*foreach(OrderHistoryItemDetail item in OrderHistoryDetailList){
					item.IsVisible = false;
					item.IconWidth = 0;
				}*/
			}
		}

		private async void AddSameAgain(String id)
		{
			OrderHistory info = OrderHistoryItems.Find (s => s.order_id == id);
			if (info == null) 
				return;
			
			bool bResult = await DisplayAlert ("PocketButler", "Do you want to add same again", "Yes", "No");
			if (bResult == true) {
				ShowLoading ();

				var responsePaymentDetail = await PaymentService.GetOrderDetail (info.order_id, GetUserID (), GetUserToken ());

				bool isResultSuccessPayment = PaymentService.HasSuccessResult (responsePaymentDetail.result);
				if (isResultSuccessPayment) {
					var OrderDetailsInfo = responsePaymentDetail.result.order_details;
					foreach (OrderItem item in OrderDetailsInfo.order_items) {
						List<MenuExtraItem> extraItems = new List<MenuExtraItem>();

						if(item.item_extras != null && item.item_extras.Count > 0){
							foreach(ItemExtraInfo extraData in item.item_extras){
								MenuExtraItem extraItem = new MenuExtraItem(){
									id = extraData.id,
									name = extraData.name,
									unit_price = extraData.unit_price,
									currency_symbol = extraData.currency_symbol,
									currency_symbol_position_is_right = extraData.currency_symbol_position_is_right,
									is_default = extraData.is_default,
								};
								extraItems.Add(extraItem);
							}
						}

						List<MenuTypeItem> typeItems = new List<MenuTypeItem>();

						if(item.item_type_info != null && item.item_type_info.Count > 0){
							foreach (JsonObject item_type in item.item_type_info) {
								String[] keyArr = item_type.Keys.ToArray<string> ();
								if (keyArr != null && keyArr.Length > 0) {
									for (int k = 0; k < keyArr.Length; k++) {
										JsonArrayObjects jsonArr = item_type.Get<JsonArrayObjects> (keyArr [k]);
										if (jsonArr != null && jsonArr.Count > 0) {
											foreach (JsonObject item_type_detail in jsonArr) {
												String symbol = item_type_detail.Get ("currency_symbol");
												//String symbol_is_right = item_type_detail.Get ("currency_symbol_position_is_right");
												String type_id = item_type_detail.Get ("id");
												String name = item_type_detail.Get ("name");
												String price = item_type_detail.Get ("unit_price");

												MenuTypeItem typeitem = new MenuTypeItem{
													currency_symbol = symbol,
													currency_symbol_position_is_right = "",
													id = type_id,
													is_default = "",
													name = name,
													unit_price = price,
												};
												typeItems.Add (typeitem);
											}
										}
									}
								}
							}
						}

						App._DbManager.AddPaymentItem(info.restaurant_id, info.restaurant_name, item.item_id, item.item_name, item.item_price, extraItems, typeItems, item.item_additional_req, int.Parse(item.item_quantity));

					}
					double dTotalPrice = App._DbManager.GetPaymentTotalPrice (info.restaurant_id);
					Globals.Config.PaymentInfo.Price = dTotalPrice.ToString("0.00");
				} else {
					HideLoading ();
					await DisplayAlert ("Failure", responsePaymentDetail.result.message, "OK");
					return;
				}

				var response = await VenueService.GetRestaurantDetails (GetUserID(), info.restaurant_id, App.UserLatitude.ToString(), App.UserLongitude.ToString(), GetUserToken());
				bool isResultSuccess = VenueService.HasSuccessResult (response.result);
				if (isResultSuccess) {
					Globals.Config.CurrentVenue = response.result.restaurant_details;

					var RestaurantParam = response.result.restaurant_details;
					App.PageLoaderManager.ShowTypeSliderPage (RestaurantParam);

					// Add Order again
				} else {
					await DisplayAlert ("Failure", response.result.message, "OK");
				}

				HideLoading ();
			}
		}
		#endregion
    }
}
