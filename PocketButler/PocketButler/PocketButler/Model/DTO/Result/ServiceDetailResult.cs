using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PocketButler
{
	public class Servicepoint
	{
		public string name { get; set; }
		public string start_no { get; set; }
		public string end_no { get; set; }
	}

	public class ServicepointDetails
	{
		public string delievery_charges { get; set; }
		public List<Servicepoint> servicepoints { get; set; }
	}

	public class ServiceDetailResultItem : BaseResult
	{
		public string tab_amount { get; set; }
		public ServicepointDetails servicepoint_details { get; set; }
		public Userinfo user_details { get; set; }
		public string subscription_id { get; set; }
	}

	public class ServiceDetailResult
	{
		[DataMember(Name = "result")]
		public ServiceDetailResultItem result { get; set; }
	}

	public class ValidateItemResult
	{
		[DataMember(Name = "result")]
		public BaseResult result { get; set; }
	}

	public class RedeemResult : BaseResult
	{
		public double discountedAmount { get; set; }
	}

	public class RedeemItemResult
	{
		[DataMember(Name = "result")]
		public RedeemResult result { get; set; }
	}
}

