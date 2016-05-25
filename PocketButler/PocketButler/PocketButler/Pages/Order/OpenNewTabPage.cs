using System;
using PocketButler.Controls;
using Xamarin.Forms;

namespace PocketButler
{
	public class OpenNewTabPage : BasePage
	{
		#region PRIVATE MEMBERS
		protected AbsoluteLayout UILayout { get; set; }
		protected AbsoluteLayout MainLayout { get; set; }
		DarkIceImage CheckOptImage;

		CustomEntry CardHolderName;
		CustomEntry CardCVV;
		CustomEntry CardNumberEntry;
		CustomDatePicker CardExpiryDate;

		Button AmountButton;
		CustomSeekBar ValuePicker;
		double selectedTabAmount = 0;

		new DarkIceImage BackgroundImage;

		#endregion

		public OpenNewTabPage (Action RefreshEvent)
		{
			BackAppearingEvent = RefreshEvent;

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

			BuildUI();
		}

		public override void OnPageShowing ()
		{
			App.IsWindowAdjustResize = true;
			base.OnPageShowing ();
		}

		protected override void OnDisappearing ()
		{
			App.IsWindowAdjustResize = false;
			base.OnDisappearing ();
		}

		#region PRIVATE METHODS
		private void BuildUI()
		{
			String isRemember = Utils.LoadDataFromSettings ("save_payment_token");
			String paymentCardCVV = Utils.LoadDataFromSettings ("payment_card_cvv");
			String payemntCardExpireMonth = Utils.LoadDataFromSettings ("payment_card_expiremonth");
			String paymentCardExpireYear = Utils.LoadDataFromSettings ("payment_card_expireyear");
			String paymentCardHolderName = Utils.LoadDataFromSettings ("payment_card_holdername");
			String paymentCardNumber = Utils.LoadDataFromSettings ("payment_card_number");

			if (String.IsNullOrEmpty (isRemember) || isRemember.Equals ("0")) {
				paymentCardCVV = "";
				payemntCardExpireMonth = "";
				paymentCardExpireYear = "";
				paymentCardHolderName = "";
				paymentCardNumber = "";
			}

			HideNavigationBar ();

			BackgroundImage = new DarkIceImage {
				VerticalOptions = LayoutOptions.Fill,
				HorizontalOptions = LayoutOptions.Fill,
				Source = ImageSource.FromFile ("welcome_background.png"),
				Aspect = Aspect.Fill,
			};

			AbsoluteLayout.SetLayoutFlags (BackgroundImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (BackgroundImage, new Rectangle (0, 0, 1, 1));
			MainLayout.Children.Add (BackgroundImage);

			// Register Custom Navigation Bar
			var TitleLogoImage = new DarkIceImage
			{
				Source = ImageSource.FromFile("welcome_logo.png"),
				Aspect = Aspect.AspectFit,
				HorizontalOptions = LayoutOptions.FillAndExpand
			};

			AbsoluteLayout.SetLayoutFlags(TitleLogoImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(TitleLogoImage, new Rectangle(0.5, 0.05, 0.5, 0.1));
			MainLayout.Children.Add(TitleLogoImage);

			var CheckoutTitleLabel = new DILabel {
				Text = "Open New Tab",
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
				TextColor = Color.White,
				Font = Font.SystemFontOfSize(NamedSize.Medium)
			};
			AbsoluteLayout.SetLayoutFlags(CheckoutTitleLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(CheckoutTitleLabel, new Rectangle(0.5, 0.12, 0.5, 0.1));
			MainLayout.Children.Add(CheckoutTitleLabel);

			var ItemImage = new DarkIceImage {
				Source = Globals.Config.RestaurantImage,
			};
			AbsoluteLayout.SetLayoutFlags(ItemImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ItemImage, new Rectangle(0.5, 0.25, 0.5, 0.2));
			MainLayout.Children.Add(ItemImage);

			var ItemTitleLabel = new DILabel {
				Text = Globals.Config.RestaurantName,
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
				TextColor = Color.FromRgb(171, 146, 91),
				Font = Font.SystemFontOfSize(NamedSize.Medium)
			};
			AbsoluteLayout.SetLayoutFlags(ItemTitleLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ItemTitleLabel, new Rectangle(0.5, 0.4, 0.5, 0.1));
			MainLayout.Children.Add(ItemTitleLabel);

			var TotalOrderAmountLabel = new DILabel {
				Text = "Select Tab Amount",
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
				TextColor = Color.White,
				Font = Font.SystemFontOfSize(NamedSize.Medium)
			};
			AbsoluteLayout.SetLayoutFlags(TotalOrderAmountLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(TotalOrderAmountLabel, new Rectangle(0.5, 0.47, 0.5, 0.1));
			MainLayout.Children.Add(TotalOrderAmountLabel);

			double dTotalPrice = App._DbManager.GetPaymentTotalPrice(Globals.Config.RestaurantId);
			ValuePicker = new CustomSeekBar {
				VerticalOptions = LayoutOptions.FillAndExpand,
				MinValue = 1,
				MaxValue = 20,
				SelectedValue = (int)(Math.Ceiling(dTotalPrice / Globals.Config.kStepper_Unit_Value))
			};

			ValuePicker.ValueChangedEvent += TableValueChangedEvent;
			selectedTabAmount = ValuePicker.SelectedValue * Globals.Config.kStepper_Unit_Value;

			AbsoluteLayout.SetLayoutFlags(ValuePicker, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ValuePicker, new Rectangle(0.5, 0.62, 0.5, 0.03));
			MainLayout.Children.Add(ValuePicker);

			AmountButton = new Button {
				Text = "$" + ValuePicker.SelectedValue * Globals.Config.kStepper_Unit_Value,
				TextColor = Color.White,
				BackgroundColor = Color.FromRgb(171, 146, 91),
				Font = Font.SystemFontOfSize(NamedSize.Medium)
			};
			AbsoluteLayout.SetLayoutFlags(AmountButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(AmountButton, new Rectangle(0.5, 0.55, 0.5, 0.07));
			MainLayout.Children.Add(AmountButton);

			var CardInfoBackButton = new Button {
				Text = "",
				BackgroundColor = Color.FromRgb (35, 35, 35),
				BorderColor = Color.FromRgb (94, 94, 94),
				BorderRadius = 1,
				BorderWidth = 1
			};
			AbsoluteLayout.SetLayoutFlags(CardInfoBackButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(CardInfoBackButton, new Rectangle(0.5, 0.8, 1, 0.18));
			MainLayout.Children.Add(CardInfoBackButton);

			CardNumberEntry = new CustomEntry
			{
				Placeholder = "Card Number",
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				HasBorder = false,
				PlaceholderTextColor = Color.Gray,
				Keyboard = Keyboard.Numeric,
				Text = paymentCardNumber,
			};
			AbsoluteLayout.SetLayoutFlags(CardNumberEntry, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(CardNumberEntry, new Rectangle(1, 0.7, 0.98, 0.07));
			MainLayout.Children.Add(CardNumberEntry);

			DateTime expireDate = DateTime.Now;
			try{
				expireDate = DateTime.Parse(paymentCardExpireYear + "/" + payemntCardExpireMonth);
			}
			catch (Exception ex) {
				expireDate = DateTime.Now;
			}

			CardExpiryDate = new CustomDatePicker {
				Date = expireDate,
				BackgroundColor = Color.Transparent,
				YAlign = TextAlignment.Center,
				TextColor = Color.White,
				DateFormat = "MM/yyyy",
				HideCalendarIndex = Utils.HIDE_CALENDAR_DAY,
				Hint = "Expiry Date",
			};

			AbsoluteLayout.SetLayoutFlags(CardExpiryDate, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(CardExpiryDate, new Rectangle(0.8, 0.76, 0.98, 0.07));
			MainLayout.Children.Add(CardExpiryDate);

			CardCVV = new CustomEntry
			{
				Keyboard = Keyboard.Numeric,
				Placeholder = "CVV",
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				HasBorder = false,
				PlaceholderTextColor = Color.Gray,
				IsPassword = true,
				Text = paymentCardCVV,
			};
			AbsoluteLayout.SetLayoutFlags(CardCVV, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(CardCVV, new Rectangle(1, 0.76, 0.2, 0.07));
			MainLayout.Children.Add(CardCVV);

			CardHolderName = new CustomEntry
			{
				Placeholder = "Card Holder Name",
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				HasBorder = false,
				PlaceholderTextColor = Color.Gray,
				Text = paymentCardHolderName,
			};
			AbsoluteLayout.SetLayoutFlags(CardHolderName, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(CardHolderName, new Rectangle(1, 0.82, 0.98, 0.07));
			MainLayout.Children.Add(CardHolderName);

			var MakePaymentButton = new Button {
				Text = "Make Payment",
				TextColor = Color.Black,
				BackgroundColor = Color.FromRgb(171, 146, 91),
			};
			AbsoluteLayout.SetLayoutFlags(MakePaymentButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(MakePaymentButton, new Rectangle(0.5, 0.98, 0.8, 0.07));
			MainLayout.Children.Add(MakePaymentButton);
			MakePaymentButton.Clicked += (object sender, EventArgs e) => {
				DoPayment();
			};

			//AbsoluteLayout.SetLayoutFlags(MainLayout, AbsoluteLayoutFlags.All);
			//AbsoluteLayout.SetLayoutBounds(MainLayout, new Rectangle(0, 0, 1, 1));
			//UILayout.Children.Add(MainLayout);

			Content = new ScrollView() { Content = MainLayout };
		}
		#endregion

		#region EVENTS
		async private void MenuButton_Clicked()
		{
			if (BackAppearingEvent != null)
				BackAppearingEvent.Invoke ();
			await Navigation.PopAsync ();
		}

		async private void DoPayment()
		{
			double totalOrderAmount = App._DbManager.GetPaymentTotalPrice(Globals.Config.RestaurantId);
			double tabAmount = selectedTabAmount;

			if (tabAmount < totalOrderAmount)
			{
				DisplayAlert("Error", String.Format("Tab amount must be greater than or equal to order amount (${0:0.00})", totalOrderAmount), "OK");
				return;
			}
			else
			{
				Globals.Config.TotalTabAmount = tabAmount.ToString ();
				await Navigation.PushAsync(new ProcessPaymentPage(PageShowingEvent, "", CardCVV.Text, CardExpiryDate.Date.Month, CardExpiryDate.Date.Year, CardHolderName.Text, CardNumberEntry.Text, false));
			}
		}

		private void TableValueChangedEvent()
		{
			selectedTabAmount = ValuePicker.SelectedValue * Globals.Config.kStepper_Unit_Value;
			AmountButton.Text = "$" + ((int)selectedTabAmount).ToString ();
		}

		#endregion
	}
}

