using System;
using System.Threading.Tasks;
using PocketButler.Globals;
using ServiceStack.ServiceClient.Web;
using ServiceStack.Text;
using System.Collections.Generic;
using Stripe;
using ServiceStack;
using System.Net;
using System.Collections.Specialized;
using Xamarin;

namespace PocketButler
{
	public class PaymentService : BaseService
	{
		public async static Task<PaymentResult> ProcessOrderPaymentViaCC (String userid, String useremail, String usertoken, String restaurantid, String restaurantname, String shoppingcart, String stripetoken, String amount, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				PaymentResult result = await Task.Run (() => {

					var request = new PaymentRequest {
						user_id = userid,
						user_email = useremail,
						user_token = usertoken,
						restaurant_id = restaurantid,
						restaurant_name = restaurantname,
						shoppingcart = shoppingcart,
						stripe_token = stripetoken,
						amount = amount,
						pickup_time = Globals.Config.PickupTime,
					};

					var url = Config.ApiUrl + "/" + Config.API_PROCESS_PAYMENT_VIA_CC;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_email", request.user_email },
						{ "user_token", request.user_token },
						{ "restaurant_id", request.restaurant_id },
						{ "restaurant_name", request.restaurant_name },
						{ "shoppingcart", request.shoppingcart },
						{ "stripe_token", request.stripe_token },
						{ "amount", request.amount },
						{ "pickup_time", request.pickup_time },
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData);

					var parseJson = parseResponse.AsString ();

					var response = parseJson.FromJson<PaymentResult> ();

					return response;
				});

