using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using PocketButler.Globals;
using ServiceStack.ServiceClient.Web;
using ServiceStack.Text;
using System.Net;
using System.Collections.Specialized;

namespace PocketButler
{
	public class VenueService : BaseService
	{
		public async static Task<RestaurantCategoryResult> GetRestaurantCategory (String latitude, String longitude, String userid, String usertoken, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				RestaurantCategoryResult result = await Task.Run (() => {

					var request = new GetRestaurantCategoryRequest {
						lat = latitude,
						lng = longitude,
						user_id = userid,
						user_token = usertoken,
					};

					var url = Config.ApiUrl + "/" + Config.API_GET_RESTAURENT_CATEGORY;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_token", request.user_token },
						{ "lat", request.lat },
						{ "lng", request.lng }
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var response = parseResponse.FromJson<RestaurantCategoryResult> ();

					return response;
				});

				return result;

			} catch (Exception e) {
				// Track in Google Analytics
				//				GoogleAnalyticsTracker.TrackExceptionInGoogleAnalytics (e, false);

				Console.WriteLine (e);
				var response = new RestaurantCategoryResult {
					result = new RestaurantCategoryItem{
						status = "error",
						message = AppText.CommunicationError,
					}
				};

				return response;
			}
		}

		public async static Task<RestaurantListResult> GetRestaurantSearchByCoordinate (String latitude, String longitude, String categoryid, String searchkey, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				RestaurantListResult result = await Task.Run (() => {

					var request = new GetRestaurantSearchRequest {
						lat = latitude,
						lng = longitude,
						category_id = categoryid,
						search = searchkey,
					};

					var url = Config.ApiUrl + "/" + Config.API_GET_RESTAURENT_BY_COORDINATE;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "category_id", request.category_id },
						{ "search", request.search },
						{ "lat", request.lat },
						{ "lng", request.lng }
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var response = parseResponse.FromJson<RestaurantListResult> ();

					return response;
					// Post request to server and then convert response to object from JSON returned
					//return url.PostToUrl (request).FromJson<RestaurantListResult> ();

				});

