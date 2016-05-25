using System;
using System.Runtime.Serialization;

namespace PocketButler
{
	public class UserInfoResult
	{
		[DataMember(Name = "result")]
		public UserInfoItemResult result {get;set;}
	}

	public class UserInfoItemResult : BaseResult
	{
		public Userinfo userinfo { get; set; }
	}
}
