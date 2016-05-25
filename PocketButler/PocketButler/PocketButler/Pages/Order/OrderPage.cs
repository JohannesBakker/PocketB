using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using PocketButler.Controls;
using PocketButler.Services;

namespace PocketButler
{
    public class OrderPage : BasePage
    {
		#region PRIVATE MEMBERS
		protected AbsoluteLayout UILayout { get; set; }
		protected AbsoluteLayout MainLayout { get; set; }

		StackLayout PaymentItemsLayout { get; set; }
		CustomExpandableListView PaymentListView { get; set; }

		bool IsCustomizeRaised = false;

		Button PickupButton { get; set; }
		Button TableServiceButton { get; set; }

		CustomSlidePicker ServicePointPicker { get; set; }
		CustomSlidePicker TableNumberPicker { get; set; }
		CustomEntry PromotionCodeEntry { get; set; }
		Button ValidateCodeButton { get; set; }
		Button ValidateInputButton { get; set; }
		DarkIceImage ValidateImage { get; set; }
		DILabel SubTotalLabel { get; set; }
		DILabel DisCountLabel { get; set; }
		AbsoluteLayout ValidateLayout { get; set; }

		DILabel TableNumberLabel { get; set; }
		StackLayout TablePickerUnderlineLayout { get; set; }

		CustomDateTimePicker ServiceDateTimePicker { get; set; }
		Button ServiceTimeButton { get; set; }
		Button OpenNewTabButton { get; set; }

		DILabel TotalPriceLabel { get; set; }
		//TimePicker ServiceTimePicker { get; set; }
		//DatePicker ServiceDatePicker { get; set; }

		double DeliveryCharge = 0;
		double RedeemAmount = 0;
		float totalPrice = 0;
		bool redeemedAmountIsZero = false;

		ServiceDetailResultItem ServiceDetailData;
		float tabAmount = 0;
		List<Servicepoint> ServicePoints = new List<Servicepoint> ();

		int ServiceCategory = 0;

		//PocketButler.Utils.DeliveryType PaymentDeliveryType = PocketButler.Utils.DeliveryType.DeliveryTypeNone;
		PocketButler.Utils.PaymentMethod PaymentMethodType = PocketButler.Utils.PaymentMethod.PaymentMethodUnknown;

		bool IsSuccessPassCode = false;
		CreatePassCodePage PassCodePage = null;
		#endregion

		public OrderPage(Action RefreshEvent)
        {
			BackAppearingEvent = RefreshEvent;

			if (String.IsNullOrEmpty (Globals.Config.PaymentInfo.Price) == false) {
				string paymentValue = "";
				if (Globals.Config.PaymentInfo.Price.StartsWith (" $")) {
					paymentValue = Globals.Config.PaymentInfo.Price.Substring (2);
				} else
					paymentValue = Globals.Config.PaymentInfo.Price;

				if (String.IsNullOrEmpty(paymentValue) == false)
					float.TryParse (paymentValue, out totalPrice);
			}

			Title = "Order Screen";

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
			PageShowingEvent += OnPageShowing;

			GetDataFromService ();

			ChangeServiceCategory (0);
        }