				return result;

			} catch (Exception e) {
				// Track in Google Analytics
				//				GoogleAnalyticsTracker.TrackExceptionInGoogleAnalytics (e, false);

				Console.WriteLine (e);
				var response = new RestaurantListResult {
					result = new RestaurantListItem{
						status = "error",
						message = AppText.CommunicationError,
					}
				};

				return response;
			}
		}

		public async static Task<RestaurantDetailResult> GetRestaurantDetails (String userid, String restaurantid, String latitude, String longitude, String usertoken, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				RestaurantDetailResult result = await Task.Run (() => {

					var request = new GetRestaurantDetailRequest {
						lat = latitude,
						lng = longitude,
						user_id = userid,
						user_token = usertoken,
						restaurant_id = restaurantid
					};

					var url = Config.ApiUrl + "/" + Config.API_GET_RESTAURENT_DETAILS;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_token", request.user_token },
						{ "restaurant_id", request.restaurant_id },
						{ "lat", request.lat },
						{ "lng", request.lng }
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var response = parseResponse.FromJson<RestaurantDetailResult> ();

					return response;
					// Post request to server and then convert response to object from JSON returned
					//return url.PostToUrl (request).FromJson<RestaurantDetailResult> ();

				});

				return result;

			} catch (Exception e) {
				// Track in Google Analytics
				//				GoogleAnalyticsTracker.TrackExceptionInGoogleAnalytics (e, false);

				Console.WriteLine (e);
				var response = new RestaurantDetailResult {
					result = new RestaurantDetailItem{
						status = "error",
						message = AppText.CommunicationError,
					}
				};

				return response;
			}
		}

		public async static Task<RestaurantMenuDetailResult> GetRestaurantMenuItem (String latitude, String longitude, String userid, String usertoken, String searchkey, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				RestaurantMenuDetailResult result = await Task.Run (() => {

					var request = new GetRestaurantMenuItemRequest {
						lat = latitude,
						lng = longitude,
						user_id = userid,
						user_token = usertoken,
						search = searchkey
					};

					var url = Config.ApiUrl + "/" + Config.API_GET_RESTAURENT_MENUITEM_BY_COORDINATE;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_token", request.user_token },
						{ "search", request.search },
						{ "lat", request.lat },
						{ "lng", request.lng }
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var response = parseResponse.FromJson<RestaurantMenuDetailResult> ();

					return response;
					// Post request to server and then convert response to object from JSON returned
					//return url.PostToUrl (request).FromJson<RestaurantMenuDetailResult> ();

				});

				return result;

			} catch (Exception e) {
				// Track in Google Analytics
				//				GoogleAnalyticsTracker.TrackExceptionInGoogleAnalytics (e, false);

				Console.WriteLine (e);
				var response = new RestaurantMenuDetailResult {
					result = new RestaurantMenuDetailItem{
						status = "error",
						message = AppText.CommunicationError,
					}
				};

				return response;
			}
		}

		public async static Task<RestaurantMenuItemDetailResult> GetRestaurantMenuItemDetail (String userid, String usertoken, String itemid, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				RestaurantMenuItemDetailResult result = await Task.Run (() => {

					var request = new GetRestaurantMenuItemDetailRequest {
						user_id = userid,
						user_token = usertoken,
						item_id = itemid
					};

					var url = Config.ApiUrl + "/" + Config.API_GET_RESTAURENT_MENUITEM_DETAIL;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_token", request.user_token },
						{ "item_id", request.item_id }
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var response = parseResponse.FromJson<RestaurantMenuItemDetailResult> ();

					return response;
					// Post request to server and then convert response to object from JSON returned
					//return url.PostToUrl (request).FromJson<RestaurantMenuItemDetailResult> ();

				});

				return result;

			} catch (Exception e) {
				// Track in Google Analytics
				//				GoogleAnalyticsTracker.TrackExceptionInGoogleAnalytics (e, false);

				Console.WriteLine (e);
				var response = new RestaurantMenuItemDetailResult {
					result = new RestaurantMenuItemDetailItem{
						status = "error",
						message = AppText.CommunicationError,
					}
				};

				return response;
			}
		}

		public async static Task<RestaurantMenuResult> GetRestaurantMenus (String userid, String usertoken, String restaurantid, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				RestaurantMenuResult result = await Task.Run (() => {

					var request = new GetRestaurantMenuRequest {
						user_id = userid,
						user_token = usertoken,
						restaurant_id = restaurantid
					};

					var url = Config.ApiUrl + "/" + Config.API_GET_RESTAURENT_MENU;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_token", request.user_token },
						{ "restaurant_id", request.restaurant_id }
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var response = parseResponse.FromJson<RestaurantMenuResult> ();

					return response;
					// Post request to server and then convert response to object from JSON returned
					//return url.PostToUrl (request).FromJson<RestaurantMenuResult> ();

				});

				return result;

			} catch (Exception e) {
				// Track in Google Analytics
				//				GoogleAnalyticsTracker.TrackExceptionInGoogleAnalytics (e, false);

				Console.WriteLine (e);
				var response = new RestaurantMenuResult {
					result = new RestaurantMenuMainResult{
						status = "error",
						message = AppText.CommunicationError,
					}
				};

				return response;
			}
		}

		public async static Task<RestaurantListResult> GetRestaurantNearMe (String lat, String lon, String categoryid, String searchkey, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				RestaurantListResult result = await Task.Run (() => {

					var request = new GetRestaurantNearMeRequest {
						lat = lat,
						lng = lon,
						category_id = categoryid,
						search_text = searchkey
					};

					var url = Config.ApiUrl + "/" + Config.API_GET_RESTAURENT_NEARME;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "category_id", request.category_id },
						{ "search_text", request.search_text },
						{ "lat", request.lat },
						{ "lng", request.lng }
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var response = parseResponse.FromJson<RestaurantListResult> ();

					return response;
					// Post request to server and then convert response to object from JSON returned
					//return url.PostToUrl (request).FromJson<RestaurantListResult> ();
				});

				return result;

			} catch (Exception e) {
				// Track in Google Analytics
				//				GoogleAnalyticsTracker.TrackExceptionInGoogleAnalytics (e, false);

				Console.WriteLine (e);
				var response = new RestaurantListResult {
					result = new RestaurantListItem{
						status = "error",
						message = AppText.CommunicationError,
					}
				};

				return response;
			}
		}

		public async static Task<ServiceDetailResult> GetServiceDetails (String userid, String usertoken, String restaurantid, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				ServiceDetailResult result = await Task.Run (() => {

					var request = new GetTableServiceRequest {
						user_id = userid,
						user_token = usertoken,
						restaurant_id = restaurantid
					};

					var url = Config.ApiUrl + "/" + Config.API_GET_SERVICE_DETAILS;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_token", request.user_token },
						{ "restaurant_id", request.restaurant_id }
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var response = parseResponse.FromJson<ServiceDetailResult> ();

					return response;
					// Post request to server and then convert response to object from JSON returned
					//return url.PostToUrl (request).FromJson<ServiceDetailResult> ();
				});

				return result;

			} catch (Exception e) {
				// Track in Google Analytics
				//				GoogleAnalyticsTracker.TrackExceptionInGoogleAnalytics (e, false);

				Console.WriteLine (e);
				var response = new ServiceDetailResult {
					result = new ServiceDetailResultItem{
						status = "error",
						message = AppText.CommunicationError,
					}
				};

				return response;
			}
		}

		public async static Task<ValidateItemResult> ValidateItems (String userid, String usertoken, String restaurantid, String itemids, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				ValidateItemResult result = await Task.Run (() => {

					var request = new ValidateItemsRequest {
						user_id = userid,
						user_token = usertoken,
						restaurant_id = restaurantid,
						item_ids = itemids,
					};

					var url = Config.ApiUrl + "/" + Config.API_CHECK_VALIDATE_ITEMS;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_token", request.user_token },
						{ "restaurant_id", request.restaurant_id },
						{ "item_ids", request.item_ids }
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var response = parseResponse.FromJson<ValidateItemResult> ();

					return response;
				});

				return result;

			} catch (Exception e) {
				Console.WriteLine (e);
				var response = new ValidateItemResult {
					result = new BaseResult{
						status = "error",
						message = AppText.CommunicationError,
					}
				};

				return response;
			}
		}

		public async static Task<RedeemItemResult> CalculateRedeemAmountForVenue (String userid, String usertoken, String restaurantid, String promotioncode, String totalamount, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				RedeemItemResult result = await Task.Run (() => {
					var url = Config.ApiUrl + "/" + Config.API_CHECK_REDEEM;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", userid },
						{ "user_token", usertoken },
						{ "promotion_code", promotioncode },
						{ "total_amount", totalamount },
						{ "restaurant_id", restaurantid },
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var response = parseResponse.FromJson<RedeemItemResult> ();

					return response;
				});

				return result;

			} catch (Exception e) {
				Console.WriteLine (e);
				var response = new RedeemItemResult {
					result = new RedeemResult{
						status = "error",
						message = AppText.CommunicationError,
					}
				};

				return response;
			}
		}
	}
}

