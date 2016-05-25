using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using PocketButler.Controls;
using ServiceStack.Text;

namespace PocketButler
{
    public class OrderDetailPage : BasePage
    {
		#region PRIVATE MEMBERS
		protected AbsoluteLayout UILayout { get; set; }
		protected AbsoluteLayout MainLayout { get; set; }

		protected OrderHistory OrderHistoryInfo;
		protected OrderDetails OrderDetailsInfo;
		CustomExpandableListView OrderHistoryDetailList;
		bool IsFromOrderListPage;
		#endregion

		public OrderDetailPage(OrderHistory history, Action RefreshEvent, bool isFromOrderListPage = true)
        {
			BackAppearingEvent = RefreshEvent;

			Title = "Order Details";
			IsFromOrderListPage = isFromOrderListPage;

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

			OrderHistoryInfo = history;

			if (IsFromOrderListPage == false) {
				IsCustomizeBackAction = true;
			}

			BuildUI();

        }

		protected override void OnAppearing(){
			base.OnAppearing ();

			GetOrderDetail (OrderHistoryInfo.order_id);
		}


		#region PRIVATE METHODS
		private void BuildUI()
		{
			HideNavigationBar ();

			MakeCustomNavigationBar(UILayout, null, null);

			var VenueIcon = new DarkIceImage
			{
				Source = OrderHistoryInfo.restaurant_image,
				Aspect = Aspect.AspectFit,
				HorizontalOptions = LayoutOptions.FillAndExpand
			};

			AbsoluteLayout.SetLayoutFlags(VenueIcon, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(VenueIcon, new Rectangle(0.5, 0.05, 0.5, 0.12));
			MainLayout.Children.Add(VenueIcon);

			var OrderStatusBtn = new CustomButton {
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 27,
				TextColor = Color.White,
				BorderRadius = 2,
				BorderWidth = 2,
				BorderColor = Color.Gray,
				Font = Font.SystemFontOfSize(10),
				BackgroundColor = Color.FromRgb(171, 146, 91),
			};

			if(this.OrderHistoryInfo.orderstatus.Equals(MyOrderPage.OrderStatusPending) || this.OrderHistoryInfo.orderstatus.Equals(MyOrderPage.OrderStatusStarted) 
				|| this.OrderHistoryInfo.orderstatus.Equals(MyOrderPage.OrderstatusPartialRefund)){
				OrderStatusBtn.Text = "Pending";
			} else if (this.OrderHistoryInfo.Equals(MyOrderPage.OrderStatusCompleted)){
				OrderStatusBtn.Text = "Ready";
			} else if (this.OrderHistoryInfo.Equals(MyOrderPage.OrderStatusCancelled)){
				OrderStatusBtn.Text = "Cancelled";
				OrderStatusBtn.BackgroundColor = Color.FromRgb(250, 63, 37);
			} else {
				OrderStatusBtn.IsVisible = false;
			}

			AbsoluteLayout.SetLayoutFlags(OrderStatusBtn, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(OrderStatusBtn, new Rectangle(0.95, 0.05, 0.19, 0.06));
			MainLayout.Children.Add(OrderStatusBtn);

			var RestNameLabel = new DILabel {
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Fill,
				YAlign = TextAlignment.Center,
				XAlign = TextAlignment.Center,
				IsDefaultLabel = true,
				Lines = 1,
				Text = OrderHistoryInfo.restaurant_name,
				TextColor = Color.White,
				Font = Font.SystemFontOfSize(NamedSize.Medium),
			};

			String orderId = "";
			try{
				int id = int.Parse(OrderHistoryInfo.order_id);
				orderId = String.Format("{0:0000}", id);
			}
			catch (Exception ex) {
				orderId = OrderHistoryInfo.order_id;
			}
			var OrderNumLabel = new DILabel {
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Fill,
				YAlign = TextAlignment.Center,
				XAlign = TextAlignment.Center,
				IsDefaultLabel = true,
				Lines = 1,
				Text = orderId,
				TextColor = Color.FromRgb(171, 146, 91),
				Font = Font.SystemFontOfSize(100, FontAttributes.Bold),
			};

			var OrderRefIdLabel = new DILabel {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Start,
				YAlign = TextAlignment.Center,
				IsDefaultLabel = true,
				Lines = 1,
				Text = OrderHistoryInfo.order_ref_id,
				TextColor = Color.Gray,
				Font = Font.SystemFontOfSize(NamedSize.Small),
			};

			var OrderDateLabel = new DILabel {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Start,
				YAlign = TextAlignment.Center,
				IsDefaultLabel = true,
				Lines = 1,
				Text = OrderHistoryInfo.orderdate,
				TextColor = Color.Gray,
				Font = Font.SystemFontOfSize(NamedSize.Small),
			};

			double ta;
			bool ret = Double.TryParse(OrderHistoryInfo.totalamount, out ta);
			String amount = OrderHistoryInfo.totalamount;

			if(ret){
				amount = String.Format("{0:0.00}", ta);
			}

			amount = "$" + amount;

			var AmountLabel = new DILabel {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.End,
				YAlign = TextAlignment.Center,
				XAlign = TextAlignment.End,
				IsDefaultLabel = true,
				Lines = 1,
				Text = amount,
				TextColor = Color.White,
				Font = Font.SystemFontOfSize(NamedSize.Small),
			};

			var DescLabel = new DILabel {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.End,
				YAlign = TextAlignment.Center,
				XAlign = TextAlignment.End,
				IsDefaultLabel = true,
				Lines = 1,
				Text = "Inclusive all charges",
				TextColor = Color.Gray,
				Font = Font.SystemFontOfSize(NamedSize.Small),
			};

			OrderHistoryDetailList = new CustomExpandableListView { 
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				ListData = GetOrderDetailItems(),
				DisableEdit = true,
				IsExpandDefault = true,
			};

			var StackItem = new StackLayout {
				Orientation = StackOrientation.Vertical,
				Padding = 0,
				Spacing = 10,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = {
					RestNameLabel,
					OrderNumLabel,
					new StackLayout{
						Orientation = StackOrientation.Horizontal,
						Padding = 0,
						Spacing = 0,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						VerticalOptions = LayoutOptions.Fill,
						Children = {
							new StackLayout{
								Orientation = StackOrientation.Vertical,
								Padding = new Thickness(10, 0, 0, 0),
								Spacing = 0, 
								HorizontalOptions = LayoutOptions.FillAndExpand,
								VerticalOptions = LayoutOptions.FillAndExpand,
								Children={
									OrderRefIdLabel,
									OrderDateLabel,
								},
							},
							new StackLayout{
								Orientation = StackOrientation.Vertical,
								Padding = new Thickness(0, 0, 10, 0),
								Spacing = 0,
								HorizontalOptions = LayoutOptions.End,
								VerticalOptions = LayoutOptions.FillAndExpand,
								WidthRequest = 120,
								Children = {
									AmountLabel,
									DescLabel,
								},
							},
						},
					},
					OrderHistoryDetailList,
				},
			};

			AbsoluteLayout.SetLayoutFlags(StackItem, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(StackItem, new Rectangle(0.5, 0.95, 1, 0.8));
			MainLayout.Children.Add(StackItem);

			AbsoluteLayout.SetLayoutFlags(MainLayout, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(MainLayout, new Rectangle(0, 0.7, 1, 0.9));
			UILayout.Children.Add(MainLayout);

			Content = UILayout;
		}

		async void GetOrderDetail(string order_id){

			ShowLoading ();

			var response = await PaymentService.GetOrderDetail (order_id, GetUserID (), GetUserToken ());

			HideLoading ();

			bool isResultSuccess = PaymentService.HasSuccessResult (response.result);
			if (isResultSuccess) {
				OrderDetailsInfo = response.result.order_details;
				List<ExpandableTableListItem> dataList = GetOrderDetailItems();
				OrderHistoryDetailList.ListData = dataList;
			} else {
				await DisplayAlert ("Failure", response.result.message, "OK");
			}
		}

		private List<ExpandableTableListItem> GetOrderDetailItems()
		{
			List<ExpandableTableListItem> listItems = new List<ExpandableTableListItem> ();

			if (OrderDetailsInfo == null || OrderDetailsInfo.order_items == null)
				return listItems;

			List<OrderItem> orderItemList = new List<OrderItem>(OrderDetailsInfo.order_items);

			while (orderItemList.Count > 0) {
				int curTypeCnt = 0;

				String curMenuItemId = orderItemList [0].item_id;
				String menuName = orderItemList[0].item_name;
				String menuPrice = orderItemList [0].item_price;

				double dItemPrice = 0.0;
				double.TryParse (menuPrice, out dItemPrice);

				double totalPrice = 0.0;

				List<PaymentTableItem> menuItems = new List<PaymentTableItem> ();

				// Remove same items from the list
				for (int i = orderItemList.Count - 1; i >= 0; i--) {
					if (orderItemList [i].item_id == curMenuItemId) {
						PaymentTableItem item = new PaymentTableItem ();
						item.Customization = orderItemList [i].item_additional_req;

						double itemPrice = 0.0;
						double.TryParse (orderItemList [i].item_price, out itemPrice);

						if (orderItemList [i].item_extras != null && orderItemList [i].item_extras.Count > 0) {
							foreach (ItemExtraInfo item_extra in orderItemList[i].item_extras) {
								double extra_price = 0.0;
								double.TryParse (item_extra.unit_price, out extra_price);
								itemPrice += extra_price;

								if (string.IsNullOrEmpty (item.Customization))
									item.Customization = item_extra.name;
								else
									item.Customization += "," + item_extra.name;
							}
						}

						if (orderItemList [i].item_type_info != null && orderItemList [i].item_type_info.Count > 0) {
							foreach (JsonObject item_type in orderItemList[i].item_type_info) {
								String[] keyArr = item_type.Keys.ToArray<string> ();
								if (keyArr != null && keyArr.Length > 0) {
									for (int k = 0; k < keyArr.Length; k++) {
										JsonArrayObjects jsonArr = item_type.Get<JsonArrayObjects> (keyArr [k]);
										if (jsonArr != null && jsonArr.Count > 0) {
											foreach (JsonObject item_type_detail in jsonArr) {
												double extra_price = 0.0;
												double.TryParse (item_type_detail.Get("unit_price"), out extra_price);
												if (string.IsNullOrEmpty (item.Customization))
													item.Customization = item_type_detail.Get ("name");
												else
													item.Customization += "," + item_type_detail.Get ("name");
												itemPrice += extra_price;
											}
										}
									}
								}
							}
						}

						item.ItemPrice = itemPrice.ToString("0.00");
						item.MenuId = orderItemList [i].item_id;
						item.MenuName = orderItemList [i].item_name;
						menuItems.Add (item);
						orderItemList.RemoveAt (i);
						totalPrice += itemPrice;
						curTypeCnt++;
					}
				}

				if (curTypeCnt > 0) {
					ExpandableTableListItem item = new ExpandableTableListItem {
						Count = curTypeCnt,
						ItemName = menuName,
						IsExpanded = false,
						Items = menuItems,
						Price = totalPrice.ToString("0.00"),
						MenuId = curMenuItemId,
					};

					listItems.Add (item);
				}
			}

			return listItems;
		}

		#endregion

		#region EVENTS
		void MenuButton_Clicked()
		{
			if (MasterPage != null)
				MasterPage.IsPresented = true;
		}
		#endregion
    }
}
