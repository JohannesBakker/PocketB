using System;
using System.Runtime.Serialization;

namespace PocketButler
{
	public class FavouriteRestaurantRequest : BaseRequest
	{
		[DataMember(Name = "user_id")]
		public string user_id {get;set;}

		[DataMember(Name = "user_token")]
		public string user_token {get;set;}

		[DataMember(Name = "restaurant_id")]
		public string restaurant_id {get;set;}
	}

	public class FavouriteRestaurantItemRequest : BaseRequest
	{
		[DataMember(Name = "user_id")]
		public string user_id {get;set;}

		[DataMember(Name = "user_token")]
		public string user_token {get;set;}

		[DataMember(Name = "restaurant_id")]
		public string restaurant_id {get;set;}

		[DataMember(Name = "item_id")]
		public string item_id {get;set;}
	}

	public class FavouriteShortcutItemRequest : BaseRequest
	{
		[DataMember(Name = "user_id")]
		public string user_id {get;set;}

		[DataMember(Name = "user_token")]
		public string user_token {get;set;}

		[DataMember(Name = "restaurant_id")]
		public string restaurant_id {get;set;}

		[DataMember(Name = "item_id")]
		public string item_id {get;set;}
	}

	public class FavouriteDisplayRequest : BaseRequest
	{
		[DataMember(Name = "user_id")]
		public string user_id {get;set;}

		[DataMember(Name = "user_token")]
		public string user_token {get;set;}

		[DataMember(Name = "restaurant_id")]
		public string restaurant_id {get;set;}

		[DataMember(Name = "venue_page")]
		public string venue_page {get;set;}

		[DataMember(Name = "lat")]
		public string lat {get;set;}

		[DataMember(Name = "lng")]
		public string lng {get;set;}
	}
}

