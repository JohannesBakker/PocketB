using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using PocketButler.Controls;

namespace PocketButler
{
    public class ProcessPaymentPage : BasePage
    {
		public class Menu_Table_Item
		{
			public String Id { get; set; }
			public String Name { get; set; }
			public String Price { get; set; }
			public int Count { get; set; }
			public String Customization { get; set; }

			public List<ShoppingCart_ItemExtraInfo> Extras { get; set; }
			public List<ShoppingCart_ItemTypeInfo> Types { get; set; }
		}

		#region PRIVATE MEMBERS
		protected AbsoluteLayout UILayout { get; set; }
		protected AbsoluteLayout MainLayout { get; set; }

		new DarkIceImage BackgroundImage;

		bool IsRemember = false;
		String StripeToken = "";
		String PaymentCVC = "";
		int PaymentDateMonth;
		int PaymentDateYear;
		String PaymentNumber = "";
		String PaymentName = "";

		#endregion

		public ProcessPaymentPage(Action RefreshEvent, String stripeToken, String paymentCVC, int paymentDateMonth, int paymentDateYear, String paymentName, String paymentNumber, bool isRemember)
        {
			BackAppearingEvent = RefreshEvent;

			IsRemember = isRemember;
			StripeToken = stripeToken;
			PaymentCVC = paymentCVC;
			PaymentDateMonth = paymentDateMonth;
			PaymentDateYear = paymentDateYear;
			PaymentName = paymentName;
			PaymentNumber = paymentNumber;

			HideNavigationBar ();
			Title = "";

			MainLayout = new AbsoluteLayout
			{
				VerticalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.Transparent
			};

			BuildUI();
        }

		protected override async void OnAppearing ()
		{
			base.OnAppearing ();

			DoPayment();
		}