				return result;

			} catch (Exception e) {
				// Track in Google Analytics
				//				GoogleAnalyticsTracker.TrackExceptionInGoogleAnalytics (e, false);

				Console.WriteLine (e);
				var response = new PaymentResult {
					result = new PaymentResultItem{
						status = "error",
						message = AppText.CommunicationError,
					}
				};

				return response;
			}
		}

		public async static Task<PaymentResult> ProcessOrderPaymentViaFirstTimeToken (String userid, String useremail, String usertoken, String restaurantid, String restaurantname, String shoppingcart, String stripetoken, String amount, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				PaymentResult result = await Task.Run (() => {

					var request = new PaymentRequest {
						user_id = userid,
						user_email = useremail,
						user_token = usertoken,
						restaurant_id = restaurantid,
						restaurant_name = restaurantname,
						shoppingcart = shoppingcart,
						stripe_token = stripetoken,
						amount = amount,
						pickup_time = Globals.Config.PickupTime,						
					};

					// NOTE: For whatever unknown reason, the API that was provided doesn't respond without using
					// the following method of building up the name/value pairs and then processing the data and
					// converting from json etc. This works so best to just leave things as they are unless the
					// api get's an update and works like it should.

					var url = Config.ApiUrl + "/" + Config.API_PROCESS_PAYMENT_VIA_FIRSTTIME;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_email", request.user_email },
						{ "user_token", request.user_token },
						{ "restaurant_id", request.restaurant_id },
						{ "restaurant_name", request.restaurant_name },
						{ "shoppingcart", request.shoppingcart },
						{ "stripe_token", request.stripe_token },
						{ "amount", request.amount },
						{ "pickup_time", request.pickup_time },
					};
					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData);

					var parseJson = parseResponse.AsString ();

					var response = parseJson.FromJson<PaymentResult> ();

					return response;

				});

				return result;

			} catch (Exception e) {
				// Track in Google Analytics
				//				GoogleAnalyticsTracker.TrackExceptionInGoogleAnalytics (e, false);

				Console.WriteLine (e);
				var response = new PaymentResult {
					result = new PaymentResultItem{
						status = "error",
						message = AppText.CommunicationError,
					}
				};

				return response;
			}
		}

		public async static Task<PaymentResult> ProcessOrderPaymentViaExistingToken (String userid, String useremail, String usertoken, String restaurantid, String restaurantname, String shoppingcart, String stripetoken, String amount, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				PaymentResult result = await Task.Run (() => {

					var request = new PaymentRequest {
						user_id = userid,
						user_email = useremail,
						user_token = usertoken,
						restaurant_id = restaurantid,
						restaurant_name = restaurantname,
						shoppingcart = shoppingcart,
						stripe_token = stripetoken,
						amount = amount,
						pickup_time = Globals.Config.PickupTime,
					};

					var url = Config.ApiUrl + "/" + Config.API_PROCESS_PAYMENT_VIA_EXISTING;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_email", request.user_email },
						{ "user_token", request.user_token },
						{ "restaurant_id", request.restaurant_id },
						{ "restaurant_name", request.restaurant_name },
						{ "shoppingcart", request.shoppingcart },
						{ "stripe_token", request.stripe_token },
						{ "amount", request.amount },
						{ "pickup_time", request.pickup_time },
					};
					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData);

					var parseJson = parseResponse.AsString ();

					var response = parseJson.FromJson<PaymentResult> ();

					return response;

				});

				return result;

			} catch (Exception e) {
				// Track in Google Analytics
				//				GoogleAnalyticsTracker.TrackExceptionInGoogleAnalytics (e, false);

				Console.WriteLine (e);
				var response = new PaymentResult {
					result = new PaymentResultItem{
						status = "error",
						message = AppText.CommunicationError,
					}
				};

				return response;
			}
		}

		public async static Task<PaymentResult> ProcessOrderPaymentViaNewTab (String userid, String useremail, String usertoken, String restaurantid, String restaurantname, String shoppingcart, String stripetoken, String amount, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				PaymentResult result = await Task.Run (() => {

					var request = new PaymentRequest {
						user_id = userid,
						user_email = useremail,
						user_token = usertoken,
						restaurant_id = restaurantid,
						restaurant_name = restaurantname,
						shoppingcart = shoppingcart,
						stripe_token = stripetoken,
						amount = amount,
						pickup_time = Globals.Config.PickupTime,
					};

					var url = Config.ApiUrl + "/" + Config.API_PROCESS_PAYMENT_VIA_NEWTAB;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_email", request.user_email },
						{ "user_token", request.user_token },
						{ "restaurant_id", request.restaurant_id },
						{ "restaurant_name", request.restaurant_name },
						{ "shoppingcart", request.shoppingcart },
						{ "stripe_token", request.stripe_token },
						{ "amount", request.amount },
						{ "pickup_time", request.pickup_time },
					};
					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData);

					var parseJson = parseResponse.AsString ();

					var response = parseJson.FromJson<PaymentResult> ();

					return response;

				});

				return result;

			} catch (Exception e) {
				// Track in Google Analytics
				//				GoogleAnalyticsTracker.TrackExceptionInGoogleAnalytics (e, false);

				Console.WriteLine (e);
				var response = new PaymentResult {
					result = new PaymentResultItem{
						status = "error",
						message = AppText.CommunicationError,
					}
				};

				return response;
			}
		}

		public async static Task<PaymentResult> ProcessOrderPaymentViaExistingTab (String userid, String useremail, String usertoken, String restaurantid, String restaurantname, String shoppingcart, String stripetoken, String amount, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				PaymentResult result = await Task.Run (() => {

					var request = new PaymentRequest {
						user_id = userid,
						user_email = useremail,
						user_token = usertoken,
						restaurant_id = restaurantid,
						restaurant_name = restaurantname,
						shoppingcart = shoppingcart,
						stripe_token = stripetoken,
						amount = amount,
						pickup_time = Globals.Config.PickupTime,
					};

					var url = Config.ApiUrl + "/" + Config.API_PROCESS_PAYMENT_VIA_EXISTINGTAB;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_email", request.user_email },
						{ "user_token", request.user_token },
						{ "restaurant_id", request.restaurant_id },
						{ "restaurant_name", request.restaurant_name },
						{ "shoppingcart", request.shoppingcart },
						{ "stripe_token", request.stripe_token },
						{ "amount", request.amount },
						{ "pickup_time", request.pickup_time },
					};
					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData);

					var parseJson = parseResponse.AsString ();

					var response = parseJson.FromJson<PaymentResult> ();

					return response;

				});

				return result;

			} catch (Exception e) {
				// Track in Google Analytics
				//				GoogleAnalyticsTracker.TrackExceptionInGoogleAnalytics (e, false);

				Console.WriteLine (e);
				var response = new PaymentResult {
					result = new PaymentResultItem{
						status = "error",
						message = AppText.CommunicationError,
					}
				};

				return response;
			}
		}

		public static String GetShoppingCart(String restaurantName, String restaurantId, List<PocketButler.ProcessPaymentPage.Menu_Table_Item> menu_item_list, String tableNumber, String pickupTime, String deliveryType, String deliveryCharges, String servicePointName, String paymentType)
		{
			ShoppingCartRequest request = new ShoppingCartRequest();
			request.user_info = new ShoppingCart_UserInfo ();
			request.restaurant_info = new ShoppingCart_RestaurantInfo ();
			//request.pickup_time = pickupTime;

			request.user_info.user_id = Globals.Config.USER_INFO.user_id;
			request.user_info.user_email = Globals.Config.USER_INFO.user_email;
			request.user_info.user_name = Globals.Config.USER_INFO.user_first_name + " " + Globals.Config.USER_INFO.user_last_name;
			request.user_info.user_token = Globals.Config.USER_INFO.user_token;

			request.restaurant_info.delivery_info = new ShoppingCart_DeliveryInfo ();
			request.restaurant_info.delivery_info.area_name = servicePointName;
			request.restaurant_info.delivery_info.delievery_charges = deliveryCharges;
			request.restaurant_info.delivery_info.delievery_type = deliveryType;
			request.restaurant_info.delivery_info.table_number = tableNumber;

			request.restaurant_info.ordered_items = new List<ShoppingCart_OrderItem> ();
			for (int i = 0; i < menu_item_list.Count; i++) {
				ShoppingCart_OrderItem orderOne = new ShoppingCart_OrderItem ();
				orderOne.additional_requirements = menu_item_list[i].Customization;
				orderOne.item_extras = menu_item_list[i].Extras;
				orderOne.item_id = menu_item_list[i].Id;
				orderOne.item_name = menu_item_list[i].Name;
				orderOne.item_price = menu_item_list[i].Price;
				orderOne.item_quantity = "" + menu_item_list[i].Count;
				orderOne.item_type_info = menu_item_list[i].Types;
				request.restaurant_info.ordered_items.Add (orderOne);
			}

			request.restaurant_info.payment_type = paymentType;
			request.restaurant_info.restaurant_id = restaurantId;
			request.restaurant_info.restaurant_name = restaurantName;

			return request.ToJson<ShoppingCartRequest> ();
		}

		public async static Task<String> GetStripeToken (String stripeKey, String address_country, String address_zip, String cvc, String name, String address_line1, String address_state, String address_city, int exp_month, int exp_year, String number, bool cacheData = true)
		{
			Stripe.StripeClient.DefaultPublishableKey = stripeKey;

			try {
				Card c = new Card();
				c.Name = name;
				c.AddressLine1 = address_line1;
				c.AddressLine2 = "";
				c.AddressCity = address_city;
				c.AddressState = address_state;
				c.AddressZip = address_zip;
				c.AddressCountry = address_country;
				c.ExpiryMonth = exp_month;
				c.ExpiryYear = exp_year;
				c.CVC = cvc;
				c.Number = number;

				var token = await StripeClient.CreateToken (c, stripeKey);

				if (token != null) {
					//var token1 = await StripeClient.RequestToken(token.Id, "pk_test_56MhdYupqBRJO4QJbKMWFPht");
					//TODO: Send token to your server to process a payment with
				/*	StripeTokenResult tokenResult = new StripeTokenResult();
					tokenResult.id = token.Id;
					tokenResult.created = token.Created.ToFileTimeUtc();
					tokenResult.livemode = token.LiveMode;
					tokenResult.used = token.Used;

					tokenResult.card = new CardObject();
					tokenResult.card.last4 = token.Card.Last4;
					//tokenResult.card.brand = (token.Card.Type == null) ? "" : token.Card.Type.ToString();
					tokenResult.card.exp_month = token.Card.ExpiryMonth;
					tokenResult.card.exp_year = token.Card.ExpiryYear;
					tokenResult.card.fingerprint = token.Card.Fingerprint;
					tokenResult.card.country = token.Card.Country;
					tokenResult.card.name = token.Card.Name;
					tokenResult.card.address_line1 = token.Card.AddressLine1;
					tokenResult.card.address_line2 = token.Card.AddressLine2;
					tokenResult.card.address_city = token.Card.AddressCity;
					tokenResult.card.address_state = token.Card.AddressState;
					tokenResult.card.address_zip = token.Card.AddressZip;
					tokenResult.card.address_country = token.Card.AddressCountry;
*/
					//String result = tokenResult.ToJson<StripeTokenResult>();
					return token.Id;

				} else {

				}
			} catch (Exception ex) {
				Insights.Report (ex);
				Console.WriteLine (ex.ToString ());
			}

			return "";
		}

		public async static Task<OrderHistoryListResult> GetOrderHistory (string user_id, string user_token, int pageNumber, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				OrderHistoryListResult result = await Task.Run (() => {

					var request = new OrderHistoryRequest {
						user_id = user_id,
						user_token = user_token,
						page = pageNumber.ToString()
					};

					var url = Config.ApiUrl + "/" + Config.API_GET_ORDER_HISTORY;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_token", request.user_token },
						{ "page", request.page }
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var response = parseResponse.FromJson<OrderHistoryListResult> ();

					return response;
					// Post request to server and then convert response to object from JSON returned
					//return url.PostToUrl (request).FromJson<OrderHistoryListResult> ();

				});

				return result;

			} catch (Exception e) {
				// Track in Google Analytics
				//				GoogleAnalyticsTracker.TrackExceptionInGoogleAnalytics (e, false);

				Console.WriteLine (e);
				var response = new OrderHistoryListResult {
					result = new OrderHistoryResult{
						status = "error",
						message = AppText.CommunicationError,
					}
				};

				return response;
			}
		}

		public async static Task<SimpleResult> RemoveUserOrder (string user_id, string user_token, string order_id, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				SimpleResult result = await Task.Run (() => {

					var request = new OrderRemoveRequest {
						user_id = user_id,
						user_token = user_token,
						order_id = order_id,
					};

					var url = Config.ApiUrl + "/" + Config.API_ORDER_DELETE_USER;

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_token", request.user_token },
						{ "order_id", request.order_id }
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var response = parseResponse.FromJson<SimpleResult> ();

					return response;
					// Post request to server and then convert response to object from JSON returned
					//return url.PostToUrl (request).FromJson<SimpleResult> ();

				});

				return result;

			} catch (Exception e) {
				// Track in Google Analytics
				//				GoogleAnalyticsTracker.TrackExceptionInGoogleAnalytics (e, false);

				Console.WriteLine (e);
				var response = new SimpleResult {
					result = new BaseResult{
						status = "error",
						message = AppText.CommunicationError,
					}
				};

				return response;
			}
		}

		public async static Task<OrderDetailResult> GetOrderDetail (string order_id, string user_id, string user_token, bool cacheData = true)
		{
			try {
				// Run request asynchronously
				OrderDetailResult result = await Task.Run (() => {

					var request = new OrderDetailRequest {
						user_id = user_id,
						user_token = user_token,
						order_id = order_id,
					};

					var url = Config.ApiUrl + "/" + Config.API_ORDER_DETAILS;

					// Post request to server and then convert response to object from JSON returned
					//JsonObject obj = new JsonObject();

					WebClient client = new WebClient ();

					var requestData = new NameValueCollection() {
						{ "user_id", request.user_id },
						{ "user_token", request.user_token },
						{ "order_id", request.order_id }
					};

					client.Headers.Add(Globals.Config.AccessKeyName, Globals.Config.AccessKey);
					var parseResponse = client.UploadValues (url, "POST", requestData).AsString();
					var a = parseResponse.FromJson<JsonObject> ();

					//var a = url.PostToUrl (request).FromJson<JsonObject>();
					var b = a.Get<JsonObject>("result");

					OrderDetailResult orderResult = new OrderDetailResult();
					orderResult.result = new OrderDetailsResult();
					orderResult.result.status = b.Get("status");
					orderResult.result.order_details = new OrderDetails();

					var c = b.Get<JsonObject>("order_details");

					orderResult.result.order_details.order_info = c.Get<OrderDetailInfo>("order_info");
					orderResult.result.order_details.order_items = c.Get<List<OrderItem>>("order_items");

					return orderResult;

					//return url.PostToUrl (request).FromJson<OrderDetailResult> ();

				});

				return result;

			} catch (Exception e) {
				// Track in Google Analytics
				//				GoogleAnalyticsTracker.TrackExceptionInGoogleAnalytics (e, false);

				Console.WriteLine (e);
				var response = new OrderDetailResult {
					result = new OrderDetailsResult{
						status = "error",
						message = AppText.CommunicationError,
					}
				};

				return response;
			}
		}

	}
}

