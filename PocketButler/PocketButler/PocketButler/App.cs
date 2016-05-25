using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using Xamarin;
using Com.Mixpanel.Android.Mpmetrics;

namespace PocketButler
{
    public static class App
    {
		public static IPageLoader PageLoaderManager;

		public static NavigationPage _NavigationPage;
		public static LocalDBManager _DbManager;

		public static double UserLatitude;
		public static double UserLongitude;
		public static bool IsWindowAdjustResize;

		public delegate void OnPageStartedDelegate();
		public static OnPageStartedDelegate PageStarted = null;

		public static Assembly _reflectionAssembly;
		internal static readonly MethodInfo GetDependency;

		public static bool IsLoggedInWithFacebook {
			get { return !string.IsNullOrWhiteSpace(_FacebookUserToken); }
		}

		public static string _FacebookUserToken;
		public static string FacebookUserToken {
			get { return _FacebookUserToken; }
		}

		public delegate Task<bool> PageBackPressedDelegate ();
		public static PageBackPressedDelegate BackButtonPressedEvent = null;
		//public static Action RootPagePageShowingEvent = null;

		static App()
		{
			String dbPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments) + "/" + Globals.Config.DATABASE_FILENAME;
			if (File.Exists (dbPath))
				File.Delete (dbPath);

			_DbManager = new LocalDBManager (dbPath);
			Globals.Config.PaymentInfo = new PaymentInfoViewModel ();

			GetDependency = typeof(DependencyService)
				.GetRuntimeMethods()
				.Single((method)=>
					method.Name.Equals("Get"));
		}

		public static void Init(Assembly assembly)
		{
			System.Threading.Interlocked.CompareExchange(ref _reflectionAssembly, assembly, null);
		}

		public static Stream LoadResource(String name)
		{
			return _reflectionAssembly.GetManifestResourceStream(name);
		}

		public static Page GetMainPage(IPageLoader pageLoaderManager)
        {
			Utils.SaveDataToSettings ("user_last_logged", "1");
			PageLoaderManager = pageLoaderManager;
			return new MainPage ();
        }

		public static Page GetTypeSliderPage(IPageLoader pageLoaderManager, RestaurantInfo info)
		{
			PageLoaderManager = pageLoaderManager;
			var sliderPage = new VenueTypeSliderPage (info, null);
			var mainPage = new MainPage (sliderPage);
			sliderPage.MasterPage = mainPage;
			sliderPage.IsDisableBackButton = true;
			return mainPage;
		}

		public static Page GetLoginPage(IPageLoader pageLoaderManager)
		{
			PageLoaderManager = pageLoaderManager;
			return new LoginPage ();
		}

		public static Page GetLoginPage(IPageLoader pageLoaderManager, string email = "", string firstName = "", string lastName = "", DateTime? dob = null, string facebookId = "")
		{
			if (pageLoaderManager == null) {
				return new LoginPage (email, firstName, lastName, dob, facebookId);
			}

			PageLoaderManager = pageLoaderManager;
			return new LoginPage ();
		}

		public static void LoadUserDataFromSetting()
		{
			bool isRemember = (Utils.LoadDataFromSettings ("remember_password") == "1");
			if (isRemember == true) {
				Globals.Config.LOGIN_TIME = DateTime.Now;
				Globals.Config.USER_INFO = new Userinfo();
				Globals.Config.USER_INFO.contact_address = new ContactAddress ();

				// User Info
				Globals.Config.USER_INFO.user_dob = Utils.LoadDataFromSettings("user_info_dob");
				Globals.Config.USER_INFO.user_email = Utils.LoadDataFromSettings("user_info_email");
				Globals.Config.USER_INFO.user_first_name = Utils.LoadDataFromSettings("user_info_first_name");
				Globals.Config.USER_INFO.user_id = Utils.LoadDataFromSettings("user_info_id");
				Globals.Config.USER_INFO.user_image = Utils.LoadDataFromSettings("user_info_image");
				Globals.Config.USER_INFO.user_last_name = Utils.LoadDataFromSettings("user_info_last_name");
				Globals.Config.USER_INFO.user_password = Utils.LoadDataFromSettings("user_info_password");
				Globals.Config.USER_INFO.user_token = Utils.LoadDataFromSettings("user_info_token");

				if (String.IsNullOrEmpty (Globals.Config.USER_INFO.user_id) ||
					String.IsNullOrEmpty (Globals.Config.USER_INFO.user_token)) {
					return;
				}

				// Contact Address
				Globals.Config.USER_INFO.contact_address.city_name = Utils.LoadDataFromSettings("user_contact_city_name");
				Globals.Config.USER_INFO.contact_address.country_name = Utils.LoadDataFromSettings("user_contact_country_name");
				Globals.Config.USER_INFO.contact_address.postal_code = Utils.LoadDataFromSettings("user_contact_postal_code");
				Globals.Config.USER_INFO.contact_address.state_name = Utils.LoadDataFromSettings("user_contact_state_name");
				Globals.Config.USER_INFO.contact_address.street_name = Utils.LoadDataFromSettings("user_contact_street_name");
			}

			var userInfo = new Dictionary<string, string> {
				{Insights.Traits.Email, (Globals.Config.USER_INFO != null) ? Globals.Config.USER_INFO.user_email : ""},
				{"Last Login", (Globals.Config.LOGIN_TIME != null) ? Globals.Config.LOGIN_TIME.ToString() : ""},
			};
			Insights.Identify ((Globals.Config.USER_INFO != null) ? Globals.Config.USER_INFO.user_id : "", userInfo);

			if (Globals.Config.IsMixPanel_Enabled) {
				MixpanelAPI _mixpanel = MixpanelAPI.GetInstance (Forms.Context, Globals.Config.MixPanelKey);
				_mixpanel.Identify ((Globals.Config.USER_INFO != null) ? Globals.Config.USER_INFO.user_email : "");
				_mixpanel.Track ("Landing", null);
				_mixpanel.People.Increment ("App Opens", 1);
			}
		}

		public static Page GetFirstShowPage(IPageLoader pageLoaderManager)
		{
			PageLoaderManager = pageLoaderManager;
			bool isRemember = (Utils.LoadDataFromSettings ("remember_password") == "1");
			if (isRemember == true) {
				if (String.IsNullOrEmpty (Globals.Config.USER_INFO.user_id) ||
				    String.IsNullOrEmpty (Globals.Config.USER_INFO.user_token)) {
					return new LoginPage();
				}

				String lastLoggedIn = Utils.LoadDataFromSettings ("user_last_logged");
				if (String.IsNullOrEmpty(lastLoggedIn) ||
					lastLoggedIn.Equals("0"))
					return new LoginPage();

				App.IsWindowAdjustResize = false;

				return new MainPage();
			}

			return new LoginPage();
		}

		public static void SaveFacebookToken(string token)
		{
			_FacebookUserToken = token;
		}

		public static Action SuccessfulLoginAction
		{
			get {
				return new Action (async () => {
					App.PageLoaderManager.ShowMainPage ();
				});
			}
		}

		public static Page ShowSplashPage(IPageLoader pageLoaderManager)
		{
			PageLoaderManager = pageLoaderManager;
			return new SplashPage ();
		}
    }
}
