using System;
using System.Threading;
using Xamarin.Forms;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Android.App;
using Android.Content;
using System.Text;
using Com.Mixpanel.Android.Mpmetrics;

namespace PocketButler
{
	public static class ObjectTypeHelper
	{
		public static T Cast<T>(this Java.Lang.Object obj) where T : class
		{
			var propertyInfo = obj.GetType().GetProperty("Instance");
			return propertyInfo == null ? null : propertyInfo.GetValue(obj, null) as T;
		}
	}

	public static class Utils
	{
		#region CONSTANTS
		public static String APP_PREFERENCE_KEY = "PocketButler_Settings";

		public static String API_SUCCESS_MESSAGE = "success";
		public static String ERROR_MSG_NEED_REAUTH = "This api requires authentication";
		public static String ERROR_MSG_RESOURCE_NOT_FOUND = "No resource";
		public static String ERROR_MSG_BAD_REQUEST = "Bad request";
		#endregion

		public static String HIDE_CALENDAR_YEAR = "mYearSpinner";
		public static String HIDE_CALENDAR_MONTH = "mMonthSpinner";
		public static String HIDE_CALENDAR_DAY = "mDaySpinner";

		public enum DeliveryType
		{
			DeliveryTypeNone = 0,
			DeliveryTypePickup = 1,
			DeliveryTypeTableService = 2,
			DeliveryTypeHomeDelivery = 3,
		}

		public enum PaymentMethod
		{
			PaymentMethodUnknown = 0,
			PaymentMethodCreditCard = 1,
			PaymentMethodTab = 2,
			PaymentMethodToken = 3,
		}

		#region UTIL METHODS
		public delegate void ItemClickedDelegate();
		public delegate void CategoryItemClickedDelegate (String category_id, String category_name);
		public delegate void CategorySubItemClickedDelegate (String category_id, String item_id, MenuItem item_data);
		public delegate void CategoryFavItemClickedDelegate (String category_id, String item_id, MenuItem item_data);
		public delegate Task CategoryAddItemClickedDelegate (String category_id, String item_id, MenuItem item_data);

		public delegate void PaymentDeleteClickedDelegate (String restaurant_id, String menu_id, int uid);
		public delegate void PaymentEditItemClickedDelegate (String restaurant_id, String menu_id, int uid);

		public delegate void FavDeleteClickedDelegate (String restaurant_id, String item_id, bool isshortcut, bool isshortcutlistview);

		public static void Sleep (int milliseconds)
		{
			using (EventWaitHandle tmpEvent = new ManualResetEvent (false)) {
				tmpEvent.WaitOne (milliseconds);
			}
		}

		public static string GetRootFolder(string path)
		{
			while (true)
			{
				string temp = Path.GetDirectoryName(path);
				if (String.IsNullOrEmpty(temp))
					break;
				path = temp;
			}
			return path;
		}

		// Save data to preferences
		public static void SaveDataToSettings(String key, String value){

			//store
			var prefs = Android.App.Application.Context.GetSharedPreferences(APP_PREFERENCE_KEY, FileCreationMode.Private);
			var prefEditor = prefs.Edit();
			prefEditor.PutString(key, value);
			prefEditor.Commit();
		}

		// Load data from preferences
		public static String LoadDataFromSettings(String key)
		{
			//retreive 
			var prefs = Android.App.Application.Context.GetSharedPreferences(APP_PREFERENCE_KEY, FileCreationMode.Private);              
			var value = prefs.GetString(key, "");

			return value;
		}
		#endregion

		public static string AsString(this byte[] byteArray)
		{
			return Encoding.UTF8.GetString (byteArray);
		}

		public static byte[] AsBytes(this string stringValue)
		{
			return Encoding.Default.GetBytes (stringValue);
		}

		public static String GetDistanceString(String distance)
		{
			if (String.IsNullOrEmpty (distance))
				return "";

			return distance + "km";
		}

		#region MIXPANEL
		public static void MixPanel_TrackCharge(double p0, Org.Json.JSONObject p1)
		{
			if (Globals.Config.IsMixPanel_Enabled == false)
				return;

			try{
				MixpanelAPI _mixpanel = MixpanelAPI.GetInstance (Forms.Context, Globals.Config.MixPanelKey);
				_mixpanel.People.TrackCharge(p0, p1);
			}
			catch (Exception ex)
			{
			}
		}

		public static void MixPanel_TrackCharge(double p0, string p1)
		{
			if (Globals.Config.IsMixPanel_Enabled == false)
				return;

			try{
				MixPanel_TrackCharge(p0, new Org.Json.JSONObject(p1));
			}
			catch (Exception ex)
			{
			}
		}

		public static void MixPanel_Increment(string p0, double p1)
		{
			if (Globals.Config.IsMixPanel_Enabled == false)
				return;

			try{
				MixpanelAPI _mixpanel = MixpanelAPI.GetInstance (Forms.Context, Globals.Config.MixPanelKey);
				_mixpanel.People.Increment(p0, p1);
			}
			catch (Exception ex)
			{
			}
		}

		public static void MixPanel_Track(string p0, Org.Json.JSONObject p1)
		{
			if (Globals.Config.IsMixPanel_Enabled == false)
				return;

			try{
				MixpanelAPI _mixpanel = MixpanelAPI.GetInstance (Forms.Context, Globals.Config.MixPanelKey);
				_mixpanel.Track(p0, p1);
			}
			catch (Exception ex)
			{
			}
		}

		public static void MixPanel_Track(string p0, string p1)
		{
			if (Globals.Config.IsMixPanel_Enabled == false)
				return;

			try{
				MixPanel_Track(p0, new Org.Json.JSONObject(p1));
			}
			catch (Exception ex)
			{
			}
		}
		#endregion
	}
}

