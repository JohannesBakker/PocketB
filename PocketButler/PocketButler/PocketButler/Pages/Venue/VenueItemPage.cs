using System;
using PocketButler.Controls;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace PocketButler
{
	public class VenueItemPage : BasePage
	{
		#region PRIVATE MEMBERS
		protected AbsoluteLayout UILayout { get; set; }
		protected AbsoluteLayout MainLayout { get; set; }

		new DarkIceImage BackgroundImage;
		private String Restaurant_Id = "";
		private RestaurantInfo RestaurantData = null;
		private String Item_Id = "";
		#endregion

		public VenueItemPage (RestaurantInfo info, Action RefreshEvent, String item_id = "")
		{
			BackAppearingEvent = RefreshEvent;
			this.RestaurantData = info;
			this.Item_Id = item_id;

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

		#region PRIVATE METHODS
		async private void LoadDataFromService()
		{
			ShowLoading ();

			var response = await VenueService.GetRestaurantDetails (GetUserID(), Restaurant_Id, App.UserLatitude.ToString(), App.UserLongitude.ToString(), GetUserToken());

			HideLoading ();

			bool isResultSuccess = VenueService.HasSuccessResult (response.result);
			if (isResultSuccess) {
				RestaurantData = response.result.restaurant_details;
				BuildUI ();
			}
			else
				await DisplayAlert ("Failure", response.result.message, "OK");
		}

		private void BuildUI()
		{
			HideNavigationBar ();

			BackgroundImage = new DarkIceImage {
				VerticalOptions = LayoutOptions.Fill,
				HorizontalOptions = LayoutOptions.Fill,
				Source = ImageSource.FromFile ("venue_splash.png"),
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
			/*
			var BackBorderStack = new StackLayout {
				Spacing = 0,
				Padding = 0,
				BackgroundColor = Color.White,
			};
			AbsoluteLayout.SetLayoutFlags(BackBorderStack, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(BackBorderStack, new Rectangle(0.5, 0.08, 0.72, 0.62));
			MainLayout.Children.Add(BackBorderStack);*/

			var ItemImage = new DarkIceImage
			{
				BackgroundColor = Color.White,
				Source = RestaurantData.rectangle_image,//RestaurantData.image,//RestaurantData.image_large,
				Aspect = Aspect.Fill,
				IsShowBorder = true,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
			};
			/*
			var ItemImageStack = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Spacing = 0,
				Padding = 0,
				Children = {
					ItemImage,
				}
			};*/

			AbsoluteLayout.SetLayoutFlags(ItemImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ItemImage, new Rectangle(0.5, 0.28, 0.7, 0.29));
			MainLayout.Children.Add(ItemImage);

			ItemImage.Tapped += ITEMIMAGE_Clicked;

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
			AbsoluteLayout.SetLayoutBounds(ItemLabel, new Rectangle(0.5, 0.55, 0.9, 0.2));
			MainLayout.Children.Add(ItemLabel);

			var BottomBackImage = new DarkIceImage {
				Source = ImageSource.FromFile("venue_splash_bottom_bg.png"),
				Aspect = Aspect.AspectFill,				
			};

			AbsoluteLayout.SetLayoutFlags(BottomBackImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(BottomBackImage, new Rectangle(0, 0.85, 1, 0.31));
			MainLayout.Children.Add(BottomBackImage);

			var PlaceImage = new DarkIceImage
			{
				Source = ImageSource.FromFile ("navbar_places.png"),
				Aspect = Aspect.AspectFit
			};
			AbsoluteLayout.SetLayoutFlags(PlaceImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(PlaceImage, new Rectangle(0.05, 0.65, 0.1, 0.05));
			MainLayout.Children.Add(PlaceImage);

			var PhoneImage = new DarkIceImage
			{
				Source = ImageSource.FromFile ("icon_phone.png"),
				Aspect = Aspect.AspectFit
			};
			AbsoluteLayout.SetLayoutFlags(PhoneImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(PhoneImage, new Rectangle(0.05, 0.73, 0.1, 0.05));
			MainLayout.Children.Add(PhoneImage);

			var HttpImage = new DarkIceImage
			{
				Source = ImageSource.FromFile ("icon_globe.png"),
				Aspect = Aspect.AspectFit
			};
			AbsoluteLayout.SetLayoutFlags(HttpImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(HttpImage, new Rectangle(0.05, 0.81, 0.1, 0.05));
			MainLayout.Children.Add(HttpImage);

			var PlaceUrlLabel = new DILabel {
				Text = RestaurantData.address,
				YAlign = TextAlignment.Center,
				TextColor = Color.White,
				Font = Font.SystemFontOfSize(NamedSize.Medium),
				Lines = 3,
			};
			AbsoluteLayout.SetLayoutFlags(PlaceUrlLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(PlaceUrlLabel, new Rectangle(0.43, 0.66, 0.56, 0.1));
			MainLayout.Children.Add(PlaceUrlLabel);

			PlaceUrlLabel.Tapped += PlaceUrl_Clicked;

			var PhoneUrlLabel = new DILabel {
				Text = RestaurantData.phone_no,
				YAlign = TextAlignment.Center,
				TextColor = Color.White,
				Lines = 3,
				Font = Font.SystemFontOfSize(NamedSize.Medium),
			};
			AbsoluteLayout.SetLayoutFlags(PhoneUrlLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(PhoneUrlLabel, new Rectangle(0.95, 0.73, 0.8, 0.05));
			MainLayout.Children.Add(PhoneUrlLabel);

			PhoneUrlLabel.Tapped += PhoneUrl_Clicked;

			var HttpUrlLabel = new DILabel {
				Text = RestaurantData.website,
				YAlign = TextAlignment.Center,
				TextColor = Color.White,
				Font = Font.SystemFontOfSize(NamedSize.Medium),
				Lines = 3,
				IsHtmlLabel = true,
			};
			AbsoluteLayout.SetLayoutFlags(HttpUrlLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(HttpUrlLabel, new Rectangle(0.95, 0.81, 0.8, 0.05));
			MainLayout.Children.Add(HttpUrlLabel);

			if (!RestaurantData.liquor_license.Equals ("")) {
				var LiquorLicenseLabel = new DILabel {
					Text = "Liquor license no : " + RestaurantData.liquor_license,
					YAlign = TextAlignment.Center,
					TextColor = Color.White,
					Font = Font.SystemFontOfSize(NamedSize.Micro),
					IsHtmlLabel = true,
				};
				AbsoluteLayout.SetLayoutFlags(LiquorLicenseLabel, AbsoluteLayoutFlags.All);
				AbsoluteLayout.SetLayoutBounds(LiquorLicenseLabel, new Rectangle(0.95, 0.86, 0.8, 0.05));
				MainLayout.Children.Add(LiquorLicenseLabel);
			}

			var SeperatorLineStack = new StackLayout {
				Spacing = 0,
				Padding = 0,
				BackgroundColor = Color.FromRgb(171, 146, 91),
			};
			AbsoluteLayout.SetLayoutFlags(SeperatorLineStack, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(SeperatorLineStack, new Rectangle(0, 0.9, 1, 0.005));
			MainLayout.Children.Add(SeperatorLineStack);

			var EnterLabel = new DILabel {
				Text = "Enter",
				YAlign = TextAlignment.Center,
				XAlign = TextAlignment.Center,
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				Font = Font.SystemFontOfSize(NamedSize.Large, FontAttributes.Bold)
			};
			AbsoluteLayout.SetLayoutFlags(EnterLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(EnterLabel, new Rectangle(0, 1, 1, 0.1));
			MainLayout.Children.Add(EnterLabel);

			EnterLabel.Tapped += EnterNextLabel_Clicked;

			var DistanceDescLabel = new DILabel {
				Text = "Distance from current location",
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
				TextColor = Color.White,
				LineBreakMode = LineBreakMode.WordWrap,
				Font = Font.SystemFontOfSize(NamedSize.Micro),
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
			};

			var DistanceLabel = new DILabel {
				Text = Utils.GetDistanceString(RestaurantData.distance),
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
				TextColor = Color.White,
				Font = Font.SystemFontOfSize(NamedSize.Medium),
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
			};

			var DistanceStack = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Spacing = 0,
				Padding = 0,
				BackgroundColor = Color.FromRgb(171, 146, 91),
				Children = {
					DistanceDescLabel,
					DistanceLabel,
				}
			};

			AbsoluteLayout.SetLayoutFlags (DistanceStack, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(DistanceStack, new Rectangle(1, 0.66, 0.24, 0.1));
			MainLayout.Children.Add (DistanceStack);

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

		private void PhoneUrl_Clicked(){
			Device.OpenUri (new Uri ("tel:" + RestaurantData.phone_no));
		}

		private void PlaceUrl_Clicked(){
			if (String.IsNullOrEmpty (RestaurantData.address))
				return;

			String mapLink = "";
			if (App.UserLatitude != 0 && App.UserLongitude != 0)
		    {
				mapLink = String.Format("http://maps.apple.com/?daddr={0}&saddr={1}", String.Format("{0:0.0000},{1:0.0000}", App.UserLatitude, App.UserLongitude), String.Format("{0:0.0000},{1:0.0000}", RestaurantData.lat, RestaurantData.lng));
		    }
		    else
		    {
				mapLink = String.Format("http://maps.apple.com/?q={0}", String.Format("{0:0.0000},{1:0.0000}", RestaurantData.lat, RestaurantData.lng));
		    }

			Device.OpenUri (new Uri(mapLink));
		}

		private void EnterNextLabel_Clicked()
		{
			ShowVenueSliderPage ();
		}

		private void ITEMIMAGE_Clicked()
		{
			ShowVenueSliderPage ();
		}

		async private void ShowVenueSliderPage()
		{
			ShowLoading ();

			var response = await VenueService.GetRestaurantDetails (GetUserID(), RestaurantData.id, App.UserLatitude.ToString(), App.UserLongitude.ToString(), GetUserToken());

			HideLoading ();

			bool isResultSuccess = VenueService.HasSuccessResult (response.result);
			if (isResultSuccess) {
				Globals.Config.CurrentVenue = response.result.restaurant_details;
				Globals.Config.FavoriteItemid = this.Item_Id;

				var RestaurantParam = response.result.restaurant_details;
				App.PageLoaderManager.ShowTypeSliderPage (RestaurantParam);
				//var VenueSliderPage = new VenueTypeSliderPage (RestaurantParam, null);
				//VenueSliderPage.MasterPage = MasterPage;
				//((MainPage)MasterPage).SetDetailPage (VenueSliderPage);


				//Globals.Config.IsPoptoMasterPage = true;
				//Navigation.PopToRootAsync ();
			} else {
				await DisplayAlert ("Failure", response.result.message, "OK");
			}
		}
		#endregion
	}
}

