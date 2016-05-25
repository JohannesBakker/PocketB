using System;
using System.Runtime.Serialization;

namespace PocketButler
{
	public class GetRestaurantCategoryRequest : BaseRequest
	{
		[DataMember(Name = "lat")]
		public string lat {get;set;}

		[DataMember(Name = "lng")]
		public string lng {get;set;}

		[DataMember(Name = "user_id")]
		public string user_id {get;set;}

		[DataMember(Name = "user_token")]
		public string user_token {get;set;}
	}

	public class GetRestaurantSearchRequest : BaseRequest
	{
		[DataMember(Name = "lat")]
		public string lat {get;set;}

		[DataMember(Name = "lng")]
		public string lng {get;set;}

		[DataMember(Name = "category_id")]
		public string category_id {get;set;}

		[DataMember(Name = "search")]
		public string search {get;set;}
	}

	public class GetRestaurantNearMeRequest : BaseRequest
	{
		[DataMember(Name = "lat")]
		public string lat {get;set;}

		[DataMember(Name = "lng")]
		public string lng {get;set;}

		[DataMember(Name = "category_id")]
		public string category_id {get;set;}

		[DataMember(Name = "search_text")]
		public string search_text {get;set;}
	}

	public class GetRestaurantDetailRequest : BaseRequest
	{
		[DataMember(Name = "lat")]
		public string lat {get;set;}

		[DataMember(Name = "lng")]
		public string lng {get;set;}

		[DataMember(Name = "user_id")]
		public string user_id {get;set;}

		[DataMember(Name = "restaurant_id")]
		public string restaurant_id {get;set;}

		[DataMember(Name = "user_token")]
		public string user_token {get;set;}
	}

	public class GetRestaurantMenuItemRequest : BaseRequest
	{
		[DataMember(Name = "lat")]
		public string lat {get;set;}

		[DataMember(Name = "lng")]
		public string lng {get;set;}

		[DataMember(Name = "user_id")]
		public string user_id {get;set;}

		[DataMember(Name = "search")]
		public string search {get;set;}

		[DataMember(Name = "user_token")]
		public string user_token {get;set;}
	}

	public class GetRestaurantMenuItemDetailRequest : BaseRequest
	{
		[DataMember(Name = "user_id")]
		public string user_id {get;set;}

		[DataMember(Name = "user_token")]
		public string user_token {get;set;}

		[DataMember(Name = "item_id")]
		public string item_id {get;set;}
	}

	public class GetRestaurantMenuRequest : BaseRequest
	{
		[DataMember(Name = "user_id")]
		public string user_id {get;set;}

		[DataMember(Name = "restaurant_id")]
		public string restaurant_id {get;set;}

		[DataMember(Name = "user_token")]
		public string user_token {get;set;}
	}

	public class GetTableServiceRequest : BaseResult
	{
		[DataMember(Name = "user_id")]
		public string user_id {get;set;}

		[DataMember(Name = "user_token")]
		public string user_token {get;set;}

		[DataMember(Name = "restaurant_id")]
		public string restaurant_id {get;set;}
	}

	public class ValidateItemsRequest : BaseResult
	{
		[DataMember(Name = "user_id")]
		public string user_id {get;set;}

		[DataMember(Name = "user_token")]
		public string user_token {get;set;}

		[DataMember(Name = "restaurant_id")]
		public string restaurant_id {get;set;}

		[DataMember(Name = "item_ids")]
		public string item_ids {get;set;}
	}
}