		public override async void OnPageShowing()
		{
			base.OnPageShowing ();

			if (Globals.Config.CustomizeEventHandle.IsCustomizeUpdated == true) {
				List<ExpandableTableListItem> dataList = GetListPaymentItems();
				if (dataList == null || dataList.Count == 0) {
					if (BackAppearingEvent != null)
						BackAppearingEvent.Invoke ();
					await Navigation.PopAsync ();
				}
				else
					PaymentListView.ListData = dataList;

				Globals.Config.CustomizeEventHandle.IsCustomizeUpdated = false;
			}

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
			// Register Custom Navigation Bar
			var MenuImage = new DarkIceImage
			{
				Source = ImageSource.FromFile ("navbar_sidebar.png"),
				Aspect = Aspect.AspectFit,
				IsEnablePadding = true,
			};

			MakeCustomNavigationBar(UILayout, MenuImage, null, true);

			MenuImage.Tapped += MenuButton_Clicked;

			PaymentListView = new CustomExpandableListView { 
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				ListData = GetListPaymentItems(),
				IsExpandDefault = true,
			};

			PaymentListView.DeleteDelegate = PaymentItem_Delete_Clicked;
			PaymentListView.EditDelegate = PaymentItem_Edit_Item_Clicked;

			AbsoluteLayout.SetLayoutFlags(PaymentListView, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(PaymentListView, new Rectangle(0, 0.05, 1, 0.3));
			MainLayout.Children.Add(PaymentListView);

			PickupButton = new Button {
				Text = "Pickup",
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				TextColor = Color.FromRgb (171, 146, 91),
				BorderColor = Color.FromRgb (171, 146, 91),
				BackgroundColor = Color.Transparent,
				BorderRadius = 1,
				BorderWidth = 1,
			};
			PickupButton.Clicked += (object sender, EventArgs e) => ChangeServiceCategory(0);

			TableServiceButton = new Button {
				Text = "Table Service",
				Font = Font.SystemFontOfSize (NamedSize.Medium),
				TextColor = Color.FromRgb (171, 146, 91),
				BorderColor = Color.FromRgb (171, 146, 91),
				BackgroundColor = Color.Transparent,
				BorderRadius = 1,
				BorderWidth = 1,
			};
			TableServiceButton.Clicked += (object sender, EventArgs e) => ChangeServiceCategory(1);

			AbsoluteLayout.SetLayoutFlags(PickupButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(PickupButton, new Rectangle(0.15, 0.4, 0.4, 0.07));
			MainLayout.Children.Add(PickupButton);

			AbsoluteLayout.SetLayoutFlags(TableServiceButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(TableServiceButton, new Rectangle(0.85, 0.4, 0.4, 0.07));
			MainLayout.Children.Add(TableServiceButton);

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
			AbsoluteLayout.SetLayoutBounds(ServicePointLabel, new Rectangle(0.1, 0.48, 0.5, 0.1));
			MainLayout.Children.Add(ServicePointLabel);

			AbsoluteLayout.SetLayoutFlags(ServicePointPicker, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ServicePointPicker, new Rectangle(0.98, 0.48, 0.5, 0.1));
			MainLayout.Children.Add(ServicePointPicker);

			AbsoluteLayout.SetLayoutFlags(ServicePickerUnderlineLayout, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ServicePickerUnderlineLayout, new Rectangle(0.95, 0.5, 0.5, 0.003));
			MainLayout.Children.Add(ServicePickerUnderlineLayout);

			TableNumberPicker = new CustomSlidePicker
			{
				WidthRequest = 50,
				BackgroundColor = Color.Transparent,
			};
			/*
			ServiceDatePicker = new DatePicker {
				WidthRequest = 25,
				BackgroundColor = Color.Transparent,
				IsVisible = false,
			};

			ServiceTimePicker = new TimePicker {
				WidthRequest = 25,
				BackgroundColor = Color.Transparent,
				IsVisible = false,
			};*/

			ServiceDateTimePicker = new CustomDateTimePicker {
				WidthRequest = 50,
				BackgroundColor = Color.Transparent,
				IsVisible = false,
				TextColor = Color.White,
				Date = DateTime.Now,
				Hint = "ASAP",
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
				Text = "ASAP",
			};

			ServiceTimeButton = new Button {
				BackgroundColor = Color.Transparent,
				BorderColor = Color.Transparent,
				BorderWidth = 1,
				BorderRadius = 0,
				Text = "",
			};

			ServiceTimeButton.Clicked += async (object sender, EventArgs e) => {
				var action = await DisplayActionSheet("Select Time", "Cancel", null, "ASAP", "Select Time");
				if (action == "ASAP")
				{
					ServiceDateTimePicker.Text = "ASAP";
				}
				else if (action == "Select Time")
				{
					ServiceDateTimePicker.ShowTimePicker = !ServiceDateTimePicker.ShowTimePicker;
				}
			};

			TableNumberLabel = new DILabel
			{
				Text = "Table number",
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				YAlign = TextAlignment.Center,
			};

			TablePickerUnderlineLayout = new StackLayout {
				Spacing = 0,
				Padding = 0,
				BackgroundColor = Color.Gray,
				HeightRequest = 2,
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};

			AbsoluteLayout.SetLayoutFlags(TableNumberLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(TableNumberLabel, new Rectangle(0.1, 0.55, 0.5, 0.1));
			MainLayout.Children.Add(TableNumberLabel);
			/*
			AbsoluteLayout.SetLayoutFlags(ServiceDatePicker, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ServiceDatePicker, new Rectangle(0.6, 0.55, 0.25, 0.1));
			MainLayout.Children.Add(ServiceDatePicker);

			AbsoluteLayout.SetLayoutFlags(ServiceTimePicker, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ServiceTimePicker, new Rectangle(0.98, 0.55, 0.25, 0.1));
			MainLayout.Children.Add(ServiceTimePicker);*/

			AbsoluteLayout.SetLayoutFlags(TableNumberPicker, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(TableNumberPicker, new Rectangle(0.98, 0.55, 0.5, 0.1));
			MainLayout.Children.Add(TableNumberPicker);

			AbsoluteLayout.SetLayoutFlags(ServiceDateTimePicker, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ServiceDateTimePicker, new Rectangle(0.98, 0.55, 0.5, 0.1));
			MainLayout.Children.Add(ServiceDateTimePicker);

			AbsoluteLayout.SetLayoutFlags(ServiceTimeButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ServiceTimeButton, new Rectangle(0.98, 0.55, 0.5, 0.1));
			MainLayout.Children.Add(ServiceTimeButton);

			AbsoluteLayout.SetLayoutFlags(TablePickerUnderlineLayout, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(TablePickerUnderlineLayout, new Rectangle(0.95, 0.565, 0.5, 0.003));
			MainLayout.Children.Add(TablePickerUnderlineLayout);

			PromotionCodeEntry = new CustomEntry
			{
				WidthRequest = 50,
				BackgroundColor = Color.Transparent,
				Keyboard = Keyboard.Numeric,
			};

			PromotionCodeEntry.TextChanged += (object sender, TextChangedEventArgs e) => {
				ValidateLayout.IsVisible = true;
				ValidateInputButton.IsVisible = true;
				ValidateImage.IsVisible = false;
				RedeemAmount = 0;
				redeemedAmountIsZero = false;
				ChangeDiscountLabels();
			};

			var PromotionLabel = new DILabel
			{
				Text = "Promotion Code",
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				YAlign = TextAlignment.Center,
			};

			ValidateCodeButton = new Button {
				Text = "",
				TextColor = Color.White,
				BackgroundColor = Color.FromRgb(171, 146, 91),
				BorderColor = Color.FromRgb(171, 146, 91),
				BorderRadius = 1,
				BorderWidth = 1,
				Font = Font.SystemFontOfSize(NamedSize.Medium),
			};

			ValidateInputButton = new Button {
				Text = "",
				TextColor = Color.Transparent,
				BackgroundColor = Color.Transparent,
				BorderColor = Color.Transparent,
				BorderRadius = 0,
				BorderWidth = 0,
			};

			ValidateImage = new DarkIceImage {
				Source = "tick.png",
				Aspect = Aspect.AspectFit,
				IsVisible = false,
			};

			ValidateInputButton.Clicked += async (object sender, EventArgs e) => {
				ShowLoading();
				var response = await VenueService.CalculateRedeemAmountForVenue (GetUserID(), GetUserToken(), Globals.Config.RestaurantId, PromotionCodeEntry.Text, totalPrice.ToString("0.00"));
				HideLoading ();

				bool isResultSuccess = VenueService.HasSuccessResult (response.result);
				redeemedAmountIsZero = false;
				if (isResultSuccess) {
					ValidateLayout.IsVisible = false;
					ValidateInputButton.IsVisible = false;
					ValidateImage.IsVisible = true;
					RedeemAmount = response.result.discountedAmount;

					if (RedeemAmount == 0.00)
						redeemedAmountIsZero = true;
					
					ChangeDiscountLabels();
				}
				else{
					ValidateCodeButton.IsVisible = true;
					ValidateImage.IsVisible = false;
					ChangeDiscountLabels();
					DisplayAlert ("Error", "Invalid promotion code", "OK");
				}
			};

			if (Globals.Config.CurrentVenue != null && Globals.Config.CurrentVenue.supports_promotion_code == 0)
				PromotionCodeEntry.IsEnabled = false;

			var ValidateCodeText = new DILabel {
				//IsDefaultLabel = true,
				Text = "Validate",
				TextColor = Color.White,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
				Font = Font.SystemFontOfSize(NamedSize.Medium),
			};

			var ProcodeUnderlineLayout = new StackLayout {
				Spacing = 0,
				Padding = 0,
				BackgroundColor = Color.Gray,
				HeightRequest = 2,
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};

			ValidateLayout = new AbsoluteLayout {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
			};

			AbsoluteLayout.SetLayoutFlags(ValidateCodeButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ValidateCodeButton, new Rectangle(0, 0, 1, 1));
			ValidateLayout.Children.Add(ValidateCodeButton);

			AbsoluteLayout.SetLayoutFlags(ValidateCodeText, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ValidateCodeText, new Rectangle(0, 0, 1, 1));
			ValidateLayout.Children.Add(ValidateCodeText);

			SubTotalLabel = new DILabel
			{
				WidthRequest = 50,
				BackgroundColor = Color.Transparent,
				YAlign = TextAlignment.Center,
				XAlign = TextAlignment.End,
			};

			var SubTotalTextLabel = new DILabel
			{
				Text = "Sub-Total",
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				YAlign = TextAlignment.Center,
			};

			AbsoluteLayout.SetLayoutFlags(SubTotalTextLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(SubTotalTextLabel, new Rectangle(0.1, 0.68, 0.5, 0.1));
			MainLayout.Children.Add(SubTotalTextLabel);

			AbsoluteLayout.SetLayoutFlags(SubTotalLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(SubTotalLabel, new Rectangle(0.92, 0.68, 0.5, 0.1));
			MainLayout.Children.Add(SubTotalLabel);

			DisCountLabel = new DILabel
			{
				WidthRequest = 50,
				BackgroundColor = Color.Transparent,
				Text = "$0.00",
				YAlign = TextAlignment.Center,
				XAlign = TextAlignment.End,
			};

			var DiscountTextLabel = new DILabel
			{
				Text = "Discount",
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				YAlign = TextAlignment.Center,
			};

			AbsoluteLayout.SetLayoutFlags(DiscountTextLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(DiscountTextLabel, new Rectangle(0.1, 0.74, 0.5, 0.1));
			MainLayout.Children.Add(DiscountTextLabel);

			AbsoluteLayout.SetLayoutFlags(DisCountLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(DisCountLabel, new Rectangle(0.92, 0.74, 0.5, 0.1));
			MainLayout.Children.Add(DisCountLabel);

			AbsoluteLayout.SetLayoutFlags(PromotionLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(PromotionLabel, new Rectangle(0.1, 0.62, 0.5, 0.1));
			MainLayout.Children.Add(PromotionLabel);

			AbsoluteLayout.SetLayoutFlags(PromotionCodeEntry, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(PromotionCodeEntry, new Rectangle(0.6, 0.62, 0.25, 0.1));
			MainLayout.Children.Add(PromotionCodeEntry);

			AbsoluteLayout.SetLayoutFlags(ProcodeUnderlineLayout, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ProcodeUnderlineLayout, new Rectangle(0.6, 0.635, 0.25, 0.003));
			MainLayout.Children.Add(ProcodeUnderlineLayout);

			AbsoluteLayout.SetLayoutFlags(ValidateImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ValidateImage, new Rectangle(0.8, 0.62, 0.1, 0.05));
			MainLayout.Children.Add(ValidateImage);

			AbsoluteLayout.SetLayoutFlags(ValidateLayout, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ValidateLayout, new Rectangle(0.98, 0.62, 0.25, 0.05));
			MainLayout.Children.Add(ValidateLayout);

			AbsoluteLayout.SetLayoutFlags(ValidateInputButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ValidateInputButton, new Rectangle(0.98, 0.64, 0.25, 0.1));
			MainLayout.Children.Add(ValidateInputButton);

			var TotalInfoBackButton = new Button {
				Text = "",
				BackgroundColor = Color.Transparent,
				BorderColor = Color.FromRgb (94, 94, 94),
				BorderRadius = 1,
				BorderWidth = 1
			};
			AbsoluteLayout.SetLayoutFlags(TotalInfoBackButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(TotalInfoBackButton, new Rectangle(0.5, 0.81, 1, 0.07));
			MainLayout.Children.Add(TotalInfoBackButton);

			var TotalLabel = new DILabel {
				Text = "Total",
				TextColor = Color.White,
				Font = Font.SystemFontOfSize(NamedSize.Medium),
				YAlign = TextAlignment.Center,
			};
			AbsoluteLayout.SetLayoutFlags(TotalLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(TotalLabel, new Rectangle(0.1, 0.81, 0.5, 0.07));
			MainLayout.Children.Add(TotalLabel);

			TotalPriceLabel = new DILabel {
				Text = "",
				TextColor = Color.White,
				Font = Font.SystemFontOfSize(NamedSize.Medium),
				YAlign = TextAlignment.Center,
				XAlign = TextAlignment.End,
			};
			AbsoluteLayout.SetLayoutFlags(TotalPriceLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(TotalPriceLabel, new Rectangle(0.94, 0.81, 0.2, 0.07));
			MainLayout.Children.Add(TotalPriceLabel);

			//TotalPriceLabel.BindingContext = Globals.Config.PaymentInfo;
			//TotalPriceLabel.SetBinding (Label.TextProperty, "Price");

			var HintAlcholLabel = new DILabel {
				Text = "It is against the law to sell or supply alcohol to, or to obtain alcohol on behalf of, a person under the age of 18 years.",
				TextColor = Color.White,
				Font = Font.SystemFontOfSize(NamedSize.Small, FontAttributes.Italic),
				YAlign = TextAlignment.Center,
				Lines = 2,
				IsDefaultLabel = true,
				LineBreakMode = LineBreakMode.CharacterWrap,
			};

			AbsoluteLayout.SetLayoutFlags(HintAlcholLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(HintAlcholLabel, new Rectangle(0.5, 0.95, 0.9, 0.2));
			MainLayout.Children.Add(HintAlcholLabel);
			/*
			var CheckoutButton = new Button {
				Text = "Check Out",
				TextColor = Color.Black,
				BackgroundColor = Color.FromRgb(171, 146, 91),
			};
			AbsoluteLayout.SetLayoutFlags(CheckoutButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(CheckoutButton, new Rectangle(0.5, 0.99, 0.8, 0.08));
			MainLayout.Children.Add(CheckoutButton);
			CheckoutButton.Clicked += async (object sender, EventArgs e) => {

				if(PaymentListView.ListData == null || PaymentListView.ListData.Count == 0){
					await DisplayAlert ("Info", "No Order", "OK");
					return;
				}

				if (Globals.Config.CurrentVenue.settings.is_pickup_enabled.Equals("1") && Globals.Config.CurrentVenue.settings.is_tableservice_enabled.Equals("1"))
				{
					var action = await DisplayActionSheet ("Select a method", "Cancel", null, "Pick-up", "Table Service");
					if (action == "Pick-up")
					{
						MoveToPaymentPage(0);
					}
					else if (action == "Table Service")
					{
						MoveToPaymentPage(1);
					}
				}
				else if (Globals.Config.CurrentVenue.settings.is_pickup_enabled.Equals("1"))
				{
					MoveToPaymentPage(0);
				}
				else
				{
					MoveToPaymentPage(1);
				}
			};*/

			var PayNowButton = new Button {
				Text = "Pay now",
				TextColor = Color.Black,
				BackgroundColor = Color.FromRgb(171, 146, 91),
			};
			AbsoluteLayout.SetLayoutFlags(PayNowButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(PayNowButton, new Rectangle(0.1, 0.99, 0.4, 0.08));
			MainLayout.Children.Add(PayNowButton);

			if (Globals.Config.CurrentVenue.settings.is_pickup_enabled.Equals ("0") && Globals.Config.CurrentVenue.settings.is_tableservice_enabled.Equals ("0"))
				PayNowButton.IsEnabled = false;
			
			String buttonTitle = "";
			if (Globals.Config.CurrentVenue.settings.is_tabservice_enabled.Equals ("1")) {
				if (Globals.Config.CurrentVenue != null && Globals.Config.CurrentVenue.tab_amount == 0.0)
					buttonTitle = "Open new Tab";
				else
					buttonTitle = "Pay by Tab";
			}
			
			OpenNewTabButton = new Button {
				Text = buttonTitle,
				TextColor = Color.Black,
				BackgroundColor = Color.FromRgb(171, 146, 91),
				IsVisible = (String.IsNullOrEmpty(buttonTitle) == false),
			};
			AbsoluteLayout.SetLayoutFlags(OpenNewTabButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(OpenNewTabButton, new Rectangle(0.9, 0.99, 0.4, 0.08));
			MainLayout.Children.Add(OpenNewTabButton);

			AbsoluteLayout.SetLayoutFlags(MainLayout, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(MainLayout, new Rectangle(0, 0.7, 1, 0.9));
			UILayout.Children.Add(MainLayout);

			Content = UILayout;

			PayNowButton.Clicked += (object sender, EventArgs e) => {
				DoPaymentNow();
			};

			OpenNewTabButton.Clicked += (object sender, EventArgs e) => {
				DoOpenNewTab();
			};

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
		}

		private async void MoveToPaymentPage(int type)
		{
			ShowLoading ();
			List<PaymentTableItem> items = App._DbManager.GetPaymentItems (Globals.Config.RestaurantId, true);
			String item_ids = "";
			foreach (PaymentTableItem item in items) {
				if (String.IsNullOrEmpty (item_ids))
					item_ids = item.MenuId;
				else
					item_ids += "," + item.MenuId;
			}

			var result = await VenueService.ValidateItems (GetUserID (), GetUserToken (), Globals.Config.RestaurantId, item_ids);
			HideLoading ();

			try{
				if (VenueService.HasSuccessResult (result.result)) {
					if (type == 0) { // Pick UP
						Globals.Config.PaymentType = PocketButler.Utils.DeliveryType.DeliveryTypePickup;
						await Navigation.PushAsync (new ServiceTablePage (PageShowingEvent));
					} else if (type == 1) { // Table Service
						Globals.Config.PaymentType = PocketButler.Utils.DeliveryType.DeliveryTypeTableService;
						await Navigation.PushAsync (new ServiceTablePage (PageShowingEvent));
					}
				} else {
					String message = "";
					if (string.IsNullOrEmpty (result.result.message) == false)
						message = result.result.message;
					else
						message = "Can't place order";

					DisplayAlert ("Warning", message, "OK");
				}
			}
			catch (Exception ex) {
			}
		}

		private List<ExpandableTableListItem> GetListPaymentItems()
		{
			List<ExpandableTableListItem> listItems = new List<ExpandableTableListItem> ();
			List<PaymentTableItem> paymentList = App._DbManager.GetPaymentItems (Globals.Config.RestaurantId);

			while (paymentList.Count > 0) {
				int curTypeCnt = 0;

				String curMenuItemId = paymentList [0].MenuId;
				String menuName = paymentList[0].MenuName;
				String menuPrice = paymentList [0].ItemPrice;

				double dItemPrice = 0.0;
				double.TryParse (menuPrice, out dItemPrice);

				List<PaymentTableItem> menuItems = new List<PaymentTableItem> ();

				double dTotalPrice = 0.0;

				// Remove same items from the list
				for (int i = paymentList.Count - 1; i >= 0; i--) {
					if (paymentList [i].MenuId == curMenuItemId) {

						double.TryParse (paymentList [i].ItemPrice, out dItemPrice);

						dTotalPrice += dItemPrice;
						/*
						List<PaymentExtraItem> extraItems = App._DbManager.GetPaymentExtraItems (paymentList [i].ID);
						foreach (PaymentExtraItem extraItem in extraItems) {
							double dExtra = 0.0;
							double.TryParse (extraItem.Price, out dExtra);
							dTotalPrice += dExtra;
						}*/

						menuItems.Add (paymentList [i]);
						paymentList.RemoveAt (i);
						curTypeCnt++;
					}
				}

				if (curTypeCnt > 0) {
					ExpandableTableListItem item = new ExpandableTableListItem {
						Count = curTypeCnt,
						ItemName = menuName,
						IsExpanded = false,
						Items = menuItems,
						Price = dTotalPrice.ToString("0.00"),
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

		async void PaymentItem_Delete_Clicked(String restaurant_id, String item_id, int uid)
		{
			var result = await DisplayAlert("Warning", "Do you really want to delete this item?", "Yes", "No");
			if (result == true) {
				App._DbManager.RemovePaymentItem (uid);
				Globals.Config.PaymentInfo.Price = App._DbManager.GetPaymentTotalPrice (Globals.Config.RestaurantId).ToString("0.00");
				totalPrice = (float)App._DbManager.GetPaymentTotalPrice (Globals.Config.RestaurantId);
				List<ExpandableTableListItem> dataList = GetListPaymentItems();
				if (dataList == null || dataList.Count == 0) {
					if (BackAppearingEvent != null)
						BackAppearingEvent.Invoke ();
					await Navigation.PopAsync ();
				}
				else
					PaymentListView.ListData = dataList;
				PaymentListView.ListData = dataList;
			}

			ValidateLayout.IsVisible = true;
			ValidateInputButton.IsVisible = true;
			ValidateImage.IsVisible = false;
			RedeemAmount = 0;
			redeemedAmountIsZero = false;
			ChangeDiscountLabels();
		}

		async void PaymentItem_Edit_Item_Clicked(String restaurant_id, String item_id, int uid)
		{
			PaymentTableItem item = App._DbManager.GetPaymentItemWithId(uid);
			List<PaymentExtraItem> extras = App._DbManager.GetPaymentExtraItems (uid);
			List<PaymentTypeItem> types = App._DbManager.GetPaymentTypeItems (uid);
			if (item != null) {
				IsCustomizeRaised = true;
				var customPage = new VenueCustomizePage(item_id, true, "", item.Customization, uid, true, extras, types, PageShowingEvent);
				await Navigation.PushAsync (customPage);
			}
		}
		#endregion

		private void ChangeServiceCategory(int category)
		{
			ServiceCategory = category;

			if (category == 0) {
				TableNumberLabel.Text = "Collection Time";
				PickupButton.BackgroundColor = Color.FromRgb (171, 146, 91);
				PickupButton.TextColor = Color.Black;
				TableServiceButton.BackgroundColor = Color.Transparent;
				TableServiceButton.TextColor = Color.FromRgb (171, 146, 91);

				//SubTotalLabel.Text = String.Format("${0:0.00}", totalPrice);

				TableNumberPicker.IsVisible = false;

				//Globals.Config.PaymentInfo.Price = totalPrice.ToString("0.00");
				//TablePickerUnderlineLayout.IsVisible = false;
				//ServiceDatePicker.IsVisible = true;
				//ServiceTimePicker.IsVisible = true;
				ServiceDateTimePicker.IsVisible = true;
				ServiceTimeButton.IsVisible = true;
			} else {
				TableNumberLabel.Text = "Table Number";
				TableServiceButton.BackgroundColor = Color.FromRgb (171, 146, 91);
				TableServiceButton.TextColor = Color.Black;
				PickupButton.BackgroundColor = Color.Transparent;
				PickupButton.TextColor = Color.FromRgb (171, 146, 91);

				//SubTotalLabel.Text = String.Format("${0:0.00} Delivery + ${1:0.00}", DeliveryCharge, totalPrice + DeliveryCharge);

				TableNumberPicker.IsVisible = true;

				//Globals.Config.PaymentInfo.Price = (totalPrice + DeliveryCharge).ToString("0.00");
				//TablePickerUnderlineLayout.IsVisible = true;
				//ServiceDatePicker.IsVisible = false;
				//ServiceTimePicker.IsVisible = false;

				ServiceDateTimePicker.IsVisible = false;
				ServiceTimeButton.IsVisible = false;
			}

			ChangeDiscountLabels ();
		}

		async private void GetDataFromService()
		{
			ShowLoading ();

			var response = await VenueService.GetServiceDetails (GetUserID(), GetUserToken(), Globals.Config.RestaurantId);

			HideLoading ();

			bool isResultSuccess = VenueService.HasSuccessResult (response.result);
			if (isResultSuccess) {
				var strDeliveryCharge = response.result.servicepoint_details.delievery_charges;
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
					float.TryParse(strDeliveryCharge, out fDeliveryCharge);

					DeliveryCharge = fDeliveryCharge;

					if (fDeliveryCharge > 0)
					{
						//String strDeliveryCharge = String.Format("${0:0.00}", fDeliveryCharge);
						//ChargeLabel.Text = strDeliveryCharge;
						ChangeServiceCategory(ServiceCategory);
					}
				}
				catch (Exception ex) {					
				}

				String buttonTitle = "";
				if (Globals.Config.CurrentVenue.settings.is_tabservice_enabled.Equals ("1")) {
					if (Globals.Config.CurrentVenue != null && tabAmount == 0.0)
						buttonTitle = "Open new Tab";
					else
						buttonTitle = "Pay by Tab";
				}

				OpenNewTabButton.Text = buttonTitle;
				OpenNewTabButton.IsVisible = (String.IsNullOrEmpty (buttonTitle) == false);
			} else {
				await DisplayAlert ("Failure", response.result.message, "OK");
				ServicePointPicker.IsEnabled = false;
				TableNumberPicker.IsEnabled = false;
			}
		}

		private async void DoPaymentNow()
		{
			if (ServicePointPicker.SelectedIndex < 0)
			{
				DisplayAlert("Error", "Please select a service point.", "OK");
				return;
			}
			else
			{
				double curvalue = (RedeemAmount > 0.0) ? RedeemAmount : totalPrice;
				curvalue += ((ServiceCategory == 0) ? 0 : DeliveryCharge);

				Globals.Config.PaymentInfo.TotalPrice = curvalue;
				Globals.Config.PaymentInfo.Price = curvalue.ToString("0.00");

				List<String> paramButtons = new List<String>();
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
						if (action.StartsWith("Pay by XXXX"))
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
		}

		private async void DoOpenNewTab()
		{
			if (ServicePointPicker.SelectedIndex < 0) {
				DisplayAlert ("Error", "Please select a service point.", "OK");
				return;
			} else if (TableNumberPicker.SelectedIndex < 0 && ServiceCategory == 1) {
				DisplayAlert ("Error", "Please select a table number.", "OK");
				return;
			} else {
				double curvalue = (RedeemAmount > 0.0) ? RedeemAmount : totalPrice;
				curvalue += ((ServiceCategory == 0) ? 0 : DeliveryCharge);

				Globals.Config.PaymentInfo.TotalPrice = curvalue;
				Globals.Config.PaymentInfo.Price = curvalue.ToString("0.00");

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

				if (paramButtons.Count > 0)
				{
					var action = await DisplayActionSheet ("Pick one of the following", "Cancel", null, paramButtons.ToArray());

					if(action != null && !action.Equals("Cancel")){
						if (action.StartsWith("Open new Tab") || action.StartsWith("Pay by Tab"))
						{
							PaymentMethodType = Utils.PaymentMethod.PaymentMethodTab;
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
					double totalOrderAmount = (RedeemAmount > 0.0) ? RedeemAmount : totalPrice;
					totalOrderAmount += ((ServiceCategory == 0) ? 0 : DeliveryCharge);
					//double totalOrderAmount = App._DbManager.GetPaymentTotalPrice(Globals.Config.RestaurantId);
					double tabBalanceAmount = tabAmount;

					if (tabBalanceAmount >= totalOrderAmount)
					{
						Utils.MixPanel_Track ("PaymentTab", "{Value:" + totalOrderAmount + "}");
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
				Utils.MixPanel_Track ("PaymentToken", "");
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

			// MMM dd, yyyy HH:mm
			String pickupTime = "";
			String servicePointName = ServicePointPicker.PickerList[ServicePointPicker.SelectedIndex];
			String tableNumber = (ServiceCategory == 1) ? TableNumberPicker.PickerList [TableNumberPicker.SelectedIndex] : "";
			if (ServiceDateTimePicker.IsDateSelected)
				pickupTime = ServiceDateTimePicker.Date.ToString ("MMM dd, yyyy HH:mm");
			else
				pickupTime = "";

			Globals.Config.PickupTime = pickupTime;
			
			String deliveryCharges = (ServiceCategory == 1) ? DeliveryCharge.ToString() : "0";

			String shoppingcart = PaymentService.GetShoppingCart(Globals.Config.RestaurantName, Globals.Config.RestaurantId, menuTableList, tableNumber, pickupTime, "" + (int)(ServiceCategory + 1), deliveryCharges, servicePointName, "" + (int)PaymentMethodType);
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

		private void ChangeDiscountLabels()
		{
			double subtotal = totalPrice;
			double discountAmount = (RedeemAmount > 0.0) ? Math.Abs (subtotal - RedeemAmount) : 0;
			double discountLabel = (RedeemAmount > 0.0) ? RedeemAmount : subtotal;

			if (redeemedAmountIsZero) {
				discountAmount = Math.Abs (subtotal - RedeemAmount);
				discountLabel = 0;
			}

			discountLabel += ((ServiceCategory == 0) ? 0 : DeliveryCharge);

			SubTotalLabel.Text = (ServiceCategory == 0) ? "$" + subtotal.ToString("0.00") : String.Format("${0:0.00} Delivery + ${1:0.00}", DeliveryCharge, subtotal);
			DisCountLabel.Text = "$" + discountAmount.ToString("0.00");
			TotalPriceLabel.Text = "$" + discountLabel.ToString("0.00");
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
		}
    }
}
