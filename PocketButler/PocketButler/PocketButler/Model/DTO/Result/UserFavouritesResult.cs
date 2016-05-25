using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PocketButler
{
	public class UserFavouritesItemData
	{
		public string item_id { get; set; }
		public string item_image { get; set; }
		public string item_name { get; set; }
		public string item_price { get; set; }
		public string currency_symbol { get; set; }
		public string currency_symbol_position_is_right { get; set; }
		public string is_favourite { get; set; }
		public string is_popular { get; set; }
		public RestaurantInfo restaurant_info { get; set; }
	}

	public class UserFavouritesItem : BaseResult
	{
		public List<UserFavouritesItemData> user_favourites_shortcut { get; set; }
		public List<UserFavouritesItemData> user_favourites_item { get; set; }
		public List<UserFavouritesItemData> current_restaurant_favourites { get; set; }
		public List<UserFavouritesItemData> favourites_from_this_venue_type { get; set; }
	}

	public class FavouritesRestaurantItem : BaseResult
	{
		public List<RestaurantInfo> user_favourites_restaurants { get; set; }
		public int total_venue_page { get; set; }
	}

	public class UserFavouritesResult
	{
		[DataMember(Name = "result")]
		public UserFavouritesItem result { get; set; }
	}

	public class FavouritesRestaurantResult
	{
		[DataMember(Name = "result")]
		public FavouritesRestaurantItem result { get; set; }
	}
}

