using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Net;
using System.Collections.Specialized;
using ServiceStack.Text;

namespace PocketButler
{
	public class OrderDetailResult
	{
		[DataMember(Name="result")]
		public OrderDetailsResult result { get; set;}
	}

	public class OrderDetailInfo{
		public string delievery_type { get; set;}
		public string delieverycharge { get; set; }
		public string order_id { get; set; }
		public string order_ref_id { get; set; }
		public string orderdate { get; set; }
		public string ordernetamount { get; set; }
		public string orderstatus { get; set; }
		public string ordertotalamount { get; set; }
		public string restaurantname { get; set; }
		public string servicepoint_name { get; set; }
		public string tableno { get; set;}
	}

	public class ItemExtraInfo{
		public string id { get; set;}
		public string name { get; set; }
		public string unit_price { get; set;}
		public string currency_symbol { get; set;}
		public string currency_symbol_position_is_right { get; set; }
		public string is_default { get; set;}
		//public bool is_selected { get; set; }
	}

	public class OrderItem{
		public string item_additional_req { get; set;}
		public List<ItemExtraInfo> item_extras { get; set; }
		public string item_id { get; set; }
		public string item_name { get; set; }
		public string item_price { get; set; }
		public string item_quantity { get; set; }
		public JsonArrayObjects item_type_info { get; set; }
	}

	public class OrderDetails
	{
		public OrderDetailInfo order_info { get; set; }
		public List<OrderItem> order_items { get; set;}
	}

	public class OrderDetailsResult : BaseResult{

		[DataMember(Name="order_details")]		
		public OrderDetails order_details;

	}
}

