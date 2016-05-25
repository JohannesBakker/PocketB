using System;
using PocketButler.Globals;
using ServiceStack.ServiceClient.Web;
using ServiceStack.Text;
using System.Threading.Tasks;
using ServiceStack;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using RestSharp;

namespace PocketButler.Services
{
	public class UserServices : BaseService
	{
		public async static Task<BaseResult> UpdateUserBillingAddress (String userId, String userToken, String streetName, String cityName, String stateName, String postalCode, String countryName, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				SimpleResult result = await Task.Run (() => {

					var request = new UpdateUserBillingRequest {
						user_id = userId,
						user_token = userToken,
						street_name = streetName,
						city_name = cityName,
						state_name = stateName,
						postal_code = postalCode,
						country_name = countryName
					};

					var url = Config.ApiUrl + "/" + Config.API_UPDATE_BILLING_ADDRESS;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_token", request.user_token },
						{ "street_name", request.street_name },
						{ "city_name", request.city_name },
						{ "state_name", request.state_name },
						{ "postal_code", request.postal_code },
						{ "country_name", request.country_name },
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var response = parseResponse.FromJson<SimpleResult> ();

					return response;
					// Post request to server and then convert response to object from JSON returned
					//return url.PostToUrl (request).FromJson<SimpleResult> ();

				});

				return result.result;

			} catch (Exception e) {
				// Track in Google Analytics
//				GoogleAnalyticsTracker.TrackExceptionInGoogleAnalytics (e, false);

				Console.WriteLine (e);
				var response = new BaseResult {
					status = "error",
					message = AppText.CommunicationError,
				};

				return response;
			}
		}

		public async static Task<UserInfoItemResult> UpdateUserProfile (String userId, String userToken, String firstName, String lastName, String birthday, String profileImage, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				UserInfoResult result = await Task.Run (() => {

					var request = new UpdateUserProfileRequest {
						user_id = userId,
						user_token = userToken,
						first_name = firstName,
						last_name = lastName,
						dob = birthday,
					};

					var url = Config.ApiUrl + "/" + Config.API_UPDATE_PROFILE;
					/*
					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_token", request.user_token },
						{ "first_name", request.first_name },
						{ "last_name", request.last_name },
						{ "dob", request.dob }
					};

					NameValueCollection fileData = null;
					if (imageData != null)
					{
						fileData = new NameValueCollection() {
							{ "contentType", "image/png" },
							{ "fileName", "profilepic.png" },
							{ "data", imageData },
							{ "key", "userfile" },
						};
					}

					var requestPostData = new NameValueCollection(){
						{ "postData", requestData },
						{ "fileData", fileData },
					};*/

					RestClient client = new RestClient(url);
					var restRequest = new RestRequest(Method.POST);
					restRequest.AlwaysMultipartFormData = true;
					//restRequest.RequestFormat = DataFormat.Json;
					restRequest.Timeout = 60 * 10 * 1000;

					restRequest.AddHeader("Content-Type", "multipart/form-data");
					restRequest.AddHeader(Globals.Config.AccessKeyName, Globals.Config.AccessKey);

					restRequest.AddParameter("user_id", request.user_id);
					restRequest.AddParameter("user_token", request.user_token);
					restRequest.AddParameter("first_name", request.first_name);
					restRequest.AddParameter("last_name", request.last_name);
					restRequest.AddParameter("dob", request.dob);

					//if (imageData != null)
					//	restRequest.AddFile("userfile", imageData, "profilepic.png", "image/png");
					if (String.IsNullOrEmpty(profileImage) == false)
						restRequest.AddFile("userfile", profileImage);

					var response = client.Execute(restRequest);
					return response.Content.FromJson<UserInfoResult>();
					// Post request to server and then convert response to object from JSON returned
					//return url.PostToUrl (request).FromJson<UserInfoResult> ();

				});

				return result.result;

			} catch (Exception e) {
				// Track in Google Analytics
				//				GoogleAnalyticsTracker.TrackExceptionInGoogleAnalytics (e, false);

				Console.WriteLine (e);
				var response = new UserInfoItemResult {
					status = "error",
					message = AppText.CommunicationError,
				};

				return response;
			}
		}

		private static byte[] Combine( params byte[][] arrays )
		{
			byte[] rv = new byte[ arrays.Sum( a => a.Length ) ];
			int offset = 0;
			foreach ( byte[] array in arrays ) {
				System.Buffer.BlockCopy( array, 0, rv, offset, array.Length );
				offset += array.Length;
			}
			return rv;
		}

		public async static Task<UserInfoItemResult> GetUserProfile (String userId, String userToken, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				UserInfoResult result = await Task.Run (() => {

					var request = new UpdateUserProfileRequest {
						user_id = userId,
						user_token = userToken,
					};

					var url = Config.ApiUrl + "/" + Config.API_GETPROFILE_DETAILS;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_token", request.user_token }
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var response = parseResponse.FromJson<UserInfoResult> ();

					return response;
					// Post request to server and then convert response to object from JSON returned
					//return url.PostToUrl (request).FromJson<UserInfoResult> ();

				});

				return result.result;

			} catch (Exception e) {
				// Track in Google Analytics
				//				GoogleAnalyticsTracker.TrackExceptionInGoogleAnalytics (e, false);

				Console.WriteLine (e);
				var response = new UserInfoItemResult {
					status = "error",
					message = AppText.CommunicationError,
				};

				return response;
			}
		}

		public async static Task<BaseResult> ForgetPassword (String useremail, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				SimpleResult result = await Task.Run (() => {

					var request = new ForgetPasswordRequest {
						email = useremail,
					};

					var url = Config.ApiUrl + "/" + Config.API_FORGET_PASS;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "email", request.email }
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var response = parseResponse.FromJson<SimpleResult> ();

					return response;
					// Post request to server and then convert response to object from JSON returned
					//return url.PostToUrl (request).FromJson<SimpleResult> ();

				});

				return result.result;

			} catch (Exception e) {
				// Track in Google Analytics
				//				GoogleAnalyticsTracker.TrackExceptionInGoogleAnalytics (e, false);

				Console.WriteLine (e);
				var response = new BaseResult {
					status = "error",
					message = AppText.CommunicationError,
				};

				return response;
			}
		}

		public async static Task<BaseResult> UpdateDeviceToken (String userId, String deviceType, String deviceToken, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				BaseResult result = await Task.Run (() => {

					var request = new UpdateTokenRequest {
						user_id = userId,
						device_token = deviceToken,
						device_type = deviceType
					};

					var url = Config.ApiUrl + "/" + Config.API_UPDATE_DEVICE_TOKEN;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "device_token", request.device_token },
						{ "device_type", request.device_type }
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var response = parseResponse.FromJson<BaseResult> ();

					return response;
					// Post request to server and then convert response to object from JSON returned
					//return url.PostToUrl (request).FromJson<BaseResult> ();

				});

				return result;

			} catch (Exception e) {
				// Track in Google Analytics
				//				GoogleAnalyticsTracker.TrackExceptionInGoogleAnalytics (e, false);

				Console.WriteLine (e);
				var response = new BaseResult {
					status = "error",
					message = AppText.CommunicationError,
				};

				return response;
			}
		}

	}
}

