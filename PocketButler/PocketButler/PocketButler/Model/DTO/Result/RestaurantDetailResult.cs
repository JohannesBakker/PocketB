using System;
using System.Runtime.Serialization;

namespace PocketButler
{
	public class RestaurantDetailResult
	{
		[DataMember(Name = "result")]
		public RestaurantDetailItem result { get; set; }
	}

	public class RestaurantDetailItem : BaseResult
	{
		[DataMember(Name = "restaurant_details")]
		public RestaurantInfo restaurant_details { get; set; }
	}
}

