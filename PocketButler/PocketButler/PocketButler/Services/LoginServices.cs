using System;
using PocketButler.Globals;
using ServiceStack.ServiceClient.Web;
using ServiceStack.Text;
using System.Threading.Tasks;
using ServiceStack;
using System.Net;
using System.Collections.Specialized;
using RestSharp;

namespace PocketButler.Services
{
	public class LoginServices : BaseService
	{
		public async static Task<UserInfoResult> AuthenticateUser (String email, String password, String deviceToken, String deviceType, bool cacheData = true)
		{
			try {

				// Run request asynchronously
				UserInfoResult result = await Task.Run (() => {

					var request = new UserLoginRequest {
						email = email,
						password = password,
						device_token = deviceToken,
						device_type = deviceType
					};

					var url = Config.ApiUrl + "/" + Config.API_USER_LOGIN;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "email", request.email },
						{ "password", request.password },
						{ "device_token", request.device_token },
						{ "device_type", request.device_type }
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var response = parseResponse.FromJson<UserInfoResult> ();

					return response;
					// Post request to server and then convert response to object from JSON returned
					//return url.PostToUrl (request).FromJson<UserInfoResult> ();

				});

				return result;

			} catch (Exception e) {
				// Track in Google Analytics
//				GoogleAnalyticsTracker.TrackExceptionInGoogleAnalytics (e, false);

				Console.WriteLine (e);
				var response = new UserInfoResult {
					result = new UserInfoItemResult {
						status = "error",
						message = AppText.CommunicationError,
					}
				};

				return response;

			}
		}

		public async static Task<BaseResult> CheckUserName (String email, String password, String deviceToken, String deviceType, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				SimpleResult result = await Task.Run (() => {

					var request = new UserLoginRequest {
						email = email,
						password = password,
						device_token = deviceToken,
						device_type = deviceType
					};

					var url = Config.ApiUrl + "/" + Config.API_CHECK_USERNAME;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "email", request.email },
						{ "password", request.password },
						{ "device_token", request.device_token },
						{ "device_type", request.device_type }
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var response = parseResponse.FromJson<SimpleResult> ();

					return response;
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

		public async static Task<BaseResult> CheckPassword (String userId, String userToken, String password, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				SimpleResult result = await Task.Run (() => {

					var request = new CheckPasswordRequest {
						user_id = userId,
						user_token = userToken,
						password = password,
					};

					var url = Config.ApiUrl + "/" + Config.API_CHECK_PASSWORD;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_token", request.user_token },
						{ "password", request.password }
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var response = parseResponse.FromJson<SimpleResult> ();

					return response;
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

		public async static Task<BaseResult> ChangePassword (String userId, String userToken, String newpassword, String oldpassword, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				SimpleResult result = await Task.Run (() => {

					var request = new ChangePasswordRequest {
						user_id = userId,
						user_token = userToken,
						new_password = newpassword,
						old_password = oldpassword,
					};

					var url = Config.ApiUrl + "/" + Config.API_CHANGE_PASSWORD;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_token", request.user_token },
						{ "new_password", request.new_password },
						{ "old_password", request.old_password }
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var response = parseResponse.FromJson<SimpleResult> ();

					return response;

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

		public async static Task<UserInfoResult> RegisterUser (String email, String password, String firstName, String lastName, String dob, String mobileNumber, String deviceToken, String deviceType, String profileImage, bool cacheData = true)
		{
			try {

				// Run request asynchronously
				UserInfoResult result = await Task.Run (() => {

					var request = new RegisterUserRequest {
						email = email,
						password = password,
						firstname = firstName,
						lastname = lastName,
						age = dob,
						mobile = mobileNumber,
						device_token = deviceToken,
						device_type = deviceType
					};

					var url = Config.ApiUrl + "/" + Config.API_REGISTRATION;
					/*
					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "email", request.email },
						{ "password", request.password },
						{ "first_name", request.firstname },
						{ "last_name", request.lastname },
						{ "dob", request.dob },
						{ "device_token", request.device_token },
						{ "device_type", request.device_type }
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var response = parseResponse.FromJson<UserInfoResult> ();*/

					RestClient client = new RestClient(url);
					var restRequest = new RestRequest(Method.POST);
					restRequest.AlwaysMultipartFormData = true;
					//restRequest.RequestFormat = DataFormat.Json;
					restRequest.Timeout = 60 * 10 * 1000;

					restRequest.AddHeader("Content-Type", "multipart/form-data");
					restRequest.AddHeader(Globals.Config.AccessKeyName, Globals.Config.AccessKey);

					restRequest.AddParameter("email", request.email);
					restRequest.AddParameter("password", request.password);
					restRequest.AddParameter("first_name", request.firstname);
					restRequest.AddParameter("last_name", request.lastname);
					restRequest.AddParameter("age", request.age);
					restRequest.AddParameter("mobile", request.mobile);
					restRequest.AddParameter("device_token", request.device_token);
					restRequest.AddParameter("device_type", request.device_type);

					//if (imageData != null)
					//	restRequest.AddFile("userfile", imageData, "profilepic.png", "image/png");
					if (String.IsNullOrEmpty(profileImage) == false)
						restRequest.AddFile("userfile", profileImage);

					var response = client.Execute(restRequest);
					return response.Content.FromJson<UserInfoResult>();

					//return response;

				});

				return result;

			} catch (Exception e) {
				// Track in Google Analytics
				//				GoogleAnalyticsTracker.TrackExceptionInGoogleAnalytics (e, false);

				Console.WriteLine (e);
				var response = new UserInfoResult {
					result = new UserInfoItemResult {
						status = "error",
						message = AppText.CommunicationError,
					}
				};

				return response;

			}
		}

		public async static Task<BaseResult> Logout (String userid, String userToken, String deviceToken, bool cacheData = true)
		{
			try {

				// Run request asynchronously
				SimpleResult result = await Task.Run (() => {

					var request = new LogoutRequest {
						user_id = userid,
						user_token = userToken,
						device_token = deviceToken,
					};

					var url = Config.ApiUrl + "/" + Config.API_LOGOUT;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_token", request.user_token },
						{ "device_token", request.device_token }
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var response = parseResponse.FromJson<SimpleResult> ();

					return response;
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

		public async static Task<BaseResult> VerifyUser (String userid, String verifycode, bool cacheData = true)
		{
			try {

				// Run request asynchronously
				SimpleResult result = await Task.Run (() => {

					var request = new VerifyCodeRequest {
						user_id = userid,
						verification_code = verifycode,
					};

					var url = Config.ApiUrl + "/" + Config.API_VERIFYCODE;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "verification_code", request.verification_code }
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var response = parseResponse.FromJson<SimpleResult> ();

					return response;
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
	}
}

