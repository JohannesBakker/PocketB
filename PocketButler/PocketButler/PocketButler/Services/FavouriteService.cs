using System;
using System.Threading.Tasks;
using PocketButler.Globals;
using ServiceStack.ServiceClient.Web;
using ServiceStack.Text;
using ServiceStack;
using System.Net;
using System.Collections.Specialized;

namespace PocketButler.Services
{
	public class FavouriteService : BaseService
	{
		public async static Task<BaseResult> SetFavouriteRestaurant (String userId, String userToken, String restaurantid, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				SimpleResult result = await Task.Run (() => {

					var request = new FavouriteRestaurantRequest {
						user_id = userId,
						user_token = userToken,
						restaurant_id = restaurantid,
					};

					var url = Config.ApiUrl + "/" + Config.API_SET_FAVOURITES_RESTAURENT;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_token", request.user_token },
						{ "restaurant_id", request.restaurant_id }
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

		public async static Task<BaseResult> SetFavouriteRestaurantItem (String userId, String userToken, String restaurantid, String itemid, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				SimpleResult result = await Task.Run (() => {

					var request = new FavouriteRestaurantItemRequest {
						user_id = userId,
						user_token = userToken,
						restaurant_id = restaurantid,
						item_id = itemid,
					};

					var url = Config.ApiUrl + "/" + Config.API_SET_FAVOURITES_RESTAURENT_ITEM;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_token", request.user_token },
						{ "restaurant_id", request.restaurant_id },
						{ "item_id", request.item_id }
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

		public async static Task<BaseResult> SetFavouriteRestaurantShortcutItem (String userId, String userToken, String restaurantid, String itemid, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				SimpleResult result = await Task.Run (() => {

					var request = new FavouriteRestaurantItemRequest {
						user_id = userId,
						user_token = userToken,
						restaurant_id = restaurantid,
						item_id = itemid,
					};

					var url = Config.ApiUrl + "/" + Config.API_SET_SHORTCUT_RESTAURENT_ITEM;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_token", request.user_token },
						{ "restaurant_id", request.restaurant_id },
						{ "item_id", request.item_id }
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

		public async static Task<UserFavouritesItem> DisplayUserFavourites (String userId, String userToken, String lat, String lng, String restaurantid, String venuePage, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				var request = new FavouriteDisplayRequest {
					user_id = userId,
					user_token = userToken,
					lat = lat,
					lng = lng,
					restaurant_id = restaurantid,
					venue_page = venuePage
				};

				var url = Config.ApiUrl + "/" + Config.API_DISPLAY_USER_FAVOURTIES;

				WebClient client = new WebClient ();

				var requestData = new NameValueCollection() {
					{ "user_id", request.user_id },
					{ "user_token", request.user_token },
					{ "restaurant_id", request.restaurant_id },
					{ "venue_page", request.venue_page },
					{ "lat", request.lat },
					{ "lng", request.lng }
				};

				client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
				var serverResult = client.UploadValues (url, "POST", requestData);
				if (serverResult != null)
				{
					var parseResponse = serverResult.AsString();
					var response = parseResponse.FromJson<UserFavouritesResult> ();

					return response.result;
				}
				else
				{
					return new UserFavouritesItem(){
						status = "error",
						message = AppText.CommunicationError,
					};
				}
			}
			catch (Exception ex)
			{
			}

			return new UserFavouritesItem(){
				status = "error",
				message = AppText.CommunicationError,
			};
		}

		public async static Task<FavouritesRestaurantItem> DisplayFavouritesRestaurant (String userId, String userToken, String lat, String lng, String restaurantid, String venuePage, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				FavouritesRestaurantResult result = await Task.Run (() => {

					var request = new FavouriteDisplayRequest {
						user_id = userId,
						user_token = userToken,
						lat = lat,
						lng = lng,
						restaurant_id = restaurantid,
						venue_page = venuePage
					};

					var url = Config.ApiUrl + "/" + Config.API_DISPLAY_FAVORITES_RESTAURANT;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_token", request.user_token },
						{ "restaurant_id", request.restaurant_id },
						{ "venue_page", request.venue_page },
						{ "lat", request.lat },
						{ "lng", request.lng }
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var response = parseResponse.FromJson<FavouritesRestaurantResult> ();

					return response;
					// Post request to server and then convert response to object from JSON returned
					//return url.PostToUrl (request).FromJson<FavouritesRestaurantResult> ();
				});

				return result.result;

			} catch (Exception e) {
				// Track in Google Analytics
				//				GoogleAnalyticsTracker.TrackExceptionInGoogleAnalytics (e, false);

				Console.WriteLine (e);
				var response = new FavouritesRestaurantItem {
					status = "error",
					message = AppText.CommunicationError,
				};

				return response;
			}
		}
	}
}

