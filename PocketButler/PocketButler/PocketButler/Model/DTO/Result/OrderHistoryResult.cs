using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PocketButler
{
	public class OrderHistoryListResult
	{
		[DataMember(Name="result")]
		public OrderHistoryResult result { get; set;}
	}

	public class OrderHistory
	{
		public string order_id { get; set; }
		public string order_ref_id { get; set; }
		public string restaurant_id { get; set; }
		public string restaurant_image { get; set; }
		public string restaurant_name { get; set; }
		public string totalamount { get; set; }
		public string orderstatus { get; set; }
		public string orderdate { get; set; }
	}

	public class OrderHistoryResult : BaseResult
	{
		public int total_pages { get; set; }
		public List<OrderHistory> order_list { get; set; }
	}
}

