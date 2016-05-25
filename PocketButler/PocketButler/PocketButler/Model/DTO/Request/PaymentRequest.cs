using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace PocketButler
{
	public class PaymentRequest : BaseRequest
	{
		[DataMember (Name = "user_id")]
		public string user_id { get; set; }

		[DataMember (Name = "user_email")]
		public string user_email { get; set; }

		[DataMember (Name = "user_token")]
		public string user_token { get; set; }

		[DataMember (Name = "pickup_time")]
		public string pickup_time { get; set; }

		[DataMember (Name = "restaurant_id")]
		public string restaurant_id { get; set; }

		[DataMember (Name = "restaurant_name")]
		public string restaurant_name { get; set; }

		[DataMember (Name = "shoppingcart")]
		public string shoppingcart { get; set; }

		[DataMember (Name = "stripe_token")]
		public string stripe_token { get; set; }

		[DataMember (Name = "amount")]
		public string amount { get; set; }
	}

	public class ShoppingCartRequest
	{
		[DataMember (Name = "user_info")]
		public ShoppingCart_UserInfo user_info { get; set; }

		[DataMember (Name = "restaurant_info")]
		public ShoppingCart_RestaurantInfo restaurant_info { get; set; }
	}

	public class ShoppingCart_UserInfo
	{
		[DataMember (Name = "user_email")]
		public string user_email{ get; set; }

		[DataMember (Name = "user_id")]
		public string user_id{ get; set; }

		[DataMember (Name = "user_name")]
		public string user_name{ get; set; }

		[DataMember (Name = "user_token")]
		public string user_token{ get; set; }
	}

	public class ShoppingCart_RestaurantInfo
	{
		[DataMember (Name = "restaurant_name")]
		public string restaurant_name { get; set; }

		[DataMember (Name = "ordered_items")]
		public List<ShoppingCart_OrderItem> ordered_items { get; set; }

		[DataMember (Name = "restaurant_id")]
		public string restaurant_id { get; set; }

		[DataMember (Name = "payment_type")]
		public string payment_type { get; set; }

		[DataMember (Name = "delivery_info")]
		public ShoppingCart_DeliveryInfo delivery_info { get; set; }
	}

	public class ShoppingCart_OrderItem
	{
		[DataMember (Name = "item_quantity")]
		public string item_quantity { get; set; }

		[DataMember (Name = "item_id")]
		public string item_id { get; set; }

		[DataMember (Name = "item_price")]
		public string item_price { get; set; }

		[DataMember (Name = "additional_requirements")]
		public string additional_requirements { get; set; }

		[DataMember (Name = "item_extras")]
		public List<ShoppingCart_ItemExtraInfo> item_extras { get; set; }

		[DataMember (Name = "item_name")]
		public string item_name { get; set; }

		[DataMember (Name = "item_type_info")]
		public List<ShoppingCart_ItemTypeInfo> item_type_info { get; set; }
	}

	public class ShoppingCart_ItemTypeInfo
	{
		[DataMember (Name = "type_id")]
		public string type_id { get; set; }

		[DataMember (Name = "type_name")]
		public string type_name { get; set; }

		[DataMember (Name = "type_price")]
		public string type_price { get; set; }
	}

	public class ShoppingCart_ItemExtraInfo
	{
		[DataMember (Name = "extra_id")]
		public string extra_id { get; set; }

		[DataMember (Name = "extra_name")]
		public string extra_name { get; set; }

		[DataMember (Name = "extra_price")]
		public string extra_price { get; set; }
	}

	public class ShoppingCart_DeliveryInfo
	{
		[DataMember (Name = "delievery_charges")]
		public string delievery_charges { get; set; }

		[DataMember (Name = "area_name")]
		public string area_name { get; set; }

		[DataMember (Name = "table_number")]
		public string table_number { get; set; }

		[DataMember (Name = "delievery_type")]
		public string delievery_type { get; set; }
	}

	public class StripeTokenRequest
	{
		[DataMember (Name = "address_country")]
		public string address_country;

		[DataMember (Name = "address_zip")]
		public string address_zip;

		[DataMember (Name = "cvc")]
		public string cvc;

		[DataMember (Name = "name")]
		public string name;

		[DataMember (Name = "address_line1")]
		public string address_line1;

		[DataMember (Name = "address_state")]
		public string address_state;

		[DataMember (Name = "address_city")]
		public string address_city;

		[DataMember (Name = "exp_month")]
		public string exp_month;

		[DataMember (Name = "exp_year")]
		public string exp_year;

		[DataMember (Name = "number")]
		public string number;
	}

	public class OrderHistoryRequest
	{
		[DataMember (Name="user_id")]
		public string user_id { get; set;}

		[DataMember (Name="user_token")]
		public string user_token { get; set; }

		[DataMember (Name="page")]
		public string page { get; set; }
	}

	public class OrderDetailRequest{
		[DataMember (Name="order_id")]
		public string order_id{ get; set; }

		[DataMember (Name="user_id")]
		public string user_id { get; set;}

		[DataMember (Name="user_token")]
		public string user_token { get; set;}
	}

	public class OrderRemoveRequest
	{
		[DataMember (Name="order_id")]
		public string order_id{ get; set; }

		[DataMember (Name="user_id")]
		public string user_id { get; set;}

		[DataMember (Name="user_token")]
		public string user_token { get; set;}
	}
}