		#region PRIVATE METHODS
		private void BuildUI()
		{
			BackgroundImage = new DarkIceImage {
				VerticalOptions = LayoutOptions.Fill,
				HorizontalOptions = LayoutOptions.Fill,
				Source = ImageSource.FromFile ("welcome_background.png"),
				Aspect = Aspect.Fill,
			};

			AbsoluteLayout.SetLayoutFlags (BackgroundImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (BackgroundImage, new Rectangle (0, 0, 1, 1));
			MainLayout.Children.Add (BackgroundImage);

			var LogoImage = new DarkIceImage {
				VerticalOptions = LayoutOptions.Fill,
				HorizontalOptions = LayoutOptions.Fill,
				Source = ImageSource.FromFile ("welcome_logo.png"),
				Aspect = Aspect.Fill
			};

			AbsoluteLayout.SetLayoutFlags (LogoImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (LogoImage, new Rectangle (0.5, 0.22, 0.5, 0.07));
			MainLayout.Children.Add (LogoImage);

			var BottomHintLabel = new CustomLabel {
				Text = "Please wait while we are processing your payment...",
				TextColor = Color.White,
				XAlign = TextAlignment.Center,
				Font = Font.SystemFontOfSize (NamedSize.Medium),
			};

			AbsoluteLayout.SetLayoutFlags (BottomHintLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (BottomHintLabel, new Rectangle (0.5, 0.98, 0.8, 0.1));
			MainLayout.Children.Add (BottomHintLabel);

			Content = MainLayout;
		}

		async private void DoPayment()
		{
			try{
				ShowLoading ();
				String shoppingcart = Globals.Config.ShoppingCartJsonData;
				PaymentResult response = new PaymentResult();
				int paymentType = Globals.Config.PaymentPageType;

				if (paymentType == 1 || paymentType == 2) {
					StripeToken = "";
				} else {
					string stripekey = ((IsRemember) ? Globals.Config.CurrentVenue.pb_stripe_publishable_key : Globals.Config.CurrentVenue.merchant_stripe_publishable_key);
					if (String.IsNullOrEmpty (stripekey))
						stripekey = Globals.Config.STRIPE_TEST_KEY;
					StripeToken = await PaymentService.GetStripeToken (stripekey, Globals.Config.USER_INFO.contact_address.country_name, Globals.Config.USER_INFO.contact_address.postal_code, PaymentCVC, PaymentName, Globals.Config.USER_INFO.contact_address.street_name, Globals.Config.USER_INFO.contact_address.state_name, Globals.Config.USER_INFO.contact_address.city_name, PaymentDateMonth, PaymentDateYear, PaymentNumber);
					if (String.IsNullOrEmpty (StripeToken)) {
						HideLoading ();
						await DisplayAlert ("Error", "An error occured while processing your payment.", "OK");
						if (BackAppearingEvent != null)
							BackAppearingEvent.Invoke ();
						await Navigation.PopAsync ();
						return;
					}
				}

				if (paymentType == 0) { // Open Tab
					response = await PaymentService.ProcessOrderPaymentViaNewTab(GetUserID(), GetUserEmail(), GetUserToken(), Globals.Config.RestaurantId, Globals.Config.RestaurantName, shoppingcart, StripeToken, "" + Globals.Config.TotalTabAmount);
				} else if (paymentType == 1) { // Pay by Tab
					response = await PaymentService.ProcessOrderPaymentViaExistingTab(GetUserID(), GetUserEmail(), GetUserToken(), Globals.Config.RestaurantId, Globals.Config.RestaurantName, shoppingcart, StripeToken, "" + App._DbManager.GetPaymentTotalPrice(Globals.Config.RestaurantId));
				} else if (paymentType == 2) { // Pay by Token
					response = await PaymentService.ProcessOrderPaymentViaExistingToken(GetUserID(), GetUserEmail(), GetUserToken(), Globals.Config.RestaurantId, Globals.Config.RestaurantName, shoppingcart, StripeToken, "" + App._DbManager.GetPaymentTotalPrice(Globals.Config.RestaurantId));
				} else /*if (paymentType == 3)*/ {
					if (IsRemember == true)
						response = await PaymentService.ProcessOrderPaymentViaFirstTimeToken(GetUserID(), GetUserEmail(), GetUserToken(), Globals.Config.RestaurantId, Globals.Config.RestaurantName, shoppingcart, StripeToken, "" + App._DbManager.GetPaymentTotalPrice(Globals.Config.RestaurantId));
					else
						response = await PaymentService.ProcessOrderPaymentViaCC(GetUserID(), GetUserEmail(), GetUserToken(), Globals.Config.RestaurantId, Globals.Config.RestaurantName, shoppingcart, StripeToken, "" + App._DbManager.GetPaymentTotalPrice(Globals.Config.RestaurantId));
				}

				bool isResultSuccess = VenueService.HasSuccessResult (response.result);
				OrderHistory orderDetail = null;
				if (isResultSuccess) {
					var responseOrder = await PaymentService.GetOrderHistory (GetUserID (), GetUserToken (), 0);
					if (VenueService.HasSuccessResult (responseOrder.result)) {
						foreach (OrderHistory history in responseOrder.result.order_list) {
							if (history.order_id.Equals (response.result.order_info.order_id)) {
								orderDetail = history;
								break;
							}
						}
					}
				}

				HideLoading ();

				Utils.MixPanel_Track("PaymentResult", "{Result:" + isResultSuccess + "}");
				if (isResultSuccess) {
					PaymentResultItem result = response.result;
					if (IsRemember) {
						Utils.SaveDataToSettings ("save_payment_token", (IsRemember) ? "1" : "0");
						Utils.SaveDataToSettings ("stripe_token", StripeToken);

						Utils.SaveDataToSettings ("payment_card_cvv", PaymentCVC);
						Utils.SaveDataToSettings ("payment_card_expiremonth", "" + PaymentDateMonth);
						Utils.SaveDataToSettings ("payment_card_expireyear", "" + PaymentDateYear);
						Utils.SaveDataToSettings ("payment_card_holdername", PaymentName);
						Utils.SaveDataToSettings ("payment_card_number", PaymentNumber);
					}

					await DisplayAlert ("Success", result.message, "OK");
					App._DbManager.RemovePaymentItems (Globals.Config.RestaurantId);
					Globals.Config.PaymentInfo.Price = App._DbManager.GetPaymentTotalPrice (Globals.Config.RestaurantId).ToString("0.00");

					if (orderDetail == null) {
						await DisplayAlert ("Failure", "Cannot get order history", "OK");
						if (BackAppearingEvent != null)
							BackAppearingEvent.Invoke ();
						await Navigation.PopAsync ();
					} else {
						var ResultOrderDetailPage = new OrderDetailPage (orderDetail, PageShowingEvent, false);
						await Navigation.PushAsync (ResultOrderDetailPage);
					}

				} else {
					await DisplayAlert ("Failure", response.result.message, "OK");
					if (BackAppearingEvent != null)
						BackAppearingEvent.Invoke ();
					await Navigation.PopAsync ();
				}
			}
			catch (Exception ex) {
				HideLoading ();
				if (BackAppearingEvent != null)
					BackAppearingEvent.Invoke ();
				Navigation.PopAsync ();
			}
		}

		#endregion
    }
}
