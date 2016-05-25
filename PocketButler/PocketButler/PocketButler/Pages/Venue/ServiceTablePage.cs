using System;
using Xamarin.Forms;
using PocketButler.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin;
using PocketButler.Services;

namespace PocketButler
{
	public class ServiceTablePage : BasePage
	{
		#region PRIVATE MEMBERS
		protected AbsoluteLayout UILayout { get; set; }
		protected AbsoluteLayout MainLayout { get; set; }

		//Picker ServicePointPicker;
		//Picker TableNumberPicker;
		CustomSlidePicker ServicePointPicker;
		CustomSlidePicker TableNumberPicker;
		DILabel ChargeLabel;
		String DeliveryCharge = "";

		ServiceDetailResultItem ServiceDetailData;
		float tabAmount = 0;

		List<Servicepoint> ServicePoints = new List<Servicepoint> ();
		PocketButler.Utils.DeliveryType PaymentDeliveryType = PocketButler.Utils.DeliveryType.DeliveryTypeNone;
		PocketButler.Utils.PaymentMethod PaymentMethodType = PocketButler.Utils.PaymentMethod.PaymentMethodUnknown;

		bool IsSuccessPassCode = false;
		CreatePassCodePage PassCodePage = null;
		#endregion

		public ServiceTablePage (Action RefreshEvent)
		{
			BackAppearingEvent = RefreshEvent;
			PageShowingEvent += OnPageShowing;
			Title = "";
			PaymentDeliveryType = Globals.Config.PaymentType;

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

			GetDataFromService ();
		}

		public override void OnPageShowing()
		{
			base.OnPageShowing ();

			if (PassCodePage != null)
				IsSuccessPassCode = PassCodePage.IsSuccess;

			if (IsSuccessPassCode) {
				PassCodePage = null;
				IsSuccessPassCode = false;

				Utils.MixPanel_Track("InputPin", "{VenueName: " + Globals.Config.RestaurantName + "}");

				bool isRemember = (Utils.LoadDataFromSettings ("save_payment_token") == "1");
				String stripe_token = Utils.LoadDataFromSettings ("stripe_token");

				if (Globals.Config.PaymentPageType == 0) { // loadOpenTabViewController
					Navigation.PushAsync (new OpenNewTabPage (PageShowingEvent));
				} else if (Globals.Config.PaymentPageType == 1) { // loadPayByTabViewController
					Navigation.PushAsync (new PaymentByTabPage (PageShowingEvent));
				} else if (Globals.Config.PaymentPageType == 2) { // loadPayByTokenViewController
					Navigation.PushAsync (new PaymentByTokenPage (PageShowingEvent));
				} else { // loadCreditCardViewController
					Navigation.PushAsync (new CheckoutOrderPage (PageShowingEvent));
				}
			} else {
				if (PassCodePage != null) {
					int failedCount = 0;
					String FailedCountString = Utils.LoadDataFromSettings ("FailedCount");
					int.TryParse(FailedCountString, out failedCount);
					if (failedCount > 4) {
						InputDialogHelper.InputDialogResultEvent = InputDialogResultEventForChangePin;
						InputDialogHelper.ShowInputDialog (Forms.Context, "You have reached your maximum number of attempts. Please enter your PocketButler password to change pin.");
						return;
					}
				}
			}
		}

