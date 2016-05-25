using System;
using System.Runtime.Serialization;

namespace PocketButler
{
	public class OrderInfo
	{
		public string order_id { get; set; }
	}

	public class PaymentResult
	{
		[DataMember(Name = "result")]
		public PaymentResultItem result { get; set; }
	}

	public class PaymentResultItem : BaseResult
	{
		[DataMember(Name = "order_info")]
		public OrderInfo order_info { get; set; }
	}

	public class StripeTokenResult
	{
		public string id { get; set; }
		public bool livemode { get; set; }
		public long created { get; set; }
		public bool used { get; set; }
		public string type { get; set; }

		public CardObject card { get; set; }
	}

	public class CardObject
	{
		public string id { get; set; }
		public string last4 { get; set; }
		public string brand { get; set; }
		public string funding { get; set; }
		public int exp_month { get; set; }
		public int exp_year { get; set; }
		public string fingerprint { get; set; }
		public string country { get; set; }
		public string name { get; set; }
		public string address_line1 { get; set; }
		public string address_line2 { get; set; }
		public string address_city { get; set; }
		public string address_state { get; set; }
		public string address_zip { get; set; }
		public string address_country { get; set; }
		public string dynamic_last4 { get; set; }
		public string customer { get; set; }
	}
}

