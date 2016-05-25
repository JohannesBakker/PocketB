using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Xamarin.Forms;

namespace PocketButler
{
	public class MenuCategory
	{
		public string category_id { get; set; }
		public string category_name { get; set; }
		public string is_subcat_exists { get; set; }
		[DataMember(Name = "subcategories")]
		public List<SubCategory> subcategories { get; set; }
		public string is_menuitem_exist { get; set; }
		[DataMember(Name = "menuitems")]
		public List<MenuItem> menuitems { get; set; }
	}

	public class Settings
	{
		public string is_pickup_enabled { get; set; }
		public string is_tableservice_enabled { get; set; }
		public string is_tabservice_enabled { get; set; }
	}

	public class OrderStatus
	{
		public string is_open { get; set; }
		public string can_place_order { get; set; }
	}

	public class OpeningHours
	{
		public string day { get; set; }
		public string timing { get; set; }
		public string is_closed { get; set; }
	}

	public class RestaurantInfo
	{
		public string id { get; set; }
		public string name { get; set; }
		public string address { get; set; }
		public string description { get; set; }
		public string image { get; set; }
		public string rectangle_image { get; set; }
		public string image_large { get; set; }
		public string phone_no { get; set; }
		public string email { get; set; }
		public string website { get; set; }
		public string lat { get; set; }
		public string lng { get; set; }
		public string distance { get; set; }
		public int supports_promotion_code { get; set; }
		public string is_favourite { get; set; }
		public string is_offline { get; set; }
		public bool is_closed { get; set; }
		public bool is_open_for_order { get; set; }
		public string liquor_license { get; set; }
		[DataMember(Name = "menu_categories")]
		public List<MenuCategory> menu_categories { get; set; }
		[DataMember(Name = "settings")]
		public Settings settings { get; set; }
		[DataMember(Name = "order_status")]
		public OrderStatus order_status { get; set; }
		public string mapofvenue_image { get; set; }
		[DataMember(Name = "opening_hours")]
		public List<OpeningHours> opening_hours { get; set; }
		public string merchant_stripe_publishable_key { get; set; }
		public string pb_stripe_publishable_key { get; set; }
		public float tab_amount { get; set; }

		public void CheckOrderStatus()
		{
			if (order_status != null && String.IsNullOrEmpty (order_status.is_open) == false) {
				if (order_status.is_open.Equals ("1"))
					is_closed = false;
				else
					is_closed = true;
			} else
				is_closed = false;

			if (order_status != null && String.IsNullOrEmpty (order_status.can_place_order) == false) {
				if (order_status.can_place_order.Equals ("1"))
					is_open_for_order = true;
				else
					is_open_for_order = false;
			} else
				is_open_for_order = true;
		}

		public bool CanPlaceOrder()
		{
			if (!is_closed && String.IsNullOrEmpty (is_offline) == false && is_offline.Equals ("0")) {
				if (String.IsNullOrEmpty (settings.is_pickup_enabled) == false &&
				    settings.is_pickup_enabled.Equals ("1"))
					return true;

				if (String.IsNullOrEmpty (settings.is_tableservice_enabled) == false &&
				    settings.is_tableservice_enabled.Equals ("1"))
					return true;
			}

			return false;
		}

		public string VenueStatusText()
		{
			String retText = "";
			if (is_closed)
				retText = "Closed";
			else if (String.IsNullOrEmpty (is_offline) == false && is_offline.Equals ("1"))
				retText = "Offline";
			else if ((String.IsNullOrEmpty (settings.is_pickup_enabled) == false && settings.is_pickup_enabled.Equals ("0")) &&
				(String.IsNullOrEmpty (settings.is_tableservice_enabled) == false && settings.is_tableservice_enabled.Equals ("0")))
				retText = "Offline";

			return retText;
		}

		public Color GetStatusColor(String statusText = "")
		{
			string text = statusText;
			if (String.IsNullOrEmpty(text))
				text = VenueStatusText ();
			if (String.IsNullOrEmpty (text))
				return Color.Transparent;

			if (text.Equals("Offline")) // Offline
				return Color.FromHex ("0xFF803C3C");
			else if (text.Equals("Closed")) // Closed
				return Color.FromHex ("0xFF8A8A8A");

			return Color.Transparent;
		}

		public static Color GetStatusColorFromStr(String statusText = "")
		{
			string text = statusText;
			if (String.IsNullOrEmpty (text))
				return Color.Transparent;

			if (text.Equals("Offline")) // Offline
				return Color.FromHex ("0xFF803C3C");
			else if (text.Equals("Closed")) // Closed
				return Color.FromHex ("0xFF8A8A8A");

			return Color.Transparent;
		}
	}
}

