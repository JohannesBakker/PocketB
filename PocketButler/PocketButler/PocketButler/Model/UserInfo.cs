using System;

namespace PocketButler
{
	public class ContactAddress
	{
		public string street_name { get; set; }
		public string city_name { get; set; }
		public string state_name { get; set; }
		public string postal_code { get; set; }
		public string country_name { get; set; }
	}

	public class Userinfo
	{
		public string user_id { get; set; }
		public string user_token { get; set; }
		public string user_first_name { get; set; }
		public string user_last_name { get; set; }
		public string user_dob { get; set; }
		public int user_age { get; set; }
		public string status { get; set; }
		public string mobile { get; set; }
		public string user_email { get; set; }
		public string user_password { get; set; }
		public string user_image { get; set; }
		public ContactAddress contact_address { get; set; }
	}
}

