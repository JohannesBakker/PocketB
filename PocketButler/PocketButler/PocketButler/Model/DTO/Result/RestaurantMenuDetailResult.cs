using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PocketButler
{
	public class RestaurantMenuDetailResult
	{
		[DataMember(Name = "result")]
		public RestaurantMenuDetailItem result { get; set; }
	}

	public class RestaurantMenuDetailItem : BaseResult
	{
		public List<MenuItem> restaurants_menu_items { get; set; }
	}

	public class RestaurantMenuItemDetailResult
	{
		[DataMember(Name = "result")]
		public RestaurantMenuItemDetailItem result { get; set; }
	}

	public class RestaurantMenuItemDetailItem : BaseResult
	{
		public MenuItem item_info { get; set; }
	}
}

