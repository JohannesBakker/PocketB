using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using PocketButler.Controls;

namespace PocketButler
{
    public class PaymentByTokenPage : BasePage
    {
		#region PRIVATE MEMBERS
		protected AbsoluteLayout UILayout { get; set; }
		protected AbsoluteLayout MainLayout { get; set; }
		bool IsFirstShowing = true;
		#endregion

		public PaymentByTokenPage (Action RefreshEvent)
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

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			if (IsFirstShowing == true) {
				IsFirstShowing = false;
				DoPayment ();
			}
		}

		#region PRIVATE METHODS
		private void BuildUI()
		{
			HideNavigationBar ();

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
				Text = "Pay by Token",
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
			AbsoluteLayout.SetLayoutBounds(ItemTitleLabel, new Rectangle(0.5, 0.42, 0.5, 0.1));
			MainLayout.Children.Add(ItemTitleLabel);

			var TotalOrderAmountLabel = new DILabel {
				Text = "Total Order Amount",
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
				TextColor = Color.White,
				Font = Font.SystemFontOfSize(NamedSize.Medium)
			};
			AbsoluteLayout.SetLayoutFlags(TotalOrderAmountLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(TotalOrderAmountLabel, new Rectangle(0.5, 0.5, 0.5, 0.1));
			MainLayout.Children.Add(TotalOrderAmountLabel);

			var AmountButton = new Button {
				Text = Globals.Config.PaymentInfo.Price,
				TextColor = Color.White,
				BackgroundColor = Color.FromRgb(171, 146, 91),
				Font = Font.SystemFontOfSize(NamedSize.Medium)
			};
			AbsoluteLayout.SetLayoutFlags(AmountButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(AmountButton, new Rectangle(0.5, 0.6, 0.5, 0.07));
			MainLayout.Children.Add(AmountButton);

			var MakePaymentButton = new Button {
				Text = "Pay by Token",
				TextColor = Color.Black,
				BackgroundColor = Color.FromRgb(171, 146, 91),
			};
			AbsoluteLayout.SetLayoutFlags(MakePaymentButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(MakePaymentButton, new Rectangle(0.5, 0.98, 0.8, 0.07));
			MainLayout.Children.Add(MakePaymentButton);
			MakePaymentButton.Clicked += (object sender, EventArgs e) => {
				DoPayment();
			};

			AbsoluteLayout.SetLayoutFlags(MainLayout, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(MainLayout, new Rectangle(0, 0, 1, 1));
			UILayout.Children.Add(MainLayout);

			Content = UILayout;
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
			await Navigation.PushAsync(new ProcessPaymentPage(PageShowingEvent, "", "", 0, 0, "", "", false));
		}
		#endregion
    }
}
