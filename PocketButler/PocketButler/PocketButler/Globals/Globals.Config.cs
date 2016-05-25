using System;
using Xamarin.Forms;

namespace PocketButler.Globals
{
	public static class Config
	{
		public static string ApiUrl {
			get {
				return BaseUrl + "/pocket_butler_api_v2";
			}
		}

		public static string BaseUrl {
			get {
				return "https://api.pocketbutler.com";
				//return "http://54.252.198.230";
				//return "http://54.79.117.72";
			}
		}

		public static string AccessKey{
			get {
				return "0d68ae6920f104c22b82f9b6d72b2100";
			}
		}

		public static string InsightKey{
			get{
				return "f90a4a6014ba2f0d244efd62669a46b3b4da593c";
			}
		}

		public static string MixPanelKey{
			get{
				return "84a8d058ed827ded6bbbea0057829ed1";
			}
		}

		public static string DATABASE_FILENAME = "temp_localdb_20141212_1046.db";

		public static string AccessKeyName = "pbkey";

		public static readonly string TrackingId = "UA-XXXXXXX";

		public static readonly string FACEBOOK_APP_ID = "499870533457154";
		public static readonly string FACEBOOK_APP_NAME = "PocketButler";
		public static readonly string FACEBOOK_APP_SECRET = "4190eac72279680d8b5dd25d35d6e696";

		//public static readonly string STRIPE_LIVE_KEY = "pk_live_F5Da59AvACsvytUFXICs8YaQ";
		public static readonly string STRIPE_TEST_KEY = "pk_test_htw9tkeQV45xgR8rQTHGttNn";//"pk_test_56MhdYupqBRJO4QJbKMWFPht";

		public static readonly bool IsMixPanel_Enabled = true;

		#region FONT & FONTSIZE
		public static String AppFontName = "";
		public static int TitleHeaderFontSize = 16;
		#endregion

		#region USER & LOGIN INFO
		public static bool IsGuestMode = false;
		public static String USER_EMAIL = "";
		public static Userinfo USER_INFO;
		public static DateTime LOGIN_TIME;
		#endregion

		#region RESTAURANTS & MENU ITEMS
		public static RestaurantInfo CurrentVenue = null;
		public static String FavoriteItemid = "";
		//public static bool IsPoptoMasterPage = false;
		//public static bool IsLoggingOut = false;

		public static String RestaurantId = "";
		public static String RestaurantName = "";
		public static String RestaurantImage = "";
		public static MenuItem MenuForPayment = null;
		public static PaymentInfoViewModel PaymentInfo = null;

		public static String TotalTabAmount = "";
		public static PocketButler.Utils.DeliveryType PaymentType = PocketButler.Utils.DeliveryType.DeliveryTypeNone;
		public static String ShoppingCartJsonData = "";
		public static int PaymentPageType = 0;
		public static String PickupTime = "";

		public static RestaurantInfo SliderPageRestaurant = null;
		public static bool IsSliderPageOut = false;

		public static CustomizationEventClass CustomizeEventHandle = new CustomizationEventClass();
		#endregion

		// Login API
		public static String API_CHECK_USERNAME = "check_username";
		public static String API_USER_LOGIN = "user_login";
		public static String API_UPDATE_BILLING_ADDRESS = "update_user_billing_address";
		public static String API_FORGET_PASS = "forget_pass";
		public static String API_REGISTRATION = "registration";
		public static String API_UPDATE_PROFILE = "updateuserprofile";
		public static String API_GETPROFILE_DETAILS = "getuserprofiledetails";
		public static String API_CHANGE_PASSWORD = "changepassword";
		public static String API_CHECK_PASSWORD = "checkuserpassword";
		public static String API_LOGOUT = "logout";
		public static String API_VERIFYCODE = "verify_user";

		// Venue APIS
		public static String API_GET_RESTAURENT_CATEGORY = "get_restaurant_category";
		public static String API_GET_RESTAURENT_BY_COORDINATE = "get_restaurant_search_by_coordinate";
		public static String API_GET_RESTAURENT_DETAILS = "get_restaurant_details";
		public static String API_GET_RESTAURENT_MENUITEM_BY_COORDINATE = "get_restaurant_menu_item_search_by_coordinate";
		public static String API_GET_RESTAURENT_MENU = "get_restaurant_menu";
		public static String API_GET_RESTAURENT_MENUITEM_DETAIL = "get_restaurant_menu_item_detail";
		public static String API_SET_FAVOURITES_RESTAURENT = "set_favourites_restaurant";
		public static String API_SET_FAVOURITES_RESTAURENT_ITEM = "set_favourites_restaurant_item";
		public static String API_SET_SHORTCUT_RESTAURENT_ITEM = "set_shortcut_restaurant_item";
		public static String API_DISPLAY_USER_FAVOURTIES = "display_user_favourites";
		public static String API_DISPLAY_FAVORITES_RESTAURANT = "display_favourites_restaurant";
		public static String API_GET_RESTAURENT_NEARME = "get_restaurant_nearme";
		public static String API_GET_TABLE_SERVICE_DETAILS = "get_table_service_details";
		public static String API_GET_ORDER_HISTORY = "order_history";
		public static String API_ACTIVE_ORDERS = "active_orders";
		public static String API_ORDER_DETAILS = "order_details";
		public static String API_GET_SERVICE_DETAILS = "get_service_details";
		public static String API_CHECK_VALIDATE_ITEMS = "validate_cart_items";
		public static String API_CHECK_REDEEM = "check_promotion_code";
		public static String API_ORDER_DELETE_USER = "order_delete_user";
		public static String API_UPDATE_SUBSCRIPTION_ID = "update_subscription_id";
		public static String API_UPDATE_DEVICE_TOKEN = "update_device_token";
		public static String API_PROCESS_PAYMENT_VIA_CC = "process_order_payment_via_cc";
		public static String API_PROCESS_PAYMENT_VIA_FIRSTTIME = "process_order_payment_via_firsttime_token";
		public static String API_PROCESS_PAYMENT_VIA_EXISTING = "process_order_payment_via_existing_token";
		public static String API_PROCESS_PAYMENT_VIA_NEWTAB = "process_order_payment_via_newtab";
		public static String API_PROCESS_PAYMENT_VIA_EXISTINGTAB = "process_order_payment_via_existingtab";

		public static String API_STRIPE_TOKEN = "https://api.stripe.com/v1/tokens";

		public static int kStepper_Unit_Value = 50;
		public static String kWebsite_Url = @"http://www.pocketbutler.com/";
		#region GCM Configuration
		public static String GCM_SENDER_ID = "664735194513";
		#endregion

		#region MessageBus Event
		public static String EVENT_MAIN = "MBE_MAIN";
		public static String EVENT_REFRESH_ORDER = "MPE_RO";
		#endregion

		public class CustomizationEventClass : View
		{
			public static readonly BindableProperty CustomizeEventProperty =
				BindableProperty.Create<CustomizationEventClass, bool> (
					p => p.IsCustomizeUpdated, false);

			public bool IsCustomizeUpdated {
				get {
					return (bool)GetValue (CustomizeEventProperty);
				}

				set {
					this.SetValue (CustomizeEventProperty, value);
				}
			}
		}
	}
}