		#region PRIVATE METHODS
		private void BuildUI()
		{
			var ItemImage = new DarkIceImage
			{
				Source = Globals.Config.RestaurantImage,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Aspect = Aspect.AspectFit,
			};

			// Register Custom Navigation Bar
			MakeCustomNavigationBar (UILayout, null, null, ItemImage);

			var TestBarLabel = new DILabel
			{
				Text = Globals.Config.RestaurantName,
				TextColor = Color.White,
				BackgroundColor = Color.Black,
				YAlign = TextAlignment.Center,
				XAlign = TextAlignment.Center,
			};
			AbsoluteLayout.SetLayoutFlags(TestBarLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(TestBarLabel, new Rectangle(0, 0, 1, 0.1));
			MainLayout.Children.Add(TestBarLabel);

			var PickUpLabel = new DILabel
			{
				Text = (PaymentDeliveryType == Utils.DeliveryType.DeliveryTypeTableService) ? "Table Service " : "Pick-up",
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				YAlign = TextAlignment.Center,
				XAlign = TextAlignment.Center,
			};
			AbsoluteLayout.SetLayoutFlags(PickUpLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(PickUpLabel, new Rectangle(0, 0.1, 1, 0.1));
			MainLayout.Children.Add(PickUpLabel);

			float fDeliveryChargeSpace = 0;
			if (PaymentDeliveryType == Utils.DeliveryType.DeliveryTypeTableService) {
				var DeliveryChargeLabel = new DILabel
				{
					Text = "Delivery charges:",
					TextColor = Color.Gray,
					BackgroundColor = Color.Transparent,
					YAlign = TextAlignment.End,
				};
				AbsoluteLayout.SetLayoutFlags(DeliveryChargeLabel, AbsoluteLayoutFlags.All);
				AbsoluteLayout.SetLayoutBounds(DeliveryChargeLabel, new Rectangle(0.5, 0.2, 0.9, 0.08));
				MainLayout.Children.Add(DeliveryChargeLabel);

				ChargeLabel = new DILabel
				{
					Text = "Free",
					TextColor = Color.White,
					BackgroundColor = Color.Transparent,
					YAlign = TextAlignment.End,
					XAlign = TextAlignment.End,
				};
				AbsoluteLayout.SetLayoutFlags(ChargeLabel, AbsoluteLayoutFlags.All);
				AbsoluteLayout.SetLayoutBounds(ChargeLabel, new Rectangle(0.8, 0.2, 0.5, 0.08));
				MainLayout.Children.Add(ChargeLabel);

				fDeliveryChargeSpace = 0.08f;
			}

			var SelectLabel = new DILabel
			{
				Text = (PaymentDeliveryType == Utils.DeliveryType.DeliveryTypeTableService) ? "Indicate your location:" : "Select your nearest service point:",
				TextColor = Color.Gray,
				BackgroundColor = Color.Transparent,
				YAlign = TextAlignment.End,
			};
			AbsoluteLayout.SetLayoutFlags(SelectLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(SelectLabel, new Rectangle(0.5, 0.2 + fDeliveryChargeSpace, 0.9, 0.1));
			MainLayout.Children.Add(SelectLabel);

			ServicePointPicker = new CustomSlidePicker
			{
				WidthRequest = 50,
				BackgroundColor = Color.Transparent
			};

			var ServicePointLabel = new DILabel
			{
				Text = "Service point",
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				YAlign = TextAlignment.Center,
			};

			var ServicePickerUnderlineLayout = new StackLayout {
				Spacing = 0,
				Padding = 0,
				BackgroundColor = Color.Gray,
				HeightRequest = 2,
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};

			AbsoluteLayout.SetLayoutFlags(ServicePointLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ServicePointLabel, new Rectangle(0.2, 0.32 + fDeliveryChargeSpace, 0.5, 0.1));
			MainLayout.Children.Add(ServicePointLabel);

			AbsoluteLayout.SetLayoutFlags(ServicePointPicker, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ServicePointPicker, new Rectangle(0.8, 0.32 + fDeliveryChargeSpace, 0.5, 0.1));
			MainLayout.Children.Add(ServicePointPicker);

			AbsoluteLayout.SetLayoutFlags(ServicePickerUnderlineLayout, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ServicePickerUnderlineLayout, new Rectangle(0.8, 0.36 + fDeliveryChargeSpace, 0.5, 0.005));
			MainLayout.Children.Add(ServicePickerUnderlineLayout);

			bool isTableService = (PaymentDeliveryType == Utils.DeliveryType.DeliveryTypeTableService);
			TableNumberPicker = new CustomSlidePicker
			{
				WidthRequest = 50,
				BackgroundColor = Color.Transparent,
				IsVisible = isTableService,
			};

			var TableNumberLabel = new DILabel
			{
				Text = "Table number",
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				YAlign = TextAlignment.Center,
				IsVisible = isTableService,
			};

			var TablePickerUnderlineLayout = new StackLayout {
				Spacing = 0,
				Padding = 0,
				BackgroundColor = Color.Gray,
				HeightRequest = 2,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				IsVisible = isTableService,
			};

			AbsoluteLayout.SetLayoutFlags(TableNumberLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(TableNumberLabel, new Rectangle(0.2, 0.42 + fDeliveryChargeSpace, 0.5, 0.1));
			MainLayout.Children.Add(TableNumberLabel);

			AbsoluteLayout.SetLayoutFlags(TableNumberPicker, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(TableNumberPicker, new Rectangle(0.8, 0.42 + fDeliveryChargeSpace, 0.5, 0.1));
			MainLayout.Children.Add(TableNumberPicker);

			AbsoluteLayout.SetLayoutFlags(TablePickerUnderlineLayout, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(TablePickerUnderlineLayout, new Rectangle(0.8, 0.46 + fDeliveryChargeSpace, 0.5, 0.005));
			MainLayout.Children.Add(TablePickerUnderlineLayout);

			var SecureCheckoutButton = new Button {
				Text = "Secure Checkout",
				TextColor = Color.Black,
				BackgroundColor = Color.FromRgb(171, 146, 91),
			};
			AbsoluteLayout.SetLayoutFlags(SecureCheckoutButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(SecureCheckoutButton, new Rectangle(0.5, 0.98, 0.8, 0.08));
			MainLayout.Children.Add(SecureCheckoutButton);
			SecureCheckoutButton.Clicked += async (object sender, EventArgs e) => {
				if (ServicePointPicker.SelectedIndex < 0)
				{
					DisplayAlert("Error", "Please select a service point.", "OK");
					return;
				}
				else if (TableNumberPicker.SelectedIndex < 0 && PaymentDeliveryType == Utils.DeliveryType.DeliveryTypeTableService)
				{
					DisplayAlert("Error", "Please select a table number.", "OK");
					return;
				}
				else
				{
					List<String> paramButtons = new List<String>();
					if (Globals.Config.CurrentVenue.settings.is_tabservice_enabled.Equals("1"))
					{
						String buttonString = "";
						if (tabAmount == 0.0)
							buttonString = "Open new Tab";
						else
							buttonString = "Pay by Tab";

						if (tabAmount > 0)
							buttonString += String.Format(" (${0:0.00})", tabAmount);
						paramButtons.Add(buttonString);
					}

					if (String.IsNullOrEmpty(ServiceDetailData.subscription_id) == false)
					{
						String trimmedString = ServiceDetailData.subscription_id.Substring(ServiceDetailData.subscription_id.Length >= 4 ? ServiceDetailData.subscription_id.Length - 4 : 0);
						String buttonString = String.Format(@"Pay by XXXX-{0}", trimmedString);
						paramButtons.Add(buttonString);
						paramButtons.Add("Pay by New Credit Card");
					}
					else
						paramButtons.Add("Pay by Credit Card");

					if (paramButtons.Count > 0)
					{
						var action = await DisplayActionSheet ("Pick one of the following", "Cancel", null, paramButtons.ToArray());

						if(action != null && !action.Equals("Cancel")){
							if (action.StartsWith("Open new Tab") || action.StartsWith("Pay by Tab"))
							{
								PaymentMethodType = Utils.PaymentMethod.PaymentMethodTab;
							}
							else if (action.StartsWith("Pay by XXXX"))
							{
								PaymentMethodType = Utils.PaymentMethod.PaymentMethodToken;
							}
							else
							{
								PaymentMethodType = Utils.PaymentMethod.PaymentMethodCreditCard;
							}

							ShowValidationPage();
						}
					}
					else
					{
						PaymentMethodType = Utils.PaymentMethod.PaymentMethodCreditCard;
						ShowValidationPage();
					}
				}
			};

			AbsoluteLayout.SetLayoutFlags(MainLayout, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(MainLayout, new Rectangle(0, 0.7, 1, 0.9));
			UILayout.Children.Add(MainLayout);

			ServicePointPicker.SelectionChanged = () => {
				int idx = ServicePointPicker.SelectedIndex;
				if (idx >= 0 && idx < ServicePoints.Count)
				{
					TableNumberPicker.PickerList.Clear();
					int startNumber = 0;
					int endNumber = 0;
					int.TryParse(ServicePoints[idx].start_no, out startNumber);
					int.TryParse(ServicePoints[idx].end_no, out endNumber);

					int tableNumberCnt = endNumber - startNumber + 1;
					string[] options = new string[tableNumberCnt];
					for (int i = startNumber; i <= endNumber; i++)
						options [i - startNumber] = "" + i;

					List<String> valueArrList = new List<string>();
					foreach (string option in options)
						valueArrList.Add(option);
					TableNumberPicker.PickerList = valueArrList;
					TableNumberPicker.SelectedIndex = 0;
				}
			};

			TableNumberPicker.SelectionChanged = () => {
				int idx = TableNumberPicker.SelectedIndex;
				if(idx >= 0){

				}
			};

			Content = UILayout;
		}
		#endregion

		#region EVENTS
		async private void GetDataFromService()
		{
			ShowLoading ();

			var response = await VenueService.GetServiceDetails (GetUserID(), GetUserToken(), Globals.Config.RestaurantId);

			HideLoading ();

			bool isResultSuccess = VenueService.HasSuccessResult (response.result);
			if (isResultSuccess) {
				DeliveryCharge = response.result.servicepoint_details.delievery_charges;
				ServicePoints = response.result.servicepoint_details.servicepoints;
				ServiceDetailData = response.result;
				tabAmount = 0;
				float.TryParse (ServiceDetailData.tab_amount, out tabAmount);
				if (ServicePoints.Count > 0) {
					string[] options = new string[ServicePoints.Count];
					for (int i = 0; i < ServicePoints.Count; i++)
						options [i] = ServicePoints [i].name;

					List<String> valueArrList = new List<string>();
					foreach (string option in options)
						valueArrList.Add(option);
					ServicePointPicker.PickerList = valueArrList;
					ServicePointPicker.SelectedIndex = 0;
				}

				try{
					float fDeliveryCharge = 0;
					float.TryParse(DeliveryCharge, out fDeliveryCharge);

					if (fDeliveryCharge > 0)
					{
						String strDeliveryCharge = String.Format("${0:0.00}", fDeliveryCharge);
						ChargeLabel.Text = strDeliveryCharge;
					}
				}
				catch (Exception ex) {
					Insights.Report (ex);
				}
			} else {
				await DisplayAlert ("Failure", response.result.message, "OK");
				ServicePointPicker.IsEnabled = false;
				TableNumberPicker.IsEnabled = false;
			}
		}

		async private void ShowValidationPage()
		{
			String type = "Unknown";
			if (PaymentMethodType == Utils.PaymentMethod.PaymentMethodCreditCard)
				type = "CreditCard";
			else if (PaymentMethodType == Utils.PaymentMethod.PaymentMethodTab)
				type = "Tab";
			else if (PaymentMethodType == Utils.PaymentMethod.PaymentMethodToken)
				type = "Token";

			Utils.MixPanel_Track("SelectPaymentType", "{VenueName:" + Globals.Config.RestaurantName + ", PaymentType:" + type + "}");
			// Check billing address
			// Check passcode exists
			if (PaymentMethodType == Utils.PaymentMethod.PaymentMethodTab)
			{
				if (tabAmount == 0.0)
				{					
					SaveOrderInformation (Utils.PaymentMethod.PaymentMethodTab, 0);
				}
				else
				{
					double totalOrderAmount = App._DbManager.GetPaymentTotalPrice(Globals.Config.RestaurantId);
					double tabBalanceAmount = tabAmount;

					if (tabBalanceAmount >= totalOrderAmount)
					{
						SaveOrderInformation (Utils.PaymentMethod.PaymentMethodTab, 1);
					}
					else
					{
						bool isAccept = await DisplayAlert ("Insufficient Balance", "Insufficient balance in your tab. Please open a new tab to continue.", "OK", "Cancel");
						if (isAccept == false)
							return;

						SaveOrderInformation (Utils.PaymentMethod.PaymentMethodTab, 0);
					}
				}
			}
			else if (PaymentMethodType == Utils.PaymentMethod.PaymentMethodToken)
			{
				SaveOrderInformation (Utils.PaymentMethod.PaymentMethodToken, 2);
			}
			else //if (paymentMethod == PaymentMethodCreditCard)
			{
				SaveOrderInformation (Utils.PaymentMethod.PaymentMethodCreditCard, 3);
			}
		}

		async private void SaveOrderInformation(Utils.PaymentMethod paymentMethod, int nextPageType)
		{
			// Create ShoppingCart Json
			List<PaymentTableItem> paymentList = App._DbManager.GetPaymentItems (Globals.Config.RestaurantId, true);
			List<PocketButler.ProcessPaymentPage.Menu_Table_Item> menuTableList = new List<PocketButler.ProcessPaymentPage.Menu_Table_Item> ();

			for (int i = 0; i < paymentList.Count; i++) {
				List<ShoppingCart_ItemExtraInfo> extraList = new List<ShoppingCart_ItemExtraInfo> ();
				List<ShoppingCart_ItemTypeInfo> typeList = new List<ShoppingCart_ItemTypeInfo> ();

				// Get Extra Items
				List<PaymentExtraItem> payment_extras = App._DbManager.GetPaymentExtraItems (paymentList [i].ID);
				foreach (PaymentExtraItem item in payment_extras) {
					extraList.Add (new ShoppingCart_ItemExtraInfo (){
						extra_id = item.ExtraId,
						extra_name = item.Name,
						extra_price = item.Price,
					});
				}

				// Get Type Items
				List<PaymentTypeItem> payment_types = App._DbManager.GetPaymentTypeItems (paymentList [i].ID);
				foreach (PaymentTypeItem itemType in payment_types) {
					typeList.Add (new ShoppingCart_ItemTypeInfo (){
						type_id = itemType.TypeID.ToString(),
						type_name = itemType.Name,
						type_price = itemType.Price,
					});
				}

				menuTableList.Add (new ProcessPaymentPage.Menu_Table_Item (){
					Id = paymentList[i].MenuId,
					Name = paymentList[i].MenuName,
					Price = paymentList[i].ItemPrice,
					Count = 1,
					Customization = paymentList[i].Customization,
					Extras = extraList,
					Types = typeList,
				});
			}

			Globals.Config.PaymentPageType = nextPageType;

			String servicePointName = ServicePointPicker.PickerList[ServicePointPicker.SelectedIndex];
			String tableNumber = (PaymentDeliveryType == Utils.DeliveryType.DeliveryTypeTableService) ? TableNumberPicker.PickerList [TableNumberPicker.SelectedIndex] : "";
			String deliveryCharges = (PaymentDeliveryType == Utils.DeliveryType.DeliveryTypeTableService) ? DeliveryCharge : "0";

			String shoppingcart = PaymentService.GetShoppingCart(Globals.Config.RestaurantName, Globals.Config.RestaurantId, menuTableList, tableNumber, "", "" + (int)PaymentDeliveryType, deliveryCharges, servicePointName, "" + (int)PaymentMethodType);
			Globals.Config.ShoppingCartJsonData = shoppingcart;
			//await Navigation.PushAsync(new CreatePassCodePage(false));

			int failedCount = 0;
			String FailedCountString = Utils.LoadDataFromSettings ("FailedCount");
			int.TryParse(FailedCountString, out failedCount);
			if (failedCount > 4) {
				InputDialogHelper.InputDialogResultEvent = InputDialogResultEventForChangePin;
				InputDialogHelper.ShowInputDialog (Forms.Context, "You have reached your maximum number of attempts. Please enter your PocketButler password to change pin.");
				return;
			}

			PassCodePage = new CreatePassCodePage (PageShowingEvent, false);
			await Navigation.PushAsync (PassCodePage);

			// You have reached your maximum number of attempts. Please enter your PocketButler password to change pin.
		}

		async void InputDialogResultEventForChangePin(String resultMsg)
		{
			if (Globals.Config.USER_INFO == null)
				return;

			ShowLoading ();
			var response = await LoginServices.CheckPassword(GetUserID(), GetUserToken(), resultMsg);
			bool isSuccess = LoginServices.HasSuccessResult (response);
			HideLoading ();

			if (isSuccess) {
				Utils.SaveDataToSettings ("FailedCount", "");
				PassCodePage = new CreatePassCodePage (PageShowingEvent, false);
				await Navigation.PushAsync (PassCodePage);
			} else {
				await DisplayAlert ("Error", "Invalid Password", "OK");
				return;
			}
		}
		#endregion
	}
}