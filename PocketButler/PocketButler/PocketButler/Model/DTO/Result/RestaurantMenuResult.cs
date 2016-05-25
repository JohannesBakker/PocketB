using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ServiceStack.Text;

namespace PocketButler
{
	public class MenuTypeItem
	{
		public string id { get; set; }
		public string name { get; set; }
		public string unit_price { get; set; }
		public string currency_symbol { get; set; }
		public string currency_symbol_position_is_right { get; set; }
		public string is_default { get; set; }
	}

	public class MenuTypes
	{
		[DataMember(Name = "Select one")]
		public List<MenuTypeItem> Select_one;
	}

	public class MenuExtraItem
	{
		public string id { get; set; }
		public string name { get; set; }
		public string unit_price { get; set; }
		public string currency_symbol { get; set; }
		public string currency_symbol_position_is_right { get; set; }
		public string is_default { get; set; }
	}

	public class MenuItem
	{
		public string item_id { get; set; }
		public string item_image { get; set; }
		public string item_name { get; set; }
		public string item_price { get; set; }
		public string currency_symbol { get; set; }
		public string currency_symbol_position_is_right { get; set; }
		public string is_favourite { get; set; }
		public string is_popular { get; set; }
		public string item_description { get; set; }
		public string item_image_large { get; set; }
		//public List<MenuTypes> types { get; set; }
		public JsonArrayObjects types { get; set; }
		public List<MenuExtraItem> extras { get; set; }
		public List<MenuItem> related_items { get; set; }
		public RestaurantInfo restaurant_info { get; set; }
		public bool IsPopularCategory { get; set; }
		public string Category_Id { get; set; }
		public int item_same_count { get; set; }
	}

	public class SubCategory
	{
		public string sub_category_id { get; set; }
		public string sub_category_name { get; set; }
		public List<MenuItem> menuitems { get; set; }
	}

	public class RestaurantsMenu
	{
		public string category_id { get; set; }
		public string category_name { get; set; }
		public List<SubCategory> sub_category { get; set; }
	}

	public class RestaurantMenuMainResult : BaseResult
	{
		public List<RestaurantsMenu> restaurants_menu { get; set; }
	}

	public class RestaurantMenuResult
	{
		[DataMember(Name = "result")]
		public RestaurantMenuMainResult result { get; set; }
	}
}

