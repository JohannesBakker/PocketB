using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PocketButler
{
	public class Category
	{
		public string category_id { get; set; }
		public string category_icon { get; set; }
		public string category_name { get; set; }
		public string category_desctiption { get; set; }
	}

	public class RestaurantCategoryResult
	{
		[DataMember(Name = "result")]
		public RestaurantCategoryItem result {get;set;}
	}
	public class RestaurantCategoryItem : BaseResult
	{
		public List<Category> categories { get; set; }
		public List<UserFavouritesItemData> user_favourites_shortcut { get; set; }
	}
}

