using System;
using System.Runtime.Serialization;

namespace PocketButler
{
	public class RegisterUserRequest : BaseRequest
	{
		[DataMember(Name = "email")]
		public string email {get;set;}

		[DataMember(Name = "password")]
		public string password {get;set;}

		[DataMember(Name = "first_name")]
		public string firstname {get;set;}

		[DataMember(Name = "last_name")]
		public string lastname {get;set;}

		[DataMember(Name = "age")]
		public string age {get;set;}

		[DataMember(Name = "mobile")]
		public string mobile {get;set;}

		[DataMember(Name = "device_token")]
		public string device_token {get;set;}

		[DataMember(Name = "device_type")]
		public string device_type {get;set;}

	}

	public class ForgetPasswordRequest : BaseRequest
	{
		[DataMember(Name = "email")]
		public string email {get;set;}
	}

	public class GetUserProfileRequest : BaseRequest
	{
		[DataMember(Name = "user_id")]
		public string user_id {get;set;}

		[DataMember(Name = "user_token")]
		public string user_token {get;set;}
	}

	public class LogoutRequest : BaseRequest
	{
		[DataMember(Name = "user_id")]
		public string user_id {get;set;}

		[DataMember(Name = "user_token")]
		public string user_token {get;set;}

		[DataMember(Name = "device_token")]
		public string device_token {get;set;}
	}

	public class VerifyCodeRequest : BaseRequest
	{
		[DataMember(Name = "user_id")]
		public string user_id {get;set;}

		[DataMember(Name = "verification_code")]
		public string verification_code {get;set;}
	}

	public class ChangePasswordRequest : BaseRequest
	{
		[DataMember(Name = "user_id")]
		public string user_id {get;set;}

		[DataMember(Name = "user_token")]
		public string user_token {get;set;}

		[DataMember(Name = "new_password")]
		public string new_password {get;set;}

		[DataMember(Name = "old_password")]
		public string old_password {get;set;}
	}

	public class CheckPasswordRequest : BaseRequest
	{
		[DataMember(Name = "user_id")]
		public string user_id {get;set;}

		[DataMember(Name = "user_token")]
		public string user_token {get;set;}

		[DataMember(Name = "password")]
		public string password {get;set;}
	}

	public class UserLoginRequest : BaseRequest
	{
		[DataMember(Name = "email")]
		public string email {get;set;}

		[DataMember(Name = "password")]
		public string password {get;set;}

		[DataMember(Name = "device_token")]
		public string device_token {get;set;}

		[DataMember(Name = "device_type")]
		public string device_type {get;set;}

	}

	public class UpdateUserBillingRequest : BaseRequest
	{
		[DataMember(Name = "user_id")]
		public string user_id {get;set;}

		[DataMember(Name = "user_token")]
		public string user_token {get;set;}

		[DataMember(Name = "street_name")]
		public string street_name {get;set;}

		[DataMember(Name = "city_name")]
		public string city_name {get;set;}

		[DataMember(Name = "state_name")]
		public string state_name {get;set;}

		[DataMember(Name = "postal_code")]
		public string postal_code {get;set;}

		[DataMember(Name = "country_name")]
		public string country_name {get;set;}
	}

	public class UpdateUserProfileRequest : BaseRequest
	{
		[DataMember(Name = "user_id")]
		public string user_id {get;set;}

		[DataMember(Name = "user_token")]
		public string user_token {get;set;}

		[DataMember(Name = "first_name")]
		public string first_name {get;set;}

		[DataMember(Name = "last_name")]
		public string last_name {get;set;}

		[DataMember(Name = "dob")]
		public string dob {get;set;}
	}

	public class UpdateTokenRequest : BaseRequest
	{
		[DataMember(Name = "user_id")]
		public string user_id {get;set;}

		[DataMember(Name = "device_type")]
		public string device_type {get;set;}

		[DataMember(Name = "device_token")]
		public string device_token {get;set;}
	}

}

