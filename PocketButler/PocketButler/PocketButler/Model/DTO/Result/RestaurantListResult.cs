using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PocketButler
{
	public class RestaurantListResult
	{
		[DataMember(Name = "result")]
		public RestaurantListItem result {get;set;}
	}

	public class RestaurantListItem : BaseResult
	{
		public List<RestaurantInfo> restaurants_list { get; set; }
		public int total_pages { get; set; }
	}
}

